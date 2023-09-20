using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Newtonsoft.Json;
using TaleWorlds.Library.Http;

namespace TaleWorlds.Diamond.Rest
{
	public class SessionlessClientRestDriver : ISessionlessClientDriver
	{
		public SessionlessClientRestDriver(string address, ushort port, bool isSecure, IHttpDriver platformNetworkClient)
		{
			this._isSecure = isSecure;
			this._platformNetworkClient = platformNetworkClient;
			this._address = address;
			this._port = port;
			this._restDataJsonConverter = new RestDataJsonConverter();
		}

		private void AssignRequestJob(SessionlessClientRestSessionTask requestMessageTask)
		{
			requestMessageTask.SetRequestData(this._address, this._port, this._isSecure, this._platformNetworkClient);
		}

		private void TickTask(SessionlessClientRestSessionTask messageTask)
		{
			messageTask.Tick();
			IHttpRequestTask request = messageTask.Request;
			if (request.State == HttpRequestTaskState.Finished)
			{
				if (request.Successful)
				{
					string responseData = messageTask.Request.ResponseData;
					if (string.IsNullOrEmpty(responseData))
					{
						messageTask.SetFinishedAsFailed();
						return;
					}
					SessionlessRestResponse sessionlessRestResponse = JsonConvert.DeserializeObject<SessionlessRestResponse>(responseData, new JsonConverter[] { this._restDataJsonConverter });
					if (sessionlessRestResponse.Successful)
					{
						messageTask.SetFinishedAsSuccessful(sessionlessRestResponse);
						return;
					}
					messageTask.SetFinishedAsFailed(sessionlessRestResponse);
					return;
				}
				else
				{
					messageTask.SetFinishedAsFailed();
				}
			}
		}

		private void SendMessage(SessionlessRestRequestMessage message)
		{
			SessionlessClientRestDriver.<>c__DisplayClass8_0 CS$<>8__locals1 = new SessionlessClientRestDriver.<>c__DisplayClass8_0();
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.clientRestSessionTask = new SessionlessClientRestSessionTask(message);
			this.AssignRequestJob(CS$<>8__locals1.clientRestSessionTask);
			Task.Run(delegate
			{
				SessionlessClientRestDriver.<>c__DisplayClass8_0.<<SendMessage>b__0>d <<SendMessage>b__0>d;
				<<SendMessage>b__0>d.<>4__this = CS$<>8__locals1;
				<<SendMessage>b__0>d.<>t__builder = AsyncTaskMethodBuilder.Create();
				<<SendMessage>b__0>d.<>1__state = -1;
				AsyncTaskMethodBuilder <>t__builder = <<SendMessage>b__0>d.<>t__builder;
				<>t__builder.Start<SessionlessClientRestDriver.<>c__DisplayClass8_0.<<SendMessage>b__0>d>(ref <<SendMessage>b__0>d);
				return <<SendMessage>b__0>d.<>t__builder.Task;
			});
		}

		void ISessionlessClientDriver.SendMessage(Message message)
		{
			this.SendMessage(new SessionlessRestDataRequestMessage(message, MessageType.Message));
		}

		async Task<TResult> ISessionlessClientDriver.CallFunction<TResult>(Message message)
		{
			SessionlessClientRestDriver.<>c__DisplayClass10_0<TResult> CS$<>8__locals1 = new SessionlessClientRestDriver.<>c__DisplayClass10_0<TResult>();
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.clientRestSessionTask = new SessionlessClientRestSessionTask(new SessionlessRestDataRequestMessage(message, MessageType.Function));
			this.AssignRequestJob(CS$<>8__locals1.clientRestSessionTask);
			await Task.Run(delegate
			{
				SessionlessClientRestDriver.<>c__DisplayClass10_0<TResult>.<<TaleWorlds-Diamond-ISessionlessClientDriver-CallFunction>b__0>d <<TaleWorlds-Diamond-ISessionlessClientDriver-CallFunction>b__0>d;
				<<TaleWorlds-Diamond-ISessionlessClientDriver-CallFunction>b__0>d.<>4__this = CS$<>8__locals1;
				<<TaleWorlds-Diamond-ISessionlessClientDriver-CallFunction>b__0>d.<>t__builder = AsyncTaskMethodBuilder.Create();
				<<TaleWorlds-Diamond-ISessionlessClientDriver-CallFunction>b__0>d.<>1__state = -1;
				AsyncTaskMethodBuilder <>t__builder = <<TaleWorlds-Diamond-ISessionlessClientDriver-CallFunction>b__0>d.<>t__builder;
				<>t__builder.Start<SessionlessClientRestDriver.<>c__DisplayClass10_0<TResult>.<<TaleWorlds-Diamond-ISessionlessClientDriver-CallFunction>b__0>d>(ref <<TaleWorlds-Diamond-ISessionlessClientDriver-CallFunction>b__0>d);
				return <<TaleWorlds-Diamond-ISessionlessClientDriver-CallFunction>b__0>d.<>t__builder.Task;
			});
			if (CS$<>8__locals1.clientRestSessionTask.Successful)
			{
				return (TResult)((object)CS$<>8__locals1.clientRestSessionTask.RestResponse.FunctionResult.GetFunctionResult());
			}
			throw new Exception("Could not call function with " + message.GetType().Name);
		}

		async Task<bool> ISessionlessClientDriver.CheckConnection()
		{
			bool flag;
			try
			{
				string text = "http://";
				if (this._isSecure)
				{
					text = "https://";
				}
				string text2 = string.Concat(new object[] { text, this._address, ":", this._port, "/Data/Ping" });
				await this._platformNetworkClient.HttpGetString(text2, false);
				flag = true;
			}
			catch
			{
				flag = false;
			}
			return flag;
		}

		private readonly string _address;

		private readonly ushort _port;

		private RestDataJsonConverter _restDataJsonConverter;

		private bool _isSecure;

		private IHttpDriver _platformNetworkClient;
	}
}
