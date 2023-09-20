using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Newtonsoft.Json;
using TaleWorlds.Library.Http;

namespace TaleWorlds.Diamond.Rest
{
	// Token: 0x02000037 RID: 55
	public class SessionlessClientRestDriver : ISessionlessClientDriver
	{
		// Token: 0x06000109 RID: 265 RVA: 0x0000391D File Offset: 0x00001B1D
		public SessionlessClientRestDriver(string address, ushort port, bool isSecure, IHttpDriver platformNetworkClient)
		{
			this._isSecure = isSecure;
			this._platformNetworkClient = platformNetworkClient;
			this._address = address;
			this._port = port;
			this._restDataJsonConverter = new RestDataJsonConverter();
		}

		// Token: 0x0600010A RID: 266 RVA: 0x0000394D File Offset: 0x00001B4D
		private void AssignRequestJob(SessionlessClientRestSessionTask requestMessageTask)
		{
			requestMessageTask.SetRequestData(this._address, this._port, this._isSecure, this._platformNetworkClient);
		}

		// Token: 0x0600010B RID: 267 RVA: 0x00003970 File Offset: 0x00001B70
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

		// Token: 0x0600010C RID: 268 RVA: 0x000039EC File Offset: 0x00001BEC
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

		// Token: 0x0600010D RID: 269 RVA: 0x00003A30 File Offset: 0x00001C30
		void ISessionlessClientDriver.SendMessage(Message message)
		{
			this.SendMessage(new SessionlessRestDataRequestMessage(message, MessageType.Message));
		}

		// Token: 0x0600010E RID: 270 RVA: 0x00003A40 File Offset: 0x00001C40
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

		// Token: 0x0600010F RID: 271 RVA: 0x00003A90 File Offset: 0x00001C90
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

		// Token: 0x04000046 RID: 70
		private readonly string _address;

		// Token: 0x04000047 RID: 71
		private readonly ushort _port;

		// Token: 0x04000048 RID: 72
		private RestDataJsonConverter _restDataJsonConverter;

		// Token: 0x04000049 RID: 73
		private bool _isSecure;

		// Token: 0x0400004A RID: 74
		private IHttpDriver _platformNetworkClient;
	}
}
