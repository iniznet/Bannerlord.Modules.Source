using System;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000134 RID: 308
	public class RectilinearSchiltronFormation : SquareFormation
	{
		// Token: 0x06000F75 RID: 3957 RVA: 0x0002E3E7 File Offset: 0x0002C5E7
		public RectilinearSchiltronFormation(IFormation owner)
			: base(owner)
		{
		}

		// Token: 0x06000F76 RID: 3958 RVA: 0x0002E3F0 File Offset: 0x0002C5F0
		public override IFormationArrangement Clone(IFormation formation)
		{
			return new RectilinearSchiltronFormation(formation);
		}

		// Token: 0x17000370 RID: 880
		// (get) Token: 0x06000F77 RID: 3959 RVA: 0x0002E3F8 File Offset: 0x0002C5F8
		public override float MaximumWidth
		{
			get
			{
				int num;
				int maximumRankCount = SquareFormation.GetMaximumRankCount(base.GetUnitCountWithOverride(), out num);
				return SquareFormation.GetSideWidthFromUnitCount(base.GetUnitsPerSideFromRankCount(maximumRankCount), this.owner.MaximumInterval, base.UnitDiameter);
			}
		}

		// Token: 0x06000F78 RID: 3960 RVA: 0x0002E430 File Offset: 0x0002C630
		public void Form()
		{
			int unitCountWithOverride = base.GetUnitCountWithOverride();
			int num;
			int maximumRankCount = SquareFormation.GetMaximumRankCount(unitCountWithOverride, out num);
			if (unitCountWithOverride <= num * (num - 1))
			{
				base.DisableRearOfLastRank = true;
			}
			else
			{
				base.DisableRearOfLastRank = false;
			}
			base.FormFromRankCount(maximumRankCount);
		}
	}
}
