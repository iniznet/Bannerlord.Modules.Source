using System;
using TaleWorlds.Diamond;
using TaleWorlds.MountAndBlade.Diamond;

namespace Messages.FromLobbyServer.ToClient
{
	[Serializable]
	public class GetPlayerStatsMessageResult : FunctionResult
	{
		public PlayerStatsBase[] PlayerStats { get; private set; }

		public GetPlayerStatsMessageResult(PlayerStatsBase[] playerStats)
		{
			this.PlayerStats = playerStats;
		}
	}
}
