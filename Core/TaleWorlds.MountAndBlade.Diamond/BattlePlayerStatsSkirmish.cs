using System;

namespace TaleWorlds.MountAndBlade.Diamond
{
	[Serializable]
	public class BattlePlayerStatsSkirmish : BattlePlayerStatsBase
	{
		public BattlePlayerStatsSkirmish()
		{
			base.GameType = "Skirmish";
		}

		public int MVPs { get; set; }

		public int Score { get; set; }
	}
}
