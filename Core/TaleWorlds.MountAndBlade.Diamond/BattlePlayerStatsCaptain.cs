using System;

namespace TaleWorlds.MountAndBlade.Diamond
{
	[Serializable]
	public class BattlePlayerStatsCaptain : BattlePlayerStatsBase
	{
		public BattlePlayerStatsCaptain()
		{
			base.GameType = "Captain";
		}

		public int CaptainsKilled { get; set; }

		public int MVPs { get; set; }

		public int Score { get; set; }
	}
}
