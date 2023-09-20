using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace TaleWorlds.MountAndBlade
{
	public class BehaviorDestroySiegeWeapons : BehaviorComponent
	{
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

		public override TextObject GetBehaviorString()
		{
			TextObject behaviorString = base.GetBehaviorString();
			TextObject textObject = GameTexts.FindText("str_formation_ai_side_strings", base.Formation.AI.Side.ToString());
			behaviorString.SetTextVariable("SIDE_STRING", textObject);
			behaviorString.SetTextVariable("IS_GENERAL_SIDE", "0");
			return behaviorString;
		}

		public override void OnValidBehaviorSideChanged()
		{
			base.OnValidBehaviorSideChanged();
			this.DetermineTargetWeapons();
		}

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

		protected override void OnBehaviorActivatedAux()
		{
			this.DetermineTargetWeapons();
			base.Formation.ArrangementOrder = ((base.Formation.QuerySystem.IsCavalryFormation || base.Formation.QuerySystem.IsRangedCavalryFormation) ? ArrangementOrder.ArrangementOrderSkein : ArrangementOrder.ArrangementOrderLine);
			base.Formation.FacingOrder = FacingOrder.FacingOrderLookAtEnemy;
			base.Formation.FiringOrder = FiringOrder.FiringOrderFireAtWill;
			base.Formation.FormOrder = FormOrder.FormOrderDeep;
			base.Formation.WeaponUsageOrder = WeaponUsageOrder.WeaponUsageOrderUseAny;
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

		private readonly List<SiegeWeapon> _allWeapons;

		private List<SiegeWeapon> _targetWeapons;

		public SiegeWeapon LastTargetWeapon;

		private bool _isTargetPrimaryWeapon;
	}
}
