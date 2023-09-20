using System;
using TaleWorlds.Diamond;
using TaleWorlds.MountAndBlade.Diamond;

namespace Messages.FromLobbyServer.ToClient
{
	// Token: 0x0200002D RID: 45
	[Serializable]
	public class GetPlayerCountInQueueResult : FunctionResult
	{
		// Token: 0x17000038 RID: 56
		// (get) Token: 0x06000095 RID: 149 RVA: 0x00002695 File Offset: 0x00000895
		// (set) Token: 0x06000096 RID: 150 RVA: 0x0000269D File Offset: 0x0000089D
		public MatchmakingQueueStats MatchmakingQueueStats { get; private set; }

		// Token: 0x06000097 RID: 151 RVA: 0x000026A6 File Offset: 0x000008A6
		public GetPlayerCountInQueueResult(MatchmakingQueueStats matchmakingQueueStats)
		{
			this.MatchmakingQueueStats = matchmakingQueueStats;
		}
	}
}
