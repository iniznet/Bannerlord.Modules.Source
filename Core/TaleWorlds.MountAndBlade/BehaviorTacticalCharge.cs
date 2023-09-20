using System;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.MountAndBlade
{
	public class BehaviorTacticalCharge : BehaviorComponent
	{
		public BehaviorTacticalCharge(Formation formation)
			: base(formation)
		{
			this._lastTarget = null;
			base.CurrentOrder = MovementOrder.MovementOrderCharge;
			this.CurrentFacingOrder = FacingOrder.FacingOrderLookAtEnemy;
			this._chargeState = BehaviorTacticalCharge.ChargeState.Undetermined;
			base.BehaviorCoherence = 0.5f;
			this._desiredChargeStopDistance = 20f;
		}

		public override void TickOccasionally()
		{
			base.TickOccasionally();
			if (base.Formation.AI.ActiveBehavior != this)
			{
				return;
			}
			this.CalculateCurrentOrder();
			base.Formation.SetMovementOrder(base.CurrentOrder);
			base.Formation.FacingOrder = this.CurrentFacingOrder;
		}

		private BehaviorTacticalCharge.ChargeState CheckAndChangeState()
		{
			BehaviorTacticalCharge.ChargeState chargeState = this._chargeState;
			if (base.Formation.QuerySystem.ClosestEnemyFormation == null)
			{
				chargeState = BehaviorTacticalCharge.ChargeState.Undetermined;
			}
			else
			{
				switch (this._chargeState)
				{
				case BehaviorTacticalCharge.ChargeState.Undetermined:
					if (base.Formation.QuerySystem.ClosestEnemyFormation != null && ((!base.Formation.QuerySystem.IsCavalryFormation && !base.Formation.QuerySystem.IsRangedCavalryFormation) || base.Formation.QuerySystem.AveragePosition.Distance(base.Formation.QuerySystem.ClosestEnemyFormation.MedianPosition.AsVec2) / base.Formation.QuerySystem.MovementSpeedMaximum <= 5f))
					{
						chargeState = BehaviorTacticalCharge.ChargeState.Charging;
					}
					break;
				case BehaviorTacticalCharge.ChargeState.Charging:
					if (this._lastTarget == null || this._lastTarget.Formation.CountOfUnits == 0)
					{
						chargeState = BehaviorTacticalCharge.ChargeState.Undetermined;
					}
					else if (!base.Formation.QuerySystem.IsCavalryFormation && !base.Formation.QuerySystem.IsRangedCavalryFormation)
					{
						if (!base.Formation.QuerySystem.IsInfantryFormation || !base.Formation.QuerySystem.ClosestEnemyFormation.IsCavalryFormation)
						{
							chargeState = BehaviorTacticalCharge.ChargeState.Charging;
						}
						else
						{
							Vec2 vec = base.Formation.QuerySystem.AveragePosition - base.Formation.QuerySystem.ClosestEnemyFormation.AveragePosition;
							float num = vec.Normalize();
							Vec2 currentVelocity = base.Formation.QuerySystem.ClosestEnemyFormation.CurrentVelocity;
							float num2 = currentVelocity.Normalize();
							if (num / num2 <= 6f && vec.DotProduct(currentVelocity) > 0.5f)
							{
								this._chargeState = BehaviorTacticalCharge.ChargeState.Bracing;
							}
						}
					}
					else if (this._initialChargeDirection.DotProduct(base.Formation.QuerySystem.ClosestEnemyFormation.MedianPosition.AsVec2 - base.Formation.QuerySystem.AveragePosition) <= 0f)
					{
						chargeState = BehaviorTacticalCharge.ChargeState.ChargingPast;
					}
					break;
				case BehaviorTacticalCharge.ChargeState.ChargingPast:
					if (this._chargingPastTimer.Check(Mission.Current.CurrentTime) || base.Formation.QuerySystem.AveragePosition.Distance(base.Formation.QuerySystem.ClosestEnemyFormation.MedianPosition.AsVec2) >= this._desiredChargeStopDistance)
					{
						chargeState = BehaviorTacticalCharge.ChargeState.Reforming;
					}
					break;
				case BehaviorTacticalCharge.ChargeState.Reforming:
					if (this._reformTimer.Check(Mission.Current.CurrentTime) || base.Formation.QuerySystem.AveragePosition.Distance(base.Formation.QuerySystem.ClosestEnemyFormation.MedianPosition.AsVec2) <= 30f)
					{
						chargeState = BehaviorTacticalCharge.ChargeState.Charging;
					}
					break;
				case BehaviorTacticalCharge.ChargeState.Bracing:
				{
					bool flag = false;
					if (base.Formation.QuerySystem.IsInfantryFormation && base.Formation.QuerySystem.ClosestEnemyFormation.IsCavalryFormation)
					{
						Vec2 vec2 = base.Formation.QuerySystem.AveragePosition - base.Formation.QuerySystem.ClosestEnemyFormation.AveragePosition;
						float num3 = vec2.Normalize();
						Vec2 currentVelocity2 = base.Formation.QuerySystem.ClosestEnemyFormation.CurrentVelocity;
						float num4 = currentVelocity2.Normalize();
						if (num3 / num4 <= 8f && vec2.DotProduct(currentVelocity2) > 0.33f)
						{
							flag = true;
						}
					}
					if (!flag)
					{
						this._bracePosition = Vec2.Invalid;
						this._chargeState = BehaviorTacticalCharge.ChargeState.Charging;
					}
					break;
				}
				}
			}
			return chargeState;
		}

		protected override void CalculateCurrentOrder()
		{
			if (base.Formation.QuerySystem.ClosestEnemyFormation == null || ((base.Formation.QuerySystem.IsCavalryFormation || base.Formation.QuerySystem.IsRangedCavalryFormation) && (base.Formation.QuerySystem.ClosestEnemyFormation.IsCavalryFormation || base.Formation.QuerySystem.ClosestEnemyFormation.IsRangedCavalryFormation)))
			{
				base.CurrentOrder = MovementOrder.MovementOrderCharge;
				return;
			}
			BehaviorTacticalCharge.ChargeState chargeState = this.CheckAndChangeState();
			if (chargeState != this._chargeState)
			{
				this._chargeState = chargeState;
				switch (this._chargeState)
				{
				case BehaviorTacticalCharge.ChargeState.Undetermined:
					base.CurrentOrder = MovementOrder.MovementOrderCharge;
					break;
				case BehaviorTacticalCharge.ChargeState.Charging:
					this._lastTarget = base.Formation.QuerySystem.ClosestEnemyFormation;
					if (base.Formation.QuerySystem.IsCavalryFormation || base.Formation.QuerySystem.IsRangedCavalryFormation)
					{
						this._initialChargeDirection = this._lastTarget.MedianPosition.AsVec2 - base.Formation.QuerySystem.AveragePosition;
						float num = this._initialChargeDirection.Normalize();
						this._desiredChargeStopDistance = MBMath.ClampFloat(num, 20f, 50f);
					}
					break;
				case BehaviorTacticalCharge.ChargeState.ChargingPast:
					this._chargingPastTimer = new Timer(Mission.Current.CurrentTime, 5f, true);
					break;
				case BehaviorTacticalCharge.ChargeState.Reforming:
					this._reformTimer = new Timer(Mission.Current.CurrentTime, 2f, true);
					break;
				case BehaviorTacticalCharge.ChargeState.Bracing:
				{
					Vec2 vec = (base.Formation.QuerySystem.Team.MedianTargetFormationPosition.AsVec2 - base.Formation.QuerySystem.AveragePosition).Normalized();
					this._bracePosition = base.Formation.QuerySystem.AveragePosition + vec * 5f;
					break;
				}
				}
			}
			switch (this._chargeState)
			{
			case BehaviorTacticalCharge.ChargeState.Undetermined:
				if (base.Formation.QuerySystem.ClosestEnemyFormation != null && (base.Formation.QuerySystem.IsCavalryFormation || base.Formation.QuerySystem.IsRangedCavalryFormation))
				{
					base.CurrentOrder = MovementOrder.MovementOrderMove(base.Formation.QuerySystem.ClosestEnemyFormation.MedianPosition);
				}
				else
				{
					base.CurrentOrder = MovementOrder.MovementOrderCharge;
				}
				this.CurrentFacingOrder = FacingOrder.FacingOrderLookAtEnemy;
				return;
			case BehaviorTacticalCharge.ChargeState.Charging:
			{
				if (base.Formation.QuerySystem.IsCavalryFormation || base.Formation.QuerySystem.IsRangedCavalryFormation)
				{
					Vec2 vec2 = (this._lastTarget.MedianPosition.AsVec2 - base.Formation.QuerySystem.AveragePosition).Normalized();
					WorldPosition medianPosition = this._lastTarget.MedianPosition;
					Vec2 vec3 = medianPosition.AsVec2 + vec2 * this._desiredChargeStopDistance;
					medianPosition.SetVec2(vec3);
					base.CurrentOrder = MovementOrder.MovementOrderMove(medianPosition);
					this.CurrentFacingOrder = FacingOrder.FacingOrderLookAtDirection(vec2);
					return;
				}
				if (base.Formation.Width >= base.Formation.QuerySystem.ClosestEnemyFormation.Formation.Width * (1f + ((base.Formation.GetReadonlyMovementOrderReference().OrderEnum != MovementOrder.MovementOrderEnum.Charge) ? 0.1f : 0f)))
				{
					base.CurrentOrder = MovementOrder.MovementOrderCharge;
					this.CurrentFacingOrder = FacingOrder.FacingOrderLookAtEnemy;
					return;
				}
				WorldPosition medianPosition2 = base.Formation.QuerySystem.ClosestEnemyFormation.MedianPosition;
				base.CurrentOrder = MovementOrder.MovementOrderMove(medianPosition2);
				this.CurrentFacingOrder = FacingOrder.FacingOrderLookAtEnemy;
				return;
			}
			case BehaviorTacticalCharge.ChargeState.ChargingPast:
			{
				Vec2 vec4 = (base.Formation.QuerySystem.AveragePosition - this._lastTarget.MedianPosition.AsVec2).Normalized();
				this._lastReformDestination = this._lastTarget.MedianPosition;
				Vec2 vec5 = this._lastTarget.MedianPosition.AsVec2 + vec4 * this._desiredChargeStopDistance;
				this._lastReformDestination.SetVec2(vec5);
				base.CurrentOrder = MovementOrder.MovementOrderMove(this._lastReformDestination);
				this.CurrentFacingOrder = FacingOrder.FacingOrderLookAtDirection(vec4);
				return;
			}
			case BehaviorTacticalCharge.ChargeState.Reforming:
				base.CurrentOrder = MovementOrder.MovementOrderMove(this._lastReformDestination);
				this.CurrentFacingOrder = FacingOrder.FacingOrderLookAtEnemy;
				return;
			case BehaviorTacticalCharge.ChargeState.Bracing:
			{
				WorldPosition medianPosition3 = base.Formation.QuerySystem.MedianPosition;
				medianPosition3.SetVec2(this._bracePosition);
				base.CurrentOrder = MovementOrder.MovementOrderMove(medianPosition3);
				return;
			}
			default:
				return;
			}
		}

		protected override void OnBehaviorActivatedAux()
		{
			this.CalculateCurrentOrder();
			base.Formation.SetMovementOrder(base.CurrentOrder);
			base.Formation.FacingOrder = this.CurrentFacingOrder;
			if (base.Formation.QuerySystem.IsCavalryFormation || base.Formation.QuerySystem.IsRangedCavalryFormation)
			{
				base.Formation.ArrangementOrder = ArrangementOrder.ArrangementOrderSkein;
			}
			else
			{
				base.Formation.ArrangementOrder = ArrangementOrder.ArrangementOrderLine;
			}
			base.Formation.FiringOrder = FiringOrder.FiringOrderFireAtWill;
			base.Formation.FormOrder = FormOrder.FormOrderWide;
		}

		public override TextObject GetBehaviorString()
		{
			TextObject behaviorString = base.GetBehaviorString();
			if (base.Formation.QuerySystem.ClosestEnemyFormation != null)
			{
				behaviorString.SetTextVariable("AI_SIDE", GameTexts.FindText("str_formation_ai_side_strings", base.Formation.QuerySystem.ClosestEnemyFormation.Formation.AI.Side.ToString()));
				behaviorString.SetTextVariable("CLASS", GameTexts.FindText("str_formation_class_string", base.Formation.QuerySystem.ClosestEnemyFormation.Formation.PhysicalClass.GetName()));
			}
			return behaviorString;
		}

		public override float NavmeshlessTargetPositionPenalty
		{
			get
			{
				return 1f;
			}
		}

		private float CalculateAIWeight()
		{
			FormationQuerySystem querySystem = base.Formation.QuerySystem;
			if (querySystem.ClosestEnemyFormation == null)
			{
				return 0f;
			}
			float num = querySystem.AveragePosition.Distance(querySystem.ClosestEnemyFormation.MedianPosition.AsVec2) / querySystem.MovementSpeedMaximum;
			float num3;
			if (!querySystem.IsCavalryFormation && !querySystem.IsRangedCavalryFormation)
			{
				float num2 = MBMath.ClampFloat(num, 4f, 10f);
				num3 = MBMath.Lerp(0.8f, 1f, 1f - (num2 - 4f) / 6f, 1E-05f);
			}
			else if (num <= 4f)
			{
				float num4 = MBMath.ClampFloat(num, 0f, 4f);
				num3 = MBMath.Lerp(0.8f, 1.2f, num4 / 4f, 1E-05f);
			}
			else
			{
				float num5 = MBMath.ClampFloat(num, 4f, 10f);
				num3 = MBMath.Lerp(0.8f, 1.2f, 1f - (num5 - 4f) / 6f, 1E-05f);
			}
			float num6 = 1f;
			if (num <= 4f)
			{
				float length = (querySystem.AveragePosition - querySystem.ClosestEnemyFormation.MedianPosition.AsVec2).Length;
				if (length > 1E-45f)
				{
					WorldPosition medianPosition = querySystem.MedianPosition;
					medianPosition.SetVec2(querySystem.AveragePosition);
					float navMeshZ = medianPosition.GetNavMeshZ();
					if (!float.IsNaN(navMeshZ))
					{
						float num7 = (navMeshZ - querySystem.ClosestEnemyFormation.MedianPosition.GetNavMeshZ()) / length;
						num6 = MBMath.Lerp(0.9f, 1.1f, (MBMath.ClampFloat(num7, -0.58f, 0.58f) + 0.58f) / 1.16f, 1E-05f);
					}
				}
			}
			float num8 = 1f;
			if (num <= 4f && num >= 1.5f)
			{
				num8 = 1.2f;
			}
			float num9 = 1f;
			if (num <= 4f && querySystem.ClosestEnemyFormation.ClosestEnemyFormation != querySystem)
			{
				num9 = 1.2f;
			}
			float num10 = querySystem.GetClassWeightedFactor(1f, 1f, 1.5f, 1.5f) * querySystem.ClosestEnemyFormation.GetClassWeightedFactor(1f, 1f, 0.5f, 0.5f);
			return num3 * num6 * num8 * num9 * num10;
		}

		protected override float GetAiWeight()
		{
			float num = 0f;
			if (base.Formation.QuerySystem.ClosestEnemyFormation == null)
			{
				return 0f;
			}
			bool flag;
			if (!(base.Formation.Team.TeamAI is TeamAISiegeComponent))
			{
				flag = true;
			}
			else if ((base.Formation.Team.TeamAI as TeamAISiegeComponent).CalculateIsChargePastWallsApplicable(base.Formation.AI.Side))
			{
				flag = true;
			}
			else
			{
				bool flag2 = TeamAISiegeComponent.IsFormationInsideCastle(base.Formation.QuerySystem.ClosestEnemyFormation.Formation, true, 0.51f);
				flag = flag2 == TeamAISiegeComponent.IsFormationInsideCastle(base.Formation, true, flag2 ? 0.9f : 0.1f);
			}
			if (flag)
			{
				num = this.CalculateAIWeight();
			}
			return num;
		}

		private BehaviorTacticalCharge.ChargeState _chargeState;

		private FormationQuerySystem _lastTarget;

		private Vec2 _initialChargeDirection;

		private float _desiredChargeStopDistance;

		private WorldPosition _lastReformDestination;

		private Timer _chargingPastTimer;

		private Timer _reformTimer;

		private Vec2 _bracePosition = Vec2.Invalid;

		private enum ChargeState
		{
			Undetermined,
			Charging,
			ChargingPast,
			Reforming,
			Bracing
		}
	}
}
