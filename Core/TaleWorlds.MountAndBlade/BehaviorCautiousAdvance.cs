using System;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.LinQuick;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x020000FB RID: 251
	public sealed class BehaviorCautiousAdvance : BehaviorComponent
	{
		// Token: 0x06000C6A RID: 3178 RVA: 0x0001913C File Offset: 0x0001733C
		public BehaviorCautiousAdvance()
		{
			base.BehaviorCoherence = 1f;
			this._cantShootTimer = new Timer(0f, 0f, true);
			this._switchedToShieldWallTimer = new Timer(0f, 0f, true);
		}

		// Token: 0x06000C6B RID: 3179 RVA: 0x000191B0 File Offset: 0x000173B0
		public BehaviorCautiousAdvance(Formation formation)
			: base(formation)
		{
			base.BehaviorCoherence = 1f;
			this._cantShootTimer = new Timer(0f, 0f, true);
			this._switchedToShieldWallTimer = new Timer(0f, 0f, true);
			this.CalculateCurrentOrder();
		}

		// Token: 0x06000C6C RID: 3180 RVA: 0x0001922C File Offset: 0x0001742C
		protected override void CalculateCurrentOrder()
		{
			WorldPosition worldPosition = base.Formation.QuerySystem.MedianPosition;
			bool flag = false;
			Vec2 vec;
			if (this._targetFormation == null || this._archerFormation == null)
			{
				vec = base.Formation.Direction;
				worldPosition.SetVec2(base.Formation.QuerySystem.AveragePosition);
			}
			else
			{
				vec = this._targetFormation.MedianPosition.AsVec2 - base.Formation.QuerySystem.AveragePosition;
				float num = vec.Normalize();
				float num2 = this._archerFormation.QuerySystem.RangedUnitRatio * 0.5f / (float)this._archerFormation.Arrangement.RankCount;
				switch (this._behaviorState)
				{
				case BehaviorCautiousAdvance.BehaviorState.Approaching:
					if (num < this._cantShootDistance * 0.8f)
					{
						this._behaviorState = BehaviorCautiousAdvance.BehaviorState.Shooting;
						this._cantShoot = false;
						flag = true;
					}
					else if (this._archerFormation.QuerySystem.MakingRangedAttackRatio >= num2 * 1.2f)
					{
						this._behaviorState = BehaviorCautiousAdvance.BehaviorState.Shooting;
						this._cantShoot = false;
						flag = true;
					}
					if (this._behaviorState == BehaviorCautiousAdvance.BehaviorState.Shooting)
					{
						this._shootPosition = base.Formation.QuerySystem.AveragePosition + vec * 5f;
					}
					break;
				case BehaviorCautiousAdvance.BehaviorState.Shooting:
					if (this._archerFormation.QuerySystem.MakingRangedAttackRatio <= num2)
					{
						if (num > this._archerFormation.QuerySystem.MaximumMissileRange)
						{
							this._behaviorState = BehaviorCautiousAdvance.BehaviorState.Approaching;
							this._cantShootDistance = MathF.Min(this._cantShootDistance, this._archerFormation.QuerySystem.MaximumMissileRange * 0.9f);
							this._shootPosition = Vec2.Invalid;
						}
						else if (!this._cantShoot)
						{
							this._cantShoot = true;
							this._cantShootTimer.Reset(Mission.Current.CurrentTime, (this._archerFormation == null) ? 10f : MBMath.Lerp(10f, 15f, (MBMath.ClampFloat((float)this._archerFormation.CountOfUnits, 10f, 60f) - 10f) * 0.02f, 1E-05f));
						}
						else if (this._cantShootTimer.Check(Mission.Current.CurrentTime))
						{
							this._behaviorState = BehaviorCautiousAdvance.BehaviorState.Approaching;
							this._cantShootDistance = MathF.Min(this._cantShootDistance, num);
							this._shootPosition = Vec2.Invalid;
						}
					}
					else
					{
						this._cantShootDistance = MathF.Max(this._cantShootDistance, num);
						this._cantShoot = false;
						if (((!this._targetFormation.IsRangedFormation && !this._targetFormation.IsRangedCavalryFormation) || (num < this._targetFormation.MissileRangeAdjusted && this._targetFormation.MakingRangedAttackRatio < 0.1f)) && num < MathF.Min(this._archerFormation.QuerySystem.MissileRangeAdjusted * 0.4f, this._cantShootDistance * 0.667f))
						{
							this._behaviorState = BehaviorCautiousAdvance.BehaviorState.PullingBack;
							this._shootPosition = Vec2.Invalid;
						}
					}
					break;
				case BehaviorCautiousAdvance.BehaviorState.PullingBack:
					if (num > MathF.Min(this._cantShootDistance, this._archerFormation.QuerySystem.MissileRangeAdjusted) * 0.8f)
					{
						this._behaviorState = BehaviorCautiousAdvance.BehaviorState.Shooting;
						this._cantShoot = false;
						this._shootPosition = base.Formation.QuerySystem.AveragePosition + vec * 5f;
						flag = true;
					}
					break;
				}
				switch (this._behaviorState)
				{
				case BehaviorCautiousAdvance.BehaviorState.Approaching:
					worldPosition = this._targetFormation.MedianPosition;
					if (this._switchedToShieldWallRecently && !this._switchedToShieldWallTimer.Check(Mission.Current.CurrentTime) && base.Formation.QuerySystem.FormationDispersedness > 2f)
					{
						if (this._reformPosition.IsValid)
						{
							worldPosition.SetVec2(this._reformPosition);
						}
						else
						{
							vec = (base.Formation.QuerySystem.Team.MedianTargetFormationPosition.AsVec2 - base.Formation.QuerySystem.AveragePosition).Normalized();
							this._reformPosition = base.Formation.QuerySystem.AveragePosition + vec * 5f;
							worldPosition.SetVec2(this._reformPosition);
						}
					}
					else
					{
						this._switchedToShieldWallRecently = false;
						this._reformPosition = Vec2.Invalid;
						worldPosition.SetVec2(this._targetFormation.AveragePosition);
					}
					break;
				case BehaviorCautiousAdvance.BehaviorState.Shooting:
					if (this._shootPosition.IsValid)
					{
						worldPosition.SetVec2(this._shootPosition);
					}
					else
					{
						worldPosition.SetVec2(base.Formation.QuerySystem.AveragePosition);
					}
					break;
				case BehaviorCautiousAdvance.BehaviorState.PullingBack:
					worldPosition = base.Formation.QuerySystem.MedianPosition;
					worldPosition.SetVec2(base.Formation.QuerySystem.AveragePosition);
					break;
				}
			}
			if (!base.CurrentOrder.CreateNewOrderWorldPosition(base.Formation, WorldPosition.WorldPositionEnforcedCache.None).IsValid || this._behaviorState != BehaviorCautiousAdvance.BehaviorState.Shooting || flag || base.CurrentOrder.CreateNewOrderWorldPosition(base.Formation, WorldPosition.WorldPositionEnforcedCache.NavMeshVec3).GetNavMeshVec3().DistanceSquared(worldPosition.GetNavMeshVec3()) >= base.Formation.Depth * base.Formation.Depth)
			{
				base.CurrentOrder = MovementOrder.MovementOrderMove(worldPosition);
			}
			if (!this.CurrentFacingOrder.GetDirection(base.Formation, null).IsValid || this._behaviorState != BehaviorCautiousAdvance.BehaviorState.Shooting || flag || this.CurrentFacingOrder.GetDirection(base.Formation, null).DotProduct(vec) <= MBMath.Lerp(0.5f, 1f, 1f - MBMath.ClampFloat(base.Formation.Width, 1f, 20f) * 0.05f, 1E-05f))
			{
				this.CurrentFacingOrder = FacingOrder.FacingOrderLookAtDirection(vec);
			}
		}

		// Token: 0x06000C6D RID: 3181 RVA: 0x00019838 File Offset: 0x00017A38
		protected override void OnBehaviorActivatedAux()
		{
			IEnumerable<Formation> enumerable = base.Formation.Team.FormationsIncludingEmpty.WhereQ((Formation f) => f.CountOfUnits > 0 && f != base.Formation && f.QuerySystem.IsRangedFormation);
			if (enumerable.AnyQ<Formation>())
			{
				this._archerFormation = enumerable.MaxBy((Formation f) => f.QuerySystem.FormationPower);
			}
			this._cantShoot = false;
			this._cantShootDistance = float.MaxValue;
			this._behaviorState = BehaviorCautiousAdvance.BehaviorState.Shooting;
			this._cantShootTimer.Reset(Mission.Current.CurrentTime, (this._archerFormation == null) ? 10f : MBMath.Lerp(10f, 15f, (MBMath.ClampFloat((float)this._archerFormation.CountOfUnits, 10f, 60f) - 10f) * 0.02f, 1E-05f));
			this._targetFormation = base.Formation.QuerySystem.ClosestSignificantlyLargeEnemyFormation ?? base.Formation.QuerySystem.ClosestEnemyFormation;
			this.CalculateCurrentOrder();
			base.Formation.SetMovementOrder(base.CurrentOrder);
			base.Formation.FacingOrder = this.CurrentFacingOrder;
			this._isInShieldWallDistance = true;
			base.Formation.ArrangementOrder = ArrangementOrder.ArrangementOrderLine;
			base.Formation.FiringOrder = FiringOrder.FiringOrderFireAtWill;
			base.Formation.FormOrder = FormOrder.FormOrderWide;
			base.Formation.WeaponUsageOrder = WeaponUsageOrder.WeaponUsageOrderUseAny;
		}

		// Token: 0x06000C6E RID: 3182 RVA: 0x000199AB File Offset: 0x00017BAB
		public override void OnBehaviorCanceled()
		{
		}

		// Token: 0x06000C6F RID: 3183 RVA: 0x000199B0 File Offset: 0x00017BB0
		public override void TickOccasionally()
		{
			this._targetFormation = base.Formation.QuerySystem.ClosestSignificantlyLargeEnemyFormation ?? base.Formation.QuerySystem.ClosestEnemyFormation;
			if (base.Formation.IsInfantry())
			{
				bool flag = this._targetFormation != null && (base.Formation.QuerySystem.IsUnderRangedAttack || base.Formation.QuerySystem.AveragePosition.DistanceSquared(base.CurrentOrder.GetPosition(base.Formation)) < 25f + (this._isInShieldWallDistance ? 75f : 0f)) && base.Formation.QuerySystem.AveragePosition.DistanceSquared(this._targetFormation.MedianPosition.AsVec2) > 100f - (this._isInShieldWallDistance ? 75f : 0f);
				if (flag != this._isInShieldWallDistance)
				{
					this._isInShieldWallDistance = flag;
					if (this._isInShieldWallDistance)
					{
						ArrangementOrder arrangementOrder = (base.Formation.QuerySystem.HasShield ? ArrangementOrder.ArrangementOrderShieldWall : ArrangementOrder.ArrangementOrderLoose);
						if (base.Formation.ArrangementOrder != arrangementOrder)
						{
							base.Formation.ArrangementOrder = arrangementOrder;
							this._switchedToShieldWallRecently = true;
							this._switchedToShieldWallTimer.Reset(Mission.Current.CurrentTime, 5f);
						}
					}
					else if (!(base.Formation.ArrangementOrder == ArrangementOrder.ArrangementOrderLine))
					{
						base.Formation.ArrangementOrder = ArrangementOrder.ArrangementOrderLine;
					}
				}
			}
			this.CalculateCurrentOrder();
			base.Formation.SetMovementOrder(base.CurrentOrder);
			base.Formation.FacingOrder = this.CurrentFacingOrder;
		}

		// Token: 0x06000C70 RID: 3184 RVA: 0x00019B75 File Offset: 0x00017D75
		protected override float GetAiWeight()
		{
			return 1f;
		}

		// Token: 0x040002DD RID: 733
		private bool _isInShieldWallDistance;

		// Token: 0x040002DE RID: 734
		private bool _switchedToShieldWallRecently;

		// Token: 0x040002DF RID: 735
		private Timer _switchedToShieldWallTimer;

		// Token: 0x040002E0 RID: 736
		private Vec2 _reformPosition = Vec2.Invalid;

		// Token: 0x040002E1 RID: 737
		private Formation _archerFormation;

		// Token: 0x040002E2 RID: 738
		private bool _cantShoot;

		// Token: 0x040002E3 RID: 739
		private float _cantShootDistance = float.MaxValue;

		// Token: 0x040002E4 RID: 740
		private BehaviorCautiousAdvance.BehaviorState _behaviorState = BehaviorCautiousAdvance.BehaviorState.Shooting;

		// Token: 0x040002E5 RID: 741
		private Timer _cantShootTimer;

		// Token: 0x040002E6 RID: 742
		private Vec2 _shootPosition = Vec2.Invalid;

		// Token: 0x040002E7 RID: 743
		private FormationQuerySystem _targetFormation;

		// Token: 0x0200043C RID: 1084
		private enum BehaviorState
		{
			// Token: 0x04001840 RID: 6208
			Approaching,
			// Token: 0x04001841 RID: 6209
			Shooting,
			// Token: 0x04001842 RID: 6210
			PullingBack
		}
	}
}
