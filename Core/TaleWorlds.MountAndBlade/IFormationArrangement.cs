using System;
using System.Collections.Generic;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000130 RID: 304
	public interface IFormationArrangement
	{
		// Token: 0x1700034B RID: 843
		// (get) Token: 0x06000E98 RID: 3736
		// (set) Token: 0x06000E99 RID: 3737
		float Width { get; set; }

		// Token: 0x1700034C RID: 844
		// (get) Token: 0x06000E9A RID: 3738
		float Depth { get; }

		// Token: 0x1700034D RID: 845
		// (get) Token: 0x06000E9B RID: 3739
		// (set) Token: 0x06000E9C RID: 3740
		float FlankWidth { get; set; }

		// Token: 0x1700034E RID: 846
		// (get) Token: 0x06000E9D RID: 3741
		float RankDepth { get; }

		// Token: 0x1700034F RID: 847
		// (get) Token: 0x06000E9E RID: 3742
		float MinimumWidth { get; }

		// Token: 0x17000350 RID: 848
		// (get) Token: 0x06000E9F RID: 3743
		float MaximumWidth { get; }

		// Token: 0x17000351 RID: 849
		// (get) Token: 0x06000EA0 RID: 3744
		float MinimumFlankWidth { get; }

		// Token: 0x17000352 RID: 850
		// (get) Token: 0x06000EA1 RID: 3745
		bool? IsLoose { get; }

		// Token: 0x06000EA2 RID: 3746
		IFormationUnit GetPlayerUnit();

		// Token: 0x06000EA3 RID: 3747
		MBList<IFormationUnit> GetAllUnits();

		// Token: 0x06000EA4 RID: 3748
		MBList<IFormationUnit> GetUnpositionedUnits();

		// Token: 0x17000353 RID: 851
		// (get) Token: 0x06000EA5 RID: 3749
		int UnitCount { get; }

		// Token: 0x17000354 RID: 852
		// (get) Token: 0x06000EA6 RID: 3750
		int RankCount { get; }

		// Token: 0x17000355 RID: 853
		// (get) Token: 0x06000EA7 RID: 3751
		int PositionedUnitCount { get; }

		// Token: 0x06000EA8 RID: 3752
		bool AddUnit(IFormationUnit unit);

		// Token: 0x06000EA9 RID: 3753
		void RemoveUnit(IFormationUnit unit);

		// Token: 0x06000EAA RID: 3754
		IFormationUnit GetUnit(int fileIndex, int rankIndex);

		// Token: 0x06000EAB RID: 3755
		void OnBatchRemoveStart();

		// Token: 0x06000EAC RID: 3756
		void OnBatchRemoveEnd();

		// Token: 0x06000EAD RID: 3757
		Vec2? GetLocalPositionOfUnitOrDefault(int unitIndex);

		// Token: 0x06000EAE RID: 3758
		Vec2? GetLocalPositionOfUnitOrDefault(IFormationUnit unit);

		// Token: 0x06000EAF RID: 3759
		Vec2? GetLocalPositionOfUnitOrDefaultWithAdjustment(IFormationUnit unit, float distanceBetweenAgentsAdjustment);

		// Token: 0x06000EB0 RID: 3760
		Vec2? GetLocalDirectionOfUnitOrDefault(int unitIndex);

		// Token: 0x06000EB1 RID: 3761
		Vec2? GetLocalDirectionOfUnitOrDefault(IFormationUnit unit);

		// Token: 0x06000EB2 RID: 3762
		WorldPosition? GetWorldPositionOfUnitOrDefault(int unitIndex);

		// Token: 0x06000EB3 RID: 3763
		WorldPosition? GetWorldPositionOfUnitOrDefault(IFormationUnit unit);

		// Token: 0x06000EB4 RID: 3764
		List<IFormationUnit> GetUnitsToPop(int count);

		// Token: 0x06000EB5 RID: 3765
		List<IFormationUnit> GetUnitsToPop(int count, Vec3 targetPosition);

		// Token: 0x06000EB6 RID: 3766
		IEnumerable<IFormationUnit> GetUnitsToPopWithCondition(int count, Func<IFormationUnit, bool> conditionFunction);

		// Token: 0x06000EB7 RID: 3767
		void SwitchUnitLocations(IFormationUnit firstUnit, IFormationUnit secondUnit);

		// Token: 0x06000EB8 RID: 3768
		void SwitchUnitLocationsWithUnpositionedUnit(IFormationUnit firstUnit, IFormationUnit secondUnit);

		// Token: 0x06000EB9 RID: 3769
		void SwitchUnitLocationsWithBackMostUnit(IFormationUnit unit);

		// Token: 0x06000EBA RID: 3770
		IFormationUnit GetNeighborUnitOfLeftSide(IFormationUnit unit);

		// Token: 0x06000EBB RID: 3771
		IFormationUnit GetNeighborUnitOfRightSide(IFormationUnit unit);

		// Token: 0x06000EBC RID: 3772
		Vec2? GetLocalWallDirectionOfRelativeFormationLocation(IFormationUnit unit);

		// Token: 0x06000EBD RID: 3773
		IEnumerable<Vec2> GetUnavailableUnitPositions();

		// Token: 0x06000EBE RID: 3774
		float GetOccupationWidth(int unitCount);

		// Token: 0x06000EBF RID: 3775
		Vec2? CreateNewPosition(int unitIndex);

		// Token: 0x06000EC0 RID: 3776
		void BeforeFormationFrameChange();

		// Token: 0x06000EC1 RID: 3777
		void OnFormationFrameChanged();

		// Token: 0x06000EC2 RID: 3778
		bool IsTurnBackwardsNecessary(Vec2 previousPosition, WorldPosition? newPosition, Vec2 previousDirection, bool hasNewDirection, Vec2? newDirection);

		// Token: 0x06000EC3 RID: 3779
		void TurnBackwards();

		// Token: 0x06000EC4 RID: 3780
		void OnFormationDispersed();

		// Token: 0x06000EC5 RID: 3781
		void Reset();

		// Token: 0x06000EC6 RID: 3782
		IFormationArrangement Clone(IFormation formation);

		// Token: 0x06000EC7 RID: 3783
		void DeepCopyFrom(IFormationArrangement arrangement);

		// Token: 0x06000EC8 RID: 3784
		void RearrangeTo(IFormationArrangement arrangement);

		// Token: 0x06000EC9 RID: 3785
		void RearrangeFrom(IFormationArrangement arrangement);

		// Token: 0x06000ECA RID: 3786
		void RearrangeTransferUnits(IFormationArrangement arrangement);

		// Token: 0x14000006 RID: 6
		// (add) Token: 0x06000ECB RID: 3787
		// (remove) Token: 0x06000ECC RID: 3788
		event Action OnWidthChanged;

		// Token: 0x14000007 RID: 7
		// (add) Token: 0x06000ECD RID: 3789
		// (remove) Token: 0x06000ECE RID: 3790
		event Action OnShapeChanged;

		// Token: 0x06000ECF RID: 3791
		void ReserveMiddleFrontUnitPosition(IFormationUnit vanguard);

		// Token: 0x06000ED0 RID: 3792
		void ReleaseMiddleFrontUnitPosition();

		// Token: 0x06000ED1 RID: 3793
		Vec2 GetLocalPositionOfReservedUnitPosition();

		// Token: 0x06000ED2 RID: 3794
		void OnTickOccasionallyOfUnit(IFormationUnit unit, bool arrangementChangeAllowed);

		// Token: 0x06000ED3 RID: 3795
		void OnUnitLostMount(IFormationUnit unit);

		// Token: 0x06000ED4 RID: 3796
		float GetDirectionChangeTendencyOfUnit(IFormationUnit unit);

		// Token: 0x17000356 RID: 854
		// (set) Token: 0x06000ED5 RID: 3797
		bool AreLocalPositionsDirty { set; }
	}
}
