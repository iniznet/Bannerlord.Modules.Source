using System;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000137 RID: 311
	public class WedgeFormation : LineFormation
	{
		// Token: 0x06000F9E RID: 3998 RVA: 0x0002EE50 File Offset: 0x0002D050
		public WedgeFormation(IFormation owner)
			: base(owner, true)
		{
		}

		// Token: 0x06000F9F RID: 3999 RVA: 0x0002EE5A File Offset: 0x0002D05A
		public override IFormationArrangement Clone(IFormation formation)
		{
			return new WedgeFormation(formation);
		}

		// Token: 0x06000FA0 RID: 4000 RVA: 0x0002EE64 File Offset: 0x0002D064
		private int GetUnitCountOfRank(int rankIndex)
		{
			int num = rankIndex * 2 * 3 + 3;
			return MathF.Min(base.FileCount, num);
		}

		// Token: 0x06000FA1 RID: 4001 RVA: 0x0002EE88 File Offset: 0x0002D088
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

		// Token: 0x06000FA2 RID: 4002 RVA: 0x0002EEC4 File Offset: 0x0002D0C4
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
