using System;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	public class SquareFormation : LineFormation
	{
		public override float Width
		{
			get
			{
				return SquareFormation.GetSideWidthFromUnitCount(this.UnitCountOfOuterSide, base.Interval, base.UnitDiameter);
			}
			set
			{
				this.FormFromBorderSideWidth(value);
			}
		}

		public override float Depth
		{
			get
			{
				return SquareFormation.GetSideWidthFromUnitCount(this.UnitCountOfOuterSide, base.Interval, base.UnitDiameter);
			}
		}

		public override float MinimumWidth
		{
			get
			{
				int num;
				int maximumRankCount = SquareFormation.GetMaximumRankCount(base.GetUnitCountWithOverride(), out num);
				return SquareFormation.GetSideWidthFromUnitCount(this.GetUnitsPerSideFromRankCount(maximumRankCount), this.owner.MinimumInterval, base.UnitDiameter);
			}
		}

		public override float MaximumWidth
		{
			get
			{
				return SquareFormation.GetSideWidthFromUnitCount(this.GetUnitsPerSideFromRankCount(1), this.owner.MaximumInterval, base.UnitDiameter);
			}
		}

		private int UnitCountOfOuterSide
		{
			get
			{
				return MathF.Ceiling((float)base.FileCount / 4f) + 1;
			}
		}

		private int MaxRank
		{
			get
			{
				return (this.UnitCountOfOuterSide + 1) / 2;
			}
		}

		private new float Distance
		{
			get
			{
				return base.Interval;
			}
		}

		protected bool DisableRearOfLastRank
		{
			get
			{
				return this._disableRearOfLastRank;
			}
			set
			{
				if (this._disableRearOfLastRank != value)
				{
					this._disableRearOfLastRank = value;
					base.OnFormationFrameChanged();
				}
			}
		}

		public SquareFormation(IFormation owner)
			: base(owner, true, true)
		{
		}

		public override IFormationArrangement Clone(IFormation formation)
		{
			return new SquareFormation(formation);
		}

		public override void DeepCopyFrom(IFormationArrangement arrangement)
		{
			base.DeepCopyFrom(arrangement);
			this.DisableRearOfLastRank = (arrangement as SquareFormation).DisableRearOfLastRank;
		}

		public void FormFromBorderSideWidth(float borderSideWidth)
		{
			int num = MathF.Max(1, (int)((borderSideWidth - base.UnitDiameter) / (base.Interval + base.UnitDiameter) + 1E-05f)) + 1;
			this.FormFromBorderUnitCountPerSide(num);
		}

		public void FormFromBorderUnitCountPerSide(int unitCountPerSide)
		{
			if (unitCountPerSide == 1)
			{
				base.FlankWidth = base.UnitDiameter;
				return;
			}
			base.FlankWidth = (float)(4 * (unitCountPerSide - 1) - 1) * (base.Interval + base.UnitDiameter) + base.UnitDiameter;
		}

		public int GetUnitsPerSideFromRankCount(int rankCount)
		{
			int unitCountWithOverride = base.GetUnitCountWithOverride();
			int num;
			rankCount = MathF.Min(SquareFormation.GetMaximumRankCount(unitCountWithOverride, out num), rankCount);
			float num2 = (float)unitCountWithOverride / (4f * (float)rankCount) + (float)rankCount;
			int num3 = MathF.Ceiling(num2);
			int num4 = MathF.Round(num2);
			if (num4 < num3 && num4 * num4 == unitCountWithOverride)
			{
				num3 = num4;
			}
			if (num3 == 0)
			{
				num3 = 1;
			}
			return num3;
		}

		protected static int GetMaximumRankCount(int unitCount, out int minimumFlankCount)
		{
			int num = (int)MathF.Sqrt((float)unitCount);
			if (num * num != unitCount)
			{
				num++;
			}
			minimumFlankCount = num;
			return MathF.Max(1, (num + 1) / 2);
		}

		public void FormFromRankCount(int rankCount)
		{
			int unitsPerSideFromRankCount = this.GetUnitsPerSideFromRankCount(rankCount);
			this.FormFromBorderUnitCountPerSide(unitsPerSideFromRankCount);
		}

		private SquareFormation.Side GetSideOfUnitPosition(int fileIndex)
		{
			return (SquareFormation.Side)(fileIndex / (this.UnitCountOfOuterSide - 1));
		}

		private SquareFormation.Side? GetSideOfUnitPosition(int fileIndex, int rankIndex)
		{
			SquareFormation.Side sideOfUnitPosition = this.GetSideOfUnitPosition(fileIndex);
			if (rankIndex == 0)
			{
				return new SquareFormation.Side?(sideOfUnitPosition);
			}
			int num = this.UnitCountOfOuterSide - 2 * rankIndex;
			if (num == 1 && sideOfUnitPosition != SquareFormation.Side.Front)
			{
				return null;
			}
			int num2 = fileIndex % (this.UnitCountOfOuterSide - 1);
			int num3 = this.UnitCountOfOuterSide - num;
			num3 /= 2;
			if (num2 >= num3 && this.UnitCountOfOuterSide - num2 - 1 > num3)
			{
				return new SquareFormation.Side?(sideOfUnitPosition);
			}
			return null;
		}

		private Vec2 GetLocalPositionOfUnitAux(int fileIndex, int rankIndex, float usedInterval)
		{
			if (this.UnitCountOfOuterSide == 1)
			{
				return Vec2.Zero;
			}
			SquareFormation.Side sideOfUnitPosition = this.GetSideOfUnitPosition(fileIndex);
			float num = (float)(this.UnitCountOfOuterSide - 1) * (usedInterval + base.UnitDiameter);
			float num2 = (float)(fileIndex % (this.UnitCountOfOuterSide - 1)) * (usedInterval + base.UnitDiameter);
			float num3 = (float)rankIndex * (this.Distance + base.UnitDiameter);
			Vec2 vec;
			switch (sideOfUnitPosition)
			{
			case SquareFormation.Side.Front:
				vec = new Vec2(-num / 2f, 0f);
				vec += new Vec2(num2, -num3);
				break;
			case SquareFormation.Side.Right:
				vec = new Vec2(num / 2f, 0f);
				vec += new Vec2(-num3, -num2);
				break;
			case SquareFormation.Side.Rear:
				vec = new Vec2(num / 2f, -num);
				vec += new Vec2(-num2, num3);
				break;
			case SquareFormation.Side.Left:
				vec = new Vec2(-num / 2f, -num);
				vec += new Vec2(num3, num2);
				break;
			default:
				Debug.FailedAssert("false", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\AI\\Formation\\SquareFormation.cs", "GetLocalPositionOfUnitAux", 391);
				vec = Vec2.Zero;
				break;
			}
			return vec;
		}

		protected override Vec2 GetLocalPositionOfUnit(int fileIndex, int rankIndex)
		{
			int num = this.ShiftFileIndex(fileIndex);
			return this.GetLocalPositionOfUnitAux(num, rankIndex, base.Interval);
		}

		protected override Vec2 GetLocalPositionOfUnitWithAdjustment(int fileIndex, int rankIndex, float distanceBetweenAgentsAdjustment)
		{
			int num = this.ShiftFileIndex(fileIndex);
			return this.GetLocalPositionOfUnitAux(num, rankIndex, base.Interval + distanceBetweenAgentsAdjustment);
		}

		protected override Vec2 GetLocalDirectionOfUnit(int fileIndex, int rankIndex)
		{
			int num = this.ShiftFileIndex(fileIndex);
			switch (this.GetSideOfUnitPosition(num))
			{
			case SquareFormation.Side.Front:
				return Vec2.Forward;
			case SquareFormation.Side.Right:
				return Vec2.Side;
			case SquareFormation.Side.Rear:
				return -Vec2.Forward;
			case SquareFormation.Side.Left:
				return -Vec2.Side;
			default:
				Debug.FailedAssert("false", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\AI\\Formation\\SquareFormation.cs", "GetLocalDirectionOfUnit", 470);
				return Vec2.Forward;
			}
		}

		protected override bool IsUnitPositionRestrained(int fileIndex, int rankIndex)
		{
			if (base.IsUnitPositionRestrained(fileIndex, rankIndex))
			{
				return true;
			}
			if (rankIndex >= this.MaxRank)
			{
				return true;
			}
			int num = this.ShiftFileIndex(fileIndex);
			SquareFormation.Side? sideOfUnitPosition = this.GetSideOfUnitPosition(num, rankIndex);
			return (this.DisableRearOfLastRank && rankIndex == 0 && sideOfUnitPosition != null && num >= (this.UnitCountOfOuterSide - 1) * 2 && num <= (this.UnitCountOfOuterSide - 1) * 3) || sideOfUnitPosition == null;
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

		private SquareFormation.Side GetSideOfLocalPosition(Vec2 localPosition)
		{
			float num = (float)(this.UnitCountOfOuterSide - 1) * (base.Interval + base.UnitDiameter);
			Vec2 vec = new Vec2(0f, -num / 2f);
			Vec2 vec2 = localPosition - vec;
			vec2.y *= (base.Interval + base.UnitDiameter) / (this.Distance + base.UnitDiameter);
			float num2 = vec2.RotationInRadians;
			if (num2 < 0f)
			{
				num2 += 6.2831855f;
			}
			if (num2 <= 0.7863982f || num2 > 5.4987874f)
			{
				return SquareFormation.Side.Front;
			}
			if (num2 <= 2.3571944f)
			{
				return SquareFormation.Side.Left;
			}
			if (num2 <= 3.927991f)
			{
				return SquareFormation.Side.Rear;
			}
			return SquareFormation.Side.Right;
		}

		protected override bool TryGetUnitPositionIndexFromLocalPosition(Vec2 localPosition, out int fileIndex, out int rankIndex)
		{
			SquareFormation.Side sideOfLocalPosition = this.GetSideOfLocalPosition(localPosition);
			float num = (float)(this.UnitCountOfOuterSide - 1) * (base.Interval + base.UnitDiameter);
			float num2;
			float num3;
			switch (sideOfLocalPosition)
			{
			case SquareFormation.Side.Front:
			{
				Vec2 vec = localPosition - new Vec2(-num / 2f, 0f);
				num2 = vec.x;
				num3 = -vec.y;
				break;
			}
			case SquareFormation.Side.Right:
			{
				Vec2 vec2 = localPosition - new Vec2(num / 2f, 0f);
				num2 = -vec2.y;
				num3 = -vec2.x;
				break;
			}
			case SquareFormation.Side.Rear:
			{
				Vec2 vec3 = localPosition - new Vec2(num / 2f, -num);
				num2 = -vec3.x;
				num3 = vec3.y;
				break;
			}
			case SquareFormation.Side.Left:
			{
				Vec2 vec4 = localPosition - new Vec2(-num / 2f, -num);
				num2 = vec4.y;
				num3 = vec4.x;
				break;
			}
			default:
				Debug.FailedAssert("false", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\AI\\Formation\\SquareFormation.cs", "TryGetUnitPositionIndexFromLocalPosition", 601);
				num2 = 0f;
				num3 = 0f;
				break;
			}
			rankIndex = MathF.Round(num3 / (this.Distance + base.UnitDiameter));
			if (rankIndex < 0 || rankIndex >= base.RankCount || rankIndex >= this.MaxRank)
			{
				fileIndex = -1;
				return false;
			}
			int num4 = MathF.Round(num2 / (base.Interval + base.UnitDiameter));
			if (num4 >= this.UnitCountOfOuterSide - 1)
			{
				fileIndex = 1;
				return false;
			}
			int num5 = num4 + (this.UnitCountOfOuterSide - 1) * (int)sideOfLocalPosition;
			fileIndex = this.UnshiftFileIndex(num5);
			return fileIndex >= 0 && fileIndex < base.FileCount;
		}

		private int ShiftFileIndex(int fileIndex)
		{
			int num = this.UnitCountOfOuterSide + this.UnitCountOfOuterSide / 2 - 2;
			int num2 = fileIndex - num;
			if (num2 < 0)
			{
				num2 += (this.UnitCountOfOuterSide - 1) * 4;
			}
			return num2;
		}

		private int UnshiftFileIndex(int shiftedFileIndex)
		{
			int num = this.UnitCountOfOuterSide + this.UnitCountOfOuterSide / 2 - 2;
			int num2 = shiftedFileIndex + num;
			if (num2 >= (this.UnitCountOfOuterSide - 1) * 4)
			{
				num2 -= (this.UnitCountOfOuterSide - 1) * 4;
			}
			return num2;
		}

		protected static float GetSideWidthFromUnitCount(int sideUnitCount, float interval, float unitDiameter)
		{
			if (sideUnitCount > 0)
			{
				return (float)(sideUnitCount - 1) * (interval + unitDiameter) + unitDiameter;
			}
			return 0f;
		}

		private bool _disableRearOfLastRank;

		private enum Side
		{
			Front,
			Right,
			Rear,
			Left
		}
	}
}
