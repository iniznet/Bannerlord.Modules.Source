using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Settlements.Workshops;
using TaleWorlds.CampaignSystem.ViewModelCollection.ClanManagement.ClanFinance;
using TaleWorlds.CampaignSystem.ViewModelCollection.ClanManagement.Supporters;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.ClanManagement.Categories
{
	// Token: 0x02000113 RID: 275
	public class ClanIncomeVM : ViewModel
	{
		// Token: 0x170008EF RID: 2287
		// (get) Token: 0x06001A1C RID: 6684 RVA: 0x0005E7B4 File Offset: 0x0005C9B4
		// (set) Token: 0x06001A1D RID: 6685 RVA: 0x0005E7BC File Offset: 0x0005C9BC
		public int TotalIncome { get; private set; }

		// Token: 0x06001A1E RID: 6686 RVA: 0x0005E7C8 File Offset: 0x0005C9C8
		public ClanIncomeVM(Action onRefresh, Action<ClanCardSelectionInfo> openCardSelectionPopup)
		{
			this._onRefresh = onRefresh;
			this._openCardSelectionPopup = openCardSelectionPopup;
			this._clan = Hero.MainHero.Clan;
			this.Incomes = new MBBindingList<ClanFinanceWorkshopItemVM>();
			this.SupporterGroups = new MBBindingList<ClanSupporterGroupVM>();
			this.Alleys = new MBBindingList<ClanFinanceAlleyItemVM>();
			this.RefreshList();
		}

		// Token: 0x06001A1F RID: 6687 RVA: 0x0005E820 File Offset: 0x0005CA20
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.NameText = GameTexts.FindText("str_sort_by_name_label", null).ToString();
			this.IncomeText = GameTexts.FindText("str_income", null).ToString();
			this.LocationText = GameTexts.FindText("str_tooltip_label_location", null).ToString();
			this.NoAdditionalIncomesText = GameTexts.FindText("str_clan_no_additional_incomes", null).ToString();
			this.Incomes.ApplyActionOnAllItems(delegate(ClanFinanceWorkshopItemVM x)
			{
				x.RefreshValues();
			});
			ClanFinanceWorkshopItemVM currentSelectedIncome = this.CurrentSelectedIncome;
			if (currentSelectedIncome == null)
			{
				return;
			}
			currentSelectedIncome.RefreshValues();
		}

		// Token: 0x06001A20 RID: 6688 RVA: 0x0005E8C8 File Offset: 0x0005CAC8
		public void RefreshList()
		{
			this.Incomes.Clear();
			foreach (Settlement settlement in Settlement.All)
			{
				if (settlement.IsTown)
				{
					foreach (Workshop workshop in settlement.Town.Workshops)
					{
						if (workshop.Owner == Hero.MainHero)
						{
							this.Incomes.Add(new ClanFinanceWorkshopItemVM(workshop, new Action<ClanFinanceWorkshopItemVM>(this.OnIncomeSelection), new Action(this.OnRefresh), this._openCardSelectionPopup));
						}
					}
				}
			}
			this.RefreshSupporters();
			this.RefreshAlleys();
			GameTexts.SetVariable("STR1", GameTexts.FindText("str_clan_workshops", null));
			GameTexts.SetVariable("LEFT", Hero.MainHero.OwnedWorkshops.Count);
			GameTexts.SetVariable("RIGHT", Campaign.Current.Models.WorkshopModel.GetMaxWorkshopCountForTier(Clan.PlayerClan.Tier));
			GameTexts.SetVariable("STR2", GameTexts.FindText("str_LEFT_over_RIGHT_in_paranthesis", null));
			this.WorkshopText = GameTexts.FindText("str_STR1_space_STR2", null).ToString();
			this.SupportersText = new TextObject("{=RzFyGnWJ}Supporters", null).ToString();
			this.AlleysText = new TextObject("{=7tKjfMSb}Alleys", null).ToString();
			this.RefreshTotalIncome();
			this.OnIncomeSelection(this.GetDefaultIncome());
			this.RefreshValues();
		}

		// Token: 0x06001A21 RID: 6689 RVA: 0x0005EA54 File Offset: 0x0005CC54
		private void RefreshSupporters()
		{
			foreach (ClanSupporterGroupVM clanSupporterGroupVM in this.SupporterGroups)
			{
				clanSupporterGroupVM.Supporters.Clear();
			}
			this.SupporterGroups.Clear();
			Dictionary<float, List<Hero>> dictionary = new Dictionary<float, List<Hero>>();
			NotablePowerModel notablePowerModel = Campaign.Current.Models.NotablePowerModel;
			foreach (Hero hero in Clan.PlayerClan.SupporterNotables.OrderBy((Hero x) => x.Power))
			{
				if (hero.CurrentSettlement != null)
				{
					float influenceBonusToClan = notablePowerModel.GetInfluenceBonusToClan(hero);
					List<Hero> list;
					if (dictionary.TryGetValue(influenceBonusToClan, out list))
					{
						list.Add(hero);
					}
					else
					{
						dictionary.Add(influenceBonusToClan, new List<Hero> { hero });
					}
				}
			}
			foreach (KeyValuePair<float, List<Hero>> keyValuePair in dictionary)
			{
				if (keyValuePair.Value.Count > 0)
				{
					ClanSupporterGroupVM clanSupporterGroupVM2 = new ClanSupporterGroupVM(notablePowerModel.GetPowerRankName(keyValuePair.Value.FirstOrDefault<Hero>()), keyValuePair.Key, new Action<ClanSupporterGroupVM>(this.OnSupporterSelection));
					foreach (Hero hero2 in keyValuePair.Value)
					{
						clanSupporterGroupVM2.AddSupporter(hero2);
					}
					this.SupporterGroups.Add(clanSupporterGroupVM2);
				}
			}
			foreach (ClanSupporterGroupVM clanSupporterGroupVM3 in this.SupporterGroups)
			{
				clanSupporterGroupVM3.Refresh();
			}
		}

		// Token: 0x06001A22 RID: 6690 RVA: 0x0005EC68 File Offset: 0x0005CE68
		private void RefreshAlleys()
		{
			this.Alleys.Clear();
			foreach (Alley alley in Hero.MainHero.OwnedAlleys)
			{
				this.Alleys.Add(new ClanFinanceAlleyItemVM(alley, this._openCardSelectionPopup, new Action<ClanFinanceAlleyItemVM>(this.OnAlleySelection), new Action(this.OnRefresh)));
			}
		}

		// Token: 0x06001A23 RID: 6691 RVA: 0x0005ECF4 File Offset: 0x0005CEF4
		private ClanFinanceWorkshopItemVM GetDefaultIncome()
		{
			return this.Incomes.FirstOrDefault<ClanFinanceWorkshopItemVM>();
		}

		// Token: 0x06001A24 RID: 6692 RVA: 0x0005ED04 File Offset: 0x0005CF04
		public void SelectWorkshop(Workshop workshop)
		{
			foreach (ClanFinanceWorkshopItemVM clanFinanceWorkshopItemVM in this.Incomes)
			{
				if (clanFinanceWorkshopItemVM != null)
				{
					ClanFinanceWorkshopItemVM clanFinanceWorkshopItemVM2 = clanFinanceWorkshopItemVM;
					if (clanFinanceWorkshopItemVM2.Workshop == workshop)
					{
						this.OnIncomeSelection(clanFinanceWorkshopItemVM2);
						break;
					}
				}
			}
		}

		// Token: 0x06001A25 RID: 6693 RVA: 0x0005ED64 File Offset: 0x0005CF64
		public void SelectAlley(Alley alley)
		{
			for (int i = 0; i < this.Alleys.Count; i++)
			{
				if (this.Alleys[i].Alley == alley)
				{
					this.OnAlleySelection(this.Alleys[i]);
					return;
				}
			}
		}

		// Token: 0x06001A26 RID: 6694 RVA: 0x0005EDB0 File Offset: 0x0005CFB0
		private void OnAlleySelection(ClanFinanceAlleyItemVM alley)
		{
			if (alley == null)
			{
				if (this.CurrentSelectedAlley != null)
				{
					this.CurrentSelectedAlley.IsSelected = false;
				}
				this.CurrentSelectedAlley = null;
				return;
			}
			this.OnAlleySelection(null);
			if (this.CurrentSelectedAlley != null)
			{
				this.CurrentSelectedAlley.IsSelected = false;
			}
			this.CurrentSelectedAlley = alley;
			if (alley != null)
			{
				alley.IsSelected = true;
			}
		}

		// Token: 0x06001A27 RID: 6695 RVA: 0x0005EE08 File Offset: 0x0005D008
		private void OnIncomeSelection(ClanFinanceWorkshopItemVM income)
		{
			if (income == null)
			{
				if (this.CurrentSelectedIncome != null)
				{
					this.CurrentSelectedIncome.IsSelected = false;
				}
				this.CurrentSelectedIncome = null;
				return;
			}
			this.OnSupporterSelection(null);
			if (this.CurrentSelectedIncome != null)
			{
				this.CurrentSelectedIncome.IsSelected = false;
			}
			this.CurrentSelectedIncome = income;
			if (income != null)
			{
				income.IsSelected = true;
			}
		}

		// Token: 0x06001A28 RID: 6696 RVA: 0x0005EE60 File Offset: 0x0005D060
		private void OnSupporterSelection(ClanSupporterGroupVM supporter)
		{
			if (supporter == null)
			{
				if (this.CurrentSelectedSupporterGroup != null)
				{
					this.CurrentSelectedSupporterGroup.IsSelected = false;
				}
				this.CurrentSelectedSupporterGroup = null;
				return;
			}
			this.OnIncomeSelection(null);
			if (this.CurrentSelectedSupporterGroup != null)
			{
				this.CurrentSelectedSupporterGroup.IsSelected = false;
			}
			this.CurrentSelectedSupporterGroup = supporter;
			if (this.CurrentSelectedSupporterGroup != null)
			{
				this.CurrentSelectedSupporterGroup.IsSelected = true;
			}
		}

		// Token: 0x06001A29 RID: 6697 RVA: 0x0005EEC2 File Offset: 0x0005D0C2
		public void RefreshTotalIncome()
		{
			this.TotalIncome = this.Incomes.Sum((ClanFinanceWorkshopItemVM i) => i.Income);
		}

		// Token: 0x06001A2A RID: 6698 RVA: 0x0005EEF4 File Offset: 0x0005D0F4
		public void OnRefresh()
		{
			Action onRefresh = this._onRefresh;
			if (onRefresh == null)
			{
				return;
			}
			onRefresh();
		}

		// Token: 0x170008F0 RID: 2288
		// (get) Token: 0x06001A2B RID: 6699 RVA: 0x0005EF06 File Offset: 0x0005D106
		// (set) Token: 0x06001A2C RID: 6700 RVA: 0x0005EF0E File Offset: 0x0005D10E
		[DataSourceProperty]
		public ClanFinanceAlleyItemVM CurrentSelectedAlley
		{
			get
			{
				return this._currentSelectedAlley;
			}
			set
			{
				if (value != this._currentSelectedAlley)
				{
					this._currentSelectedAlley = value;
					base.OnPropertyChangedWithValue<ClanFinanceAlleyItemVM>(value, "CurrentSelectedAlley");
					this.IsAnyValidAlleySelected = value != null;
					this.IsAnyValidIncomeSelected = false;
					this.IsAnyValidSupporterSelected = false;
				}
			}
		}

		// Token: 0x170008F1 RID: 2289
		// (get) Token: 0x06001A2D RID: 6701 RVA: 0x0005EF44 File Offset: 0x0005D144
		// (set) Token: 0x06001A2E RID: 6702 RVA: 0x0005EF4C File Offset: 0x0005D14C
		[DataSourceProperty]
		public ClanFinanceWorkshopItemVM CurrentSelectedIncome
		{
			get
			{
				return this._currentSelectedIncome;
			}
			set
			{
				if (value != this._currentSelectedIncome)
				{
					this._currentSelectedIncome = value;
					base.OnPropertyChangedWithValue<ClanFinanceWorkshopItemVM>(value, "CurrentSelectedIncome");
					this.IsAnyValidIncomeSelected = value != null;
					this.IsAnyValidSupporterSelected = false;
					this.IsAnyValidAlleySelected = false;
				}
			}
		}

		// Token: 0x170008F2 RID: 2290
		// (get) Token: 0x06001A2F RID: 6703 RVA: 0x0005EF82 File Offset: 0x0005D182
		// (set) Token: 0x06001A30 RID: 6704 RVA: 0x0005EF8A File Offset: 0x0005D18A
		[DataSourceProperty]
		public ClanSupporterGroupVM CurrentSelectedSupporterGroup
		{
			get
			{
				return this._currentSelectedSupporterGroup;
			}
			set
			{
				if (value != this._currentSelectedSupporterGroup)
				{
					this._currentSelectedSupporterGroup = value;
					base.OnPropertyChangedWithValue<ClanSupporterGroupVM>(value, "CurrentSelectedSupporterGroup");
					this.IsAnyValidSupporterSelected = value != null;
					this.IsAnyValidIncomeSelected = false;
					this.IsAnyValidAlleySelected = false;
				}
			}
		}

		// Token: 0x170008F3 RID: 2291
		// (get) Token: 0x06001A31 RID: 6705 RVA: 0x0005EFC0 File Offset: 0x0005D1C0
		// (set) Token: 0x06001A32 RID: 6706 RVA: 0x0005EFC8 File Offset: 0x0005D1C8
		[DataSourceProperty]
		public bool IsAnyValidAlleySelected
		{
			get
			{
				return this._isAnyValidAlleySelected;
			}
			set
			{
				if (value != this._isAnyValidAlleySelected)
				{
					this._isAnyValidAlleySelected = value;
					base.OnPropertyChangedWithValue(value, "IsAnyValidAlleySelected");
				}
			}
		}

		// Token: 0x170008F4 RID: 2292
		// (get) Token: 0x06001A33 RID: 6707 RVA: 0x0005EFE6 File Offset: 0x0005D1E6
		// (set) Token: 0x06001A34 RID: 6708 RVA: 0x0005EFEE File Offset: 0x0005D1EE
		[DataSourceProperty]
		public bool IsAnyValidIncomeSelected
		{
			get
			{
				return this._isAnyValidIncomeSelected;
			}
			set
			{
				if (value != this._isAnyValidIncomeSelected)
				{
					this._isAnyValidIncomeSelected = value;
					base.OnPropertyChangedWithValue(value, "IsAnyValidIncomeSelected");
				}
			}
		}

		// Token: 0x170008F5 RID: 2293
		// (get) Token: 0x06001A35 RID: 6709 RVA: 0x0005F00C File Offset: 0x0005D20C
		// (set) Token: 0x06001A36 RID: 6710 RVA: 0x0005F014 File Offset: 0x0005D214
		[DataSourceProperty]
		public bool IsAnyValidSupporterSelected
		{
			get
			{
				return this._isAnyValidSupporterSelected;
			}
			set
			{
				if (value != this._isAnyValidSupporterSelected)
				{
					this._isAnyValidSupporterSelected = value;
					base.OnPropertyChangedWithValue(value, "IsAnyValidSupporterSelected");
				}
			}
		}

		// Token: 0x170008F6 RID: 2294
		// (get) Token: 0x06001A37 RID: 6711 RVA: 0x0005F032 File Offset: 0x0005D232
		// (set) Token: 0x06001A38 RID: 6712 RVA: 0x0005F03A File Offset: 0x0005D23A
		[DataSourceProperty]
		public string IncomeText
		{
			get
			{
				return this._incomeText;
			}
			set
			{
				if (value != this._incomeText)
				{
					this._incomeText = value;
					base.OnPropertyChangedWithValue<string>(value, "IncomeText");
				}
			}
		}

		// Token: 0x170008F7 RID: 2295
		// (get) Token: 0x06001A39 RID: 6713 RVA: 0x0005F05D File Offset: 0x0005D25D
		// (set) Token: 0x06001A3A RID: 6714 RVA: 0x0005F065 File Offset: 0x0005D265
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

		// Token: 0x170008F8 RID: 2296
		// (get) Token: 0x06001A3B RID: 6715 RVA: 0x0005F083 File Offset: 0x0005D283
		// (set) Token: 0x06001A3C RID: 6716 RVA: 0x0005F08B File Offset: 0x0005D28B
		[DataSourceProperty]
		public string NameText
		{
			get
			{
				return this._nameText;
			}
			set
			{
				if (value != this._nameText)
				{
					this._nameText = value;
					base.OnPropertyChangedWithValue<string>(value, "NameText");
				}
			}
		}

		// Token: 0x170008F9 RID: 2297
		// (get) Token: 0x06001A3D RID: 6717 RVA: 0x0005F0AE File Offset: 0x0005D2AE
		// (set) Token: 0x06001A3E RID: 6718 RVA: 0x0005F0B6 File Offset: 0x0005D2B6
		[DataSourceProperty]
		public string LocationText
		{
			get
			{
				return this._locationText;
			}
			set
			{
				if (value != this._locationText)
				{
					this._locationText = value;
					base.OnPropertyChangedWithValue<string>(value, "LocationText");
				}
			}
		}

		// Token: 0x170008FA RID: 2298
		// (get) Token: 0x06001A3F RID: 6719 RVA: 0x0005F0D9 File Offset: 0x0005D2D9
		// (set) Token: 0x06001A40 RID: 6720 RVA: 0x0005F0E1 File Offset: 0x0005D2E1
		[DataSourceProperty]
		public string WorkshopText
		{
			get
			{
				return this._workshopsText;
			}
			set
			{
				if (value != this._workshopsText)
				{
					this._workshopsText = value;
					base.OnPropertyChangedWithValue<string>(value, "WorkshopText");
				}
			}
		}

		// Token: 0x170008FB RID: 2299
		// (get) Token: 0x06001A41 RID: 6721 RVA: 0x0005F104 File Offset: 0x0005D304
		// (set) Token: 0x06001A42 RID: 6722 RVA: 0x0005F10C File Offset: 0x0005D30C
		[DataSourceProperty]
		public string SupportersText
		{
			get
			{
				return this._supportersText;
			}
			set
			{
				if (value != this._supportersText)
				{
					this._supportersText = value;
					base.OnPropertyChangedWithValue<string>(value, "SupportersText");
				}
			}
		}

		// Token: 0x170008FC RID: 2300
		// (get) Token: 0x06001A43 RID: 6723 RVA: 0x0005F12F File Offset: 0x0005D32F
		// (set) Token: 0x06001A44 RID: 6724 RVA: 0x0005F137 File Offset: 0x0005D337
		[DataSourceProperty]
		public string AlleysText
		{
			get
			{
				return this._alleysText;
			}
			set
			{
				if (value != this._alleysText)
				{
					this._alleysText = value;
					base.OnPropertyChangedWithValue<string>(value, "AlleysText");
				}
			}
		}

		// Token: 0x170008FD RID: 2301
		// (get) Token: 0x06001A45 RID: 6725 RVA: 0x0005F15A File Offset: 0x0005D35A
		// (set) Token: 0x06001A46 RID: 6726 RVA: 0x0005F162 File Offset: 0x0005D362
		[DataSourceProperty]
		public string NoAdditionalIncomesText
		{
			get
			{
				return this._noAdditionalIncomesText;
			}
			set
			{
				if (this._noAdditionalIncomesText != value)
				{
					this._noAdditionalIncomesText = value;
					base.OnPropertyChangedWithValue<string>(value, "NoAdditionalIncomesText");
				}
			}
		}

		// Token: 0x170008FE RID: 2302
		// (get) Token: 0x06001A47 RID: 6727 RVA: 0x0005F185 File Offset: 0x0005D385
		// (set) Token: 0x06001A48 RID: 6728 RVA: 0x0005F18D File Offset: 0x0005D38D
		[DataSourceProperty]
		public MBBindingList<ClanFinanceWorkshopItemVM> Incomes
		{
			get
			{
				return this._incomes;
			}
			set
			{
				if (value != this._incomes)
				{
					this._incomes = value;
					base.OnPropertyChangedWithValue<MBBindingList<ClanFinanceWorkshopItemVM>>(value, "Incomes");
				}
			}
		}

		// Token: 0x170008FF RID: 2303
		// (get) Token: 0x06001A49 RID: 6729 RVA: 0x0005F1AB File Offset: 0x0005D3AB
		// (set) Token: 0x06001A4A RID: 6730 RVA: 0x0005F1B3 File Offset: 0x0005D3B3
		[DataSourceProperty]
		public MBBindingList<ClanSupporterGroupVM> SupporterGroups
		{
			get
			{
				return this._supporterGroups;
			}
			set
			{
				if (value != this._supporterGroups)
				{
					this._supporterGroups = value;
					base.OnPropertyChangedWithValue<MBBindingList<ClanSupporterGroupVM>>(value, "SupporterGroups");
				}
			}
		}

		// Token: 0x17000900 RID: 2304
		// (get) Token: 0x06001A4B RID: 6731 RVA: 0x0005F1D1 File Offset: 0x0005D3D1
		// (set) Token: 0x06001A4C RID: 6732 RVA: 0x0005F1D9 File Offset: 0x0005D3D9
		[DataSourceProperty]
		public MBBindingList<ClanFinanceAlleyItemVM> Alleys
		{
			get
			{
				return this._alleys;
			}
			set
			{
				if (value != this._alleys)
				{
					this._alleys = value;
					base.OnPropertyChangedWithValue<MBBindingList<ClanFinanceAlleyItemVM>>(value, "Alleys");
				}
			}
		}

		// Token: 0x04000C5F RID: 3167
		private readonly Clan _clan;

		// Token: 0x04000C60 RID: 3168
		private readonly Action _onRefresh;

		// Token: 0x04000C61 RID: 3169
		private readonly Action<ClanCardSelectionInfo> _openCardSelectionPopup;

		// Token: 0x04000C63 RID: 3171
		private MBBindingList<ClanFinanceWorkshopItemVM> _incomes;

		// Token: 0x04000C64 RID: 3172
		private MBBindingList<ClanSupporterGroupVM> _supporterGroups;

		// Token: 0x04000C65 RID: 3173
		private MBBindingList<ClanFinanceAlleyItemVM> _alleys;

		// Token: 0x04000C66 RID: 3174
		private ClanFinanceAlleyItemVM _currentSelectedAlley;

		// Token: 0x04000C67 RID: 3175
		private ClanFinanceWorkshopItemVM _currentSelectedIncome;

		// Token: 0x04000C68 RID: 3176
		private ClanSupporterGroupVM _currentSelectedSupporterGroup;

		// Token: 0x04000C69 RID: 3177
		private bool _isSelected;

		// Token: 0x04000C6A RID: 3178
		private string _nameText;

		// Token: 0x04000C6B RID: 3179
		private string _incomeText;

		// Token: 0x04000C6C RID: 3180
		private string _locationText;

		// Token: 0x04000C6D RID: 3181
		private string _workshopsText;

		// Token: 0x04000C6E RID: 3182
		private string _supportersText;

		// Token: 0x04000C6F RID: 3183
		private string _alleysText;

		// Token: 0x04000C70 RID: 3184
		private string _noAdditionalIncomesText;

		// Token: 0x04000C71 RID: 3185
		private bool _isAnyValidAlleySelected;

		// Token: 0x04000C72 RID: 3186
		private bool _isAnyValidIncomeSelected;

		// Token: 0x04000C73 RID: 3187
		private bool _isAnyValidSupporterSelected;
	}
}
