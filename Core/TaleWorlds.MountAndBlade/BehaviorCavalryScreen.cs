using System;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.LinQuick;
using TaleWorlds.Localization;

namespace TaleWorlds.MountAndBlade
{
	public class BehaviorCavalryScreen : BehaviorComponent
	{
		public BehaviorCavalryScreen(Formation formation)
			: base(formation)
		{
			this._behaviorSide = formation.AI.Side;
			this._mainFormation = formation.Team.FormationsIncludingEmpty.FirstOrDefaultQ((Formation f) => f.CountOfUnits > 0 && f.AI.IsMainFormation);
			this.CalculateCurrentOrder();
		}

		public override void OnValidBehaviorSideChanged()
		{
			base.OnValidBehaviorSideChanged();
			this._mainFormation = base.Formation.Team.FormationsIncludingEmpty.FirstOrDefaultQ((Formation f) => f.CountOfUnits > 0 && f.AI.IsMainFormation);
		}

		protected override void CalculateCurrentOrder()
		{
			if (this._mainFormation == null || base.Formation.AI.IsMainFormation || (base.Formation.AI.Side != FormationAI.BehaviorSide.Left && base.Formation.AI.Side != FormationAI.BehaviorSide.Right))
			{
				this._flankingEnemyCavalryFormation = null;
				WorldPosition medianPosition = base.Formation.QuerySystem.MedianPosition;
				medianPosition.SetVec2(base.Formation.QuerySystem.AveragePosition);
				base.CurrentOrder = MovementOrder.MovementOrderMove(medianPosition);
				return;
			}
			float currentTime = Mission.Current.CurrentTime;
			if (this._threatFormationCacheTime + 5f < currentTime)
			{
				this._threatFormationCacheTime = currentTime;
				Vec2 vec = ((base.Formation.AI.Side == FormationAI.BehaviorSide.Left) ? this._mainFormation.Direction.LeftVec() : this._mainFormation.Direction.RightVec()).Normalized() - this._mainFormation.Direction.Normalized();
				this._flankingEnemyCavalryFormation = null;
				float num = float.MinValue;
				foreach (Team team in Mission.Current.Teams)
				{
					if (team.IsEnemyOf(base.Formation.Team))
					{
						foreach (Formation formation in team.FormationsIncludingSpecialAndEmpty)
						{
							if (formation.CountOfUnits > 0)
							{
								Vec2 vec2 = formation.QuerySystem.MedianPosition.AsVec2 - this._mainFormation.QuerySystem.MedianPosition.AsVec2;
								if (vec.Normalized().DotProduct(vec2.Normalized()) > 0.9238795f)
								{
									float formationPower = formation.QuerySystem.FormationPower;
									if (formationPower > num)
									{
										num = formationPower;
										this._flankingEnemyCavalryFormation = formation;
									}
								}
							}
						}
					}
				}
			}
			WorldPosition worldPosition;
			if (this._flankingEnemyCavalryFormation == null)
			{
				worldPosition = base.Formation.QuerySystem.MedianPosition;
				worldPosition.SetVec2(base.Formation.QuerySystem.AveragePosition);
			}
			else
			{
				Vec2 vec3 = this._flankingEnemyCavalryFormation.QuerySystem.MedianPosition.AsVec2 - this._mainFormation.QuerySystem.MedianPosition.AsVec2;
				float num2 = vec3.Normalize() * 0.5f;
				worldPosition = this._mainFormation.QuerySystem.MedianPosition;
				worldPosition.SetVec2(worldPosition.AsVec2 + num2 * vec3);
			}
			base.CurrentOrder = MovementOrder.MovementOrderMove(worldPosition);
		}

		public override void TickOccasionally()
		{
			this.CalculateCurrentOrder();
			base.Formation.SetMovementOrder(base.CurrentOrder);
		}

		protected override void OnBehaviorActivatedAux()
		{
			this.CalculateCurrentOrder();
			base.Formation.SetMovementOrder(base.CurrentOrder);
			base.Formation.ArrangementOrder = ArrangementOrder.ArrangementOrderSkein;
			base.Formation.FacingOrder = FacingOrder.FacingOrderLookAtEnemy;
			base.Formation.FiringOrder = FiringOrder.FiringOrderFireAtWill;
			base.Formation.FormOrder = FormOrder.FormOrderDeep;
		}

		public override TextObject GetBehaviorString()
		{
			TextObject behaviorString = base.GetBehaviorString();
			TextObject textObject = GameTexts.FindText("str_formation_ai_side_strings", base.Formation.AI.Side.ToString());
			behaviorString.SetTextVariable("SIDE_STRING", textObject);
			if (this._mainFormation != null)
			{
				behaviorString.SetTextVariable("AI_SIDE", GameTexts.FindText("str_formation_ai_side_strings", this._mainFormation.AI.Side.ToString()));
				behaviorString.SetTextVariable("CLASS", GameTexts.FindText("str_formation_class_string", this._mainFormation.PhysicalClass.GetName()));
			}
			return behaviorString;
		}

		protected override float GetAiWeight()
		{
			if (this._mainFormation == null || !this._mainFormation.AI.IsMainFormation)
			{
				this._mainFormation = base.Formation.Team.FormationsIncludingEmpty.FirstOrDefaultQ((Formation f) => f.CountOfUnits > 0 && f.AI.IsMainFormation);
			}
			if (this._flankingEnemyCavalryFormation == null)
			{
				return 0f;
			}
			return 1.2f;
		}

		private Formation _mainFormation;

		private Formation _flankingEnemyCavalryFormation;

		private float _threatFormationCacheTime;

		private const float _threatFormationCacheExpireTime = 5f;
	}
}
