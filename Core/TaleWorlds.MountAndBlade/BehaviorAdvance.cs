﻿using System;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	public sealed class BehaviorAdvance : BehaviorComponent
	{
		public BehaviorAdvance(Formation formation)
			: base(formation)
		{
			base.BehaviorCoherence = 0.8f;
			this._switchedToShieldWallTimer = new Timer(0f, 0f, true);
			this.CalculateCurrentOrder();
		}

		protected override void CalculateCurrentOrder()
		{
			FormationQuerySystem.FormationIntegrityDataGroup formationIntegrityData = base.Formation.QuerySystem.FormationIntegrityData;
			if (this._switchedToShieldWallRecently && !this._switchedToShieldWallTimer.Check(Mission.Current.CurrentTime) && formationIntegrityData.DeviationOfPositionsExcludeFarAgents > formationIntegrityData.AverageMaxUnlimitedSpeedExcludeFarAgents * 0.5f)
			{
				WorldPosition medianPosition = base.Formation.QuerySystem.MedianPosition;
				if (this._reformPosition.IsValid)
				{
					medianPosition.SetVec2(this._reformPosition);
				}
				else
				{
					Vec2 vec = (base.Formation.QuerySystem.Team.MedianTargetFormationPosition.AsVec2 - base.Formation.QuerySystem.AveragePosition).Normalized();
					this._reformPosition = base.Formation.QuerySystem.AveragePosition + vec * 5f;
					medianPosition.SetVec2(this._reformPosition);
				}
				base.CurrentOrder = MovementOrder.MovementOrderMove(medianPosition);
				return;
			}
			this._switchedToShieldWallRecently = false;
			bool flag = false;
			if (base.Formation.QuerySystem.ClosestEnemyFormation != null && base.Formation.QuerySystem.ClosestEnemyFormation.IsCavalryFormation)
			{
				Vec2 vec2 = base.Formation.QuerySystem.AveragePosition - base.Formation.QuerySystem.ClosestEnemyFormation.AveragePosition;
				float num = vec2.Normalize();
				Vec2 currentVelocity = base.Formation.QuerySystem.ClosestEnemyFormation.CurrentVelocity;
				float num2 = currentVelocity.Normalize();
				if (num < 30f && num2 > 2f && vec2.DotProduct(currentVelocity) > 0.5f)
				{
					flag = true;
					WorldPosition medianPosition2 = base.Formation.QuerySystem.MedianPosition;
					if (this._reformPosition.IsValid)
					{
						medianPosition2.SetVec2(this._reformPosition);
					}
					else
					{
						Vec2 vec3 = (base.Formation.QuerySystem.Team.MedianTargetFormationPosition.AsVec2 - base.Formation.QuerySystem.AveragePosition).Normalized();
						this._reformPosition = base.Formation.QuerySystem.AveragePosition + vec3 * 5f;
						medianPosition2.SetVec2(this._reformPosition);
					}
					base.CurrentOrder = MovementOrder.MovementOrderMove(medianPosition2);
				}
			}
			if (!flag)
			{
				this._reformPosition = Vec2.Invalid;
				int num3 = 0;
				bool flag2 = false;
				foreach (Team team in Mission.Current.Teams)
				{
					if (team.IsEnemyOf(base.Formation.Team))
					{
						using (List<Formation>.Enumerator enumerator2 = team.FormationsIncludingSpecialAndEmpty.GetEnumerator())
						{
							while (enumerator2.MoveNext())
							{
								if (enumerator2.Current.CountOfUnits > 0)
								{
									num3++;
									flag2 = num3 == 1;
									if (num3 > 1)
									{
										break;
									}
								}
							}
						}
					}
				}
				FormationQuerySystem formationQuerySystem = (flag2 ? base.Formation.QuerySystem.ClosestEnemyFormation : base.Formation.QuerySystem.Team.MedianTargetFormation);
				if (formationQuerySystem != null)
				{
					WorldPosition medianPosition3 = formationQuerySystem.MedianPosition;
					medianPosition3.SetVec2(medianPosition3.AsVec2 + formationQuerySystem.Formation.Direction * formationQuerySystem.Formation.Depth * 0.5f);
					Vec2 vec4 = -formationQuerySystem.Formation.Direction;
					base.CurrentOrder = MovementOrder.MovementOrderMove(medianPosition3);
					this.CurrentFacingOrder = FacingOrder.FacingOrderLookAtDirection(vec4);
					return;
				}
				WorldPosition worldPosition = (flag2 ? base.Formation.QuerySystem.ClosestEnemyFormation.MedianPosition : base.Formation.QuerySystem.Team.MedianTargetFormationPosition);
				Vec2 vec5 = (base.Formation.QuerySystem.Team.MedianTargetFormationPosition.AsVec2 - base.Formation.QuerySystem.AveragePosition).Normalized();
				base.CurrentOrder = MovementOrder.MovementOrderMove(worldPosition);
				this.CurrentFacingOrder = FacingOrder.FacingOrderLookAtDirection(vec5);
			}
		}

		protected override void OnBehaviorActivatedAux()
		{
			this.CalculateCurrentOrder();
			base.Formation.SetMovementOrder(base.CurrentOrder);
			base.Formation.FacingOrder = this.CurrentFacingOrder;
			this._isInShieldWallDistance = false;
			base.Formation.ArrangementOrder = ArrangementOrder.ArrangementOrderLine;
			base.Formation.FiringOrder = FiringOrder.FiringOrderFireAtWill;
			base.Formation.FormOrder = FormOrder.FormOrderWide;
		}

		public override void TickOccasionally()
		{
			this.CalculateCurrentOrder();
			base.Formation.SetMovementOrder(base.CurrentOrder);
			base.Formation.FacingOrder = this.CurrentFacingOrder;
			if (base.Formation.PhysicalClass.IsMeleeInfantry())
			{
				bool flag = false;
				if (base.Formation.QuerySystem.ClosestEnemyFormation != null && base.Formation.QuerySystem.IsUnderRangedAttack)
				{
					float num = base.Formation.QuerySystem.AveragePosition.DistanceSquared(base.Formation.QuerySystem.ClosestEnemyFormation.MedianPosition.AsVec2);
					if (num < 6400f + (this._isInShieldWallDistance ? 3600f : 0f) && num > 100f - (this._isInShieldWallDistance ? 75f : 0f))
					{
						flag = true;
					}
				}
				if (flag != this._isInShieldWallDistance)
				{
					this._isInShieldWallDistance = flag;
					if (this._isInShieldWallDistance)
					{
						if (base.Formation.QuerySystem.HasShield)
						{
							base.Formation.ArrangementOrder = ArrangementOrder.ArrangementOrderShieldWall;
						}
						else
						{
							base.Formation.ArrangementOrder = ArrangementOrder.ArrangementOrderLoose;
						}
						this._switchedToShieldWallRecently = true;
						this._switchedToShieldWallTimer.Reset(Mission.Current.CurrentTime, 5f);
					}
					else
					{
						base.Formation.ArrangementOrder = ArrangementOrder.ArrangementOrderLine;
					}
				}
			}
			base.Formation.SetMovementOrder(base.CurrentOrder);
		}

		protected override float GetAiWeight()
		{
			return 1f;
		}

		private bool _isInShieldWallDistance;

		private bool _switchedToShieldWallRecently;

		private Timer _switchedToShieldWallTimer;

		private Vec2 _reformPosition = Vec2.Invalid;
	}
}
