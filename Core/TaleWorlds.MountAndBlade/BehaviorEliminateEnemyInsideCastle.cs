using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Engine;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000105 RID: 261
	public class BehaviorEliminateEnemyInsideCastle : BehaviorComponent
	{
		// Token: 0x06000CCF RID: 3279 RVA: 0x0001CD30 File Offset: 0x0001AF30
		public BehaviorEliminateEnemyInsideCastle(Formation formation)
			: base(formation)
		{
			this._behaviorState = BehaviorEliminateEnemyInsideCastle.BehaviorState.UnSet;
			this._behaviorSide = formation.AI.Side;
			this.ResetOrderPositions();
		}

		// Token: 0x06000CD0 RID: 3280 RVA: 0x0001CD57 File Offset: 0x0001AF57
		protected override void CalculateCurrentOrder()
		{
			base.CalculateCurrentOrder();
			base.CurrentOrder = ((this._behaviorState == BehaviorEliminateEnemyInsideCastle.BehaviorState.Attacking) ? this._attackOrder : this._gatherOrder);
		}

		// Token: 0x06000CD1 RID: 3281 RVA: 0x0001CD7C File Offset: 0x0001AF7C
		private void DetermineMostImportantInvadingEnemyFormation()
		{
			float num = float.MinValue;
			this._targetEnemyFormation = null;
			foreach (Team team in base.Formation.Team.Mission.Teams)
			{
				if (team.IsEnemyOf(base.Formation.Team))
				{
					for (int i = 0; i < Math.Min(team.FormationsIncludingSpecialAndEmpty.Count, 8); i++)
					{
						Formation formation = team.FormationsIncludingSpecialAndEmpty[i];
						if (formation.CountOfUnits > 0 && TeamAISiegeComponent.IsFormationInsideCastle(formation, true, 0.4f))
						{
							float formationPower = formation.QuerySystem.FormationPower;
							if (formationPower > num)
							{
								num = formationPower;
								this._targetEnemyFormation = formation;
							}
						}
					}
				}
			}
		}

		// Token: 0x06000CD2 RID: 3282 RVA: 0x0001CE5C File Offset: 0x0001B05C
		private void ConfirmGatheringSide()
		{
			SiegeLane siegeLane = TeamAISiegeComponent.SiegeLanes.FirstOrDefault((SiegeLane sl) => sl.LaneSide == this._behaviorSide);
			if (siegeLane == null || siegeLane.LaneState >= SiegeLane.LaneStateEnum.Conceited)
			{
				this.ResetOrderPositions();
			}
		}

		// Token: 0x06000CD3 RID: 3283 RVA: 0x0001CE94 File Offset: 0x0001B094
		private FormationAI.BehaviorSide DetermineGatheringSide()
		{
			this.DetermineMostImportantInvadingEnemyFormation();
			if (this._targetEnemyFormation == null)
			{
				if (this._behaviorState == BehaviorEliminateEnemyInsideCastle.BehaviorState.Attacking)
				{
					this._behaviorState = BehaviorEliminateEnemyInsideCastle.BehaviorState.UnSet;
				}
				return this._behaviorSide;
			}
			int connectedSides = TeamAISiegeComponent.QuerySystem.DeterminePositionAssociatedSide(this._targetEnemyFormation.QuerySystem.MedianPosition.GetNavMeshVec3());
			IEnumerable<SiegeLane> enumerable = TeamAISiegeComponent.SiegeLanes.Where((SiegeLane sl) => sl.LaneState != SiegeLane.LaneStateEnum.Conceited && !SiegeQuerySystem.AreSidesRelated(sl.LaneSide, connectedSides));
			FormationAI.BehaviorSide behaviorSide = this._behaviorSide;
			if (enumerable.Any<SiegeLane>())
			{
				if (enumerable.Count<SiegeLane>() > 1)
				{
					int leastDangerousLaneState = enumerable.Min((SiegeLane pgl) => (int)pgl.LaneState);
					IEnumerable<SiegeLane> enumerable2 = enumerable.Where((SiegeLane pgl) => pgl.LaneState == (SiegeLane.LaneStateEnum)leastDangerousLaneState);
					behaviorSide = ((enumerable2.Count<SiegeLane>() > 1) ? enumerable2.MinBy((SiegeLane ldl) => SiegeQuerySystem.SideDistance(1 << connectedSides, 1 << (int)ldl.LaneSide)).LaneSide : enumerable2.First<SiegeLane>().LaneSide);
				}
				else
				{
					behaviorSide = enumerable.First<SiegeLane>().LaneSide;
				}
			}
			return behaviorSide;
		}

		// Token: 0x06000CD4 RID: 3284 RVA: 0x0001CFB0 File Offset: 0x0001B1B0
		private void ResetOrderPositions()
		{
			this._behaviorSide = this.DetermineGatheringSide();
			SiegeLane siegeLane = TeamAISiegeComponent.SiegeLanes.FirstOrDefault((SiegeLane sl) => sl.LaneSide == this._behaviorSide);
			WorldFrame? worldFrame;
			if (siegeLane == null)
			{
				worldFrame = null;
			}
			else
			{
				List<ICastleKeyPosition> defensePoints = siegeLane.DefensePoints;
				if (defensePoints == null)
				{
					worldFrame = null;
				}
				else
				{
					ICastleKeyPosition castleKeyPosition = defensePoints.FirstOrDefault((ICastleKeyPosition dp) => dp.AttackerSiegeWeapon is UsableMachine && !(dp.AttackerSiegeWeapon as UsableMachine).IsDisabled);
					worldFrame = ((castleKeyPosition != null) ? new WorldFrame?(castleKeyPosition.DefenseWaitFrame) : null);
				}
			}
			WorldFrame worldFrame2 = worldFrame ?? WorldFrame.Invalid;
			object obj;
			if (siegeLane == null)
			{
				obj = null;
			}
			else
			{
				List<ICastleKeyPosition> defensePoints2 = siegeLane.DefensePoints;
				if (defensePoints2 == null)
				{
					obj = null;
				}
				else
				{
					ICastleKeyPosition castleKeyPosition2 = defensePoints2.FirstOrDefault((ICastleKeyPosition dp) => dp.AttackerSiegeWeapon is UsableMachine && !(dp.AttackerSiegeWeapon as UsableMachine).IsDisabled);
					obj = ((castleKeyPosition2 != null) ? castleKeyPosition2.WaitPosition : null);
				}
			}
			object obj2;
			if ((obj2 = obj) == null)
			{
				if (siegeLane == null)
				{
					obj2 = null;
				}
				else
				{
					List<ICastleKeyPosition> defensePoints3 = siegeLane.DefensePoints;
					obj2 = ((defensePoints3 != null) ? defensePoints3.FirstOrDefault<ICastleKeyPosition>().WaitPosition : null);
				}
			}
			this._gatheringTacticalPos = obj2;
			if (this._gatheringTacticalPos != null)
			{
				this._gatherOrder = MovementOrder.MovementOrderMove(this._gatheringTacticalPos.Position);
				this._gatheringFacingOrder = FacingOrder.FacingOrderLookAtEnemy;
			}
			else if (worldFrame2.Origin.IsValid)
			{
				worldFrame2.Rotation.f.Normalize();
				this._gatherOrder = MovementOrder.MovementOrderMove(worldFrame2.Origin);
				this._gatheringFacingOrder = FacingOrder.FacingOrderLookAtEnemy;
			}
			else
			{
				this._gatherOrder = MovementOrder.MovementOrderMove(base.Formation.QuerySystem.MedianPosition);
				this._gatheringFacingOrder = FacingOrder.FacingOrderLookAtEnemy;
			}
			this._attackOrder = MovementOrder.MovementOrderChargeToTarget(this._targetEnemyFormation);
			this._attackFacingOrder = FacingOrder.FacingOrderLookAtEnemy;
			base.CurrentOrder = ((this._behaviorState == BehaviorEliminateEnemyInsideCastle.BehaviorState.Attacking) ? this._attackOrder : this._gatherOrder);
			this.CurrentFacingOrder = ((this._behaviorState == BehaviorEliminateEnemyInsideCastle.BehaviorState.Attacking) ? this._attackFacingOrder : this._gatheringFacingOrder);
		}

		// Token: 0x06000CD5 RID: 3285 RVA: 0x0001D1A8 File Offset: 0x0001B3A8
		public override void OnValidBehaviorSideChanged()
		{
			base.OnValidBehaviorSideChanged();
			this.ResetOrderPositions();
		}

		// Token: 0x06000CD6 RID: 3286 RVA: 0x0001D1B8 File Offset: 0x0001B3B8
		public override void TickOccasionally()
		{
			base.TickOccasionally();
			if (this._behaviorState != BehaviorEliminateEnemyInsideCastle.BehaviorState.Attacking)
			{
				this.ConfirmGatheringSide();
			}
			bool flag;
			if (this._behaviorState == BehaviorEliminateEnemyInsideCastle.BehaviorState.Attacking)
			{
				flag = this._targetEnemyFormation != null;
			}
			else
			{
				flag = this._targetEnemyFormation != null && (base.Formation.QuerySystem.MedianPosition.GetNavMeshVec3().DistanceSquared(this._gatherOrder.CreateNewOrderWorldPosition(base.Formation, WorldPosition.WorldPositionEnforcedCache.NavMeshVec3).GetNavMeshVec3()) < 100f || base.Formation.QuerySystem.FormationIntegrityData.DeviationOfPositionsExcludeFarAgents / ((base.Formation.QuerySystem.IdealAverageDisplacement != 0f) ? base.Formation.QuerySystem.IdealAverageDisplacement : 1f) <= 3f);
			}
			BehaviorEliminateEnemyInsideCastle.BehaviorState behaviorState = (flag ? BehaviorEliminateEnemyInsideCastle.BehaviorState.Attacking : BehaviorEliminateEnemyInsideCastle.BehaviorState.Gathering);
			if (behaviorState != this._behaviorState)
			{
				this._behaviorState = behaviorState;
				base.CurrentOrder = ((this._behaviorState == BehaviorEliminateEnemyInsideCastle.BehaviorState.Attacking) ? this._attackOrder : this._gatherOrder);
				this.CurrentFacingOrder = ((this._behaviorState == BehaviorEliminateEnemyInsideCastle.BehaviorState.Attacking) ? this._attackFacingOrder : this._gatheringFacingOrder);
			}
			base.Formation.SetMovementOrder(base.CurrentOrder);
			base.Formation.FacingOrder = this.CurrentFacingOrder;
			if (this._behaviorState == BehaviorEliminateEnemyInsideCastle.BehaviorState.Gathering && this._gatheringTacticalPos != null)
			{
				base.Formation.FormOrder = FormOrder.FormOrderCustom(this._gatheringTacticalPos.Width);
			}
		}

		// Token: 0x06000CD7 RID: 3287 RVA: 0x0001D334 File Offset: 0x0001B534
		protected override void OnBehaviorActivatedAux()
		{
			this._behaviorState = BehaviorEliminateEnemyInsideCastle.BehaviorState.UnSet;
			this._behaviorSide = base.Formation.AI.Side;
			this.ResetOrderPositions();
			base.Formation.SetMovementOrder(base.CurrentOrder);
			base.Formation.FacingOrder = this.CurrentFacingOrder;
			base.Formation.ArrangementOrder = ArrangementOrder.ArrangementOrderLine;
			base.Formation.FiringOrder = FiringOrder.FiringOrderFireAtWill;
			base.Formation.FormOrder = FormOrder.FormOrderWide;
			base.Formation.WeaponUsageOrder = WeaponUsageOrder.WeaponUsageOrderUseAny;
		}

		// Token: 0x17000315 RID: 789
		// (get) Token: 0x06000CD8 RID: 3288 RVA: 0x0001D3C6 File Offset: 0x0001B5C6
		public override float NavmeshlessTargetPositionPenalty
		{
			get
			{
				return 1f;
			}
		}

		// Token: 0x06000CD9 RID: 3289 RVA: 0x0001D3CD File Offset: 0x0001B5CD
		protected override float GetAiWeight()
		{
			return 1f;
		}

		// Token: 0x04000313 RID: 787
		private BehaviorEliminateEnemyInsideCastle.BehaviorState _behaviorState;

		// Token: 0x04000314 RID: 788
		private MovementOrder _gatherOrder;

		// Token: 0x04000315 RID: 789
		private MovementOrder _attackOrder;

		// Token: 0x04000316 RID: 790
		private FacingOrder _gatheringFacingOrder;

		// Token: 0x04000317 RID: 791
		private FacingOrder _attackFacingOrder;

		// Token: 0x04000318 RID: 792
		private TacticalPosition _gatheringTacticalPos;

		// Token: 0x04000319 RID: 793
		private Formation _targetEnemyFormation;

		// Token: 0x02000443 RID: 1091
		private enum BehaviorState
		{
			// Token: 0x04001856 RID: 6230
			UnSet,
			// Token: 0x04001857 RID: 6231
			Gathering,
			// Token: 0x04001858 RID: 6232
			Attacking
		}
	}
}
