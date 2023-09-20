using System;
using TaleWorlds.Core;

namespace TaleWorlds.MountAndBlade
{
	public class StandingPointWithWeaponRequirement : StandingPoint
	{
		public StandingPointWithWeaponRequirement()
		{
			this.AutoSheathWeapons = false;
			this._requiredWeaponClass1 = WeaponClass.Undefined;
			this._requiredWeaponClass2 = WeaponClass.Undefined;
			this._hasAlternative = base.HasAlternative();
		}

		protected internal override void OnInit()
		{
			base.OnInit();
		}

		public void InitRequiredWeaponClasses(WeaponClass requiredWeaponClass1, WeaponClass requiredWeaponClass2 = WeaponClass.Undefined)
		{
			this._requiredWeaponClass1 = requiredWeaponClass1;
			this._requiredWeaponClass2 = requiredWeaponClass2;
		}

		public void InitRequiredWeapon(ItemObject weapon)
		{
			this._requiredWeapon = weapon;
		}

		public void InitGivenWeapon(ItemObject weapon)
		{
			this._givenWeapon = weapon;
		}

		public override bool IsDisabledForAgent(Agent agent)
		{
			EquipmentIndex wieldedItemIndex = agent.GetWieldedItemIndex(Agent.HandIndex.MainHand);
			if (this._requiredWeapon != null)
			{
				if (wieldedItemIndex != EquipmentIndex.None && agent.Equipment[wieldedItemIndex].Item == this._requiredWeapon)
				{
					return base.IsDisabledForAgent(agent);
				}
			}
			else if (this._givenWeapon != null)
			{
				if (wieldedItemIndex == EquipmentIndex.None || agent.Equipment[wieldedItemIndex].Item != this._givenWeapon)
				{
					return base.IsDisabledForAgent(agent);
				}
			}
			else if ((this._requiredWeaponClass1 != WeaponClass.Undefined || this._requiredWeaponClass2 != WeaponClass.Undefined) && wieldedItemIndex != EquipmentIndex.None)
			{
				for (EquipmentIndex equipmentIndex = EquipmentIndex.WeaponItemBeginSlot; equipmentIndex < EquipmentIndex.NumAllWeaponSlots; equipmentIndex++)
				{
					if (!agent.Equipment[equipmentIndex].IsEmpty && (agent.Equipment[equipmentIndex].CurrentUsageItem.WeaponClass == this._requiredWeaponClass1 || agent.Equipment[equipmentIndex].CurrentUsageItem.WeaponClass == this._requiredWeaponClass2) && (!agent.Equipment[equipmentIndex].CurrentUsageItem.IsConsumable || agent.Equipment[equipmentIndex].Amount < agent.Equipment[equipmentIndex].ModifiedMaxAmount || equipmentIndex == EquipmentIndex.ExtraWeaponSlot))
					{
						return base.IsDisabledForAgent(agent);
					}
				}
			}
			return true;
		}

		public void SetHasAlternative(bool hasAlternative)
		{
			this._hasAlternative = hasAlternative;
		}

		public override bool HasAlternative()
		{
			return this._hasAlternative;
		}

		public void SetUsingBattleSide(BattleSideEnum side)
		{
			this.StandingPointSide = side;
		}

		private ItemObject _requiredWeapon;

		private ItemObject _givenWeapon;

		private WeaponClass _requiredWeaponClass1;

		private WeaponClass _requiredWeaponClass2;

		private bool _hasAlternative;
	}
}
