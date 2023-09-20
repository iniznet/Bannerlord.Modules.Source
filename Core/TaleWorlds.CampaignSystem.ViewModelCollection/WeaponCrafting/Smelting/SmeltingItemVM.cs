using System;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.WeaponCrafting.Smelting
{
	public class SmeltingItemVM : ViewModel
	{
		public EquipmentElement EquipmentElement { get; private set; }

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

		public override void RefreshValues()
		{
			base.RefreshValues();
			this.Name = this.EquipmentElement.Item.Name.ToString();
		}

		public void ExecuteSelection()
		{
			this._onSelection(this);
		}

		public void ExecuteShowItemTooltip()
		{
			InformationManager.ShowTooltip(typeof(ItemObject), new object[] { this.EquipmentElement });
		}

		public void ExecuteHideItemTooltip()
		{
			MBInformationManager.HideInformations();
		}

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

		private readonly Action<SmeltingItemVM> _onSelection;

		private readonly Action<SmeltingItemVM, bool> _onItemLockedStateChange;

		private ImageIdentifierVM _visual;

		private string _name;

		private int _numOfItems;

		private MBBindingList<CraftingResourceItemVM> _inputMaterials;

		private MBBindingList<CraftingResourceItemVM> _yield;

		private HintViewModel _lockHint;

		private bool _isSelected;

		private bool _isLocked;

		private bool _hasMoreThanOneItem;
	}
}
