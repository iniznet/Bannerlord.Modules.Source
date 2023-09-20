using System;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x020000F9 RID: 249
	public sealed class BehaviorAdvance : BehaviorComponent
	{
		// Token: 0x06000C58 RID: 3160 RVA: 0x000180EF File Offset: 0x000162EF
		public BehaviorAdvance(Formation formation)
			: base(formation)
		{
			base.BehaviorCoherence = 0.8f;
			this._switchedToShieldWallTimer = new Timer(0f, 0f, true);
			this.CalculateCurrentOrder();
		}

		// Token: 0x06000C59 RID: 3161 RVA: 0x0001812C File Offset: 0x0001632C
		protected override void CalculateCurrentOrder()
		{
			if (this._switchedToShieldWallRecently && !this._switchedToShieldWallTimer.Check(Mission.Current.CurrentTime) && base.Formation.QuerySystem.FormationDispersedness > 2f)
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
				WorldPosition worldPosition = (flag2 ? base.Formation.QuerySystem.ClosestEnemyFormation.MedianPosition : base.Formation.QuerySystem.Team.MedianTargetFormationPosition);
				Vec2 vec4 = (base.Formation.QuerySystem.Team.MedianTargetFormationPosition.AsVec2 - base.Formation.QuerySystem.AveragePosition).Normalized();
				base.CurrentOrder = MovementOrder.MovementOrderMove(worldPosition);
				this.CurrentFacingOrder = FacingOrder.FacingOrderLookAtDirection(vec4);
			}
		}

		// Token: 0x06000C5A RID: 3162 RVA: 0x000184D4 File Offset: 0x000166D4
		protected override void OnBehaviorActivatedAux()
		{
			this.CalculateCurrentOrder();
			base.Formation.SetMovementOrder(base.CurrentOrder);
			base.Formation.FacingOrder = this.CurrentFacingOrder;
			this._isInShieldWallDistance = false;
			base.Formation.ArrangementOrder = ArrangementOrder.ArrangementOrderLine;
			base.Formation.FiringOrder = FiringOrder.FiringOrderFireAtWill;
			base.Formation.FormOrder = FormOrder.FormOrderWide;
			base.Formation.WeaponUsageOrder = WeaponUsageOrder.WeaponUsageOrderUseAny;
		}

		// Token: 0x06000C5B RID: 3163 RVA: 0x00018550 File Offset: 0x00016750
		public override void TickOccasionally()
		{
			this.CalculateCurrentOrder();
			base.Formation.SetMovementOrder(base.CurrentOrder);
			base.Formation.FacingOrder = this.CurrentFacingOrder;
			if (base.Formation.IsInfantry())
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

		// Token: 0x06000C5C RID: 3164 RVA: 0x000186C0 File Offset: 0x000168C0
		protected override float GetAiWeight()
		{
			return 1f;
		}

		// Token: 0x040002CB RID: 715
		private bool _isInShieldWallDistance;

		// Token: 0x040002CC RID: 716
		private bool _switchedToShieldWallRecently;

		// Token: 0x040002CD RID: 717
		private Timer _switchedToShieldWallTimer;

		// Token: 0x040002CE RID: 718
		private Vec2 _reformPosition = Vec2.Invalid;
	}
}
