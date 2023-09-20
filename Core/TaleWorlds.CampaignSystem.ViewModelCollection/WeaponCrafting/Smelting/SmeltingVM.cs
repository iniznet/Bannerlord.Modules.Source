using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem.CampaignBehaviors;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.WeaponCrafting.Smelting
{
	// Token: 0x020000EF RID: 239
	public class SmeltingVM : ViewModel
	{
		// Token: 0x06001664 RID: 5732 RVA: 0x00053584 File Offset: 0x00051784
		public SmeltingVM(Action updateValuesOnSelectItemAction, Action updateValuesOnSmeltItemAction)
		{
			this.SortController = new SmeltingSortControllerVM();
			this._updateValuesOnSelectItemAction = updateValuesOnSelectItemAction;
			this._updateValuesOnSmeltItemAction = updateValuesOnSmeltItemAction;
			this._playerItemRoster = MobileParty.MainParty.ItemRoster;
			this._smithingBehavior = Campaign.Current.GetCampaignBehavior<ICraftingCampaignBehavior>();
			IViewDataTracker campaignBehavior = Campaign.Current.GetCampaignBehavior<IViewDataTracker>();
			this._lockedItemIDs = campaignBehavior.GetInventoryLocks().ToList<string>();
			this.RefreshList();
			this.RefreshValues();
		}

		// Token: 0x06001665 RID: 5733 RVA: 0x000535F8 File Offset: 0x000517F8
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.SelectAllHint = new HintViewModel(new TextObject("{=k1E9DuKi}Select All", null), null);
			SmeltingItemVM currentSelectedItem = this.CurrentSelectedItem;
			if (currentSelectedItem != null)
			{
				currentSelectedItem.RefreshValues();
			}
			this.SmeltableItemList.ApplyActionOnAllItems(delegate(SmeltingItemVM x)
			{
				x.RefreshValues();
			});
			this.SortController.RefreshValues();
		}

		// Token: 0x06001666 RID: 5734 RVA: 0x00053668 File Offset: 0x00051868
		internal void OnCraftingHeroChanged(CraftingAvailableHeroItemVM newHero)
		{
		}

		// Token: 0x06001667 RID: 5735 RVA: 0x0005366C File Offset: 0x0005186C
		public void RefreshList()
		{
			this.SmeltableItemList = new MBBindingList<SmeltingItemVM>();
			this.SortController.SetListToControl(this.SmeltableItemList);
			for (int i = 0; i < this._playerItemRoster.Count; i++)
			{
				ItemRosterElement elementCopyAtIndex = this._playerItemRoster.GetElementCopyAtIndex(i);
				if (elementCopyAtIndex.EquipmentElement.Item.IsCraftedWeapon)
				{
					bool flag = this.IsItemLocked(elementCopyAtIndex.EquipmentElement);
					SmeltingItemVM smeltingItemVM = new SmeltingItemVM(elementCopyAtIndex.EquipmentElement, new Action<SmeltingItemVM>(this.OnItemSelection), new Action<SmeltingItemVM, bool>(this.ProcessLockItem), flag, elementCopyAtIndex.Amount);
					this.SmeltableItemList.Add(smeltingItemVM);
				}
			}
			if (this.SmeltableItemList.Count == 0)
			{
				this.CurrentSelectedItem = null;
			}
		}

		// Token: 0x06001668 RID: 5736 RVA: 0x0005372C File Offset: 0x0005192C
		private void OnItemSelection(SmeltingItemVM newItem)
		{
			if (newItem != this.CurrentSelectedItem)
			{
				if (this.CurrentSelectedItem != null)
				{
					this.CurrentSelectedItem.IsSelected = false;
				}
				this.CurrentSelectedItem = newItem;
				this.CurrentSelectedItem.IsSelected = true;
			}
			this._updateValuesOnSelectItemAction();
			WeaponDesign weaponDesign = this.CurrentSelectedItem.EquipmentElement.Item.WeaponDesign;
			this.WeaponTypeName = ((weaponDesign != null) ? weaponDesign.Template.TemplateName.ToString() : null) ?? string.Empty;
			WeaponDesign weaponDesign2 = this.CurrentSelectedItem.EquipmentElement.Item.WeaponDesign;
			this.WeaponTypeCode = ((weaponDesign2 != null) ? weaponDesign2.Template.StringId : null) ?? string.Empty;
		}

		// Token: 0x06001669 RID: 5737 RVA: 0x000537EC File Offset: 0x000519EC
		public void TrySmeltingSelectedItems(Hero currentCraftingHero)
		{
			if (this._currentSelectedItem != null)
			{
				if (this._currentSelectedItem.IsLocked)
				{
					string text = new TextObject("{=wMiLUTNY}Are you sure you want to smelt this weapon? It is locked in the inventory.", null).ToString();
					InformationManager.ShowInquiry(new InquiryData("", text, true, true, GameTexts.FindText("str_yes", null).ToString(), GameTexts.FindText("str_no", null).ToString(), delegate
					{
						this.SmeltSelectedItems(currentCraftingHero);
					}, null, "", 0f, null, null, null), false, false);
					return;
				}
				this.SmeltSelectedItems(currentCraftingHero);
			}
		}

		// Token: 0x0600166A RID: 5738 RVA: 0x00053890 File Offset: 0x00051A90
		private void ProcessLockItem(SmeltingItemVM item, bool isLocked)
		{
			if (item == null)
			{
				return;
			}
			string itemLockStringID = CampaignUIHelper.GetItemLockStringID(item.EquipmentElement);
			if (isLocked && !this._lockedItemIDs.Contains(itemLockStringID))
			{
				this._lockedItemIDs.Add(itemLockStringID);
				return;
			}
			if (!isLocked && this._lockedItemIDs.Contains(itemLockStringID))
			{
				this._lockedItemIDs.Remove(itemLockStringID);
			}
		}

		// Token: 0x0600166B RID: 5739 RVA: 0x000538EC File Offset: 0x00051AEC
		private void SmeltSelectedItems(Hero currentCraftingHero)
		{
			if (this._currentSelectedItem != null && this._smithingBehavior != null)
			{
				ICraftingCampaignBehavior smithingBehavior = this._smithingBehavior;
				if (smithingBehavior != null)
				{
					smithingBehavior.DoSmelting(currentCraftingHero, this._currentSelectedItem.EquipmentElement);
				}
			}
			this.RefreshList();
			this.SortController.SortByCurrentState();
			if (this.CurrentSelectedItem != null)
			{
				int num = this.SmeltableItemList.FindIndex((SmeltingItemVM i) => i.EquipmentElement.Item == this.CurrentSelectedItem.EquipmentElement.Item);
				SmeltingItemVM smeltingItemVM = ((num != -1) ? this.SmeltableItemList[num] : this.SmeltableItemList.FirstOrDefault<SmeltingItemVM>());
				this.OnItemSelection(smeltingItemVM);
			}
			this._updateValuesOnSmeltItemAction();
		}

		// Token: 0x0600166C RID: 5740 RVA: 0x00053988 File Offset: 0x00051B88
		private bool IsItemLocked(EquipmentElement equipmentElement)
		{
			string itemLockStringID = CampaignUIHelper.GetItemLockStringID(equipmentElement);
			return this._lockedItemIDs.Contains(itemLockStringID);
		}

		// Token: 0x0600166D RID: 5741 RVA: 0x000539A8 File Offset: 0x00051BA8
		public void SaveItemLockStates()
		{
			Campaign.Current.GetCampaignBehavior<IViewDataTracker>().SetInventoryLocks(this._lockedItemIDs);
		}

		// Token: 0x17000794 RID: 1940
		// (get) Token: 0x0600166E RID: 5742 RVA: 0x000539BF File Offset: 0x00051BBF
		// (set) Token: 0x0600166F RID: 5743 RVA: 0x000539C7 File Offset: 0x00051BC7
		[DataSourceProperty]
		public string WeaponTypeName
		{
			get
			{
				return this._weaponTypeName;
			}
			set
			{
				if (value != this._weaponTypeName)
				{
					this._weaponTypeName = value;
					base.OnPropertyChangedWithValue<string>(value, "WeaponTypeName");
				}
			}
		}

		// Token: 0x17000795 RID: 1941
		// (get) Token: 0x06001670 RID: 5744 RVA: 0x000539EA File Offset: 0x00051BEA
		// (set) Token: 0x06001671 RID: 5745 RVA: 0x000539F2 File Offset: 0x00051BF2
		[DataSourceProperty]
		public string WeaponTypeCode
		{
			get
			{
				return this._weaponTypeCode;
			}
			set
			{
				if (value != this._weaponTypeCode)
				{
					this._weaponTypeCode = value;
					base.OnPropertyChangedWithValue<string>(value, "WeaponTypeCode");
				}
			}
		}

		// Token: 0x17000796 RID: 1942
		// (get) Token: 0x06001672 RID: 5746 RVA: 0x00053A15 File Offset: 0x00051C15
		// (set) Token: 0x06001673 RID: 5747 RVA: 0x00053A1D File Offset: 0x00051C1D
		[DataSourceProperty]
		public SmeltingItemVM CurrentSelectedItem
		{
			get
			{
				return this._currentSelectedItem;
			}
			set
			{
				if (value != this._currentSelectedItem)
				{
					this._currentSelectedItem = value;
					base.OnPropertyChangedWithValue<SmeltingItemVM>(value, "CurrentSelectedItem");
					this.IsAnyItemSelected = value != null;
				}
			}
		}

		// Token: 0x17000797 RID: 1943
		// (get) Token: 0x06001674 RID: 5748 RVA: 0x00053A45 File Offset: 0x00051C45
		// (set) Token: 0x06001675 RID: 5749 RVA: 0x00053A4D File Offset: 0x00051C4D
		[DataSourceProperty]
		public bool IsAnyItemSelected
		{
			get
			{
				return this._isAnyItemSelected;
			}
			set
			{
				if (value != this._isAnyItemSelected)
				{
					this._isAnyItemSelected = value;
					base.OnPropertyChangedWithValue(value, "IsAnyItemSelected");
				}
			}
		}

		// Token: 0x17000798 RID: 1944
		// (get) Token: 0x06001676 RID: 5750 RVA: 0x00053A6B File Offset: 0x00051C6B
		// (set) Token: 0x06001677 RID: 5751 RVA: 0x00053A73 File Offset: 0x00051C73
		[DataSourceProperty]
		public MBBindingList<SmeltingItemVM> SmeltableItemList
		{
			get
			{
				return this._smeltableItemList;
			}
			set
			{
				if (value != this._smeltableItemList)
				{
					this._smeltableItemList = value;
					base.OnPropertyChangedWithValue<MBBindingList<SmeltingItemVM>>(value, "SmeltableItemList");
				}
			}
		}

		// Token: 0x17000799 RID: 1945
		// (get) Token: 0x06001678 RID: 5752 RVA: 0x00053A91 File Offset: 0x00051C91
		// (set) Token: 0x06001679 RID: 5753 RVA: 0x00053A99 File Offset: 0x00051C99
		[DataSourceProperty]
		public HintViewModel SelectAllHint
		{
			get
			{
				return this._selectAllHint;
			}
			set
			{
				if (value != this._selectAllHint)
				{
					this._selectAllHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "SelectAllHint");
				}
			}
		}

		// Token: 0x1700079A RID: 1946
		// (get) Token: 0x0600167A RID: 5754 RVA: 0x00053AB7 File Offset: 0x00051CB7
		// (set) Token: 0x0600167B RID: 5755 RVA: 0x00053ABF File Offset: 0x00051CBF
		[DataSourceProperty]
		public SmeltingSortControllerVM SortController
		{
			get
			{
				return this._sortController;
			}
			set
			{
				if (value != this._sortController)
				{
					this._sortController = value;
					base.OnPropertyChangedWithValue<SmeltingSortControllerVM>(value, "SortController");
				}
			}
		}

		// Token: 0x04000A7E RID: 2686
		private ItemRoster _playerItemRoster;

		// Token: 0x04000A7F RID: 2687
		private Action _updateValuesOnSelectItemAction;

		// Token: 0x04000A80 RID: 2688
		private Action _updateValuesOnSmeltItemAction;

		// Token: 0x04000A81 RID: 2689
		private List<string> _lockedItemIDs;

		// Token: 0x04000A82 RID: 2690
		private readonly ICraftingCampaignBehavior _smithingBehavior;

		// Token: 0x04000A83 RID: 2691
		private string _weaponTypeName;

		// Token: 0x04000A84 RID: 2692
		private string _weaponTypeCode;

		// Token: 0x04000A85 RID: 2693
		private SmeltingItemVM _currentSelectedItem;

		// Token: 0x04000A86 RID: 2694
		private MBBindingList<SmeltingItemVM> _smeltableItemList;

		// Token: 0x04000A87 RID: 2695
		private SmeltingSortControllerVM _sortController;

		// Token: 0x04000A88 RID: 2696
		private HintViewModel _selectAllHint;

		// Token: 0x04000A89 RID: 2697
		private bool _isAnyItemSelected;
	}
}
