using System;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000102 RID: 258
	public class BehaviorDefendSiegeWeapon : BehaviorComponent
	{
		// Token: 0x06000CB6 RID: 3254 RVA: 0x0001BF68 File Offset: 0x0001A168
		public BehaviorDefendSiegeWeapon(Formation formation)
			: base(formation)
		{
			this.CalculateCurrentOrder();
		}

		// Token: 0x06000CB7 RID: 3255 RVA: 0x0001BF82 File Offset: 0x0001A182
		public void SetDefensePositionFromTactic(WorldPosition defensePosition)
		{
			this._defensePosition = defensePosition;
		}

		// Token: 0x06000CB8 RID: 3256 RVA: 0x0001BF8B File Offset: 0x0001A18B
		public void SetDefendedSiegeWeaponFromTactic(SiegeWeapon siegeWeapon)
		{
			this._defendedSiegeWeapon = siegeWeapon;
		}

		// Token: 0x06000CB9 RID: 3257 RVA: 0x0001BF94 File Offset: 0x0001A194
		public override TextObject GetBehaviorString()
		{
			TextObject behaviorString = base.GetBehaviorString();
			TextObject textObject = GameTexts.FindText("str_formation_ai_side_strings", base.Formation.AI.Side.ToString());
			behaviorString.SetTextVariable("SIDE_STRING", textObject);
			behaviorString.SetTextVariable("IS_GENERAL_SIDE", "0");
			return behaviorString;
		}

		// Token: 0x06000CBA RID: 3258 RVA: 0x0001BFF0 File Offset: 0x0001A1F0
		protected override void CalculateCurrentOrder()
		{
			float num = 5f;
			Vec2 vec;
			if (this._tacticalDefendPosition != null)
			{
				if (!this._tacticalDefendPosition.IsInsurmountable)
				{
					vec = this._tacticalDefendPosition.Direction;
				}
				else
				{
					vec = (base.Formation.Team.QuerySystem.AverageEnemyPosition - this._tacticalDefendPosition.Position.AsVec2).Normalized();
				}
			}
			else if (base.Formation.QuerySystem.ClosestEnemyFormation == null)
			{
				vec = base.Formation.Direction;
			}
			else if (this._defendedSiegeWeapon != null)
			{
				vec = base.Formation.QuerySystem.ClosestEnemyFormation.MedianPosition.AsVec2 - this._defendedSiegeWeapon.GameEntity.GlobalPosition.AsVec2;
				num = vec.Normalize();
				num = MathF.Min(num, 5f);
				float num2;
				if (this._defendedSiegeWeapon.WaitEntity != null)
				{
					num2 = (this._defendedSiegeWeapon.WaitEntity.GlobalPosition - this._defendedSiegeWeapon.GameEntity.GlobalPosition).Length;
				}
				else
				{
					num2 = 3f;
				}
				num = MathF.Max(num, num2);
			}
			else
			{
				vec = ((base.Formation.Direction.DotProduct((base.Formation.QuerySystem.ClosestEnemyFormation.MedianPosition.AsVec2 - base.Formation.QuerySystem.AveragePosition).Normalized()) < 0.5f) ? (base.Formation.QuerySystem.ClosestEnemyFormation.MedianPosition.AsVec2 - base.Formation.QuerySystem.AveragePosition) : base.Formation.Direction).Normalized();
			}
			if (this._tacticalDefendPosition != null)
			{
				if (!this._tacticalDefendPosition.IsInsurmountable)
				{
					base.CurrentOrder = MovementOrder.MovementOrderMove(this._tacticalDefendPosition.Position);
				}
				else
				{
					Vec2 vec2 = this._tacticalDefendPosition.Position.AsVec2 + this._tacticalDefendPosition.Width * 0.5f * vec;
					WorldPosition position = this._tacticalDefendPosition.Position;
					position.SetVec2(vec2);
					base.CurrentOrder = MovementOrder.MovementOrderMove(position);
				}
				if (!this._tacticalDefendPosition.IsInsurmountable)
				{
					this.CurrentFacingOrder = FacingOrder.FacingOrderLookAtDirection(vec);
					return;
				}
				this.CurrentFacingOrder = FacingOrder.FacingOrderLookAtEnemy;
				return;
			}
			else
			{
				if (this._defensePosition.IsValid)
				{
					WorldPosition defensePosition = this._defensePosition;
					defensePosition.SetVec2(this._defensePosition.AsVec2 + vec * num);
					base.CurrentOrder = MovementOrder.MovementOrderMove(defensePosition);
					this.CurrentFacingOrder = FacingOrder.FacingOrderLookAtDirection(vec);
					return;
				}
				WorldPosition medianPosition = base.Formation.QuerySystem.MedianPosition;
				medianPosition.SetVec2(base.Formation.QuerySystem.AveragePosition);
				base.CurrentOrder = MovementOrder.MovementOrderMove(medianPosition);
				this.CurrentFacingOrder = FacingOrder.FacingOrderLookAtDirection(vec);
				return;
			}
		}

		// Token: 0x06000CBB RID: 3259 RVA: 0x0001C31C File Offset: 0x0001A51C
		public override void TickOccasionally()
		{
			this.CalculateCurrentOrder();
			base.Formation.SetMovementOrder(base.CurrentOrder);
			base.Formation.FacingOrder = this.CurrentFacingOrder;
			if (base.Formation.QuerySystem.AveragePosition.DistanceSquared(base.CurrentOrder.GetPosition(base.Formation)) < 100f)
			{
				if (base.Formation.QuerySystem.HasShield)
				{
					base.Formation.ArrangementOrder = ArrangementOrder.ArrangementOrderLine;
				}
				else if (base.Formation.QuerySystem.ClosestSignificantlyLargeEnemyFormation != null && base.Formation.QuerySystem.AveragePosition.DistanceSquared(base.Formation.QuerySystem.ClosestSignificantlyLargeEnemyFormation.MedianPosition.AsVec2) > 100f && base.Formation.QuerySystem.UnderRangedAttackRatio > 0.2f - ((base.Formation.ArrangementOrder.OrderEnum == ArrangementOrder.ArrangementOrderEnum.Loose) ? 0.1f : 0f))
				{
					base.Formation.ArrangementOrder = ArrangementOrder.ArrangementOrderLoose;
				}
				if (this._tacticalDefendPosition != null)
				{
					float num;
					if (this._tacticalDefendPosition.TacticalPositionType == TacticalPosition.TacticalPositionTypeEnum.ChokePoint)
					{
						num = this._tacticalDefendPosition.Width;
					}
					else
					{
						int countOfUnits = base.Formation.CountOfUnits;
						float num2 = base.Formation.Interval * (float)(countOfUnits - 1) + base.Formation.UnitDiameter * (float)countOfUnits;
						num = MathF.Min(this._tacticalDefendPosition.Width, num2 / 3f);
					}
					base.Formation.FormOrder = FormOrder.FormOrderCustom(num);
					return;
				}
			}
			else
			{
				base.Formation.ArrangementOrder = ArrangementOrder.ArrangementOrderLoose;
			}
		}

		// Token: 0x06000CBC RID: 3260 RVA: 0x0001C4D8 File Offset: 0x0001A6D8
		protected override void OnBehaviorActivatedAux()
		{
			this.CalculateCurrentOrder();
			base.Formation.SetMovementOrder(base.CurrentOrder);
			base.Formation.FacingOrder = this.CurrentFacingOrder;
			base.Formation.ArrangementOrder = ArrangementOrder.ArrangementOrderLoose;
			base.Formation.FiringOrder = FiringOrder.FiringOrderFireAtWill;
			base.Formation.FormOrder = FormOrder.FormOrderWide;
			base.Formation.WeaponUsageOrder = WeaponUsageOrder.WeaponUsageOrderUseAny;
		}

		// Token: 0x06000CBD RID: 3261 RVA: 0x0001C54D File Offset: 0x0001A74D
		public override void ResetBehavior()
		{
			base.ResetBehavior();
			this._defensePosition = WorldPosition.Invalid;
			this._tacticalDefendPosition = null;
		}

		// Token: 0x06000CBE RID: 3262 RVA: 0x0001C567 File Offset: 0x0001A767
		protected override float GetAiWeight()
		{
			return 1f;
		}

		// Token: 0x0400030B RID: 779
		private WorldPosition _defensePosition = WorldPosition.Invalid;

		// Token: 0x0400030C RID: 780
		private TacticalPosition _tacticalDefendPosition;

		// Token: 0x0400030D RID: 781
		private SiegeWeapon _defendedSiegeWeapon;
	}
}
