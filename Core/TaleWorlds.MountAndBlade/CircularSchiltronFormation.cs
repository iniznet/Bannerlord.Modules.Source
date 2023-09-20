using System;

namespace TaleWorlds.MountAndBlade
{
	public class CircularSchiltronFormation : CircularFormation
	{
		public CircularSchiltronFormation(IFormation owner)
			: base(owner)
		{
		}

		public override IFormationArrangement Clone(IFormation formation)
		{
			return new CircularSchiltronFormation(formation);
		}

		public override float MaximumWidth
		{
			get
			{
				int unitCountWithOverride = base.GetUnitCountWithOverride();
				int currentMaximumRankCount = base.GetCurrentMaximumRankCount(unitCountWithOverride);
				float num = this.owner.MaximumInterval + base.UnitDiameter;
				float num2 = this.owner.MaximumDistance + base.UnitDiameter;
				return base.GetCircumferenceAux(unitCountWithOverride, currentMaximumRankCount, num, num2) / 3.1415927f;
			}
		}

		public void Form()
		{
			int unitCountWithOverride = base.GetUnitCountWithOverride();
			int currentMaximumRankCount = base.GetCurrentMaximumRankCount(unitCountWithOverride);
			float circumferenceFromRankCount = base.GetCircumferenceFromRankCount(currentMaximumRankCount);
			base.FormFromCircumference(circumferenceFromRankCount);
		}
	}
}
