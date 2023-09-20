using System;
using TaleWorlds.Diamond;
using TaleWorlds.MountAndBlade.Diamond;

namespace Messages.FromLobbyServer.ToClient
{
	// Token: 0x02000031 RID: 49
	[Serializable]
	public class GetRankedLeaderboardMessageResult : FunctionResult
	{
		// Token: 0x1700003C RID: 60
		// (get) Token: 0x060000A1 RID: 161 RVA: 0x00002715 File Offset: 0x00000915
		// (set) Token: 0x060000A2 RID: 162 RVA: 0x0000271D File Offset: 0x0000091D
		public PlayerLeaderboardData[] LeaderboardPlayers { get; private set; }

		// Token: 0x060000A3 RID: 163 RVA: 0x00002726 File Offset: 0x00000926
		public GetRankedLeaderboardMessageResult(PlayerLeaderboardData[] leaderboardPlayers)
		{
			this.LeaderboardPlayers = leaderboardPlayers;
		}
	}
}
