using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Engine;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000127 RID: 295
	public class BehaviorWaitForLadders : BehaviorComponent
	{
		// Token: 0x06000DC7 RID: 3527 RVA: 0x000268D8 File Offset: 0x00024AD8
		public BehaviorWaitForLadders(Formation formation)
			: base(formation)
		{
			this._behaviorSide = formation.AI.Side;
			this._ladders = Mission.Current.ActiveMissionObjects.OfType<SiegeLadder>().ToList<SiegeLadder>();
			this._ladders.RemoveAll((SiegeLadder l) => l.IsDeactivated || l.WeaponSide != this._behaviorSide);
			this._teamAISiegeComponent = (TeamAISiegeComponent)formation.Team.TeamAI;
			SiegeLane siegeLane = TeamAISiegeComponent.SiegeLanes.FirstOrDefault((SiegeLane sl) => sl.LaneSide == this._behaviorSide);
			object obj;
			if (siegeLane == null)
			{
				obj = null;
			}
			else
			{
				obj = siegeLane.DefensePoints.FirstOrDefault((ICastleKeyPosition dp) => dp is WallSegment && (dp as WallSegment).IsBreachedWall);
			}
			this._breachedWallSegment = obj as WallSegment;
			this.ResetFollowOrder();
			this._stopOrder = MovementOrder.MovementOrderStop;
			if (this._followOrder.OrderEnum != MovementOrder.MovementOrderEnum.Invalid)
			{
				base.CurrentOrder = this._followOrder;
				this._behaviorState = BehaviorWaitForLadders.BehaviorState.Follow;
				return;
			}
			base.CurrentOrder = this._stopOrder;
			this._behaviorState = BehaviorWaitForLadders.BehaviorState.Stop;
		}

		// Token: 0x06000DC8 RID: 3528 RVA: 0x000269DC File Offset: 0x00024BDC
		private void ResetFollowOrder()
		{
			this._followedEntity = null;
			this._followTacticalPosition = null;
			if (this._ladders.Count > 0)
			{
				SiegeLadder siegeLadder;
				if ((siegeLadder = this._ladders.FirstOrDefault((SiegeLadder l) => !l.IsDeactivated && l.InitialWaitPosition.HasScriptOfType<TacticalPosition>())) == null)
				{
					siegeLadder = this._ladders.FirstOrDefault((SiegeLadder l) => !l.IsDeactivated);
				}
				this._followedEntity = siegeLadder.InitialWaitPosition;
				if (this._followedEntity == null)
				{
					this._followedEntity = this._ladders.FirstOrDefault((SiegeLadder l) => !l.IsDeactivated).InitialWaitPosition;
				}
				this._followOrder = MovementOrder.MovementOrderFollowEntity(this._followedEntity);
			}
			else if (this._breachedWallSegment != null)
			{
				this._followedEntity = this._breachedWallSegment.GameEntity.CollectChildrenEntitiesWithTag("attacker_wait_pos").FirstOrDefault<GameEntity>();
				this._followOrder = MovementOrder.MovementOrderFollowEntity(this._followedEntity);
			}
			else
			{
				this._followOrder = MovementOrder.MovermentOrderNull;
			}
			if (this._followedEntity != null)
			{
				this._followTacticalPosition = this._followedEntity.GetFirstScriptOfType<TacticalPosition>();
			}
		}

		// Token: 0x06000DC9 RID: 3529 RVA: 0x00026B28 File Offset: 0x00024D28
		public override void OnValidBehaviorSideChanged()
		{
			base.OnValidBehaviorSideChanged();
			this._ladders = Mission.Current.ActiveMissionObjects.OfType<SiegeLadder>().ToList<SiegeLadder>();
			this._ladders.RemoveAll((SiegeLadder l) => l.IsDeactivated || l.WeaponSide != this._behaviorSide);
			SiegeLane siegeLane = TeamAISiegeComponent.SiegeLanes.FirstOrDefault((SiegeLane sl) => sl.LaneSide == this._behaviorSide);
			object obj;
			if (siegeLane == null)
			{
				obj = null;
			}
			else
			{
				obj = siegeLane.DefensePoints.FirstOrDefault((ICastleKeyPosition dp) => dp is WallSegment && (dp as WallSegment).IsBreachedWall);
			}
			this._breachedWallSegment = obj as WallSegment;
			this.ResetFollowOrder();
			this._behaviorState = BehaviorWaitForLadders.BehaviorState.Unset;
		}

		// Token: 0x06000DCA RID: 3530 RVA: 0x00026BCC File Offset: 0x00024DCC
		protected override void CalculateCurrentOrder()
		{
			BehaviorWaitForLadders.BehaviorState behaviorState = ((this._followOrder.OrderEnum != MovementOrder.MovementOrderEnum.Invalid) ? BehaviorWaitForLadders.BehaviorState.Follow : BehaviorWaitForLadders.BehaviorState.Stop);
			if (behaviorState != this._behaviorState)
			{
				if (behaviorState == BehaviorWaitForLadders.BehaviorState.Follow)
				{
					base.CurrentOrder = this._followOrder;
					if (this._followTacticalPosition != null)
					{
						this.CurrentFacingOrder = FacingOrder.FacingOrderLookAtDirection(this._followTacticalPosition.Direction);
					}
					else
					{
						this.CurrentFacingOrder = FacingOrder.FacingOrderLookAtEnemy;
					}
				}
				else
				{
					base.CurrentOrder = this._stopOrder;
					this.CurrentFacingOrder = FacingOrder.FacingOrderLookAtEnemy;
				}
				this._behaviorState = behaviorState;
			}
		}

		// Token: 0x06000DCB RID: 3531 RVA: 0x00026C50 File Offset: 0x00024E50
		public override void TickOccasionally()
		{
			base.TickOccasionally();
			if (this._ladders.RemoveAll((SiegeLadder l) => l.IsDeactivated) > 0)
			{
				this.ResetFollowOrder();
				this.CalculateCurrentOrder();
			}
			base.Formation.SetMovementOrder(base.CurrentOrder);
			base.Formation.FacingOrder = this.CurrentFacingOrder;
			if (this._behaviorState == BehaviorWaitForLadders.BehaviorState.Follow && this._followTacticalPosition != null)
			{
				base.Formation.FormOrder = FormOrder.FormOrderCustom(this._followTacticalPosition.Width);
			}
			foreach (SiegeLadder siegeLadder in this._ladders)
			{
				if (siegeLadder.IsUsedByFormation(base.Formation))
				{
					base.Formation.StopUsingMachine(siegeLadder, false);
				}
			}
		}

		// Token: 0x06000DCC RID: 3532 RVA: 0x00026D44 File Offset: 0x00024F44
		protected override void OnBehaviorActivatedAux()
		{
			base.Formation.ArrangementOrder = (base.Formation.QuerySystem.HasShield ? ArrangementOrder.ArrangementOrderShieldWall : ArrangementOrder.ArrangementOrderLine);
			base.Formation.FacingOrder = FacingOrder.FacingOrderLookAtEnemy;
			base.Formation.FiringOrder = FiringOrder.FiringOrderFireAtWill;
			base.Formation.FormOrder = FormOrder.FormOrderWide;
			base.Formation.WeaponUsageOrder = WeaponUsageOrder.WeaponUsageOrderUseAny;
		}

		// Token: 0x17000325 RID: 805
		// (get) Token: 0x06000DCD RID: 3533 RVA: 0x00026DBA File Offset: 0x00024FBA
		public override float NavmeshlessTargetPositionPenalty
		{
			get
			{
				return 1f;
			}
		}

		// Token: 0x06000DCE RID: 3534 RVA: 0x00026DC4 File Offset: 0x00024FC4
		protected override float GetAiWeight()
		{
			float num = 0f;
			if (this._followOrder.OrderEnum != MovementOrder.MovementOrderEnum.Invalid && !this._teamAISiegeComponent.AreLaddersReady)
			{
				num = ((!this._teamAISiegeComponent.IsCastleBreached()) ? 1f : 0.5f);
			}
			return num;
		}

		// Token: 0x04000365 RID: 869
		private const string WallWaitPositionTag = "attacker_wait_pos";

		// Token: 0x04000366 RID: 870
		private List<SiegeLadder> _ladders;

		// Token: 0x04000367 RID: 871
		private WallSegment _breachedWallSegment;

		// Token: 0x04000368 RID: 872
		private TeamAISiegeComponent _teamAISiegeComponent;

		// Token: 0x04000369 RID: 873
		private MovementOrder _stopOrder;

		// Token: 0x0400036A RID: 874
		private MovementOrder _followOrder;

		// Token: 0x0400036B RID: 875
		private BehaviorWaitForLadders.BehaviorState _behaviorState;

		// Token: 0x0400036C RID: 876
		private GameEntity _followedEntity;

		// Token: 0x0400036D RID: 877
		private TacticalPosition _followTacticalPosition;

		// Token: 0x0200045D RID: 1117
		private enum BehaviorState
		{
			// Token: 0x040018AE RID: 6318
			Unset,
			// Token: 0x040018AF RID: 6319
			Stop,
			// Token: 0x040018B0 RID: 6320
			Follow
		}
	}
}
