using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Engine;

namespace TaleWorlds.MountAndBlade
{
	public class BehaviorWaitForLadders : BehaviorComponent
	{
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

		protected override void OnBehaviorActivatedAux()
		{
			base.Formation.ArrangementOrder = (base.Formation.QuerySystem.HasShield ? ArrangementOrder.ArrangementOrderShieldWall : ArrangementOrder.ArrangementOrderLine);
			base.Formation.FacingOrder = FacingOrder.FacingOrderLookAtEnemy;
			base.Formation.FiringOrder = FiringOrder.FiringOrderFireAtWill;
			base.Formation.FormOrder = FormOrder.FormOrderWide;
			base.Formation.WeaponUsageOrder = WeaponUsageOrder.WeaponUsageOrderUseAny;
		}

		public override float NavmeshlessTargetPositionPenalty
		{
			get
			{
				return 1f;
			}
		}

		protected override float GetAiWeight()
		{
			float num = 0f;
			if (this._followOrder.OrderEnum != MovementOrder.MovementOrderEnum.Invalid && !this._teamAISiegeComponent.AreLaddersReady)
			{
				num = ((!this._teamAISiegeComponent.IsCastleBreached()) ? 1f : 0.5f);
			}
			return num;
		}

		private const string WallWaitPositionTag = "attacker_wait_pos";

		private List<SiegeLadder> _ladders;

		private WallSegment _breachedWallSegment;

		private TeamAISiegeComponent _teamAISiegeComponent;

		private MovementOrder _stopOrder;

		private MovementOrder _followOrder;

		private BehaviorWaitForLadders.BehaviorState _behaviorState;

		private GameEntity _followedEntity;

		private TacticalPosition _followTacticalPosition;

		private enum BehaviorState
		{
			Unset,
			Stop,
			Follow
		}
	}
}
