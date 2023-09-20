using System;
using TaleWorlds.Diamond;
using TaleWorlds.MountAndBlade.Diamond;

namespace Messages.FromLobbyServer.ToClient
{
	// Token: 0x02000026 RID: 38
	[Serializable]
	public class GetClanLeaderboardResult : FunctionResult
	{
		// Token: 0x17000031 RID: 49
		// (get) Token: 0x06000082 RID: 130 RVA: 0x000025C7 File Offset: 0x000007C7
		// (set) Token: 0x06000083 RID: 131 RVA: 0x000025CF File Offset: 0x000007CF
		public ClanLeaderboardInfo ClanLeaderboardInfo { get; private set; }

		// Token: 0x06000084 RID: 132 RVA: 0x000025D8 File Offset: 0x000007D8
		public GetClanLeaderboardResult(ClanLeaderboardInfo info)
		{
			this.ClanLeaderboardInfo = info;
		}
	}
}
