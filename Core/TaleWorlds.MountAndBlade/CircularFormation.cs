using System;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x0200012A RID: 298
	public class CircularFormation : LineFormation
	{
		// Token: 0x06000DED RID: 3565 RVA: 0x00027944 File Offset: 0x00025B44
		public CircularFormation(IFormation owner)
			: base(owner, true, true)
		{
		}

		// Token: 0x06000DEE RID: 3566 RVA: 0x0002794F File Offset: 0x00025B4F
		public override IFormationArrangement Clone(IFormation formation)
		{
			return new CircularFormation(formation);
		}

		// Token: 0x06000DEF RID: 3567 RVA: 0x00027958 File Offset: 0x00025B58
		private float GetDistanceFromCenterOfRank(int rankIndex)
		{
			float num = this.Radius - (float)rankIndex * (base.Distance + base.UnitDiameter);
			if (num >= 0f)
			{
				return num;
			}
			return 0f;
		}

		// Token: 0x06000DF0 RID: 3568 RVA: 0x0002798C File Offset: 0x00025B8C
		protected override bool IsDeepenApplicable()
		{
			return this.Radius - (float)base.RankCount * (base.Distance + base.UnitDiameter) >= 0f;
		}

		// Token: 0x06000DF1 RID: 3569 RVA: 0x000279B4 File Offset: 0x00025BB4
		protected override bool IsNarrowApplicable(int amount)
		{
			return ((float)(base.FileCount - 1 - amount) * (base.Interval + base.UnitDiameter) + base.UnitDiameter) / 6.2831855f - (float)base.RankCount * (base.Distance + base.UnitDiameter) >= 0f;
		}

		// Token: 0x06000DF2 RID: 3570 RVA: 0x00027A08 File Offset: 0x00025C08
		private int GetUnitCountOfRank(int rankIndex)
		{
			if (rankIndex == 0)
			{
				return base.FileCount;
			}
			float distanceFromCenterOfRank = this.GetDistanceFromCenterOfRank(rankIndex);
			int num = MathF.Floor(6.2831855f * distanceFromCenterOfRank / (base.Interval + base.UnitDiameter));
			return MathF.Max(1, num);
		}

		// Token: 0x17000329 RID: 809
		// (get) Token: 0x06000DF3 RID: 3571 RVA: 0x00027A49 File Offset: 0x00025C49
		// (set) Token: 0x06000DF4 RID: 3572 RVA: 0x00027A54 File Offset: 0x00025C54
		public override float Width
		{
			get
			{
				return this.Diameter;
			}
			set
			{
				float num = 3.1415927f * value;
				this.FormFromCircumference(num);
			}
		}

		// Token: 0x1700032A RID: 810
		// (get) Token: 0x06000DF5 RID: 3573 RVA: 0x00027A72 File Offset: 0x00025C72
		public override float Depth
		{
			get
			{
				return this.Diameter;
			}
		}

		// Token: 0x1700032B RID: 811
		// (get) Token: 0x06000DF6 RID: 3574 RVA: 0x00027A7A File Offset: 0x00025C7A
		private float Diameter
		{
			get
			{
				return 2f * this.Radius;
			}
		}

		// Token: 0x1700032C RID: 812
		// (get) Token: 0x06000DF7 RID: 3575 RVA: 0x00027A88 File Offset: 0x00025C88
		private float Radius
		{
			get
			{
				return (base.FlankWidth + base.Interval) / 6.2831855f;
			}
		}

		// Token: 0x1700032D RID: 813
		// (get) Token: 0x06000DF8 RID: 3576 RVA: 0x00027AA0 File Offset: 0x00025CA0
		public override float MinimumWidth
		{
			get
			{
				int unitCountWithOverride = base.GetUnitCountWithOverride();
				int currentMaximumRankCount = this.GetCurrentMaximumRankCount(unitCountWithOverride);
				float num = this.owner.MinimumInterval + base.UnitDiameter;
				float num2 = this.owner.MinimumDistance + base.UnitDiameter;
				return this.GetCircumferenceAux(unitCountWithOverride, currentMaximumRankCount, num, num2) / 3.1415927f;
			}
		}

		// Token: 0x1700032E RID: 814
		// (get) Token: 0x06000DF9 RID: 3577 RVA: 0x00027AF4 File Offset: 0x00025CF4
		public override float MaximumWidth
		{
			get
			{
				int unitCountWithOverride = base.GetUnitCountWithOverride();
				float num = this.owner.MaximumInterval + base.UnitDiameter;
				return MathF.Max(0f, (float)unitCountWithOverride * num) / 3.1415927f;
			}
		}

		// Token: 0x1700032F RID: 815
		// (get) Token: 0x06000DFA RID: 3578 RVA: 0x00027B2F File Offset: 0x00025D2F
		private int MaxRank
		{
			get
			{
				return MathF.Floor(this.Radius / (base.Distance + base.UnitDiameter));
			}
		}

		// Token: 0x06000DFB RID: 3579 RVA: 0x00027B4C File Offset: 0x00025D4C
		protected override bool IsUnitPositionRestrained(int fileIndex, int rankIndex)
		{
			if (base.IsUnitPositionRestrained(fileIndex, rankIndex))
			{
				return true;
			}
			if (rankIndex > this.MaxRank)
			{
				return true;
			}
			int unitCountOfRank = this.GetUnitCountOfRank(rankIndex);
			int num = (base.FileCount - unitCountOfRank) / 2;
			return fileIndex < num || fileIndex >= num + unitCountOfRank;
		}

		// Token: 0x06000DFC RID: 3580 RVA: 0x00027B90 File Offset: 0x00025D90
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

		// Token: 0x06000DFD RID: 3581 RVA: 0x00027BD8 File Offset: 0x00025DD8
		protected override Vec2 GetLocalDirectionOfUnit(int fileIndex, int rankIndex)
		{
			int unitCountOfRank = this.GetUnitCountOfRank(rankIndex);
			int num = (base.FileCount - unitCountOfRank) / 2;
			Vec2 vec = Vec2.FromRotation((float)((fileIndex - num) * 2) * 3.1415927f / (float)unitCountOfRank + 3.1415927f);
			vec.x *= -1f;
			return vec;
		}

		// Token: 0x06000DFE RID: 3582 RVA: 0x00027C24 File Offset: 0x00025E24
		protected override Vec2 GetLocalPositionOfUnit(int fileIndex, int rankIndex)
		{
			Vec2 vec = new Vec2(0f, -this.Radius);
			Vec2 localDirectionOfUnit = this.GetLocalDirectionOfUnit(fileIndex, rankIndex);
			float distanceFromCenterOfRank = this.GetDistanceFromCenterOfRank(rankIndex);
			return vec + localDirectionOfUnit * distanceFromCenterOfRank;
		}

		// Token: 0x06000DFF RID: 3583 RVA: 0x00027C5F File Offset: 0x00025E5F
		protected override Vec2 GetLocalPositionOfUnitWithAdjustment(int fileIndex, int rankIndex, float distanceBetweenAgentsAdjustment)
		{
			return this.GetLocalPositionOfUnit(fileIndex, rankIndex);
		}

		// Token: 0x06000E00 RID: 3584 RVA: 0x00027C6C File Offset: 0x00025E6C
		protected override bool TryGetUnitPositionIndexFromLocalPosition(Vec2 localPosition, out int fileIndex, out int rankIndex)
		{
			Vec2 vec = new Vec2(0f, -this.Radius);
			Vec2 vec2 = localPosition - vec;
			float length = vec2.Length;
			rankIndex = MathF.Round((length - this.Radius) / (base.Distance + base.UnitDiameter) * -1f);
			if (rankIndex < 0 || rankIndex >= base.RankCount)
			{
				fileIndex = -1;
				return false;
			}
			if (this.Radius - (float)rankIndex * (base.Distance + base.UnitDiameter) < 0f)
			{
				fileIndex = -1;
				return false;
			}
			int unitCountOfRank = this.GetUnitCountOfRank(rankIndex);
			int num = (base.FileCount - unitCountOfRank) / 2;
			vec2.x *= -1f;
			float num2 = vec2.RotationInRadians;
			num2 -= 3.1415927f;
			if (num2 < 0f)
			{
				num2 += 6.2831855f;
			}
			int num3 = MathF.Round(num2 / 2f / 3.1415927f * (float)unitCountOfRank);
			fileIndex = num3 + num;
			return fileIndex >= 0 && fileIndex < base.FileCount;
		}

		// Token: 0x06000E01 RID: 3585 RVA: 0x00027D74 File Offset: 0x00025F74
		protected int GetCurrentMaximumRankCount(int unitCount)
		{
			int num = 0;
			int i = 0;
			float num2 = base.Interval + base.UnitDiameter;
			float num3 = base.Distance + base.UnitDiameter;
			while (i < unitCount)
			{
				float num4 = (float)num * num3;
				int num5 = (int)(6.2831855f * num4 / num2);
				i += MathF.Max(1, num5);
				num++;
			}
			return MathF.Max(num, 1);
		}

		// Token: 0x06000E02 RID: 3586 RVA: 0x00027DD0 File Offset: 0x00025FD0
		public float GetCircumferenceFromRankCount(int rankCount)
		{
			int unitCountWithOverride = base.GetUnitCountWithOverride();
			rankCount = MathF.Min(this.GetCurrentMaximumRankCount(unitCountWithOverride), rankCount);
			float num = base.Interval + base.UnitDiameter;
			float num2 = base.Distance + base.UnitDiameter;
			return this.GetCircumferenceAux(unitCountWithOverride, rankCount, num, num2);
		}

		// Token: 0x06000E03 RID: 3587 RVA: 0x00027E1C File Offset: 0x0002601C
		public void FormFromCircumference(float circumference)
		{
			int unitCountWithOverride = base.GetUnitCountWithOverride();
			int currentMaximumRankCount = this.GetCurrentMaximumRankCount(unitCountWithOverride);
			float num = base.Interval + base.UnitDiameter;
			float num2 = base.Distance + base.UnitDiameter;
			float circumferenceAux = this.GetCircumferenceAux(unitCountWithOverride, currentMaximumRankCount, num, num2);
			float num3 = MathF.Max(0f, (float)unitCountWithOverride * num);
			circumference = MBMath.ClampFloat(circumference, circumferenceAux, num3);
			base.FlankWidth = Math.Max(circumference - base.Interval, base.UnitDiameter);
		}

		// Token: 0x06000E04 RID: 3588 RVA: 0x00027E98 File Offset: 0x00026098
		protected float GetCircumferenceAux(int unitCount, int rankCount, float radialInterval, float distanceInterval)
		{
			float num = (float)(6.283185307179586 * (double)distanceInterval);
			float num2 = MathF.Max(0f, (float)unitCount * radialInterval);
			float num3;
			int unitCountAux;
			do
			{
				num3 = num2;
				num2 = MathF.Max(0f, num3 - num);
				unitCountAux = CircularFormation.GetUnitCountAux(num2, rankCount, radialInterval, distanceInterval);
			}
			while (unitCountAux > unitCount && num3 > 0f);
			return num3;
		}

		// Token: 0x06000E05 RID: 3589 RVA: 0x00027EEC File Offset: 0x000260EC
		private static int GetUnitCountAux(float circumference, int rankCount, float radialInterval, float distanceInterval)
		{
			int num = 0;
			double num2 = 6.283185307179586 * (double)distanceInterval;
			for (int i = 1; i <= rankCount; i++)
			{
				num += (int)(Math.Max(0.0, (double)circumference - (double)(rankCount - i) * num2) / (double)radialInterval);
			}
			return num;
		}
	}
}
