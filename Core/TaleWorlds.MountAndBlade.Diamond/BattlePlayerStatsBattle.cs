using System;

namespace TaleWorlds.MountAndBlade.Diamond
{
	[Serializable]
	public class BattlePlayerStatsBattle : BattlePlayerStatsBase
	{
		public BattlePlayerStatsBattle()
		{
			base.GameType = "Battle";
		}

		public int RoundsWon { get; set; }

		public int RoundsLost { get; set; }
	}
}
