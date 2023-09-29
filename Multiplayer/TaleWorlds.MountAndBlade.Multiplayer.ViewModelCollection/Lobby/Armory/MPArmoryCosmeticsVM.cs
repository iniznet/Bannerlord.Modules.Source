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
using TaleWorlds.MountAndBlade.Diamond.Cosmetics;
using TaleWorlds.MountAndBlade.Diamond.Cosmetics.CosmeticTypes;
using TaleWorlds.MountAndBlade.Multiplayer.ViewModelCollection.Lobby.Armory.CosmeticCategory;
using TaleWorlds.MountAndBlade.Multiplayer.ViewModelCollection.Lobby.Armory.CosmeticItem;
using TaleWorlds.MountAndBlade.ViewModelCollection.Input;

namespace TaleWorlds.MountAndBlade.Multiplayer.ViewModelCollection.Lobby.Armory
{
	public class MPArmoryCosmeticsVM : ViewModel
	{
		public static event Action<MPArmoryCosmeticItemBaseVM> OnCosmeticPreview;

		public static event Action<MPArmoryCosmeticItemBaseVM> OnRemoveCosmeticFromPreview;

		public static event Action<List<EquipmentElement>> OnEquipmentRefreshed;

		public static event Action OnTauntAssignmentRefresh;

		public MPArmoryCosmeticsVM(Func<List<IReadOnlyPerkObject>> getSelectedPerks)
		{
			this._getSelectedPerks = getSelectedPerks;
			this._usedCosmetics = new Dictionary<string, List<string>>();
			this._ownedCosmetics = new List<string>();
			this._clothingCategoriesLookup = new Dictionary<MPArmoryCosmeticsVM.ClothingCategory, MPArmoryClothingCosmeticCategoryVM>();
			this._tauntCategoriesLookup = new Dictionary<MPArmoryCosmeticsVM.TauntCategoryFlag, MPArmoryTauntCosmeticCategoryVM>();
			this._cosmeticItemsLookup = new Dictionary<string, MPArmoryCosmeticItemBaseVM>();
			this.AvailableCategories = new MBBindingList<MPArmoryCosmeticCategoryBaseVM>();
			this.SortCategories = new SelectorVM<SelectorItemVM>(0, new Action<SelectorVM<SelectorItemVM>>(this.OnSortCategoryUpdated));
			this.SortOrders = new SelectorVM<SelectorItemVM>(0, new Action<SelectorVM<SelectorItemVM>>(this.OnSortOrderUpdated));
			this.TauntSlots = new MBBindingList<MPArmoryCosmeticTauntSlotVM>();
			this.InitializeCosmeticItemComparers();
			this.InitializeAllCosmetics();
			this.InitializeCallbacks();
			this.IsLoading = true;
			this.SortCategories.AddItem(new SelectorItemVM(new TextObject("{=J2wEawTl}Category", null)));
			this.SortCategories.AddItem(new SelectorItemVM(new TextObject("{=ebUrBmHK}Price", null)));
			this.SortCategories.AddItem(new SelectorItemVM(new TextObject("{=bD8nTS86}Rarity", null)));
			this.SortCategories.AddItem(new SelectorItemVM(new TextObject("{=PDdh1sBj}Name", null)));
			this.SortCategories.SelectedIndex = 0;
			this.SortOrders.AddItem(new SelectorItemVM(new TextObject("{=mOmFzU78}Ascending", null)));
			this.SortOrders.AddItem(new SelectorItemVM(new TextObject("{=FgFUsncP}Descending", null)));
			this.SortOrders.SelectedIndex = 0;
			this.RefreshAvailableCategoriesBy(0);
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
			this._allCosmetics.ForEach(delegate(MPArmoryCosmeticItemBaseVM c)
			{
				c.RefreshValues();
			});
			this.AvailableCategories.ApplyActionOnAllItems(delegate(MPArmoryCosmeticCategoryBaseVM c)
			{
				c.RefreshValues();
			});
		}

		private void InitializeCallbacks()
		{
			MPArmoryClothingCosmeticCategoryVM.OnSelected += this.OnClothingCosmeticCategorySelected;
			MPArmoryTauntCosmeticCategoryVM.OnSelected += this.OnTauntCosmeticCategorySelected;
			MPArmoryCosmeticItemBaseVM.OnPreviewed += this.EquipItemOnHeroPreview;
			MPArmoryCosmeticItemBaseVM.OnEquipped += this.OnCosmeticEquipRequested;
			MPArmoryCosmeticTauntSlotVM.OnFocusChanged += this.OnTauntSlotFocusChanged;
			MPArmoryCosmeticTauntSlotVM.OnSelected += this.OnTauntSlotSelected;
			MPArmoryCosmeticTauntSlotVM.OnPreview += this.OnTauntSlotPreview;
			MPArmoryCosmeticTauntSlotVM.OnTauntEquipped += this.OnTauntItemEquipped;
		}

		private void FinalizeCallbacks()
		{
			MPArmoryClothingCosmeticCategoryVM.OnSelected -= this.OnClothingCosmeticCategorySelected;
			MPArmoryTauntCosmeticCategoryVM.OnSelected -= this.OnTauntCosmeticCategorySelected;
			MPArmoryCosmeticItemBaseVM.OnPreviewed -= this.EquipItemOnHeroPreview;
			MPArmoryCosmeticItemBaseVM.OnEquipped -= this.OnCosmeticEquipRequested;
			MPArmoryCosmeticTauntSlotVM.OnFocusChanged -= this.OnTauntSlotFocusChanged;
			MPArmoryCosmeticTauntSlotVM.OnSelected -= this.OnTauntSlotSelected;
			MPArmoryCosmeticTauntSlotVM.OnPreview -= this.OnTauntSlotPreview;
			MPArmoryCosmeticTauntSlotVM.OnTauntEquipped -= this.OnTauntItemEquipped;
		}

		public override void OnFinalize()
		{
			base.OnFinalize();
			this.FinalizeCallbacks();
			this.AvailableCategories.ApplyActionOnAllItems(delegate(MPArmoryCosmeticCategoryBaseVM c)
			{
				c.OnFinalize();
			});
		}

		public async void OnTick(float dt)
		{
			if (NetworkMain.GameClient == null)
			{
				this._isNetworkCosmeticsDirty = false;
				this._isLocalCosmeticsDirty = false;
			}
			if (!this._isSendingCosmeticData && !this._isRetrievingCosmeticData)
			{
				if (this._isNetworkCosmeticsDirty)
				{
					this.RefreshCosmeticInfoFromNetworkAux();
					this._isNetworkCosmeticsDirty = false;
				}
				if (this._isLocalCosmeticsDirty)
				{
					await this.UpdateUsedCosmeticsAux();
					this._isLocalCosmeticsDirty = false;
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

		private void InitializeAllCosmetics()
		{
			this._tauntCategoriesLookup.Clear();
			this._tauntCategoriesLookup.Add(MPArmoryCosmeticsVM.TauntCategoryFlag.All, new MPArmoryTauntCosmeticCategoryVM(MPArmoryCosmeticsVM.TauntCategoryFlag.All));
			foreach (object obj in Enum.GetValues(typeof(MPArmoryCosmeticsVM.TauntCategoryFlag)))
			{
				MPArmoryCosmeticsVM.TauntCategoryFlag tauntCategoryFlag = (MPArmoryCosmeticsVM.TauntCategoryFlag)obj;
				if (tauntCategoryFlag > MPArmoryCosmeticsVM.TauntCategoryFlag.None && tauntCategoryFlag < MPArmoryCosmeticsVM.TauntCategoryFlag.All)
				{
					this._tauntCategoriesLookup.Add(tauntCategoryFlag, new MPArmoryTauntCosmeticCategoryVM(tauntCategoryFlag));
				}
			}
			this._clothingCategoriesLookup.Clear();
			for (MPArmoryCosmeticsVM.ClothingCategory clothingCategory = MPArmoryCosmeticsVM.ClothingCategory.ClothingCategoriesBegin; clothingCategory < MPArmoryCosmeticsVM.ClothingCategory.ClothingCategoriesEnd; clothingCategory++)
			{
				this._clothingCategoriesLookup.Add(clothingCategory, new MPArmoryClothingCosmeticCategoryVM(clothingCategory));
			}
			this._allCosmetics = new List<MPArmoryCosmeticItemBaseVM>();
			List<CosmeticElement> list = CosmeticsManager.CosmeticElementsList.ToList<CosmeticElement>();
			for (int i = 0; i < list.Count; i++)
			{
				ClothingCosmeticElement clothingCosmeticElement;
				TauntCosmeticElement tauntCosmeticElement;
				if (list[i].Type == null && (clothingCosmeticElement = list[i] as ClothingCosmeticElement) != null)
				{
					MPArmoryCosmeticClothingItemVM mparmoryCosmeticClothingItemVM = new MPArmoryCosmeticClothingItemVM(clothingCosmeticElement, clothingCosmeticElement.Id);
					mparmoryCosmeticClothingItemVM.IsUnlocked = clothingCosmeticElement.IsFree;
					mparmoryCosmeticClothingItemVM.IsSelectable = true;
					this._allCosmetics.Add(mparmoryCosmeticClothingItemVM);
					this._cosmeticItemsLookup.Add(clothingCosmeticElement.Id, mparmoryCosmeticClothingItemVM);
					this._clothingCategoriesLookup[mparmoryCosmeticClothingItemVM.ClothingCategory].AvailableCosmetics.Add(mparmoryCosmeticClothingItemVM);
					this._clothingCategoriesLookup[MPArmoryCosmeticsVM.ClothingCategory.ClothingCategoriesBegin].AvailableCosmetics.Add(mparmoryCosmeticClothingItemVM);
				}
				else if (list[i].Type == 3 && (tauntCosmeticElement = list[i] as TauntCosmeticElement) != null)
				{
					MPArmoryCosmeticTauntItemVM mparmoryCosmeticTauntItemVM = new MPArmoryCosmeticTauntItemVM(tauntCosmeticElement.Id, tauntCosmeticElement, tauntCosmeticElement.Id);
					mparmoryCosmeticTauntItemVM.IsUnlocked = tauntCosmeticElement.IsFree;
					mparmoryCosmeticTauntItemVM.IsSelectable = true;
					this._allCosmetics.Add(mparmoryCosmeticTauntItemVM);
					this._cosmeticItemsLookup.Add(tauntCosmeticElement.Id, mparmoryCosmeticTauntItemVM);
					foreach (object obj2 in Enum.GetValues(typeof(MPArmoryCosmeticsVM.TauntCategoryFlag)))
					{
						MPArmoryCosmeticsVM.TauntCategoryFlag tauntCategoryFlag2 = (MPArmoryCosmeticsVM.TauntCategoryFlag)obj2;
						if (tauntCategoryFlag2 > MPArmoryCosmeticsVM.TauntCategoryFlag.None && tauntCategoryFlag2 <= MPArmoryCosmeticsVM.TauntCategoryFlag.All && (mparmoryCosmeticTauntItemVM.TauntCategory & tauntCategoryFlag2) != MPArmoryCosmeticsVM.TauntCategoryFlag.None)
						{
							this._tauntCategoriesLookup[tauntCategoryFlag2].AvailableCosmetics.Add(mparmoryCosmeticTauntItemVM);
						}
					}
				}
			}
			for (int j = 0; j < TauntCosmeticElement.MaxNumberOfTaunts; j++)
			{
				this.TauntSlots.Add(new MPArmoryCosmeticTauntSlotVM(j));
			}
		}

		private void OnClothingCosmeticCategorySelected(MPArmoryClothingCosmeticCategoryVM selectedCosmetic)
		{
			this.FilterClothingsByCategory(selectedCosmetic);
		}

		private void OnTauntCosmeticCategorySelected(MPArmoryTauntCosmeticCategoryVM selectedCosmetic)
		{
			this.FilterTauntsByCategory(selectedCosmetic);
		}

		public void RefreshAvailableCategoriesBy(CosmeticsManager.CosmeticType type)
		{
			this._currentCosmeticType = type;
			this.AvailableCategories.Clear();
			if (type == null)
			{
				using (Dictionary<MPArmoryCosmeticsVM.ClothingCategory, MPArmoryClothingCosmeticCategoryVM>.Enumerator enumerator = this._clothingCategoriesLookup.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						KeyValuePair<MPArmoryCosmeticsVM.ClothingCategory, MPArmoryClothingCosmeticCategoryVM> keyValuePair = enumerator.Current;
						this.AvailableCategories.Add(keyValuePair.Value);
					}
					goto IL_9B;
				}
			}
			if (type == 3)
			{
				foreach (KeyValuePair<MPArmoryCosmeticsVM.TauntCategoryFlag, MPArmoryTauntCosmeticCategoryVM> keyValuePair2 in this._tauntCategoriesLookup)
				{
					this.AvailableCategories.Add(keyValuePair2.Value);
				}
			}
			IL_9B:
			if (this.AvailableCategories.Count > 0)
			{
				if (type == null && this._currentClothingCategory != MPArmoryCosmeticsVM.ClothingCategory.Invalid)
				{
					this.FilterClothingsByCategory(this._clothingCategoriesLookup[this._currentClothingCategory]);
					return;
				}
				if (type == 3)
				{
					MPArmoryCosmeticsVM.TauntCategoryFlag tauntCategoryFlag = ((this._currentTauntCategory != MPArmoryCosmeticsVM.TauntCategoryFlag.None) ? this._currentTauntCategory : MPArmoryCosmeticsVM.TauntCategoryFlag.All);
					this.FilterTauntsByCategory(this._tauntCategoriesLookup[tauntCategoryFlag]);
				}
			}
		}

		public void RefreshPlayerData(PlayerData playerData)
		{
			this.Loot = playerData.Gold;
		}

		public void RefreshCosmeticInfoFromNetwork()
		{
			this._isNetworkCosmeticsDirty = true;
		}

		private void RefreshCosmeticInfoFromNetworkAux()
		{
			this._isRetrievingCosmeticData = true;
			this.IsLoading = true;
			this.HasCosmeticInfoReceived = true;
			this.IsLoading = false;
			string text = NetworkMain.GameClient.PlayerData.UserId.ToString();
			List<ValueTuple<string, int>> tauntIndicesForPlayer = TauntCosmeticElement.GetTauntIndicesForPlayer(text);
			this._ownedCosmetics = NetworkMain.GameClient.OwnedCosmetics.ToList<string>();
			this.RefreshTaunts(text, tauntIndicesForPlayer);
			IReadOnlyDictionary<string, List<string>> usedCosmetics = NetworkMain.GameClient.UsedCosmetics;
			this._usedCosmetics = new Dictionary<string, List<string>>();
			foreach (KeyValuePair<string, List<string>> keyValuePair in usedCosmetics)
			{
				this._usedCosmetics.Add(keyValuePair.Key, new List<string>());
				foreach (string text2 in usedCosmetics[keyValuePair.Key])
				{
					this._usedCosmetics[keyValuePair.Key].Add(text2);
				}
			}
			this.RefreshSelectedClass(this._selectedClass, this._getSelectedPerks());
			this._isRetrievingCosmeticData = false;
		}

		private async Task<bool> UpdateUsedCosmeticsAux()
		{
			this._isSendingCosmeticData = true;
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
			foreach (KeyValuePair<string, List<ValueTuple<string, bool>>> keyValuePair3 in dictionary)
			{
				List<ItemObject.ItemTypeEnum> list = new List<ItemObject.ItemTypeEnum>();
				foreach (ValueTuple<string, bool> valueTuple in keyValuePair3.Value)
				{
					string item = valueTuple.Item1;
					MPArmoryCosmeticItemBaseVM mparmoryCosmeticItemBaseVM;
					MPArmoryCosmeticClothingItemVM mparmoryCosmeticClothingItemVM;
					if (valueTuple.Item2 && this._cosmeticItemsLookup.TryGetValue(item, out mparmoryCosmeticItemBaseVM) && (mparmoryCosmeticClothingItemVM = mparmoryCosmeticItemBaseVM as MPArmoryCosmeticClothingItemVM) != null)
					{
						ItemObject.ItemTypeEnum itemType = mparmoryCosmeticClothingItemVM.EquipmentElement.Item.ItemType;
						list.Add(itemType);
					}
				}
			}
			List<ValueTuple<string, int>> list2 = new List<ValueTuple<string, int>>();
			for (int i = 0; i < this.TauntSlots.Count; i++)
			{
				MPArmoryCosmeticTauntItemVM assignedTauntItem = this.TauntSlots[i].AssignedTauntItem;
				if (assignedTauntItem != null)
				{
					ValueTuple<string, int> valueTuple2 = new ValueTuple<string, int>(assignedTauntItem.TauntID, i);
					list2.Add(valueTuple2);
				}
			}
			TauntCosmeticElement.SetTauntIndicesForPlayer(NetworkMain.GameClient.PlayerData.UserId.ToString(), list2);
			bool flag = await NetworkMain.GameClient.UpdateUsedCosmeticItems(dictionary);
			this._isSendingCosmeticData = false;
			return flag;
		}

		public void RefreshSelectedClass(MultiplayerClassDivisions.MPHeroClass selectedClass, List<IReadOnlyPerkObject> selectedPerks)
		{
			this._selectedClass = selectedClass;
			if (this._selectedClass == null)
			{
				return;
			}
			this._selectedClassDefaultEquipment = this._selectedClass.HeroCharacter.Equipment.Clone(false);
			if (selectedPerks != null)
			{
				MPArmoryVM.ApplyPerkEffectsToEquipment(ref this._selectedClassDefaultEquipment, selectedPerks);
			}
			this._selectedTroopID = this._selectedClass.StringId;
			MPArmoryCosmeticCategoryBaseVM activeCategory = this.ActiveCategory;
			if (activeCategory != null)
			{
				activeCategory.Sort(this._currentItemComparer);
			}
			if (this._ownedCosmetics != null)
			{
				using (List<string>.Enumerator enumerator = this._ownedCosmetics.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						string ownedCosmeticID = enumerator.Current;
						MPArmoryCosmeticCategoryBaseVM activeCategory2 = this.ActiveCategory;
						MPArmoryCosmeticItemBaseVM mparmoryCosmeticItemBaseVM = ((activeCategory2 != null) ? activeCategory2.AvailableCosmetics.FirstOrDefault((MPArmoryCosmeticItemBaseVM c) => c.CosmeticID == ownedCosmeticID) : null);
						if (mparmoryCosmeticItemBaseVM != null)
						{
							mparmoryCosmeticItemBaseVM.IsUnlocked = true;
						}
					}
				}
			}
			this.RefreshFilters();
		}

		private void EquipItemOnHeroPreview(MPArmoryCosmeticItemBaseVM itemVM)
		{
			if (itemVM == null)
			{
				Debug.FailedAssert("Previewing null item", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade.Multiplayer.ViewModelCollection\\Lobby\\Armory\\MPArmoryCosmeticsVM.cs", "EquipItemOnHeroPreview", 505);
				return;
			}
			Action<MPArmoryCosmeticItemBaseVM> onCosmeticPreview = MPArmoryCosmeticsVM.OnCosmeticPreview;
			if (onCosmeticPreview == null)
			{
				return;
			}
			onCosmeticPreview(itemVM);
		}

		private void OnCosmeticEquipRequested(MPArmoryCosmeticItemBaseVM cosmeticItemVM)
		{
			if (cosmeticItemVM.CosmeticType == null)
			{
				this.OnItemEquipRequested((MPArmoryCosmeticClothingItemVM)cosmeticItemVM);
				return;
			}
			if (cosmeticItemVM.CosmeticType == 3)
			{
				this.OnTauntEquipRequested((MPArmoryCosmeticTauntItemVM)cosmeticItemVM);
			}
		}

		private void OnItemEquipRequested(MPArmoryCosmeticClothingItemVM itemVM)
		{
			if (itemVM.IsUsed && !itemVM.Cosmetic.IsFree && this.ActiveCategory != null && this.ActiveCategory.CosmeticType == null && itemVM.ClothingCosmeticElement.ReplaceItemsId.Count > 0 && this._selectedClassDefaultEquipment != null)
			{
				for (int i = 0; i < itemVM.ClothingCosmeticElement.ReplaceItemsId.Count; i++)
				{
					string replacedItemId = itemVM.ClothingCosmeticElement.ReplaceItemsId[i];
					Func<MPArmoryCosmeticItemBaseVM, bool> <>9__1;
					for (EquipmentIndex equipmentIndex = 5; equipmentIndex < 10; equipmentIndex++)
					{
						ItemObject item = this._selectedClassDefaultEquipment[equipmentIndex].Item;
						if (((item != null) ? item.StringId : null) == replacedItemId)
						{
							IEnumerable<MPArmoryCosmeticItemBaseVM> availableCosmetics = this.ActiveCategory.AvailableCosmetics;
							Func<MPArmoryCosmeticItemBaseVM, bool> func;
							if ((func = <>9__1) == null)
							{
								func = (<>9__1 = (MPArmoryCosmeticItemBaseVM c) => c.Cosmetic.Id == replacedItemId);
							}
							MPArmoryCosmeticClothingItemVM mparmoryCosmeticClothingItemVM = availableCosmetics.FirstOrDefault(func) as MPArmoryCosmeticClothingItemVM;
							if (mparmoryCosmeticClothingItemVM != null)
							{
								this.OnClothingItemEquipped(mparmoryCosmeticClothingItemVM, true);
								this._isLocalCosmeticsDirty = true;
								return;
							}
						}
					}
				}
			}
			if (itemVM.ClothingCosmeticElement.ReplaceItemless.Any((Tuple<string, string> r) => r.Item1 == this._selectedClass.StringId))
			{
				if (itemVM.IsUsed)
				{
					itemVM.IsUsed = false;
					Dictionary<string, List<string>> usedCosmetics = this._usedCosmetics;
					if (usedCosmetics != null)
					{
						usedCosmetics[this._selectedTroopID].Remove(itemVM.CosmeticID);
					}
					Action<MPArmoryCosmeticItemBaseVM> onRemoveCosmeticFromPreview = MPArmoryCosmeticsVM.OnRemoveCosmeticFromPreview;
					if (onRemoveCosmeticFromPreview != null)
					{
						onRemoveCosmeticFromPreview(itemVM);
					}
				}
				else
				{
					itemVM.ActionText = itemVM.UnequipText;
					this.OnClothingItemEquipped(itemVM, true);
				}
			}
			else
			{
				this.OnClothingItemEquipped(itemVM, true);
			}
			this._isLocalCosmeticsDirty = true;
		}

		private void OnClothingItemEquipped(MPArmoryCosmeticClothingItemVM itemVM, bool forceRemove = true)
		{
			this.EquipItemOnHeroPreview(itemVM);
			if (!this._usedCosmetics.ContainsKey(this._selectedTroopID))
			{
				this._usedCosmetics.Add(this._selectedTroopID, new List<string>());
			}
			if (itemVM.CosmeticID != string.Empty && !this._usedCosmetics[this._selectedTroopID].Contains(itemVM.CosmeticID))
			{
				this._usedCosmetics[this._selectedTroopID].Add(itemVM.CosmeticID);
			}
			foreach (MPArmoryCosmeticItemBaseVM mparmoryCosmeticItemBaseVM in this.ActiveCategory.AvailableCosmetics)
			{
				if (((MPArmoryCosmeticClothingItemVM)mparmoryCosmeticItemBaseVM).EquipmentElement.Item.ItemType == itemVM.EquipmentElement.Item.ItemType)
				{
					mparmoryCosmeticItemBaseVM.IsUsed = false;
					if (itemVM.Cosmetic.Id != mparmoryCosmeticItemBaseVM.Cosmetic.Id && forceRemove)
					{
						List<string> list = this._usedCosmetics[this._selectedTroopID];
						if (list != null)
						{
							list.Remove(mparmoryCosmeticItemBaseVM.CosmeticID);
						}
					}
				}
			}
			itemVM.IsUsed = true;
			if (this.ActiveCategory != null)
			{
				this.UpdateKeyBindingsForCategory(this.ActiveCategory);
			}
		}

		public void ClearTauntSelections()
		{
			if (this.SelectedTauntItem == null && this.SelectedTauntSlot == null)
			{
				return;
			}
			this.OnTauntEquipRequested(null);
			this.OnTauntSlotSelected(null);
			foreach (MPArmoryCosmeticTauntSlotVM mparmoryCosmeticTauntSlotVM in this.TauntSlots)
			{
				mparmoryCosmeticTauntSlotVM.IsAcceptingTaunts = false;
				mparmoryCosmeticTauntSlotVM.IsFocused = false;
			}
		}

		private void OnTauntEquipRequested(MPArmoryCosmeticTauntItemVM tauntItem)
		{
			if (this.SelectedTauntItem != null)
			{
				if (this.SelectedTauntItem == tauntItem)
				{
					this.ClearTauntSelections();
					return;
				}
				this.SelectedTauntItem.IsSelected = false;
			}
			this.SelectedTauntItem = tauntItem;
			if (this.SelectedTauntItem != null)
			{
				MPArmoryCosmeticTauntSlotVM mparmoryCosmeticTauntSlotVM = null;
				for (int i = 0; i < this.TauntSlots.Count; i++)
				{
					MPArmoryCosmeticTauntItemVM assignedTauntItem = this.TauntSlots[i].AssignedTauntItem;
					if (((assignedTauntItem != null) ? assignedTauntItem.CosmeticID : null) == tauntItem.CosmeticID)
					{
						mparmoryCosmeticTauntSlotVM = this.TauntSlots[i];
						break;
					}
				}
				if (mparmoryCosmeticTauntSlotVM != null)
				{
					this.SelectedTauntItem = null;
					mparmoryCosmeticTauntSlotVM.AssignTauntItem(null, false);
					this.ClearTauntSelections();
					this._isLocalCosmeticsDirty = true;
					return;
				}
				this.SelectedTauntItem.IsSelected = true;
				this.SelectedTauntItem.ActionText = this.SelectedTauntItem.CancelEquipText;
				using (IEnumerator<MPArmoryCosmeticItemBaseVM> enumerator = this.ActiveCategory.AvailableCosmetics.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						MPArmoryCosmeticItemBaseVM mparmoryCosmeticItemBaseVM = enumerator.Current;
						mparmoryCosmeticItemBaseVM.IsSelectable = mparmoryCosmeticItemBaseVM == this.SelectedTauntItem;
					}
					goto IL_137;
				}
			}
			foreach (MPArmoryCosmeticItemBaseVM mparmoryCosmeticItemBaseVM2 in this.ActiveCategory.AvailableCosmetics)
			{
				mparmoryCosmeticItemBaseVM2.IsSelectable = true;
			}
			IL_137:
			foreach (MPArmoryCosmeticTauntSlotVM mparmoryCosmeticTauntSlotVM2 in this.TauntSlots)
			{
				mparmoryCosmeticTauntSlotVM2.IsAcceptingTaunts = mparmoryCosmeticTauntSlotVM2.AssignedTauntItem != tauntItem;
			}
			Action onTauntAssignmentRefresh = MPArmoryCosmeticsVM.OnTauntAssignmentRefresh;
			if (onTauntAssignmentRefresh == null)
			{
				return;
			}
			onTauntAssignmentRefresh();
		}

		private void OnTauntSlotFocusChanged(MPArmoryCosmeticTauntSlotVM changedSlot, bool isFocused)
		{
			foreach (MPArmoryCosmeticTauntSlotVM mparmoryCosmeticTauntSlotVM in this.TauntSlots)
			{
				mparmoryCosmeticTauntSlotVM.IsFocused = isFocused && changedSlot == mparmoryCosmeticTauntSlotVM;
				if (mparmoryCosmeticTauntSlotVM.IsAcceptingTaunts)
				{
					mparmoryCosmeticTauntSlotVM.EmptySlotKeyVisual.SetForcedVisibility(new bool?(false));
					mparmoryCosmeticTauntSlotVM.SelectKeyVisual.SetForcedVisibility(null);
				}
				else if (mparmoryCosmeticTauntSlotVM.AssignedTauntItem != null)
				{
					mparmoryCosmeticTauntSlotVM.EmptySlotKeyVisual.SetForcedVisibility(null);
					mparmoryCosmeticTauntSlotVM.SelectKeyVisual.SetForcedVisibility(new bool?(false));
				}
				else
				{
					mparmoryCosmeticTauntSlotVM.EmptySlotKeyVisual.SetForcedVisibility(new bool?(false));
				}
				bool? flag = ((!mparmoryCosmeticTauntSlotVM.IsAcceptingTaunts && mparmoryCosmeticTauntSlotVM.AssignedTauntItem != null) ? null : new bool?(false));
				bool? flag2 = ((mparmoryCosmeticTauntSlotVM.AssignedTauntItem != null || mparmoryCosmeticTauntSlotVM.IsAcceptingTaunts) ? null : new bool?(false));
				InputKeyItemVM emptySlotKeyVisual = mparmoryCosmeticTauntSlotVM.EmptySlotKeyVisual;
				if (emptySlotKeyVisual != null)
				{
					emptySlotKeyVisual.SetForcedVisibility(flag);
				}
				InputKeyItemVM selectKeyVisual = mparmoryCosmeticTauntSlotVM.SelectKeyVisual;
				if (selectKeyVisual != null)
				{
					selectKeyVisual.SetForcedVisibility(flag2);
				}
			}
		}

		private void OnTauntSlotPreview(MPArmoryCosmeticTauntSlotVM previewSlot)
		{
			if (previewSlot != null)
			{
				MPArmoryCosmeticTauntItemVM assignedTauntItem = previewSlot.AssignedTauntItem;
				if (assignedTauntItem == null)
				{
					return;
				}
				assignedTauntItem.ExecutePreview();
			}
		}

		private void OnTauntSlotSelected(MPArmoryCosmeticTauntSlotVM selectedSlot)
		{
			if (this.SelectedTauntSlot == null && this.SelectedTauntItem == null && selectedSlot != null && selectedSlot.IsEmpty)
			{
				return;
			}
			MPArmoryCosmeticTauntSlotVM selectedTauntSlot = this.SelectedTauntSlot;
			this.SelectedTauntSlot = selectedSlot;
			if (selectedTauntSlot != null)
			{
				selectedTauntSlot.IsSelected = false;
			}
			if (this.SelectedTauntSlot != null)
			{
				this.SelectedTauntSlot.IsSelected = true;
			}
			if (((selectedSlot != null) ? selectedSlot.AssignedTauntItem : null) != null)
			{
				bool flag = false;
				for (int i = 0; i < this.ActiveCategory.AvailableCosmetics.Count; i++)
				{
					if (this.ActiveCategory.AvailableCosmetics[i] == selectedSlot.AssignedTauntItem)
					{
						flag = true;
						break;
					}
				}
				MPArmoryTauntCosmeticCategoryVM mparmoryTauntCosmeticCategoryVM;
				if (!flag && this._tauntCategoriesLookup.TryGetValue(MPArmoryCosmeticsVM.TauntCategoryFlag.All, out mparmoryTauntCosmeticCategoryVM))
				{
					this.FilterTauntsByCategory(mparmoryTauntCosmeticCategoryVM);
				}
			}
			foreach (MPArmoryCosmeticItemBaseVM mparmoryCosmeticItemBaseVM in this.ActiveCategory.AvailableCosmetics)
			{
				mparmoryCosmeticItemBaseVM.IsSelectable = selectedSlot == null || mparmoryCosmeticItemBaseVM == ((selectedSlot != null) ? selectedSlot.AssignedTauntItem : null);
			}
			if (this.SelectedTauntItem == null)
			{
				MPArmoryCosmeticTauntSlotVM selectedTauntSlot2 = this.SelectedTauntSlot;
				if (selectedTauntSlot2 != null && !selectedTauntSlot2.IsEmpty)
				{
					foreach (MPArmoryCosmeticTauntSlotVM mparmoryCosmeticTauntSlotVM in this.TauntSlots)
					{
						mparmoryCosmeticTauntSlotVM.IsAcceptingTaunts = mparmoryCosmeticTauntSlotVM != selectedSlot;
					}
				}
			}
			if (this.SelectedTauntSlot != null)
			{
				bool flag2 = false;
				if (this.SelectedTauntItem != null && this.SelectedTauntSlot.AssignedTauntItem != this.SelectedTauntItem)
				{
					MPArmoryCosmeticTauntSlotVM mparmoryCosmeticTauntSlotVM2 = null;
					for (int j = 0; j < this.TauntSlots.Count; j++)
					{
						if (this.TauntSlots[j].AssignedTauntItem == this.SelectedTauntItem)
						{
							mparmoryCosmeticTauntSlotVM2 = this.TauntSlots[j];
							break;
						}
					}
					if (mparmoryCosmeticTauntSlotVM2 != null)
					{
						MPArmoryCosmeticTauntItemVM assignedTauntItem = this.SelectedTauntSlot.AssignedTauntItem;
						MPArmoryCosmeticTauntItemVM assignedTauntItem2 = mparmoryCosmeticTauntSlotVM2.AssignedTauntItem;
						this.SelectedTauntSlot.AssignTauntItem(assignedTauntItem2, true);
						mparmoryCosmeticTauntSlotVM2.AssignTauntItem(assignedTauntItem, true);
					}
					else
					{
						this.SelectedTauntSlot.AssignTauntItem(this.SelectedTauntItem, false);
					}
					flag2 = true;
					this.ClearTauntSelections();
				}
				else if (selectedTauntSlot != null && !selectedTauntSlot.IsEmpty && this.SelectedTauntSlot != selectedTauntSlot)
				{
					MPArmoryCosmeticTauntItemVM assignedTauntItem3 = selectedTauntSlot.AssignedTauntItem;
					MPArmoryCosmeticTauntItemVM assignedTauntItem4 = this.SelectedTauntSlot.AssignedTauntItem;
					this.SelectedTauntSlot.AssignTauntItem(assignedTauntItem3, true);
					selectedTauntSlot.AssignTauntItem(assignedTauntItem4, true);
					flag2 = true;
					this.ClearTauntSelections();
				}
				if (flag2)
				{
					this._isLocalCosmeticsDirty = true;
				}
			}
			Action onTauntAssignmentRefresh = MPArmoryCosmeticsVM.OnTauntAssignmentRefresh;
			if (onTauntAssignmentRefresh == null)
			{
				return;
			}
			onTauntAssignmentRefresh();
		}

		private void OnTauntItemEquipped(MPArmoryCosmeticTauntSlotVM equippedSlot, MPArmoryCosmeticTauntItemVM previousTauntItem, bool isSwapping)
		{
			NetworkMain.GameClient.PlayerData.UserId.ToString();
			for (int i = 0; i < this.TauntSlots.Count; i++)
			{
				MPArmoryCosmeticTauntItemVM assignedTauntItem = this.TauntSlots[i].AssignedTauntItem;
				if (assignedTauntItem != null && !assignedTauntItem.IsUnlocked)
				{
					Debug.FailedAssert("Assigned a taunt without ownership: " + assignedTauntItem.TauntID, "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade.Multiplayer.ViewModelCollection\\Lobby\\Armory\\MPArmoryCosmeticsVM.cs", "OnTauntItemEquipped", 850);
				}
			}
			this._isLocalCosmeticsDirty = true;
		}

		public void OnItemObtained(string cosmeticID, int finalLoot)
		{
			this._ownedCosmetics.Add(cosmeticID);
			this.RefreshCosmeticInfoFromNetwork();
			this.Loot = finalLoot;
		}

		private void OnSortCategoryUpdated(SelectorVM<SelectorItemVM> selector)
		{
			if (this.SortCategories.SelectedIndex == -1)
			{
				this.SortCategories.SelectedIndex = 0;
			}
			this._currentItemComparer = this._itemComparers[selector.SelectedIndex];
			MPArmoryCosmeticCategoryBaseVM activeCategory = this.ActiveCategory;
			if (activeCategory == null)
			{
				return;
			}
			activeCategory.Sort(this._currentItemComparer);
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
			MPArmoryCosmeticCategoryBaseVM activeCategory = this.ActiveCategory;
			if (activeCategory == null)
			{
				return;
			}
			activeCategory.Sort(this._currentItemComparer);
		}

		private void RefreshFilters()
		{
			MPArmoryClothingCosmeticCategoryVM mparmoryClothingCosmeticCategoryVM;
			if (this._currentCosmeticType == null && this._clothingCategoriesLookup.TryGetValue(this._currentClothingCategory, out mparmoryClothingCosmeticCategoryVM))
			{
				this.FilterClothingsByCategory(mparmoryClothingCosmeticCategoryVM);
				return;
			}
			MPArmoryTauntCosmeticCategoryVM mparmoryTauntCosmeticCategoryVM;
			if (this._currentCosmeticType == 3 && this._tauntCategoriesLookup.TryGetValue(this._currentTauntCategory, out mparmoryTauntCosmeticCategoryVM))
			{
				this.FilterTauntsByCategory(mparmoryTauntCosmeticCategoryVM);
			}
		}

		private void FilterClothingsByCategory(MPArmoryClothingCosmeticCategoryVM clothingCategory)
		{
			if (this._currentCosmeticType != null)
			{
				this.RefreshAvailableCategoriesBy(0);
				return;
			}
			if (clothingCategory == null)
			{
				Debug.FailedAssert("Trying to filter by null clothing category", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade.Multiplayer.ViewModelCollection\\Lobby\\Armory\\MPArmoryCosmeticsVM.cs", "FilterClothingsByCategory", 913);
				return;
			}
			this._currentClothingCategory = clothingCategory.ClothingCategory;
			foreach (KeyValuePair<MPArmoryCosmeticsVM.ClothingCategory, MPArmoryClothingCosmeticCategoryVM> keyValuePair in this._clothingCategoriesLookup)
			{
				keyValuePair.Value.IsSelected = false;
			}
			clothingCategory.SetDefaultEquipments(this._selectedClassDefaultEquipment);
			this.ActiveCategory = clothingCategory;
			if (this._selectedClass != null)
			{
				foreach (MPArmoryCosmeticItemBaseVM mparmoryCosmeticItemBaseVM in this._allCosmetics)
				{
					if (mparmoryCosmeticItemBaseVM.CosmeticType == null)
					{
						clothingCategory.ReplaceCosmeticWithDefaultItem((MPArmoryCosmeticClothingItemVM)mparmoryCosmeticItemBaseVM, clothingCategory.ClothingCategory, this._selectedClass, this._ownedCosmetics);
					}
				}
			}
			this.ActiveCategory.Sort(this._currentItemComparer);
			this.RefreshEquipment();
			if (this.ActiveCategory != null)
			{
				this.ActiveCategory.IsSelected = true;
				this.UpdateKeyBindingsForCategory(this.ActiveCategory);
			}
		}

		private void FilterTauntsByCategory(MPArmoryTauntCosmeticCategoryVM tauntCategory)
		{
			if (this._currentCosmeticType != 3)
			{
				this.RefreshAvailableCategoriesBy(3);
			}
			this._currentTauntCategory = tauntCategory.TauntCategory;
			foreach (KeyValuePair<MPArmoryCosmeticsVM.TauntCategoryFlag, MPArmoryTauntCosmeticCategoryVM> keyValuePair in this._tauntCategoriesLookup)
			{
				keyValuePair.Value.IsSelected = false;
			}
			this.ActiveCategory = tauntCategory;
			if (this.ActiveCategory != null)
			{
				this.ActiveCategory.IsSelected = true;
				this.UpdateKeyBindingsForCategory(this.ActiveCategory);
			}
			this.ActiveCategory.Sort(this._currentItemComparer);
		}

		private void RefreshEquipment()
		{
			Dictionary<EquipmentIndex, bool> dictionary = new Dictionary<EquipmentIndex, bool>();
			for (EquipmentIndex equipmentIndex = 5; equipmentIndex < 10; equipmentIndex++)
			{
				dictionary.Add(equipmentIndex, false);
			}
			List<EquipmentElement> list = new List<EquipmentElement>();
			using (IEnumerator<MPArmoryCosmeticItemBaseVM> enumerator = this.ActiveCategory.AvailableCosmetics.Where((MPArmoryCosmeticItemBaseVM c) => c.Cosmetic.Rarity == 0).GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					MPArmoryCosmeticClothingItemVM mparmoryCosmeticClothingItemVM;
					if ((mparmoryCosmeticClothingItemVM = enumerator.Current as MPArmoryCosmeticClothingItemVM) != null)
					{
						this.OnClothingItemEquipped(mparmoryCosmeticClothingItemVM, false);
						dictionary[mparmoryCosmeticClothingItemVM.EquipmentElement.Item.GetCosmeticEquipmentIndex()] = true;
						list.Add(mparmoryCosmeticClothingItemVM.EquipmentElement);
					}
				}
			}
			if (!string.IsNullOrEmpty(this._selectedTroopID))
			{
				Dictionary<string, List<string>> usedCosmetics = this._usedCosmetics;
				if (usedCosmetics != null && usedCosmetics.ContainsKey(this._selectedTroopID))
				{
					Dictionary<string, List<string>> dictionary2 = new Dictionary<string, List<string>>();
					foreach (string text in this._usedCosmetics.Keys)
					{
						List<string> list2 = new List<string>();
						foreach (string text2 in this._usedCosmetics[text])
						{
							list2.Add(text2);
						}
						dictionary2.Add(text, list2);
					}
					using (List<string>.Enumerator enumerator3 = dictionary2[this._selectedTroopID].GetEnumerator())
					{
						while (enumerator3.MoveNext())
						{
							string cosmeticID = enumerator3.Current;
							MPArmoryCosmeticClothingItemVM cosmeticItem = (MPArmoryCosmeticClothingItemVM)this._allCosmetics.First((MPArmoryCosmeticItemBaseVM c) => c.CosmeticID == cosmeticID);
							if (cosmeticItem != null)
							{
								EquipmentIndex cosmeticEquipmentIndex = cosmeticItem.EquipmentElement.Item.GetCosmeticEquipmentIndex();
								if (!Extensions.IsEmpty<Tuple<string, string>>((cosmeticItem.Cosmetic as ClothingCosmeticElement).ReplaceItemless) || !this._selectedClassDefaultEquipment[cosmeticEquipmentIndex].IsEmpty)
								{
									EquipmentElement equipmentElement = list.FirstOrDefault((EquipmentElement i) => i.Item.GetCosmeticEquipmentIndex() == cosmeticItem.EquipmentElement.Item.GetCosmeticEquipmentIndex());
									if (!equipmentElement.IsEmpty)
									{
										list.Remove(equipmentElement);
										list.Add(cosmeticItem.EquipmentElement);
									}
									this.OnClothingItemEquipped(cosmeticItem, true);
									dictionary[cosmeticEquipmentIndex] = true;
								}
							}
						}
					}
				}
			}
			foreach (EquipmentIndex equipmentIndex2 in dictionary.Keys)
			{
				if (!dictionary[equipmentIndex2])
				{
					MPArmoryClothingCosmeticCategoryVM mparmoryClothingCosmeticCategoryVM = (MPArmoryClothingCosmeticCategoryVM)this.ActiveCategory;
					if (mparmoryClothingCosmeticCategoryVM != null)
					{
						mparmoryClothingCosmeticCategoryVM.OnEquipmentRefreshed(equipmentIndex2);
					}
				}
			}
			Action<List<EquipmentElement>> onEquipmentRefreshed = MPArmoryCosmeticsVM.OnEquipmentRefreshed;
			if (onEquipmentRefreshed == null)
			{
				return;
			}
			onEquipmentRefreshed(list);
		}

		private void RefreshTaunts(string playerId, List<ValueTuple<string, int>> tauntIndices)
		{
			if (tauntIndices == null)
			{
				tauntIndices = new List<ValueTuple<string, int>>();
				foreach (MPArmoryCosmeticItemBaseVM mparmoryCosmeticItemBaseVM in (from c in this._tauntCategoriesLookup.SelectMany((KeyValuePair<MPArmoryCosmeticsVM.TauntCategoryFlag, MPArmoryTauntCosmeticCategoryVM> c) => c.Value.AvailableCosmetics)
					where c.Cosmetic.IsFree
					select c).Distinct<MPArmoryCosmeticItemBaseVM>())
				{
					ValueTuple<string, int> valueTuple = new ValueTuple<string, int>(mparmoryCosmeticItemBaseVM.CosmeticID, tauntIndices.Count);
					tauntIndices.Add(valueTuple);
				}
			}
			foreach (KeyValuePair<MPArmoryCosmeticsVM.TauntCategoryFlag, MPArmoryTauntCosmeticCategoryVM> keyValuePair in this._tauntCategoriesLookup)
			{
				foreach (MPArmoryCosmeticItemBaseVM mparmoryCosmeticItemBaseVM2 in keyValuePair.Value.AvailableCosmetics)
				{
					mparmoryCosmeticItemBaseVM2.IsUnlocked = mparmoryCosmeticItemBaseVM2.Cosmetic.IsFree || this._ownedCosmetics.Contains(mparmoryCosmeticItemBaseVM2.CosmeticID);
				}
			}
			for (int i = 0; i < this.TauntSlots.Count; i++)
			{
				this.TauntSlots[i].AssignTauntItem(null, false);
			}
			for (int j = 0; j < tauntIndices.Count; j++)
			{
				string item = tauntIndices[j].Item1;
				int item2 = tauntIndices[j].Item2;
				MPArmoryCosmeticItemBaseVM mparmoryCosmeticItemBaseVM3;
				MPArmoryCosmeticTauntItemVM mparmoryCosmeticTauntItemVM;
				if (this._cosmeticItemsLookup.TryGetValue(item, out mparmoryCosmeticItemBaseVM3) && (mparmoryCosmeticTauntItemVM = mparmoryCosmeticItemBaseVM3 as MPArmoryCosmeticTauntItemVM) != null)
				{
					if (!mparmoryCosmeticTauntItemVM.IsUnlocked)
					{
						Debug.FailedAssert("Trying to add non-owned cosmetic to taunt slot: " + mparmoryCosmeticTauntItemVM.TauntID, "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade.Multiplayer.ViewModelCollection\\Lobby\\Armory\\MPArmoryCosmeticsVM.cs", "RefreshTaunts", 1090);
					}
					else if (item2 >= 0 && item2 < this.TauntSlots.Count)
					{
						this.TauntSlots[item2].AssignTauntItem(mparmoryCosmeticTauntItemVM, false);
					}
				}
			}
		}

		private void UpdateTauntAssignmentState()
		{
			this.IsTauntAssignmentActive = this.SelectedTauntItem != null || this.SelectedTauntSlot != null;
		}

		private void ExecuteRefreshCosmeticInfo()
		{
			this.RefreshCosmeticInfoFromNetwork();
		}

		private void ExecuteResetPreview()
		{
			this.RefreshSelectedClass(this._selectedClass, this._getSelectedPerks());
		}

		public void RefreshKeyBindings(HotKey actionKey, HotKey previewKey)
		{
			this.ActionInputKey = InputKeyItemVM.CreateFromHotKey(actionKey, false);
			this.PreviewInputKey = InputKeyItemVM.CreateFromHotKey(previewKey, false);
			for (int i = 0; i < this.AvailableCategories.Count; i++)
			{
				this.UpdateKeyBindingsForCategory(this.AvailableCategories[i]);
			}
		}

		private void UpdateKeyBindingsForCategory(MPArmoryCosmeticCategoryBaseVM categoryVM)
		{
			if (this.ActionInputKey != null && this.PreviewInputKey != null)
			{
				for (int i = 0; i < categoryVM.AvailableCosmetics.Count; i++)
				{
					categoryVM.AvailableCosmetics[i].RefreshKeyBindings(this.ActionInputKey.HotKey, this.PreviewInputKey.HotKey);
				}
			}
		}

		[DataSourceProperty]
		public InputKeyItemVM ActionInputKey
		{
			get
			{
				return this._actionInputKey;
			}
			set
			{
				if (value != this._actionInputKey)
				{
					this._actionInputKey = value;
					base.OnPropertyChangedWithValue<InputKeyItemVM>(value, "ActionInputKey");
				}
			}
		}

		[DataSourceProperty]
		public InputKeyItemVM PreviewInputKey
		{
			get
			{
				return this._previewInputKey;
			}
			set
			{
				if (value != this._previewInputKey)
				{
					this._previewInputKey = value;
					base.OnPropertyChangedWithValue<InputKeyItemVM>(value, "PreviewInputKey");
				}
			}
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
		public bool IsManagingTaunts
		{
			get
			{
				return this._isManagingTaunts;
			}
			set
			{
				if (value != this._isManagingTaunts)
				{
					this._isManagingTaunts = value;
					base.OnPropertyChangedWithValue(value, "IsManagingTaunts");
				}
			}
		}

		[DataSourceProperty]
		public bool IsTauntAssignmentActive
		{
			get
			{
				return this._isTauntAssignmentActive;
			}
			set
			{
				if (value != this._isTauntAssignmentActive)
				{
					this._isTauntAssignmentActive = value;
					base.OnPropertyChangedWithValue(value, "IsTauntAssignmentActive");
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
		public MPArmoryCosmeticCategoryBaseVM ActiveCategory
		{
			get
			{
				return this._activeCategory;
			}
			set
			{
				if (value != this._activeCategory)
				{
					this._activeCategory = value;
					base.OnPropertyChangedWithValue<MPArmoryCosmeticCategoryBaseVM>(value, "ActiveCategory");
				}
			}
		}

		[DataSourceProperty]
		public MPArmoryCosmeticTauntSlotVM SelectedTauntSlot
		{
			get
			{
				return this._selectedTauntSlot;
			}
			set
			{
				if (value != this._selectedTauntSlot)
				{
					this._selectedTauntSlot = value;
					base.OnPropertyChangedWithValue<MPArmoryCosmeticTauntSlotVM>(value, "SelectedTauntSlot");
					this.UpdateTauntAssignmentState();
				}
			}
		}

		[DataSourceProperty]
		public MPArmoryCosmeticTauntItemVM SelectedTauntItem
		{
			get
			{
				return this._selectedTauntItem;
			}
			set
			{
				if (value != this._selectedTauntItem)
				{
					this._selectedTauntItem = value;
					base.OnPropertyChangedWithValue<MPArmoryCosmeticTauntItemVM>(value, "SelectedTauntItem");
					this.UpdateTauntAssignmentState();
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
		public MBBindingList<MPArmoryCosmeticTauntSlotVM> TauntSlots
		{
			get
			{
				return this._tauntSlots;
			}
			set
			{
				if (value != this._tauntSlots)
				{
					this._tauntSlots = value;
					base.OnPropertyChangedWithValue<MBBindingList<MPArmoryCosmeticTauntSlotVM>>(value, "TauntSlots");
				}
			}
		}

		[DataSourceProperty]
		public MBBindingList<MPArmoryCosmeticCategoryBaseVM> AvailableCategories
		{
			get
			{
				return this._availableCategories;
			}
			set
			{
				if (value != this._availableCategories)
				{
					this._availableCategories = value;
					base.OnPropertyChangedWithValue<MBBindingList<MPArmoryCosmeticCategoryBaseVM>>(value, "AvailableCategories");
				}
			}
		}

		private readonly Func<List<IReadOnlyPerkObject>> _getSelectedPerks;

		private List<MPArmoryCosmeticItemBaseVM> _allCosmetics;

		private List<string> _ownedCosmetics;

		private Dictionary<string, List<string>> _usedCosmetics;

		private Equipment _selectedClassDefaultEquipment;

		private MPArmoryCosmeticsVM.CosmeticItemComparer _currentItemComparer;

		private List<MPArmoryCosmeticsVM.CosmeticItemComparer> _itemComparers;

		private Dictionary<MPArmoryCosmeticsVM.ClothingCategory, MPArmoryClothingCosmeticCategoryVM> _clothingCategoriesLookup;

		private Dictionary<MPArmoryCosmeticsVM.TauntCategoryFlag, MPArmoryTauntCosmeticCategoryVM> _tauntCategoriesLookup;

		private Dictionary<string, MPArmoryCosmeticItemBaseVM> _cosmeticItemsLookup;

		private MultiplayerClassDivisions.MPHeroClass _selectedClass;

		private string _selectedTroopID;

		private bool _isLocalCosmeticsDirty;

		private bool _isNetworkCosmeticsDirty;

		private bool _isSendingCosmeticData;

		private bool _isRetrievingCosmeticData;

		private CosmeticsManager.CosmeticType _currentCosmeticType;

		private MPArmoryCosmeticsVM.ClothingCategory _currentClothingCategory;

		private MPArmoryCosmeticsVM.TauntCategoryFlag _currentTauntCategory;

		private InputKeyItemVM _actionInputKey;

		private InputKeyItemVM _previewInputKey;

		private int _loot;

		private bool _isLoading;

		private bool _hasCosmeticInfoReceived;

		private bool _isManagingTaunts;

		private bool _isTauntAssignmentActive;

		private string _cosmeticInfoErrorText;

		private HintViewModel _allCategoriesHint;

		private HintViewModel _bodyCategoryHint;

		private HintViewModel _headCategoryHint;

		private HintViewModel _shoulderCategoryHint;

		private HintViewModel _handCategoryHint;

		private HintViewModel _legCategoryHint;

		private HintViewModel _resetPreviewHint;

		private MPArmoryCosmeticCategoryBaseVM _activeCategory;

		private MPArmoryCosmeticTauntSlotVM _selectedTauntSlot;

		private MPArmoryCosmeticTauntItemVM _selectedTauntItem;

		private SelectorVM<SelectorItemVM> _sortCategories;

		private SelectorVM<SelectorItemVM> _sortOrders;

		private MBBindingList<MPArmoryCosmeticTauntSlotVM> _tauntSlots;

		private MBBindingList<MPArmoryCosmeticCategoryBaseVM> _availableCategories;

		public enum ClothingCategory
		{
			Invalid = -1,
			ClothingCategoriesBegin,
			All = 0,
			HeadArmor,
			Cape,
			BodyArmor,
			HandArmor,
			LegArmor,
			ClothingCategoriesEnd
		}

		[Flags]
		public enum TauntCategoryFlag
		{
			None = 0,
			UsableWithMount = 1,
			UsableWithOneHanded = 2,
			UsableWithTwoHanded = 4,
			UsableWithBow = 8,
			UsableWithCrossbow = 16,
			UsableWithShield = 32,
			All = 63
		}

		public abstract class CosmeticItemComparer : IComparer<MPArmoryCosmeticItemBaseVM>
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

			public abstract int Compare(MPArmoryCosmeticItemBaseVM x, MPArmoryCosmeticItemBaseVM y);

			private bool _isAscending;
		}

		private class CosmeticItemNameComparer : MPArmoryCosmeticsVM.CosmeticItemComparer
		{
			public override int Compare(MPArmoryCosmeticItemBaseVM x, MPArmoryCosmeticItemBaseVM y)
			{
				return x.Name.CompareTo(y.Name) * base._sortMultiplier;
			}
		}

		private class CosmeticItemCostComparer : MPArmoryCosmeticsVM.CosmeticItemComparer
		{
			public override int Compare(MPArmoryCosmeticItemBaseVM x, MPArmoryCosmeticItemBaseVM y)
			{
				int num = x.Cost.CompareTo(y.Cost);
				if (num == 0)
				{
					MPArmoryCosmeticClothingItemVM mparmoryCosmeticClothingItemVM;
					MPArmoryCosmeticClothingItemVM mparmoryCosmeticClothingItemVM2;
					MPArmoryCosmeticTauntItemVM mparmoryCosmeticTauntItemVM;
					MPArmoryCosmeticTauntItemVM mparmoryCosmeticTauntItemVM2;
					if ((mparmoryCosmeticClothingItemVM = x as MPArmoryCosmeticClothingItemVM) != null && (mparmoryCosmeticClothingItemVM2 = y as MPArmoryCosmeticClothingItemVM) != null)
					{
						num = mparmoryCosmeticClothingItemVM.EquipmentElement.Item.ItemType.CompareTo(mparmoryCosmeticClothingItemVM2.EquipmentElement.Item.ItemType);
					}
					else if ((mparmoryCosmeticTauntItemVM = x as MPArmoryCosmeticTauntItemVM) != null && (mparmoryCosmeticTauntItemVM2 = y as MPArmoryCosmeticTauntItemVM) != null)
					{
						num = mparmoryCosmeticTauntItemVM.Name.CompareTo(mparmoryCosmeticTauntItemVM2.Name);
					}
				}
				return num * base._sortMultiplier;
			}
		}

		private class CosmeticItemRarityComparer : MPArmoryCosmeticsVM.CosmeticItemComparer
		{
			public override int Compare(MPArmoryCosmeticItemBaseVM x, MPArmoryCosmeticItemBaseVM y)
			{
				int num = x.Cosmetic.Rarity.CompareTo(y.Cosmetic.Rarity);
				if (num == 0)
				{
					MPArmoryCosmeticClothingItemVM mparmoryCosmeticClothingItemVM;
					MPArmoryCosmeticClothingItemVM mparmoryCosmeticClothingItemVM2;
					MPArmoryCosmeticTauntItemVM mparmoryCosmeticTauntItemVM;
					MPArmoryCosmeticTauntItemVM mparmoryCosmeticTauntItemVM2;
					if ((mparmoryCosmeticClothingItemVM = x as MPArmoryCosmeticClothingItemVM) != null && (mparmoryCosmeticClothingItemVM2 = y as MPArmoryCosmeticClothingItemVM) != null)
					{
						num = mparmoryCosmeticClothingItemVM.EquipmentElement.Item.ItemType.CompareTo(mparmoryCosmeticClothingItemVM2.EquipmentElement.Item.ItemType);
					}
					else if ((mparmoryCosmeticTauntItemVM = x as MPArmoryCosmeticTauntItemVM) != null && (mparmoryCosmeticTauntItemVM2 = y as MPArmoryCosmeticTauntItemVM) != null)
					{
						num = mparmoryCosmeticTauntItemVM.Name.CompareTo(mparmoryCosmeticTauntItemVM2.Name);
					}
				}
				return num * base._sortMultiplier;
			}
		}

		private class CosmeticItemCategoryComparer : MPArmoryCosmeticsVM.CosmeticItemComparer
		{
			public override int Compare(MPArmoryCosmeticItemBaseVM x, MPArmoryCosmeticItemBaseVM y)
			{
				MPArmoryCosmeticClothingItemVM mparmoryCosmeticClothingItemVM;
				MPArmoryCosmeticClothingItemVM mparmoryCosmeticClothingItemVM2;
				if ((mparmoryCosmeticClothingItemVM = x as MPArmoryCosmeticClothingItemVM) != null && (mparmoryCosmeticClothingItemVM2 = y as MPArmoryCosmeticClothingItemVM) != null)
				{
					return mparmoryCosmeticClothingItemVM.EquipmentElement.Item.ItemType.CompareTo(mparmoryCosmeticClothingItemVM2.EquipmentElement.Item.ItemType) * base._sortMultiplier;
				}
				return 0;
			}
		}
	}
}
