using System;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x0200012B RID: 299
	public class CircularSchiltronFormation : CircularFormation
	{
		// Token: 0x06000E06 RID: 3590 RVA: 0x00027F33 File Offset: 0x00026133
		public CircularSchiltronFormation(IFormation owner)
			: base(owner)
		{
		}

		// Token: 0x06000E07 RID: 3591 RVA: 0x00027F3C File Offset: 0x0002613C
		public override IFormationArrangement Clone(IFormation formation)
		{
			return new CircularSchiltronFormation(formation);
		}

		// Token: 0x17000330 RID: 816
		// (get) Token: 0x06000E08 RID: 3592 RVA: 0x00027F44 File Offset: 0x00026144
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

		// Token: 0x06000E09 RID: 3593 RVA: 0x00027F98 File Offset: 0x00026198
		public void Form()
		{
			int unitCountWithOverride = base.GetUnitCountWithOverride();
			int currentMaximumRankCount = base.GetCurrentMaximumRankCount(unitCountWithOverride);
			float circumferenceFromRankCount = base.GetCircumferenceFromRankCount(currentMaximumRankCount);
			base.FormFromCircumference(circumferenceFromRankCount);
		}
	}
}
