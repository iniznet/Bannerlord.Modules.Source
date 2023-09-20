using System;
using TaleWorlds.CampaignSystem.Inventory;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.Inventory
{
	public class InventoryTradeVM : ViewModel
	{
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

		private ItemRosterElement? FindItemFromSide(EquipmentElement item, InventoryLogic.InventorySide side)
		{
			return this._inventoryLogic.FindItemFromSide(side, item);
		}

		private void ThisStockUpdated()
		{
			this.ExecuteApplyTransaction();
			this.OtherStock = this.TotalStock - this.ThisStock;
			this.IsThisStockIncreasable = this.OtherStock > 0;
			this.IsOtherStockIncreasable = this.OtherStock < this.TotalStock;
			this.UpdateProperties();
		}

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

		public void ExecuteIncreaseThisStock()
		{
			if (this.ThisStock < this.TotalStock)
			{
				this.ThisStock++;
			}
		}

		public void ExecuteIncreaseOtherStock()
		{
			if (this.ThisStock > 0)
			{
				this.ThisStock--;
			}
		}

		public void ExecuteReset()
		{
			this.ThisStock = this.InitialThisStock;
		}

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

		private InventoryLogic _inventoryLogic;

		private ItemRosterElement _referenceItemRoster;

		private Action<int, bool> _onApplyTransaction;

		private string _pieceLblSingular;

		private string _pieceLblPlural;

		private bool _isPlayerItem;

		private string _thisStockLbl;

		private string _otherStockLbl;

		private string _averagePriceLbl;

		private string _pieceLbl;

		private HintViewModel _applyExchangeHint;

		private bool _isExchangeAvailable;

		private string _averagePrice;

		private string _pieceChange;

		private string _priceChange;

		private int _thisStock = -1;

		private int _initialThisStock;

		private int _otherStock = -1;

		private int _initialOtherStock;

		private int _totalStock;

		private bool _isThisStockIncreasable;

		private bool _isOtherStockIncreasable;

		private bool _isTrading;

		private bool _isTradeable;
	}
}
