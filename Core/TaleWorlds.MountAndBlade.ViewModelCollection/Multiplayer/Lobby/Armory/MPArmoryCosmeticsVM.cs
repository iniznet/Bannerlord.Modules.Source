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
	public class MPArmoryCosmeticsVM : ViewModel
	{
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

		public void RefreshPlayerData(PlayerData playerData)
		{
			this.Loot = playerData.Gold;
		}

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

		private void EquipItemOnHeroPreview(MPArmoryCosmeticItemVM itemVM)
		{
			Action<EquipmentIndex, EquipmentElement> onHeroPreviewItemEquipped = this._onHeroPreviewItemEquipped;
			if (onHeroPreviewItemEquipped == null)
			{
				return;
			}
			onHeroPreviewItemEquipped(this.GetEquipmentIndexOfItemObject(itemVM.EquipmentElement.Item), itemVM.EquipmentElement);
		}

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

		private void OnItemPurchaseRequested(MPArmoryCosmeticItemVM itemVM)
		{
			Action<MPArmoryCosmeticItemVM> onItemObtainRequested = this._onItemObtainRequested;
			if (onItemObtainRequested == null)
			{
				return;
			}
			onItemObtainRequested(itemVM);
		}

		public void OnItemObtained(string cosmeticID, int finalLoot)
		{
			this._ownedCosmetics.Add(cosmeticID);
			this.RefreshCosmeticsInfo();
			this.Loot = finalLoot;
		}

		private void OnSortCategoryUpdated(SelectorVM<SelectorItemVM> selector)
		{
			if (this.SortCategories.SelectedIndex == -1)
			{
				this.SortCategories.SelectedIndex = 0;
			}
			this._currentItemComparer = this._itemComparers[selector.SelectedIndex];
			this.AvailableCosmetics.Sort(this._currentItemComparer);
		}

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

		private void ExecuteRefreshCosmeticInfo()
		{
			this.RefreshCosmeticsInfo();
		}

		private void ExecuteResetPreview()
		{
			this.RefreshSelectedClass(this._selectedClass, this._getSelectedPerks());
		}

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

		private readonly Action<EquipmentIndex, EquipmentElement> _onHeroPreviewItemEquipped;

		private readonly Action _resetHeroEquipment;

		private readonly Action<MPArmoryCosmeticItemVM> _onItemObtainRequested;

		private readonly Func<List<IReadOnlyPerkObject>> _getSelectedPerks;

		private List<MPArmoryCosmeticItemVM> _allCosmetics;

		private List<string> _ownedCosmetics;

		private Dictionary<string, List<string>> _usedCosmetics;

		private List<string> _defaultCosmeticIDs;

		private ItemObject.ItemTypeEnum _currentFilterCategory;

		private MPArmoryCosmeticsVM.CosmeticItemComparer _currentItemComparer;

		private List<MPArmoryCosmeticsVM.CosmeticItemComparer> _itemComparers;

		private MultiplayerClassDivisions.MPHeroClass _selectedClass;

		private Equipment _selectedClassDefaultEquipment;

		private string _selectedTroopID;

		private bool _isUpdatingUsedCosmetics;

		private HotKey _actionKey;

		private HotKey _previewKey;

		private int _loot;

		private bool _isLoading;

		private bool _hasCosmeticInfoReceived;

		private string _cosmeticInfoErrorText;

		private HintViewModel _allCategoriesHint;

		private HintViewModel _bodyCategoryHint;

		private HintViewModel _headCategoryHint;

		private HintViewModel _shoulderCategoryHint;

		private HintViewModel _handCategoryHint;

		private HintViewModel _legCategoryHint;

		private HintViewModel _resetPreviewHint;

		private SelectorVM<SelectorItemVM> _sortCategories;

		private SelectorVM<SelectorItemVM> _sortOrders;

		private MBBindingList<MPArmoryCosmeticItemVM> _availableCosmetics;

		private abstract class CosmeticItemComparer : IComparer<MPArmoryCosmeticItemVM>
		{
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

			public void SetSortMode(bool isAscending)
			{
				this._isAscending = isAscending;
			}

			public abstract int Compare(MPArmoryCosmeticItemVM x, MPArmoryCosmeticItemVM y);

			private bool _isAscending;
		}

		private class CosmeticItemNameComparer : MPArmoryCosmeticsVM.CosmeticItemComparer
		{
			public override int Compare(MPArmoryCosmeticItemVM x, MPArmoryCosmeticItemVM y)
			{
				return x.Name.CompareTo(y.Name) * base._sortMultiplier;
			}
		}

		private class CosmeticItemCostComparer : MPArmoryCosmeticsVM.CosmeticItemComparer
		{
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

		private class CosmeticItemRarityComparer : MPArmoryCosmeticsVM.CosmeticItemComparer
		{
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

		private class CosmeticItemCategoryComparer : MPArmoryCosmeticsVM.CosmeticItemComparer
		{
			public override int Compare(MPArmoryCosmeticItemVM x, MPArmoryCosmeticItemVM y)
			{
				return x.EquipmentElement.Item.ItemType.CompareTo(y.EquipmentElement.Item.ItemType) * base._sortMultiplier;
			}
		}
	}
}
