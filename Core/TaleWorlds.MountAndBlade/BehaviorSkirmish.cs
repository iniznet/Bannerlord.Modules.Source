using System;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x0200011E RID: 286
	public class BehaviorSkirmish : BehaviorComponent
	{
		// Token: 0x06000D8C RID: 3468 RVA: 0x00023570 File Offset: 0x00021770
		public BehaviorSkirmish(Formation formation)
			: base(formation)
		{
			base.BehaviorCoherence = 0.5f;
			this._cantShootTimer = new Timer(0f, 0f, true);
			this._pullBackTimer = new Timer(0f, 0f, true);
			this.CalculateCurrentOrder();
		}

		// Token: 0x06000D8D RID: 3469 RVA: 0x000235E0 File Offset: 0x000217E0
		protected override void CalculateCurrentOrder()
		{
			WorldPosition worldPosition = base.Formation.QuerySystem.MedianPosition;
			bool flag = false;
			Vec2 vec;
			if (base.Formation.QuerySystem.ClosestSignificantlyLargeEnemyFormation == null)
			{
				vec = base.Formation.Direction;
				worldPosition.SetVec2(base.Formation.QuerySystem.AveragePosition);
			}
			else
			{
				vec = base.Formation.QuerySystem.ClosestSignificantlyLargeEnemyFormation.MedianPosition.AsVec2 - base.Formation.QuerySystem.AveragePosition;
				float num = vec.Normalize();
				float num2 = base.Formation.QuerySystem.ClosestSignificantlyLargeEnemyFormation.CurrentVelocity.DotProduct(vec);
				float num3 = MBMath.Lerp(5f, 10f, (MBMath.ClampFloat((float)base.Formation.CountOfUnits, 10f, 60f) - 10f) * 0.02f, 1E-05f) * num2;
				num += num3;
				float num4 = MBMath.Lerp(0.1f, 0.33f, 1f - MBMath.ClampFloat((float)base.Formation.CountOfUnits, 1f, 50f) * 0.02f, 1E-05f) * base.Formation.QuerySystem.RangedUnitRatio;
				switch (this._behaviorState)
				{
				case BehaviorSkirmish.BehaviorState.Approaching:
					if (num < this._cantShootDistance * 0.8f)
					{
						this._behaviorState = BehaviorSkirmish.BehaviorState.Shooting;
						this._cantShoot = false;
						flag = true;
					}
					else if (base.Formation.QuerySystem.MakingRangedAttackRatio >= num4 * 1.2f)
					{
						this._behaviorState = BehaviorSkirmish.BehaviorState.Shooting;
						this._cantShoot = false;
						flag = true;
					}
					break;
				case BehaviorSkirmish.BehaviorState.Shooting:
					if (base.Formation.QuerySystem.MakingRangedAttackRatio <= num4)
					{
						if (num > base.Formation.QuerySystem.MaximumMissileRange)
						{
							this._behaviorState = BehaviorSkirmish.BehaviorState.Approaching;
							this._cantShootDistance = MathF.Min(this._cantShootDistance, base.Formation.QuerySystem.MaximumMissileRange * 0.9f);
						}
						else if (!this._cantShoot)
						{
							this._cantShoot = true;
							this._cantShootTimer.Reset(Mission.Current.CurrentTime, MBMath.Lerp(5f, 10f, (MBMath.ClampFloat((float)base.Formation.CountOfUnits, 10f, 60f) - 10f) * 0.02f, 1E-05f));
						}
						else if (this._cantShootTimer.Check(Mission.Current.CurrentTime))
						{
							this._behaviorState = BehaviorSkirmish.BehaviorState.Approaching;
							this._cantShootDistance = MathF.Min(this._cantShootDistance, num);
						}
					}
					else
					{
						this._cantShootDistance = MathF.Max(this._cantShootDistance, num);
						this._cantShoot = false;
						if (this._pullBackTimer.Check(Mission.Current.CurrentTime) && base.Formation.QuerySystem.ClosestSignificantlyLargeEnemyFormation.IsInfantryFormation && num < MathF.Min(base.Formation.QuerySystem.MissileRangeAdjusted * 0.4f, this._cantShootDistance * 0.666f))
						{
							this._behaviorState = BehaviorSkirmish.BehaviorState.PullingBack;
							this._pullBackTimer.Reset(Mission.Current.CurrentTime, 10f);
						}
					}
					break;
				case BehaviorSkirmish.BehaviorState.PullingBack:
					if (num > MathF.Min(this._cantShootDistance, base.Formation.QuerySystem.MissileRangeAdjusted) * 0.8f)
					{
						this._behaviorState = BehaviorSkirmish.BehaviorState.Shooting;
						this._cantShoot = false;
						flag = true;
					}
					else if (this._pullBackTimer.Check(Mission.Current.CurrentTime) && base.Formation.QuerySystem.MakingRangedAttackRatio <= num4 * 0.5f)
					{
						this._behaviorState = BehaviorSkirmish.BehaviorState.Shooting;
						this._cantShoot = false;
						flag = true;
						this._pullBackTimer.Reset(Mission.Current.CurrentTime, 5f);
					}
					break;
				}
				switch (this._behaviorState)
				{
				case BehaviorSkirmish.BehaviorState.Approaching:
				{
					bool flag2 = false;
					if (this._alternatePositionUsed)
					{
						float num5 = base.Formation.QuerySystem.AveragePosition.DistanceSquared(base.Formation.QuerySystem.ClosestSignificantlyLargeEnemyFormation.AveragePosition);
						Vec2 vec2 = (base.Formation.QuerySystem.AveragePosition + base.Formation.QuerySystem.ClosestSignificantlyLargeEnemyFormation.AveragePosition) * 0.5f;
						bool flag3 = (double)this._alternatePosition.AsVec2.DistanceSquared(vec2) > (double)num5 * 0.0625;
						if (!flag3)
						{
							int num6 = -1;
							Vec3 navMeshVec = this._alternatePosition.GetNavMeshVec3();
							Mission.Current.Scene.GetNavigationMeshForPosition(ref navMeshVec, out num6, 1.5f);
							Agent medianAgent = base.Formation.GetMedianAgent(true, true, base.Formation.QuerySystem.AveragePosition);
							flag3 = (medianAgent != null && medianAgent.GetCurrentNavigationFaceId() % 10 == 1) != (num6 % 10 == 1);
						}
						if (flag3)
						{
							Agent medianAgent2 = base.Formation.GetMedianAgent(true, true, base.Formation.QuerySystem.AveragePosition);
							bool flag4 = medianAgent2 != null && medianAgent2.GetCurrentNavigationFaceId() % 10 == 1;
							Agent medianAgent3 = base.Formation.QuerySystem.ClosestSignificantlyLargeEnemyFormation.Formation.GetMedianAgent(true, true, base.Formation.QuerySystem.ClosestSignificantlyLargeEnemyFormation.AveragePosition);
							if (flag4 == (medianAgent3 != null && medianAgent3.GetCurrentNavigationFaceId() % 10 == 1))
							{
								this._alternatePositionUsed = false;
								this._alternatePosition = WorldPosition.Invalid;
							}
							else
							{
								flag2 = true;
							}
						}
					}
					else if (Mission.Current.MissionTeamAIType == Mission.MissionTeamAITypeEnum.Siege || Mission.Current.MissionTeamAIType == Mission.MissionTeamAITypeEnum.SallyOut)
					{
						Agent medianAgent4 = base.Formation.GetMedianAgent(true, true, base.Formation.QuerySystem.AveragePosition);
						bool flag5 = medianAgent4 != null && medianAgent4.GetCurrentNavigationFaceId() % 10 == 1;
						Agent medianAgent5 = base.Formation.QuerySystem.ClosestSignificantlyLargeEnemyFormation.Formation.GetMedianAgent(true, true, base.Formation.QuerySystem.ClosestSignificantlyLargeEnemyFormation.AveragePosition);
						if (flag5 != (medianAgent5 != null && medianAgent5.GetCurrentNavigationFaceId() % 10 == 1))
						{
							this._alternatePositionUsed = true;
							flag2 = true;
						}
					}
					if (this._alternatePositionUsed)
					{
						if (flag2)
						{
							this._alternatePosition = new WorldPosition(Mission.Current.Scene, new Vec3((base.Formation.QuerySystem.AveragePosition + base.Formation.QuerySystem.ClosestSignificantlyLargeEnemyFormation.AveragePosition) * 0.5f, base.Formation.QuerySystem.MedianPosition.GetNavMeshZ(), -1f));
						}
						worldPosition = this._alternatePosition;
					}
					else
					{
						worldPosition = base.Formation.QuerySystem.ClosestSignificantlyLargeEnemyFormation.MedianPosition;
						worldPosition.SetVec2(base.Formation.QuerySystem.ClosestSignificantlyLargeEnemyFormation.AveragePosition);
					}
					break;
				}
				case BehaviorSkirmish.BehaviorState.Shooting:
					worldPosition.SetVec2(base.Formation.QuerySystem.AveragePosition + base.Formation.QuerySystem.CurrentVelocity.Normalized() * (base.Formation.Depth * 0.5f));
					break;
				case BehaviorSkirmish.BehaviorState.PullingBack:
					worldPosition = base.Formation.QuerySystem.ClosestSignificantlyLargeEnemyFormation.MedianPosition;
					worldPosition.SetVec2(worldPosition.AsVec2 - vec * (base.Formation.QuerySystem.MissileRangeAdjusted - base.Formation.Depth * 0.5f - 10f));
					break;
				}
			}
			if (!base.CurrentOrder.GetPosition(base.Formation).IsValid || this._behaviorState != BehaviorSkirmish.BehaviorState.Shooting || flag)
			{
				base.CurrentOrder = MovementOrder.MovementOrderMove(worldPosition);
			}
			if (!this.CurrentFacingOrder.GetDirection(base.Formation, null).IsValid || this._behaviorState != BehaviorSkirmish.BehaviorState.Shooting || flag)
			{
				this.CurrentFacingOrder = FacingOrder.FacingOrderLookAtDirection(vec);
			}
		}

		// Token: 0x06000D8E RID: 3470 RVA: 0x00023DFD File Offset: 0x00021FFD
		public override void TickOccasionally()
		{
			this.CalculateCurrentOrder();
			base.Formation.SetMovementOrder(base.CurrentOrder);
			base.Formation.FacingOrder = this.CurrentFacingOrder;
		}

		// Token: 0x06000D8F RID: 3471 RVA: 0x00023E28 File Offset: 0x00022028
		protected override void OnBehaviorActivatedAux()
		{
			this._cantShoot = false;
			this._cantShootDistance = float.MaxValue;
			this._behaviorState = BehaviorSkirmish.BehaviorState.Shooting;
			this._cantShootTimer.Reset(Mission.Current.CurrentTime, MBMath.Lerp(5f, 10f, (MBMath.ClampFloat((float)base.Formation.CountOfUnits, 10f, 60f) - 10f) * 0.02f, 1E-05f));
			this._pullBackTimer.Reset(0f, 0f);
			this.CalculateCurrentOrder();
			base.Formation.SetMovementOrder(base.CurrentOrder);
			base.Formation.FacingOrder = this.CurrentFacingOrder;
			base.Formation.ArrangementOrder = ArrangementOrder.ArrangementOrderLoose;
			base.Formation.FiringOrder = FiringOrder.FiringOrderFireAtWill;
			base.Formation.FormOrder = FormOrder.FormOrderWide;
			base.Formation.WeaponUsageOrder = WeaponUsageOrder.WeaponUsageOrderUseAny;
		}

		// Token: 0x06000D90 RID: 3472 RVA: 0x00023F1C File Offset: 0x0002211C
		protected override float GetAiWeight()
		{
			FormationQuerySystem querySystem = base.Formation.QuerySystem;
			return MBMath.Lerp(0.1f, 1f, MBMath.ClampFloat(querySystem.RangedUnitRatio + querySystem.RangedCavalryUnitRatio, 0f, 0.5f) * 2f, 1E-05f);
		}

		// Token: 0x04000346 RID: 838
		private bool _cantShoot;

		// Token: 0x04000347 RID: 839
		private float _cantShootDistance = float.MaxValue;

		// Token: 0x04000348 RID: 840
		private bool _alternatePositionUsed;

		// Token: 0x04000349 RID: 841
		private WorldPosition _alternatePosition = WorldPosition.Invalid;

		// Token: 0x0400034A RID: 842
		private BehaviorSkirmish.BehaviorState _behaviorState = BehaviorSkirmish.BehaviorState.Shooting;

		// Token: 0x0400034B RID: 843
		private Timer _cantShootTimer;

		// Token: 0x0400034C RID: 844
		private Timer _pullBackTimer;

		// Token: 0x02000457 RID: 1111
		private enum BehaviorState
		{
			// Token: 0x04001893 RID: 6291
			Approaching,
			// Token: 0x04001894 RID: 6292
			Shooting,
			// Token: 0x04001895 RID: 6293
			PullingBack
		}
	}
}
