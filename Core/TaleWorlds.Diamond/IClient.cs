using System;
using System.Threading.Tasks;

namespace TaleWorlds.Diamond
{
	// Token: 0x0200000A RID: 10
	public interface IClient
	{
		// Token: 0x17000009 RID: 9
		// (get) Token: 0x0600002F RID: 47
		bool IsInCriticalState { get; }

		// Token: 0x1700000A RID: 10
		// (get) Token: 0x06000030 RID: 48
		long AliveCheckTimeInMiliSeconds { get; }

		// Token: 0x06000031 RID: 49
		void HandleMessage(Message message);

		// Token: 0x06000032 RID: 50
		void OnConnected();

		// Token: 0x06000033 RID: 51
		void OnCantConnect();

		// Token: 0x06000034 RID: 52
		void OnDisconnected();

		// Token: 0x06000035 RID: 53
		Task<bool> CheckConnection();

		// Token: 0x1700000B RID: 11
		// (get) Token: 0x06000036 RID: 54
		ILoginAccessProvider AccessProvider { get; }
	}
}
