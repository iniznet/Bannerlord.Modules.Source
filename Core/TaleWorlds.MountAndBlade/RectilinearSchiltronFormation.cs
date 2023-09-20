﻿using System;

namespace TaleWorlds.MountAndBlade
{
	public class RectilinearSchiltronFormation : SquareFormation
	{
		public RectilinearSchiltronFormation(IFormation owner)
			: base(owner)
		{
		}

		public override IFormationArrangement Clone(IFormation formation)
		{
			return new RectilinearSchiltronFormation(formation);
		}

		public override float MaximumWidth
		{
			get
			{
				int num;
				int maximumRankCount = SquareFormation.GetMaximumRankCount(base.GetUnitCountWithOverride(), out num);
				return SquareFormation.GetSideWidthFromUnitCount(base.GetUnitsPerSideFromRankCount(maximumRankCount), this.owner.MaximumInterval, base.UnitDiameter);
			}
		}

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
