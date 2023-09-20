using System;

namespace TaleWorlds.MountAndBlade.Diamond
{
	[Serializable]
	public class BattlePlayerStatsTeamDeathmatch : BattlePlayerStatsBase
	{
		public BattlePlayerStatsTeamDeathmatch()
		{
			base.GameType = "TeamDeathmatch";
		}

		public int Score { get; set; }
	}
}
