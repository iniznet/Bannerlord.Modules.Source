using System;
using TaleWorlds.Diamond;
using TaleWorlds.MountAndBlade.Diamond;

namespace Messages.FromLobbyServer.ToClient
{
	[Serializable]
	public class GetPlayerCountInQueueResult : FunctionResult
	{
		public MatchmakingQueueStats MatchmakingQueueStats { get; private set; }

		public GetPlayerCountInQueueResult(MatchmakingQueueStats matchmakingQueueStats)
		{
			this.MatchmakingQueueStats = matchmakingQueueStats;
		}
	}
}
