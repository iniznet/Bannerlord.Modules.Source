using System;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;

namespace TaleWorlds.Core.ViewModelCollection
{
	// Token: 0x0200000D RID: 13
	public class ItemVM : ViewModel
	{
		// Token: 0x0600006B RID: 107 RVA: 0x00002A5A File Offset: 0x00000C5A
		public ItemVM()
		{
			this.RefreshValues();
		}

		// Token: 0x0600006C RID: 108 RVA: 0x00002A76 File Offset: 0x00000C76
		public override void RefreshValues()
		{
			base.RefreshValues();
		}

		// Token: 0x17000022 RID: 34
		// (get) Token: 0x0600006D RID: 109 RVA: 0x00002A7E File Offset: 0x00000C7E
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

		// Token: 0x17000023 RID: 35
		// (get) Token: 0x0600006E RID: 110 RVA: 0x00002A96 File Offset: 0x00000C96
		// (set) Token: 0x0600006F RID: 111 RVA: 0x00002A9E File Offset: 0x00000C9E
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

		// Token: 0x17000024 RID: 36
		// (get) Token: 0x06000070 RID: 112 RVA: 0x00002ABC File Offset: 0x00000CBC
		// (set) Token: 0x06000071 RID: 113 RVA: 0x00002AC4 File Offset: 0x00000CC4
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

		// Token: 0x17000025 RID: 37
		// (get) Token: 0x06000072 RID: 114 RVA: 0x00002AE7 File Offset: 0x00000CE7
		// (set) Token: 0x06000073 RID: 115 RVA: 0x00002AEF File Offset: 0x00000CEF
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

		// Token: 0x17000026 RID: 38
		// (get) Token: 0x06000074 RID: 116 RVA: 0x00002B12 File Offset: 0x00000D12
		// (set) Token: 0x06000075 RID: 117 RVA: 0x00002B1A File Offset: 0x00000D1A
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

		// Token: 0x17000027 RID: 39
		// (get) Token: 0x06000076 RID: 118 RVA: 0x00002B38 File Offset: 0x00000D38
		// (set) Token: 0x06000077 RID: 119 RVA: 0x00002B40 File Offset: 0x00000D40
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

		// Token: 0x17000028 RID: 40
		// (get) Token: 0x06000078 RID: 120 RVA: 0x00002B5E File Offset: 0x00000D5E
		// (set) Token: 0x06000079 RID: 121 RVA: 0x00002B66 File Offset: 0x00000D66
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

		// Token: 0x17000029 RID: 41
		// (get) Token: 0x0600007A RID: 122 RVA: 0x00002B84 File Offset: 0x00000D84
		// (set) Token: 0x0600007B RID: 123 RVA: 0x00002B8C File Offset: 0x00000D8C
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

		// Token: 0x1700002A RID: 42
		// (get) Token: 0x0600007C RID: 124 RVA: 0x00002BAA File Offset: 0x00000DAA
		// (set) Token: 0x0600007D RID: 125 RVA: 0x00002BB2 File Offset: 0x00000DB2
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

		// Token: 0x1700002B RID: 43
		// (get) Token: 0x0600007E RID: 126 RVA: 0x00002BD0 File Offset: 0x00000DD0
		// (set) Token: 0x0600007F RID: 127 RVA: 0x00002BD8 File Offset: 0x00000DD8
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

		// Token: 0x1700002C RID: 44
		// (get) Token: 0x06000080 RID: 128 RVA: 0x00002BF6 File Offset: 0x00000DF6
		// (set) Token: 0x06000081 RID: 129 RVA: 0x00002BFE File Offset: 0x00000DFE
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

		// Token: 0x1700002D RID: 45
		// (get) Token: 0x06000082 RID: 130 RVA: 0x00002C1C File Offset: 0x00000E1C
		// (set) Token: 0x06000083 RID: 131 RVA: 0x00002C24 File Offset: 0x00000E24
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

		// Token: 0x1700002E RID: 46
		// (get) Token: 0x06000084 RID: 132 RVA: 0x00002C42 File Offset: 0x00000E42
		// (set) Token: 0x06000085 RID: 133 RVA: 0x00002C4A File Offset: 0x00000E4A
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

		// Token: 0x1700002F RID: 47
		// (get) Token: 0x06000086 RID: 134 RVA: 0x00002C68 File Offset: 0x00000E68
		// (set) Token: 0x06000087 RID: 135 RVA: 0x00002C70 File Offset: 0x00000E70
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

		// Token: 0x17000030 RID: 48
		// (get) Token: 0x06000088 RID: 136 RVA: 0x00002C8E File Offset: 0x00000E8E
		// (set) Token: 0x06000089 RID: 137 RVA: 0x00002C96 File Offset: 0x00000E96
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

		// Token: 0x0600008A RID: 138 RVA: 0x00002CB4 File Offset: 0x00000EB4
		public void ExecutePreviewItem()
		{
			if (!UiStringHelper.IsStringNoneOrEmptyForUi(this.StringId))
			{
				ItemVM.ProcessPreviewItem(this);
			}
		}

		// Token: 0x0600008B RID: 139 RVA: 0x00002CCE File Offset: 0x00000ECE
		public void ExecuteUnequipItem()
		{
			if (!UiStringHelper.IsStringNoneOrEmptyForUi(this.StringId))
			{
				ItemVM.ProcessUnequipItem(this);
			}
		}

		// Token: 0x0600008C RID: 140 RVA: 0x00002CE8 File Offset: 0x00000EE8
		public void ExecuteEquipItem()
		{
			if (!UiStringHelper.IsStringNoneOrEmptyForUi(this.StringId))
			{
				ItemVM.ProcessEquipItem(this);
			}
		}

		// Token: 0x0600008D RID: 141 RVA: 0x00002D02 File Offset: 0x00000F02
		public static void ReleaseStaticContent()
		{
			ItemVM.ProcessEquipItem = null;
			ItemVM.ProcessPreviewItem = null;
			ItemVM.ProcessUnequipItem = null;
			ItemVM.ProcessBuyItem = null;
			ItemVM.ProcessItemSelect = null;
			ItemVM.ProcessItemTooltip = null;
		}

		// Token: 0x0600008E RID: 142 RVA: 0x00002D28 File Offset: 0x00000F28
		public void ExecuteRefreshTooltip()
		{
			if (ItemVM.ProcessItemTooltip != null && !UiStringHelper.IsStringNoneOrEmptyForUi(this.StringId))
			{
				ItemVM.ProcessItemTooltip(this);
			}
		}

		// Token: 0x0600008F RID: 143 RVA: 0x00002D49 File Offset: 0x00000F49
		public void ExecuteCancelTooltip()
		{
		}

		// Token: 0x06000090 RID: 144 RVA: 0x00002D4B File Offset: 0x00000F4B
		public void ExecuteBuyItem()
		{
			if (!UiStringHelper.IsStringNoneOrEmptyForUi(this.StringId))
			{
				ItemVM.ProcessBuyItem(this, false);
			}
		}

		// Token: 0x06000091 RID: 145 RVA: 0x00002D66 File Offset: 0x00000F66
		public void ExecuteSelectItem()
		{
			if (!UiStringHelper.IsStringNoneOrEmptyForUi(this.StringId))
			{
				ItemVM.ProcessItemSelect(this);
			}
		}

		// Token: 0x06000092 RID: 146 RVA: 0x00002D80 File Offset: 0x00000F80
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

		// Token: 0x06000093 RID: 147 RVA: 0x00002E60 File Offset: 0x00001060
		protected void SetItemTypeId()
		{
			this.TypeId = (int)this.ItemRosterElement.EquipmentElement.Item.Type;
		}

		// Token: 0x04000021 RID: 33
		public static Action<ItemVM> ProcessEquipItem;

		// Token: 0x04000022 RID: 34
		public static Action<ItemVM> ProcessPreviewItem;

		// Token: 0x04000023 RID: 35
		public static Action<ItemVM> ProcessUnequipItem;

		// Token: 0x04000024 RID: 36
		public static Action<ItemVM, bool> ProcessBuyItem;

		// Token: 0x04000025 RID: 37
		public static Action<ItemVM> ProcessItemSelect;

		// Token: 0x04000026 RID: 38
		public static Action<ItemVM> ProcessItemTooltip;

		// Token: 0x04000027 RID: 39
		private int _typeId;

		// Token: 0x04000028 RID: 40
		private int _itemCost = -1;

		// Token: 0x04000029 RID: 41
		private bool _isFiltered;

		// Token: 0x0400002A RID: 42
		private string _itemDescription;

		// Token: 0x0400002B RID: 43
		public ItemRosterElement ItemRosterElement;

		// Token: 0x0400002C RID: 44
		public EquipmentIndex _itemType = EquipmentIndex.None;

		// Token: 0x0400002D RID: 45
		private ImageIdentifierVM _imageIdentifier;

		// Token: 0x0400002E RID: 46
		private HintViewModel _previewHint;

		// Token: 0x0400002F RID: 47
		private HintViewModel _equipHint;

		// Token: 0x04000030 RID: 48
		private BasicTooltipViewModel _buyAndEquip;

		// Token: 0x04000031 RID: 49
		private BasicTooltipViewModel _sellHint;

		// Token: 0x04000032 RID: 50
		private BasicTooltipViewModel _buyHint;

		// Token: 0x04000033 RID: 51
		private HintViewModel _lockHint;

		// Token: 0x04000034 RID: 52
		private BasicTooltipViewModel _slaughterHint;

		// Token: 0x04000035 RID: 53
		private BasicTooltipViewModel _donateHint;

		// Token: 0x04000036 RID: 54
		private string _stringId;
	}
}
