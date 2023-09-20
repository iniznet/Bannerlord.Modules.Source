using System;
using TaleWorlds.Diamond;
using TaleWorlds.MountAndBlade.Diamond;

namespace Messages.FromLobbyServer.ToClient
{
	[Serializable]
	public class GetAverageMatchmakingWaitTimesResult : FunctionResult
	{
		public MatchmakingWaitTimeStats MatchmakingWaitTimeStats { get; private set; }

		public GetAverageMatchmakingWaitTimesResult(MatchmakingWaitTimeStats matchmakingWaitTimeStats)
		{
			this.MatchmakingWaitTimeStats = matchmakingWaitTimeStats;
		}
	}
}
