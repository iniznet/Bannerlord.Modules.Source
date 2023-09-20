using System;
using System.Threading.Tasks;
using TaleWorlds.Diamond.ClientApplication;

namespace TaleWorlds.Diamond
{
	public abstract class SessionlessClient<T> : DiamondClientApplicationObject, ISessionlessClient where T : SessionlessClient<T>
	{
		protected SessionlessClient(DiamondClientApplication diamondClientApplication, ISessionlessClientDriverProvider<T> driverProvider)
			: base(diamondClientApplication)
		{
			this._clientDriver = driverProvider.CreateDriver((T)((object)this));
		}

		protected void SendMessage(Message message)
		{
			this._clientDriver.SendMessage(message);
		}

		protected async Task<TResult> CallFunction<TResult>(Message message) where TResult : FunctionResult
		{
			return await this._clientDriver.CallFunction<TResult>(message);
		}

		public Task<bool> CheckConnection()
		{
			return this._clientDriver.CheckConnection();
		}

		private ISessionlessClientDriver _clientDriver;
	}
}
