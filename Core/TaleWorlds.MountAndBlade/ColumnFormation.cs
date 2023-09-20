using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	public class ColumnFormation : IFormationArrangement
	{
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

		protected int FileCount
		{
			get
			{
				return this._units2D.Count1;
			}
		}

		public int RankCount
		{
			get
			{
				return this._units2D.Count2;
			}
		}

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

		protected float Distance
		{
			get
			{
				return this.owner.Distance * 1f + 0.5f;
			}
		}

		protected float Interval
		{
			get
			{
				return this.owner.Interval * 1.5f;
			}
		}

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

		public IFormationArrangement Clone(IFormation formation)
		{
			return new ColumnFormation(formation, this.Vanguard, this.ColumnCount);
		}

		public void DeepCopyFrom(IFormationArrangement arrangement)
		{
		}

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

		public float Depth
		{
			get
			{
				return this.RankDepth;
			}
		}

		public float RankDepth
		{
			get
			{
				return (float)(this.RankCount - 1) * (this.Distance + this.owner.UnitDiameter) + this.owner.UnitDiameter;
			}
		}

		public float MinimumWidth
		{
			get
			{
				return this.MinimumFlankWidth;
			}
		}

		public IFormationUnit GetPlayerUnit()
		{
			return this._allUnits.FirstOrDefault((IFormationUnit unit) => unit.IsPlayerUnit);
		}

		public float MaximumWidth
		{
			get
			{
				return this.MinimumFlankWidth;
			}
		}

		public float MinimumFlankWidth
		{
			get
			{
				return this.FlankWidth;
			}
		}

		public MBList<IFormationUnit> GetAllUnits()
		{
			return this._allUnits;
		}

		public MBList<IFormationUnit> GetUnpositionedUnits()
		{
			return null;
		}

		public bool? IsLoose
		{
			get
			{
				return new bool?(false);
			}
		}

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

		private void Deepen()
		{
			ColumnFormation.Deepen(this);
		}

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

		private void Shorten()
		{
			ColumnFormation.Shorten(this);
		}

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

		private IFormationUnit TryGetUnit(int fileIndex, int rankIndex)
		{
			if (fileIndex >= 0 && fileIndex < this.FileCount && rankIndex >= 0 && rankIndex < this.RankCount)
			{
				return this._units2D[fileIndex, rankIndex];
			}
			return null;
		}

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

		public IFormationUnit GetUnit(int fileIndex, int rankIndex)
		{
			return this._units2D[fileIndex, rankIndex];
		}

		public void OnBatchRemoveStart()
		{
		}

		public void OnBatchRemoveEnd()
		{
		}

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

		private Vec2 GetLocalPositionOfUnit(int fileIndex, int rankIndex)
		{
			float num = (float)(this.FileCount - 1) * (this.owner.Interval + this.owner.UnitDiameter);
			return new Vec2((float)fileIndex * (this.owner.Interval + this.owner.UnitDiameter) - num / 2f, (float)(-(float)rankIndex) * (this.owner.Distance + this.owner.UnitDiameter));
		}

		private Vec2 GetLocalDirectionOfUnit(int fileIndex, int rankIndex)
		{
			return Vec2.Forward;
		}

		private WorldPosition? GetWorldPositionOfUnit(int fileIndex, int rankIndex)
		{
			return null;
		}

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

		public Vec2? GetLocalPositionOfUnitOrDefault(IFormationUnit unit)
		{
			return new Vec2?(this.GetLocalPositionOfUnit(unit.FormationFileIndex, unit.FormationRankIndex));
		}

		public Vec2? GetLocalPositionOfUnitOrDefaultWithAdjustment(IFormationUnit unit, float distanceBetweenAgentsAdjustment)
		{
			return new Vec2?(this.GetLocalPositionOfUnit(unit.FormationFileIndex, unit.FormationRankIndex));
		}

		public WorldPosition? GetWorldPositionOfUnitOrDefault(IFormationUnit unit)
		{
			return this.GetWorldPositionOfUnit(unit.FormationFileIndex, unit.FormationRankIndex);
		}

		public Vec2? GetLocalDirectionOfUnitOrDefault(IFormationUnit unit)
		{
			return new Vec2?(this.GetLocalDirectionOfUnit(unit.FormationFileIndex, unit.FormationRankIndex));
		}

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

		public List<IFormationUnit> GetUnitsToPop(int count, Vec3 targetPosition)
		{
			return this.GetUnitsToPop(count);
		}

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

		public void SwitchUnitLocations(IFormationUnit firstUnit, IFormationUnit secondUnit)
		{
			this.SwitchUnitLocationsAux(firstUnit, secondUnit);
			this.AdjustFollowDataOfUnitPosition(firstUnit.FormationFileIndex, firstUnit.FormationRankIndex);
			this.AdjustFollowDataOfUnitPosition(secondUnit.FormationFileIndex, secondUnit.FormationRankIndex);
		}

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

		public void SwitchUnitLocationsWithUnpositionedUnit(IFormationUnit firstUnit, IFormationUnit secondUnit)
		{
			Debug.FailedAssert("Column formation should NOT have an unpositioned unit", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\AI\\Formation\\ColumnFormation.cs", "SwitchUnitLocationsWithUnpositionedUnit", 1168);
		}

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

		public float GetUnitsDistanceToFrontLine(IFormationUnit unit)
		{
			return -1f;
		}

		public Vec2? GetLocalDirectionOfRelativeFormationLocation(IFormationUnit unit)
		{
			return null;
		}

		public Vec2? GetLocalWallDirectionOfRelativeFormationLocation(IFormationUnit unit)
		{
			return null;
		}

		public IEnumerable<Vec2> GetUnavailableUnitPositions()
		{
			yield break;
		}

		public float GetOccupationWidth(int unitCount)
		{
			return this.FlankWidth;
		}

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

		public void InvalidateCacheOfUnitAux(Vec2 roundedLocalPosition)
		{
		}

		public void BeforeFormationFrameChange()
		{
		}

		public void OnFormationFrameChanged()
		{
		}

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

		public void OnUnitLostMount(IFormationUnit unit)
		{
			this.RemoveUnit(unit);
			this.AddUnit(unit);
		}

		public bool IsTurnBackwardsNecessary(Vec2 previousPosition, WorldPosition? newPosition, Vec2 previousDirection, bool hasNewDirection, Vec2? newDirection)
		{
			return newPosition != null && this.RankCount > 0 && (newPosition.Value.AsVec2 - previousPosition).LengthSquared >= this.RankDepth * this.RankDepth && MathF.Abs(MBMath.GetSmallestDifferenceBetweenTwoAngles(this.CalculateArrangementOrientation().RotationInRadians, (newPosition.Value.AsVec2 - previousPosition).Normalized().RotationInRadians)) >= 2.3561945f;
		}

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

		public void OnFormationDispersed()
		{
			foreach (IFormationUnit formationUnit in this.GetAllUnits().ToArray())
			{
				this.SwitchUnitIfLeftBehind(formationUnit);
			}
		}

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

		public event Action OnWidthChanged;

		public event Action OnShapeChanged;

		public virtual void RearrangeFrom(IFormationArrangement arrangement)
		{
		}

		public virtual void RearrangeTo(IFormationArrangement arrangement)
		{
		}

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

		public int UnitCount
		{
			get
			{
				return this.GetAllUnits().Count;
			}
		}

		public int PositionedUnitCount
		{
			get
			{
				return this.UnitCount;
			}
		}

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

		public void FormFromWidth(float width)
		{
			this.ColumnCount = MathF.Ceiling(width);
		}

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

		public void ReleaseMiddleFrontUnitPosition()
		{
			this.IsMiddleFrontUnitPositionReserved = false;
			this.Vanguard = null;
			this._isMiddleFrontUnitPositionUsedByVanguardInFormation = false;
		}

		private ValueTuple<int, int> GetMiddleFrontUnitPosition()
		{
			return new ValueTuple<int, int>(this.VanguardFileIndex, 0);
		}

		public Vec2 GetLocalPositionOfReservedUnitPosition()
		{
			return Vec2.Zero;
		}

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

		private void SetUnitToFollow(IFormationUnit unit, IFormationUnit unitToFollow, int columnOffset = 0)
		{
			Vec2 followVector = this.GetFollowVector(columnOffset);
			this.owner.SetUnitToFollow(unit, unitToFollow, followVector);
		}

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

		public float GetDirectionChangeTendencyOfUnit(IFormationUnit unit)
		{
			if (this.RankCount == 1 || unit.FormationRankIndex == -1)
			{
				return 0f;
			}
			return (float)unit.FormationRankIndex * 1f / (float)(this.RankCount - 1);
		}

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

		bool IFormationArrangement.AreLocalPositionsDirty
		{
			set
			{
			}
		}

		private readonly IFormation owner;

		private IFormationUnit _vanguard;

		private MBList2D<IFormationUnit> _units2D;

		private MBList2D<IFormationUnit> _units2DWorkspace;

		private MBList<IFormationUnit> _allUnits;

		private bool isExpandingFromRightSide = true;

		private bool IsMiddleFrontUnitPositionReserved;

		private bool _isMiddleFrontUnitPositionUsedByVanguardInFormation;
	}
}
