using System;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;

namespace TaleWorlds.Core.ViewModelCollection
{
	public class ItemVM : ViewModel
	{
		public ItemVM()
		{
			this.RefreshValues();
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
		}

		[DataSourceProperty]
		public EquipmentIndex ItemType
		{
			get
			{
				if (this._itemType == EquipmentIndex.None)
				{
					return this.GetItemTypeWithItemObject();
				}
				return this._itemType;
			}
		}

		[DataSourceProperty]
		public ImageIdentifierVM ImageIdentifier
		{
			get
			{
				return this._imageIdentifier;
			}
			set
			{
				if (value != this._imageIdentifier)
				{
					this._imageIdentifier = value;
					base.OnPropertyChangedWithValue<ImageIdentifierVM>(value, "ImageIdentifier");
				}
			}
		}

		[DataSourceProperty]
		public string StringId
		{
			get
			{
				return this._stringId;
			}
			set
			{
				if (value != this._stringId)
				{
					this._stringId = value;
					base.OnPropertyChangedWithValue<string>(value, "StringId");
				}
			}
		}

		[DataSourceProperty]
		public string ItemDescription
		{
			get
			{
				return this._itemDescription;
			}
			set
			{
				if (value != this._itemDescription)
				{
					this._itemDescription = value;
					base.OnPropertyChangedWithValue<string>(value, "ItemDescription");
				}
			}
		}

		[DataSourceProperty]
		public bool IsFiltered
		{
			get
			{
				return this._isFiltered;
			}
			set
			{
				if (value != this._isFiltered)
				{
					this._isFiltered = value;
					base.OnPropertyChangedWithValue(value, "IsFiltered");
				}
			}
		}

		[DataSourceProperty]
		public int ItemCost
		{
			get
			{
				return this._itemCost;
			}
			set
			{
				if (value != this._itemCost)
				{
					this._itemCost = value;
					base.OnPropertyChangedWithValue(value, "ItemCost");
				}
			}
		}

		[DataSourceProperty]
		public int TypeId
		{
			get
			{
				return this._typeId;
			}
			set
			{
				if (value != this._typeId)
				{
					this._typeId = value;
					base.OnPropertyChangedWithValue(value, "TypeId");
				}
			}
		}

		[DataSourceProperty]
		public HintViewModel PreviewHint
		{
			get
			{
				return this._previewHint;
			}
			set
			{
				if (value != this._previewHint)
				{
					this._previewHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "PreviewHint");
				}
			}
		}

		[DataSourceProperty]
		public HintViewModel EquipHint
		{
			get
			{
				return this._equipHint;
			}
			set
			{
				if (value != this._equipHint)
				{
					this._equipHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "EquipHint");
				}
			}
		}

		[DataSourceProperty]
		public BasicTooltipViewModel SlaughterHint
		{
			get
			{
				return this._slaughterHint;
			}
			set
			{
				if (value != this._slaughterHint)
				{
					this._slaughterHint = value;
					base.OnPropertyChangedWithValue<BasicTooltipViewModel>(value, "SlaughterHint");
				}
			}
		}

		[DataSourceProperty]
		public BasicTooltipViewModel DonateHint
		{
			get
			{
				return this._donateHint;
			}
			set
			{
				if (value != this._donateHint)
				{
					this._donateHint = value;
					base.OnPropertyChangedWithValue<BasicTooltipViewModel>(value, "DonateHint");
				}
			}
		}

		[DataSourceProperty]
		public BasicTooltipViewModel BuyAndEquipHint
		{
			get
			{
				return this._buyAndEquip;
			}
			set
			{
				if (value != this._buyAndEquip)
				{
					this._buyAndEquip = value;
					base.OnPropertyChangedWithValue<BasicTooltipViewModel>(value, "BuyAndEquipHint");
				}
			}
		}

		[DataSourceProperty]
		public BasicTooltipViewModel SellHint
		{
			get
			{
				return this._sellHint;
			}
			set
			{
				if (value != this._sellHint)
				{
					this._sellHint = value;
					base.OnPropertyChangedWithValue<BasicTooltipViewModel>(value, "SellHint");
				}
			}
		}

		[DataSourceProperty]
		public BasicTooltipViewModel BuyHint
		{
			get
			{
				return this._buyHint;
			}
			set
			{
				if (value != this._buyHint)
				{
					this._buyHint = value;
					base.OnPropertyChangedWithValue<BasicTooltipViewModel>(value, "BuyHint");
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

		public void ExecutePreviewItem()
		{
			if (!UiStringHelper.IsStringNoneOrEmptyForUi(this.StringId))
			{
				ItemVM.ProcessPreviewItem(this);
			}
		}

		public void ExecuteUnequipItem()
		{
			if (!UiStringHelper.IsStringNoneOrEmptyForUi(this.StringId))
			{
				ItemVM.ProcessUnequipItem(this);
			}
		}

		public void ExecuteEquipItem()
		{
			if (!UiStringHelper.IsStringNoneOrEmptyForUi(this.StringId))
			{
				ItemVM.ProcessEquipItem(this);
			}
		}

		public static void ReleaseStaticContent()
		{
			ItemVM.ProcessEquipItem = null;
			ItemVM.ProcessPreviewItem = null;
			ItemVM.ProcessUnequipItem = null;
			ItemVM.ProcessBuyItem = null;
			ItemVM.ProcessItemSelect = null;
			ItemVM.ProcessItemTooltip = null;
		}

		public void ExecuteRefreshTooltip()
		{
			if (ItemVM.ProcessItemTooltip != null && !UiStringHelper.IsStringNoneOrEmptyForUi(this.StringId))
			{
				ItemVM.ProcessItemTooltip(this);
			}
		}

		public void ExecuteCancelTooltip()
		{
		}

		public void ExecuteBuyItem()
		{
			if (!UiStringHelper.IsStringNoneOrEmptyForUi(this.StringId))
			{
				ItemVM.ProcessBuyItem(this, false);
			}
		}

		public void ExecuteSelectItem()
		{
			if (!UiStringHelper.IsStringNoneOrEmptyForUi(this.StringId))
			{
				ItemVM.ProcessItemSelect(this);
			}
		}

		public EquipmentIndex GetItemTypeWithItemObject()
		{
			if (this.ItemRosterElement.EquipmentElement.Item == null)
			{
				return EquipmentIndex.None;
			}
			ItemObject.ItemTypeEnum type = this.ItemRosterElement.EquipmentElement.Item.Type;
			switch (type)
			{
			case ItemObject.ItemTypeEnum.Horse:
				return EquipmentIndex.ArmorItemEndSlot;
			case ItemObject.ItemTypeEnum.OneHandedWeapon:
			case ItemObject.ItemTypeEnum.TwoHandedWeapon:
			case ItemObject.ItemTypeEnum.Polearm:
			case ItemObject.ItemTypeEnum.Bow:
			case ItemObject.ItemTypeEnum.Crossbow:
			case ItemObject.ItemTypeEnum.Thrown:
			case ItemObject.ItemTypeEnum.Goods:
				break;
			case ItemObject.ItemTypeEnum.Arrows:
				return EquipmentIndex.WeaponItemBeginSlot;
			case ItemObject.ItemTypeEnum.Bolts:
				return EquipmentIndex.WeaponItemBeginSlot;
			case ItemObject.ItemTypeEnum.Shield:
				if (this._typeId == 0)
				{
					this._typeId = 1;
				}
				return EquipmentIndex.WeaponItemBeginSlot;
			case ItemObject.ItemTypeEnum.HeadArmor:
				return EquipmentIndex.NumAllWeaponSlots;
			case ItemObject.ItemTypeEnum.BodyArmor:
				return EquipmentIndex.Body;
			case ItemObject.ItemTypeEnum.LegArmor:
				return EquipmentIndex.Leg;
			case ItemObject.ItemTypeEnum.HandArmor:
				return EquipmentIndex.Gloves;
			default:
				switch (type)
				{
				case ItemObject.ItemTypeEnum.Cape:
					return EquipmentIndex.Cape;
				case ItemObject.ItemTypeEnum.HorseHarness:
					return EquipmentIndex.HorseHarness;
				case ItemObject.ItemTypeEnum.Banner:
					return EquipmentIndex.ExtraWeaponSlot;
				}
				break;
			}
			if (this.ItemRosterElement.EquipmentElement.Item.WeaponComponent != null)
			{
				return EquipmentIndex.WeaponItemBeginSlot;
			}
			return EquipmentIndex.None;
		}

		protected void SetItemTypeId()
		{
			this.TypeId = (int)this.ItemRosterElement.EquipmentElement.Item.Type;
		}

		public static Action<ItemVM> ProcessEquipItem;

		public static Action<ItemVM> ProcessPreviewItem;

		public static Action<ItemVM> ProcessUnequipItem;

		public static Action<ItemVM, bool> ProcessBuyItem;

		public static Action<ItemVM> ProcessItemSelect;

		public static Action<ItemVM> ProcessItemTooltip;

		private int _typeId;

		private int _itemCost = -1;

		private bool _isFiltered;

		private string _itemDescription;

		public ItemRosterElement ItemRosterElement;

		public EquipmentIndex _itemType = EquipmentIndex.None;

		private ImageIdentifierVM _imageIdentifier;

		private HintViewModel _previewHint;

		private HintViewModel _equipHint;

		private BasicTooltipViewModel _buyAndEquip;

		private BasicTooltipViewModel _sellHint;

		private BasicTooltipViewModel _buyHint;

		private HintViewModel _lockHint;

		private BasicTooltipViewModel _slaughterHint;

		private BasicTooltipViewModel _donateHint;

		private string _stringId;
	}
}
