using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x0200012C RID: 300
	public class ColumnFormation : IFormationArrangement
	{
		// Token: 0x17000331 RID: 817
		// (get) Token: 0x06000E0A RID: 3594 RVA: 0x00027FC3 File Offset: 0x000261C3
		// (set) Token: 0x06000E0B RID: 3595 RVA: 0x00027FCB File Offset: 0x000261CB
		public IFormationUnit Vanguard
		{
			get
			{
				return this._vanguard;
			}
			private set
			{
				this.SetVanguard(value);
			}
		}

		// Token: 0x17000332 RID: 818
		// (get) Token: 0x06000E0C RID: 3596 RVA: 0x00027FD4 File Offset: 0x000261D4
		// (set) Token: 0x06000E0D RID: 3597 RVA: 0x00027FDC File Offset: 0x000261DC
		public int ColumnCount
		{
			get
			{
				return this.FileCount;
			}
			set
			{
				this.SetColumnCount(value);
			}
		}

		// Token: 0x17000333 RID: 819
		// (get) Token: 0x06000E0E RID: 3598 RVA: 0x00027FE5 File Offset: 0x000261E5
		protected int FileCount
		{
			get
			{
				return this._units2D.Count1;
			}
		}

		// Token: 0x17000334 RID: 820
		// (get) Token: 0x06000E0F RID: 3599 RVA: 0x00027FF2 File Offset: 0x000261F2
		public int RankCount
		{
			get
			{
				return this._units2D.Count2;
			}
		}

		// Token: 0x17000335 RID: 821
		// (get) Token: 0x06000E10 RID: 3600 RVA: 0x00027FFF File Offset: 0x000261FF
		private int VanguardFileIndex
		{
			get
			{
				if (this.FileCount % 2 != 0)
				{
					return this.FileCount / 2;
				}
				if (this.isExpandingFromRightSide)
				{
					return this.FileCount / 2 - 1;
				}
				return this.FileCount / 2;
			}
		}

		// Token: 0x17000336 RID: 822
		// (get) Token: 0x06000E11 RID: 3601 RVA: 0x0002802F File Offset: 0x0002622F
		protected float Distance
		{
			get
			{
				return this.owner.Distance * 1f + 0.5f;
			}
		}

		// Token: 0x17000337 RID: 823
		// (get) Token: 0x06000E12 RID: 3602 RVA: 0x00028048 File Offset: 0x00026248
		protected float Interval
		{
			get
			{
				return this.owner.Interval * 1.5f;
			}
		}

		// Token: 0x06000E13 RID: 3603 RVA: 0x0002805C File Offset: 0x0002625C
		public ColumnFormation(IFormation ownerFormation, IFormationUnit vanguard = null, int columnCount = 1)
		{
			this.owner = ownerFormation;
			this._units2D = new MBList2D<IFormationUnit>(columnCount, 1);
			this._units2DWorkspace = new MBList2D<IFormationUnit>(columnCount, 1);
			this.ReconstructUnitsFromUnits2D();
			this._vanguard = vanguard;
			Action onShapeChanged = this.OnShapeChanged;
			if (onShapeChanged == null)
			{
				return;
			}
			onShapeChanged();
		}

		// Token: 0x06000E14 RID: 3604 RVA: 0x000280B4 File Offset: 0x000262B4
		public IFormationArrangement Clone(IFormation formation)
		{
			return new ColumnFormation(formation, this.Vanguard, this.ColumnCount);
		}

		// Token: 0x06000E15 RID: 3605 RVA: 0x000280C8 File Offset: 0x000262C8
		public void DeepCopyFrom(IFormationArrangement arrangement)
		{
		}

		// Token: 0x17000338 RID: 824
		// (get) Token: 0x06000E16 RID: 3606 RVA: 0x000280CA File Offset: 0x000262CA
		// (set) Token: 0x06000E17 RID: 3607 RVA: 0x000280D2 File Offset: 0x000262D2
		public float Width
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

		// Token: 0x17000339 RID: 825
		// (get) Token: 0x06000E18 RID: 3608 RVA: 0x000280DB File Offset: 0x000262DB
		// (set) Token: 0x06000E19 RID: 3609 RVA: 0x0002810C File Offset: 0x0002630C
		public float FlankWidth
		{
			get
			{
				return (float)(this.FileCount - 1) * (this.owner.Interval + this.owner.UnitDiameter) + this.owner.UnitDiameter;
			}
			set
			{
				int num = MathF.Max(0, (int)((value - this.owner.UnitDiameter) / (this.owner.Interval + this.owner.UnitDiameter) + 1E-05f)) + 1;
				num = MathF.Max(num, 1);
				this.SetColumnCount(num);
				Action onWidthChanged = this.OnWidthChanged;
				if (onWidthChanged == null)
				{
					return;
				}
				onWidthChanged();
			}
		}

		// Token: 0x1700033A RID: 826
		// (get) Token: 0x06000E1A RID: 3610 RVA: 0x0002816D File Offset: 0x0002636D
		public float Depth
		{
			get
			{
				return this.RankDepth;
			}
		}

		// Token: 0x1700033B RID: 827
		// (get) Token: 0x06000E1B RID: 3611 RVA: 0x00028175 File Offset: 0x00026375
		public float RankDepth
		{
			get
			{
				return (float)(this.RankCount - 1) * (this.Distance + this.owner.UnitDiameter) + this.owner.UnitDiameter;
			}
		}

		// Token: 0x1700033C RID: 828
		// (get) Token: 0x06000E1C RID: 3612 RVA: 0x0002819F File Offset: 0x0002639F
		public float MinimumWidth
		{
			get
			{
				return this.MinimumFlankWidth;
			}
		}

		// Token: 0x06000E1D RID: 3613 RVA: 0x000281A7 File Offset: 0x000263A7
		public IFormationUnit GetPlayerUnit()
		{
			return this._allUnits.FirstOrDefault((IFormationUnit unit) => unit.IsPlayerUnit);
		}

		// Token: 0x1700033D RID: 829
		// (get) Token: 0x06000E1E RID: 3614 RVA: 0x000281D3 File Offset: 0x000263D3
		public float MaximumWidth
		{
			get
			{
				return this.MinimumFlankWidth;
			}
		}

		// Token: 0x1700033E RID: 830
		// (get) Token: 0x06000E1F RID: 3615 RVA: 0x000281DB File Offset: 0x000263DB
		public float MinimumFlankWidth
		{
			get
			{
				return this.FlankWidth;
			}
		}

		// Token: 0x06000E20 RID: 3616 RVA: 0x000281E3 File Offset: 0x000263E3
		public MBList<IFormationUnit> GetAllUnits()
		{
			return this._allUnits;
		}

		// Token: 0x06000E21 RID: 3617 RVA: 0x000281EB File Offset: 0x000263EB
		public MBList<IFormationUnit> GetUnpositionedUnits()
		{
			return null;
		}

		// Token: 0x1700033F RID: 831
		// (get) Token: 0x06000E22 RID: 3618 RVA: 0x000281EE File Offset: 0x000263EE
		public bool? IsLoose
		{
			get
			{
				return new bool?(false);
			}
		}

		// Token: 0x06000E23 RID: 3619 RVA: 0x000281F8 File Offset: 0x000263F8
		private bool IsUnitPositionAvailable(int fileIndex, int rankIndex)
		{
			if (this.IsMiddleFrontUnitPositionReserved)
			{
				ValueTuple<int, int> middleFrontUnitPosition = this.GetMiddleFrontUnitPosition();
				if (fileIndex == middleFrontUnitPosition.Item1 && rankIndex == middleFrontUnitPosition.Item2)
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x06000E24 RID: 3620 RVA: 0x0002822C File Offset: 0x0002642C
		private bool GetNextVacancy(out int fileIndex, out int rankIndex)
		{
			if (this.RankCount == 0)
			{
				fileIndex = -1;
				rankIndex = -1;
				return false;
			}
			rankIndex = this.RankCount - 1;
			for (int i = 0; i < this.ColumnCount; i++)
			{
				int columnOffsetFromColumnIndex = ColumnFormation.GetColumnOffsetFromColumnIndex(i, this.isExpandingFromRightSide);
				fileIndex = this.VanguardFileIndex + columnOffsetFromColumnIndex;
				if (this._units2D[fileIndex, rankIndex] == null && this.IsUnitPositionAvailable(fileIndex, rankIndex))
				{
					return true;
				}
			}
			fileIndex = -1;
			rankIndex = -1;
			return false;
		}

		// Token: 0x06000E25 RID: 3621 RVA: 0x000282A4 File Offset: 0x000264A4
		private IFormationUnit GetLastUnit()
		{
			if (this.RankCount == 0)
			{
				return null;
			}
			int num = this.RankCount - 1;
			for (int i = this.ColumnCount - 1; i >= 0; i--)
			{
				int columnOffsetFromColumnIndex = ColumnFormation.GetColumnOffsetFromColumnIndex(i, this.isExpandingFromRightSide);
				int num2 = this.VanguardFileIndex + columnOffsetFromColumnIndex;
				IFormationUnit formationUnit = this._units2D[num2, num];
				if (formationUnit != null)
				{
					return formationUnit;
				}
			}
			return null;
		}

		// Token: 0x06000E26 RID: 3622 RVA: 0x00028304 File Offset: 0x00026504
		private void Deepen()
		{
			ColumnFormation.Deepen(this);
		}

		// Token: 0x06000E27 RID: 3623 RVA: 0x0002830C File Offset: 0x0002650C
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
		}

		// Token: 0x06000E28 RID: 3624 RVA: 0x0002838C File Offset: 0x0002658C
		private static void Deepen(ColumnFormation formation)
		{
			formation._units2DWorkspace.ResetWithNewCount(formation.FileCount, formation.RankCount + 1);
			for (int i = 0; i < formation.FileCount; i++)
			{
				formation._units2D.CopyRowTo(i, 0, formation._units2DWorkspace, i, 0, formation.RankCount);
			}
			MBList2D<IFormationUnit> units2D = formation._units2D;
			formation._units2D = formation._units2DWorkspace;
			formation._units2DWorkspace = units2D;
			formation.ReconstructUnitsFromUnits2D();
			Action onShapeChanged = formation.OnShapeChanged;
			if (onShapeChanged == null)
			{
				return;
			}
			onShapeChanged();
		}

		// Token: 0x06000E29 RID: 3625 RVA: 0x0002840E File Offset: 0x0002660E
		private void Shorten()
		{
			ColumnFormation.Shorten(this);
		}

		// Token: 0x06000E2A RID: 3626 RVA: 0x00028418 File Offset: 0x00026618
		private static void Shorten(ColumnFormation formation)
		{
			formation._units2DWorkspace.ResetWithNewCount(formation.FileCount, formation.RankCount - 1);
			for (int i = 0; i < formation.FileCount; i++)
			{
				formation._units2D.CopyRowTo(i, 0, formation._units2DWorkspace, i, 0, formation.RankCount - 1);
			}
			MBList2D<IFormationUnit> units2D = formation._units2D;
			formation._units2D = formation._units2DWorkspace;
			formation._units2DWorkspace = units2D;
			formation.ReconstructUnitsFromUnits2D();
			Action onShapeChanged = formation.OnShapeChanged;
			if (onShapeChanged == null)
			{
				return;
			}
			onShapeChanged();
		}

		// Token: 0x06000E2B RID: 3627 RVA: 0x0002849C File Offset: 0x0002669C
		public bool AddUnit(IFormationUnit unit)
		{
			int num = 0;
			bool flag = false;
			while (!flag && num < 100)
			{
				num++;
				if (num > 10)
				{
					Debug.FailedAssert("false", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\AI\\Formation\\ColumnFormation.cs", "AddUnit", 370);
				}
				int num2;
				int num3;
				if (this.GetNextVacancy(out num2, out num3))
				{
					unit.FormationFileIndex = num2;
					unit.FormationRankIndex = num3;
					this._units2D[num2, num3] = unit;
					this.ReconstructUnitsFromUnits2D();
					flag = true;
				}
				else
				{
					this.Deepen();
				}
			}
			if (flag)
			{
				int num4;
				IFormationUnit unitToFollow = this.GetUnitToFollow(unit, out num4);
				this.SetUnitToFollow(unit, unitToFollow, num4);
				Action onShapeChanged = this.OnShapeChanged;
				if (onShapeChanged != null)
				{
					onShapeChanged();
				}
			}
			return flag;
		}

		// Token: 0x06000E2C RID: 3628 RVA: 0x0002853E File Offset: 0x0002673E
		private IFormationUnit TryGetUnit(int fileIndex, int rankIndex)
		{
			if (fileIndex >= 0 && fileIndex < this.FileCount && rankIndex >= 0 && rankIndex < this.RankCount)
			{
				return this._units2D[fileIndex, rankIndex];
			}
			return null;
		}

		// Token: 0x06000E2D RID: 3629 RVA: 0x0002856C File Offset: 0x0002676C
		private void AdjustFollowDataOfUnitPosition(int fileIndex, int rankIndex)
		{
			IFormationUnit formationUnit = this._units2D[fileIndex, rankIndex];
			if (fileIndex == this.VanguardFileIndex)
			{
				if (formationUnit != null)
				{
					IFormationUnit formationUnit2 = this.TryGetUnit(fileIndex, rankIndex - 1);
					this.SetUnitToFollow(formationUnit, formationUnit2 ?? this.Vanguard, 0);
				}
				for (int i = 1; i < this.ColumnCount; i++)
				{
					int columnOffsetFromColumnIndex = ColumnFormation.GetColumnOffsetFromColumnIndex(i, this.isExpandingFromRightSide);
					IFormationUnit formationUnit3 = this._units2D[fileIndex + columnOffsetFromColumnIndex, rankIndex];
					if (formationUnit3 != null)
					{
						this.SetUnitToFollow(formationUnit3, formationUnit ?? this.Vanguard, columnOffsetFromColumnIndex);
					}
				}
				IFormationUnit formationUnit4 = this.TryGetUnit(fileIndex, rankIndex + 1);
				if (formationUnit4 != null)
				{
					this.SetUnitToFollow(formationUnit4, formationUnit ?? this.Vanguard, 0);
					return;
				}
			}
			else if (formationUnit != null)
			{
				IFormationUnit formationUnit5 = this._units2D[this.VanguardFileIndex, rankIndex];
				int columnOffsetFromColumnIndex2 = ColumnFormation.GetColumnOffsetFromColumnIndex(fileIndex, this.isExpandingFromRightSide);
				this.SetUnitToFollow(formationUnit, formationUnit5 ?? this.Vanguard, columnOffsetFromColumnIndex2);
			}
		}

		// Token: 0x06000E2E RID: 3630 RVA: 0x0002865C File Offset: 0x0002685C
		private void ShiftUnitsForward(int fileIndex, int rankIndex)
		{
			for (;;)
			{
				IFormationUnit formationUnit = this.TryGetUnit(fileIndex, rankIndex + 1);
				if (formationUnit == null)
				{
					break;
				}
				IFormationUnit formationUnit2 = formationUnit;
				int formationRankIndex = formationUnit2.FormationRankIndex;
				formationUnit2.FormationRankIndex = formationRankIndex - 1;
				this._units2D[fileIndex, rankIndex] = formationUnit;
				this._units2D[fileIndex, rankIndex + 1] = null;
				this.ReconstructUnitsFromUnits2D();
				this.AdjustFollowDataOfUnitPosition(fileIndex, rankIndex);
				rankIndex++;
			}
			int num = 0;
			if (rankIndex == this.RankCount - 1)
			{
				for (int i = 0; i < this.ColumnCount; i++)
				{
					int columnOffsetFromColumnIndex = ColumnFormation.GetColumnOffsetFromColumnIndex(i, this.isExpandingFromRightSide);
					if (this.VanguardFileIndex + columnOffsetFromColumnIndex == fileIndex)
					{
						num = i + 1;
					}
				}
			}
			IFormationUnit formationUnit3 = null;
			for (int j = this.ColumnCount - 1; j >= num; j--)
			{
				int columnOffsetFromColumnIndex2 = ColumnFormation.GetColumnOffsetFromColumnIndex(j, this.isExpandingFromRightSide);
				int num2 = this.VanguardFileIndex + columnOffsetFromColumnIndex2;
				formationUnit3 = this._units2D[num2, this.RankCount - 1];
				if (formationUnit3 != null)
				{
					break;
				}
			}
			if (formationUnit3 != null)
			{
				this._units2D[formationUnit3.FormationFileIndex, formationUnit3.FormationRankIndex] = null;
				formationUnit3.FormationFileIndex = fileIndex;
				formationUnit3.FormationRankIndex = rankIndex;
				this._units2D[fileIndex, rankIndex] = formationUnit3;
				this.ReconstructUnitsFromUnits2D();
				this.AdjustFollowDataOfUnitPosition(fileIndex, rankIndex);
			}
			if (this.IsLastRankEmpty())
			{
				this.Shorten();
			}
			Action onShapeChanged = this.OnShapeChanged;
			if (onShapeChanged == null)
			{
				return;
			}
			onShapeChanged();
		}

		// Token: 0x06000E2F RID: 3631 RVA: 0x000287AC File Offset: 0x000269AC
		private void ShiftUnitsBackwardForMakingRoomForVanguard(int fileIndex, int rankIndex)
		{
			if (this.RankCount == 1)
			{
				bool flag = false;
				int num = -1;
				for (int i = 0; i < this.ColumnCount; i++)
				{
					int columnOffsetFromColumnIndex = ColumnFormation.GetColumnOffsetFromColumnIndex(i, this.isExpandingFromRightSide);
					if (this._units2D[this.VanguardFileIndex + columnOffsetFromColumnIndex, 0] == null)
					{
						flag = true;
						num = this.VanguardFileIndex + columnOffsetFromColumnIndex;
						break;
					}
				}
				if (flag)
				{
					IFormationUnit formationUnit = this._units2D[fileIndex, rankIndex];
					this._units2D[fileIndex, rankIndex] = null;
					this._units2D[num, 0] = formationUnit;
					this.ReconstructUnitsFromUnits2D();
					formationUnit.FormationFileIndex = num;
					formationUnit.FormationRankIndex = 0;
					return;
				}
				ColumnFormation.Deepen(this);
				IFormationUnit formationUnit2 = this._units2D[fileIndex, rankIndex];
				this._units2D[fileIndex, rankIndex] = null;
				this._units2D[fileIndex, rankIndex + 1] = formationUnit2;
				this.ReconstructUnitsFromUnits2D();
				IFormationUnit formationUnit3 = formationUnit2;
				int num2 = formationUnit3.FormationRankIndex;
				formationUnit3.FormationRankIndex = num2 + 1;
				return;
			}
			else
			{
				int num3 = rankIndex;
				IFormationUnit formationUnit4 = null;
				for (rankIndex = this.RankCount - 1; rankIndex >= num3; rankIndex--)
				{
					IFormationUnit formationUnit5 = this._units2D[fileIndex, rankIndex];
					this.TryGetUnit(fileIndex, rankIndex + 1);
					this._units2D[fileIndex, rankIndex] = null;
					if (rankIndex + 1 < this.RankCount)
					{
						IFormationUnit formationUnit6 = formationUnit5;
						int num2 = formationUnit6.FormationRankIndex;
						formationUnit6.FormationRankIndex = num2 + 1;
						this._units2D[fileIndex, rankIndex + 1] = formationUnit5;
					}
					else
					{
						formationUnit4 = formationUnit5;
						if (formationUnit4 != null)
						{
							formationUnit4.FormationFileIndex = -1;
							formationUnit4.FormationRankIndex = -1;
						}
					}
					this.ReconstructUnitsFromUnits2D();
				}
				for (rankIndex = this.RankCount - 1; rankIndex >= num3; rankIndex--)
				{
					this.AdjustFollowDataOfUnitPosition(fileIndex, rankIndex);
				}
				if (formationUnit4 != null)
				{
					this.AddUnit(formationUnit4);
				}
				Action onShapeChanged = this.OnShapeChanged;
				if (onShapeChanged == null)
				{
					return;
				}
				onShapeChanged();
				return;
			}
		}

		// Token: 0x06000E30 RID: 3632 RVA: 0x00028970 File Offset: 0x00026B70
		private bool IsLastRankEmpty()
		{
			if (this.RankCount == 0)
			{
				return false;
			}
			for (int i = 0; i < this.FileCount; i++)
			{
				if (this._units2D[i, this.RankCount - 1] != null)
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x06000E31 RID: 3633 RVA: 0x000289B4 File Offset: 0x00026BB4
		public void RemoveUnit(IFormationUnit unit)
		{
			int formationFileIndex = unit.FormationFileIndex;
			int formationRankIndex = unit.FormationRankIndex;
			if (GameNetwork.IsServer)
			{
				MBDebug.Print(string.Concat(new object[] { "Removing unit at ", formationFileIndex, " ", formationRankIndex, " from column arrangement\nFileCount&RankCount: ", this.FileCount, " ", this.RankCount }), 0, Debug.DebugColor.White, 17592186044416UL);
			}
			this._units2D[unit.FormationFileIndex, unit.FormationRankIndex] = null;
			this.ReconstructUnitsFromUnits2D();
			this.ShiftUnitsForward(unit.FormationFileIndex, unit.FormationRankIndex);
			if (this.IsLastRankEmpty())
			{
				this.Shorten();
			}
			unit.FormationFileIndex = -1;
			unit.FormationRankIndex = -1;
			this.SetUnitToFollow(unit, null, 0);
			Action onShapeChanged = this.OnShapeChanged;
			if (onShapeChanged != null)
			{
				onShapeChanged();
			}
			if (this.Vanguard == unit && !((Agent)unit).IsActive())
			{
				this._vanguard = null;
				if (this.FileCount > 0 && this.RankCount > 0)
				{
					this.AdjustFollowDataOfUnitPosition(formationFileIndex, formationRankIndex);
				}
			}
		}

		// Token: 0x06000E32 RID: 3634 RVA: 0x00028ADD File Offset: 0x00026CDD
		public IFormationUnit GetUnit(int fileIndex, int rankIndex)
		{
			return this._units2D[fileIndex, rankIndex];
		}

		// Token: 0x06000E33 RID: 3635 RVA: 0x00028AEC File Offset: 0x00026CEC
		public void OnBatchRemoveStart()
		{
		}

		// Token: 0x06000E34 RID: 3636 RVA: 0x00028AEE File Offset: 0x00026CEE
		public void OnBatchRemoveEnd()
		{
		}

		// Token: 0x06000E35 RID: 3637 RVA: 0x00028AF0 File Offset: 0x00026CF0
		[Conditional("DEBUG")]
		private void AssertUnitPositions()
		{
			for (int i = 0; i < this.FileCount; i++)
			{
				for (int j = 0; j < this.RankCount; j++)
				{
					IFormationUnit formationUnit = this._units2D[i, j];
				}
			}
		}

		// Token: 0x06000E36 RID: 3638 RVA: 0x00028B30 File Offset: 0x00026D30
		[Conditional("DEBUG")]
		private void AssertUnit(IFormationUnit unit, bool isAssertingFollowed = true)
		{
			if (unit == null)
			{
				return;
			}
			if (isAssertingFollowed)
			{
				int num;
				this.GetUnitToFollow(unit, out num);
			}
		}

		// Token: 0x06000E37 RID: 3639 RVA: 0x00028B50 File Offset: 0x00026D50
		private static int GetColumnOffsetFromColumnIndex(int columnIndex, bool isExpandingFromRightSide)
		{
			int num;
			if (isExpandingFromRightSide)
			{
				num = (columnIndex + 1) / 2 * ((columnIndex % 2 == 0) ? (-1) : 1);
			}
			else
			{
				num = (columnIndex + 1) / 2 * ((columnIndex % 2 == 0) ? 1 : (-1));
			}
			return num;
		}

		// Token: 0x06000E38 RID: 3640 RVA: 0x00028B84 File Offset: 0x00026D84
		private IFormationUnit GetUnitToFollow(IFormationUnit unit, out int columnOffset)
		{
			IFormationUnit formationUnit;
			if (unit.FormationFileIndex == this.VanguardFileIndex)
			{
				columnOffset = 0;
				if (unit.FormationRankIndex > 0)
				{
					formationUnit = this._units2D[unit.FormationFileIndex, unit.FormationRankIndex - 1];
				}
				else
				{
					formationUnit = null;
				}
			}
			else
			{
				columnOffset = unit.FormationFileIndex - this.VanguardFileIndex;
				formationUnit = this._units2D[this.VanguardFileIndex, unit.FormationRankIndex];
			}
			if (formationUnit == null)
			{
				formationUnit = this.Vanguard;
			}
			return formationUnit;
		}

		// Token: 0x06000E39 RID: 3641 RVA: 0x00028BFD File Offset: 0x00026DFD
		private IEnumerable<ValueTuple<int, int>> GetOrderedUnitPositionIndices()
		{
			int num2;
			for (int rankIndex = 0; rankIndex < this.RankCount; rankIndex = num2 + 1)
			{
				for (int columnIndex = 0; columnIndex < this.ColumnCount; columnIndex = num2 + 1)
				{
					int columnOffsetFromColumnIndex = ColumnFormation.GetColumnOffsetFromColumnIndex(columnIndex, this.isExpandingFromRightSide);
					int num = this.VanguardFileIndex + columnOffsetFromColumnIndex;
					if (this.IsUnitPositionAvailable(num, rankIndex))
					{
						yield return new ValueTuple<int, int>(num, rankIndex);
					}
					num2 = columnIndex;
				}
				num2 = rankIndex;
			}
			yield break;
		}

		// Token: 0x06000E3A RID: 3642 RVA: 0x00028C10 File Offset: 0x00026E10
		private Vec2 GetLocalPositionOfUnit(int fileIndex, int rankIndex)
		{
			float num = (float)(this.FileCount - 1) * (this.owner.Interval + this.owner.UnitDiameter);
			return new Vec2((float)fileIndex * (this.owner.Interval + this.owner.UnitDiameter) - num / 2f, (float)(-(float)rankIndex) * (this.owner.Distance + this.owner.UnitDiameter));
		}

		// Token: 0x06000E3B RID: 3643 RVA: 0x00028C81 File Offset: 0x00026E81
		private Vec2 GetLocalDirectionOfUnit(int fileIndex, int rankIndex)
		{
			return Vec2.Forward;
		}

		// Token: 0x06000E3C RID: 3644 RVA: 0x00028C88 File Offset: 0x00026E88
		private WorldPosition? GetWorldPositionOfUnit(int fileIndex, int rankIndex)
		{
			return null;
		}

		// Token: 0x06000E3D RID: 3645 RVA: 0x00028CA0 File Offset: 0x00026EA0
		public Vec2? GetLocalPositionOfUnitOrDefault(int unitIndex)
		{
			ValueTuple<int, int> valueTuple = (from i in this.GetOrderedUnitPositionIndices()
				where this.IsUnitPositionAvailable(i.Item1, i.Item2)
				select i).ElementAtOrValue(unitIndex, new ValueTuple<int, int>(-1, -1));
			Vec2? vec;
			if (valueTuple.Item1 != -1 && valueTuple.Item2 != -1)
			{
				int item = valueTuple.Item1;
				int item2 = valueTuple.Item2;
				vec = new Vec2?(this.GetLocalPositionOfUnit(item, item2));
			}
			else
			{
				vec = null;
			}
			return vec;
		}

		// Token: 0x06000E3E RID: 3646 RVA: 0x00028D0C File Offset: 0x00026F0C
		public Vec2? GetLocalDirectionOfUnitOrDefault(int unitIndex)
		{
			ValueTuple<int, int> valueTuple = (from i in this.GetOrderedUnitPositionIndices()
				where this.IsUnitPositionAvailable(i.Item1, i.Item2)
				select i).ElementAtOrValue(unitIndex, new ValueTuple<int, int>(-1, -1));
			Vec2? vec;
			if (valueTuple.Item1 != -1 && valueTuple.Item2 != -1)
			{
				int item = valueTuple.Item1;
				int item2 = valueTuple.Item2;
				vec = new Vec2?(this.GetLocalDirectionOfUnit(item, item2));
			}
			else
			{
				vec = null;
			}
			return vec;
		}

		// Token: 0x06000E3F RID: 3647 RVA: 0x00028D78 File Offset: 0x00026F78
		public WorldPosition? GetWorldPositionOfUnitOrDefault(int unitIndex)
		{
			ValueTuple<int, int> valueTuple = (from i in this.GetOrderedUnitPositionIndices()
				where this.IsUnitPositionAvailable(i.Item1, i.Item2)
				select i).ElementAtOrValue(unitIndex, new ValueTuple<int, int>(-1, -1));
			WorldPosition? worldPosition;
			if (valueTuple.Item1 != -1 && valueTuple.Item2 != -1)
			{
				int item = valueTuple.Item1;
				int item2 = valueTuple.Item2;
				worldPosition = this.GetWorldPositionOfUnit(item, item2);
			}
			else
			{
				worldPosition = null;
			}
			return worldPosition;
		}

		// Token: 0x06000E40 RID: 3648 RVA: 0x00028DDE File Offset: 0x00026FDE
		public Vec2? GetLocalPositionOfUnitOrDefault(IFormationUnit unit)
		{
			return new Vec2?(this.GetLocalPositionOfUnit(unit.FormationFileIndex, unit.FormationRankIndex));
		}

		// Token: 0x06000E41 RID: 3649 RVA: 0x00028DF7 File Offset: 0x00026FF7
		public Vec2? GetLocalPositionOfUnitOrDefaultWithAdjustment(IFormationUnit unit, float distanceBetweenAgentsAdjustment)
		{
			return new Vec2?(this.GetLocalPositionOfUnit(unit.FormationFileIndex, unit.FormationRankIndex));
		}

		// Token: 0x06000E42 RID: 3650 RVA: 0x00028E10 File Offset: 0x00027010
		public WorldPosition? GetWorldPositionOfUnitOrDefault(IFormationUnit unit)
		{
			return this.GetWorldPositionOfUnit(unit.FormationFileIndex, unit.FormationRankIndex);
		}

		// Token: 0x06000E43 RID: 3651 RVA: 0x00028E24 File Offset: 0x00027024
		public Vec2? GetLocalDirectionOfUnitOrDefault(IFormationUnit unit)
		{
			return new Vec2?(this.GetLocalDirectionOfUnit(unit.FormationFileIndex, unit.FormationRankIndex));
		}

		// Token: 0x06000E44 RID: 3652 RVA: 0x00028E40 File Offset: 0x00027040
		public List<IFormationUnit> GetUnitsToPop(int count)
		{
			List<IFormationUnit> list = new List<IFormationUnit>();
			for (int i = this.RankCount - 1; i >= 0; i--)
			{
				for (int j = this.ColumnCount - 1; j >= 0; j--)
				{
					int columnOffsetFromColumnIndex = ColumnFormation.GetColumnOffsetFromColumnIndex(j, this.isExpandingFromRightSide);
					int num = this.VanguardFileIndex + columnOffsetFromColumnIndex;
					IFormationUnit formationUnit = this._units2D[num, i];
					if (formationUnit != null)
					{
						list.Add(formationUnit);
						count--;
						if (count == 0)
						{
							return list;
						}
					}
				}
			}
			return list;
		}

		// Token: 0x06000E45 RID: 3653 RVA: 0x00028EB7 File Offset: 0x000270B7
		public List<IFormationUnit> GetUnitsToPop(int count, Vec3 targetPosition)
		{
			return this.GetUnitsToPop(count);
		}

		// Token: 0x06000E46 RID: 3654 RVA: 0x00028EC0 File Offset: 0x000270C0
		public IEnumerable<IFormationUnit> GetUnitsToPopWithCondition(int count, Func<IFormationUnit, bool> currentCondition)
		{
			int num2;
			for (int rankIndex = this.RankCount - 1; rankIndex >= 0; rankIndex = num2 - 1)
			{
				for (int columnIndex = this.ColumnCount - 1; columnIndex >= 0; columnIndex = num2 - 1)
				{
					int columnOffsetFromColumnIndex = ColumnFormation.GetColumnOffsetFromColumnIndex(columnIndex, this.isExpandingFromRightSide);
					int num = this.VanguardFileIndex + columnOffsetFromColumnIndex;
					IFormationUnit formationUnit = this._units2D[num, rankIndex];
					if (formationUnit != null && currentCondition(formationUnit))
					{
						yield return formationUnit;
						num2 = count;
						count = num2 - 1;
						if (count == 0)
						{
							yield break;
						}
					}
					num2 = columnIndex;
				}
				num2 = rankIndex;
			}
			yield break;
		}

		// Token: 0x06000E47 RID: 3655 RVA: 0x00028EDE File Offset: 0x000270DE
		public void SwitchUnitLocations(IFormationUnit firstUnit, IFormationUnit secondUnit)
		{
			this.SwitchUnitLocationsAux(firstUnit, secondUnit);
			this.AdjustFollowDataOfUnitPosition(firstUnit.FormationFileIndex, firstUnit.FormationRankIndex);
			this.AdjustFollowDataOfUnitPosition(secondUnit.FormationFileIndex, secondUnit.FormationRankIndex);
		}

		// Token: 0x06000E48 RID: 3656 RVA: 0x00028F0C File Offset: 0x0002710C
		private void SwitchUnitLocationsAux(IFormationUnit firstUnit, IFormationUnit secondUnit)
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

		// Token: 0x06000E49 RID: 3657 RVA: 0x00028F83 File Offset: 0x00027183
		public void SwitchUnitLocationsWithUnpositionedUnit(IFormationUnit firstUnit, IFormationUnit secondUnit)
		{
			Debug.FailedAssert("Column formation should NOT have an unpositioned unit", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\AI\\Formation\\ColumnFormation.cs", "SwitchUnitLocationsWithUnpositionedUnit", 1168);
		}

		// Token: 0x06000E4A RID: 3658 RVA: 0x00028FA0 File Offset: 0x000271A0
		public void SwitchUnitLocationsWithBackMostUnit(IFormationUnit unit)
		{
			Agent agent;
			if (this.Vanguard == null || (agent = this.Vanguard as Agent) == null || agent != unit)
			{
				IFormationUnit lastUnit = this.GetLastUnit();
				if (lastUnit != null && unit != null && unit != lastUnit)
				{
					this.SwitchUnitLocations(unit, lastUnit);
				}
			}
		}

		// Token: 0x06000E4B RID: 3659 RVA: 0x00028FE1 File Offset: 0x000271E1
		public float GetUnitsDistanceToFrontLine(IFormationUnit unit)
		{
			return -1f;
		}

		// Token: 0x06000E4C RID: 3660 RVA: 0x00028FE8 File Offset: 0x000271E8
		public Vec2? GetLocalDirectionOfRelativeFormationLocation(IFormationUnit unit)
		{
			return null;
		}

		// Token: 0x06000E4D RID: 3661 RVA: 0x00029000 File Offset: 0x00027200
		public Vec2? GetLocalWallDirectionOfRelativeFormationLocation(IFormationUnit unit)
		{
			return null;
		}

		// Token: 0x06000E4E RID: 3662 RVA: 0x00029016 File Offset: 0x00027216
		public IEnumerable<Vec2> GetUnavailableUnitPositions()
		{
			yield break;
		}

		// Token: 0x06000E4F RID: 3663 RVA: 0x0002901F File Offset: 0x0002721F
		public float GetOccupationWidth(int unitCount)
		{
			return this.FlankWidth;
		}

		// Token: 0x06000E50 RID: 3664 RVA: 0x00029028 File Offset: 0x00027228
		public Vec2? CreateNewPosition(int unitIndex)
		{
			int num = MathF.Ceiling((float)unitIndex * 1f / (float)this.ColumnCount);
			if (num > this.RankCount)
			{
				this._units2D.ResetWithNewCount(this.ColumnCount, num);
				this.ReconstructUnitsFromUnits2D();
			}
			Action onShapeChanged = this.OnShapeChanged;
			if (onShapeChanged != null)
			{
				onShapeChanged();
			}
			return null;
		}

		// Token: 0x06000E51 RID: 3665 RVA: 0x00029086 File Offset: 0x00027286
		public void InvalidateCacheOfUnitAux(Vec2 roundedLocalPosition)
		{
		}

		// Token: 0x06000E52 RID: 3666 RVA: 0x00029088 File Offset: 0x00027288
		public void BeforeFormationFrameChange()
		{
		}

		// Token: 0x06000E53 RID: 3667 RVA: 0x0002908A File Offset: 0x0002728A
		public void OnFormationFrameChanged()
		{
		}

		// Token: 0x06000E54 RID: 3668 RVA: 0x0002908C File Offset: 0x0002728C
		private Vec2 CalculateArrangementOrientation()
		{
			IFormationUnit formationUnit = this.Vanguard ?? this._units2D[this.GetMiddleFrontUnitPosition().Item1, this.GetMiddleFrontUnitPosition().Item2];
			if (formationUnit is Agent && this.owner is Formation)
			{
				return ((formationUnit as Agent).Position.AsVec2 - (this.owner as Formation).QuerySystem.MedianPosition.AsVec2).Normalized();
			}
			Debug.FailedAssert("Unexpected case", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\AI\\Formation\\ColumnFormation.cs", "CalculateArrangementOrientation", 1251);
			return this.GetLocalDirectionOfUnit(formationUnit.FormationFileIndex, formationUnit.FormationRankIndex);
		}

		// Token: 0x06000E55 RID: 3669 RVA: 0x00029143 File Offset: 0x00027343
		public void OnUnitLostMount(IFormationUnit unit)
		{
			this.RemoveUnit(unit);
			this.AddUnit(unit);
		}

		// Token: 0x06000E56 RID: 3670 RVA: 0x00029154 File Offset: 0x00027354
		public bool IsTurnBackwardsNecessary(Vec2 previousPosition, WorldPosition? newPosition, Vec2 previousDirection, bool hasNewDirection, Vec2? newDirection)
		{
			return newPosition != null && this.RankCount > 0 && (newPosition.Value.AsVec2 - previousPosition).LengthSquared >= this.RankDepth * this.RankDepth && MathF.Abs(MBMath.GetSmallestDifferenceBetweenTwoAngles(this.CalculateArrangementOrientation().RotationInRadians, (newPosition.Value.AsVec2 - previousPosition).Normalized().RotationInRadians)) >= 2.3561945f;
		}

		// Token: 0x06000E57 RID: 3671 RVA: 0x000291E8 File Offset: 0x000273E8
		public void TurnBackwards()
		{
			if (!this.IsMiddleFrontUnitPositionReserved && !this._isMiddleFrontUnitPositionUsedByVanguardInFormation && this.RankCount > 1)
			{
				bool isMiddleFrontUnitPositionReserved = this.IsMiddleFrontUnitPositionReserved;
				IFormationUnit vanguard = this._vanguard;
				if (isMiddleFrontUnitPositionReserved)
				{
					this.ReleaseMiddleFrontUnitPosition();
				}
				int rankCount = this.RankCount;
				for (int i = 0; i < rankCount / 2; i++)
				{
					for (int j = 0; j < this.FileCount; j++)
					{
						IFormationUnit formationUnit = this._units2D[j, i];
						int num = rankCount - i - 1;
						int num2 = j;
						IFormationUnit formationUnit2 = this._units2D[num2, num];
						if (formationUnit2 == null)
						{
							this._units2D[num2, num] = formationUnit;
							this._units2D[j, i] = null;
							if (formationUnit != null)
							{
								formationUnit.FormationFileIndex = num2;
								formationUnit.FormationRankIndex = num;
							}
						}
						else if (formationUnit != null && formationUnit != formationUnit2)
						{
							this.SwitchUnitLocationsAux(formationUnit, formationUnit2);
						}
					}
				}
				for (int k = 0; k < this.FileCount; k++)
				{
					if (this._units2D[k, 0] == null && this._units2D[k, 1] != null)
					{
						for (int l = 1; l < rankCount; l++)
						{
							IFormationUnit formationUnit3 = this._units2D[k, l];
							IFormationUnit formationUnit4 = formationUnit3;
							int formationRankIndex = formationUnit4.FormationRankIndex;
							formationUnit4.FormationRankIndex = formationRankIndex - 1;
							this._units2D[k, l - 1] = formationUnit3;
							this._units2D[k, l] = null;
						}
					}
				}
				this.isExpandingFromRightSide = !this.isExpandingFromRightSide;
				this.ReconstructUnitsFromUnits2D();
				foreach (IFormationUnit formationUnit5 in this.GetAllUnits())
				{
					int num3;
					IFormationUnit unitToFollow = this.GetUnitToFollow(formationUnit5, out num3);
					this.SetUnitToFollow(formationUnit5, unitToFollow, num3);
				}
				Action onShapeChanged = this.OnShapeChanged;
				if (onShapeChanged != null)
				{
					onShapeChanged();
				}
				if (isMiddleFrontUnitPositionReserved)
				{
					this.ReserveMiddleFrontUnitPosition(vanguard);
				}
				Action onShapeChanged2 = this.OnShapeChanged;
				if (onShapeChanged2 == null)
				{
					return;
				}
				onShapeChanged2();
			}
		}

		// Token: 0x06000E58 RID: 3672 RVA: 0x00029408 File Offset: 0x00027608
		public void OnFormationDispersed()
		{
			foreach (IFormationUnit formationUnit in this.GetAllUnits().ToArray())
			{
				this.SwitchUnitIfLeftBehind(formationUnit);
			}
		}

		// Token: 0x06000E59 RID: 3673 RVA: 0x0002943A File Offset: 0x0002763A
		public void Reset()
		{
			this._units2D.ResetWithNewCount(this.ColumnCount, 1);
			this.ReconstructUnitsFromUnits2D();
			Action onShapeChanged = this.OnShapeChanged;
			if (onShapeChanged == null)
			{
				return;
			}
			onShapeChanged();
		}

		// Token: 0x14000004 RID: 4
		// (add) Token: 0x06000E5A RID: 3674 RVA: 0x00029464 File Offset: 0x00027664
		// (remove) Token: 0x06000E5B RID: 3675 RVA: 0x0002949C File Offset: 0x0002769C
		public event Action OnWidthChanged;

		// Token: 0x14000005 RID: 5
		// (add) Token: 0x06000E5C RID: 3676 RVA: 0x000294D4 File Offset: 0x000276D4
		// (remove) Token: 0x06000E5D RID: 3677 RVA: 0x0002950C File Offset: 0x0002770C
		public event Action OnShapeChanged;

		// Token: 0x06000E5E RID: 3678 RVA: 0x00029541 File Offset: 0x00027741
		public virtual void RearrangeFrom(IFormationArrangement arrangement)
		{
		}

		// Token: 0x06000E5F RID: 3679 RVA: 0x00029543 File Offset: 0x00027743
		public virtual void RearrangeTo(IFormationArrangement arrangement)
		{
		}

		// Token: 0x06000E60 RID: 3680 RVA: 0x00029548 File Offset: 0x00027748
		public virtual void RearrangeTransferUnits(IFormationArrangement arrangement)
		{
			foreach (ValueTuple<int, int> valueTuple in this.GetOrderedUnitPositionIndices().ToList<ValueTuple<int, int>>())
			{
				IFormationUnit formationUnit = this._units2D[valueTuple.Item1, valueTuple.Item2];
				if (formationUnit != null)
				{
					formationUnit.FormationFileIndex = -1;
					formationUnit.FormationRankIndex = -1;
					this.SetUnitToFollow(formationUnit, null, 0);
					arrangement.AddUnit(formationUnit);
				}
			}
		}

		// Token: 0x06000E61 RID: 3681 RVA: 0x000295D4 File Offset: 0x000277D4
		private void SetVanguard(IFormationUnit vanguard)
		{
			if (this.Vanguard != null || vanguard != null)
			{
				bool flag = false;
				bool flag2 = false;
				if (this.Vanguard == null && vanguard != null)
				{
					flag2 = true;
				}
				else if (this.Vanguard != null && vanguard == null)
				{
					flag = true;
				}
				ValueTuple<int, int> middleFrontUnitPosition = this.GetMiddleFrontUnitPosition();
				if (flag)
				{
					Agent agent = this.Vanguard as Agent;
					if (((agent != null) ? agent.Formation : null) == this.owner)
					{
						this.RemoveUnit(this.Vanguard);
						this.AddUnit(this.Vanguard);
					}
					else if (this.RankCount > 0)
					{
						this.ShiftUnitsForward(middleFrontUnitPosition.Item1, middleFrontUnitPosition.Item2);
					}
				}
				else if (flag2)
				{
					Agent agent2 = vanguard as Agent;
					if (((agent2 != null) ? agent2.Formation : null) == this.owner)
					{
						this.RemoveUnit(vanguard);
						this.ShiftUnitsBackwardForMakingRoomForVanguard(middleFrontUnitPosition.Item1, middleFrontUnitPosition.Item2);
						if (this.RankCount > 0)
						{
							this._units2D[middleFrontUnitPosition.Item1, middleFrontUnitPosition.Item2] = vanguard;
							this.ReconstructUnitsFromUnits2D();
							vanguard.FormationFileIndex = middleFrontUnitPosition.Item1;
							vanguard.FormationRankIndex = middleFrontUnitPosition.Item2;
							if (this.RankCount == 2)
							{
								this.AdjustFollowDataOfUnitPosition(middleFrontUnitPosition.Item1, middleFrontUnitPosition.Item2);
								this.AdjustFollowDataOfUnitPosition(middleFrontUnitPosition.Item1, middleFrontUnitPosition.Item2 + 1);
								Action onShapeChanged = this.OnShapeChanged;
								if (onShapeChanged != null)
								{
									onShapeChanged();
								}
							}
						}
						else
						{
							this.AddUnit(vanguard);
						}
					}
					else
					{
						this.ShiftUnitsBackwardForMakingRoomForVanguard(middleFrontUnitPosition.Item1, middleFrontUnitPosition.Item2);
					}
				}
				this._vanguard = vanguard;
				if (this.RankCount > 0)
				{
					this.AdjustFollowDataOfUnitPosition(middleFrontUnitPosition.Item1, middleFrontUnitPosition.Item2);
				}
			}
		}

		// Token: 0x17000340 RID: 832
		// (get) Token: 0x06000E62 RID: 3682 RVA: 0x00029774 File Offset: 0x00027974
		public int UnitCount
		{
			get
			{
				return this.GetAllUnits().Count;
			}
		}

		// Token: 0x17000341 RID: 833
		// (get) Token: 0x06000E63 RID: 3683 RVA: 0x00029781 File Offset: 0x00027981
		public int PositionedUnitCount
		{
			get
			{
				return this.UnitCount;
			}
		}

		// Token: 0x06000E64 RID: 3684 RVA: 0x0002978C File Offset: 0x0002798C
		protected int GetUnitCountWithOverride()
		{
			int num;
			if (this.owner.OverridenUnitCount != null)
			{
				num = this.owner.OverridenUnitCount.Value;
			}
			else
			{
				num = this.UnitCount;
			}
			return num;
		}

		// Token: 0x06000E65 RID: 3685 RVA: 0x000297CC File Offset: 0x000279CC
		private void SetColumnCount(int columnCount)
		{
			if (this.ColumnCount != columnCount)
			{
				IFormationUnit[] array = this.GetAllUnits().ToArray();
				this._units2D.ResetWithNewCount(columnCount, 1);
				this.ReconstructUnitsFromUnits2D();
				foreach (IFormationUnit formationUnit in array)
				{
					formationUnit.FormationFileIndex = -1;
					formationUnit.FormationRankIndex = -1;
					this.AddUnit(formationUnit);
				}
				Action onShapeChanged = this.OnShapeChanged;
				if (onShapeChanged == null)
				{
					return;
				}
				onShapeChanged();
			}
		}

		// Token: 0x06000E66 RID: 3686 RVA: 0x00029839 File Offset: 0x00027A39
		public void FormFromWidth(float width)
		{
			this.ColumnCount = MathF.Ceiling(width);
		}

		// Token: 0x06000E67 RID: 3687 RVA: 0x00029848 File Offset: 0x00027A48
		public IFormationUnit GetNeighborUnitOfLeftSide(IFormationUnit unit)
		{
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

		// Token: 0x06000E68 RID: 3688 RVA: 0x00029890 File Offset: 0x00027A90
		public IFormationUnit GetNeighborUnitOfRightSide(IFormationUnit unit)
		{
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

		// Token: 0x06000E69 RID: 3689 RVA: 0x000298DA File Offset: 0x00027ADA
		public void ReserveMiddleFrontUnitPosition(IFormationUnit vanguard)
		{
			Agent agent = vanguard as Agent;
			if (((agent != null) ? agent.Formation : null) != this.owner)
			{
				this.IsMiddleFrontUnitPositionReserved = true;
			}
			else
			{
				this._isMiddleFrontUnitPositionUsedByVanguardInFormation = true;
			}
			this.Vanguard = vanguard;
		}

		// Token: 0x06000E6A RID: 3690 RVA: 0x0002990D File Offset: 0x00027B0D
		public void ReleaseMiddleFrontUnitPosition()
		{
			this.IsMiddleFrontUnitPositionReserved = false;
			this.Vanguard = null;
			this._isMiddleFrontUnitPositionUsedByVanguardInFormation = false;
		}

		// Token: 0x06000E6B RID: 3691 RVA: 0x00029924 File Offset: 0x00027B24
		private ValueTuple<int, int> GetMiddleFrontUnitPosition()
		{
			return new ValueTuple<int, int>(this.VanguardFileIndex, 0);
		}

		// Token: 0x06000E6C RID: 3692 RVA: 0x00029932 File Offset: 0x00027B32
		public Vec2 GetLocalPositionOfReservedUnitPosition()
		{
			return Vec2.Zero;
		}

		// Token: 0x06000E6D RID: 3693 RVA: 0x0002993C File Offset: 0x00027B3C
		public void OnTickOccasionallyOfUnit(IFormationUnit unit, bool arrangementChangeAllowed)
		{
			if (arrangementChangeAllowed && unit.FollowedUnit != this._vanguard && unit.FollowedUnit is Agent && !((Agent)unit.FollowedUnit).IsAIControlled && unit.FollowedUnit.FormationFileIndex >= 0 && unit.FollowedUnit.FormationRankIndex >= 0)
			{
				if (unit.FollowedUnit.FormationFileIndex * this._units2D.Count2 + unit.FollowedUnit.FormationRankIndex >= this._units2D.RawArray.Length || unit.FollowedUnit.FormationFileIndex * this._units2D.Count2 + unit.FollowedUnit.FormationRankIndex < 0)
				{
					Debug.Print("Followed unit has illegal formation indices!", 0, Debug.DebugColor.White, 17592186044416UL);
					Debug.Print(string.Concat(new object[] { "RankIndex: ", unit.FormationRankIndex, " FileIndex: ", unit.FormationFileIndex }), 0, Debug.DebugColor.White, 17592186044416UL);
					Debug.Print(string.Concat(new object[]
					{
						"_units2D.Capacity: ",
						this._units2D.RawArray.Length,
						" _units2D.Count1: ",
						this._units2D.Count1,
						" _units2D.Count2: ",
						this._units2D.Count2
					}), 0, Debug.DebugColor.White, 17592186044416UL);
					Debug.Print(string.Concat(new object[]
					{
						"FollowedUnit.RankIndex: ",
						unit.FollowedUnit.FormationRankIndex,
						" FollowedUnit.FileIndex: ",
						unit.FollowedUnit.FormationFileIndex
					}), 0, Debug.DebugColor.White, 17592186044416UL);
					if (!(unit.FollowedUnit.Formation is ColumnFormation))
					{
						Debug.Print("Followed unit is not in column formation", 0, Debug.DebugColor.White, 17592186044416UL);
					}
					if (((Agent)unit.FollowedUnit).IsPlayerControlled)
					{
						Debug.Print("Followed unit is player", 0, Debug.DebugColor.White, 17592186044416UL);
					}
					if (((Agent)unit).Formation.Captain == (Agent)unit.FollowedUnit)
					{
						Debug.Print("Followed unit is the captain", 0, Debug.DebugColor.White, 17592186044416UL);
					}
					Debug.Print("-------------------------------------", 0, Debug.DebugColor.White, 17592186044416UL);
					foreach (IFormationUnit formationUnit in unit.FollowedUnit.Formation.GetAllUnits())
					{
						Debug.Print(string.Concat(new object[]
						{
							"R: ",
							formationUnit.FormationRankIndex,
							" F: ",
							formationUnit.FormationFileIndex,
							" AI: ",
							((Agent)formationUnit).IsAIControlled ? "1" : "0"
						}), 0, Debug.DebugColor.White, 17592186044416UL);
					}
					Debug.Print("-------------------------------------", 0, Debug.DebugColor.White, 17592186044416UL);
				}
				IFormationUnit followedUnit = unit.FollowedUnit;
				this.RemoveUnit(unit.FollowedUnit);
				this.AddUnit(followedUnit);
			}
		}

		// Token: 0x06000E6E RID: 3694 RVA: 0x00029CA0 File Offset: 0x00027EA0
		private MBList<IFormationUnit> GetUnitsBehind(IFormationUnit unit)
		{
			MBList<IFormationUnit> mblist = new MBList<IFormationUnit>();
			bool flag = false;
			for (int i = 0; i < this.ColumnCount; i++)
			{
				int columnOffsetFromColumnIndex = ColumnFormation.GetColumnOffsetFromColumnIndex(i, this.isExpandingFromRightSide);
				int num = this.VanguardFileIndex + columnOffsetFromColumnIndex;
				if (num == unit.FormationFileIndex)
				{
					flag = true;
				}
				if (flag && this._units2D[num, unit.FormationRankIndex] != null)
				{
					mblist.Add(this._units2D[num, unit.FormationRankIndex]);
				}
			}
			for (int j = 0; j < this.FileCount; j++)
			{
				for (int k = unit.FormationRankIndex + 1; k < this.RankCount; k++)
				{
					if (this._units2D[j, k] != null)
					{
						mblist.Add(this._units2D[j, k]);
					}
				}
			}
			return mblist;
		}

		// Token: 0x06000E6F RID: 3695 RVA: 0x00029D74 File Offset: 0x00027F74
		private void SwitchUnitIfLeftBehind(IFormationUnit unit)
		{
			int num;
			IFormationUnit unitToFollow = this.GetUnitToFollow(unit, out num);
			if (unitToFollow == null)
			{
				float num2 = this.owner.UnitDiameter * 2f;
				IFormationUnit formationUnit = this.owner.GetClosestUnitTo(Vec2.Zero, new MBList<IFormationUnit> { unit }, new float?(num2));
				if (formationUnit == null)
				{
					formationUnit = this.owner.GetClosestUnitTo(Vec2.Zero, this.GetUnitsAtRanks(0, this.RankCount - 1), null);
				}
				if (formationUnit != null && formationUnit != unit && formationUnit is Agent && (formationUnit as Agent).IsAIControlled)
				{
					this.SwitchUnitLocations(unit, formationUnit);
					return;
				}
			}
			else
			{
				float num3 = this.GetFollowVector(num).Length * 1.5f;
				IFormationUnit formationUnit2 = this.owner.GetClosestUnitTo(unitToFollow, new MBList<IFormationUnit> { unit }, new float?(num3));
				if (formationUnit2 == null)
				{
					formationUnit2 = this.owner.GetClosestUnitTo(unitToFollow, this.GetUnitsBehind(unit), null);
				}
				Agent agent;
				if (formationUnit2 != null && formationUnit2 != unit && (agent = formationUnit2 as Agent) != null && agent.IsAIControlled)
				{
					this.SwitchUnitLocations(unit, agent);
				}
			}
		}

		// Token: 0x06000E70 RID: 3696 RVA: 0x00029EA8 File Offset: 0x000280A8
		private void SetUnitToFollow(IFormationUnit unit, IFormationUnit unitToFollow, int columnOffset = 0)
		{
			Vec2 followVector = this.GetFollowVector(columnOffset);
			this.owner.SetUnitToFollow(unit, unitToFollow, followVector);
		}

		// Token: 0x06000E71 RID: 3697 RVA: 0x00029ECC File Offset: 0x000280CC
		private Vec2 GetFollowVector(int columnOffset)
		{
			Vec2 vec;
			if (columnOffset == 0)
			{
				vec = -Vec2.Forward * (this.Distance + this.owner.UnitDiameter);
			}
			else
			{
				vec = Vec2.Side * (float)columnOffset * (this.owner.UnitDiameter + this.Interval);
			}
			return vec;
		}

		// Token: 0x06000E72 RID: 3698 RVA: 0x00029F25 File Offset: 0x00028125
		public float GetDirectionChangeTendencyOfUnit(IFormationUnit unit)
		{
			if (this.RankCount == 1 || unit.FormationRankIndex == -1)
			{
				return 0f;
			}
			return (float)unit.FormationRankIndex * 1f / (float)(this.RankCount - 1);
		}

		// Token: 0x06000E73 RID: 3699 RVA: 0x00029F58 File Offset: 0x00028158
		private MBList<IFormationUnit> GetUnitsAtRanks(int rankIndex1, int rankIndex2)
		{
			MBList<IFormationUnit> mblist = new MBList<IFormationUnit>();
			for (int i = 0; i < this.ColumnCount; i++)
			{
				int columnOffsetFromColumnIndex = ColumnFormation.GetColumnOffsetFromColumnIndex(i, this.isExpandingFromRightSide);
				int num = this.VanguardFileIndex + columnOffsetFromColumnIndex;
				if (this._units2D[num, rankIndex1] != null)
				{
					mblist.Add(this._units2D[num, rankIndex1]);
				}
			}
			for (int j = 0; j < this.ColumnCount; j++)
			{
				int columnOffsetFromColumnIndex2 = ColumnFormation.GetColumnOffsetFromColumnIndex(j, this.isExpandingFromRightSide);
				int num2 = this.VanguardFileIndex + columnOffsetFromColumnIndex2;
				if (this._units2D[num2, rankIndex2] != null)
				{
					mblist.Add(this._units2D[num2, rankIndex2]);
				}
			}
			return mblist;
		}

		// Token: 0x06000E74 RID: 3700 RVA: 0x0002A008 File Offset: 0x00028208
		public IEnumerable<T> GetUnitsAtVanguardFile<T>() where T : IFormationUnit
		{
			int fileIndex = this.VanguardFileIndex;
			int num;
			for (int rankIndex = 0; rankIndex < this.RankCount; rankIndex = num + 1)
			{
				if (this._units2D[fileIndex, rankIndex] != null)
				{
					yield return (T)((object)this._units2D[fileIndex, rankIndex]);
				}
				num = rankIndex;
			}
			yield break;
		}

		// Token: 0x17000342 RID: 834
		// (set) Token: 0x06000E75 RID: 3701 RVA: 0x0002A018 File Offset: 0x00028218
		bool IFormationArrangement.AreLocalPositionsDirty
		{
			set
			{
			}
		}

		// Token: 0x0400037C RID: 892
		private readonly IFormation owner;

		// Token: 0x0400037D RID: 893
		private IFormationUnit _vanguard;

		// Token: 0x0400037E RID: 894
		private MBList2D<IFormationUnit> _units2D;

		// Token: 0x0400037F RID: 895
		private MBList2D<IFormationUnit> _units2DWorkspace;

		// Token: 0x04000380 RID: 896
		private MBList<IFormationUnit> _allUnits;

		// Token: 0x04000381 RID: 897
		private bool isExpandingFromRightSide = true;

		// Token: 0x04000382 RID: 898
		private bool IsMiddleFrontUnitPositionReserved;

		// Token: 0x04000383 RID: 899
		private bool _isMiddleFrontUnitPositionUsedByVanguardInFormation;
	}
}
