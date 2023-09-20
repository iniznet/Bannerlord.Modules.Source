using System;
using TaleWorlds.CampaignSystem.Inventory;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.Inventory
{
	// Token: 0x02000078 RID: 120
	public class InventoryTradeVM : ViewModel
	{
		// Token: 0x06000AC1 RID: 2753 RVA: 0x0002A630 File Offset: 0x00028830
		public InventoryTradeVM(InventoryLogic inventoryLogic, ItemRosterElement itemRoster, InventoryLogic.InventorySide side, Action<int, bool> onApplyTransaction)
		{
			this._inventoryLogic = inventoryLogic;
			this._referenceItemRoster = itemRoster;
			this._isPlayerItem = side == InventoryLogic.InventorySide.PlayerInventory;
			this._onApplyTransaction = onApplyTransaction;
			this.PieceLbl = this._pieceLblSingular;
			InventoryLogic inventoryLogic2 = this._inventoryLogic;
			this.IsTrading = inventoryLogic2 != null && inventoryLogic2.IsTrading;
			this.UpdateItemData(itemRoster, side, true);
			this.RefreshValues();
		}

		// Token: 0x06000AC2 RID: 2754 RVA: 0x0002A6A4 File Offset: 0x000288A4
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.ThisStockLbl = GameTexts.FindText("str_inventory_this_stock", null).ToString();
			this.OtherStockLbl = GameTexts.FindText("str_inventory_total_stock", null).ToString();
			this.AveragePriceLbl = GameTexts.FindText("str_inventory_average_price", null).ToString();
			this._pieceLblSingular = GameTexts.FindText("str_inventory_piece", null).ToString();
			this._pieceLblPlural = GameTexts.FindText("str_inventory_pieces", null).ToString();
			this.ApplyExchangeHint = new HintViewModel(GameTexts.FindText("str_party_apply_exchange", null), null);
		}

		// Token: 0x06000AC3 RID: 2755 RVA: 0x0002A73C File Offset: 0x0002893C
		public void UpdateItemData(ItemRosterElement itemRoster, InventoryLogic.InventorySide side, bool forceUpdate = true)
		{
			if (side != InventoryLogic.InventorySide.OtherInventory && side != InventoryLogic.InventorySide.PlayerInventory)
			{
				return;
			}
			ItemRosterElement? itemRosterElement = new ItemRosterElement?(itemRoster);
			ItemRosterElement? itemRosterElement2 = null;
			if (side == InventoryLogic.InventorySide.PlayerInventory)
			{
				itemRosterElement2 = this.FindItemFromSide(itemRoster.EquipmentElement, InventoryLogic.InventorySide.OtherInventory);
			}
			else if (side == InventoryLogic.InventorySide.OtherInventory)
			{
				itemRosterElement2 = this.FindItemFromSide(itemRoster.EquipmentElement, InventoryLogic.InventorySide.PlayerInventory);
			}
			if (forceUpdate)
			{
				this.InitialThisStock = ((itemRosterElement != null) ? itemRosterElement.GetValueOrDefault().Amount : 0);
				this.InitialOtherStock = ((itemRosterElement2 != null) ? itemRosterElement2.GetValueOrDefault().Amount : 0);
				this.TotalStock = this.InitialThisStock + this.InitialOtherStock;
				this.ThisStock = this.InitialThisStock;
				this.OtherStock = this.InitialOtherStock;
				this.ThisStockUpdated();
			}
		}

		// Token: 0x06000AC4 RID: 2756 RVA: 0x0002A7FE File Offset: 0x000289FE
		private ItemRosterElement? FindItemFromSide(EquipmentElement item, InventoryLogic.InventorySide side)
		{
			return this._inventoryLogic.FindItemFromSide(side, item);
		}

		// Token: 0x06000AC5 RID: 2757 RVA: 0x0002A810 File Offset: 0x00028A10
		private void ThisStockUpdated()
		{
			this.ExecuteApplyTransaction();
			this.OtherStock = this.TotalStock - this.ThisStock;
			this.IsThisStockIncreasable = this.OtherStock > 0;
			this.IsOtherStockIncreasable = this.OtherStock < this.TotalStock;
			this.UpdateProperties();
		}

		// Token: 0x06000AC6 RID: 2758 RVA: 0x0002A860 File Offset: 0x00028A60
		private void UpdateProperties()
		{
			int num = this.ThisStock - this.InitialThisStock;
			bool flag = num >= 0;
			int num2 = (flag ? num : (-num));
			if (num2 == 0)
			{
				this.PieceChange = num2.ToString();
				this.PriceChange = "0";
				this.AveragePrice = "0";
				this.IsExchangeAvailable = false;
			}
			else
			{
				int num3;
				int itemTotalPrice = this._inventoryLogic.GetItemTotalPrice(this._referenceItemRoster, num2, out num3, flag);
				this.PieceChange = (flag ? "+" : "-") + num2;
				this.PriceChange = (flag ? "-" : "+") + itemTotalPrice * num2;
				this.AveragePrice = this.GetAveragePrice(itemTotalPrice, num3, flag);
				this.IsExchangeAvailable = true;
			}
			this.PieceLbl = ((num2 <= 1) ? this._pieceLblSingular : this._pieceLblPlural);
		}

		// Token: 0x06000AC7 RID: 2759 RVA: 0x0002A944 File Offset: 0x00028B44
		public string GetAveragePrice(int totalPrice, int lastPrice, bool isBuying)
		{
			InventoryLogic.InventorySide inventorySide = (isBuying ? InventoryLogic.InventorySide.OtherInventory : InventoryLogic.InventorySide.PlayerInventory);
			int costOfItemRosterElement = this._inventoryLogic.GetCostOfItemRosterElement(this._referenceItemRoster, inventorySide);
			if (costOfItemRosterElement == lastPrice)
			{
				return costOfItemRosterElement.ToString();
			}
			if (costOfItemRosterElement < lastPrice)
			{
				return costOfItemRosterElement + " - " + lastPrice;
			}
			return lastPrice + " - " + costOfItemRosterElement;
		}

		// Token: 0x06000AC8 RID: 2760 RVA: 0x0002A9A9 File Offset: 0x00028BA9
		public void ExecuteIncreaseThisStock()
		{
			if (this.ThisStock < this.TotalStock)
			{
				this.ThisStock++;
			}
		}

		// Token: 0x06000AC9 RID: 2761 RVA: 0x0002A9C7 File Offset: 0x00028BC7
		public void ExecuteIncreaseOtherStock()
		{
			if (this.ThisStock > 0)
			{
				this.ThisStock--;
			}
		}

		// Token: 0x06000ACA RID: 2762 RVA: 0x0002A9E0 File Offset: 0x00028BE0
		public void ExecuteReset()
		{
			this.ThisStock = this.InitialThisStock;
		}

		// Token: 0x06000ACB RID: 2763 RVA: 0x0002A9F0 File Offset: 0x00028BF0
		public void ExecuteApplyTransaction()
		{
			int num = this.ThisStock - this.InitialThisStock;
			if (num == 0 || this._onApplyTransaction == null)
			{
				return;
			}
			bool flag = num >= 0;
			int num2 = (flag ? num : (-num));
			bool flag2 = (this._isPlayerItem ? flag : (!flag));
			this._onApplyTransaction(num2, flag2);
		}

		// Token: 0x17000380 RID: 896
		// (get) Token: 0x06000ACC RID: 2764 RVA: 0x0002AA45 File Offset: 0x00028C45
		// (set) Token: 0x06000ACD RID: 2765 RVA: 0x0002AA4D File Offset: 0x00028C4D
		[DataSourceProperty]
		public string ThisStockLbl
		{
			get
			{
				return this._thisStockLbl;
			}
			set
			{
				if (value != this._thisStockLbl)
				{
					this._thisStockLbl = value;
					base.OnPropertyChangedWithValue<string>(value, "ThisStockLbl");
				}
			}
		}

		// Token: 0x17000381 RID: 897
		// (get) Token: 0x06000ACE RID: 2766 RVA: 0x0002AA70 File Offset: 0x00028C70
		// (set) Token: 0x06000ACF RID: 2767 RVA: 0x0002AA78 File Offset: 0x00028C78
		[DataSourceProperty]
		public string OtherStockLbl
		{
			get
			{
				return this._otherStockLbl;
			}
			set
			{
				if (value != this._otherStockLbl)
				{
					this._otherStockLbl = value;
					base.OnPropertyChangedWithValue<string>(value, "OtherStockLbl");
				}
			}
		}

		// Token: 0x17000382 RID: 898
		// (get) Token: 0x06000AD0 RID: 2768 RVA: 0x0002AA9B File Offset: 0x00028C9B
		// (set) Token: 0x06000AD1 RID: 2769 RVA: 0x0002AAA3 File Offset: 0x00028CA3
		[DataSourceProperty]
		public string PieceLbl
		{
			get
			{
				return this._pieceLbl;
			}
			set
			{
				if (value != this._pieceLbl)
				{
					this._pieceLbl = value;
					base.OnPropertyChangedWithValue<string>(value, "PieceLbl");
				}
			}
		}

		// Token: 0x17000383 RID: 899
		// (get) Token: 0x06000AD2 RID: 2770 RVA: 0x0002AAC6 File Offset: 0x00028CC6
		// (set) Token: 0x06000AD3 RID: 2771 RVA: 0x0002AACE File Offset: 0x00028CCE
		[DataSourceProperty]
		public string AveragePriceLbl
		{
			get
			{
				return this._averagePriceLbl;
			}
			set
			{
				if (value != this._averagePriceLbl)
				{
					this._averagePriceLbl = value;
					base.OnPropertyChangedWithValue<string>(value, "AveragePriceLbl");
				}
			}
		}

		// Token: 0x17000384 RID: 900
		// (get) Token: 0x06000AD4 RID: 2772 RVA: 0x0002AAF1 File Offset: 0x00028CF1
		// (set) Token: 0x06000AD5 RID: 2773 RVA: 0x0002AAF9 File Offset: 0x00028CF9
		[DataSourceProperty]
		public HintViewModel ApplyExchangeHint
		{
			get
			{
				return this._applyExchangeHint;
			}
			set
			{
				if (value != this._applyExchangeHint)
				{
					this._applyExchangeHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "ApplyExchangeHint");
				}
			}
		}

		// Token: 0x17000385 RID: 901
		// (get) Token: 0x06000AD6 RID: 2774 RVA: 0x0002AB17 File Offset: 0x00028D17
		// (set) Token: 0x06000AD7 RID: 2775 RVA: 0x0002AB1F File Offset: 0x00028D1F
		[DataSourceProperty]
		public bool IsExchangeAvailable
		{
			get
			{
				return this._isExchangeAvailable;
			}
			set
			{
				if (value != this._isExchangeAvailable)
				{
					this._isExchangeAvailable = value;
					base.OnPropertyChangedWithValue(value, "IsExchangeAvailable");
				}
			}
		}

		// Token: 0x17000386 RID: 902
		// (get) Token: 0x06000AD8 RID: 2776 RVA: 0x0002AB3D File Offset: 0x00028D3D
		// (set) Token: 0x06000AD9 RID: 2777 RVA: 0x0002AB45 File Offset: 0x00028D45
		[DataSourceProperty]
		public string PriceChange
		{
			get
			{
				return this._priceChange;
			}
			set
			{
				if (value != this._priceChange)
				{
					this._priceChange = value;
					base.OnPropertyChangedWithValue<string>(value, "PriceChange");
				}
			}
		}

		// Token: 0x17000387 RID: 903
		// (get) Token: 0x06000ADA RID: 2778 RVA: 0x0002AB68 File Offset: 0x00028D68
		// (set) Token: 0x06000ADB RID: 2779 RVA: 0x0002AB70 File Offset: 0x00028D70
		[DataSourceProperty]
		public string PieceChange
		{
			get
			{
				return this._pieceChange;
			}
			set
			{
				if (value != this._pieceChange)
				{
					this._pieceChange = value;
					base.OnPropertyChangedWithValue<string>(value, "PieceChange");
				}
			}
		}

		// Token: 0x17000388 RID: 904
		// (get) Token: 0x06000ADC RID: 2780 RVA: 0x0002AB93 File Offset: 0x00028D93
		// (set) Token: 0x06000ADD RID: 2781 RVA: 0x0002AB9B File Offset: 0x00028D9B
		[DataSourceProperty]
		public string AveragePrice
		{
			get
			{
				return this._averagePrice;
			}
			set
			{
				if (value != this._averagePrice)
				{
					this._averagePrice = value;
					base.OnPropertyChangedWithValue<string>(value, "AveragePrice");
				}
			}
		}

		// Token: 0x17000389 RID: 905
		// (get) Token: 0x06000ADE RID: 2782 RVA: 0x0002ABBE File Offset: 0x00028DBE
		// (set) Token: 0x06000ADF RID: 2783 RVA: 0x0002ABC6 File Offset: 0x00028DC6
		[DataSourceProperty]
		public int ThisStock
		{
			get
			{
				return this._thisStock;
			}
			set
			{
				if (value != this._thisStock)
				{
					this._thisStock = value;
					base.OnPropertyChangedWithValue(value, "ThisStock");
					this.ThisStockUpdated();
				}
			}
		}

		// Token: 0x1700038A RID: 906
		// (get) Token: 0x06000AE0 RID: 2784 RVA: 0x0002ABEA File Offset: 0x00028DEA
		// (set) Token: 0x06000AE1 RID: 2785 RVA: 0x0002ABF2 File Offset: 0x00028DF2
		[DataSourceProperty]
		public int InitialThisStock
		{
			get
			{
				return this._initialThisStock;
			}
			set
			{
				if (value != this._initialThisStock)
				{
					this._initialThisStock = value;
					base.OnPropertyChangedWithValue(value, "InitialThisStock");
				}
			}
		}

		// Token: 0x1700038B RID: 907
		// (get) Token: 0x06000AE2 RID: 2786 RVA: 0x0002AC10 File Offset: 0x00028E10
		// (set) Token: 0x06000AE3 RID: 2787 RVA: 0x0002AC18 File Offset: 0x00028E18
		[DataSourceProperty]
		public int OtherStock
		{
			get
			{
				return this._otherStock;
			}
			set
			{
				if (value != this._otherStock)
				{
					this._otherStock = value;
					base.OnPropertyChangedWithValue(value, "OtherStock");
				}
			}
		}

		// Token: 0x1700038C RID: 908
		// (get) Token: 0x06000AE4 RID: 2788 RVA: 0x0002AC36 File Offset: 0x00028E36
		// (set) Token: 0x06000AE5 RID: 2789 RVA: 0x0002AC3E File Offset: 0x00028E3E
		[DataSourceProperty]
		public int InitialOtherStock
		{
			get
			{
				return this._initialOtherStock;
			}
			set
			{
				if (value != this._initialOtherStock)
				{
					this._initialOtherStock = value;
					base.OnPropertyChangedWithValue(value, "InitialOtherStock");
				}
			}
		}

		// Token: 0x1700038D RID: 909
		// (get) Token: 0x06000AE6 RID: 2790 RVA: 0x0002AC5C File Offset: 0x00028E5C
		// (set) Token: 0x06000AE7 RID: 2791 RVA: 0x0002AC64 File Offset: 0x00028E64
		[DataSourceProperty]
		public int TotalStock
		{
			get
			{
				return this._totalStock;
			}
			set
			{
				if (value != this._totalStock)
				{
					this._totalStock = value;
					base.OnPropertyChangedWithValue(value, "TotalStock");
				}
			}
		}

		// Token: 0x1700038E RID: 910
		// (get) Token: 0x06000AE8 RID: 2792 RVA: 0x0002AC82 File Offset: 0x00028E82
		// (set) Token: 0x06000AE9 RID: 2793 RVA: 0x0002AC8A File Offset: 0x00028E8A
		[DataSourceProperty]
		public bool IsThisStockIncreasable
		{
			get
			{
				return this._isThisStockIncreasable;
			}
			set
			{
				if (value != this._isThisStockIncreasable)
				{
					this._isThisStockIncreasable = value;
					base.OnPropertyChangedWithValue(value, "IsThisStockIncreasable");
				}
			}
		}

		// Token: 0x1700038F RID: 911
		// (get) Token: 0x06000AEA RID: 2794 RVA: 0x0002ACA8 File Offset: 0x00028EA8
		// (set) Token: 0x06000AEB RID: 2795 RVA: 0x0002ACB0 File Offset: 0x00028EB0
		[DataSourceProperty]
		public bool IsOtherStockIncreasable
		{
			get
			{
				return this._isOtherStockIncreasable;
			}
			set
			{
				if (value != this._isOtherStockIncreasable)
				{
					this._isOtherStockIncreasable = value;
					base.OnPropertyChangedWithValue(value, "IsOtherStockIncreasable");
				}
			}
		}

		// Token: 0x17000390 RID: 912
		// (get) Token: 0x06000AEC RID: 2796 RVA: 0x0002ACCE File Offset: 0x00028ECE
		// (set) Token: 0x06000AED RID: 2797 RVA: 0x0002ACD6 File Offset: 0x00028ED6
		[DataSourceProperty]
		public bool IsTrading
		{
			get
			{
				return this._isTrading;
			}
			set
			{
				if (value != this._isTrading)
				{
					this._isTrading = value;
					base.OnPropertyChangedWithValue(value, "IsTrading");
				}
			}
		}

		// Token: 0x17000391 RID: 913
		// (get) Token: 0x06000AEE RID: 2798 RVA: 0x0002ACF4 File Offset: 0x00028EF4
		// (set) Token: 0x06000AEF RID: 2799 RVA: 0x0002ACFC File Offset: 0x00028EFC
		[DataSourceProperty]
		public bool IsTradeable
		{
			get
			{
				return this._isTradeable;
			}
			set
			{
				if (value != this._isTradeable)
				{
					this._isTradeable = value;
					base.OnPropertyChangedWithValue(value, "IsTradeable");
				}
			}
		}

		// Token: 0x040004DB RID: 1243
		private InventoryLogic _inventoryLogic;

		// Token: 0x040004DC RID: 1244
		private ItemRosterElement _referenceItemRoster;

		// Token: 0x040004DD RID: 1245
		private Action<int, bool> _onApplyTransaction;

		// Token: 0x040004DE RID: 1246
		private string _pieceLblSingular;

		// Token: 0x040004DF RID: 1247
		private string _pieceLblPlural;

		// Token: 0x040004E0 RID: 1248
		private bool _isPlayerItem;

		// Token: 0x040004E1 RID: 1249
		private string _thisStockLbl;

		// Token: 0x040004E2 RID: 1250
		private string _otherStockLbl;

		// Token: 0x040004E3 RID: 1251
		private string _averagePriceLbl;

		// Token: 0x040004E4 RID: 1252
		private string _pieceLbl;

		// Token: 0x040004E5 RID: 1253
		private HintViewModel _applyExchangeHint;

		// Token: 0x040004E6 RID: 1254
		private bool _isExchangeAvailable;

		// Token: 0x040004E7 RID: 1255
		private string _averagePrice;

		// Token: 0x040004E8 RID: 1256
		private string _pieceChange;

		// Token: 0x040004E9 RID: 1257
		private string _priceChange;

		// Token: 0x040004EA RID: 1258
		private int _thisStock = -1;

		// Token: 0x040004EB RID: 1259
		private int _initialThisStock;

		// Token: 0x040004EC RID: 1260
		private int _otherStock = -1;

		// Token: 0x040004ED RID: 1261
		private int _initialOtherStock;

		// Token: 0x040004EE RID: 1262
		private int _totalStock;

		// Token: 0x040004EF RID: 1263
		private bool _isThisStockIncreasable;

		// Token: 0x040004F0 RID: 1264
		private bool _isOtherStockIncreasable;

		// Token: 0x040004F1 RID: 1265
		private bool _isTrading;

		// Token: 0x040004F2 RID: 1266
		private bool _isTradeable;
	}
}
