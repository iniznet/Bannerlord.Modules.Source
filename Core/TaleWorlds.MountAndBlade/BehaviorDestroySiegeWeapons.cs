using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000104 RID: 260
	public class BehaviorDestroySiegeWeapons : BehaviorComponent
	{
		// Token: 0x06000CC5 RID: 3269 RVA: 0x0001C998 File Offset: 0x0001AB98
		private void DetermineTargetWeapons()
		{
			this._targetWeapons = this._allWeapons.Where((SiegeWeapon w) => w is IPrimarySiegeWeapon && (w as IPrimarySiegeWeapon).WeaponSide == this._behaviorSide && w.IsDestructible && !w.IsDestroyed && !w.IsDisabled).ToList<SiegeWeapon>();
			if (this._targetWeapons.IsEmpty<SiegeWeapon>())
			{
				this._targetWeapons = this._allWeapons.Where((SiegeWeapon w) => !(w is IPrimarySiegeWeapon) && w.IsDestructible && !w.IsDestroyed && !w.IsDisabled).ToList<SiegeWeapon>();
				this._isTargetPrimaryWeapon = false;
				return;
			}
			this._isTargetPrimaryWeapon = true;
		}

		// Token: 0x06000CC6 RID: 3270 RVA: 0x0001CA18 File Offset: 0x0001AC18
		public BehaviorDestroySiegeWeapons(Formation formation)
			: base(formation)
		{
			base.BehaviorCoherence = 0.2f;
			this._behaviorSide = formation.AI.Side;
			this._allWeapons = (from sw in Mission.Current.ActiveMissionObjects.FindAllWithType<SiegeWeapon>()
				where sw.Side != formation.Team.Side
				select sw).ToList<SiegeWeapon>();
			this.DetermineTargetWeapons();
			base.CurrentOrder = MovementOrder.MovementOrderCharge;
		}

		// Token: 0x06000CC7 RID: 3271 RVA: 0x0001CA9C File Offset: 0x0001AC9C
		public override TextObject GetBehaviorString()
		{
			TextObject behaviorString = base.GetBehaviorString();
			TextObject textObject = GameTexts.FindText("str_formation_ai_side_strings", base.Formation.AI.Side.ToString());
			behaviorString.SetTextVariable("SIDE_STRING", textObject);
			behaviorString.SetTextVariable("IS_GENERAL_SIDE", "0");
			return behaviorString;
		}

		// Token: 0x06000CC8 RID: 3272 RVA: 0x0001CAF6 File Offset: 0x0001ACF6
		public override void OnValidBehaviorSideChanged()
		{
			base.OnValidBehaviorSideChanged();
			this.DetermineTargetWeapons();
		}

		// Token: 0x06000CC9 RID: 3273 RVA: 0x0001CB04 File Offset: 0x0001AD04
		public override void TickOccasionally()
		{
			base.TickOccasionally();
			this._targetWeapons.RemoveAll((SiegeWeapon tw) => tw.IsDestroyed);
			if (this._targetWeapons.Count == 0)
			{
				this.DetermineTargetWeapons();
			}
			if (base.Formation.AI.ActiveBehavior == this)
			{
				if (this._targetWeapons.Count == 0)
				{
					MovementOrder currentOrder = base.CurrentOrder;
					if ((currentOrder) != MovementOrder.MovementOrderCharge)
					{
						base.CurrentOrder = MovementOrder.MovementOrderCharge;
					}
					this._isTargetPrimaryWeapon = false;
				}
				else
				{
					SiegeWeapon siegeWeapon = this._targetWeapons.MinBy((SiegeWeapon tw) => base.Formation.QuerySystem.AveragePosition.DistanceSquared(tw.GameEntity.GlobalPosition.AsVec2));
					if (base.CurrentOrder.OrderEnum != MovementOrder.MovementOrderEnum.AttackEntity || this.LastTargetWeapon != siegeWeapon)
					{
						this.LastTargetWeapon = siegeWeapon;
						base.CurrentOrder = MovementOrder.MovementOrderAttackEntity(this.LastTargetWeapon.GameEntity, true);
					}
				}
				base.Formation.SetMovementOrder(base.CurrentOrder);
			}
		}

		// Token: 0x06000CCA RID: 3274 RVA: 0x0001CC00 File Offset: 0x0001AE00
		protected override void OnBehaviorActivatedAux()
		{
			this.DetermineTargetWeapons();
			base.Formation.ArrangementOrder = ((base.Formation.QuerySystem.IsCavalryFormation || base.Formation.QuerySystem.IsRangedCavalryFormation) ? ArrangementOrder.ArrangementOrderSkein : ArrangementOrder.ArrangementOrderLine);
			base.Formation.FacingOrder = FacingOrder.FacingOrderLookAtEnemy;
			base.Formation.FiringOrder = FiringOrder.FiringOrderFireAtWill;
			base.Formation.FormOrder = FormOrder.FormOrderDeep;
			base.Formation.WeaponUsageOrder = WeaponUsageOrder.WeaponUsageOrderUseAny;
		}

		// Token: 0x17000314 RID: 788
		// (get) Token: 0x06000CCB RID: 3275 RVA: 0x0001CC8E File Offset: 0x0001AE8E
		public override float NavmeshlessTargetPositionPenalty
		{
			get
			{
				return 1f;
			}
		}

		// Token: 0x06000CCC RID: 3276 RVA: 0x0001CC95 File Offset: 0x0001AE95
		protected override float GetAiWeight()
		{
			if (this._targetWeapons.IsEmpty<SiegeWeapon>())
			{
				return 0f;
			}
			if (!this._isTargetPrimaryWeapon)
			{
				return 0.7f;
			}
			return 1f;
		}

		// Token: 0x0400030F RID: 783
		private readonly List<SiegeWeapon> _allWeapons;

		// Token: 0x04000310 RID: 784
		private List<SiegeWeapon> _targetWeapons;

		// Token: 0x04000311 RID: 785
		public SiegeWeapon LastTargetWeapon;

		// Token: 0x04000312 RID: 786
		private bool _isTargetPrimaryWeapon;
	}
}
