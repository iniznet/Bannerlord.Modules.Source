using System;
using TaleWorlds.Diamond;
using TaleWorlds.MountAndBlade.Diamond;

namespace Messages.FromLobbyServer.ToClient
{
	// Token: 0x02000023 RID: 35
	[Serializable]
	public class GetAverageMatchmakingWaitTimesResult : FunctionResult
	{
		// Token: 0x1700002E RID: 46
		// (get) Token: 0x0600007A RID: 122 RVA: 0x00002570 File Offset: 0x00000770
		// (set) Token: 0x0600007B RID: 123 RVA: 0x00002578 File Offset: 0x00000778
		public MatchmakingWaitTimeStats MatchmakingWaitTimeStats { get; private set; }

		// Token: 0x0600007C RID: 124 RVA: 0x00002581 File Offset: 0x00000781
		public GetAverageMatchmakingWaitTimesResult(MatchmakingWaitTimeStats matchmakingWaitTimeStats)
		{
			this.MatchmakingWaitTimeStats = matchmakingWaitTimeStats;
		}
	}
}
