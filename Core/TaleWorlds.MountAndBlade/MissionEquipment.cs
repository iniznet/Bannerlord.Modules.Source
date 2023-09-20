using System;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	public class MissionEquipment
	{
		public MissionEquipment()
		{
			this._weaponSlots = new MissionWeapon[5];
			this._cache = default(MissionEquipment.MissionEquipmentCache);
			this._cache.Initialize();
		}

		public MissionEquipment(Equipment spawnEquipment, Banner banner)
			: this()
		{
			for (EquipmentIndex equipmentIndex = EquipmentIndex.WeaponItemBeginSlot; equipmentIndex < EquipmentIndex.NumAllWeaponSlots; equipmentIndex++)
			{
				this._weaponSlots[(int)equipmentIndex] = new MissionWeapon(spawnEquipment[equipmentIndex].Item, spawnEquipment[equipmentIndex].ItemModifier, banner);
			}
		}

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

		public void FillFrom(Equipment sourceEquipment, Banner banner)
		{
			for (EquipmentIndex equipmentIndex = EquipmentIndex.WeaponItemBeginSlot; equipmentIndex < EquipmentIndex.NumAllWeaponSlots; equipmentIndex++)
			{
				this[equipmentIndex] = new MissionWeapon(sourceEquipment[equipmentIndex].Item, sourceEquipment[equipmentIndex].ItemModifier, banner);
			}
		}

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

		public float GetTotalWeightOfWeapons()
		{
			if (!this._cache.IsValid(MissionEquipment.MissionEquipmentCache.CachedFloat.TotalWeightOfWeapons))
			{
				this._cache.UpdateAndMarkValid(MissionEquipment.MissionEquipmentCache.CachedFloat.TotalWeightOfWeapons, this.CalculateGetTotalWeightOfWeapons());
			}
			return this._cache.GetValue(MissionEquipment.MissionEquipmentCache.CachedFloat.TotalWeightOfWeapons);
		}

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
					if (!missionWeapon2.IsEmpty && missionWeapon2.HasAnyUsageWithWeaponClass(missionWeapon.GetWeaponComponentDataForUsage(rangedUsageIndex).AmmoClass) && this[(int)equipmentIndex2].ModifiedMaxAmount > 1 && missionWeapon2.Amount > 0)
					{
						return true;
					}
				}
			}
			return false;
		}

		public int GetAmmoAmount(EquipmentIndex weaponIndex)
		{
			if (this[weaponIndex].IsAnyConsumable() && this[weaponIndex].ModifiedMaxAmount <= 1)
			{
				return (int)this[weaponIndex].ModifiedMaxAmount;
			}
			int num = 0;
			for (EquipmentIndex equipmentIndex = EquipmentIndex.WeaponItemBeginSlot; equipmentIndex < EquipmentIndex.NumAllWeaponSlots; equipmentIndex++)
			{
				if (!this[(int)equipmentIndex].IsEmpty && this[(int)equipmentIndex].CurrentUsageItem.WeaponClass == this[weaponIndex].CurrentUsageItem.AmmoClass && this[(int)equipmentIndex].ModifiedMaxAmount > 1)
				{
					num += (int)this[(int)equipmentIndex].Amount;
				}
			}
			return num;
		}

		public int GetMaxAmmo(EquipmentIndex weaponIndex)
		{
			if (this[weaponIndex].IsAnyConsumable() && this[weaponIndex].ModifiedMaxAmount <= 1)
			{
				return (int)this[weaponIndex].ModifiedMaxAmount;
			}
			int num = 0;
			for (EquipmentIndex equipmentIndex = EquipmentIndex.WeaponItemBeginSlot; equipmentIndex < EquipmentIndex.NumAllWeaponSlots; equipmentIndex++)
			{
				if (!this[(int)equipmentIndex].IsEmpty && this[(int)equipmentIndex].CurrentUsageItem.WeaponClass == this[weaponIndex].CurrentUsageItem.AmmoClass && this[(int)equipmentIndex].ModifiedMaxAmount > 1)
				{
					num += (int)this[(int)equipmentIndex].ModifiedMaxAmount;
				}
			}
			return num;
		}

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

		public void SetUsageIndexOfSlot(EquipmentIndex slotIndex, int usageIndex)
		{
			this._weaponSlots[(int)slotIndex].CurrentUsageIndex = usageIndex;
			this._cache.InvalidateOnWeaponUsageIndexUpdated();
		}

		public void SetReloadPhaseOfSlot(EquipmentIndex slotIndex, short reloadPhase)
		{
			this._weaponSlots[(int)slotIndex].ReloadPhase = reloadPhase;
		}

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

		public void SetConsumedAmmoOfSlot(EquipmentIndex slotIndex, short count)
		{
			this._weaponSlots[(int)slotIndex].ConsumeAmmo(count);
			this._cache.InvalidateOnWeaponAmmoUpdated();
		}

		public void AttachWeaponToWeaponInSlot(EquipmentIndex slotIndex, ref MissionWeapon weapon, ref MatrixFrame attachLocalFrame)
		{
			this._weaponSlots[(int)slotIndex].AttachWeapon(weapon, ref attachLocalFrame);
		}

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

		public bool ContainsNonConsumableRangedWeaponWithAmmo()
		{
			if (!this._cache.IsValid(MissionEquipment.MissionEquipmentCache.CachedBool.ContainsNonConsumableRangedWeaponWithAmmo))
			{
				this.GatherInformationAndUpdateCache();
			}
			return this._cache.GetValue(MissionEquipment.MissionEquipmentCache.CachedBool.ContainsNonConsumableRangedWeaponWithAmmo);
		}

		public bool ContainsMeleeWeapon()
		{
			if (!this._cache.IsValid(MissionEquipment.MissionEquipmentCache.CachedBool.ContainsMeleeWeapon))
			{
				this.GatherInformationAndUpdateCache();
			}
			return this._cache.GetValue(MissionEquipment.MissionEquipmentCache.CachedBool.ContainsMeleeWeapon);
		}

		public bool ContainsShield()
		{
			if (!this._cache.IsValid(MissionEquipment.MissionEquipmentCache.CachedBool.ContainsShield))
			{
				this.GatherInformationAndUpdateCache();
			}
			return this._cache.GetValue(MissionEquipment.MissionEquipmentCache.CachedBool.ContainsShield);
		}

		public bool ContainsSpear()
		{
			if (!this._cache.IsValid(MissionEquipment.MissionEquipmentCache.CachedBool.ContainsSpear))
			{
				this.GatherInformationAndUpdateCache();
			}
			return this._cache.GetValue(MissionEquipment.MissionEquipmentCache.CachedBool.ContainsSpear);
		}

		public bool ContainsThrownWeapon()
		{
			if (!this._cache.IsValid(MissionEquipment.MissionEquipmentCache.CachedBool.ContainsThrownWeapon))
			{
				this.GatherInformationAndUpdateCache();
			}
			return this._cache.GetValue(MissionEquipment.MissionEquipmentCache.CachedBool.ContainsThrownWeapon);
		}

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
					containsNonConsumableRangedWeaponWithAmmo = containsNonConsumableRangedWeaponWithAmmo || this.GetAmmoAmount(equipmentIndex) > 0;
				}
			}
		}

		public void SetGlossMultipliersOfWeaponsRandomly(int seed)
		{
			for (EquipmentIndex equipmentIndex = EquipmentIndex.WeaponItemBeginSlot; equipmentIndex < EquipmentIndex.NumAllWeaponSlots; equipmentIndex++)
			{
				this._weaponSlots[(int)equipmentIndex].SetRandomGlossMultiplier(seed);
			}
		}

		private readonly MissionWeapon[] _weaponSlots;

		private MissionEquipment.MissionEquipmentCache _cache;

		private struct MissionEquipmentCache
		{
			public void Initialize()
			{
				this._cachedBool = default(StackArray.StackArray5Bool);
				this._validity = default(StackArray.StackArray6Bool);
			}

			public bool IsValid(MissionEquipment.MissionEquipmentCache.CachedBool queriedData)
			{
				return this._validity[(int)queriedData];
			}

			public void UpdateAndMarkValid(MissionEquipment.MissionEquipmentCache.CachedBool data, bool value)
			{
				this._cachedBool[(int)data] = value;
				this._validity[(int)data] = true;
			}

			public bool GetValue(MissionEquipment.MissionEquipmentCache.CachedBool data)
			{
				return this._cachedBool[(int)data];
			}

			public bool IsValid(MissionEquipment.MissionEquipmentCache.CachedFloat queriedData)
			{
				return this._validity[(int)(5 + queriedData)];
			}

			public void UpdateAndMarkValid(MissionEquipment.MissionEquipmentCache.CachedFloat data, float value)
			{
				this._cachedFloat = value;
				this._validity[(int)(5 + data)] = true;
			}

			public float GetValue(MissionEquipment.MissionEquipmentCache.CachedFloat data)
			{
				return this._cachedFloat;
			}

			public void InvalidateOnWeaponSlotUpdated()
			{
				this._validity[0] = false;
				this._validity[1] = false;
				this._validity[2] = false;
				this._validity[3] = false;
				this._validity[4] = false;
				this._validity[5] = false;
			}

			public void InvalidateOnWeaponUsageIndexUpdated()
			{
			}

			public void InvalidateOnWeaponAmmoUpdated()
			{
				this._validity[5] = false;
			}

			public void InvalidateOnWeaponAmmoAvailabilityChanged()
			{
				this._validity[3] = false;
			}

			public void InvalidateOnWeaponHitPointsUpdated()
			{
				this._validity[5] = false;
			}

			public void InvalidateOnWeaponDestroyed()
			{
				this._validity[1] = false;
			}

			private const int CachedBoolCount = 5;

			private const int CachedFloatCount = 1;

			private const int TotalCachedCount = 6;

			private float _cachedFloat;

			private StackArray.StackArray5Bool _cachedBool;

			private StackArray.StackArray6Bool _validity;

			public enum CachedBool
			{
				ContainsMeleeWeapon,
				ContainsShield,
				ContainsSpear,
				ContainsNonConsumableRangedWeaponWithAmmo,
				ContainsThrownWeapon,
				Count
			}

			public enum CachedFloat
			{
				TotalWeightOfWeapons,
				Count
			}
		}
	}
}
