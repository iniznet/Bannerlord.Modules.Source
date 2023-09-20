using System;

namespace TaleWorlds.MountAndBlade.Diamond
{
	[Serializable]
	public class BattlePlayerStatsBase
	{
		public int Kills { get; set; }

		public int Assists { get; set; }

		public int Deaths { get; set; }

		public int PlayTime { get; set; }
	}
}
