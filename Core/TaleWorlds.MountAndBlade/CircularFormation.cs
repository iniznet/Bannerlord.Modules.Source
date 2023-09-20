using System;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	public class CircularFormation : LineFormation
	{
		public CircularFormation(IFormation owner)
			: base(owner, true, true)
		{
		}

		public override IFormationArrangement Clone(IFormation formation)
		{
			return new CircularFormation(formation);
		}

		private float GetDistanceFromCenterOfRank(int rankIndex)
		{
			float num = this.Radius - (float)rankIndex * (base.Distance + base.UnitDiameter);
			if (num >= 0f)
			{
				return num;
			}
			return 0f;
		}

		protected override bool IsDeepenApplicable()
		{
			return this.Radius - (float)base.RankCount * (base.Distance + base.UnitDiameter) >= 0f;
		}

		protected override bool IsNarrowApplicable(int amount)
		{
			return ((float)(base.FileCount - 1 - amount) * (base.Interval + base.UnitDiameter) + base.UnitDiameter) / 6.2831855f - (float)base.RankCount * (base.Distance + base.UnitDiameter) >= 0f;
		}

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

		public override float Depth
		{
			get
			{
				return this.Diameter;
			}
		}

		private float Diameter
		{
			get
			{
				return 2f * this.Radius;
			}
		}

		private float Radius
		{
			get
			{
				return (base.FlankWidth + base.Interval) / 6.2831855f;
			}
		}

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

		public override float MaximumWidth
		{
			get
			{
				int unitCountWithOverride = base.GetUnitCountWithOverride();
				float num = this.owner.MaximumInterval + base.UnitDiameter;
				return MathF.Max(0f, (float)unitCountWithOverride * num) / 3.1415927f;
			}
		}

		private int MaxRank
		{
			get
			{
				return MathF.Floor(this.Radius / (base.Distance + base.UnitDiameter));
			}
		}

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

		protected override Vec2 GetLocalDirectionOfUnit(int fileIndex, int rankIndex)
		{
			int unitCountOfRank = this.GetUnitCountOfRank(rankIndex);
			int num = (base.FileCount - unitCountOfRank) / 2;
			Vec2 vec = Vec2.FromRotation((float)((fileIndex - num) * 2) * 3.1415927f / (float)unitCountOfRank + 3.1415927f);
			vec.x *= -1f;
			return vec;
		}

		public override Vec2? GetLocalDirectionOfUnitOrDefault(IFormationUnit unit)
		{
			if (unit.FormationFileIndex < 0 || unit.FormationRankIndex < 0)
			{
				return null;
			}
			return new Vec2?(this.GetLocalDirectionOfUnit(unit.FormationFileIndex, unit.FormationRankIndex));
		}

		protected override Vec2 GetLocalPositionOfUnit(int fileIndex, int rankIndex)
		{
			Vec2 vec = new Vec2(0f, -this.Radius);
			Vec2 localDirectionOfUnit = this.GetLocalDirectionOfUnit(fileIndex, rankIndex);
			float distanceFromCenterOfRank = this.GetDistanceFromCenterOfRank(rankIndex);
			return vec + localDirectionOfUnit * distanceFromCenterOfRank;
		}

		protected override Vec2 GetLocalPositionOfUnitWithAdjustment(int fileIndex, int rankIndex, float distanceBetweenAgentsAdjustment)
		{
			return this.GetLocalPositionOfUnit(fileIndex, rankIndex);
		}

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

		public float GetCircumferenceFromRankCount(int rankCount)
		{
			int unitCountWithOverride = base.GetUnitCountWithOverride();
			rankCount = MathF.Min(this.GetCurrentMaximumRankCount(unitCountWithOverride), rankCount);
			float num = base.Interval + base.UnitDiameter;
			float num2 = base.Distance + base.UnitDiameter;
			return this.GetCircumferenceAux(unitCountWithOverride, rankCount, num, num2);
		}

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
