using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000132 RID: 306
	public class LineFormation : IFormationArrangement
	{
		// Token: 0x1700035D RID: 861
		// (get) Token: 0x06000EDE RID: 3806 RVA: 0x0002A5D2 File Offset: 0x000287D2
		protected int FileCount
		{
			get
			{
				return this._units2D.Count1;
			}
		}

		// Token: 0x1700035E RID: 862
		// (get) Token: 0x06000EDF RID: 3807 RVA: 0x0002A5DF File Offset: 0x000287DF
		public int RankCount
		{
			get
			{
				return this._units2D.Count2;
			}
		}

		// Token: 0x1700035F RID: 863
		// (get) Token: 0x06000EE0 RID: 3808 RVA: 0x0002A5EC File Offset: 0x000287EC
		// (set) Token: 0x06000EE1 RID: 3809 RVA: 0x0002A5F4 File Offset: 0x000287F4
		public bool AreLocalPositionsDirty { protected get; set; }

		// Token: 0x17000360 RID: 864
		// (get) Token: 0x06000EE2 RID: 3810 RVA: 0x0002A5FD File Offset: 0x000287FD
		protected float Interval
		{
			get
			{
				return this.owner.Interval;
			}
		}

		// Token: 0x17000361 RID: 865
		// (get) Token: 0x06000EE3 RID: 3811 RVA: 0x0002A60A File Offset: 0x0002880A
		protected float Distance
		{
			get
			{
				return this.owner.Distance;
			}
		}

		// Token: 0x17000362 RID: 866
		// (get) Token: 0x06000EE4 RID: 3812 RVA: 0x0002A617 File Offset: 0x00028817
		protected float UnitDiameter
		{
			get
			{
				return this.owner.UnitDiameter;
			}
		}

		// Token: 0x06000EE5 RID: 3813 RVA: 0x0002A624 File Offset: 0x00028824
		public int GetFileCountFromWidth(float width)
		{
			return MathF.Max(MathF.Max(0, (int)((width - this.UnitDiameter) / (this.Interval + this.UnitDiameter) + 1E-05f)) + 1, this.MinimumFileCount);
		}

		// Token: 0x17000363 RID: 867
		// (get) Token: 0x06000EE6 RID: 3814 RVA: 0x0002A656 File Offset: 0x00028856
		// (set) Token: 0x06000EE7 RID: 3815 RVA: 0x0002A65E File Offset: 0x0002885E
		public virtual float Width
		{
			get
			{
				return this.FlankWidth;
			}
			set
			{
				this.FlankWidth = value;
			}
		}

		// Token: 0x17000364 RID: 868
		// (get) Token: 0x06000EE8 RID: 3816 RVA: 0x0002A667 File Offset: 0x00028867
		public virtual float Depth
		{
			get
			{
				return this.RankDepth;
			}
		}

		// Token: 0x17000365 RID: 869
		// (get) Token: 0x06000EE9 RID: 3817 RVA: 0x0002A66F File Offset: 0x0002886F
		// (set) Token: 0x06000EEA RID: 3818 RVA: 0x0002A690 File Offset: 0x00028890
		public float FlankWidth
		{
			get
			{
				return (float)(this.FileCount - 1) * (this.Interval + this.UnitDiameter) + this.UnitDiameter;
			}
			set
			{
				int fileCountFromWidth = this.GetFileCountFromWidth(value);
				if (fileCountFromWidth > this.FileCount)
				{
					LineFormation.WidenFormation(this, fileCountFromWidth - this.FileCount);
				}
				else if (fileCountFromWidth < this.FileCount)
				{
					LineFormation.NarrowFormation(this, this.FileCount - fileCountFromWidth);
				}
				Action onWidthChanged = this.OnWidthChanged;
				if (onWidthChanged != null)
				{
					onWidthChanged();
				}
				Action onShapeChanged = this.OnShapeChanged;
				if (onShapeChanged == null)
				{
					return;
				}
				onShapeChanged();
			}
		}

		// Token: 0x17000366 RID: 870
		// (get) Token: 0x06000EEB RID: 3819 RVA: 0x0002A6F8 File Offset: 0x000288F8
		private int MinimumFileCount
		{
			get
			{
				if (this.IsTransforming)
				{
					return 1;
				}
				int unitCountWithOverride = this.GetUnitCountWithOverride();
				return MathF.Max(1, (int)MathF.Sqrt((float)unitCountWithOverride));
			}
		}

		// Token: 0x17000367 RID: 871
		// (get) Token: 0x06000EEC RID: 3820 RVA: 0x0002A724 File Offset: 0x00028924
		public float RankDepth
		{
			get
			{
				return (float)(this.RankCount - 1) * (this.Distance + this.UnitDiameter) + this.UnitDiameter;
			}
		}

		// Token: 0x17000368 RID: 872
		// (get) Token: 0x06000EED RID: 3821 RVA: 0x0002A744 File Offset: 0x00028944
		public float MinimumFlankWidth
		{
			get
			{
				return (float)(this.MinimumFileCount - 1) * (this.MinimumInterval + this.UnitDiameter) + this.UnitDiameter;
			}
		}

		// Token: 0x17000369 RID: 873
		// (get) Token: 0x06000EEE RID: 3822 RVA: 0x0002A764 File Offset: 0x00028964
		public virtual float MinimumWidth
		{
			get
			{
				return this.MinimumFlankWidth;
			}
		}

		// Token: 0x1700036A RID: 874
		// (get) Token: 0x06000EEF RID: 3823 RVA: 0x0002A76C File Offset: 0x0002896C
		private float MinimumInterval
		{
			get
			{
				return this.owner.MinimumInterval;
			}
		}

		// Token: 0x1700036B RID: 875
		// (get) Token: 0x06000EF0 RID: 3824 RVA: 0x0002A77C File Offset: 0x0002897C
		public virtual float MaximumWidth
		{
			get
			{
				float num = this.UnitDiameter;
				int unitCountWithOverride = this.GetUnitCountWithOverride();
				if (unitCountWithOverride > 0)
				{
					num += (float)(unitCountWithOverride - 1) * (this.owner.MaximumInterval + this.UnitDiameter);
				}
				return num;
			}
		}

		// Token: 0x06000EF1 RID: 3825 RVA: 0x0002A7B8 File Offset: 0x000289B8
		protected int GetUnitCountWithOverride()
		{
			int? overridenUnitCount = this.owner.OverridenUnitCount;
			if (overridenUnitCount == null)
			{
				return this.UnitCount;
			}
			return overridenUnitCount.GetValueOrDefault();
		}

		// Token: 0x1700036C RID: 876
		// (get) Token: 0x06000EF2 RID: 3826 RVA: 0x0002A7E8 File Offset: 0x000289E8
		// (set) Token: 0x06000EF3 RID: 3827 RVA: 0x0002A7F0 File Offset: 0x000289F0
		public bool IsStaggered
		{
			get
			{
				return this._isStaggered;
			}
			set
			{
				if (this._isStaggered != value)
				{
					this._isStaggered = value;
					Action onShapeChanged = this.OnShapeChanged;
					if (onShapeChanged == null)
					{
						return;
					}
					onShapeChanged();
				}
			}
		}

		// Token: 0x1700036D RID: 877
		// (get) Token: 0x06000EF4 RID: 3828 RVA: 0x0002A814 File Offset: 0x00028A14
		public virtual bool? IsLoose
		{
			get
			{
				return null;
			}
		}

		// Token: 0x14000008 RID: 8
		// (add) Token: 0x06000EF5 RID: 3829 RVA: 0x0002A82C File Offset: 0x00028A2C
		// (remove) Token: 0x06000EF6 RID: 3830 RVA: 0x0002A864 File Offset: 0x00028A64
		public event Action OnWidthChanged;

		// Token: 0x14000009 RID: 9
		// (add) Token: 0x06000EF7 RID: 3831 RVA: 0x0002A89C File Offset: 0x00028A9C
		// (remove) Token: 0x06000EF8 RID: 3832 RVA: 0x0002A8D4 File Offset: 0x00028AD4
		public event Action OnShapeChanged;

		// Token: 0x06000EF9 RID: 3833 RVA: 0x0002A90C File Offset: 0x00028B0C
		public LineFormation(IFormation ownerFormation, bool isStaggered = true)
		{
			this.owner = ownerFormation;
			this.IsStaggered = isStaggered;
			this._units2D = new MBList2D<IFormationUnit>(1, 1);
			this.UnitPositionAvailabilities = new MBList2D<int>(1, 1);
			this._globalPositions = new MBList2D<WorldPosition>(1, 1);
			this._units2DWorkspace = new MBList2D<IFormationUnit>(1, 1);
			this._unitPositionAvailabilitiesWorkspace = new MBList2D<int>(1, 1);
			this._globalPositionsWorkspace = new MBList2D<WorldPosition>(1, 1);
			this._cachedOrderedUnitPositionIndices = new MBArrayList<Vec2i>();
			this._cachedOrderedAndAvailableUnitPositionIndices = new MBArrayList<Vec2i>();
			this._cachedOrderedLocalPositions = new MBArrayList<Vec2>();
			this._unpositionedUnits = new MBList<IFormationUnit>();
			this._displacedUnitsWorkspace = new MBWorkspace<MBQueue<ValueTuple<IFormationUnit, int, int>>>();
			this._finalOccupationsWorkspace = new MBWorkspace<MBArrayList<Vec2i>>();
			this._filledInUnitPositionsWorkspace = new MBWorkspace<MBArrayList<Vec2i>>();
			this._toBeFilledInGapsWorkspace = new MBWorkspace<MBQueue<Vec2i>>();
			this._finalVacanciesWorkspace = new MBWorkspace<MBArrayList<Vec2i>>();
			this._filledInGapsWorkspace = new MBWorkspace<MBArrayList<Vec2i>>();
			this._toBeEmptiedOutUnitPositionsWorkspace = new MBWorkspace<MBArrayList<Vec2i>>();
			this.ReconstructUnitsFromUnits2D();
			Action onShapeChanged = this.OnShapeChanged;
			if (onShapeChanged == null)
			{
				return;
			}
			onShapeChanged();
		}

		// Token: 0x06000EFA RID: 3834 RVA: 0x0002AA1C File Offset: 0x00028C1C
		protected LineFormation(IFormation ownerFormation, bool isDeformingOnWidthChange, bool isStaggered = true)
			: this(ownerFormation, isStaggered)
		{
			this._isDeformingOnWidthChange = isDeformingOnWidthChange;
		}

		// Token: 0x06000EFB RID: 3835 RVA: 0x0002AA2D File Offset: 0x00028C2D
		public virtual IFormationArrangement Clone(IFormation formation)
		{
			return new LineFormation(formation, this._isDeformingOnWidthChange, this.IsStaggered);
		}

		// Token: 0x06000EFC RID: 3836 RVA: 0x0002AA44 File Offset: 0x00028C44
		public virtual void DeepCopyFrom(IFormationArrangement arrangement)
		{
			LineFormation lineFormation = arrangement as LineFormation;
			this.IsStaggered = lineFormation.IsStaggered;
			this.IsTransforming = lineFormation.IsTransforming;
		}

		// Token: 0x06000EFD RID: 3837 RVA: 0x0002AA70 File Offset: 0x00028C70
		public void Reset()
		{
			this._units2D = new MBList2D<IFormationUnit>(1, 1);
			this.UnitPositionAvailabilities = new MBList2D<int>(1, 1);
			this._globalPositions = new MBList2D<WorldPosition>(1, 1);
			this._units2DWorkspace = new MBList2D<IFormationUnit>(1, 1);
			this._unitPositionAvailabilitiesWorkspace = new MBList2D<int>(1, 1);
			this._globalPositionsWorkspace = new MBList2D<WorldPosition>(1, 1);
			this._cachedOrderedUnitPositionIndices = new MBArrayList<Vec2i>();
			this._cachedOrderedAndAvailableUnitPositionIndices = new MBArrayList<Vec2i>();
			this._cachedOrderedLocalPositions = new MBArrayList<Vec2>();
			this._unpositionedUnits.Clear();
			this.ReconstructUnitsFromUnits2D();
			Action onShapeChanged = this.OnShapeChanged;
			if (onShapeChanged == null)
			{
				return;
			}
			onShapeChanged();
		}

		// Token: 0x06000EFE RID: 3838 RVA: 0x0002AB10 File Offset: 0x00028D10
		protected virtual bool IsUnitPositionRestrained(int fileIndex, int rankIndex)
		{
			if (this._isMiddleFrontUnitPositionReserved)
			{
				Vec2i middleFrontUnitPosition = this.GetMiddleFrontUnitPosition();
				return fileIndex == middleFrontUnitPosition.Item1 && rankIndex == middleFrontUnitPosition.Item2;
			}
			return false;
		}

		// Token: 0x06000EFF RID: 3839 RVA: 0x0002AB48 File Offset: 0x00028D48
		protected virtual void MakeRestrainedPositionsUnavailable()
		{
			if (this._isMiddleFrontUnitPositionReserved)
			{
				Vec2i middleFrontUnitPosition = this.GetMiddleFrontUnitPosition();
				this.UnitPositionAvailabilities[middleFrontUnitPosition.Item1, middleFrontUnitPosition.Item2] = 1;
			}
		}

		// Token: 0x06000F00 RID: 3840 RVA: 0x0002AB7E File Offset: 0x00028D7E
		public bool IsUnitPositionAvailable(int fileIndex, int rankIndex)
		{
			return this.UnitPositionAvailabilities[fileIndex, rankIndex] == 2;
		}

		// Token: 0x06000F01 RID: 3841 RVA: 0x0002AB90 File Offset: 0x00028D90
		private Vec2i GetNearestAvailableNeighbourPositionIndex(int fileIndex, int rankIndex)
		{
			for (int i = 1; i < this.FileCount + this.RankCount; i++)
			{
				bool flag = true;
				bool flag2 = true;
				bool flag3 = true;
				bool flag4 = true;
				int num = 0;
				int num2 = 0;
				while (num2 <= i && (flag || flag2 || flag3 || flag4))
				{
					int num3 = i - num2;
					num = num2;
					int num4 = fileIndex - num2;
					int num5 = fileIndex + num2;
					int num6 = rankIndex - num3;
					int num7 = rankIndex + num3;
					if (flag && (num4 < 0 || num6 < 0))
					{
						flag = false;
					}
					if (flag3 && (num4 < 0 || num7 >= this.RankCount))
					{
						flag3 = false;
					}
					if (flag2 && (num5 >= this.FileCount || num6 < 0))
					{
						flag2 = false;
					}
					if (flag4 && (num5 >= this.FileCount || num7 >= this.RankCount))
					{
						flag4 = false;
					}
					if (flag && this.UnitPositionAvailabilities[num4, num6] == 2)
					{
						return new Vec2i(num4, num6);
					}
					if (flag3 && this.UnitPositionAvailabilities[num4, num7] == 2)
					{
						return new Vec2i(num4, num7);
					}
					if (flag2 && this.UnitPositionAvailabilities[num5, num6] == 2)
					{
						return new Vec2i(num5, num6);
					}
					if (flag4 && this.UnitPositionAvailabilities[num5, num7] == 2)
					{
						return new Vec2i(num5, num7);
					}
					num2++;
				}
				flag2 = (flag = (flag3 = (flag4 = true)));
				int num8 = 0;
				while (num8 < i - num && (flag || flag2 || flag3 || flag4))
				{
					int num9 = i - num8;
					int num10 = fileIndex - num9;
					int num11 = fileIndex + num9;
					int num12 = rankIndex - num8;
					int num13 = rankIndex + num8;
					if (flag && (num10 < 0 || num12 < 0))
					{
						flag = false;
					}
					if (flag3 && (num10 < 0 || num13 >= this.RankCount))
					{
						flag3 = false;
					}
					if (flag2 && (num11 >= this.FileCount || num12 < 0))
					{
						flag2 = false;
					}
					if (flag4 && (num11 >= this.FileCount || num13 >= this.RankCount))
					{
						flag4 = false;
					}
					if (flag && this.UnitPositionAvailabilities[num10, num12] == 2)
					{
						return new Vec2i(num10, num12);
					}
					if (flag3 && this.UnitPositionAvailabilities[num10, num13] == 2)
					{
						return new Vec2i(num10, num13);
					}
					if (flag2 && this.UnitPositionAvailabilities[num11, num12] == 2)
					{
						return new Vec2i(num11, num12);
					}
					if (flag4 && this.UnitPositionAvailabilities[num11, num13] == 2)
					{
						return new Vec2i(num11, num13);
					}
					num8++;
				}
			}
			return LineFormation.InvalidPositionIndex;
		}

		// Token: 0x06000F02 RID: 3842 RVA: 0x0002ADF8 File Offset: 0x00028FF8
		private bool GetNextVacancy(out int fileIndex, out int rankIndex)
		{
			int num = this.FileCount * this.RankCount;
			for (int i = 0; i < num; i++)
			{
				Vec2i orderedUnitPositionIndex = this.GetOrderedUnitPositionIndex(i);
				fileIndex = orderedUnitPositionIndex.Item1;
				rankIndex = orderedUnitPositionIndex.Item2;
				if (this._units2D[fileIndex, rankIndex] == null && this.IsUnitPositionAvailable(fileIndex, rankIndex))
				{
					return true;
				}
			}
			fileIndex = -1;
			rankIndex = -1;
			return false;
		}

		// Token: 0x06000F03 RID: 3843 RVA: 0x0002AE60 File Offset: 0x00029060
		private IFormationUnit GetLastUnit()
		{
			int num = -1;
			IFormationUnit formationUnit = null;
			foreach (IFormationUnit formationUnit2 in this._allUnits)
			{
				int formationFileIndex = formationUnit2.FormationFileIndex;
				int formationRankIndex = formationUnit2.FormationRankIndex;
				int num2 = formationFileIndex + formationRankIndex;
				if (num2 > num)
				{
					num = num2;
					formationUnit = formationUnit2;
				}
			}
			return formationUnit;
		}

		// Token: 0x06000F04 RID: 3844 RVA: 0x0002AED4 File Offset: 0x000290D4
		private static Vec2i GetOrderedUnitPositionIndexAux(int fileIndexBegin, int fileIndexEnd, int rankIndexBegin, int rankIndexEnd, int unitIndex)
		{
			int num = fileIndexEnd - fileIndexBegin + 1;
			int num2 = unitIndex / num;
			int num3 = unitIndex - num2 * num;
			if (num % 2 == 1)
			{
				num3 = num / 2 + ((num3 % 2 == 0) ? 1 : (-1)) * (num3 + 1) / 2;
			}
			else
			{
				num3 = num / 2 - 1 + ((num3 % 2 == 0) ? (-1) : 1) * (num3 + 1) / 2;
			}
			return new Vec2i(num3 + fileIndexBegin, num2 + rankIndexBegin);
		}

		// Token: 0x06000F05 RID: 3845 RVA: 0x0002AF30 File Offset: 0x00029130
		private Vec2i GetOrderedUnitPositionIndex(int unitIndex)
		{
			return LineFormation.GetOrderedUnitPositionIndexAux(0, this.FileCount - 1, 0, this.RankCount - 1, unitIndex);
		}

		// Token: 0x06000F06 RID: 3846 RVA: 0x0002AF4A File Offset: 0x0002914A
		private static IEnumerable<Vec2i> GetOrderedUnitPositionIndicesAux(int fileIndexBegin, int fileIndexEnd, int rankIndexBegin, int rankIndexEnd)
		{
			int fileCount = fileIndexEnd - fileIndexBegin + 1;
			if (fileCount % 2 == 1)
			{
				int centerFileIndex = fileCount / 2;
				int num;
				for (int rankIndex = rankIndexBegin; rankIndex <= rankIndexEnd; rankIndex = num + 1)
				{
					yield return new Vec2i(fileIndexBegin + centerFileIndex, rankIndex);
					for (int fileIndexOffset = 1; fileIndexOffset <= centerFileIndex; fileIndexOffset = num + 1)
					{
						yield return new Vec2i(fileIndexBegin + centerFileIndex - fileIndexOffset, rankIndex);
						if (centerFileIndex + fileIndexOffset < fileCount)
						{
							yield return new Vec2i(fileIndexBegin + centerFileIndex + fileIndexOffset, rankIndex);
						}
						num = fileIndexOffset;
					}
					num = rankIndex;
				}
			}
			else
			{
				int centerFileIndex = fileCount / 2 - 1;
				int num;
				for (int rankIndex = rankIndexBegin; rankIndex <= rankIndexEnd; rankIndex = num + 1)
				{
					yield return new Vec2i(fileIndexBegin + centerFileIndex, rankIndex);
					for (int fileIndexOffset = 1; fileIndexOffset <= centerFileIndex + 1; fileIndexOffset = num + 1)
					{
						yield return new Vec2i(fileIndexBegin + centerFileIndex + fileIndexOffset, rankIndex);
						if (centerFileIndex - fileIndexOffset >= 0)
						{
							yield return new Vec2i(fileIndexBegin + centerFileIndex - fileIndexOffset, rankIndex);
						}
						num = fileIndexOffset;
					}
					num = rankIndex;
				}
			}
			yield break;
		}

		// Token: 0x06000F07 RID: 3847 RVA: 0x0002AF6F File Offset: 0x0002916F
		private IEnumerable<Vec2i> GetOrderedUnitPositionIndices()
		{
			return LineFormation.GetOrderedUnitPositionIndicesAux(0, this.FileCount - 1, 0, this.RankCount - 1);
		}

		// Token: 0x06000F08 RID: 3848 RVA: 0x0002AF88 File Offset: 0x00029188
		public Vec2? GetLocalPositionOfUnitOrDefault(int unitIndex)
		{
			Vec2i vec2i = ((unitIndex < this._cachedOrderedAndAvailableUnitPositionIndices.Count) ? this._cachedOrderedAndAvailableUnitPositionIndices.ElementAt(unitIndex) : LineFormation.InvalidPositionIndex);
			Vec2? vec;
			if (vec2i != LineFormation.InvalidPositionIndex)
			{
				int item = vec2i.Item1;
				int item2 = vec2i.Item2;
				vec = new Vec2?(this.GetLocalPositionOfUnit(item, item2));
			}
			else
			{
				vec = null;
			}
			return vec;
		}

		// Token: 0x06000F09 RID: 3849 RVA: 0x0002AFEE File Offset: 0x000291EE
		public Vec2? GetLocalDirectionOfUnitOrDefault(int unitIndex)
		{
			return new Vec2?(Vec2.Forward);
		}

		// Token: 0x06000F0A RID: 3850 RVA: 0x0002AFFC File Offset: 0x000291FC
		public WorldPosition? GetWorldPositionOfUnitOrDefault(int unitIndex)
		{
			Vec2i vec2i = ((unitIndex < this._cachedOrderedAndAvailableUnitPositionIndices.Count) ? this._cachedOrderedAndAvailableUnitPositionIndices.ElementAt(unitIndex) : LineFormation.InvalidPositionIndex);
			WorldPosition? worldPosition;
			if (vec2i != LineFormation.InvalidPositionIndex)
			{
				int item = vec2i.Item1;
				int item2 = vec2i.Item2;
				worldPosition = new WorldPosition?(this._globalPositions[item, item2]);
			}
			else
			{
				worldPosition = null;
			}
			return worldPosition;
		}

		// Token: 0x06000F0B RID: 3851 RVA: 0x0002B067 File Offset: 0x00029267
		public IEnumerable<Vec2> GetUnavailableUnitPositions()
		{
			int num;
			for (int fileIndex = 0; fileIndex < this.FileCount; fileIndex = num + 1)
			{
				for (int rankIndex = 0; rankIndex < this.RankCount; rankIndex = num + 1)
				{
					if (this.UnitPositionAvailabilities[fileIndex, rankIndex] == 1 && !this.IsUnitPositionRestrained(fileIndex, rankIndex))
					{
						yield return this.GetLocalPositionOfUnit(fileIndex, rankIndex);
					}
					num = rankIndex;
				}
				num = fileIndex;
			}
			yield break;
		}

		// Token: 0x06000F0C RID: 3852 RVA: 0x0002B077 File Offset: 0x00029277
		private void InsertUnit(IFormationUnit unit, int fileIndex, int rankIndex)
		{
			unit.FormationFileIndex = fileIndex;
			unit.FormationRankIndex = rankIndex;
			this._units2D[fileIndex, rankIndex] = unit;
			this.ReconstructUnitsFromUnits2D();
			Action onShapeChanged = this.OnShapeChanged;
			if (onShapeChanged == null)
			{
				return;
			}
			onShapeChanged();
		}

		// Token: 0x06000F0D RID: 3853 RVA: 0x0002B0AC File Offset: 0x000292AC
		public bool AddUnit(IFormationUnit unit)
		{
			bool flag = false;
			while (!flag && !this.AreLastRanksCompletelyUnavailable(3))
			{
				int num;
				int num2;
				if (this.GetNextVacancy(out num, out num2))
				{
					unit.FormationFileIndex = num;
					unit.FormationRankIndex = num2;
					this._units2D[num, num2] = unit;
					this.ReconstructUnitsFromUnits2D();
					flag = true;
				}
				else
				{
					if (!this.IsDeepenApplicable())
					{
						break;
					}
					this.Deepen();
				}
			}
			if (!flag)
			{
				this._unpositionedUnits.Add(unit);
				this.ReconstructUnitsFromUnits2D();
			}
			if (flag)
			{
				if (this.FileCount < this.MinimumFileCount)
				{
					LineFormation.WidenFormation(this, this.MinimumFileCount - this.FileCount);
				}
				Action onShapeChanged = this.OnShapeChanged;
				if (onShapeChanged != null)
				{
					onShapeChanged();
				}
				if (unit is Agent)
				{
					bool hasMount = (unit as Agent).HasMount;
					if ((this.owner is Formation && (this.owner as Formation).CalculateHasSignificantNumberOfMounted) != this._isCavalry)
					{
						this.BatchUnitPositionAvailabilities(true);
					}
					else if (this._isCavalry != hasMount && this.owner is Formation)
					{
						(this.owner as Formation).QuerySystem.ForceExpireCavalryUnitRatio();
						if ((this.owner as Formation).CalculateHasSignificantNumberOfMounted != this._isCavalry)
						{
							this.BatchUnitPositionAvailabilities(true);
						}
					}
				}
			}
			return flag;
		}

		// Token: 0x06000F0E RID: 3854 RVA: 0x0002B1E9 File Offset: 0x000293E9
		public void RemoveUnit(IFormationUnit unit)
		{
			if (this._unpositionedUnits.Remove(unit))
			{
				this.ReconstructUnitsFromUnits2D();
				return;
			}
			this.RemoveUnit(unit, true, false);
		}

		// Token: 0x06000F0F RID: 3855 RVA: 0x0002B209 File Offset: 0x00029409
		public IFormationUnit GetUnit(int fileIndex, int rankIndex)
		{
			return this._units2D[fileIndex, rankIndex];
		}

		// Token: 0x06000F10 RID: 3856 RVA: 0x0002B21A File Offset: 0x0002941A
		public void OnBatchRemoveStart()
		{
			if (this._isBatchRemovingUnits)
			{
				return;
			}
			this._isBatchRemovingUnits = true;
			this._gapFillMinRanksPerFileForBatchRemove.Clear();
			this._batchRemoveInvolvesUnavailablePositions = false;
		}

		// Token: 0x06000F11 RID: 3857 RVA: 0x0002B240 File Offset: 0x00029440
		public void OnBatchRemoveEnd()
		{
			if (!this._isBatchRemovingUnits)
			{
				return;
			}
			if (this._gapFillMinRanksPerFileForBatchRemove.Count > 0)
			{
				for (int i = 0; i < this._gapFillMinRanksPerFileForBatchRemove.Count; i++)
				{
					int num = this._gapFillMinRanksPerFileForBatchRemove[i];
					if (i < this.FileCount && num < this.RankCount)
					{
						LineFormation.FillInTheGapsOfFile(this, i, num, true);
					}
				}
				this.FillInTheGapsOfFormationAfterRemove(this._batchRemoveInvolvesUnavailablePositions);
				this._gapFillMinRanksPerFileForBatchRemove.Clear();
			}
			this._isBatchRemovingUnits = false;
		}

		// Token: 0x06000F12 RID: 3858 RVA: 0x0002B2C0 File Offset: 0x000294C0
		public List<IFormationUnit> GetUnitsToPop(int count)
		{
			List<IFormationUnit> list = new List<IFormationUnit>();
			if (this._unpositionedUnits.Count > 0)
			{
				int num = Math.Min(count, this._unpositionedUnits.Count);
				list.AddRange(this._unpositionedUnits.Take(num));
				count -= num;
			}
			if (count > 0)
			{
				for (int i = this.FileCount * this.RankCount - 1; i >= 0; i--)
				{
					Vec2i orderedUnitPositionIndex = this.GetOrderedUnitPositionIndex(i);
					int item = orderedUnitPositionIndex.Item1;
					int item2 = orderedUnitPositionIndex.Item2;
					if (this._units2D[item, item2] != null)
					{
						list.Add(this._units2D[item, item2]);
						count--;
						if (count == 0)
						{
							break;
						}
					}
				}
			}
			return list;
		}

		// Token: 0x06000F13 RID: 3859 RVA: 0x0002B374 File Offset: 0x00029574
		private void PickUnitsWithRespectToPosition(Agent agent, float distanceSquared, ref LinkedList<Tuple<IFormationUnit, float>> collection, ref List<IFormationUnit> chosenUnits, int countToChoose, bool chooseClosest)
		{
			if (collection.Count < countToChoose)
			{
				LinkedListNode<Tuple<IFormationUnit, float>> linkedListNode = null;
				for (LinkedListNode<Tuple<IFormationUnit, float>> linkedListNode2 = collection.First; linkedListNode2 != null; linkedListNode2 = linkedListNode2.Next)
				{
					if (chooseClosest ? (linkedListNode2.Value.Item2 < distanceSquared) : (linkedListNode2.Value.Item2 > distanceSquared))
					{
						linkedListNode = linkedListNode2;
						break;
					}
				}
				if (linkedListNode != null)
				{
					collection.AddBefore(linkedListNode, new LinkedListNode<Tuple<IFormationUnit, float>>(new Tuple<IFormationUnit, float>(agent, distanceSquared)));
					return;
				}
				collection.AddLast(new LinkedListNode<Tuple<IFormationUnit, float>>(new Tuple<IFormationUnit, float>(agent, distanceSquared)));
				return;
			}
			else
			{
				if (chooseClosest ? (distanceSquared < collection.First<Tuple<IFormationUnit, float>>().Item2) : (distanceSquared > collection.First<Tuple<IFormationUnit, float>>().Item2))
				{
					LinkedListNode<Tuple<IFormationUnit, float>> linkedListNode3 = null;
					for (LinkedListNode<Tuple<IFormationUnit, float>> linkedListNode4 = collection.First.Next; linkedListNode4 != null; linkedListNode4 = linkedListNode4.Next)
					{
						if (chooseClosest ? (linkedListNode4.Value.Item2 < distanceSquared) : (linkedListNode4.Value.Item2 > distanceSquared))
						{
							linkedListNode3 = linkedListNode4;
							break;
						}
					}
					if (linkedListNode3 != null)
					{
						collection.AddBefore(linkedListNode3, new LinkedListNode<Tuple<IFormationUnit, float>>(new Tuple<IFormationUnit, float>(agent, distanceSquared)));
					}
					else
					{
						collection.AddLast(new LinkedListNode<Tuple<IFormationUnit, float>>(new Tuple<IFormationUnit, float>(agent, distanceSquared)));
					}
					if (!chooseClosest)
					{
						chosenUnits.Add(collection.First<Tuple<IFormationUnit, float>>().Item1);
					}
					collection.RemoveFirst();
					return;
				}
				if (!chooseClosest)
				{
					chosenUnits.Add(agent);
				}
				return;
			}
		}

		// Token: 0x06000F14 RID: 3860 RVA: 0x0002B4BB File Offset: 0x000296BB
		public IEnumerable<IFormationUnit> GetUnitsToPopWithCondition(int count, Func<IFormationUnit, bool> currentCondition)
		{
			IEnumerable<IFormationUnit> unpositionedUnits = this._unpositionedUnits;
			Func<IFormationUnit, bool> <>9__0;
			Func<IFormationUnit, bool> func;
			if ((func = <>9__0) == null)
			{
				func = (<>9__0 = (IFormationUnit uu) => currentCondition(uu));
			}
			int num;
			foreach (IFormationUnit formationUnit in unpositionedUnits.Where(func))
			{
				yield return formationUnit;
				num = count;
				count = num - 1;
				if (count == 0)
				{
					yield break;
				}
			}
			IEnumerator<IFormationUnit> enumerator = null;
			for (int i = this.FileCount * this.RankCount - 1; i >= 0; i = num - 1)
			{
				Vec2i orderedUnitPositionIndex = this.GetOrderedUnitPositionIndex(i);
				int item = orderedUnitPositionIndex.Item1;
				int item2 = orderedUnitPositionIndex.Item2;
				if (this._units2D[item, item2] != null && currentCondition(this._units2D[item, item2]))
				{
					yield return this._units2D[item, item2];
					num = count;
					count = num - 1;
					if (count == 0)
					{
						yield break;
					}
				}
				num = i;
			}
			yield break;
			yield break;
		}

		// Token: 0x06000F15 RID: 3861 RVA: 0x0002B4DC File Offset: 0x000296DC
		private void TryToKeepDepth()
		{
			if (this.FileCount > this.MinimumFileCount)
			{
				int num = this.CountUnitsAtRank(this.RankCount - 1);
				int num2 = this.RankCount - 1;
				if (num + num2 <= this.FileCount && MBRandom.RandomInt(this.RankCount * 2) == 0 && this.IsNarrowApplicable((this.FileCount > 2) ? 2 : 1))
				{
					LineFormation.NarrowFormation(this, (this.FileCount > 2) ? 2 : 1);
				}
			}
		}

		// Token: 0x06000F16 RID: 3862 RVA: 0x0002B550 File Offset: 0x00029750
		public List<IFormationUnit> GetUnitsToPop(int count, Vec3 targetPosition)
		{
			List<IFormationUnit> list = new List<IFormationUnit>();
			if (this._unpositionedUnits.Count > 0)
			{
				int num = Math.Min(count, this._unpositionedUnits.Count);
				if (num < this._unpositionedUnits.Count)
				{
					LinkedList<Tuple<IFormationUnit, float>> linkedList = new LinkedList<Tuple<IFormationUnit, float>>();
					bool flag = (float)num <= (float)this._unpositionedUnits.Count * 0.5f;
					int num2 = (flag ? num : (this._unpositionedUnits.Count - num));
					for (int i = 0; i < this._unpositionedUnits.Count; i++)
					{
						Agent agent = this._unpositionedUnits[i] as Agent;
						if (agent == null)
						{
							if (flag)
							{
								linkedList.AddFirst(new Tuple<IFormationUnit, float>(this._unpositionedUnits[i], float.MinValue));
								if (linkedList.Count > num)
								{
									linkedList.RemoveLast();
								}
							}
							else if (linkedList.Count < num2)
							{
								linkedList.AddLast(new Tuple<IFormationUnit, float>(this._unpositionedUnits[i], float.MinValue));
							}
							else
							{
								list.Add(this._unpositionedUnits[i]);
							}
						}
						else
						{
							float num3 = agent.Position.DistanceSquared(targetPosition);
							this.PickUnitsWithRespectToPosition(agent, num3, ref linkedList, ref list, num2, flag);
						}
					}
					if (flag)
					{
						list.AddRange(linkedList.Select((Tuple<IFormationUnit, float> tuple) => tuple.Item1));
					}
					count -= num;
				}
				else
				{
					list.AddRange(this._unpositionedUnits.Take(num));
					count -= num;
				}
			}
			if (count > 0)
			{
				int num4 = count;
				int num5 = this.UnitCount - this._unpositionedUnits.Count;
				bool flag2 = num5 == num4;
				bool flag3 = (float)count <= (float)num5 * 0.5f;
				LinkedList<Tuple<IFormationUnit, float>> linkedList2 = (flag2 ? null : new LinkedList<Tuple<IFormationUnit, float>>());
				int num6 = (flag3 ? num4 : (num5 - num4));
				for (int j = this.FileCount * this.RankCount - 1; j >= 0; j--)
				{
					Vec2i orderedUnitPositionIndex = this.GetOrderedUnitPositionIndex(j);
					int item = orderedUnitPositionIndex.Item1;
					int item2 = orderedUnitPositionIndex.Item2;
					if (this._units2D[item, item2] != null)
					{
						if (flag2)
						{
							list.Add(this._units2D[item, item2]);
							count--;
							if (count == 0)
							{
								break;
							}
						}
						else
						{
							Agent agent2 = this._units2D[item, item2] as Agent;
							if (agent2 == null)
							{
								if (flag3)
								{
									linkedList2.AddFirst(new Tuple<IFormationUnit, float>(this._unpositionedUnits[j], float.MinValue));
									if (linkedList2.Count > num4)
									{
										linkedList2.RemoveLast();
									}
								}
								else if (linkedList2.Count < num6)
								{
									linkedList2.AddLast(new Tuple<IFormationUnit, float>(this._unpositionedUnits[j], float.MinValue));
								}
								else
								{
									list.Add(this._unpositionedUnits[j]);
								}
							}
							else
							{
								float num7 = agent2.Position.DistanceSquared(targetPosition);
								this.PickUnitsWithRespectToPosition(agent2, num7, ref linkedList2, ref list, num6, flag3);
							}
						}
					}
				}
				if (!flag2 && flag3)
				{
					list.AddRange(linkedList2.Select((Tuple<IFormationUnit, float> tuple) => tuple.Item1));
				}
			}
			return list;
		}

		// Token: 0x06000F17 RID: 3863 RVA: 0x0002B8A8 File Offset: 0x00029AA8
		private void RemoveUnit(IFormationUnit unit, bool fillInTheGap, bool isRemovingFromAnUnavailablePosition = false)
		{
			if (fillInTheGap)
			{
			}
			int formationFileIndex = unit.FormationFileIndex;
			int formationRankIndex = unit.FormationRankIndex;
			if (unit.FormationFileIndex < 0 || unit.FormationRankIndex < 0 || unit.FormationFileIndex >= this.FileCount || unit.FormationRankIndex >= this.RankCount)
			{
				object[] array = new object[12];
				array[0] = "Unit removed has file-rank indices: ";
				array[1] = unit.FormationFileIndex;
				array[2] = " ";
				array[3] = unit.FormationRankIndex;
				array[4] = " while line formation has file-rank counts of ";
				array[5] = this.FileCount;
				array[6] = " ";
				array[7] = this.RankCount;
				array[8] = " agent state is ";
				int num = 9;
				Agent agent = unit as Agent;
				array[num] = ((agent != null) ? new AgentState?(agent.State) : null);
				array[10] = " unit detachment is ";
				int num2 = 11;
				Agent agent2 = unit as Agent;
				array[num2] = ((agent2 != null) ? agent2.Detachment : null);
				Debug.Print(string.Concat(array), 0, Debug.DebugColor.White, 17592186044416UL);
			}
			this._units2D[unit.FormationFileIndex, unit.FormationRankIndex] = null;
			this.ReconstructUnitsFromUnits2D();
			unit.FormationFileIndex = -1;
			unit.FormationRankIndex = -1;
			if (fillInTheGap)
			{
				if (this._isBatchRemovingUnits)
				{
					int num3 = formationFileIndex - this._gapFillMinRanksPerFileForBatchRemove.Count + 1;
					for (int i = 0; i < num3; i++)
					{
						this._gapFillMinRanksPerFileForBatchRemove.Add(int.MaxValue);
					}
					this._gapFillMinRanksPerFileForBatchRemove[formationFileIndex] = MathF.Min(formationRankIndex, this._gapFillMinRanksPerFileForBatchRemove[formationFileIndex]);
					this._batchRemoveInvolvesUnavailablePositions = this._batchRemoveInvolvesUnavailablePositions || isRemovingFromAnUnavailablePosition;
				}
				else
				{
					LineFormation.FillInTheGapsOfFile(this, formationFileIndex, formationRankIndex, true);
					this.FillInTheGapsOfFormationAfterRemove(isRemovingFromAnUnavailablePosition);
				}
			}
			Action onShapeChanged = this.OnShapeChanged;
			if (onShapeChanged == null)
			{
				return;
			}
			onShapeChanged();
		}

		// Token: 0x06000F18 RID: 3864 RVA: 0x0002BA74 File Offset: 0x00029C74
		protected virtual bool TryGetUnitPositionIndexFromLocalPosition(Vec2 localPosition, out int fileIndex, out int rankIndex)
		{
			rankIndex = MathF.Round(-localPosition.y / (this.Distance + this.UnitDiameter));
			if (rankIndex >= this.RankCount)
			{
				fileIndex = -1;
				return false;
			}
			if (this.IsStaggered && rankIndex % 2 == 1)
			{
				localPosition.x -= (this.Interval + this.UnitDiameter) * 0.5f;
			}
			float num = (float)(this.FileCount - 1) * (this.Interval + this.UnitDiameter);
			fileIndex = MathF.Round((localPosition.x + num / 2f) / (this.Interval + this.UnitDiameter));
			return fileIndex >= 0 && fileIndex < this.FileCount;
		}

		// Token: 0x06000F19 RID: 3865 RVA: 0x0002BB28 File Offset: 0x00029D28
		protected virtual Vec2 GetLocalPositionOfUnit(int fileIndex, int rankIndex)
		{
			float num = (float)(this.FileCount - 1) * (this.Interval + this.UnitDiameter);
			Vec2 vec = new Vec2((float)fileIndex * (this.Interval + this.UnitDiameter) - num / 2f, (float)(-(float)rankIndex) * (this.Distance + this.UnitDiameter));
			if (this.IsStaggered && rankIndex % 2 == 1)
			{
				vec.x += (this.Interval + this.UnitDiameter) * 0.5f;
			}
			return vec;
		}

		// Token: 0x06000F1A RID: 3866 RVA: 0x0002BBAC File Offset: 0x00029DAC
		protected virtual Vec2 GetLocalPositionOfUnitWithAdjustment(int fileIndex, int rankIndex, float distanceBetweenAgentsAdjustment)
		{
			float num = this.Interval + distanceBetweenAgentsAdjustment;
			float num2 = (float)(this.FileCount - 1) * (num + this.UnitDiameter);
			Vec2 vec = new Vec2((float)fileIndex * (num + this.UnitDiameter) - num2 / 2f, (float)(-(float)rankIndex) * (this.Distance + this.UnitDiameter));
			if (this.IsStaggered && rankIndex % 2 == 1)
			{
				vec.x += (num + this.UnitDiameter) * 0.5f;
			}
			return vec;
		}

		// Token: 0x06000F1B RID: 3867 RVA: 0x0002BC28 File Offset: 0x00029E28
		protected virtual Vec2 GetLocalDirectionOfUnit(int fileIndex, int rankIndex)
		{
			return Vec2.Forward;
		}

		// Token: 0x06000F1C RID: 3868 RVA: 0x0002BC30 File Offset: 0x00029E30
		public Vec2? GetLocalPositionOfUnitOrDefault(IFormationUnit unit)
		{
			if (this._unpositionedUnits.Contains(unit))
			{
				return null;
			}
			return new Vec2?(this.GetLocalPositionOfUnit(unit.FormationFileIndex, unit.FormationRankIndex));
		}

		// Token: 0x06000F1D RID: 3869 RVA: 0x0002BC6C File Offset: 0x00029E6C
		public Vec2? GetLocalPositionOfUnitOrDefaultWithAdjustment(IFormationUnit unit, float distanceBetweenAgentsAdjustment)
		{
			if (this._unpositionedUnits.Contains(unit))
			{
				return null;
			}
			return new Vec2?(this.GetLocalPositionOfUnitWithAdjustment(unit.FormationFileIndex, unit.FormationRankIndex, distanceBetweenAgentsAdjustment));
		}

		// Token: 0x06000F1E RID: 3870 RVA: 0x0002BCA9 File Offset: 0x00029EA9
		public Vec2? GetLocalDirectionOfUnitOrDefault(IFormationUnit unit)
		{
			return new Vec2?(Vec2.Forward);
		}

		// Token: 0x06000F1F RID: 3871 RVA: 0x0002BCB8 File Offset: 0x00029EB8
		public WorldPosition? GetWorldPositionOfUnitOrDefault(IFormationUnit unit)
		{
			if (this._unpositionedUnits.Contains(unit))
			{
				return null;
			}
			return new WorldPosition?(this._globalPositions[unit.FormationFileIndex, unit.FormationRankIndex]);
		}

		// Token: 0x06000F20 RID: 3872 RVA: 0x0002BCFC File Offset: 0x00029EFC
		private void ReconstructUnitsFromUnits2D()
		{
			if (this._allUnits == null)
			{
				this._allUnits = new MBList<IFormationUnit>();
			}
			this._allUnits.Clear();
			for (int i = 0; i < this._units2D.Count1; i++)
			{
				for (int j = 0; j < this._units2D.Count2; j++)
				{
					if (this._units2D[i, j] != null)
					{
						this._allUnits.Add(this._units2D[i, j]);
					}
				}
			}
			for (int k = 0; k < this._unpositionedUnits.Count; k++)
			{
				this._allUnits.Add(this._unpositionedUnits[k]);
			}
		}

		// Token: 0x06000F21 RID: 3873 RVA: 0x0002BDA7 File Offset: 0x00029FA7
		private void FillInTheGapsOfFormationAfterRemove(bool hasUnavailablePositions)
		{
			this.TryReaddingUnpositionedUnits();
			LineFormation.FillInTheGapsOfMiddleRanks(this, null);
			this.TryToKeepDepth();
		}

		// Token: 0x06000F22 RID: 3874 RVA: 0x0002BDBF File Offset: 0x00029FBF
		private static void WidenFormation(LineFormation formation, int fileCountFromBothFlanks)
		{
			if (fileCountFromBothFlanks % 2 == 0)
			{
				LineFormation.WidenFormation(formation, fileCountFromBothFlanks / 2, fileCountFromBothFlanks / 2);
				return;
			}
			if (formation.FileCount % 2 == 0)
			{
				LineFormation.WidenFormation(formation, fileCountFromBothFlanks / 2 + 1, fileCountFromBothFlanks / 2);
				return;
			}
			LineFormation.WidenFormation(formation, fileCountFromBothFlanks / 2, fileCountFromBothFlanks / 2 + 1);
		}

		// Token: 0x06000F23 RID: 3875 RVA: 0x0002BDFC File Offset: 0x00029FFC
		private static void WidenFormation(LineFormation formation, int fileCountFromLeftFlank, int fileCountFromRightFlank)
		{
			formation._units2DWorkspace.ResetWithNewCount(formation.FileCount + fileCountFromLeftFlank + fileCountFromRightFlank, formation.RankCount);
			formation._unitPositionAvailabilitiesWorkspace.ResetWithNewCount(formation.FileCount + fileCountFromLeftFlank + fileCountFromRightFlank, formation.RankCount);
			formation._globalPositionsWorkspace.ResetWithNewCount(formation.FileCount + fileCountFromLeftFlank + fileCountFromRightFlank, formation.RankCount);
			for (int i = 0; i < formation.FileCount; i++)
			{
				int num = i + fileCountFromLeftFlank;
				formation._units2D.CopyRowTo(i, 0, formation._units2DWorkspace, num, 0, formation.RankCount);
				formation.UnitPositionAvailabilities.CopyRowTo(i, 0, formation._unitPositionAvailabilitiesWorkspace, num, 0, formation.RankCount);
				formation._globalPositions.CopyRowTo(i, 0, formation._globalPositionsWorkspace, num, 0, formation.RankCount);
				if (fileCountFromLeftFlank > 0)
				{
					for (int j = 0; j < formation.RankCount; j++)
					{
						if (formation._units2D[i, j] != null)
						{
							formation._units2D[i, j].FormationFileIndex += fileCountFromLeftFlank;
						}
					}
				}
			}
			MBList2D<IFormationUnit> units2D = formation._units2D;
			formation._units2D = formation._units2DWorkspace;
			formation._units2DWorkspace = units2D;
			formation.ReconstructUnitsFromUnits2D();
			MBList2D<int> unitPositionAvailabilities = formation.UnitPositionAvailabilities;
			formation.UnitPositionAvailabilities = formation._unitPositionAvailabilitiesWorkspace;
			formation._unitPositionAvailabilitiesWorkspace = unitPositionAvailabilities;
			MBList2D<WorldPosition> globalPositions = formation._globalPositions;
			formation._globalPositions = formation._globalPositionsWorkspace;
			formation._globalPositionsWorkspace = globalPositions;
			formation.BatchUnitPositionAvailabilities(true);
			if (formation._isDeformingOnWidthChange || (fileCountFromLeftFlank + fileCountFromRightFlank) % 2 == 1)
			{
				formation.OnFormationFrameChanged();
			}
			else
			{
				LineFormation.ShiftUnitsForwardsForWideningFormation(formation);
				formation.TryReaddingUnpositionedUnits();
				while (formation.RankCount > 1 && formation.IsRankEmpty(formation.RankCount - 1))
				{
					formation.Shorten();
				}
			}
			Action onShapeChanged = formation.OnShapeChanged;
			if (onShapeChanged == null)
			{
				return;
			}
			onShapeChanged();
		}

		// Token: 0x06000F24 RID: 3876 RVA: 0x0002BFC0 File Offset: 0x0002A1C0
		private static void GetToBeFilledInAndToBeEmptiedOutUnitPositions(LineFormation formation, MBQueue<Vec2i> toBeFilledInUnitPositions, MBArrayList<Vec2i> toBeEmptiedOutUnitPositions)
		{
			int num = 0;
			int num2 = formation.FileCount * formation.RankCount - 1;
			for (;;)
			{
				Vec2i orderedUnitPositionIndex = formation.GetOrderedUnitPositionIndex(num);
				int item = orderedUnitPositionIndex.Item1;
				int item2 = orderedUnitPositionIndex.Item2;
				Vec2i orderedUnitPositionIndex2 = formation.GetOrderedUnitPositionIndex(num2);
				int item3 = orderedUnitPositionIndex2.Item1;
				int item4 = orderedUnitPositionIndex2.Item2;
				if (item2 >= item4)
				{
					break;
				}
				if (formation._units2D[item, item2] != null || !formation.IsUnitPositionAvailable(item, item2))
				{
					num++;
				}
				else if (formation._units2D[item3, item4] == null)
				{
					num2--;
				}
				else
				{
					toBeFilledInUnitPositions.Enqueue(new Vec2i(item, item2));
					toBeEmptiedOutUnitPositions.Add(new Vec2i(item3, item4));
					num++;
					num2--;
				}
			}
		}

		// Token: 0x06000F25 RID: 3877 RVA: 0x0002C07B File Offset: 0x0002A27B
		private static Vec2i GetUnitPositionForFillInFromNearby(LineFormation formation, int relocationFileIndex, int relocationRankIndex, Func<LineFormation, int, int, bool> predicate, bool isRelocationUnavailable = false)
		{
			return LineFormation.GetUnitPositionForFillInFromNearby(formation, relocationFileIndex, relocationRankIndex, predicate, LineFormation.InvalidPositionIndex, isRelocationUnavailable);
		}

		// Token: 0x06000F26 RID: 3878 RVA: 0x0002C090 File Offset: 0x0002A290
		private static Vec2i GetUnitPositionForFillInFromNearby(LineFormation formation, int relocationFileIndex, int relocationRankIndex, Func<LineFormation, int, int, bool> predicate, Vec2i lastFinalOccupation, bool isRelocationUnavailable = false)
		{
			int fileCount = formation.FileCount;
			int rankCount = formation.RankCount;
			bool flag = relocationFileIndex >= fileCount / 2;
			if (lastFinalOccupation != LineFormation.InvalidPositionIndex)
			{
				flag = lastFinalOccupation.Item1 <= relocationFileIndex;
			}
			for (int i = 1; i <= fileCount + rankCount; i++)
			{
				for (int j = MathF.Min(i, rankCount - 1 - relocationRankIndex); j >= 0; j--)
				{
					int num = i - j;
					if (flag && relocationFileIndex - num >= 0 && predicate(formation, relocationFileIndex - num, relocationRankIndex + j))
					{
						return new Vec2i(relocationFileIndex - num, relocationRankIndex + j);
					}
					if (relocationFileIndex + num < fileCount && predicate(formation, relocationFileIndex + num, relocationRankIndex + j))
					{
						return new Vec2i(relocationFileIndex + num, relocationRankIndex + j);
					}
					if (!flag && relocationFileIndex - num >= 0 && predicate(formation, relocationFileIndex - num, relocationRankIndex + j))
					{
						return new Vec2i(relocationFileIndex - num, relocationRankIndex + j);
					}
				}
			}
			return LineFormation.InvalidPositionIndex;
		}

		// Token: 0x06000F27 RID: 3879 RVA: 0x0002C188 File Offset: 0x0002A388
		private static void ShiftUnitsForwardsForWideningFormation(LineFormation formation)
		{
			MBQueue<Vec2i> mbqueue = formation._toBeFilledInGapsWorkspace.StartUsingWorkspace();
			MBArrayList<Vec2i> mbarrayList = formation._finalVacanciesWorkspace.StartUsingWorkspace();
			MBArrayList<Vec2i> mbarrayList2 = formation._filledInGapsWorkspace.StartUsingWorkspace();
			LineFormation.GetToBeFilledInAndToBeEmptiedOutUnitPositions(formation, mbqueue, mbarrayList);
			if (formation._shiftUnitsForwardsPredicateDelegate == null)
			{
				formation._shiftUnitsForwardsPredicateDelegate = new Func<LineFormation, int, int, bool>(LineFormation.<>c.<>9.<ShiftUnitsForwardsForWideningFormation>g__ShiftUnitForwardsPredicate|126_0);
			}
			while (mbqueue.Count > 0)
			{
				Vec2i vec2i = mbqueue.Dequeue();
				Vec2i unitPositionForFillInFromNearby = LineFormation.GetUnitPositionForFillInFromNearby(formation, vec2i.Item1, vec2i.Item2, formation._shiftUnitsForwardsPredicateDelegate, false);
				if (unitPositionForFillInFromNearby != LineFormation.InvalidPositionIndex)
				{
					int item = unitPositionForFillInFromNearby.Item1;
					int item2 = unitPositionForFillInFromNearby.Item2;
					IFormationUnit formationUnit = formation._units2D[item, item2];
					formation.RelocateUnit(formationUnit, vec2i.Item1, vec2i.Item2);
					mbarrayList2.Add(vec2i);
					Vec2i vec2i2 = new Vec2i(item, item2);
					if (!mbarrayList.Contains(vec2i2))
					{
						mbqueue.Enqueue(vec2i2);
					}
				}
			}
			formation._toBeFilledInGapsWorkspace.StopUsingWorkspace();
			formation._finalVacanciesWorkspace.StopUsingWorkspace();
			formation._filledInGapsWorkspace.StopUsingWorkspace();
		}

		// Token: 0x06000F28 RID: 3880 RVA: 0x0002C2A4 File Offset: 0x0002A4A4
		private static void DeepenFormation(LineFormation formation, int rankCountFromFront, int rankCountFromRear)
		{
			formation._units2DWorkspace.ResetWithNewCount(formation.FileCount, formation.RankCount + rankCountFromFront + rankCountFromRear);
			formation._unitPositionAvailabilitiesWorkspace.ResetWithNewCount(formation.FileCount, formation.RankCount + rankCountFromFront + rankCountFromRear);
			formation._globalPositionsWorkspace.ResetWithNewCount(formation.FileCount, formation.RankCount + rankCountFromFront + rankCountFromRear);
			for (int i = 0; i < formation.FileCount; i++)
			{
				formation._units2D.CopyRowTo(i, 0, formation._units2DWorkspace, i, rankCountFromFront, formation.RankCount);
				formation.UnitPositionAvailabilities.CopyRowTo(i, 0, formation._unitPositionAvailabilitiesWorkspace, i, rankCountFromFront, formation.RankCount);
				formation._globalPositions.CopyRowTo(i, 0, formation._globalPositionsWorkspace, i, rankCountFromFront, formation.RankCount);
				if (rankCountFromFront > 0)
				{
					for (int j = 0; j < formation.RankCount; j++)
					{
						if (formation._units2D[i, j] != null)
						{
							formation._units2D[i, j].FormationRankIndex += rankCountFromFront;
						}
					}
				}
			}
			MBList2D<IFormationUnit> units2D = formation._units2D;
			formation._units2D = formation._units2DWorkspace;
			formation._units2DWorkspace = units2D;
			formation.ReconstructUnitsFromUnits2D();
			MBList2D<int> unitPositionAvailabilities = formation.UnitPositionAvailabilities;
			formation.UnitPositionAvailabilities = formation._unitPositionAvailabilitiesWorkspace;
			formation._unitPositionAvailabilitiesWorkspace = unitPositionAvailabilities;
			MBList2D<WorldPosition> globalPositions = formation._globalPositions;
			formation._globalPositions = formation._globalPositionsWorkspace;
			formation._globalPositionsWorkspace = globalPositions;
			formation.BatchUnitPositionAvailabilities(true);
			Action onShapeChanged = formation.OnShapeChanged;
			if (onShapeChanged == null)
			{
				return;
			}
			onShapeChanged();
		}

		// Token: 0x06000F29 RID: 3881 RVA: 0x0002C419 File Offset: 0x0002A619
		protected virtual bool IsDeepenApplicable()
		{
			return true;
		}

		// Token: 0x06000F2A RID: 3882 RVA: 0x0002C41C File Offset: 0x0002A61C
		private void Deepen()
		{
			LineFormation.DeepenFormation(this, 0, 1);
		}

		// Token: 0x06000F2B RID: 3883 RVA: 0x0002C428 File Offset: 0x0002A628
		private static bool DeepenForVacancy(LineFormation formation, int requestedVacancyCount, int fileOffsetFromLeftFlank, int fileOffsetFromRightFlank)
		{
			int num = 0;
			bool? flag = null;
			while (flag == null)
			{
				int num2 = formation.RankCount - 1;
				for (int i = fileOffsetFromLeftFlank; i < formation.FileCount - fileOffsetFromRightFlank; i++)
				{
					if (formation._units2D[i, num2] == null && formation.IsUnitPositionAvailable(i, num2))
					{
						num++;
					}
				}
				if (num >= requestedVacancyCount)
				{
					flag = new bool?(true);
				}
				else if (!formation.AreLastRanksCompletelyUnavailable(3))
				{
					if (formation.IsDeepenApplicable())
					{
						formation.Deepen();
					}
					else
					{
						flag = new bool?(false);
					}
				}
				else
				{
					flag = new bool?(false);
				}
			}
			return flag.Value;
		}

		// Token: 0x06000F2C RID: 3884 RVA: 0x0002C4C3 File Offset: 0x0002A6C3
		protected virtual bool IsNarrowApplicable(int amount)
		{
			return true;
		}

		// Token: 0x06000F2D RID: 3885 RVA: 0x0002C4C8 File Offset: 0x0002A6C8
		private static void NarrowFormation(LineFormation formation, int fileCountFromBothFlanks)
		{
			int num = fileCountFromBothFlanks / 2;
			int num2 = fileCountFromBothFlanks / 2;
			if (fileCountFromBothFlanks % 2 != 0)
			{
				if (formation.FileCount % 2 == 0)
				{
					num2++;
				}
				else
				{
					num++;
				}
			}
			if (formation.IsNarrowApplicable(num + num2))
			{
				LineFormation.NarrowFormation(formation, num, num2);
			}
		}

		// Token: 0x06000F2E RID: 3886 RVA: 0x0002C50C File Offset: 0x0002A70C
		private static bool ShiftUnitsBackwardsForNewUnavailableUnitPositions(LineFormation formation)
		{
			MBArrayList<Vec2i> mbarrayList = formation._toBeEmptiedOutUnitPositionsWorkspace.StartUsingWorkspace();
			for (int i = 0; i < formation.FileCount * formation.RankCount; i++)
			{
				Vec2i orderedUnitPositionIndex = formation.GetOrderedUnitPositionIndex(i);
				if (formation._units2D[orderedUnitPositionIndex.Item1, orderedUnitPositionIndex.Item2] != null && !formation.IsUnitPositionAvailable(orderedUnitPositionIndex.Item1, orderedUnitPositionIndex.Item2))
				{
					mbarrayList.Add(orderedUnitPositionIndex);
				}
			}
			bool flag = mbarrayList.Count > 0;
			if (flag)
			{
				MBQueue<ValueTuple<IFormationUnit, int, int>> mbqueue = formation._displacedUnitsWorkspace.StartUsingWorkspace();
				for (int j = mbarrayList.Count - 1; j >= 0; j--)
				{
					Vec2i vec2i = mbarrayList[j];
					IFormationUnit formationUnit = formation._units2D[vec2i.Item1, vec2i.Item2];
					if (formationUnit != null)
					{
						formation.RemoveUnit(formationUnit, false, true);
						mbqueue.Enqueue(ValueTuple.Create<IFormationUnit, int, int>(formationUnit, vec2i.Item1, vec2i.Item2));
					}
				}
				LineFormation.DeepenForVacancy(formation, mbqueue.Count, 0, 0);
				MBArrayList<Vec2i> mbarrayList2 = formation._finalOccupationsWorkspace.StartUsingWorkspace();
				int num = 0;
				int num2 = 0;
				while (num2 < formation.FileCount * formation.RankCount && num < mbqueue.Count)
				{
					Vec2i orderedUnitPositionIndex2 = formation.GetOrderedUnitPositionIndex(num2);
					if (formation._units2D[orderedUnitPositionIndex2.Item1, orderedUnitPositionIndex2.Item2] == null && formation.IsUnitPositionAvailable(orderedUnitPositionIndex2.Item1, orderedUnitPositionIndex2.Item2))
					{
						mbarrayList2.Add(orderedUnitPositionIndex2);
						num++;
					}
					num2++;
				}
				LineFormation.ShiftUnitsBackwardsAux(formation, mbqueue, mbarrayList2);
				formation._displacedUnitsWorkspace.StopUsingWorkspace();
				formation._finalOccupationsWorkspace.StopUsingWorkspace();
			}
			formation._toBeEmptiedOutUnitPositionsWorkspace.StopUsingWorkspace();
			return flag;
		}

		// Token: 0x06000F2F RID: 3887 RVA: 0x0002C6C0 File Offset: 0x0002A8C0
		private static void ShiftUnitsBackwardsForNarrowingFormation(LineFormation formation, int fileCountFromLeftFlank, int fileCountFromRightFlank)
		{
			MBQueue<ValueTuple<IFormationUnit, int, int>> mbqueue = formation._displacedUnitsWorkspace.StartUsingWorkspace();
			foreach (Vec2i vec2i in (from p in formation.GetOrderedUnitPositionIndices()
				where p.Item1 < fileCountFromLeftFlank || p.Item1 >= formation.FileCount - fileCountFromRightFlank
				select p).Reverse<Vec2i>())
			{
				IFormationUnit formationUnit = formation._units2D[vec2i.Item1, vec2i.Item2];
				if (formationUnit != null)
				{
					formation.RemoveUnit(formationUnit, false, false);
					mbqueue.Enqueue(ValueTuple.Create<IFormationUnit, int, int>(formationUnit, vec2i.Item1, vec2i.Item2));
				}
			}
			LineFormation.DeepenForVacancy(formation, mbqueue.Count, fileCountFromLeftFlank, fileCountFromRightFlank);
			IEnumerable<Vec2i> enumerable = (from p in LineFormation.GetOrderedUnitPositionIndicesAux(fileCountFromLeftFlank, formation.FileCount - 1 - fileCountFromRightFlank, 0, formation.RankCount - 1)
				where formation._units2D[p.Item1, p.Item2] == null && formation.IsUnitPositionAvailable(p.Item1, p.Item2)
				select p).Take(mbqueue.Count);
			MBArrayList<Vec2i> mbarrayList = formation._finalOccupationsWorkspace.StartUsingWorkspace();
			mbarrayList.AddRange(enumerable);
			LineFormation.ShiftUnitsBackwardsAux(formation, mbqueue, mbarrayList);
			formation._displacedUnitsWorkspace.StopUsingWorkspace();
			formation._finalOccupationsWorkspace.StopUsingWorkspace();
		}

		// Token: 0x06000F30 RID: 3888 RVA: 0x0002C850 File Offset: 0x0002AA50
		private static void ShiftUnitsBackwardsAux(LineFormation formation, MBQueue<ValueTuple<IFormationUnit, int, int>> displacedUnits, MBArrayList<Vec2i> finalOccupations)
		{
			MBArrayList<Vec2i> mbarrayList = formation._filledInUnitPositionsWorkspace.StartUsingWorkspace();
			if (formation._shiftUnitsBackwardsPredicateDelegate == null)
			{
				formation._shiftUnitsBackwardsPredicateDelegate = new Func<LineFormation, int, int, bool>(LineFormation.<>c.<>9.<ShiftUnitsBackwardsAux>g__ShiftUnitsBackwardsPredicate|135_0);
			}
			while (!displacedUnits.IsEmpty<ValueTuple<IFormationUnit, int, int>>())
			{
				ValueTuple<IFormationUnit, int, int> valueTuple = displacedUnits.Dequeue();
				IFormationUnit item = valueTuple.Item1;
				int item2 = valueTuple.Item2;
				int item3 = valueTuple.Item3;
				Vec2i unitPositionForFillInFromNearby = LineFormation.GetUnitPositionForFillInFromNearby(formation, item2, item3, formation._shiftUnitsBackwardsPredicateDelegate, (finalOccupations.Count == 1) ? finalOccupations[0] : LineFormation.InvalidPositionIndex, true);
				if (unitPositionForFillInFromNearby != LineFormation.InvalidPositionIndex)
				{
					IFormationUnit formationUnit = formation._units2D[unitPositionForFillInFromNearby.Item1, unitPositionForFillInFromNearby.Item2];
					if (formationUnit != null)
					{
						formation.RemoveUnit(formationUnit, false, false);
						displacedUnits.Enqueue(ValueTuple.Create<IFormationUnit, int, int>(formationUnit, unitPositionForFillInFromNearby.Item1, unitPositionForFillInFromNearby.Item2));
					}
					mbarrayList.Add(unitPositionForFillInFromNearby);
					formation.InsertUnit(item, unitPositionForFillInFromNearby.Item1, unitPositionForFillInFromNearby.Item2);
				}
				else
				{
					float num = float.MaxValue;
					Vec2i vec2i = LineFormation.InvalidPositionIndex;
					for (int i = 0; i < finalOccupations.Count; i++)
					{
						if (mbarrayList.IndexOf(finalOccupations[i]) < 0)
						{
							float num2 = (float)(MathF.Abs(finalOccupations[i].Item1 - item2) + MathF.Abs(finalOccupations[i].Item2 - item3));
							if (num2 < num)
							{
								num = num2;
								vec2i = finalOccupations[i];
							}
						}
					}
					if (vec2i != LineFormation.InvalidPositionIndex)
					{
						mbarrayList.Add(vec2i);
						formation.InsertUnit(item, vec2i.Item1, vec2i.Item2);
					}
					else
					{
						formation._unpositionedUnits.Add(item);
						formation.ReconstructUnitsFromUnits2D();
					}
				}
			}
			formation._filledInUnitPositionsWorkspace.StopUsingWorkspace();
		}

		// Token: 0x06000F31 RID: 3889 RVA: 0x0002CA16 File Offset: 0x0002AC16
		private static void NarrowFormation(LineFormation formation, int fileCountFromLeftFlank, int fileCountFromRightFlank)
		{
			LineFormation.ShiftUnitsBackwardsForNarrowingFormation(formation, fileCountFromLeftFlank, fileCountFromRightFlank);
			LineFormation.NarrowFormationAux(formation, fileCountFromLeftFlank, fileCountFromRightFlank);
		}

		// Token: 0x06000F32 RID: 3890 RVA: 0x0002CA28 File Offset: 0x0002AC28
		private static void NarrowFormationAux(LineFormation formation, int fileCountFromLeftFlank, int fileCountFromRightFlank)
		{
			formation._units2DWorkspace.ResetWithNewCount(formation.FileCount - fileCountFromLeftFlank - fileCountFromRightFlank, formation.RankCount);
			formation._unitPositionAvailabilitiesWorkspace.ResetWithNewCount(formation.FileCount - fileCountFromLeftFlank - fileCountFromRightFlank, formation.RankCount);
			formation._globalPositionsWorkspace.ResetWithNewCount(formation.FileCount - fileCountFromLeftFlank - fileCountFromRightFlank, formation.RankCount);
			for (int i = fileCountFromLeftFlank; i < formation.FileCount - fileCountFromRightFlank; i++)
			{
				int num = i - fileCountFromLeftFlank;
				formation._units2D.CopyRowTo(i, 0, formation._units2DWorkspace, num, 0, formation.RankCount);
				formation.UnitPositionAvailabilities.CopyRowTo(i, 0, formation._unitPositionAvailabilitiesWorkspace, num, 0, formation.RankCount);
				formation._globalPositions.CopyRowTo(i, 0, formation._globalPositionsWorkspace, num, 0, formation.RankCount);
				if (fileCountFromLeftFlank > 0)
				{
					for (int j = 0; j < formation.RankCount; j++)
					{
						if (formation._units2D[i, j] != null)
						{
							formation._units2D[i, j].FormationFileIndex -= fileCountFromLeftFlank;
						}
					}
				}
			}
			MBList2D<IFormationUnit> units2D = formation._units2D;
			formation._units2D = formation._units2DWorkspace;
			formation._units2DWorkspace = units2D;
			formation.ReconstructUnitsFromUnits2D();
			MBList2D<int> unitPositionAvailabilities = formation.UnitPositionAvailabilities;
			formation.UnitPositionAvailabilities = formation._unitPositionAvailabilitiesWorkspace;
			formation._unitPositionAvailabilitiesWorkspace = unitPositionAvailabilities;
			MBList2D<WorldPosition> globalPositions = formation._globalPositions;
			formation._globalPositions = formation._globalPositionsWorkspace;
			formation._globalPositionsWorkspace = globalPositions;
			formation.BatchUnitPositionAvailabilities(true);
			Action onShapeChanged = formation.OnShapeChanged;
			if (onShapeChanged != null)
			{
				onShapeChanged();
			}
			if (formation._isDeformingOnWidthChange || (fileCountFromLeftFlank + fileCountFromRightFlank) % 2 == 1)
			{
				formation.OnFormationFrameChanged();
			}
		}

		// Token: 0x06000F33 RID: 3891 RVA: 0x0002CBC0 File Offset: 0x0002ADC0
		private static void ShortenFormation(LineFormation formation, int front, int rear)
		{
			formation._units2DWorkspace.ResetWithNewCount(formation.FileCount, formation.RankCount - front - rear);
			formation._unitPositionAvailabilitiesWorkspace.ResetWithNewCount(formation.FileCount, formation.RankCount - front - rear);
			formation._globalPositionsWorkspace.ResetWithNewCount(formation.FileCount, formation.RankCount - front - rear);
			for (int i = 0; i < formation.FileCount; i++)
			{
				formation._units2D.CopyRowTo(i, front, formation._units2DWorkspace, i, 0, formation.RankCount - rear - front);
				formation.UnitPositionAvailabilities.CopyRowTo(i, front, formation._unitPositionAvailabilitiesWorkspace, i, 0, formation.RankCount - rear - front);
				formation._globalPositions.CopyRowTo(i, front, formation._globalPositionsWorkspace, i, 0, formation.RankCount - rear - front);
				if (front > 0)
				{
					for (int j = front; j < formation.RankCount - rear; j++)
					{
						if (formation._units2D[i, j] != null)
						{
							formation._units2D[i, j].FormationRankIndex -= front;
						}
					}
				}
			}
			MBList2D<IFormationUnit> units2D = formation._units2D;
			formation._units2D = formation._units2DWorkspace;
			formation._units2DWorkspace = units2D;
			formation.ReconstructUnitsFromUnits2D();
			MBList2D<int> unitPositionAvailabilities = formation.UnitPositionAvailabilities;
			formation.UnitPositionAvailabilities = formation._unitPositionAvailabilitiesWorkspace;
			formation._unitPositionAvailabilitiesWorkspace = unitPositionAvailabilities;
			MBList2D<WorldPosition> globalPositions = formation._globalPositions;
			formation._globalPositions = formation._globalPositionsWorkspace;
			formation._globalPositionsWorkspace = globalPositions;
			formation.BatchUnitPositionAvailabilities(true);
			Action onShapeChanged = formation.OnShapeChanged;
			if (onShapeChanged == null)
			{
				return;
			}
			onShapeChanged();
		}

		// Token: 0x06000F34 RID: 3892 RVA: 0x0002CD43 File Offset: 0x0002AF43
		private void Shorten()
		{
			LineFormation.ShortenFormation(this, 0, 1);
		}

		// Token: 0x06000F35 RID: 3893 RVA: 0x0002CD50 File Offset: 0x0002AF50
		private void GetFrontAndRearOfFile(int fileIndex, out bool isFileEmtpy, out int rankIndexOfFront, out int rankIndexOfRear, bool includeUnavailablePositions = false)
		{
			rankIndexOfFront = -1;
			rankIndexOfRear = this.RankCount;
			for (int i = 0; i < this.RankCount; i++)
			{
				if (this._units2D[fileIndex, i] != null)
				{
					rankIndexOfFront = i;
					break;
				}
			}
			if (includeUnavailablePositions)
			{
				if (rankIndexOfFront != -1)
				{
					for (int j = rankIndexOfFront - 1; j >= 0; j--)
					{
						if (this.IsUnitPositionAvailable(fileIndex, j))
						{
							break;
						}
						rankIndexOfFront = j;
					}
				}
				else
				{
					bool flag = true;
					for (int k = 0; k < this.RankCount; k++)
					{
						if (this.IsUnitPositionAvailable(fileIndex, k))
						{
							flag = false;
							break;
						}
					}
					if (flag)
					{
						rankIndexOfFront = 0;
					}
				}
			}
			for (int l = this.RankCount - 1; l >= 0; l--)
			{
				if (this._units2D[fileIndex, l] != null)
				{
					rankIndexOfRear = l;
					break;
				}
			}
			if (includeUnavailablePositions)
			{
				if (rankIndexOfRear != this.RankCount)
				{
					for (int m = rankIndexOfRear + 1; m < this.RankCount; m++)
					{
						if (this.IsUnitPositionAvailable(fileIndex, m))
						{
							break;
						}
						rankIndexOfRear = m;
					}
				}
				else
				{
					bool flag2 = true;
					for (int n = 0; n < this.RankCount; n++)
					{
						if (this.IsUnitPositionAvailable(fileIndex, n))
						{
							flag2 = false;
							break;
						}
					}
					if (flag2)
					{
						rankIndexOfRear = this.RankCount - 1;
					}
				}
			}
			if (rankIndexOfFront == -1 && rankIndexOfRear == this.RankCount)
			{
				isFileEmtpy = true;
				return;
			}
			isFileEmtpy = false;
		}

		// Token: 0x06000F36 RID: 3894 RVA: 0x0002CE94 File Offset: 0x0002B094
		private void GetFlanksOfRank(int rankIndex, out bool isRankEmpty, out int fileIndexOfLeftFlank, out int fileIndexOfRightFlank, bool includeUnavailablePositions = false)
		{
			fileIndexOfLeftFlank = -1;
			fileIndexOfRightFlank = this.FileCount;
			for (int i = 0; i < this.FileCount; i++)
			{
				if (this._units2D[i, rankIndex] != null)
				{
					fileIndexOfLeftFlank = i;
					break;
				}
			}
			if (includeUnavailablePositions)
			{
				if (fileIndexOfLeftFlank != -1)
				{
					for (int j = fileIndexOfLeftFlank - 1; j >= 0; j--)
					{
						if (this.IsUnitPositionAvailable(j, rankIndex))
						{
							break;
						}
						fileIndexOfLeftFlank = j;
					}
				}
				else
				{
					bool flag = true;
					for (int k = 0; k < this.FileCount; k++)
					{
						if (this.IsUnitPositionAvailable(k, rankIndex))
						{
							flag = false;
							break;
						}
					}
					if (flag)
					{
						fileIndexOfLeftFlank = 0;
					}
				}
			}
			for (int l = this.FileCount - 1; l >= 0; l--)
			{
				if (this._units2D[l, rankIndex] != null)
				{
					fileIndexOfRightFlank = l;
					break;
				}
			}
			if (includeUnavailablePositions)
			{
				if (fileIndexOfRightFlank != this.FileCount)
				{
					for (int m = fileIndexOfRightFlank + 1; m < this.FileCount; m++)
					{
						if (this.IsUnitPositionAvailable(m, rankIndex))
						{
							break;
						}
						fileIndexOfRightFlank = m;
					}
				}
				else
				{
					bool flag2 = true;
					for (int n = 0; n < this.FileCount; n++)
					{
						if (this.IsUnitPositionAvailable(n, rankIndex))
						{
							flag2 = false;
							break;
						}
					}
					if (flag2)
					{
						fileIndexOfRightFlank = this.FileCount - 1;
					}
				}
			}
			if (fileIndexOfLeftFlank == -1 && fileIndexOfRightFlank == this.FileCount)
			{
				isRankEmpty = true;
				return;
			}
			isRankEmpty = false;
		}

		// Token: 0x06000F37 RID: 3895 RVA: 0x0002CFD6 File Offset: 0x0002B1D6
		private static void FillInTheGapsOfFile(LineFormation formation, int fileIndex, int rankIndex = 0, bool isCheckingLastRankForEmptiness = true)
		{
			LineFormation.FillInTheGapsOfFileAux(formation, fileIndex, rankIndex);
			while (isCheckingLastRankForEmptiness && formation.RankCount > 1 && formation.IsRankEmpty(formation.RankCount - 1))
			{
				formation.Shorten();
			}
		}

		// Token: 0x06000F38 RID: 3896 RVA: 0x0002D004 File Offset: 0x0002B204
		private static void FillInTheGapsOfFileAux(LineFormation formation, int fileIndex, int rankIndex)
		{
			for (;;)
			{
				int num = -1;
				while (rankIndex < formation.RankCount - 1)
				{
					if (formation._units2D[fileIndex, rankIndex] == null && formation.IsUnitPositionAvailable(fileIndex, rankIndex))
					{
						num = rankIndex;
						break;
					}
					rankIndex++;
				}
				int num2 = -1;
				while (rankIndex < formation.RankCount)
				{
					if (formation._units2D[fileIndex, rankIndex] != null)
					{
						num2 = rankIndex;
						break;
					}
					rankIndex++;
				}
				if (num == -1 || num2 == -1)
				{
					break;
				}
				formation.RelocateUnit(formation._units2D[fileIndex, num2], fileIndex, num);
				rankIndex = num + 1;
			}
		}

		// Token: 0x06000F39 RID: 3897 RVA: 0x0002D08C File Offset: 0x0002B28C
		private static void FillInTheGapsOfMiddleRanks(LineFormation formation, List<IFormationUnit> relocatedUnits = null)
		{
			int num = formation.RankCount - 1;
			for (int i = 0; i < formation.FileCount; i++)
			{
				if (formation._units2D[i, num] == null && !formation.IsFileFullyOccupied(i))
				{
					int num4;
					for (;;)
					{
						bool flag;
						int num2;
						int num3;
						formation.GetFrontAndRearOfFile(i, out flag, out num2, out num3, true);
						if (num3 == num)
						{
							goto IL_D7;
						}
						num4 = num3 + 1;
						if (flag)
						{
							num4 = -1;
							for (int j = 0; j < formation.RankCount; j++)
							{
								if (formation.IsUnitPositionAvailable(i, j))
								{
									num4 = j;
									break;
								}
							}
						}
						IFormationUnit unitToFillIn = LineFormation.GetUnitToFillIn(formation, i, num4);
						if (unitToFillIn == null)
						{
							break;
						}
						formation.RelocateUnit(unitToFillIn, i, num4);
						if (relocatedUnits != null)
						{
							relocatedUnits.Add(unitToFillIn);
						}
						if (formation.IsRankEmpty(num))
						{
							formation.Shorten();
							num = formation.RankCount - 1;
						}
					}
					for (int k = num4 + 1; k < formation.RankCount; k++)
					{
					}
				}
				IL_D7:;
			}
			while (formation.RankCount > 1 && formation.IsRankEmpty(formation.RankCount - 1))
			{
				formation.Shorten();
			}
			LineFormation.AlignLastRank(formation);
		}

		// Token: 0x06000F3A RID: 3898 RVA: 0x0002D1A8 File Offset: 0x0002B3A8
		private static void AlignRankToLeft(LineFormation formation, int fileIndex, int rankIndex)
		{
			int num = -1;
			while (fileIndex < formation.FileCount - 1)
			{
				if (formation._units2D[fileIndex, rankIndex] == null && formation.IsUnitPositionAvailable(fileIndex, rankIndex))
				{
					num = fileIndex;
					break;
				}
				fileIndex++;
			}
			int num2 = -1;
			while (fileIndex < formation.FileCount)
			{
				if (formation._units2D[fileIndex, rankIndex] != null)
				{
					num2 = fileIndex;
					break;
				}
				fileIndex++;
			}
			if (num != -1 && num2 != -1)
			{
				formation.RelocateUnit(formation._units2D[num2, rankIndex], num, rankIndex);
				LineFormation.AlignRankToLeft(formation, num + 1, rankIndex);
			}
		}

		// Token: 0x06000F3B RID: 3899 RVA: 0x0002D234 File Offset: 0x0002B434
		private static void AlignRankToRight(LineFormation formation, int fileIndex, int rankIndex)
		{
			int num = -1;
			while (fileIndex > 0)
			{
				if (formation._units2D[fileIndex, rankIndex] == null && formation.IsUnitPositionAvailable(fileIndex, rankIndex))
				{
					num = fileIndex;
					break;
				}
				fileIndex--;
			}
			int num2 = -1;
			while (fileIndex >= 0)
			{
				if (formation._units2D[fileIndex, rankIndex] != null)
				{
					num2 = fileIndex;
					break;
				}
				fileIndex--;
			}
			if (num != -1 && num2 != -1)
			{
				formation.RelocateUnit(formation._units2D[num2, rankIndex], num, rankIndex);
				LineFormation.AlignRankToRight(formation, num - 1, rankIndex);
			}
		}

		// Token: 0x06000F3C RID: 3900 RVA: 0x0002D2B4 File Offset: 0x0002B4B4
		private static void AlignLastRank(LineFormation formation)
		{
			int num = formation.RankCount - 1;
			bool flag;
			int num2;
			int num3;
			formation.GetFlanksOfRank(num, out flag, out num2, out num3, true);
			if (num == 0 && flag)
			{
				return;
			}
			LineFormation.AlignRankToLeft(formation, num2, num);
			bool flag2 = false;
			bool flag3 = false;
			for (;;)
			{
				formation.GetFlanksOfRank(num, out flag, out num2, out num3, true);
				if (!flag2 && num2 < formation.FileCount - num3 - 1 - 1)
				{
					int num4;
					int num5;
					formation.GetFlanksOfRank(num, out flag, out num4, out num5, false);
					formation.RelocateUnit(formation._units2D[num5, num], num3 + 1, num);
					LineFormation.AlignRankToRight(formation, num3 + 1, num);
					flag3 = true;
				}
				else
				{
					if (flag3 || num2 - 1 <= formation.FileCount - num3 - 1)
					{
						break;
					}
					int num6;
					int num7;
					formation.GetFlanksOfRank(num, out flag, out num6, out num7, false);
					formation.RelocateUnit(formation._units2D[num6, num], num2 - 1, num);
					LineFormation.AlignRankToLeft(formation, num2 - 1, num);
					flag2 = true;
				}
			}
		}

		// Token: 0x06000F3D RID: 3901 RVA: 0x0002D390 File Offset: 0x0002B590
		private int CountUnitsAtRank(int rankIndex)
		{
			int num = 0;
			for (int i = 0; i < this.FileCount; i++)
			{
				if (this._units2D[i, rankIndex] != null)
				{
					num++;
				}
			}
			return num;
		}

		// Token: 0x06000F3E RID: 3902 RVA: 0x0002D3C4 File Offset: 0x0002B5C4
		private bool IsRankEmpty(int rankIndex)
		{
			for (int i = 0; i < this.FileCount; i++)
			{
				if (this._units2D[i, rankIndex] != null)
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x06000F3F RID: 3903 RVA: 0x0002D3F4 File Offset: 0x0002B5F4
		private bool IsFileFullyOccupied(int fileIndex)
		{
			bool flag = true;
			for (int i = 0; i < this.RankCount; i++)
			{
				if (this._units2D[fileIndex, i] == null && this.IsUnitPositionAvailable(fileIndex, i))
				{
					flag = false;
					break;
				}
			}
			return flag;
		}

		// Token: 0x06000F40 RID: 3904 RVA: 0x0002D434 File Offset: 0x0002B634
		private bool IsRankFullyOccupied(int rankIndex)
		{
			bool flag = true;
			for (int i = 0; i < this.FileCount; i++)
			{
				if (this._units2D[i, rankIndex] == null && this.IsUnitPositionAvailable(i, rankIndex))
				{
					flag = false;
					break;
				}
			}
			return flag;
		}

		// Token: 0x06000F41 RID: 3905 RVA: 0x0002D474 File Offset: 0x0002B674
		private static IFormationUnit GetUnitToFillIn(LineFormation formation, int relocationFileIndex, int relocationRankIndex)
		{
			int i = formation.RankCount - 1;
			while (i >= 0)
			{
				if (relocationRankIndex == i)
				{
					return null;
				}
				bool flag;
				int num;
				int num2;
				formation.GetFlanksOfRank(i, out flag, out num, out num2, false);
				if (!flag)
				{
					if (relocationFileIndex > num2)
					{
						return formation._units2D[num2, i];
					}
					if (relocationFileIndex < num)
					{
						return formation._units2D[num, i];
					}
					if (num2 - relocationFileIndex > relocationFileIndex - num)
					{
						return formation._units2D[num, i];
					}
					return formation._units2D[num2, i];
				}
				else
				{
					i--;
				}
			}
			Debug.FailedAssert("This line should not be reached.", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\AI\\Formation\\LineFormation.cs", "GetUnitToFillIn", 3155);
			return null;
		}

		// Token: 0x06000F42 RID: 3906 RVA: 0x0002D510 File Offset: 0x0002B710
		private void RelocateUnit(IFormationUnit unit, int fileIndex, int rankIndex)
		{
			this._units2D[unit.FormationFileIndex, unit.FormationRankIndex] = null;
			this._units2D[fileIndex, rankIndex] = unit;
			this.ReconstructUnitsFromUnits2D();
			unit.FormationFileIndex = fileIndex;
			unit.FormationRankIndex = rankIndex;
			Action onShapeChanged = this.OnShapeChanged;
			if (onShapeChanged == null)
			{
				return;
			}
			onShapeChanged();
		}

		// Token: 0x1700036E RID: 878
		// (get) Token: 0x06000F43 RID: 3907 RVA: 0x0002D567 File Offset: 0x0002B767
		public int UnitCount
		{
			get
			{
				return this.GetAllUnits().Count;
			}
		}

		// Token: 0x1700036F RID: 879
		// (get) Token: 0x06000F44 RID: 3908 RVA: 0x0002D574 File Offset: 0x0002B774
		public int PositionedUnitCount
		{
			get
			{
				return this.UnitCount - this._unpositionedUnits.Count;
			}
		}

		// Token: 0x06000F45 RID: 3909 RVA: 0x0002D588 File Offset: 0x0002B788
		public IFormationUnit GetPlayerUnit()
		{
			return this._allUnits.FirstOrDefault((IFormationUnit unit) => unit.IsPlayerUnit);
		}

		// Token: 0x06000F46 RID: 3910 RVA: 0x0002D5B4 File Offset: 0x0002B7B4
		public MBList<IFormationUnit> GetAllUnits()
		{
			return this._allUnits;
		}

		// Token: 0x06000F47 RID: 3911 RVA: 0x0002D5BC File Offset: 0x0002B7BC
		public MBList<IFormationUnit> GetUnpositionedUnits()
		{
			return this._unpositionedUnits;
		}

		// Token: 0x06000F48 RID: 3912 RVA: 0x0002D5C4 File Offset: 0x0002B7C4
		public Vec2? GetLocalDirectionOfRelativeFormationLocation(IFormationUnit unit)
		{
			if (this._unpositionedUnits.Contains(unit))
			{
				return null;
			}
			Vec2 vec = new Vec2((float)unit.FormationFileIndex, (float)(-(float)unit.FormationRankIndex)) - new Vec2((float)(this.FileCount - 1) * 0.5f, (float)(this.RankCount - 1) * -0.5f);
			vec.Normalize();
			return new Vec2?(vec);
		}

		// Token: 0x06000F49 RID: 3913 RVA: 0x0002D634 File Offset: 0x0002B834
		public Vec2? GetLocalWallDirectionOfRelativeFormationLocation(IFormationUnit unit)
		{
			if (this._unpositionedUnits.Contains(unit))
			{
				return null;
			}
			Vec2 vec = new Vec2((float)unit.FormationFileIndex, (float)(-(float)unit.FormationRankIndex)) - new Vec2((float)(this.FileCount - 1) * 0.5f, (float)(-(float)this.RankCount));
			vec.Normalize();
			return new Vec2?(vec);
		}

		// Token: 0x06000F4A RID: 3914 RVA: 0x0002D69D File Offset: 0x0002B89D
		public void GetFormationInfo(out int fileCount, out int rankCount)
		{
			fileCount = this.FileCount;
			rankCount = this.RankCount;
		}

		// Token: 0x06000F4B RID: 3915 RVA: 0x0002D6B0 File Offset: 0x0002B8B0
		[Conditional("DEBUG")]
		private void AssertUnit(IFormationUnit unit, bool isAssertingUnitPositionAvailability = true)
		{
			if (isAssertingUnitPositionAvailability)
			{
				this.IsUnitPositionRestrained(unit.FormationFileIndex, unit.FormationRankIndex);
				if (this._isMiddleFrontUnitPositionReserved && this.GetMiddleFrontUnitPosition().Item1 == unit.FormationFileIndex)
				{
					bool flag = this.GetMiddleFrontUnitPosition().Item2 == unit.FormationRankIndex;
				}
				this.IsUnitPositionAvailable(unit.FormationFileIndex, unit.FormationRankIndex);
			}
		}

		// Token: 0x06000F4C RID: 3916 RVA: 0x0002D71E File Offset: 0x0002B91E
		[Conditional("DEBUG")]
		private void AssertUnpositionedUnit(IFormationUnit unit)
		{
		}

		// Token: 0x06000F4D RID: 3917 RVA: 0x0002D720 File Offset: 0x0002B920
		public float GetUnitsDistanceToFrontLine(IFormationUnit unit)
		{
			if (this._unpositionedUnits.Contains(unit))
			{
				return -1f;
			}
			return (float)unit.FormationRankIndex * (this.Distance + this.UnitDiameter) + this.UnitDiameter * 0.5f;
		}

		// Token: 0x06000F4E RID: 3918 RVA: 0x0002D758 File Offset: 0x0002B958
		public IFormationUnit GetNeighborUnitOfLeftSide(IFormationUnit unit)
		{
			if (this._unpositionedUnits.Contains(unit))
			{
				return null;
			}
			int formationRankIndex = unit.FormationRankIndex;
			for (int i = unit.FormationFileIndex - 1; i >= 0; i--)
			{
				if (this._units2D[i, formationRankIndex] != null)
				{
					return this._units2D[i, formationRankIndex];
				}
			}
			return null;
		}

		// Token: 0x06000F4F RID: 3919 RVA: 0x0002D7B0 File Offset: 0x0002B9B0
		public IFormationUnit GetNeighborUnitOfRightSide(IFormationUnit unit)
		{
			if (this._unpositionedUnits.Contains(unit))
			{
				return null;
			}
			int formationRankIndex = unit.FormationRankIndex;
			for (int i = unit.FormationFileIndex + 1; i < this.FileCount; i++)
			{
				if (this._units2D[i, formationRankIndex] != null)
				{
					return this._units2D[i, formationRankIndex];
				}
			}
			return null;
		}

		// Token: 0x06000F50 RID: 3920 RVA: 0x0002D80C File Offset: 0x0002BA0C
		public void SwitchUnitLocationsWithUnpositionedUnit(IFormationUnit firstUnit, IFormationUnit secondUnit)
		{
			int formationFileIndex = firstUnit.FormationFileIndex;
			int formationRankIndex = firstUnit.FormationRankIndex;
			this._unpositionedUnits.Remove(secondUnit);
			this._units2D[formationFileIndex, formationRankIndex] = secondUnit;
			secondUnit.FormationFileIndex = formationFileIndex;
			secondUnit.FormationRankIndex = formationRankIndex;
			firstUnit.FormationFileIndex = -1;
			firstUnit.FormationRankIndex = -1;
			this._unpositionedUnits.Add(firstUnit);
			this.ReconstructUnitsFromUnits2D();
			Action onShapeChanged = this.OnShapeChanged;
			if (onShapeChanged == null)
			{
				return;
			}
			onShapeChanged();
		}

		// Token: 0x06000F51 RID: 3921 RVA: 0x0002D880 File Offset: 0x0002BA80
		public void SwitchUnitLocations(IFormationUnit firstUnit, IFormationUnit secondUnit)
		{
			int formationFileIndex = firstUnit.FormationFileIndex;
			int formationRankIndex = firstUnit.FormationRankIndex;
			int formationFileIndex2 = secondUnit.FormationFileIndex;
			int formationRankIndex2 = secondUnit.FormationRankIndex;
			this._units2D[formationFileIndex, formationRankIndex] = secondUnit;
			this._units2D[formationFileIndex2, formationRankIndex2] = firstUnit;
			this.ReconstructUnitsFromUnits2D();
			firstUnit.FormationFileIndex = formationFileIndex2;
			firstUnit.FormationRankIndex = formationRankIndex2;
			secondUnit.FormationFileIndex = formationFileIndex;
			secondUnit.FormationRankIndex = formationRankIndex;
			Action onShapeChanged = this.OnShapeChanged;
			if (onShapeChanged == null)
			{
				return;
			}
			onShapeChanged();
		}

		// Token: 0x06000F52 RID: 3922 RVA: 0x0002D8F8 File Offset: 0x0002BAF8
		public void SwitchUnitLocationsWithBackMostUnit(IFormationUnit unit)
		{
			IFormationUnit lastUnit = this.GetLastUnit();
			if (lastUnit != null && unit != null && unit != lastUnit)
			{
				this.SwitchUnitLocations(unit, lastUnit);
			}
		}

		// Token: 0x06000F53 RID: 3923 RVA: 0x0002D91E File Offset: 0x0002BB1E
		public void BeforeFormationFrameChange()
		{
		}

		// Token: 0x06000F54 RID: 3924 RVA: 0x0002D920 File Offset: 0x0002BB20
		public void BatchUnitPositionAvailabilities(bool isUpdatingCachedOrderedLocalPositions = true)
		{
			if (isUpdatingCachedOrderedLocalPositions)
			{
				this.AreLocalPositionsDirty = true;
			}
			bool areLocalPositionsDirty = this.AreLocalPositionsDirty;
			this.AreLocalPositionsDirty = false;
			if (areLocalPositionsDirty)
			{
				this._cachedOrderedUnitPositionIndices.Clear();
				for (int i = 0; i < this.FileCount * this.RankCount; i++)
				{
					this._cachedOrderedUnitPositionIndices.Add(this.GetOrderedUnitPositionIndex(i));
				}
				this._cachedOrderedLocalPositions.Clear();
				for (int j = 0; j < this._cachedOrderedUnitPositionIndices.Count; j++)
				{
					Vec2i vec2i = this._cachedOrderedUnitPositionIndices[j];
					this._cachedOrderedLocalPositions.Add(this.GetLocalPositionOfUnit(vec2i.Item1, vec2i.Item2));
				}
			}
			this.MakeRestrainedPositionsUnavailable();
			if (!this.owner.BatchUnitPositions(this._cachedOrderedUnitPositionIndices, this._cachedOrderedLocalPositions, this.UnitPositionAvailabilities, this._globalPositions, this.FileCount, this.RankCount))
			{
				for (int k = 0; k < this.FileCount; k++)
				{
					for (int l = 0; l < this.RankCount; l++)
					{
						this.UnitPositionAvailabilities[k, l] = 1;
					}
				}
			}
			if (areLocalPositionsDirty)
			{
				this._cachedOrderedAndAvailableUnitPositionIndices.Clear();
				for (int m = 0; m < this._cachedOrderedUnitPositionIndices.Count; m++)
				{
					Vec2i vec2i2 = this._cachedOrderedUnitPositionIndices[m];
					if (this.UnitPositionAvailabilities[vec2i2.Item1, vec2i2.Item2] == 2)
					{
						this._cachedOrderedAndAvailableUnitPositionIndices.Add(vec2i2);
					}
				}
			}
			Formation formation;
			this._isCavalry = (formation = this.owner as Formation) != null && formation.CalculateHasSignificantNumberOfMounted;
		}

		// Token: 0x06000F55 RID: 3925 RVA: 0x0002DAC0 File Offset: 0x0002BCC0
		public void OnFormationFrameChanged()
		{
			this.UnitPositionAvailabilities.Clear();
			this.BatchUnitPositionAvailabilities(false);
			bool flag = LineFormation.ShiftUnitsBackwardsForNewUnavailableUnitPositions(this);
			for (int i = 0; i < this.FileCount; i++)
			{
				LineFormation.FillInTheGapsOfFile(this, i, 0, i == this.FileCount - 1);
			}
			bool flag2 = flag;
			bool flag3 = this.TryReaddingUnpositionedUnits();
			if (flag2 && !flag3)
			{
				this.owner.OnUnitAddedOrRemoved();
			}
			LineFormation.FillInTheGapsOfMiddleRanks(this, null);
		}

		// Token: 0x06000F56 RID: 3926 RVA: 0x0002DB2C File Offset: 0x0002BD2C
		private bool TryReaddingUnpositionedUnits()
		{
			bool flag = this._unpositionedUnits.Count > 0;
			for (int i = this._unpositionedUnits.Count - 1; i >= 0; i--)
			{
				i = MathF.Min(i, this._unpositionedUnits.Count - 1);
				if (i < 0)
				{
					break;
				}
				IFormationUnit formationUnit = this._unpositionedUnits[i];
				this.RemoveUnit(formationUnit);
				if (!this.AddUnit(formationUnit))
				{
					break;
				}
			}
			if (flag)
			{
				this.owner.OnUnitAddedOrRemoved();
			}
			return flag;
		}

		// Token: 0x06000F57 RID: 3927 RVA: 0x0002DBA4 File Offset: 0x0002BDA4
		private bool AreLastRanksCompletelyUnavailable(int numberOfRanksToCheck = 3)
		{
			bool flag = true;
			if (this.RankCount < numberOfRanksToCheck)
			{
				flag = false;
			}
			else
			{
				for (int i = 0; i < this.FileCount; i++)
				{
					for (int j = this.RankCount - numberOfRanksToCheck; j < this.RankCount; j++)
					{
						if (this.IsUnitPositionAvailable(i, j))
						{
							i = 2147483646;
							flag = false;
							break;
						}
					}
				}
			}
			return flag;
		}

		// Token: 0x06000F58 RID: 3928 RVA: 0x0002DC00 File Offset: 0x0002BE00
		[Conditional("DEBUG")]
		private void AssertUnitPositions()
		{
			for (int i = 0; i < this._units2D.Count1; i++)
			{
				for (int j = 0; j < this._units2D.Count2; j++)
				{
					IFormationUnit formationUnit = this._units2D[i, j];
				}
			}
			foreach (IFormationUnit formationUnit2 in this._unpositionedUnits)
			{
			}
		}

		// Token: 0x06000F59 RID: 3929 RVA: 0x0002DC88 File Offset: 0x0002BE88
		[Conditional("DEBUG")]
		private void AssertFilePositions(int fileIndex)
		{
			bool flag;
			int num;
			int num2;
			this.GetFrontAndRearOfFile(fileIndex, out flag, out num, out num2, true);
			if (!flag)
			{
				for (int i = num; i <= num2; i++)
				{
				}
			}
		}

		// Token: 0x06000F5A RID: 3930 RVA: 0x0002DCB4 File Offset: 0x0002BEB4
		[Conditional("DEBUG")]
		private void AssertRankPositions(int rankIndex)
		{
			bool flag;
			int num;
			int num2;
			this.GetFlanksOfRank(rankIndex, out flag, out num, out num2, true);
			if (!flag)
			{
				for (int i = num; i <= num2; i++)
				{
				}
			}
		}

		// Token: 0x06000F5B RID: 3931 RVA: 0x0002DCE0 File Offset: 0x0002BEE0
		public void OnFormationDispersed()
		{
			IEnumerable<Vec2i> enumerable = from i in this.GetOrderedUnitPositionIndices()
				where this.IsUnitPositionAvailable(i.Item1, i.Item2)
				select i;
			MBList<IFormationUnit> mblist = this.GetAllUnits().ToMBList<IFormationUnit>();
			foreach (Vec2i vec2i in enumerable)
			{
				int item = vec2i.Item1;
				int item2 = vec2i.Item2;
				IFormationUnit formationUnit = this._units2D[item, item2];
				if (formationUnit != null)
				{
					IFormationUnit closestUnitTo = this.owner.GetClosestUnitTo(this.GetLocalPositionOfUnit(item, item2), mblist, null);
					mblist[mblist.IndexOf(closestUnitTo)] = null;
					if (formationUnit != closestUnitTo)
					{
						if (closestUnitTo.FormationFileIndex == -1)
						{
							this.SwitchUnitLocationsWithUnpositionedUnit(formationUnit, closestUnitTo);
						}
						else
						{
							this.SwitchUnitLocations(formationUnit, closestUnitTo);
						}
					}
				}
			}
		}

		// Token: 0x06000F5C RID: 3932 RVA: 0x0002DDC4 File Offset: 0x0002BFC4
		public void OnUnitLostMount(IFormationUnit unit)
		{
		}

		// Token: 0x06000F5D RID: 3933 RVA: 0x0002DDC8 File Offset: 0x0002BFC8
		public bool IsTurnBackwardsNecessary(Vec2 previousPosition, WorldPosition? newPosition, Vec2 previousDirection, bool hasNewDirection, Vec2? newDirection)
		{
			return hasNewDirection && MathF.Abs(MBMath.GetSmallestDifferenceBetweenTwoAngles(newDirection.Value.RotationInRadians, previousDirection.RotationInRadians)) >= 2.3561945f;
		}

		// Token: 0x06000F5E RID: 3934 RVA: 0x0002DE08 File Offset: 0x0002C008
		public void TurnBackwards()
		{
			for (int i = 0; i <= this.FileCount / 2; i++)
			{
				int num = i;
				int num2 = this.FileCount - 1 - i;
				for (int j = 0; j < this.RankCount; j++)
				{
					int num3 = j;
					int num4 = this.RankCount - 1 - j;
					IFormationUnit formationUnit = this._units2D[num, num3];
					IFormationUnit formationUnit2 = this._units2D[num2, num4];
					if (formationUnit != formationUnit2)
					{
						if (formationUnit != null && formationUnit2 != null)
						{
							this.SwitchUnitLocations(formationUnit, formationUnit2);
						}
						else if (formationUnit != null)
						{
							if (this.IsUnitPositionAvailable(num2, num4))
							{
								this.RelocateUnit(formationUnit, num2, num4);
							}
						}
						else if (formationUnit2 != null && this.IsUnitPositionAvailable(num, num3))
						{
							this.RelocateUnit(formationUnit2, num, num3);
						}
					}
				}
			}
		}

		// Token: 0x06000F5F RID: 3935 RVA: 0x0002DED4 File Offset: 0x0002C0D4
		public float GetOccupationWidth(int unitCount)
		{
			if (unitCount < 1)
			{
				return 0f;
			}
			int num = this.FileCount - 1;
			int num2 = 0;
			for (int i = 0; i < this.FileCount * this.RankCount; i++)
			{
				Vec2i orderedUnitPositionIndex = this.GetOrderedUnitPositionIndex(i);
				if (orderedUnitPositionIndex.Item1 < num)
				{
					num = orderedUnitPositionIndex.Item1;
				}
				if (orderedUnitPositionIndex.Item1 > num2)
				{
					num2 = orderedUnitPositionIndex.Item1;
				}
				if (this.IsUnitPositionAvailable(orderedUnitPositionIndex.Item1, orderedUnitPositionIndex.Item2))
				{
					unitCount--;
					if (unitCount == 0)
					{
						break;
					}
				}
			}
			return (float)(num2 - num) * (this.Interval + this.UnitDiameter) + this.UnitDiameter;
		}

		// Token: 0x06000F60 RID: 3936 RVA: 0x0002DF74 File Offset: 0x0002C174
		public void InvalidateCacheOfUnitAux(Vec2 roundedLocalPosition)
		{
			int num;
			int num2;
			if (this.TryGetUnitPositionIndexFromLocalPosition(roundedLocalPosition, out num, out num2))
			{
				this.UnitPositionAvailabilities[num, num2] = 0;
			}
		}

		// Token: 0x06000F61 RID: 3937 RVA: 0x0002DF9C File Offset: 0x0002C19C
		public Vec2? CreateNewPosition(int unitIndex)
		{
			Vec2? vec = null;
			int num = 100;
			while (vec == null && num > 0 && !this.AreLastRanksCompletelyUnavailable(3) && this.IsDeepenApplicable())
			{
				this.Deepen();
				vec = this.GetLocalPositionOfUnitOrDefault(unitIndex);
				num--;
			}
			return vec;
		}

		// Token: 0x06000F62 RID: 3938 RVA: 0x0002DFE7 File Offset: 0x0002C1E7
		public virtual void RearrangeFrom(IFormationArrangement arrangement)
		{
			this.BatchUnitPositionAvailabilities(true);
		}

		// Token: 0x06000F63 RID: 3939 RVA: 0x0002DFF0 File Offset: 0x0002C1F0
		public virtual void RearrangeTo(IFormationArrangement arrangement)
		{
			if (arrangement is ColumnFormation)
			{
				this.IsTransforming = true;
				this.ReleaseMiddleFrontUnitPosition();
			}
		}

		// Token: 0x06000F64 RID: 3940 RVA: 0x0002E008 File Offset: 0x0002C208
		public virtual void RearrangeTransferUnits(IFormationArrangement arrangement)
		{
			LineFormation lineFormation;
			if ((lineFormation = arrangement as LineFormation) != null)
			{
				lineFormation._units2D = this._units2D;
				lineFormation._allUnits = this._allUnits;
				lineFormation.UnitPositionAvailabilities = this.UnitPositionAvailabilities;
				lineFormation._globalPositions = this._globalPositions;
				lineFormation._unpositionedUnits = this._unpositionedUnits;
				lineFormation.AreLocalPositionsDirty = true;
				lineFormation.OnFormationFrameChanged();
				return;
			}
			for (int i = 0; i < this.FileCount * this.RankCount; i++)
			{
				Vec2i orderedUnitPositionIndex = this.GetOrderedUnitPositionIndex(i);
				int item = orderedUnitPositionIndex.Item1;
				int item2 = orderedUnitPositionIndex.Item2;
				IFormationUnit formationUnit = this._units2D[item, item2];
				if (formationUnit != null)
				{
					formationUnit.FormationFileIndex = -1;
					formationUnit.FormationRankIndex = -1;
					arrangement.AddUnit(formationUnit);
				}
			}
			foreach (IFormationUnit formationUnit2 in this._unpositionedUnits)
			{
				arrangement.AddUnit(formationUnit2);
			}
		}

		// Token: 0x06000F65 RID: 3941 RVA: 0x0002E114 File Offset: 0x0002C314
		public static float CalculateWidth(float interval, float unitDiameter, int unitCountOnLine)
		{
			return (float)MathF.Max(0, unitCountOnLine - 1) * (interval + unitDiameter) + unitDiameter;
		}

		// Token: 0x06000F66 RID: 3942 RVA: 0x0002E126 File Offset: 0x0002C326
		public void FormFromFlankWidth(int unitCountOnLine, bool skipSingleFileChangesForPerformance = false)
		{
			if (skipSingleFileChangesForPerformance && MathF.Abs(this.FileCount - unitCountOnLine) <= 1)
			{
				return;
			}
			this.FlankWidth = LineFormation.CalculateWidth(this.Interval, this.UnitDiameter, unitCountOnLine);
		}

		// Token: 0x06000F67 RID: 3943 RVA: 0x0002E154 File Offset: 0x0002C354
		public void ReserveMiddleFrontUnitPosition(IFormationUnit vanguard)
		{
			this._isMiddleFrontUnitPositionReserved = true;
			this.OnFormationFrameChanged();
		}

		// Token: 0x06000F68 RID: 3944 RVA: 0x0002E163 File Offset: 0x0002C363
		public void ReleaseMiddleFrontUnitPosition()
		{
			this._isMiddleFrontUnitPositionReserved = false;
			this.OnFormationFrameChanged();
		}

		// Token: 0x06000F69 RID: 3945 RVA: 0x0002E172 File Offset: 0x0002C372
		private Vec2i GetMiddleFrontUnitPosition()
		{
			return this.GetOrderedUnitPositionIndex(0);
		}

		// Token: 0x06000F6A RID: 3946 RVA: 0x0002E17C File Offset: 0x0002C37C
		public Vec2 GetLocalPositionOfReservedUnitPosition()
		{
			Vec2i middleFrontUnitPosition = this.GetMiddleFrontUnitPosition();
			return this.GetLocalPositionOfUnit(middleFrontUnitPosition.Item1, middleFrontUnitPosition.Item2);
		}

		// Token: 0x06000F6B RID: 3947 RVA: 0x0002E1A4 File Offset: 0x0002C3A4
		public virtual void OnTickOccasionallyOfUnit(IFormationUnit unit, bool arrangementChangeAllowed)
		{
			Agent agent;
			if (!arrangementChangeAllowed || (agent = unit as Agent) == null)
			{
				return;
			}
			Agent agent2;
			if (unit.FormationRankIndex > 0 && agent.HasShieldCached && (agent2 = this._units2D[unit.FormationFileIndex, unit.FormationRankIndex - 1] as Agent) != null && agent2.Banner == null)
			{
				if (!agent2.HasShieldCached)
				{
					this.SwitchUnitLocations(unit, agent2);
					return;
				}
				int num = 1;
				while (unit.FormationFileIndex - num >= 0 || unit.FormationFileIndex + num < this.FileCount)
				{
					Agent agent3;
					if (unit.FormationFileIndex - num >= 0 && (agent3 = this._units2D[unit.FormationFileIndex - num, unit.FormationRankIndex - 1] as Agent) != null && !agent3.HasShieldCached && agent3.Banner == null)
					{
						this.SwitchUnitLocations(unit, agent3);
						return;
					}
					Agent agent4;
					if (unit.FormationFileIndex + num < this.FileCount && (agent4 = this._units2D[unit.FormationFileIndex + num, unit.FormationRankIndex - 1] as Agent) != null && !agent4.HasShieldCached && agent4.Banner == null)
					{
						this.SwitchUnitLocations(unit, agent4);
						return;
					}
					num++;
				}
			}
		}

		// Token: 0x06000F6C RID: 3948 RVA: 0x0002E2DC File Offset: 0x0002C4DC
		public virtual float GetDirectionChangeTendencyOfUnit(IFormationUnit unit)
		{
			if (this.RankCount == 1 || unit.FormationRankIndex == -1)
			{
				return 0f;
			}
			return (float)unit.FormationRankIndex * 1f / (float)(this.RankCount - 1);
		}

		// Token: 0x06000F6D RID: 3949 RVA: 0x0002E30D File Offset: 0x0002C50D
		public int GetCachedOrderedAndAvailableUnitPositionIndicesCount()
		{
			return this._cachedOrderedAndAvailableUnitPositionIndices.Count;
		}

		// Token: 0x06000F6E RID: 3950 RVA: 0x0002E31A File Offset: 0x0002C51A
		public Vec2i GetCachedOrderedAndAvailableUnitPositionIndexAt(int i)
		{
			return this._cachedOrderedAndAvailableUnitPositionIndices[i];
		}

		// Token: 0x06000F6F RID: 3951 RVA: 0x0002E328 File Offset: 0x0002C528
		public WorldPosition GetGlobalPositionAtIndex(int indexX, int indexY)
		{
			return this._globalPositions[indexX, indexY];
		}

		// Token: 0x04000386 RID: 902
		protected const int UnitPositionAvailabilityValueOfUnprocessed = 0;

		// Token: 0x04000387 RID: 903
		protected const int UnitPositionAvailabilityValueOfUnavailable = 1;

		// Token: 0x04000388 RID: 904
		protected const int UnitPositionAvailabilityValueOfAvailable = 2;

		// Token: 0x04000389 RID: 905
		private static readonly Vec2i InvalidPositionIndex = new Vec2i(-1, -1);

		// Token: 0x0400038A RID: 906
		protected readonly IFormation owner;

		// Token: 0x0400038B RID: 907
		private MBList2D<IFormationUnit> _units2D;

		// Token: 0x0400038C RID: 908
		private MBList2D<IFormationUnit> _units2DWorkspace;

		// Token: 0x0400038D RID: 909
		private MBList<IFormationUnit> _allUnits;

		// Token: 0x0400038E RID: 910
		private bool _isBatchRemovingUnits;

		// Token: 0x0400038F RID: 911
		private readonly List<int> _gapFillMinRanksPerFileForBatchRemove = new List<int>();

		// Token: 0x04000390 RID: 912
		private bool _batchRemoveInvolvesUnavailablePositions;

		// Token: 0x04000391 RID: 913
		private MBList<IFormationUnit> _unpositionedUnits;

		// Token: 0x04000392 RID: 914
		protected MBList2D<int> UnitPositionAvailabilities;

		// Token: 0x04000393 RID: 915
		private MBList2D<int> _unitPositionAvailabilitiesWorkspace;

		// Token: 0x04000394 RID: 916
		private MBList2D<WorldPosition> _globalPositions;

		// Token: 0x04000395 RID: 917
		private MBList2D<WorldPosition> _globalPositionsWorkspace;

		// Token: 0x04000396 RID: 918
		private readonly MBWorkspace<MBQueue<ValueTuple<IFormationUnit, int, int>>> _displacedUnitsWorkspace;

		// Token: 0x04000397 RID: 919
		private readonly MBWorkspace<MBArrayList<Vec2i>> _finalOccupationsWorkspace;

		// Token: 0x04000398 RID: 920
		private readonly MBWorkspace<MBArrayList<Vec2i>> _filledInUnitPositionsWorkspace;

		// Token: 0x04000399 RID: 921
		private readonly MBWorkspace<MBQueue<Vec2i>> _toBeFilledInGapsWorkspace;

		// Token: 0x0400039A RID: 922
		private readonly MBWorkspace<MBArrayList<Vec2i>> _finalVacanciesWorkspace;

		// Token: 0x0400039B RID: 923
		private readonly MBWorkspace<MBArrayList<Vec2i>> _filledInGapsWorkspace;

		// Token: 0x0400039C RID: 924
		private readonly MBWorkspace<MBArrayList<Vec2i>> _toBeEmptiedOutUnitPositionsWorkspace;

		// Token: 0x0400039D RID: 925
		private MBArrayList<Vec2i> _cachedOrderedUnitPositionIndices;

		// Token: 0x0400039E RID: 926
		private MBArrayList<Vec2i> _cachedOrderedAndAvailableUnitPositionIndices;

		// Token: 0x0400039F RID: 927
		private MBArrayList<Vec2> _cachedOrderedLocalPositions;

		// Token: 0x040003A0 RID: 928
		private Func<LineFormation, int, int, bool> _shiftUnitsBackwardsPredicateDelegate;

		// Token: 0x040003A1 RID: 929
		private Func<LineFormation, int, int, bool> _shiftUnitsForwardsPredicateDelegate;

		// Token: 0x040003A3 RID: 931
		private bool _isCavalry;

		// Token: 0x040003A4 RID: 932
		private bool _isStaggered = true;

		// Token: 0x040003A7 RID: 935
		private readonly bool _isDeformingOnWidthChange;

		// Token: 0x040003A8 RID: 936
		private bool _isMiddleFrontUnitPositionReserved;

		// Token: 0x040003A9 RID: 937
		protected bool IsTransforming;
	}
}
