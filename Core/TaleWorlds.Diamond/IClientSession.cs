using System;
using System.Threading.Tasks;

namespace TaleWorlds.Diamond
{
	// Token: 0x0200000B RID: 11
	public interface IClientSession
	{
		// Token: 0x06000037 RID: 55
		void Connect();

		// Token: 0x06000038 RID: 56
		void Disconnect();

		// Token: 0x06000039 RID: 57
		void Tick();

		// Token: 0x0600003A RID: 58
		Task<LoginResult> Login(LoginMessage message);

		// Token: 0x0600003B RID: 59
		void SendMessage(Message message);

		// Token: 0x0600003C RID: 60
		Task<T> CallFunction<T>(Message message) where T : FunctionResult;

		// Token: 0x0600003D RID: 61
		Task<bool> CheckConnection();
	}
}
