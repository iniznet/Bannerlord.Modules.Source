using System;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.Party
{
	// Token: 0x02000027 RID: 39
	public class PartyTradeVM : ViewModel
	{
		// Token: 0x060002F3 RID: 755 RVA: 0x00013220 File Offset: 0x00011420
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

		// Token: 0x060002F4 RID: 756 RVA: 0x00013284 File Offset: 0x00011484
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.ThisStockLbl = GameTexts.FindText("str_party_your_party", null).ToString();
			this.TotalStockLbl = GameTexts.FindText("str_party_total_units", null).ToString();
		}

		// Token: 0x060002F5 RID: 757 RVA: 0x000132B8 File Offset: 0x000114B8
		public void UpdateTroopData(TroopRosterElement troopRoster, PartyScreenLogic.PartyRosterSide side, bool forceUpdate = true)
		{
			if (side != PartyScreenLogic.PartyRosterSide.Left && side != PartyScreenLogic.PartyRosterSide.Right)
			{
				Debug.FailedAssert("Troop has to be either from left or right side", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.CampaignSystem.ViewModelCollection\\Party\\PartyTradeVM.cs", "UpdateTroopData", 49);
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

		// Token: 0x060002F6 RID: 758 RVA: 0x0001338C File Offset: 0x0001158C
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

		// Token: 0x060002F7 RID: 759 RVA: 0x000133DC File Offset: 0x000115DC
		private void ThisStockUpdated()
		{
			this.ExecuteApplyTransaction();
			this.OtherStock = this.TotalStock - this.ThisStock;
			this.IsThisStockIncreasable = this.OtherStock > 0;
			this.IsOtherStockIncreasable = this.OtherStock < this.TotalStock && this.IsTransfarable;
		}

		// Token: 0x060002F8 RID: 760 RVA: 0x0001342E File Offset: 0x0001162E
		public void ExecuteIncreasePlayerStock()
		{
			if (this.OtherStock > 0)
			{
				this.ThisStock++;
			}
		}

		// Token: 0x060002F9 RID: 761 RVA: 0x00013447 File Offset: 0x00011647
		public void ExecuteIncreaseOtherStock()
		{
			if (this.OtherStock < this.TotalStock)
			{
				this.ThisStock--;
			}
		}

		// Token: 0x060002FA RID: 762 RVA: 0x00013465 File Offset: 0x00011665
		public void ExecuteReset()
		{
			this.OtherStock = this.InitialOtherStock;
		}

		// Token: 0x060002FB RID: 763 RVA: 0x00013474 File Offset: 0x00011674
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

		// Token: 0x170000CC RID: 204
		// (get) Token: 0x060002FC RID: 764 RVA: 0x000134E4 File Offset: 0x000116E4
		// (set) Token: 0x060002FD RID: 765 RVA: 0x000134EC File Offset: 0x000116EC
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

		// Token: 0x170000CD RID: 205
		// (get) Token: 0x060002FE RID: 766 RVA: 0x0001350A File Offset: 0x0001170A
		// (set) Token: 0x060002FF RID: 767 RVA: 0x00013512 File Offset: 0x00011712
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

		// Token: 0x170000CE RID: 206
		// (get) Token: 0x06000300 RID: 768 RVA: 0x00013535 File Offset: 0x00011735
		// (set) Token: 0x06000301 RID: 769 RVA: 0x0001353D File Offset: 0x0001173D
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

		// Token: 0x170000CF RID: 207
		// (get) Token: 0x06000302 RID: 770 RVA: 0x00013560 File Offset: 0x00011760
		// (set) Token: 0x06000303 RID: 771 RVA: 0x00013568 File Offset: 0x00011768
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

		// Token: 0x170000D0 RID: 208
		// (get) Token: 0x06000304 RID: 772 RVA: 0x0001358C File Offset: 0x0001178C
		// (set) Token: 0x06000305 RID: 773 RVA: 0x00013594 File Offset: 0x00011794
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

		// Token: 0x170000D1 RID: 209
		// (get) Token: 0x06000306 RID: 774 RVA: 0x000135B2 File Offset: 0x000117B2
		// (set) Token: 0x06000307 RID: 775 RVA: 0x000135BA File Offset: 0x000117BA
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

		// Token: 0x170000D2 RID: 210
		// (get) Token: 0x06000308 RID: 776 RVA: 0x000135D8 File Offset: 0x000117D8
		// (set) Token: 0x06000309 RID: 777 RVA: 0x000135E0 File Offset: 0x000117E0
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

		// Token: 0x170000D3 RID: 211
		// (get) Token: 0x0600030A RID: 778 RVA: 0x000135FE File Offset: 0x000117FE
		// (set) Token: 0x0600030B RID: 779 RVA: 0x00013606 File Offset: 0x00011806
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

		// Token: 0x170000D4 RID: 212
		// (get) Token: 0x0600030C RID: 780 RVA: 0x00013624 File Offset: 0x00011824
		// (set) Token: 0x0600030D RID: 781 RVA: 0x0001362C File Offset: 0x0001182C
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

		// Token: 0x170000D5 RID: 213
		// (get) Token: 0x0600030E RID: 782 RVA: 0x0001364A File Offset: 0x0001184A
		// (set) Token: 0x0600030F RID: 783 RVA: 0x00013652 File Offset: 0x00011852
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

		// Token: 0x04000153 RID: 339
		private readonly PartyScreenLogic _partyScreenLogic;

		// Token: 0x04000154 RID: 340
		private readonly Action<int, bool> _onApplyTransaction;

		// Token: 0x04000155 RID: 341
		private readonly bool _isPrisoner;

		// Token: 0x04000156 RID: 342
		private TroopRosterElement _referenceTroopRoster;

		// Token: 0x04000157 RID: 343
		private readonly PartyScreenLogic.PartyRosterSide _side;

		// Token: 0x04000158 RID: 344
		private PartyScreenLogic.PartyRosterSide _otherSide;

		// Token: 0x04000159 RID: 345
		private bool _isTransfarable;

		// Token: 0x0400015A RID: 346
		private string _thisStockLbl;

		// Token: 0x0400015B RID: 347
		private string _totalStockLbl;

		// Token: 0x0400015C RID: 348
		private int _thisStock = -1;

		// Token: 0x0400015D RID: 349
		private int _initialThisStock;

		// Token: 0x0400015E RID: 350
		private int _otherStock;

		// Token: 0x0400015F RID: 351
		private int _initialOtherStock;

		// Token: 0x04000160 RID: 352
		private int _totalStock;

		// Token: 0x04000161 RID: 353
		private bool _isThisStockIncreasable;

		// Token: 0x04000162 RID: 354
		private bool _isOtherStockIncreasable;
	}
}
