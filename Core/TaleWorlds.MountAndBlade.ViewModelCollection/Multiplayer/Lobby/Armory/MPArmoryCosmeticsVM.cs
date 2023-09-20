using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Core.ViewModelCollection.Selector;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.Diamond;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.Lobby.Armory
{
	// Token: 0x020000A6 RID: 166
	public class MPArmoryCosmeticsVM : ViewModel
	{
		// Token: 0x06000FE3 RID: 4067 RVA: 0x000345F4 File Offset: 0x000327F4
		public MPArmoryCosmeticsVM(Action<EquipmentIndex, EquipmentElement> onHeroPreviewItemEquipped, Action resetHeroEquipment, Action<MPArmoryCosmeticItemVM> onItemObtainRequested, Func<List<IReadOnlyPerkObject>> getSelectedPerks)
		{
			this._onHeroPreviewItemEquipped = onHeroPreviewItemEquipped;
			this._resetHeroEquipment = resetHeroEquipment;
			this._onItemObtainRequested = onItemObtainRequested;
			this._getSelectedPerks = getSelectedPerks;
			this.InitializeAllCosmetics();
			this.InitializeCosmeticItemComparers();
			this.AvailableCosmetics = new MBBindingList<MPArmoryCosmeticItemVM>();
			this.SortCategories = new SelectorVM<SelectorItemVM>(0, new Action<SelectorVM<SelectorItemVM>>(this.OnSortCategoryUpdated));
			this.SortOrders = new SelectorVM<SelectorItemVM>(0, new Action<SelectorVM<SelectorItemVM>>(this.OnSortOrderUpdated));
			this._defaultCosmeticIDs = new List<string>();
			this.IsLoading = true;
			this.SortCategories.AddItem(new SelectorItemVM(new TextObject("{=J2wEawTl}Category", null)));
			this.SortCategories.AddItem(new SelectorItemVM(new TextObject("{=ebUrBmHK}Price", null)));
			this.SortCategories.AddItem(new SelectorItemVM(new TextObject("{=bD8nTS86}Rarity", null)));
			this.SortCategories.AddItem(new SelectorItemVM(new TextObject("{=PDdh1sBj}Name", null)));
			this.SortCategories.SelectedIndex = 0;
			this.SortOrders.AddItem(new SelectorItemVM(new TextObject("{=mOmFzU78}Ascending", null)));
			this.SortOrders.AddItem(new SelectorItemVM(new TextObject("{=FgFUsncP}Descending", null)));
			this.SortOrders.SelectedIndex = 0;
			this.RefreshValues();
		}

		// Token: 0x06000FE4 RID: 4068 RVA: 0x00034740 File Offset: 0x00032940
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.SortCategories.RefreshValues();
			this.SortOrders.RefreshValues();
			this.CosmeticInfoErrorText = new TextObject("{=ehkVpzpa}Unable to get cosmetic information", null).ToString();
			this.AllCategoriesHint = new HintViewModel(new TextObject("{=yfa7tpbK}All", null), null);
			this.BodyCategoryHint = new HintViewModel(GameTexts.FindText("str_inventory_type_13", null), null);
			this.HeadCategoryHint = new HintViewModel(GameTexts.FindText("str_inventory_type_12", null), null);
			this.ShoulderCategoryHint = new HintViewModel(GameTexts.FindText("str_inventory_type_22", null), null);
			this.HandCategoryHint = new HintViewModel(GameTexts.FindText("str_inventory_type_15", null), null);
			this.LegCategoryHint = new HintViewModel(GameTexts.FindText("str_inventory_type_14", null), null);
			this.ResetPreviewHint = new HintViewModel(new TextObject("{=imUnCFgZ}Reset preview", null), null);
			this._allCosmetics.ForEach(delegate(MPArmoryCosmeticItemVM c)
			{
				c.RefreshValues();
			});
			this.AvailableCosmetics.ApplyActionOnAllItems(delegate(MPArmoryCosmeticItemVM c)
			{
				c.RefreshValues();
			});
		}

		// Token: 0x06000FE5 RID: 4069 RVA: 0x00034874 File Offset: 0x00032A74
		private void InitializeAllCosmetics()
		{
			this._allCosmetics = new List<MPArmoryCosmeticItemVM>();
			MBReadOnlyList<CosmeticsManager.CosmeticElement> getCosmeticElementList = CosmeticsManager.GetCosmeticElementList;
			for (int i = 0; i < getCosmeticElementList.Count; i++)
			{
				if (getCosmeticElementList[i].Type == CosmeticsManager.CosmeticType.Clothing)
				{
					MPArmoryCosmeticItemVM mparmoryCosmeticItemVM = new MPArmoryCosmeticItemVM(getCosmeticElementList[i], getCosmeticElementList[i].Id, new Action<MPArmoryCosmeticItemVM>(this.OnItemEquipRequested), new Action<MPArmoryCosmeticItemVM>(this.OnItemPurchaseRequested), new Action<MPArmoryCosmeticItemVM>(this.EquipItemOnHeroPreview));
					mparmoryCosmeticItemVM.IsUnlocked = getCosmeticElementList[i].IsFree;
					this._allCosmetics.Add(mparmoryCosmeticItemVM);
				}
			}
		}

		// Token: 0x06000FE6 RID: 4070 RVA: 0x0003490C File Offset: 0x00032B0C
		private void InitializeCosmeticItemComparers()
		{
			this._itemComparers = new List<MPArmoryCosmeticsVM.CosmeticItemComparer>
			{
				new MPArmoryCosmeticsVM.CosmeticItemCategoryComparer(),
				new MPArmoryCosmeticsVM.CosmeticItemCostComparer(),
				new MPArmoryCosmeticsVM.CosmeticItemRarityComparer(),
				new MPArmoryCosmeticsVM.CosmeticItemNameComparer()
			};
			this._currentItemComparer = this._itemComparers[0];
		}

		// Token: 0x06000FE7 RID: 4071 RVA: 0x00034962 File Offset: 0x00032B62
		public void RefreshPlayerData(PlayerData playerData)
		{
			this.Loot = playerData.Gold;
		}

		// Token: 0x06000FE8 RID: 4072 RVA: 0x00034970 File Offset: 0x00032B70
		public void RefreshCosmeticsInfo()
		{
			this.IsLoading = true;
			this.HasCosmeticInfoReceived = true;
			this.IsLoading = false;
			this._ownedCosmetics = NetworkMain.GameClient.OwnedCosmetics.ToList<string>();
			IReadOnlyDictionary<string, List<string>> usedCosmetics = NetworkMain.GameClient.UsedCosmetics;
			this._usedCosmetics = new Dictionary<string, List<string>>();
			foreach (KeyValuePair<string, List<string>> keyValuePair in usedCosmetics)
			{
				this._usedCosmetics.Add(keyValuePair.Key, new List<string>());
				foreach (string text in usedCosmetics[keyValuePair.Key])
				{
					this._usedCosmetics[keyValuePair.Key].Add(text);
				}
			}
			this.RefreshSelectedClass(this._selectedClass, this._getSelectedPerks());
		}

		// Token: 0x06000FE9 RID: 4073 RVA: 0x00034A7C File Offset: 0x00032C7C
		public async Task<bool> UpdateUsedCosmetics()
		{
			IReadOnlyDictionary<string, List<string>> usedCosmetics = NetworkMain.GameClient.UsedCosmetics;
			Dictionary<string, List<ValueTuple<string, bool>>> dictionary = new Dictionary<string, List<ValueTuple<string, bool>>>();
			foreach (string text in this._usedCosmetics.Keys)
			{
				dictionary.Add(text, new List<ValueTuple<string, bool>>());
			}
			foreach (KeyValuePair<string, List<string>> keyValuePair in usedCosmetics)
			{
				foreach (string text2 in keyValuePair.Value)
				{
					if (!this._usedCosmetics[keyValuePair.Key].Contains(text2))
					{
						dictionary[keyValuePair.Key].Add(new ValueTuple<string, bool>(text2, false));
					}
				}
			}
			foreach (KeyValuePair<string, List<string>> keyValuePair2 in this._usedCosmetics)
			{
				if (!usedCosmetics.ContainsKey(keyValuePair2.Key))
				{
					using (List<string>.Enumerator enumerator3 = keyValuePair2.Value.GetEnumerator())
					{
						while (enumerator3.MoveNext())
						{
							string text3 = enumerator3.Current;
							dictionary[keyValuePair2.Key].Add(new ValueTuple<string, bool>(text3, true));
						}
						continue;
					}
				}
				foreach (string text4 in keyValuePair2.Value)
				{
					if (!usedCosmetics[keyValuePair2.Key].Contains(text4))
					{
						dictionary[keyValuePair2.Key].Add(new ValueTuple<string, bool>(text4, true));
					}
				}
			}
			foreach (string text5 in dictionary.Keys)
			{
				List<ItemObject.ItemTypeEnum> list = new List<ItemObject.ItemTypeEnum>();
				foreach (ValueTuple<string, bool> valueTuple in dictionary[text5])
				{
					string usedCosmeticID = valueTuple.Item1;
					if (valueTuple.Item2)
					{
						ItemObject.ItemTypeEnum itemType = this._allCosmetics.FirstOrDefault((MPArmoryCosmeticItemVM c) => c.CosmeticID == usedCosmeticID).EquipmentElement.Item.ItemType;
						list.Add(itemType);
					}
				}
			}
			return await NetworkMain.GameClient.UpdateUsedCosmeticItems(dictionary);
		}

		// Token: 0x06000FEA RID: 4074 RVA: 0x00034AC4 File Offset: 0x00032CC4
		public void RefreshSelectedClass(MultiplayerClassDivisions.MPHeroClass selectedClass, List<IReadOnlyPerkObject> selectedPerks)
		{
			this._selectedClass = selectedClass;
			this._selectedClassDefaultEquipment = this._selectedClass.HeroCharacter.Equipment.Clone(false);
			if (selectedPerks != null)
			{
				MPArmoryVM.ApplyPerkEffectsToEquipment(ref this._selectedClassDefaultEquipment, selectedPerks);
			}
			this._selectedTroopID = this._selectedClass.StringId;
			this.AvailableCosmetics.Sort(this._currentItemComparer);
			if (this._ownedCosmetics != null)
			{
				using (List<string>.Enumerator enumerator = this._ownedCosmetics.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						string ownedCosmeticID = enumerator.Current;
						MPArmoryCosmeticItemVM mparmoryCosmeticItemVM = this.AvailableCosmetics.FirstOrDefault((MPArmoryCosmeticItemVM c) => c.CosmeticID == ownedCosmeticID);
						if (mparmoryCosmeticItemVM != null)
						{
							mparmoryCosmeticItemVM.IsUnlocked = true;
						}
					}
				}
			}
			this.FilterCosmeticsByCategory(this._currentFilterCategory);
		}

		// Token: 0x06000FEB RID: 4075 RVA: 0x00034BA8 File Offset: 0x00032DA8
		private void AddDefaultItem(ItemObject item)
		{
			MPArmoryCosmeticItemVM mparmoryCosmeticItemVM = new MPArmoryCosmeticItemVM(new CosmeticsManager.CosmeticElement(item.StringId, CosmeticsManager.CosmeticRarity.Default, 0, CosmeticsManager.CosmeticType.Clothing), string.Empty, new Action<MPArmoryCosmeticItemVM>(this.OnItemEquipRequested), null, new Action<MPArmoryCosmeticItemVM>(this.EquipItemOnHeroPreview))
			{
				IsUnlocked = true
			};
			if (this._actionKey != null && this._previewKey != null)
			{
				mparmoryCosmeticItemVM.RefreshKeyBindings(this._actionKey, this._previewKey);
			}
			if (this._currentFilterCategory == ItemObject.ItemTypeEnum.Invalid || this._currentFilterCategory == item.ItemType)
			{
				this.AvailableCosmetics.Add(mparmoryCosmeticItemVM);
			}
		}

		// Token: 0x06000FEC RID: 4076 RVA: 0x00034C34 File Offset: 0x00032E34
		private void EquipItemOnHeroPreview(MPArmoryCosmeticItemVM itemVM)
		{
			Action<EquipmentIndex, EquipmentElement> onHeroPreviewItemEquipped = this._onHeroPreviewItemEquipped;
			if (onHeroPreviewItemEquipped == null)
			{
				return;
			}
			onHeroPreviewItemEquipped(this.GetEquipmentIndexOfItemObject(itemVM.EquipmentElement.Item), itemVM.EquipmentElement);
		}

		// Token: 0x06000FED RID: 4077 RVA: 0x00034C6C File Offset: 0x00032E6C
		private async void OnItemEquipRequested(MPArmoryCosmeticItemVM itemVM)
		{
			CosmeticsManager.ClothingCosmeticElement clothingCosmeticElement;
			if ((clothingCosmeticElement = itemVM.Cosmetic as CosmeticsManager.ClothingCosmeticElement) != null && clothingCosmeticElement.ReplaceItemless.Any((Tuple<string, string> r) => r.Item1 == this._selectedClass.StringId))
			{
				if (itemVM.IsUsed)
				{
					itemVM.IsUsed = false;
					itemVM.IsUnlockedUpdated();
					Dictionary<string, List<string>> usedCosmetics = this._usedCosmetics;
					if (usedCosmetics != null)
					{
						usedCosmetics[this._selectedTroopID].Remove(itemVM.CosmeticID);
					}
					Action<EquipmentIndex, EquipmentElement> onHeroPreviewItemEquipped = this._onHeroPreviewItemEquipped;
					if (onHeroPreviewItemEquipped != null)
					{
						onHeroPreviewItemEquipped(this.GetEquipmentIndexOfItemObject(itemVM.EquipmentElement.Item), EquipmentElement.Invalid);
					}
				}
				else
				{
					itemVM.ActionText = itemVM.UnequipText;
					this.OnItemEquipped(itemVM, true);
				}
			}
			else
			{
				this.OnItemEquipped(itemVM, true);
			}
			if (!this._isUpdatingUsedCosmetics)
			{
				this._isUpdatingUsedCosmetics = true;
				await this.UpdateUsedCosmetics();
				this._isUpdatingUsedCosmetics = false;
			}
		}

		// Token: 0x06000FEE RID: 4078 RVA: 0x00034CB0 File Offset: 0x00032EB0
		private void OnItemEquipped(MPArmoryCosmeticItemVM itemVM, bool forceRemove = true)
		{
			this.EquipItemOnHeroPreview(itemVM);
			Dictionary<string, List<string>> usedCosmetics = this._usedCosmetics;
			if (usedCosmetics != null && !usedCosmetics.ContainsKey(this._selectedTroopID))
			{
				this._usedCosmetics.Add(this._selectedTroopID, new List<string>());
			}
			if (itemVM.CosmeticID != string.Empty && !this._usedCosmetics[this._selectedTroopID].Contains(itemVM.CosmeticID))
			{
				this._usedCosmetics[this._selectedTroopID].Add(itemVM.CosmeticID);
			}
			foreach (MPArmoryCosmeticItemVM mparmoryCosmeticItemVM in this.AvailableCosmetics)
			{
				if (mparmoryCosmeticItemVM.EquipmentElement.Item.ItemType == itemVM.EquipmentElement.Item.ItemType)
				{
					mparmoryCosmeticItemVM.IsUsed = false;
					if (itemVM.Cosmetic.Id != mparmoryCosmeticItemVM.Cosmetic.Id && forceRemove)
					{
						Dictionary<string, List<string>> usedCosmetics2 = this._usedCosmetics;
						if (usedCosmetics2 != null)
						{
							usedCosmetics2[this._selectedTroopID].Remove(mparmoryCosmeticItemVM.CosmeticID);
						}
					}
				}
			}
			itemVM.IsUsed = true;
		}

		// Token: 0x06000FEF RID: 4079 RVA: 0x00034DF4 File Offset: 0x00032FF4
		private void OnItemPurchaseRequested(MPArmoryCosmeticItemVM itemVM)
		{
			Action<MPArmoryCosmeticItemVM> onItemObtainRequested = this._onItemObtainRequested;
			if (onItemObtainRequested == null)
			{
				return;
			}
			onItemObtainRequested(itemVM);
		}

		// Token: 0x06000FF0 RID: 4080 RVA: 0x00034E07 File Offset: 0x00033007
		public void OnItemObtained(string cosmeticID, int finalLoot)
		{
			this._ownedCosmetics.Add(cosmeticID);
			this.RefreshCosmeticsInfo();
			this.Loot = finalLoot;
		}

		// Token: 0x06000FF1 RID: 4081 RVA: 0x00034E24 File Offset: 0x00033024
		private void OnSortCategoryUpdated(SelectorVM<SelectorItemVM> selector)
		{
			if (this.SortCategories.SelectedIndex == -1)
			{
				this.SortCategories.SelectedIndex = 0;
			}
			this._currentItemComparer = this._itemComparers[selector.SelectedIndex];
			this.AvailableCosmetics.Sort(this._currentItemComparer);
		}

		// Token: 0x06000FF2 RID: 4082 RVA: 0x00034E74 File Offset: 0x00033074
		private void OnSortOrderUpdated(SelectorVM<SelectorItemVM> selector)
		{
			if (this.SortOrders.SelectedIndex == -1)
			{
				this.SortOrders.SelectedIndex = 0;
			}
			foreach (MPArmoryCosmeticsVM.CosmeticItemComparer cosmeticItemComparer in this._itemComparers)
			{
				cosmeticItemComparer.SetSortMode(selector.SelectedIndex == 0);
			}
			this.AvailableCosmetics.Sort(this._currentItemComparer);
		}

		// Token: 0x06000FF3 RID: 4083 RVA: 0x00034EF8 File Offset: 0x000330F8
		private EquipmentIndex GetEquipmentIndexOfItemObject(ItemObject itemObject)
		{
			if (itemObject == null)
			{
				return EquipmentIndex.None;
			}
			ItemObject.ItemTypeEnum type = itemObject.Type;
			if (type <= ItemObject.ItemTypeEnum.HandArmor)
			{
				if (type == ItemObject.ItemTypeEnum.Horse)
				{
					return EquipmentIndex.ArmorItemEndSlot;
				}
				switch (type)
				{
				case ItemObject.ItemTypeEnum.HeadArmor:
					return EquipmentIndex.NumAllWeaponSlots;
				case ItemObject.ItemTypeEnum.BodyArmor:
					return EquipmentIndex.Body;
				case ItemObject.ItemTypeEnum.LegArmor:
					return EquipmentIndex.Leg;
				case ItemObject.ItemTypeEnum.HandArmor:
					return EquipmentIndex.Gloves;
				}
			}
			else
			{
				if (type == ItemObject.ItemTypeEnum.Cape)
				{
					return EquipmentIndex.Cape;
				}
				if (type == ItemObject.ItemTypeEnum.HorseHarness)
				{
					return EquipmentIndex.HorseHarness;
				}
			}
			return EquipmentIndex.None;
		}

		// Token: 0x06000FF4 RID: 4084 RVA: 0x00034F54 File Offset: 0x00033154
		public void ExecuteFilterByCategory(int categoryIndex)
		{
			ItemObject.ItemTypeEnum itemTypeEnum;
			switch (categoryIndex)
			{
			case 1:
				itemTypeEnum = ItemObject.ItemTypeEnum.BodyArmor;
				break;
			case 2:
				itemTypeEnum = ItemObject.ItemTypeEnum.HeadArmor;
				break;
			case 3:
				itemTypeEnum = ItemObject.ItemTypeEnum.Cape;
				break;
			case 4:
				itemTypeEnum = ItemObject.ItemTypeEnum.HandArmor;
				break;
			case 5:
				itemTypeEnum = ItemObject.ItemTypeEnum.LegArmor;
				break;
			default:
				itemTypeEnum = ItemObject.ItemTypeEnum.Invalid;
				break;
			}
			this._currentFilterCategory = itemTypeEnum;
			this.FilterCosmeticsByCategory(itemTypeEnum);
		}

		// Token: 0x06000FF5 RID: 4085 RVA: 0x00034FAC File Offset: 0x000331AC
		private void FilterCosmeticsByCategory(ItemObject.ItemTypeEnum itemType = ItemObject.ItemTypeEnum.Invalid)
		{
			this.AvailableCosmetics.Clear();
			this._defaultCosmeticIDs.Clear();
			for (EquipmentIndex equipmentIndex = EquipmentIndex.NumAllWeaponSlots; equipmentIndex < EquipmentIndex.ArmorItemEndSlot; equipmentIndex++)
			{
				ItemObject item = this._selectedClassDefaultEquipment[equipmentIndex].Item;
				if (item != null)
				{
					this._defaultCosmeticIDs.Add(this._selectedClassDefaultEquipment[equipmentIndex].Item.StringId);
					this.AddDefaultItem(item);
				}
			}
			foreach (MPArmoryCosmeticItemVM mparmoryCosmeticItemVM in this._allCosmetics)
			{
				bool flag = mparmoryCosmeticItemVM.EquipmentElement.Item.ItemType == itemType || itemType == ItemObject.ItemTypeEnum.Invalid;
				CosmeticsManager.ClothingCosmeticElement clothingCosmeticElement;
				bool flag2 = (clothingCosmeticElement = mparmoryCosmeticItemVM.Cosmetic as CosmeticsManager.ClothingCosmeticElement) != null && (clothingCosmeticElement.ReplaceItemsId.Any((string c) => this._defaultCosmeticIDs.Contains(c)) || clothingCosmeticElement.ReplaceItemless.Any((Tuple<string, string> r) => r.Item1 == this._selectedClass.StringId));
				if (flag && flag2)
				{
					this.AvailableCosmetics.Add(mparmoryCosmeticItemVM);
					MPArmoryCosmeticItemVM mparmoryCosmeticItemVM2 = mparmoryCosmeticItemVM;
					List<string> ownedCosmetics = this._ownedCosmetics;
					mparmoryCosmeticItemVM2.IsUnlocked = (ownedCosmetics != null && ownedCosmetics.Contains(mparmoryCosmeticItemVM.CosmeticID)) || mparmoryCosmeticItemVM.Cosmetic.IsFree;
				}
			}
			this.AvailableCosmetics.Sort(this._currentItemComparer);
			this.RefreshEquipment();
		}

		// Token: 0x06000FF6 RID: 4086 RVA: 0x00035128 File Offset: 0x00033328
		private void RefreshEquipment()
		{
			Dictionary<EquipmentIndex, bool> dictionary = new Dictionary<EquipmentIndex, bool>();
			for (EquipmentIndex equipmentIndex2 = EquipmentIndex.NumAllWeaponSlots; equipmentIndex2 < EquipmentIndex.ArmorItemEndSlot; equipmentIndex2++)
			{
				dictionary.Add(equipmentIndex2, false);
			}
			foreach (MPArmoryCosmeticItemVM mparmoryCosmeticItemVM in this.AvailableCosmetics.Where((MPArmoryCosmeticItemVM c) => c.Cosmetic.Rarity == CosmeticsManager.CosmeticRarity.Default))
			{
				this.OnItemEquipped(mparmoryCosmeticItemVM, false);
				dictionary[this.GetEquipmentIndexOfItemObject(mparmoryCosmeticItemVM.EquipmentElement.Item)] = true;
			}
			Dictionary<string, List<string>> usedCosmetics = this._usedCosmetics;
			if (usedCosmetics != null && usedCosmetics.ContainsKey(this._selectedTroopID))
			{
				Dictionary<string, List<string>> dictionary2 = new Dictionary<string, List<string>>();
				foreach (string text in this._usedCosmetics.Keys)
				{
					List<string> list = new List<string>();
					foreach (string text2 in this._usedCosmetics[text])
					{
						list.Add(text2);
					}
					dictionary2.Add(text, list);
				}
				using (List<string>.Enumerator enumerator3 = dictionary2[this._selectedTroopID].GetEnumerator())
				{
					while (enumerator3.MoveNext())
					{
						string cosmeticID = enumerator3.Current;
						MPArmoryCosmeticItemVM mparmoryCosmeticItemVM2 = this._allCosmetics.First((MPArmoryCosmeticItemVM c) => c.CosmeticID == cosmeticID);
						EquipmentIndex equipmentIndexOfItemObject = this.GetEquipmentIndexOfItemObject(mparmoryCosmeticItemVM2.EquipmentElement.Item);
						if (!(mparmoryCosmeticItemVM2.Cosmetic as CosmeticsManager.ClothingCosmeticElement).ReplaceItemless.IsEmpty<Tuple<string, string>>() || !this._selectedClassDefaultEquipment[equipmentIndexOfItemObject].IsEmpty)
						{
							this.OnItemEquipped(mparmoryCosmeticItemVM2, true);
							dictionary[equipmentIndexOfItemObject] = true;
						}
					}
				}
			}
			using (Dictionary<EquipmentIndex, bool>.KeyCollection.Enumerator enumerator4 = dictionary.Keys.GetEnumerator())
			{
				while (enumerator4.MoveNext())
				{
					EquipmentIndex equipmentIndex = enumerator4.Current;
					if (!dictionary[equipmentIndex])
					{
						IEnumerable<MPArmoryCosmeticItemVM> availableCosmetics = this.AvailableCosmetics;
						Func<MPArmoryCosmeticItemVM, bool> func;
						Func<MPArmoryCosmeticItemVM, bool> <>9__2;
						if ((func = <>9__2) == null)
						{
							func = (<>9__2 = (MPArmoryCosmeticItemVM c) => this.GetEquipmentIndexOfItemObject(c.EquipmentElement.Item) == equipmentIndex);
						}
						foreach (MPArmoryCosmeticItemVM mparmoryCosmeticItemVM3 in availableCosmetics.Where(func))
						{
							mparmoryCosmeticItemVM3.IsUsed = false;
						}
					}
				}
			}
		}

		// Token: 0x06000FF7 RID: 4087 RVA: 0x0003542C File Offset: 0x0003362C
		public void RefreshKeyBindings(HotKey actionKey, HotKey previewKey)
		{
			foreach (MPArmoryCosmeticItemVM mparmoryCosmeticItemVM in this._allCosmetics)
			{
				mparmoryCosmeticItemVM.RefreshKeyBindings(actionKey, previewKey);
			}
			foreach (MPArmoryCosmeticItemVM mparmoryCosmeticItemVM2 in this._availableCosmetics)
			{
				mparmoryCosmeticItemVM2.RefreshKeyBindings(actionKey, previewKey);
			}
			this._actionKey = actionKey;
			this._previewKey = previewKey;
		}

		// Token: 0x06000FF8 RID: 4088 RVA: 0x000354C8 File Offset: 0x000336C8
		private void ExecuteRefreshCosmeticInfo()
		{
			this.RefreshCosmeticsInfo();
		}

		// Token: 0x06000FF9 RID: 4089 RVA: 0x000354D0 File Offset: 0x000336D0
		private void ExecuteResetPreview()
		{
			this.RefreshSelectedClass(this._selectedClass, this._getSelectedPerks());
		}

		// Token: 0x1700051B RID: 1307
		// (get) Token: 0x06000FFA RID: 4090 RVA: 0x000354E9 File Offset: 0x000336E9
		// (set) Token: 0x06000FFB RID: 4091 RVA: 0x000354F1 File Offset: 0x000336F1
		[DataSourceProperty]
		public int Loot
		{
			get
			{
				return this._loot;
			}
			set
			{
				if (value != this._loot)
				{
					this._loot = value;
					base.OnPropertyChangedWithValue(value, "Loot");
				}
			}
		}

		// Token: 0x1700051C RID: 1308
		// (get) Token: 0x06000FFC RID: 4092 RVA: 0x0003550F File Offset: 0x0003370F
		// (set) Token: 0x06000FFD RID: 4093 RVA: 0x00035517 File Offset: 0x00033717
		[DataSourceProperty]
		public bool IsLoading
		{
			get
			{
				return this._isLoading;
			}
			set
			{
				if (value != this._isLoading)
				{
					this._isLoading = value;
					base.OnPropertyChangedWithValue(value, "IsLoading");
				}
			}
		}

		// Token: 0x1700051D RID: 1309
		// (get) Token: 0x06000FFE RID: 4094 RVA: 0x00035535 File Offset: 0x00033735
		// (set) Token: 0x06000FFF RID: 4095 RVA: 0x0003553D File Offset: 0x0003373D
		[DataSourceProperty]
		public bool HasCosmeticInfoReceived
		{
			get
			{
				return this._hasCosmeticInfoReceived;
			}
			set
			{
				if (value != this._hasCosmeticInfoReceived)
				{
					this._hasCosmeticInfoReceived = value;
					base.OnPropertyChangedWithValue(value, "HasCosmeticInfoReceived");
				}
			}
		}

		// Token: 0x1700051E RID: 1310
		// (get) Token: 0x06001000 RID: 4096 RVA: 0x0003555B File Offset: 0x0003375B
		// (set) Token: 0x06001001 RID: 4097 RVA: 0x00035563 File Offset: 0x00033763
		[DataSourceProperty]
		public string CosmeticInfoErrorText
		{
			get
			{
				return this._cosmeticInfoErrorText;
			}
			set
			{
				if (value != this._cosmeticInfoErrorText)
				{
					this._cosmeticInfoErrorText = value;
					base.OnPropertyChangedWithValue<string>(value, "CosmeticInfoErrorText");
				}
			}
		}

		// Token: 0x1700051F RID: 1311
		// (get) Token: 0x06001002 RID: 4098 RVA: 0x00035586 File Offset: 0x00033786
		// (set) Token: 0x06001003 RID: 4099 RVA: 0x0003558E File Offset: 0x0003378E
		[DataSourceProperty]
		public HintViewModel AllCategoriesHint
		{
			get
			{
				return this._allCategoriesHint;
			}
			set
			{
				if (value != this._allCategoriesHint)
				{
					this._allCategoriesHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "AllCategoriesHint");
				}
			}
		}

		// Token: 0x17000520 RID: 1312
		// (get) Token: 0x06001004 RID: 4100 RVA: 0x000355AC File Offset: 0x000337AC
		// (set) Token: 0x06001005 RID: 4101 RVA: 0x000355B4 File Offset: 0x000337B4
		[DataSourceProperty]
		public HintViewModel BodyCategoryHint
		{
			get
			{
				return this._bodyCategoryHint;
			}
			set
			{
				if (value != this._bodyCategoryHint)
				{
					this._bodyCategoryHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "BodyCategoryHint");
				}
			}
		}

		// Token: 0x17000521 RID: 1313
		// (get) Token: 0x06001006 RID: 4102 RVA: 0x000355D2 File Offset: 0x000337D2
		// (set) Token: 0x06001007 RID: 4103 RVA: 0x000355DA File Offset: 0x000337DA
		[DataSourceProperty]
		public HintViewModel HeadCategoryHint
		{
			get
			{
				return this._headCategoryHint;
			}
			set
			{
				if (value != this._headCategoryHint)
				{
					this._headCategoryHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "HeadCategoryHint");
				}
			}
		}

		// Token: 0x17000522 RID: 1314
		// (get) Token: 0x06001008 RID: 4104 RVA: 0x000355F8 File Offset: 0x000337F8
		// (set) Token: 0x06001009 RID: 4105 RVA: 0x00035600 File Offset: 0x00033800
		[DataSourceProperty]
		public HintViewModel ShoulderCategoryHint
		{
			get
			{
				return this._shoulderCategoryHint;
			}
			set
			{
				if (value != this._shoulderCategoryHint)
				{
					this._shoulderCategoryHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "ShoulderCategoryHint");
				}
			}
		}

		// Token: 0x17000523 RID: 1315
		// (get) Token: 0x0600100A RID: 4106 RVA: 0x0003561E File Offset: 0x0003381E
		// (set) Token: 0x0600100B RID: 4107 RVA: 0x00035626 File Offset: 0x00033826
		[DataSourceProperty]
		public HintViewModel HandCategoryHint
		{
			get
			{
				return this._handCategoryHint;
			}
			set
			{
				if (value != this._handCategoryHint)
				{
					this._handCategoryHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "HandCategoryHint");
				}
			}
		}

		// Token: 0x17000524 RID: 1316
		// (get) Token: 0x0600100C RID: 4108 RVA: 0x00035644 File Offset: 0x00033844
		// (set) Token: 0x0600100D RID: 4109 RVA: 0x0003564C File Offset: 0x0003384C
		[DataSourceProperty]
		public HintViewModel LegCategoryHint
		{
			get
			{
				return this._legCategoryHint;
			}
			set
			{
				if (value != this._legCategoryHint)
				{
					this._legCategoryHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "LegCategoryHint");
				}
			}
		}

		// Token: 0x17000525 RID: 1317
		// (get) Token: 0x0600100E RID: 4110 RVA: 0x0003566A File Offset: 0x0003386A
		// (set) Token: 0x0600100F RID: 4111 RVA: 0x00035672 File Offset: 0x00033872
		[DataSourceProperty]
		public HintViewModel ResetPreviewHint
		{
			get
			{
				return this._resetPreviewHint;
			}
			set
			{
				if (value != this._resetPreviewHint)
				{
					this._resetPreviewHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "ResetPreviewHint");
				}
			}
		}

		// Token: 0x17000526 RID: 1318
		// (get) Token: 0x06001010 RID: 4112 RVA: 0x00035690 File Offset: 0x00033890
		// (set) Token: 0x06001011 RID: 4113 RVA: 0x00035698 File Offset: 0x00033898
		[DataSourceProperty]
		public SelectorVM<SelectorItemVM> SortCategories
		{
			get
			{
				return this._sortCategories;
			}
			set
			{
				if (value != this._sortCategories)
				{
					this._sortCategories = value;
					base.OnPropertyChangedWithValue<SelectorVM<SelectorItemVM>>(value, "SortCategories");
				}
			}
		}

		// Token: 0x17000527 RID: 1319
		// (get) Token: 0x06001012 RID: 4114 RVA: 0x000356B6 File Offset: 0x000338B6
		// (set) Token: 0x06001013 RID: 4115 RVA: 0x000356BE File Offset: 0x000338BE
		[DataSourceProperty]
		public SelectorVM<SelectorItemVM> SortOrders
		{
			get
			{
				return this._sortOrders;
			}
			set
			{
				if (value != this._sortOrders)
				{
					this._sortOrders = value;
					base.OnPropertyChangedWithValue<SelectorVM<SelectorItemVM>>(value, "SortOrders");
				}
			}
		}

		// Token: 0x17000528 RID: 1320
		// (get) Token: 0x06001014 RID: 4116 RVA: 0x000356DC File Offset: 0x000338DC
		// (set) Token: 0x06001015 RID: 4117 RVA: 0x000356E4 File Offset: 0x000338E4
		[DataSourceProperty]
		public MBBindingList<MPArmoryCosmeticItemVM> AvailableCosmetics
		{
			get
			{
				return this._availableCosmetics;
			}
			set
			{
				if (value != this._availableCosmetics)
				{
					this._availableCosmetics = value;
					base.OnPropertyChangedWithValue<MBBindingList<MPArmoryCosmeticItemVM>>(value, "AvailableCosmetics");
				}
			}
		}

		// Token: 0x0400077F RID: 1919
		private readonly Action<EquipmentIndex, EquipmentElement> _onHeroPreviewItemEquipped;

		// Token: 0x04000780 RID: 1920
		private readonly Action _resetHeroEquipment;

		// Token: 0x04000781 RID: 1921
		private readonly Action<MPArmoryCosmeticItemVM> _onItemObtainRequested;

		// Token: 0x04000782 RID: 1922
		private readonly Func<List<IReadOnlyPerkObject>> _getSelectedPerks;

		// Token: 0x04000783 RID: 1923
		private List<MPArmoryCosmeticItemVM> _allCosmetics;

		// Token: 0x04000784 RID: 1924
		private List<string> _ownedCosmetics;

		// Token: 0x04000785 RID: 1925
		private Dictionary<string, List<string>> _usedCosmetics;

		// Token: 0x04000786 RID: 1926
		private List<string> _defaultCosmeticIDs;

		// Token: 0x04000787 RID: 1927
		private ItemObject.ItemTypeEnum _currentFilterCategory;

		// Token: 0x04000788 RID: 1928
		private MPArmoryCosmeticsVM.CosmeticItemComparer _currentItemComparer;

		// Token: 0x04000789 RID: 1929
		private List<MPArmoryCosmeticsVM.CosmeticItemComparer> _itemComparers;

		// Token: 0x0400078A RID: 1930
		private MultiplayerClassDivisions.MPHeroClass _selectedClass;

		// Token: 0x0400078B RID: 1931
		private Equipment _selectedClassDefaultEquipment;

		// Token: 0x0400078C RID: 1932
		private string _selectedTroopID;

		// Token: 0x0400078D RID: 1933
		private bool _isUpdatingUsedCosmetics;

		// Token: 0x0400078E RID: 1934
		private HotKey _actionKey;

		// Token: 0x0400078F RID: 1935
		private HotKey _previewKey;

		// Token: 0x04000790 RID: 1936
		private int _loot;

		// Token: 0x04000791 RID: 1937
		private bool _isLoading;

		// Token: 0x04000792 RID: 1938
		private bool _hasCosmeticInfoReceived;

		// Token: 0x04000793 RID: 1939
		private string _cosmeticInfoErrorText;

		// Token: 0x04000794 RID: 1940
		private HintViewModel _allCategoriesHint;

		// Token: 0x04000795 RID: 1941
		private HintViewModel _bodyCategoryHint;

		// Token: 0x04000796 RID: 1942
		private HintViewModel _headCategoryHint;

		// Token: 0x04000797 RID: 1943
		private HintViewModel _shoulderCategoryHint;

		// Token: 0x04000798 RID: 1944
		private HintViewModel _handCategoryHint;

		// Token: 0x04000799 RID: 1945
		private HintViewModel _legCategoryHint;

		// Token: 0x0400079A RID: 1946
		private HintViewModel _resetPreviewHint;

		// Token: 0x0400079B RID: 1947
		private SelectorVM<SelectorItemVM> _sortCategories;

		// Token: 0x0400079C RID: 1948
		private SelectorVM<SelectorItemVM> _sortOrders;

		// Token: 0x0400079D RID: 1949
		private MBBindingList<MPArmoryCosmeticItemVM> _availableCosmetics;

		// Token: 0x02000202 RID: 514
		private abstract class CosmeticItemComparer : IComparer<MPArmoryCosmeticItemVM>
		{
			// Token: 0x170007DD RID: 2013
			// (get) Token: 0x06001ABB RID: 6843 RVA: 0x00056902 File Offset: 0x00054B02
			protected int _sortMultiplier
			{
				get
				{
					if (!this._isAscending)
					{
						return -1;
					}
					return 1;
				}
			}

			// Token: 0x06001ABC RID: 6844 RVA: 0x0005690F File Offset: 0x00054B0F
			public void SetSortMode(bool isAscending)
			{
				this._isAscending = isAscending;
			}

			// Token: 0x06001ABD RID: 6845
			public abstract int Compare(MPArmoryCosmeticItemVM x, MPArmoryCosmeticItemVM y);

			// Token: 0x04000E4F RID: 3663
			private bool _isAscending;
		}

		// Token: 0x02000203 RID: 515
		private class CosmeticItemNameComparer : MPArmoryCosmeticsVM.CosmeticItemComparer
		{
			// Token: 0x06001ABF RID: 6847 RVA: 0x00056920 File Offset: 0x00054B20
			public override int Compare(MPArmoryCosmeticItemVM x, MPArmoryCosmeticItemVM y)
			{
				return x.Name.CompareTo(y.Name) * base._sortMultiplier;
			}
		}

		// Token: 0x02000204 RID: 516
		private class CosmeticItemCostComparer : MPArmoryCosmeticsVM.CosmeticItemComparer
		{
			// Token: 0x06001AC1 RID: 6849 RVA: 0x00056944 File Offset: 0x00054B44
			public override int Compare(MPArmoryCosmeticItemVM x, MPArmoryCosmeticItemVM y)
			{
				int num = x.Cost.CompareTo(y.Cost);
				if (num == 0)
				{
					num = x.EquipmentElement.Item.ItemType.CompareTo(y.EquipmentElement.Item.ItemType);
				}
				return num * base._sortMultiplier;
			}
		}

		// Token: 0x02000205 RID: 517
		private class CosmeticItemRarityComparer : MPArmoryCosmeticsVM.CosmeticItemComparer
		{
			// Token: 0x06001AC3 RID: 6851 RVA: 0x000569B4 File Offset: 0x00054BB4
			public override int Compare(MPArmoryCosmeticItemVM x, MPArmoryCosmeticItemVM y)
			{
				int num = x.Cosmetic.Rarity.CompareTo(y.Cosmetic.Rarity);
				if (num == 0)
				{
					return x.EquipmentElement.Item.ItemType.CompareTo(y.EquipmentElement.Item.ItemType);
				}
				return num * base._sortMultiplier;
			}
		}

		// Token: 0x02000206 RID: 518
		private class CosmeticItemCategoryComparer : MPArmoryCosmeticsVM.CosmeticItemComparer
		{
			// Token: 0x06001AC5 RID: 6853 RVA: 0x00056A38 File Offset: 0x00054C38
			public override int Compare(MPArmoryCosmeticItemVM x, MPArmoryCosmeticItemVM y)
			{
				return x.EquipmentElement.Item.ItemType.CompareTo(y.EquipmentElement.Item.ItemType) * base._sortMultiplier;
			}
		}
	}
}
