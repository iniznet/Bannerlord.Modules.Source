using System;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	public class WedgeFormation : LineFormation
	{
		public WedgeFormation(IFormation owner)
			: base(owner, true)
		{
		}

		public override IFormationArrangement Clone(IFormation formation)
		{
			return new WedgeFormation(formation);
		}

		private int GetUnitCountOfRank(int rankIndex)
		{
			int num = rankIndex * 2 * 3 + 3;
			return MathF.Min(base.FileCount, num);
		}

		protected override bool IsUnitPositionRestrained(int fileIndex, int rankIndex)
		{
			if (base.IsUnitPositionRestrained(fileIndex, rankIndex))
			{
				return true;
			}
			int unitCountOfRank = this.GetUnitCountOfRank(rankIndex);
			int num = (base.FileCount - unitCountOfRank) / 2;
			return fileIndex < num || fileIndex >= num + unitCountOfRank;
		}

		protected override void MakeRestrainedPositionsUnavailable()
		{
			for (int i = 0; i < base.FileCount; i++)
			{
				for (int j = 0; j < base.RankCount; j++)
				{
					if (this.IsUnitPositionRestrained(i, j))
					{
						this.UnitPositionAvailabilities[i, j] = 1;
					}
				}
			}
		}
	}
}
