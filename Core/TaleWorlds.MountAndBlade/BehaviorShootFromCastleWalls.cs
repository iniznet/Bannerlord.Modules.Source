using System;
using TaleWorlds.Engine;

namespace TaleWorlds.MountAndBlade
{
	public class BehaviorShootFromCastleWalls : BehaviorComponent
	{
		public GameEntity ArcherPosition
		{
			get
			{
				return this._archerPosition;
			}
			set
			{
				if (this._archerPosition != value)
				{
					this.OnArcherPositionSet(value);
				}
			}
		}

		public BehaviorShootFromCastleWalls(Formation formation)
			: base(formation)
		{
			this.OnArcherPositionSet(this._archerPosition);
			base.BehaviorCoherence = 0f;
		}

		private void OnArcherPositionSet(GameEntity value)
		{
			this._archerPosition = value;
			if (!(this._archerPosition != null))
			{
				this._tacticalArcherPosition = null;
				WorldPosition medianPosition = base.Formation.QuerySystem.MedianPosition;
				medianPosition.SetVec2(base.Formation.CurrentPosition);
				base.CurrentOrder = MovementOrder.MovementOrderMove(medianPosition);
				this.CurrentFacingOrder = FacingOrder.FacingOrderLookAtEnemy;
				return;
			}
			this._tacticalArcherPosition = this._archerPosition.GetFirstScriptOfType<TacticalPosition>();
			if (this._tacticalArcherPosition != null)
			{
				base.CurrentOrder = MovementOrder.MovementOrderMove(this._tacticalArcherPosition.Position);
				this.CurrentFacingOrder = FacingOrder.FacingOrderLookAtDirection(this._tacticalArcherPosition.Direction);
				return;
			}
			base.CurrentOrder = MovementOrder.MovementOrderMove(this._archerPosition.GlobalPosition.ToWorldPosition());
			this.CurrentFacingOrder = FacingOrder.FacingOrderLookAtDirection(this._archerPosition.GetGlobalFrame().rotation.f.AsVec2);
		}

		public override void TickOccasionally()
		{
			base.Formation.SetMovementOrder(base.CurrentOrder);
			base.Formation.FacingOrder = this.CurrentFacingOrder;
			if (this._tacticalArcherPosition != null)
			{
				base.Formation.FormOrder = FormOrder.FormOrderCustom(this._tacticalArcherPosition.Width);
			}
			foreach (Team team in base.Formation.Team.Mission.Teams)
			{
				if (team.IsEnemyOf(base.Formation.Team))
				{
					if (!this._areStrategicArcherAreasAbandoned)
					{
						if (team.QuerySystem.InsideWallsRatio > 0.6f)
						{
							base.Formation.ArrangementOrder = ArrangementOrder.ArrangementOrderLine;
							this._areStrategicArcherAreasAbandoned = true;
							break;
						}
						break;
					}
					else
					{
						if (team.QuerySystem.InsideWallsRatio <= 0.4f)
						{
							base.Formation.ArrangementOrder = ArrangementOrder.ArrangementOrderScatter;
							this._areStrategicArcherAreasAbandoned = false;
							break;
						}
						break;
					}
				}
			}
		}

		protected override void OnBehaviorActivatedAux()
		{
			base.Formation.SetMovementOrder(base.CurrentOrder);
			base.Formation.FacingOrder = this.CurrentFacingOrder;
			base.Formation.ArrangementOrder = ArrangementOrder.ArrangementOrderScatter;
			base.Formation.FiringOrder = FiringOrder.FiringOrderFireAtWill;
			base.Formation.FormOrder = FormOrder.FormOrderWide;
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
			return 10f * (base.Formation.QuerySystem.RangedCavalryUnitRatio + base.Formation.QuerySystem.RangedUnitRatio);
		}

		private GameEntity _archerPosition;

		private TacticalPosition _tacticalArcherPosition;

		private bool _areStrategicArcherAreasAbandoned;
	}
}
