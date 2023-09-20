using System;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.WeaponCrafting.Smelting
{
	// Token: 0x020000ED RID: 237
	public class SmeltingItemVM : ViewModel
	{
		// Token: 0x17000781 RID: 1921
		// (get) Token: 0x06001632 RID: 5682 RVA: 0x00052EFB File Offset: 0x000510FB
		// (set) Token: 0x06001631 RID: 5681 RVA: 0x00052EF2 File Offset: 0x000510F2
		public EquipmentElement EquipmentElement { get; private set; }

		// Token: 0x06001633 RID: 5683 RVA: 0x00052F04 File Offset: 0x00051104
		public SmeltingItemVM(EquipmentElement equipmentElement, Action<SmeltingItemVM> onSelection, Action<SmeltingItemVM, bool> onItemLockedStateChange, bool isLocked, int numOfItems)
		{
			this._onSelection = onSelection;
			this._onItemLockedStateChange = onItemLockedStateChange;
			this.EquipmentElement = equipmentElement;
			this.Yield = new MBBindingList<CraftingResourceItemVM>();
			this.InputMaterials = new MBBindingList<CraftingResourceItemVM>();
			this.LockHint = new HintViewModel(GameTexts.FindText("str_inventory_lock", null), null);
			int[] smeltingOutputForItem = Campaign.Current.Models.SmithingModel.GetSmeltingOutputForItem(equipmentElement.Item);
			for (int i = 0; i < smeltingOutputForItem.Length; i++)
			{
				if (smeltingOutputForItem[i] > 0)
				{
					this.Yield.Add(new CraftingResourceItemVM((CraftingMaterials)i, smeltingOutputForItem[i], 0));
				}
				else if (smeltingOutputForItem[i] < 0)
				{
					this.InputMaterials.Add(new CraftingResourceItemVM((CraftingMaterials)i, -smeltingOutputForItem[i], 0));
				}
			}
			this.IsLocked = isLocked;
			this.Visual = new ImageIdentifierVM(equipmentElement.Item, "");
			this.NumOfItems = numOfItems;
			this.HasMoreThanOneItem = this.NumOfItems > 1;
			this.RefreshValues();
		}

		// Token: 0x06001634 RID: 5684 RVA: 0x00052FF8 File Offset: 0x000511F8
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.Name = this.EquipmentElement.Item.Name.ToString();
		}

		// Token: 0x06001635 RID: 5685 RVA: 0x00053029 File Offset: 0x00051229
		public void ExecuteSelection()
		{
			this._onSelection(this);
		}

		// Token: 0x06001636 RID: 5686 RVA: 0x00053037 File Offset: 0x00051237
		public void ExecuteShowItemTooltip()
		{
			InformationManager.ShowTooltip(typeof(ItemObject), new object[] { this.EquipmentElement });
		}

		// Token: 0x06001637 RID: 5687 RVA: 0x0005305C File Offset: 0x0005125C
		public void ExecuteHideItemTooltip()
		{
			MBInformationManager.HideInformations();
		}

		// Token: 0x17000782 RID: 1922
		// (get) Token: 0x06001638 RID: 5688 RVA: 0x00053063 File Offset: 0x00051263
		// (set) Token: 0x06001639 RID: 5689 RVA: 0x0005306B File Offset: 0x0005126B
		[DataSourceProperty]
		public ImageIdentifierVM Visual
		{
			get
			{
				return this._visual;
			}
			set
			{
				if (value != this._visual)
				{
					this._visual = value;
					base.OnPropertyChangedWithValue<ImageIdentifierVM>(value, "Visual");
				}
			}
		}

		// Token: 0x17000783 RID: 1923
		// (get) Token: 0x0600163A RID: 5690 RVA: 0x00053089 File Offset: 0x00051289
		// (set) Token: 0x0600163B RID: 5691 RVA: 0x00053091 File Offset: 0x00051291
		[DataSourceProperty]
		public MBBindingList<CraftingResourceItemVM> Yield
		{
			get
			{
				return this._yield;
			}
			set
			{
				if (value != this._yield)
				{
					this._yield = value;
					base.OnPropertyChangedWithValue<MBBindingList<CraftingResourceItemVM>>(value, "Yield");
				}
			}
		}

		// Token: 0x17000784 RID: 1924
		// (get) Token: 0x0600163C RID: 5692 RVA: 0x000530AF File Offset: 0x000512AF
		// (set) Token: 0x0600163D RID: 5693 RVA: 0x000530B7 File Offset: 0x000512B7
		[DataSourceProperty]
		public MBBindingList<CraftingResourceItemVM> InputMaterials
		{
			get
			{
				return this._inputMaterials;
			}
			set
			{
				if (value != this._inputMaterials)
				{
					this._inputMaterials = value;
					base.OnPropertyChangedWithValue<MBBindingList<CraftingResourceItemVM>>(value, "InputMaterials");
				}
			}
		}

		// Token: 0x17000785 RID: 1925
		// (get) Token: 0x0600163E RID: 5694 RVA: 0x000530D5 File Offset: 0x000512D5
		// (set) Token: 0x0600163F RID: 5695 RVA: 0x000530DD File Offset: 0x000512DD
		[DataSourceProperty]
		public string Name
		{
			get
			{
				return this._name;
			}
			set
			{
				if (value != this._name)
				{
					this._name = value;
					base.OnPropertyChangedWithValue<string>(value, "Name");
				}
			}
		}

		// Token: 0x17000786 RID: 1926
		// (get) Token: 0x06001640 RID: 5696 RVA: 0x00053100 File Offset: 0x00051300
		// (set) Token: 0x06001641 RID: 5697 RVA: 0x00053108 File Offset: 0x00051308
		[DataSourceProperty]
		public int NumOfItems
		{
			get
			{
				return this._numOfItems;
			}
			set
			{
				if (value != this._numOfItems)
				{
					this._numOfItems = value;
					base.OnPropertyChangedWithValue(value, "NumOfItems");
				}
			}
		}

		// Token: 0x17000787 RID: 1927
		// (get) Token: 0x06001642 RID: 5698 RVA: 0x00053126 File Offset: 0x00051326
		// (set) Token: 0x06001643 RID: 5699 RVA: 0x0005312E File Offset: 0x0005132E
		[DataSourceProperty]
		public bool HasMoreThanOneItem
		{
			get
			{
				return this._hasMoreThanOneItem;
			}
			set
			{
				if (value != this._hasMoreThanOneItem)
				{
					this._hasMoreThanOneItem = value;
					base.OnPropertyChangedWithValue(value, "HasMoreThanOneItem");
				}
			}
		}

		// Token: 0x17000788 RID: 1928
		// (get) Token: 0x06001644 RID: 5700 RVA: 0x0005314C File Offset: 0x0005134C
		// (set) Token: 0x06001645 RID: 5701 RVA: 0x00053154 File Offset: 0x00051354
		[DataSourceProperty]
		public bool IsSelected
		{
			get
			{
				return this._isSelected;
			}
			set
			{
				if (value != this._isSelected)
				{
					this._isSelected = value;
					base.OnPropertyChangedWithValue(value, "IsSelected");
				}
			}
		}

		// Token: 0x17000789 RID: 1929
		// (get) Token: 0x06001646 RID: 5702 RVA: 0x00053172 File Offset: 0x00051372
		// (set) Token: 0x06001647 RID: 5703 RVA: 0x0005317A File Offset: 0x0005137A
		[DataSourceProperty]
		public HintViewModel LockHint
		{
			get
			{
				return this._lockHint;
			}
			set
			{
				if (value != this._lockHint)
				{
					this._lockHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "LockHint");
				}
			}
		}

		// Token: 0x1700078A RID: 1930
		// (get) Token: 0x06001648 RID: 5704 RVA: 0x00053198 File Offset: 0x00051398
		// (set) Token: 0x06001649 RID: 5705 RVA: 0x000531A0 File Offset: 0x000513A0
		[DataSourceProperty]
		public bool IsLocked
		{
			get
			{
				return this._isLocked;
			}
			set
			{
				if (value != this._isLocked)
				{
					this._isLocked = value;
					base.OnPropertyChangedWithValue(value, "IsLocked");
					this._onItemLockedStateChange(this, value);
				}
			}
		}

		// Token: 0x04000A66 RID: 2662
		private readonly Action<SmeltingItemVM> _onSelection;

		// Token: 0x04000A67 RID: 2663
		private readonly Action<SmeltingItemVM, bool> _onItemLockedStateChange;

		// Token: 0x04000A68 RID: 2664
		private ImageIdentifierVM _visual;

		// Token: 0x04000A69 RID: 2665
		private string _name;

		// Token: 0x04000A6A RID: 2666
		private int _numOfItems;

		// Token: 0x04000A6B RID: 2667
		private MBBindingList<CraftingResourceItemVM> _inputMaterials;

		// Token: 0x04000A6C RID: 2668
		private MBBindingList<CraftingResourceItemVM> _yield;

		// Token: 0x04000A6D RID: 2669
		private HintViewModel _lockHint;

		// Token: 0x04000A6E RID: 2670
		private bool _isSelected;

		// Token: 0x04000A6F RID: 2671
		private bool _isLocked;

		// Token: 0x04000A70 RID: 2672
		private bool _hasMoreThanOneItem;
	}
}
