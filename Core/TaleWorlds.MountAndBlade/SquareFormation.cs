using System;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000136 RID: 310
	public class SquareFormation : LineFormation
	{
		// Token: 0x17000371 RID: 881
		// (get) Token: 0x06000F7F RID: 3967 RVA: 0x0002E62A File Offset: 0x0002C82A
		// (set) Token: 0x06000F80 RID: 3968 RVA: 0x0002E644 File Offset: 0x0002C844
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

		// Token: 0x17000372 RID: 882
		// (get) Token: 0x06000F81 RID: 3969 RVA: 0x0002E65A File Offset: 0x0002C85A
		public override float Depth
		{
			get
			{
				return SquareFormation.GetSideWidthFromUnitCount(this.UnitCountOfOuterSide, base.Interval, base.UnitDiameter);
			}
		}

		// Token: 0x17000373 RID: 883
		// (get) Token: 0x06000F82 RID: 3970 RVA: 0x0002E674 File Offset: 0x0002C874
		public override float MinimumWidth
		{
			get
			{
				int num;
				int maximumRankCount = SquareFormation.GetMaximumRankCount(base.GetUnitCountWithOverride(), out num);
				return SquareFormation.GetSideWidthFromUnitCount(this.GetUnitsPerSideFromRankCount(maximumRankCount), this.owner.MinimumInterval, base.UnitDiameter);
			}
		}

		// Token: 0x17000374 RID: 884
		// (get) Token: 0x06000F83 RID: 3971 RVA: 0x0002E6AC File Offset: 0x0002C8AC
		public override float MaximumWidth
		{
			get
			{
				return SquareFormation.GetSideWidthFromUnitCount(this.GetUnitsPerSideFromRankCount(1), this.owner.MaximumInterval, base.UnitDiameter);
			}
		}

		// Token: 0x17000375 RID: 885
		// (get) Token: 0x06000F84 RID: 3972 RVA: 0x0002E6CB File Offset: 0x0002C8CB
		private int UnitCountOfOuterSide
		{
			get
			{
				return MathF.Ceiling((float)base.FileCount / 4f) + 1;
			}
		}

		// Token: 0x17000376 RID: 886
		// (get) Token: 0x06000F85 RID: 3973 RVA: 0x0002E6E1 File Offset: 0x0002C8E1
		private int MaxRank
		{
			get
			{
				return (this.UnitCountOfOuterSide + 1) / 2;
			}
		}

		// Token: 0x17000377 RID: 887
		// (get) Token: 0x06000F86 RID: 3974 RVA: 0x0002E6ED File Offset: 0x0002C8ED
		private new float Distance
		{
			get
			{
				return base.Interval;
			}
		}

		// Token: 0x17000378 RID: 888
		// (get) Token: 0x06000F87 RID: 3975 RVA: 0x0002E6F5 File Offset: 0x0002C8F5
		// (set) Token: 0x06000F88 RID: 3976 RVA: 0x0002E6FD File Offset: 0x0002C8FD
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

		// Token: 0x06000F89 RID: 3977 RVA: 0x0002E715 File Offset: 0x0002C915
		public SquareFormation(IFormation owner)
			: base(owner, true, true)
		{
		}

		// Token: 0x06000F8A RID: 3978 RVA: 0x0002E720 File Offset: 0x0002C920
		public override IFormationArrangement Clone(IFormation formation)
		{
			return new SquareFormation(formation);
		}

		// Token: 0x06000F8B RID: 3979 RVA: 0x0002E728 File Offset: 0x0002C928
		public override void DeepCopyFrom(IFormationArrangement arrangement)
		{
			base.DeepCopyFrom(arrangement);
			this.DisableRearOfLastRank = (arrangement as SquareFormation).DisableRearOfLastRank;
		}

		// Token: 0x06000F8C RID: 3980 RVA: 0x0002E744 File Offset: 0x0002C944
		public void FormFromBorderSideWidth(float borderSideWidth)
		{
			int num = MathF.Max(1, (int)((borderSideWidth - base.UnitDiameter) / (base.Interval + base.UnitDiameter) + 1E-05f)) + 1;
			this.FormFromBorderUnitCountPerSide(num);
		}

		// Token: 0x06000F8D RID: 3981 RVA: 0x0002E77E File Offset: 0x0002C97E
		public void FormFromBorderUnitCountPerSide(int unitCountPerSide)
		{
			if (unitCountPerSide == 1)
			{
				base.FlankWidth = base.UnitDiameter;
				return;
			}
			base.FlankWidth = (float)(4 * (unitCountPerSide - 1) - 1) * (base.Interval + base.UnitDiameter) + base.UnitDiameter;
		}

		// Token: 0x06000F8E RID: 3982 RVA: 0x0002E7B4 File Offset: 0x0002C9B4
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

		// Token: 0x06000F8F RID: 3983 RVA: 0x0002E808 File Offset: 0x0002CA08
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

		// Token: 0x06000F90 RID: 3984 RVA: 0x0002E838 File Offset: 0x0002CA38
		public void FormFromRankCount(int rankCount)
		{
			int unitsPerSideFromRankCount = this.GetUnitsPerSideFromRankCount(rankCount);
			this.FormFromBorderUnitCountPerSide(unitsPerSideFromRankCount);
		}

		// Token: 0x06000F91 RID: 3985 RVA: 0x0002E854 File Offset: 0x0002CA54
		private SquareFormation.Side GetSideOfUnitPosition(int fileIndex)
		{
			return (SquareFormation.Side)(fileIndex / (this.UnitCountOfOuterSide - 1));
		}

		// Token: 0x06000F92 RID: 3986 RVA: 0x0002E860 File Offset: 0x0002CA60
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

		// Token: 0x06000F93 RID: 3987 RVA: 0x0002E8DC File Offset: 0x0002CADC
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

		// Token: 0x06000F94 RID: 3988 RVA: 0x0002EA10 File Offset: 0x0002CC10
		protected override Vec2 GetLocalPositionOfUnit(int fileIndex, int rankIndex)
		{
			int num = this.ShiftFileIndex(fileIndex);
			return this.GetLocalPositionOfUnitAux(num, rankIndex, base.Interval);
		}

		// Token: 0x06000F95 RID: 3989 RVA: 0x0002EA34 File Offset: 0x0002CC34
		protected override Vec2 GetLocalPositionOfUnitWithAdjustment(int fileIndex, int rankIndex, float distanceBetweenAgentsAdjustment)
		{
			int num = this.ShiftFileIndex(fileIndex);
			return this.GetLocalPositionOfUnitAux(num, rankIndex, base.Interval + distanceBetweenAgentsAdjustment);
		}

		// Token: 0x06000F96 RID: 3990 RVA: 0x0002EA5C File Offset: 0x0002CC5C
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

		// Token: 0x06000F97 RID: 3991 RVA: 0x0002EAD4 File Offset: 0x0002CCD4
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

		// Token: 0x06000F98 RID: 3992 RVA: 0x0002EB44 File Offset: 0x0002CD44
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

		// Token: 0x06000F99 RID: 3993 RVA: 0x0002EB8C File Offset: 0x0002CD8C
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

		// Token: 0x06000F9A RID: 3994 RVA: 0x0002EC34 File Offset: 0x0002CE34
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

		// Token: 0x06000F9B RID: 3995 RVA: 0x0002EDC4 File Offset: 0x0002CFC4
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

		// Token: 0x06000F9C RID: 3996 RVA: 0x0002EDFC File Offset: 0x0002CFFC
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

		// Token: 0x06000F9D RID: 3997 RVA: 0x0002EE3A File Offset: 0x0002D03A
		protected static float GetSideWidthFromUnitCount(int sideUnitCount, float interval, float unitDiameter)
		{
			if (sideUnitCount > 0)
			{
				return (float)(sideUnitCount - 1) * (interval + unitDiameter) + unitDiameter;
			}
			return 0f;
		}

		// Token: 0x040003AA RID: 938
		private bool _disableRearOfLastRank;

		// Token: 0x02000470 RID: 1136
		private enum Side
		{
			// Token: 0x04001914 RID: 6420
			Front,
			// Token: 0x04001915 RID: 6421
			Right,
			// Token: 0x04001916 RID: 6422
			Rear,
			// Token: 0x04001917 RID: 6423
			Left
		}
	}
}
