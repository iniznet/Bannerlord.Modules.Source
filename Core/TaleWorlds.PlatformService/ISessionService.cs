using System;

namespace TaleWorlds.PlatformService
{
	// Token: 0x0200000E RID: 14
	public interface ISessionService
	{
		// Token: 0x0600004F RID: 79
		void OnJoinJoinableSession(string connectionString);

		// Token: 0x06000050 RID: 80
		void OnLeaveJoinableSession();
	}
}
