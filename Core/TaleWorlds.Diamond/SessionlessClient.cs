using System;
using System.Threading.Tasks;
using TaleWorlds.Diamond.ClientApplication;

namespace TaleWorlds.Diamond
{
	// Token: 0x0200001E RID: 30
	public abstract class SessionlessClient<T> : DiamondClientApplicationObject, ISessionlessClient where T : SessionlessClient<T>
	{
		// Token: 0x0600008A RID: 138 RVA: 0x00002C81 File Offset: 0x00000E81
		protected SessionlessClient(DiamondClientApplication diamondClientApplication, ISessionlessClientDriverProvider<T> driverProvider)
			: base(diamondClientApplication)
		{
			this._clientDriver = driverProvider.CreateDriver((T)((object)this));
		}

		// Token: 0x0600008B RID: 139 RVA: 0x00002C9C File Offset: 0x00000E9C
		protected void SendMessage(Message message)
		{
			this._clientDriver.SendMessage(message);
		}

		// Token: 0x0600008C RID: 140 RVA: 0x00002CAC File Offset: 0x00000EAC
		protected async Task<TResult> CallFunction<TResult>(Message message) where TResult : FunctionResult
		{
			return await this._clientDriver.CallFunction<TResult>(message);
		}

		// Token: 0x0600008D RID: 141 RVA: 0x00002CF9 File Offset: 0x00000EF9
		public Task<bool> CheckConnection()
		{
			return this._clientDriver.CheckConnection();
		}

		// Token: 0x04000024 RID: 36
		private ISessionlessClientDriver _clientDriver;
	}
}
