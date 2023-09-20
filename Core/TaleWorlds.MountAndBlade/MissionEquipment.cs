using System;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000246 RID: 582
	public class MissionEquipment
	{
		// Token: 0x06001F82 RID: 8066 RVA: 0x0006FAE3 File Offset: 0x0006DCE3
		public MissionEquipment()
		{
			this._weaponSlots = new MissionWeapon[5];
			this._cache = default(MissionEquipment.MissionEquipmentCache);
			this._cache.Initialize();
		}

		// Token: 0x06001F83 RID: 8067 RVA: 0x0006FB10 File Offset: 0x0006DD10
		public MissionEquipment(Equipment spawnEquipment, Banner banner)
			: this()
		{
			for (EquipmentIndex equipmentIndex = EquipmentIndex.WeaponItemBeginSlot; equipmentIndex < EquipmentIndex.NumAllWeaponSlots; equipmentIndex++)
			{
				this._weaponSlots[(int)equipmentIndex] = new MissionWeapon(spawnEquipment[equipmentIndex].Item, spawnEquipment[equipmentIndex].ItemModifier, banner);
			}
		}

		// Token: 0x1700062C RID: 1580
		public MissionWeapon this[int index]
		{
			get
			{
				return this._weaponSlots[index];
			}
			set
			{
				this._weaponSlots[index] = value;
				this._cache.InvalidateOnWeaponSlotUpdated();
			}
		}

		// Token: 0x1700062D RID: 1581
		public MissionWeapon this[EquipmentIndex index]
		{
			get
			{
				return this._weaponSlots[(int)index];
			}
			set
			{
				this[(int)index] = value;
			}
		}

		// Token: 0x06001F88 RID: 8072 RVA: 0x0006FBA0 File Offset: 0x0006DDA0
		public void FillFrom(Equipment sourceEquipment, Banner banner)
		{
			for (EquipmentIndex equipmentIndex = EquipmentIndex.WeaponItemBeginSlot; equipmentIndex < EquipmentIndex.NumAllWeaponSlots; equipmentIndex++)
			{
				this[equipmentIndex] = new MissionWeapon(sourceEquipment[equipmentIndex].Item, sourceEquipment[equipmentIndex].ItemModifier, banner);
			}
		}

		// Token: 0x06001F89 RID: 8073 RVA: 0x0006FBE4 File Offset: 0x0006DDE4
		private float CalculateGetTotalWeightOfWeapons()
		{
			float num = 0f;
			for (EquipmentIndex equipmentIndex = EquipmentIndex.WeaponItemBeginSlot; equipmentIndex < EquipmentIndex.NumAllWeaponSlots; equipmentIndex++)
			{
				MissionWeapon missionWeapon = this[equipmentIndex];
				if (!missionWeapon.IsEmpty)
				{
					if (missionWeapon.CurrentUsageItem.IsShield)
					{
						if (missionWeapon.HitPoints > 0)
						{
							num += missionWeapon.GetWeight();
						}
					}
					else
					{
						num += missionWeapon.GetWeight();
					}
				}
			}
			return num;
		}

		// Token: 0x06001F8A RID: 8074 RVA: 0x0006FC43 File Offset: 0x0006DE43
		public float GetTotalWeightOfWeapons()
		{
			if (!this._cache.IsValid(MissionEquipment.MissionEquipmentCache.CachedFloat.TotalWeightOfWeapons))
			{
				this._cache.UpdateAndMarkValid(MissionEquipment.MissionEquipmentCache.CachedFloat.TotalWeightOfWeapons, this.CalculateGetTotalWeightOfWeapons());
			}
			return this._cache.GetValue(MissionEquipment.MissionEquipmentCache.CachedFloat.TotalWeightOfWeapons);
		}

		// Token: 0x06001F8B RID: 8075 RVA: 0x0006FC74 File Offset: 0x0006DE74
		public static EquipmentIndex SelectWeaponPickUpSlot(Agent agentPickingUp, MissionWeapon weaponBeingPickedUp, bool isStuckMissile)
		{
			EquipmentIndex equipmentIndex = EquipmentIndex.None;
			if (weaponBeingPickedUp.Item.ItemFlags.HasAnyFlag(ItemFlags.DropOnWeaponChange | ItemFlags.DropOnAnyAction))
			{
				equipmentIndex = EquipmentIndex.ExtraWeaponSlot;
			}
			else
			{
				Agent.HandIndex handIndex = (weaponBeingPickedUp.Item.ItemFlags.HasAnyFlag(ItemFlags.HeldInOffHand) ? Agent.HandIndex.OffHand : Agent.HandIndex.MainHand);
				EquipmentIndex wieldedItemIndex = agentPickingUp.GetWieldedItemIndex(handIndex);
				MissionWeapon missionWeapon = ((wieldedItemIndex != EquipmentIndex.None) ? agentPickingUp.Equipment[wieldedItemIndex] : MissionWeapon.Invalid);
				if (isStuckMissile)
				{
					bool flag = false;
					bool flag2 = false;
					bool isConsumable = weaponBeingPickedUp.Item.PrimaryWeapon.IsConsumable;
					if (isConsumable)
					{
						flag = !missionWeapon.IsEmpty && missionWeapon.IsEqualTo(weaponBeingPickedUp) && missionWeapon.HasEnoughSpaceForAmount((int)weaponBeingPickedUp.Amount);
						flag2 = !missionWeapon.IsEmpty && missionWeapon.IsSameType(weaponBeingPickedUp) && missionWeapon.HasEnoughSpaceForAmount((int)weaponBeingPickedUp.Amount);
					}
					EquipmentIndex equipmentIndex2 = EquipmentIndex.None;
					EquipmentIndex equipmentIndex3 = EquipmentIndex.None;
					EquipmentIndex equipmentIndex4 = EquipmentIndex.None;
					EquipmentIndex equipmentIndex5 = EquipmentIndex.WeaponItemBeginSlot;
					while (equipmentIndex5 < EquipmentIndex.ExtraWeaponSlot)
					{
						if (!isConsumable)
						{
							goto IL_19B;
						}
						if (equipmentIndex3 != EquipmentIndex.None && !agentPickingUp.Equipment[equipmentIndex5].IsEmpty && agentPickingUp.Equipment[equipmentIndex5].IsEqualTo(weaponBeingPickedUp) && agentPickingUp.Equipment[equipmentIndex5].HasEnoughSpaceForAmount((int)weaponBeingPickedUp.Amount))
						{
							equipmentIndex3 = equipmentIndex5;
						}
						else
						{
							if (equipmentIndex4 != EquipmentIndex.None || agentPickingUp.Equipment[equipmentIndex5].IsEmpty || !agentPickingUp.Equipment[equipmentIndex5].IsSameType(weaponBeingPickedUp) || !agentPickingUp.Equipment[equipmentIndex5].HasEnoughSpaceForAmount((int)weaponBeingPickedUp.Amount))
							{
								goto IL_19B;
							}
							equipmentIndex4 = equipmentIndex5;
						}
						IL_1BC:
						equipmentIndex5++;
						continue;
						IL_19B:
						if (equipmentIndex2 == EquipmentIndex.None && agentPickingUp.Equipment[equipmentIndex5].IsEmpty)
						{
							equipmentIndex2 = equipmentIndex5;
							goto IL_1BC;
						}
						goto IL_1BC;
					}
					if (flag)
					{
						equipmentIndex = wieldedItemIndex;
					}
					else if (equipmentIndex3 != EquipmentIndex.None)
					{
						equipmentIndex = equipmentIndex4;
					}
					else if (flag2)
					{
						equipmentIndex = wieldedItemIndex;
					}
					else if (equipmentIndex4 != EquipmentIndex.None)
					{
						equipmentIndex = equipmentIndex4;
					}
					else if (equipmentIndex2 != EquipmentIndex.None)
					{
						equipmentIndex = equipmentIndex2;
					}
				}
				else
				{
					bool isConsumable2 = weaponBeingPickedUp.Item.PrimaryWeapon.IsConsumable;
					if (isConsumable2 && weaponBeingPickedUp.Amount == 0)
					{
						equipmentIndex = EquipmentIndex.None;
					}
					else
					{
						if (handIndex == Agent.HandIndex.OffHand && wieldedItemIndex != EquipmentIndex.None)
						{
							for (int i = 0; i < 4; i++)
							{
								if (i != (int)wieldedItemIndex && !agentPickingUp.Equipment[i].IsEmpty && agentPickingUp.Equipment[i].Item.ItemFlags.HasAnyFlag(ItemFlags.HeldInOffHand))
								{
									equipmentIndex = wieldedItemIndex;
									break;
								}
							}
						}
						if (equipmentIndex == EquipmentIndex.None && isConsumable2)
						{
							for (EquipmentIndex equipmentIndex6 = EquipmentIndex.WeaponItemBeginSlot; equipmentIndex6 < EquipmentIndex.ExtraWeaponSlot; equipmentIndex6++)
							{
								if (!agentPickingUp.Equipment[equipmentIndex6].IsEmpty && agentPickingUp.Equipment[equipmentIndex6].IsSameType(weaponBeingPickedUp) && agentPickingUp.Equipment[equipmentIndex6].Amount < agentPickingUp.Equipment[equipmentIndex6].ModifiedMaxAmount)
								{
									equipmentIndex = equipmentIndex6;
									break;
								}
							}
						}
						if (equipmentIndex == EquipmentIndex.None)
						{
							for (EquipmentIndex equipmentIndex7 = EquipmentIndex.WeaponItemBeginSlot; equipmentIndex7 < EquipmentIndex.ExtraWeaponSlot; equipmentIndex7++)
							{
								if (agentPickingUp.Equipment[equipmentIndex7].IsEmpty)
								{
									equipmentIndex = equipmentIndex7;
									break;
								}
							}
						}
						if (equipmentIndex == EquipmentIndex.None)
						{
							for (EquipmentIndex equipmentIndex8 = EquipmentIndex.WeaponItemBeginSlot; equipmentIndex8 < EquipmentIndex.ExtraWeaponSlot; equipmentIndex8++)
							{
								if (!agentPickingUp.Equipment[equipmentIndex8].IsEmpty && agentPickingUp.Equipment[equipmentIndex8].IsAnyConsumable() && agentPickingUp.Equipment[equipmentIndex8].Amount == 0)
								{
									equipmentIndex = equipmentIndex8;
									break;
								}
							}
						}
						if (equipmentIndex == EquipmentIndex.None && !missionWeapon.IsEmpty)
						{
							equipmentIndex = wieldedItemIndex;
						}
						if (equipmentIndex == EquipmentIndex.None)
						{
							equipmentIndex = EquipmentIndex.WeaponItemBeginSlot;
						}
					}
				}
			}
			return equipmentIndex;
		}

		// Token: 0x06001F8C RID: 8076 RVA: 0x00070038 File Offset: 0x0006E238
		public bool HasAmmo(EquipmentIndex equipmentIndex, out int rangedUsageIndex, out bool hasLoadedAmmo, out bool noAmmoInThisSlot)
		{
			hasLoadedAmmo = false;
			noAmmoInThisSlot = false;
			MissionWeapon missionWeapon = this._weaponSlots[(int)equipmentIndex];
			rangedUsageIndex = missionWeapon.GetRangedUsageIndex();
			if (rangedUsageIndex >= 0)
			{
				if (missionWeapon.Ammo > 0)
				{
					hasLoadedAmmo = true;
					return true;
				}
				noAmmoInThisSlot = missionWeapon.IsAnyConsumable() && missionWeapon.Amount == 0;
				for (EquipmentIndex equipmentIndex2 = EquipmentIndex.WeaponItemBeginSlot; equipmentIndex2 < EquipmentIndex.NumAllWeaponSlots; equipmentIndex2++)
				{
					MissionWeapon missionWeapon2 = this[(int)equipmentIndex2];
					if (!missionWeapon2.IsEmpty && missionWeapon2.HasAnyUsageWithWeaponClass(missionWeapon.GetWeaponComponentDataForUsage(rangedUsageIndex).AmmoClass) && missionWeapon2.Amount > 0)
					{
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x06001F8D RID: 8077 RVA: 0x000700D0 File Offset: 0x0006E2D0
		public int GetAmmoAmount(WeaponClass ammoClass)
		{
			int num = 0;
			for (EquipmentIndex equipmentIndex = EquipmentIndex.WeaponItemBeginSlot; equipmentIndex < EquipmentIndex.NumAllWeaponSlots; equipmentIndex++)
			{
				if (!this[(int)equipmentIndex].IsEmpty && this[(int)equipmentIndex].CurrentUsageItem.WeaponClass == ammoClass)
				{
					num += (int)this[(int)equipmentIndex].Amount;
				}
			}
			return num;
		}

		// Token: 0x06001F8E RID: 8078 RVA: 0x00070128 File Offset: 0x0006E328
		public int GetAmmoSlotIndexOfWeapon(WeaponClass ammoClass)
		{
			for (EquipmentIndex equipmentIndex = EquipmentIndex.WeaponItemBeginSlot; equipmentIndex < EquipmentIndex.NumAllWeaponSlots; equipmentIndex++)
			{
				if (!this[(int)equipmentIndex].IsEmpty && this[(int)equipmentIndex].CurrentUsageItem.WeaponClass == ammoClass)
				{
					return (int)equipmentIndex;
				}
			}
			return -1;
		}

		// Token: 0x06001F8F RID: 8079 RVA: 0x0007016C File Offset: 0x0006E36C
		public int GetMaxAmmo(WeaponClass ammoClass)
		{
			int num = 0;
			for (EquipmentIndex equipmentIndex = EquipmentIndex.WeaponItemBeginSlot; equipmentIndex < EquipmentIndex.NumAllWeaponSlots; equipmentIndex++)
			{
				if (!this[(int)equipmentIndex].IsEmpty && this[(int)equipmentIndex].CurrentUsageItem.WeaponClass == ammoClass)
				{
					num += (int)this[(int)equipmentIndex].ModifiedMaxAmount;
				}
			}
			return num;
		}

		// Token: 0x06001F90 RID: 8080 RVA: 0x000701C4 File Offset: 0x0006E3C4
		public void GetAmmoCountAndIndexOfType(ItemObject.ItemTypeEnum itemType, out int ammoCount, out EquipmentIndex eIndex, EquipmentIndex equippedIndex = EquipmentIndex.None)
		{
			ItemObject.ItemTypeEnum ammoTypeForItemType = ItemObject.GetAmmoTypeForItemType(itemType);
			ItemObject itemObject;
			if (equippedIndex != EquipmentIndex.None)
			{
				itemObject = this[equippedIndex].Item;
				ammoCount = 0;
			}
			else
			{
				itemObject = null;
				ammoCount = -1;
			}
			eIndex = equippedIndex;
			if (ammoTypeForItemType != ItemObject.ItemTypeEnum.Invalid)
			{
				for (EquipmentIndex equipmentIndex = EquipmentIndex.Weapon3; equipmentIndex >= EquipmentIndex.WeaponItemBeginSlot; equipmentIndex--)
				{
					if (!this[equipmentIndex].IsEmpty && this[equipmentIndex].Item.Type == ammoTypeForItemType)
					{
						int amount = (int)this[equipmentIndex].Amount;
						if (amount > 0)
						{
							if (itemObject == null)
							{
								eIndex = equipmentIndex;
								itemObject = this[equipmentIndex].Item;
								ammoCount = amount;
							}
							else if (itemObject.Id == this[equipmentIndex].Item.Id)
							{
								ammoCount += amount;
							}
						}
					}
				}
			}
		}

		// Token: 0x06001F91 RID: 8081 RVA: 0x00070298 File Offset: 0x0006E498
		public static bool DoesWeaponFitToSlot(EquipmentIndex slotIndex, MissionWeapon weapon)
		{
			bool flag;
			if (weapon.IsEmpty)
			{
				flag = true;
			}
			else if (weapon.Item.ItemFlags.HasAnyFlag(ItemFlags.DropOnWeaponChange | ItemFlags.DropOnAnyAction))
			{
				flag = slotIndex == EquipmentIndex.ExtraWeaponSlot;
			}
			else
			{
				flag = slotIndex >= EquipmentIndex.WeaponItemBeginSlot && slotIndex < EquipmentIndex.ExtraWeaponSlot;
			}
			return flag;
		}

		// Token: 0x06001F92 RID: 8082 RVA: 0x000702E0 File Offset: 0x0006E4E0
		public void CheckLoadedAmmos()
		{
			for (EquipmentIndex equipmentIndex = EquipmentIndex.WeaponItemBeginSlot; equipmentIndex < EquipmentIndex.NumAllWeaponSlots; equipmentIndex++)
			{
				if (!this[equipmentIndex].IsEmpty && this[equipmentIndex].Item.PrimaryWeapon.WeaponClass == WeaponClass.Crossbow)
				{
					int num;
					EquipmentIndex equipmentIndex2;
					this.GetAmmoCountAndIndexOfType(this[equipmentIndex].Item.Type, out num, out equipmentIndex2, EquipmentIndex.None);
					if (equipmentIndex2 != EquipmentIndex.None)
					{
						MissionWeapon missionWeapon = this._weaponSlots[(int)equipmentIndex2].Consume(MathF.Min(this[equipmentIndex].MaxAmmo, this._weaponSlots[(int)equipmentIndex2].Amount));
						this._weaponSlots[(int)equipmentIndex].ReloadAmmo(missionWeapon, this._weaponSlots[(int)equipmentIndex].ReloadPhaseCount);
					}
				}
			}
			this._cache.InvalidateOnWeaponAmmoUpdated();
		}

		// Token: 0x06001F93 RID: 8083 RVA: 0x000703BA File Offset: 0x0006E5BA
		public void SetUsageIndexOfSlot(EquipmentIndex slotIndex, int usageIndex)
		{
			this._weaponSlots[(int)slotIndex].CurrentUsageIndex = usageIndex;
			this._cache.InvalidateOnWeaponUsageIndexUpdated();
		}

		// Token: 0x06001F94 RID: 8084 RVA: 0x000703D9 File Offset: 0x0006E5D9
		public void SetReloadPhaseOfSlot(EquipmentIndex slotIndex, short reloadPhase)
		{
			this._weaponSlots[(int)slotIndex].ReloadPhase = reloadPhase;
		}

		// Token: 0x06001F95 RID: 8085 RVA: 0x000703F0 File Offset: 0x0006E5F0
		public void SetAmountOfSlot(EquipmentIndex slotIndex, short dataValue, bool addOverflowToMaxAmount = false)
		{
			if (addOverflowToMaxAmount)
			{
				short num = dataValue - this._weaponSlots[(int)slotIndex].Amount;
				if (num > 0)
				{
					this._weaponSlots[(int)slotIndex].AddExtraModifiedMaxValue(num);
				}
			}
			short amount = this._weaponSlots[(int)slotIndex].Amount;
			this._weaponSlots[(int)slotIndex].Amount = dataValue;
			this._cache.InvalidateOnWeaponAmmoUpdated();
			if ((amount != 0 && dataValue == 0) || (amount == 0 && dataValue != 0))
			{
				this._cache.InvalidateOnWeaponAmmoAvailabilityChanged();
			}
		}

		// Token: 0x06001F96 RID: 8086 RVA: 0x00070474 File Offset: 0x0006E674
		public void SetHitPointsOfSlot(EquipmentIndex slotIndex, short dataValue, bool addOverflowToMaxHitPoints = false)
		{
			if (addOverflowToMaxHitPoints)
			{
				short num = dataValue - this._weaponSlots[(int)slotIndex].HitPoints;
				if (num > 0)
				{
					this._weaponSlots[(int)slotIndex].AddExtraModifiedMaxValue(num);
				}
			}
			this._weaponSlots[(int)slotIndex].HitPoints = dataValue;
			this._cache.InvalidateOnWeaponHitPointsUpdated();
			if (dataValue == 0)
			{
				this._cache.InvalidateOnWeaponDestroyed();
			}
		}

		// Token: 0x06001F97 RID: 8087 RVA: 0x000704DC File Offset: 0x0006E6DC
		public void SetReloadedAmmoOfSlot(EquipmentIndex slotIndex, EquipmentIndex ammoSlotIndex, short totalAmmo)
		{
			if (ammoSlotIndex == EquipmentIndex.None)
			{
				this._weaponSlots[(int)slotIndex].SetAmmo(MissionWeapon.Invalid);
			}
			else
			{
				MissionWeapon missionWeapon = this._weaponSlots[(int)ammoSlotIndex];
				missionWeapon.Amount = totalAmmo;
				this._weaponSlots[(int)slotIndex].SetAmmo(missionWeapon);
			}
			this._cache.InvalidateOnWeaponAmmoUpdated();
		}

		// Token: 0x06001F98 RID: 8088 RVA: 0x00070537 File Offset: 0x0006E737
		public void SetConsumedAmmoOfSlot(EquipmentIndex slotIndex, short count)
		{
			this._weaponSlots[(int)slotIndex].ConsumeAmmo(count);
			this._cache.InvalidateOnWeaponAmmoUpdated();
		}

		// Token: 0x06001F99 RID: 8089 RVA: 0x00070556 File Offset: 0x0006E756
		public void AttachWeaponToWeaponInSlot(EquipmentIndex slotIndex, ref MissionWeapon weapon, ref MatrixFrame attachLocalFrame)
		{
			this._weaponSlots[(int)slotIndex].AttachWeapon(weapon, ref attachLocalFrame);
		}

		// Token: 0x06001F9A RID: 8090 RVA: 0x00070570 File Offset: 0x0006E770
		public bool HasShield()
		{
			for (EquipmentIndex equipmentIndex = EquipmentIndex.WeaponItemBeginSlot; equipmentIndex < EquipmentIndex.NumAllWeaponSlots; equipmentIndex++)
			{
				WeaponComponentData currentUsageItem = this._weaponSlots[(int)equipmentIndex].CurrentUsageItem;
				if (currentUsageItem != null && currentUsageItem.IsShield)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06001F9B RID: 8091 RVA: 0x000705AC File Offset: 0x0006E7AC
		public bool HasAnyWeapon()
		{
			for (EquipmentIndex equipmentIndex = EquipmentIndex.WeaponItemBeginSlot; equipmentIndex < EquipmentIndex.NumAllWeaponSlots; equipmentIndex++)
			{
				if (this._weaponSlots[(int)equipmentIndex].CurrentUsageItem != null)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06001F9C RID: 8092 RVA: 0x000705DC File Offset: 0x0006E7DC
		public bool HasAnyWeaponWithFlags(WeaponFlags flags)
		{
			for (EquipmentIndex equipmentIndex = EquipmentIndex.WeaponItemBeginSlot; equipmentIndex < EquipmentIndex.NumAllWeaponSlots; equipmentIndex++)
			{
				WeaponComponentData currentUsageItem = this._weaponSlots[(int)equipmentIndex].CurrentUsageItem;
				if (currentUsageItem != null && currentUsageItem.WeaponFlags.HasAllFlags(flags))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06001F9D RID: 8093 RVA: 0x0007061C File Offset: 0x0006E81C
		public ItemObject GetBanner()
		{
			ItemObject itemObject = null;
			MissionWeapon missionWeapon = this._weaponSlots[4];
			ItemObject item = missionWeapon.Item;
			if (item != null && item.IsBannerItem && item.BannerComponent != null)
			{
				itemObject = item;
			}
			return itemObject;
		}

		// Token: 0x06001F9E RID: 8094 RVA: 0x00070658 File Offset: 0x0006E858
		public bool HasRangedWeapon(WeaponClass requiredAmmoClass = WeaponClass.Undefined)
		{
			for (EquipmentIndex equipmentIndex = EquipmentIndex.WeaponItemBeginSlot; equipmentIndex < EquipmentIndex.NumAllWeaponSlots; equipmentIndex++)
			{
				WeaponComponentData currentUsageItem = this._weaponSlots[(int)equipmentIndex].CurrentUsageItem;
				if (currentUsageItem != null && currentUsageItem.IsRangedWeapon && (requiredAmmoClass == WeaponClass.Undefined || currentUsageItem.AmmoClass == requiredAmmoClass))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06001F9F RID: 8095 RVA: 0x0007069D File Offset: 0x0006E89D
		public bool ContainsNonConsumableRangedWeaponWithAmmo()
		{
			if (!this._cache.IsValid(MissionEquipment.MissionEquipmentCache.CachedBool.ContainsNonConsumableRangedWeaponWithAmmo))
			{
				this.GatherInformationAndUpdateCache();
			}
			return this._cache.GetValue(MissionEquipment.MissionEquipmentCache.CachedBool.ContainsNonConsumableRangedWeaponWithAmmo);
		}

		// Token: 0x06001FA0 RID: 8096 RVA: 0x000706BF File Offset: 0x0006E8BF
		public bool ContainsMeleeWeapon()
		{
			if (!this._cache.IsValid(MissionEquipment.MissionEquipmentCache.CachedBool.ContainsMeleeWeapon))
			{
				this.GatherInformationAndUpdateCache();
			}
			return this._cache.GetValue(MissionEquipment.MissionEquipmentCache.CachedBool.ContainsMeleeWeapon);
		}

		// Token: 0x06001FA1 RID: 8097 RVA: 0x000706E1 File Offset: 0x0006E8E1
		public bool ContainsShield()
		{
			if (!this._cache.IsValid(MissionEquipment.MissionEquipmentCache.CachedBool.ContainsShield))
			{
				this.GatherInformationAndUpdateCache();
			}
			return this._cache.GetValue(MissionEquipment.MissionEquipmentCache.CachedBool.ContainsShield);
		}

		// Token: 0x06001FA2 RID: 8098 RVA: 0x00070703 File Offset: 0x0006E903
		public bool ContainsSpear()
		{
			if (!this._cache.IsValid(MissionEquipment.MissionEquipmentCache.CachedBool.ContainsSpear))
			{
				this.GatherInformationAndUpdateCache();
			}
			return this._cache.GetValue(MissionEquipment.MissionEquipmentCache.CachedBool.ContainsSpear);
		}

		// Token: 0x06001FA3 RID: 8099 RVA: 0x00070725 File Offset: 0x0006E925
		public bool ContainsThrownWeapon()
		{
			if (!this._cache.IsValid(MissionEquipment.MissionEquipmentCache.CachedBool.ContainsThrownWeapon))
			{
				this.GatherInformationAndUpdateCache();
			}
			return this._cache.GetValue(MissionEquipment.MissionEquipmentCache.CachedBool.ContainsThrownWeapon);
		}

		// Token: 0x06001FA4 RID: 8100 RVA: 0x00070748 File Offset: 0x0006E948
		private void GatherInformationAndUpdateCache()
		{
			bool flag;
			bool flag2;
			bool flag3;
			bool flag4;
			bool flag5;
			this.GatherInformation(out flag, out flag2, out flag3, out flag4, out flag5);
			this._cache.UpdateAndMarkValid(MissionEquipment.MissionEquipmentCache.CachedBool.ContainsMeleeWeapon, flag);
			this._cache.UpdateAndMarkValid(MissionEquipment.MissionEquipmentCache.CachedBool.ContainsShield, flag2);
			this._cache.UpdateAndMarkValid(MissionEquipment.MissionEquipmentCache.CachedBool.ContainsSpear, flag3);
			this._cache.UpdateAndMarkValid(MissionEquipment.MissionEquipmentCache.CachedBool.ContainsNonConsumableRangedWeaponWithAmmo, flag4);
			this._cache.UpdateAndMarkValid(MissionEquipment.MissionEquipmentCache.CachedBool.ContainsThrownWeapon, flag5);
		}

		// Token: 0x06001FA5 RID: 8101 RVA: 0x000707A8 File Offset: 0x0006E9A8
		private void GatherInformation(out bool containsMeleeWeapon, out bool containsShield, out bool containsSpear, out bool containsNonConsumableRangedWeaponWithAmmo, out bool containsThrownWeapon)
		{
			containsMeleeWeapon = false;
			containsShield = false;
			containsSpear = false;
			containsNonConsumableRangedWeaponWithAmmo = false;
			containsThrownWeapon = false;
			for (EquipmentIndex equipmentIndex = EquipmentIndex.WeaponItemBeginSlot; equipmentIndex < EquipmentIndex.NumAllWeaponSlots; equipmentIndex++)
			{
				bool flag;
				bool flag2;
				bool flag3;
				bool flag4;
				bool flag5;
				WeaponClass weaponClass;
				this._weaponSlots[(int)equipmentIndex].GatherInformationFromWeapon(out flag, out flag2, out flag3, out flag4, out flag5, out weaponClass);
				containsMeleeWeapon = containsMeleeWeapon || flag;
				containsShield = containsShield || flag2;
				containsSpear = containsSpear || flag3;
				containsThrownWeapon = containsThrownWeapon || flag5;
				if (flag4)
				{
					containsNonConsumableRangedWeaponWithAmmo = containsNonConsumableRangedWeaponWithAmmo || this.GetAmmoAmount(weaponClass) > 0;
				}
			}
		}

		// Token: 0x06001FA6 RID: 8102 RVA: 0x00070824 File Offset: 0x0006EA24
		public void SetGlossMultipliersOfWeaponsRandomly(int seed)
		{
			for (EquipmentIndex equipmentIndex = EquipmentIndex.WeaponItemBeginSlot; equipmentIndex < EquipmentIndex.NumAllWeaponSlots; equipmentIndex++)
			{
				this._weaponSlots[(int)equipmentIndex].SetRandomGlossMultiplier(seed);
			}
		}

		// Token: 0x04000BB6 RID: 2998
		private readonly MissionWeapon[] _weaponSlots;

		// Token: 0x04000BB7 RID: 2999
		private MissionEquipment.MissionEquipmentCache _cache;

		// Token: 0x02000558 RID: 1368
		private struct MissionEquipmentCache
		{
			// Token: 0x06003A30 RID: 14896 RVA: 0x000EB185 File Offset: 0x000E9385
			public void Initialize()
			{
				this._cachedBool = default(StackArray.StackArray5Bool);
				this._validity = default(StackArray.StackArray6Bool);
			}

			// Token: 0x06003A31 RID: 14897 RVA: 0x000EB19F File Offset: 0x000E939F
			public bool IsValid(MissionEquipment.MissionEquipmentCache.CachedBool queriedData)
			{
				return this._validity[(int)queriedData];
			}

			// Token: 0x06003A32 RID: 14898 RVA: 0x000EB1B0 File Offset: 0x000E93B0
			public void UpdateAndMarkValid(MissionEquipment.MissionEquipmentCache.CachedBool data, bool value)
			{
				this._cachedBool[(int)data] = value;
				this._validity[(int)data] = true;
			}

			// Token: 0x06003A33 RID: 14899 RVA: 0x000EB1D9 File Offset: 0x000E93D9
			public bool GetValue(MissionEquipment.MissionEquipmentCache.CachedBool data)
			{
				return this._cachedBool[(int)data];
			}

			// Token: 0x06003A34 RID: 14900 RVA: 0x000EB1E7 File Offset: 0x000E93E7
			public bool IsValid(MissionEquipment.MissionEquipmentCache.CachedFloat queriedData)
			{
				return this._validity[(int)(5 + queriedData)];
			}

			// Token: 0x06003A35 RID: 14901 RVA: 0x000EB1F8 File Offset: 0x000E93F8
			public void UpdateAndMarkValid(MissionEquipment.MissionEquipmentCache.CachedFloat data, float value)
			{
				this._cachedFloat = value;
				this._validity[(int)(5 + data)] = true;
			}

			// Token: 0x06003A36 RID: 14902 RVA: 0x000EB21D File Offset: 0x000E941D
			public float GetValue(MissionEquipment.MissionEquipmentCache.CachedFloat data)
			{
				return this._cachedFloat;
			}

			// Token: 0x06003A37 RID: 14903 RVA: 0x000EB228 File Offset: 0x000E9428
			public void InvalidateOnWeaponSlotUpdated()
			{
				this._validity[0] = false;
				this._validity[1] = false;
				this._validity[2] = false;
				this._validity[3] = false;
				this._validity[4] = false;
				this._validity[5] = false;
			}

			// Token: 0x06003A38 RID: 14904 RVA: 0x000EB283 File Offset: 0x000E9483
			public void InvalidateOnWeaponUsageIndexUpdated()
			{
			}

			// Token: 0x06003A39 RID: 14905 RVA: 0x000EB285 File Offset: 0x000E9485
			public void InvalidateOnWeaponAmmoUpdated()
			{
				this._validity[5] = false;
			}

			// Token: 0x06003A3A RID: 14906 RVA: 0x000EB294 File Offset: 0x000E9494
			public void InvalidateOnWeaponAmmoAvailabilityChanged()
			{
				this._validity[3] = false;
			}

			// Token: 0x06003A3B RID: 14907 RVA: 0x000EB2A3 File Offset: 0x000E94A3
			public void InvalidateOnWeaponHitPointsUpdated()
			{
				this._validity[5] = false;
			}

			// Token: 0x06003A3C RID: 14908 RVA: 0x000EB2B2 File Offset: 0x000E94B2
			public void InvalidateOnWeaponDestroyed()
			{
				this._validity[1] = false;
			}

			// Token: 0x04001CB8 RID: 7352
			private const int CachedBoolCount = 5;

			// Token: 0x04001CB9 RID: 7353
			private const int CachedFloatCount = 1;

			// Token: 0x04001CBA RID: 7354
			private const int TotalCachedCount = 6;

			// Token: 0x04001CBB RID: 7355
			private float _cachedFloat;

			// Token: 0x04001CBC RID: 7356
			private StackArray.StackArray5Bool _cachedBool;

			// Token: 0x04001CBD RID: 7357
			private StackArray.StackArray6Bool _validity;

			// Token: 0x020006F3 RID: 1779
			public enum CachedBool
			{
				// Token: 0x0400231A RID: 8986
				ContainsMeleeWeapon,
				// Token: 0x0400231B RID: 8987
				ContainsShield,
				// Token: 0x0400231C RID: 8988
				ContainsSpear,
				// Token: 0x0400231D RID: 8989
				ContainsNonConsumableRangedWeaponWithAmmo,
				// Token: 0x0400231E RID: 8990
				ContainsThrownWeapon,
				// Token: 0x0400231F RID: 8991
				Count
			}

			// Token: 0x020006F4 RID: 1780
			public enum CachedFloat
			{
				// Token: 0x04002321 RID: 8993
				TotalWeightOfWeapons,
				// Token: 0x04002322 RID: 8994
				Count
			}
		}
	}
}
