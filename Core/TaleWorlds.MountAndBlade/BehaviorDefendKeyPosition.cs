using System;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	public class BehaviorDefendKeyPosition : BehaviorComponent
	{
		public WorldPosition DefensePosition
		{
			get
			{
				return this._behaviorPosition.Value;
			}
			set
			{
				this._defensePosition = value;
			}
		}

		public BehaviorDefendKeyPosition(Formation formation)
			: base(formation)
		{
			this._behaviorPosition = new QueryData<WorldPosition>(() => Mission.Current.FindBestDefendingPosition(this.EnemyClusterPosition, this._defensePosition), 5f);
			this.CalculateCurrentOrder();
		}

		protected override void CalculateCurrentOrder()
		{
			Vec2 vec;
			if (base.Formation.QuerySystem.ClosestEnemyFormation == null)
			{
				vec = base.Formation.Direction;
			}
			else
			{
				vec = ((base.Formation.Direction.DotProduct((base.Formation.QuerySystem.ClosestEnemyFormation.MedianPosition.AsVec2 - base.Formation.QuerySystem.AveragePosition).Normalized()) < 0.5f) ? (base.Formation.QuerySystem.ClosestEnemyFormation.MedianPosition.AsVec2 - base.Formation.QuerySystem.AveragePosition) : base.Formation.Direction).Normalized();
			}
			if (this.DefensePosition.IsValid)
			{
				base.CurrentOrder = MovementOrder.MovementOrderMove(this.DefensePosition);
			}
			else
			{
				WorldPosition medianPosition = base.Formation.QuerySystem.MedianPosition;
				medianPosition.SetVec2(base.Formation.QuerySystem.AveragePosition);
				base.CurrentOrder = MovementOrder.MovementOrderMove(medianPosition);
			}
			this.CurrentFacingOrder = FacingOrder.FacingOrderLookAtDirection(vec);
		}

		public override void TickOccasionally()
		{
			this.CalculateCurrentOrder();
			base.Formation.SetMovementOrder(base.CurrentOrder);
			base.Formation.FacingOrder = this.CurrentFacingOrder;
			if (base.Formation.QuerySystem.HasShield && base.Formation.QuerySystem.AveragePosition.DistanceSquared(base.CurrentOrder.GetPosition(base.Formation)) < base.Formation.Depth * base.Formation.Depth * 4f)
			{
				base.Formation.ArrangementOrder = ArrangementOrder.ArrangementOrderShieldWall;
				return;
			}
			base.Formation.ArrangementOrder = ArrangementOrder.ArrangementOrderLoose;
		}

		protected override void OnBehaviorActivatedAux()
		{
			base.Formation.ArrangementOrder = ArrangementOrder.ArrangementOrderLoose;
			base.Formation.FiringOrder = FiringOrder.FiringOrderFireAtWill;
			base.Formation.FormOrder = FormOrder.FormOrderWide;
			base.Formation.WeaponUsageOrder = WeaponUsageOrder.WeaponUsageOrderUseAny;
		}

		protected override float GetAiWeight()
		{
			return 10f;
		}

		private WorldPosition _defensePosition = WorldPosition.Invalid;

		public WorldPosition EnemyClusterPosition = WorldPosition.Invalid;

		private readonly QueryData<WorldPosition> _behaviorPosition;
	}
}
