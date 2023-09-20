using System;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.Party
{
	public class PartyTradeVM : ViewModel
	{
		public static event Action RemoveZeroCounts;

		public PartyTradeVM(PartyScreenLogic partyScreenLogic, TroopRosterElement troopRoster, PartyScreenLogic.PartyRosterSide side, bool isTransfarable, bool isPrisoner, Action<int, bool> onApplyTransaction)
		{
			this._partyScreenLogic = partyScreenLogic;
			this._referenceTroopRoster = troopRoster;
			this._side = side;
			this._onApplyTransaction = onApplyTransaction;
			this._otherSide = ((side == PartyScreenLogic.PartyRosterSide.Right) ? PartyScreenLogic.PartyRosterSide.Left : PartyScreenLogic.PartyRosterSide.Right);
			this.IsTransfarable = isTransfarable;
			this._isPrisoner = isPrisoner;
			this.UpdateTroopData(troopRoster, side, true);
			this.RefreshValues();
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
			this.ThisStockLbl = GameTexts.FindText("str_party_your_party", null).ToString();
			this.TotalStockLbl = GameTexts.FindText("str_party_total_units", null).ToString();
		}

		public void UpdateTroopData(TroopRosterElement troopRoster, PartyScreenLogic.PartyRosterSide side, bool forceUpdate = true)
		{
			if (side != PartyScreenLogic.PartyRosterSide.Left && side != PartyScreenLogic.PartyRosterSide.Right)
			{
				Debug.FailedAssert("Troop has to be either from left or right side", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.CampaignSystem.ViewModelCollection\\Party\\PartyTradeVM.cs", "UpdateTroopData", 50);
				return;
			}
			TroopRosterElement? troopRosterElement = null;
			TroopRosterElement? troopRosterElement2 = null;
			troopRosterElement = new TroopRosterElement?(troopRoster);
			troopRosterElement2 = this.FindTroopFromSide(troopRoster.Character, this._otherSide, this._isPrisoner);
			this.InitialThisStock = ((troopRosterElement != null) ? troopRosterElement.GetValueOrDefault().Number : 0);
			this.InitialOtherStock = ((troopRosterElement2 != null) ? troopRosterElement2.GetValueOrDefault().Number : 0);
			this.TotalStock = this.InitialThisStock + this.InitialOtherStock;
			this.ThisStock = this.InitialThisStock;
			this.OtherStock = this.InitialOtherStock;
			if (forceUpdate)
			{
				this.ThisStockUpdated();
			}
		}

		public TroopRosterElement? FindTroopFromSide(CharacterObject character, PartyScreenLogic.PartyRosterSide side, bool isPrisoner)
		{
			TroopRosterElement? troopRosterElement = null;
			TroopRoster[] array = (isPrisoner ? this._partyScreenLogic.PrisonerRosters : this._partyScreenLogic.MemberRosters);
			int num = array[(int)side].FindIndexOfTroop(character);
			if (num >= 0)
			{
				troopRosterElement = new TroopRosterElement?(array[(int)side].GetElementCopyAtIndex(num));
			}
			return troopRosterElement;
		}

		private void ThisStockUpdated()
		{
			this.ExecuteApplyTransaction();
			this.OtherStock = this.TotalStock - this.ThisStock;
			this.IsThisStockIncreasable = this.OtherStock > 0;
			this.IsOtherStockIncreasable = this.OtherStock < this.TotalStock && this.IsTransfarable;
		}

		public void ExecuteIncreasePlayerStock()
		{
			if (this.OtherStock > 0)
			{
				this.ThisStock++;
			}
		}

		public void ExecuteIncreaseOtherStock()
		{
			if (this.OtherStock < this.TotalStock)
			{
				this.ThisStock--;
			}
		}

		public void ExecuteReset()
		{
			this.OtherStock = this.InitialOtherStock;
		}

		public void ExecuteApplyTransaction()
		{
			int num = this.ThisStock - this.InitialThisStock;
			bool flag = (num >= 0 && this._side == PartyScreenLogic.PartyRosterSide.Right) || (num <= 0 && this._side == PartyScreenLogic.PartyRosterSide.Left);
			if (num == 0 || this._onApplyTransaction == null)
			{
				return;
			}
			if (num < 0)
			{
				PartyScreenLogic.PartyRosterSide otherSide = this._otherSide;
			}
			else
			{
				PartyScreenLogic.PartyRosterSide side = this._side;
			}
			int num2 = MathF.Abs(num);
			this._onApplyTransaction(num2, flag);
		}

		public void ExecuteRemoveZeroCounts()
		{
			Action removeZeroCounts = PartyTradeVM.RemoveZeroCounts;
			if (removeZeroCounts == null)
			{
				return;
			}
			removeZeroCounts();
		}

		[DataSourceProperty]
		public bool IsTransfarable
		{
			get
			{
				return this._isTransfarable;
			}
			set
			{
				if (value != this._isTransfarable)
				{
					this._isTransfarable = value;
					base.OnPropertyChangedWithValue(value, "IsTransfarable");
				}
			}
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
		public string TotalStockLbl
		{
			get
			{
				return this._totalStockLbl;
			}
			set
			{
				if (value != this._totalStockLbl)
				{
					this._totalStockLbl = value;
					base.OnPropertyChangedWithValue<string>(value, "TotalStockLbl");
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

		private readonly PartyScreenLogic _partyScreenLogic;

		private readonly Action<int, bool> _onApplyTransaction;

		private readonly bool _isPrisoner;

		private TroopRosterElement _referenceTroopRoster;

		private readonly PartyScreenLogic.PartyRosterSide _side;

		private PartyScreenLogic.PartyRosterSide _otherSide;

		private bool _isTransfarable;

		private string _thisStockLbl;

		private string _totalStockLbl;

		private int _thisStock = -1;

		private int _initialThisStock;

		private int _otherStock;

		private int _initialOtherStock;

		private int _totalStock;

		private bool _isThisStockIncreasable;

		private bool _isOtherStockIncreasable;
	}
}
