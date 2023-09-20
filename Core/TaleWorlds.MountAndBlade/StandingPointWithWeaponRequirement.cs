using System;
using TaleWorlds.Core;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000365 RID: 869
	public class StandingPointWithWeaponRequirement : StandingPoint
	{
		// Token: 0x06002F5D RID: 12125 RVA: 0x000C113C File Offset: 0x000BF33C
		public StandingPointWithWeaponRequirement()
		{
			this.AutoSheathWeapons = false;
			this._requiredWeaponClass1 = WeaponClass.Undefined;
			this._requiredWeaponClass2 = WeaponClass.Undefined;
			this._hasAlternative = base.HasAlternative();
		}

		// Token: 0x06002F5E RID: 12126 RVA: 0x000C1165 File Offset: 0x000BF365
		protected internal override void OnInit()
		{
			base.OnInit();
		}

		// Token: 0x06002F5F RID: 12127 RVA: 0x000C116D File Offset: 0x000BF36D
		public void InitRequiredWeaponClasses(WeaponClass requiredWeaponClass1, WeaponClass requiredWeaponClass2 = WeaponClass.Undefined)
		{
			this._requiredWeaponClass1 = requiredWeaponClass1;
			this._requiredWeaponClass2 = requiredWeaponClass2;
		}

		// Token: 0x06002F60 RID: 12128 RVA: 0x000C117D File Offset: 0x000BF37D
		public void InitRequiredWeapon(ItemObject weapon)
		{
			this._requiredWeapon = weapon;
		}

		// Token: 0x06002F61 RID: 12129 RVA: 0x000C1186 File Offset: 0x000BF386
		public void InitGivenWeapon(ItemObject weapon)
		{
			this._givenWeapon = weapon;
		}

		// Token: 0x06002F62 RID: 12130 RVA: 0x000C1190 File Offset: 0x000BF390
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

		// Token: 0x06002F63 RID: 12131 RVA: 0x000C12E7 File Offset: 0x000BF4E7
		public void SetHasAlternative(bool hasAlternative)
		{
			this._hasAlternative = hasAlternative;
		}

		// Token: 0x06002F64 RID: 12132 RVA: 0x000C12F0 File Offset: 0x000BF4F0
		public override bool HasAlternative()
		{
			return this._hasAlternative;
		}

		// Token: 0x06002F65 RID: 12133 RVA: 0x000C12F8 File Offset: 0x000BF4F8
		public void SetUsingBattleSide(BattleSideEnum side)
		{
			this.StandingPointSide = side;
		}

		// Token: 0x0400136B RID: 4971
		private ItemObject _requiredWeapon;

		// Token: 0x0400136C RID: 4972
		private ItemObject _givenWeapon;

		// Token: 0x0400136D RID: 4973
		private WeaponClass _requiredWeaponClass1;

		// Token: 0x0400136E RID: 4974
		private WeaponClass _requiredWeaponClass2;

		// Token: 0x0400136F RID: 4975
		private bool _hasAlternative;
	}
}
