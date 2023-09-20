﻿using System;

namespace TaleWorlds.MountAndBlade.Diamond
{
	[Serializable]
	public class BattlePlayerStatsSiege : BattlePlayerStatsBase
	{
		public BattlePlayerStatsSiege()
		{
			base.GameType = "Siege";
		}

		public int WallsBreached { get; set; }

		public int SiegeEngineKills { get; set; }

		public int SiegeEnginesDestroyed { get; set; }

		public int ObjectiveGoldGained { get; set; }

		public int Score { get; set; }
	}
}
