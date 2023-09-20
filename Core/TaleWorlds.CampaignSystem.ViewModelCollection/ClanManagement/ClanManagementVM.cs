using System;
using System.Collections.Generic;
using Helpers;
using TaleWorlds.CampaignSystem.CampaignBehaviors;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Settlements.Workshops;
using TaleWorlds.CampaignSystem.ViewModelCollection.ClanManagement.Categories;
using TaleWorlds.CampaignSystem.ViewModelCollection.Input;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Core.ViewModelCollection.Tutorial;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.ClanManagement
{
	// Token: 0x02000104 RID: 260
	public class ClanManagementVM : ViewModel
	{
		// Token: 0x06001813 RID: 6163 RVA: 0x000580B4 File Offset: 0x000562B4
		public ClanManagementVM(Action onClose, Action<Hero> showHeroOnMap, Action<Hero> openPartyAsManage, Action openBannerEditor)
		{
			this._onClose = onClose;
			this._openPartyAsManage = openPartyAsManage;
			this._openBannerEditor = openBannerEditor;
			this._showHeroOnMap = showHeroOnMap;
			this._clan = Hero.MainHero.Clan;
			this._playerUpdateTracker = PlayerUpdateTracker.Current;
			this.CardSelectionPopup = new ClanCardSelectionPopupVM();
			this.ClanMembers = new ClanMembersVM(new Action(this.RefreshCategoryValues), this._showHeroOnMap);
			this.ClanFiefs = new ClanFiefsVM(new Action(this.RefreshCategoryValues), new Action<ClanCardSelectionInfo>(this.CardSelectionPopup.Open));
			this.ClanParties = new ClanPartiesVM(new Action(this.OnAnyExpenseChange), this._openPartyAsManage, new Action(this.RefreshCategoryValues), new Action<ClanCardSelectionInfo>(this.CardSelectionPopup.Open));
			this.ClanIncome = new ClanIncomeVM(new Action(this.RefreshCategoryValues), new Action<ClanCardSelectionInfo>(this.CardSelectionPopup.Open));
			this._categoryCount = 4;
			this.SetSelectedCategory(0);
			this.Leader = new HeroVM(this._clan.Leader, false);
			this.CurrentRenown = (int)Clan.PlayerClan.Renown;
			this.CurrentTier = Clan.PlayerClan.Tier;
			TextObject textObject;
			if (Campaign.Current.Models.ClanTierModel.HasUpcomingTier(Clan.PlayerClan, out textObject, false).Item2)
			{
				this.NextTierRenown = Clan.PlayerClan.RenownRequirementForNextTier;
				this.MinRenownForCurrentTier = Campaign.Current.Models.ClanTierModel.GetRequiredRenownForTier(this.CurrentTier);
				this.NextTier = Clan.PlayerClan.Tier + 1;
				this.IsRenownProgressComplete = false;
			}
			else
			{
				this.NextTierRenown = 1;
				this.MinRenownForCurrentTier = 1;
				this.NextTier = 0;
				this.IsRenownProgressComplete = true;
			}
			this.RenownHint = new BasicTooltipViewModel(() => CampaignUIHelper.GetClanRenownTooltip(Clan.PlayerClan));
			this.GoldChangeTooltip = new BasicTooltipViewModel(() => CampaignUIHelper.GetGoldTooltip(Clan.PlayerClan));
			this.RefreshDailyValues();
			this.CanChooseBanner = true;
			TextObject textObject2;
			this.PlayerCanChangeClanName = this.GetPlayerCanChangeClanNameWithReason(out textObject2);
			this.ChangeClanNameHint = new HintViewModel(textObject2, null);
			this.TutorialNotification = new ElementNotificationVM();
			this.UpdateKingdomRelatedProperties();
			Game.Current.EventManager.RegisterEvent<TutorialNotificationElementChangeEvent>(new Action<TutorialNotificationElementChangeEvent>(this.OnTutorialNotificationElementIDChange));
		}

		// Token: 0x06001814 RID: 6164 RVA: 0x00058330 File Offset: 0x00056530
		private bool GetPlayerCanChangeClanNameWithReason(out TextObject disabledReason)
		{
			TextObject textObject;
			if (!CampaignUIHelper.GetMapScreenActionIsEnabledWithReason(out textObject))
			{
				disabledReason = textObject;
				return false;
			}
			if (this._clan.Leader != Hero.MainHero)
			{
				disabledReason = new TextObject("{=GCaYjA5W}You need to be the leader of the clan to change it's name.", null);
				return false;
			}
			disabledReason = TextObject.Empty;
			return true;
		}

		// Token: 0x06001815 RID: 6165 RVA: 0x00058374 File Offset: 0x00056574
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.Name = Hero.MainHero.Clan.Name.ToString();
			this.CurrentGoldText = GameTexts.FindText("str_clan_finance_current_gold", null).ToString();
			this.TotalExpensesText = GameTexts.FindText("str_clan_finance_total_expenses", null).ToString();
			this.TotalIncomeText = GameTexts.FindText("str_clan_finance_total_income", null).ToString();
			this.DailyChangeText = GameTexts.FindText("str_clan_finance_daily_change", null).ToString();
			this.ExpectedGoldText = GameTexts.FindText("str_clan_finance_expected", null).ToString();
			this.ExpenseText = GameTexts.FindText("str_clan_expenses", null).ToString();
			this.MembersText = GameTexts.FindText("str_members", null).ToString();
			this.PartiesText = GameTexts.FindText("str_parties", null).ToString();
			this.IncomeText = GameTexts.FindText("str_other", null).ToString();
			this.FiefsText = GameTexts.FindText("str_fiefs", null).ToString();
			this.DoneLbl = GameTexts.FindText("str_done", null).ToString();
			this.LeaderText = GameTexts.FindText("str_sort_by_leader_name_label", null).ToString();
			this.FinanceText = GameTexts.FindText("str_finance", null).ToString();
			GameTexts.SetVariable("TIER", Clan.PlayerClan.Tier);
			this.CurrentRenownText = GameTexts.FindText("str_clan_tier", null).ToString();
			ElementNotificationVM tutorialNotification = this.TutorialNotification;
			if (tutorialNotification != null)
			{
				tutorialNotification.RefreshValues();
			}
			ClanMembersVM clanMembers = this._clanMembers;
			if (clanMembers != null)
			{
				clanMembers.RefreshValues();
			}
			ClanPartiesVM clanParties = this._clanParties;
			if (clanParties != null)
			{
				clanParties.RefreshValues();
			}
			ClanFiefsVM clanFiefs = this._clanFiefs;
			if (clanFiefs != null)
			{
				clanFiefs.RefreshValues();
			}
			ClanIncomeVM clanIncome = this._clanIncome;
			if (clanIncome != null)
			{
				clanIncome.RefreshValues();
			}
			HeroVM leader = this._leader;
			if (leader == null)
			{
				return;
			}
			leader.RefreshValues();
		}

		// Token: 0x06001816 RID: 6166 RVA: 0x0005854E File Offset: 0x0005674E
		public void SelectHero(Hero hero)
		{
			this.SetSelectedCategory(0);
			this.ClanMembers.SelectMember(hero);
		}

		// Token: 0x06001817 RID: 6167 RVA: 0x00058563 File Offset: 0x00056763
		public void SelectParty(PartyBase party)
		{
			this.SetSelectedCategory(1);
			this.ClanParties.SelectParty(party);
		}

		// Token: 0x06001818 RID: 6168 RVA: 0x00058578 File Offset: 0x00056778
		public void SelectSettlement(Settlement settlement)
		{
			this.SetSelectedCategory(2);
			this.ClanFiefs.SelectFief(settlement);
		}

		// Token: 0x06001819 RID: 6169 RVA: 0x0005858D File Offset: 0x0005678D
		public void SelectWorkshop(Workshop workshop)
		{
			this.SetSelectedCategory(3);
			this.ClanIncome.SelectWorkshop(workshop);
		}

		// Token: 0x0600181A RID: 6170 RVA: 0x000585A2 File Offset: 0x000567A2
		public void SelectAlley(Alley alley)
		{
			this.SetSelectedCategory(3);
			this.ClanIncome.SelectAlley(alley);
		}

		// Token: 0x0600181B RID: 6171 RVA: 0x000585B8 File Offset: 0x000567B8
		public void SelectPreviousCategory()
		{
			int num = ((this._currentCategory == 0) ? (this._categoryCount - 1) : (this._currentCategory - 1));
			this.SetSelectedCategory(num);
		}

		// Token: 0x0600181C RID: 6172 RVA: 0x000585E8 File Offset: 0x000567E8
		public void SelectNextCategory()
		{
			int num = (this._currentCategory + 1) % this._categoryCount;
			this.SetSelectedCategory(num);
		}

		// Token: 0x0600181D RID: 6173 RVA: 0x0005860C File Offset: 0x0005680C
		public void ExecuteOpenBannerEditor()
		{
			this._openBannerEditor();
		}

		// Token: 0x0600181E RID: 6174 RVA: 0x00058619 File Offset: 0x00056819
		public void UpdateBannerVisuals()
		{
			this.ClanBanner = new ImageIdentifierVM(BannerCode.CreateFrom(this._clan.Banner), true);
			this.ClanBannerHint = new HintViewModel(new TextObject("{=t1lSXN9O}Your clan's standard carried into battle", null), null);
			this.RefreshValues();
		}

		// Token: 0x0600181F RID: 6175 RVA: 0x00058654 File Offset: 0x00056854
		public void SetSelectedCategory(int index)
		{
			this.ClanMembers.IsSelected = false;
			this.ClanParties.IsSelected = false;
			this.ClanFiefs.IsSelected = false;
			this.ClanIncome.IsSelected = false;
			this._currentCategory = index;
			if (index == 0)
			{
				this.ClanMembers.IsSelected = true;
			}
			else if (index == 1)
			{
				this.ClanParties.IsSelected = true;
			}
			else if (index == 2)
			{
				this.ClanFiefs.IsSelected = true;
			}
			else
			{
				this._currentCategory = 3;
				this.ClanIncome.IsSelected = true;
			}
			this.IsMembersSelected = this.ClanMembers.IsSelected;
			this.IsPartiesSelected = this.ClanParties.IsSelected;
			this.IsFiefsSelected = this.ClanFiefs.IsSelected;
			this.IsIncomeSelected = this.ClanIncome.IsSelected;
		}

		// Token: 0x06001820 RID: 6176 RVA: 0x00058724 File Offset: 0x00056924
		private void UpdateKingdomRelatedProperties()
		{
			this.ClanIsInAKingdom = this._clan.Kingdom != null;
			if (this.ClanIsInAKingdom)
			{
				if (this._clan.Kingdom.RulingClan == this._clan)
				{
					this.IsKingdomActionEnabled = false;
					this.KingdomActionDisabledReasonHint = new BasicTooltipViewModel(() => new TextObject("{=vIPrZCZ1}You can abdicate leadership from the kingdom screen.", null).ToString());
					this.KingdomActionText = GameTexts.FindText("str_abdicate_leadership", null).ToString();
				}
				else
				{
					this.IsKingdomActionEnabled = MobileParty.MainParty.Army == null;
					this.KingdomActionText = GameTexts.FindText("str_leave_kingdom", null).ToString();
					this.KingdomActionDisabledReasonHint = new BasicTooltipViewModel();
				}
			}
			else
			{
				List<TextObject> kingdomCreationDisabledReasons;
				this.IsKingdomActionEnabled = Campaign.Current.Models.KingdomCreationModel.IsPlayerKingdomCreationPossible(out kingdomCreationDisabledReasons);
				this.KingdomActionText = GameTexts.FindText("str_create_kingdom", null).ToString();
				this.KingdomActionDisabledReasonHint = new BasicTooltipViewModel(() => CampaignUIHelper.GetHintTextFromReasons(kingdomCreationDisabledReasons));
			}
			this.UpdateBannerVisuals();
		}

		// Token: 0x06001821 RID: 6177 RVA: 0x00058848 File Offset: 0x00056A48
		public void RefreshDailyValues()
		{
			if (this.ClanIncome != null)
			{
				this.CurrentGold = Hero.MainHero.Gold;
				this.TotalIncome = (int)Campaign.Current.Models.ClanFinanceModel.CalculateClanIncome(this._clan, false, false, false).ResultNumber;
				this.TotalExpenses = (int)Campaign.Current.Models.ClanFinanceModel.CalculateClanExpenses(this._clan, false, false, false).ResultNumber;
				this.DailyChange = MathF.Abs(this.TotalIncome) - MathF.Abs(this.TotalExpenses);
				this.ExpectedGold = this.CurrentGold + this.DailyChange;
				if (this.TotalIncome == 0)
				{
					this.TotalIncomeValueText = GameTexts.FindText("str_clan_finance_value_zero", null).ToString();
				}
				else
				{
					GameTexts.SetVariable("IS_POSITIVE", (this.TotalIncome > 0) ? 1 : 0);
					GameTexts.SetVariable("NUMBER", MathF.Abs(this.TotalIncome));
					this.TotalIncomeValueText = GameTexts.FindText("str_clan_finance_value", null).ToString();
				}
				if (this.TotalExpenses == 0)
				{
					this.TotalExpensesValueText = GameTexts.FindText("str_clan_finance_value_zero", null).ToString();
				}
				else
				{
					GameTexts.SetVariable("IS_POSITIVE", (this.TotalExpenses > 0) ? 1 : 0);
					GameTexts.SetVariable("NUMBER", MathF.Abs(this.TotalExpenses));
					this.TotalExpensesValueText = GameTexts.FindText("str_clan_finance_value", null).ToString();
				}
				if (this.DailyChange == 0)
				{
					this.DailyChangeValueText = GameTexts.FindText("str_clan_finance_value_zero", null).ToString();
					return;
				}
				GameTexts.SetVariable("IS_POSITIVE", (this.DailyChange > 0) ? 1 : 0);
				GameTexts.SetVariable("NUMBER", MathF.Abs(this.DailyChange));
				this.DailyChangeValueText = GameTexts.FindText("str_clan_finance_value", null).ToString();
			}
		}

		// Token: 0x06001822 RID: 6178 RVA: 0x00058A1D File Offset: 0x00056C1D
		public void RefreshCategoryValues()
		{
			this.ClanFiefs.RefreshAllLists();
			this.ClanMembers.RefreshMembersList();
			this.ClanParties.RefreshPartiesList();
			this.ClanIncome.RefreshList();
		}

		// Token: 0x06001823 RID: 6179 RVA: 0x00058A4C File Offset: 0x00056C4C
		public void ExecuteChangeClanName()
		{
			GameTexts.SetVariable("MAX_LETTER_COUNT", 50);
			GameTexts.SetVariable("MIN_LETTER_COUNT", 1);
			InformationManager.ShowTextInquiry(new TextInquiryData(GameTexts.FindText("str_change_clan_name", null).ToString(), string.Empty, true, true, GameTexts.FindText("str_done", null).ToString(), GameTexts.FindText("str_cancel", null).ToString(), new Action<string>(this.OnChangeClanNameDone), null, false, new Func<string, Tuple<bool, string>>(FactionHelper.IsClanNameApplicable), "", ""), false, false);
		}

		// Token: 0x06001824 RID: 6180 RVA: 0x00058AD8 File Offset: 0x00056CD8
		private void OnChangeClanNameDone(string newClanName)
		{
			TextObject textObject = GameTexts.FindText("str_generic_clan_name", null);
			textObject.SetTextVariable("CLAN_NAME", new TextObject(newClanName, null));
			this._clan.InitializeClan(textObject, textObject, this._clan.Culture, this._clan.Banner, default(Vec2), false);
			this.RefreshCategoryValues();
			this.RefreshValues();
		}

		// Token: 0x06001825 RID: 6181 RVA: 0x00058B3D File Offset: 0x00056D3D
		private void OnAnyExpenseChange()
		{
			this.RefreshDailyValues();
		}

		// Token: 0x06001826 RID: 6182 RVA: 0x00058B45 File Offset: 0x00056D45
		public void ExecuteClose()
		{
			this._onClose();
		}

		// Token: 0x06001827 RID: 6183 RVA: 0x00058B54 File Offset: 0x00056D54
		public override void OnFinalize()
		{
			base.OnFinalize();
			this.ClanFiefs.OnFinalize();
			this.DoneInputKey.OnFinalize();
			this.PreviousTabInputKey.OnFinalize();
			this.NextTabInputKey.OnFinalize();
			this.CardSelectionPopup.OnFinalize();
			this.ClanMembers.OnFinalize();
			this.ClanParties.OnFinalize();
			Game.Current.EventManager.UnregisterEvent<TutorialNotificationElementChangeEvent>(new Action<TutorialNotificationElementChangeEvent>(this.OnTutorialNotificationElementIDChange));
		}

		// Token: 0x17000831 RID: 2097
		// (get) Token: 0x06001828 RID: 6184 RVA: 0x00058BCF File Offset: 0x00056DCF
		// (set) Token: 0x06001829 RID: 6185 RVA: 0x00058BD7 File Offset: 0x00056DD7
		[DataSourceProperty]
		public HeroVM Leader
		{
			get
			{
				return this._leader;
			}
			set
			{
				if (value != this._leader)
				{
					this._leader = value;
					base.OnPropertyChangedWithValue<HeroVM>(value, "Leader");
				}
			}
		}

		// Token: 0x17000832 RID: 2098
		// (get) Token: 0x0600182A RID: 6186 RVA: 0x00058BF5 File Offset: 0x00056DF5
		// (set) Token: 0x0600182B RID: 6187 RVA: 0x00058BFD File Offset: 0x00056DFD
		[DataSourceProperty]
		public ImageIdentifierVM ClanBanner
		{
			get
			{
				return this._clanBanner;
			}
			set
			{
				if (value != this._clanBanner)
				{
					this._clanBanner = value;
					base.OnPropertyChangedWithValue<ImageIdentifierVM>(value, "ClanBanner");
				}
			}
		}

		// Token: 0x17000833 RID: 2099
		// (get) Token: 0x0600182C RID: 6188 RVA: 0x00058C1B File Offset: 0x00056E1B
		// (set) Token: 0x0600182D RID: 6189 RVA: 0x00058C23 File Offset: 0x00056E23
		[DataSourceProperty]
		public ClanCardSelectionPopupVM CardSelectionPopup
		{
			get
			{
				return this._cardSelectionPopup;
			}
			set
			{
				if (value != this._cardSelectionPopup)
				{
					this._cardSelectionPopup = value;
					base.OnPropertyChangedWithValue<ClanCardSelectionPopupVM>(value, "CardSelectionPopup");
				}
			}
		}

		// Token: 0x17000834 RID: 2100
		// (get) Token: 0x0600182E RID: 6190 RVA: 0x00058C41 File Offset: 0x00056E41
		// (set) Token: 0x0600182F RID: 6191 RVA: 0x00058C49 File Offset: 0x00056E49
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

		// Token: 0x17000835 RID: 2101
		// (get) Token: 0x06001830 RID: 6192 RVA: 0x00058C6C File Offset: 0x00056E6C
		// (set) Token: 0x06001831 RID: 6193 RVA: 0x00058C74 File Offset: 0x00056E74
		[DataSourceProperty]
		public string LeaderText
		{
			get
			{
				return this._leaderText;
			}
			set
			{
				if (value != this._leaderText)
				{
					this._leaderText = value;
					base.OnPropertyChangedWithValue<string>(value, "LeaderText");
				}
			}
		}

		// Token: 0x17000836 RID: 2102
		// (get) Token: 0x06001832 RID: 6194 RVA: 0x00058C97 File Offset: 0x00056E97
		// (set) Token: 0x06001833 RID: 6195 RVA: 0x00058C9F File Offset: 0x00056E9F
		[DataSourceProperty]
		public ClanMembersVM ClanMembers
		{
			get
			{
				return this._clanMembers;
			}
			set
			{
				if (value != this._clanMembers)
				{
					this._clanMembers = value;
					base.OnPropertyChangedWithValue<ClanMembersVM>(value, "ClanMembers");
				}
			}
		}

		// Token: 0x17000837 RID: 2103
		// (get) Token: 0x06001834 RID: 6196 RVA: 0x00058CBD File Offset: 0x00056EBD
		// (set) Token: 0x06001835 RID: 6197 RVA: 0x00058CC5 File Offset: 0x00056EC5
		[DataSourceProperty]
		public ClanPartiesVM ClanParties
		{
			get
			{
				return this._clanParties;
			}
			set
			{
				if (value != this._clanParties)
				{
					this._clanParties = value;
					base.OnPropertyChangedWithValue<ClanPartiesVM>(value, "ClanParties");
				}
			}
		}

		// Token: 0x17000838 RID: 2104
		// (get) Token: 0x06001836 RID: 6198 RVA: 0x00058CE3 File Offset: 0x00056EE3
		// (set) Token: 0x06001837 RID: 6199 RVA: 0x00058CEB File Offset: 0x00056EEB
		[DataSourceProperty]
		public ClanFiefsVM ClanFiefs
		{
			get
			{
				return this._clanFiefs;
			}
			set
			{
				if (value != this._clanFiefs)
				{
					this._clanFiefs = value;
					base.OnPropertyChangedWithValue<ClanFiefsVM>(value, "ClanFiefs");
				}
			}
		}

		// Token: 0x17000839 RID: 2105
		// (get) Token: 0x06001838 RID: 6200 RVA: 0x00058D09 File Offset: 0x00056F09
		// (set) Token: 0x06001839 RID: 6201 RVA: 0x00058D11 File Offset: 0x00056F11
		[DataSourceProperty]
		public ClanIncomeVM ClanIncome
		{
			get
			{
				return this._clanIncome;
			}
			set
			{
				if (value != this._clanIncome)
				{
					this._clanIncome = value;
					base.OnPropertyChangedWithValue<ClanIncomeVM>(value, "ClanIncome");
				}
			}
		}

		// Token: 0x1700083A RID: 2106
		// (get) Token: 0x0600183A RID: 6202 RVA: 0x00058D2F File Offset: 0x00056F2F
		// (set) Token: 0x0600183B RID: 6203 RVA: 0x00058D37 File Offset: 0x00056F37
		[DataSourceProperty]
		public bool IsMembersSelected
		{
			get
			{
				return this._isMembersSelected;
			}
			set
			{
				if (value != this._isMembersSelected)
				{
					this._isMembersSelected = value;
					base.OnPropertyChangedWithValue(value, "IsMembersSelected");
				}
			}
		}

		// Token: 0x1700083B RID: 2107
		// (get) Token: 0x0600183C RID: 6204 RVA: 0x00058D55 File Offset: 0x00056F55
		// (set) Token: 0x0600183D RID: 6205 RVA: 0x00058D5D File Offset: 0x00056F5D
		[DataSourceProperty]
		public bool IsPartiesSelected
		{
			get
			{
				return this._isPartiesSelected;
			}
			set
			{
				if (value != this._isPartiesSelected)
				{
					this._isPartiesSelected = value;
					base.OnPropertyChangedWithValue(value, "IsPartiesSelected");
				}
			}
		}

		// Token: 0x1700083C RID: 2108
		// (get) Token: 0x0600183E RID: 6206 RVA: 0x00058D7B File Offset: 0x00056F7B
		// (set) Token: 0x0600183F RID: 6207 RVA: 0x00058D83 File Offset: 0x00056F83
		[DataSourceProperty]
		public bool CanSwitchTabs
		{
			get
			{
				return this._canSwitchTabs;
			}
			set
			{
				if (value != this._canSwitchTabs)
				{
					this._canSwitchTabs = value;
					base.OnPropertyChangedWithValue(value, "CanSwitchTabs");
				}
			}
		}

		// Token: 0x1700083D RID: 2109
		// (get) Token: 0x06001840 RID: 6208 RVA: 0x00058DA1 File Offset: 0x00056FA1
		// (set) Token: 0x06001841 RID: 6209 RVA: 0x00058DA9 File Offset: 0x00056FA9
		[DataSourceProperty]
		public bool IsFiefsSelected
		{
			get
			{
				return this._isFiefsSelected;
			}
			set
			{
				if (value != this._isFiefsSelected)
				{
					this._isFiefsSelected = value;
					base.OnPropertyChangedWithValue(value, "IsFiefsSelected");
				}
			}
		}

		// Token: 0x1700083E RID: 2110
		// (get) Token: 0x06001842 RID: 6210 RVA: 0x00058DC7 File Offset: 0x00056FC7
		// (set) Token: 0x06001843 RID: 6211 RVA: 0x00058DCF File Offset: 0x00056FCF
		[DataSourceProperty]
		public bool IsIncomeSelected
		{
			get
			{
				return this._isIncomeSelected;
			}
			set
			{
				if (value != this._isIncomeSelected)
				{
					this._isIncomeSelected = value;
					base.OnPropertyChangedWithValue(value, "IsIncomeSelected");
				}
			}
		}

		// Token: 0x1700083F RID: 2111
		// (get) Token: 0x06001844 RID: 6212 RVA: 0x00058DED File Offset: 0x00056FED
		// (set) Token: 0x06001845 RID: 6213 RVA: 0x00058DF5 File Offset: 0x00056FF5
		[DataSourceProperty]
		public bool ClanIsInAKingdom
		{
			get
			{
				return this._clanIsInAKingdom;
			}
			set
			{
				if (value != this._clanIsInAKingdom)
				{
					this._clanIsInAKingdom = value;
					base.OnPropertyChangedWithValue(value, "ClanIsInAKingdom");
				}
			}
		}

		// Token: 0x17000840 RID: 2112
		// (get) Token: 0x06001846 RID: 6214 RVA: 0x00058E13 File Offset: 0x00057013
		// (set) Token: 0x06001847 RID: 6215 RVA: 0x00058E1B File Offset: 0x0005701B
		[DataSourceProperty]
		public bool IsKingdomActionEnabled
		{
			get
			{
				return this._isKingdomActionEnabled;
			}
			set
			{
				if (value != this._isKingdomActionEnabled)
				{
					this._isKingdomActionEnabled = value;
					base.OnPropertyChangedWithValue(value, "IsKingdomActionEnabled");
				}
			}
		}

		// Token: 0x17000841 RID: 2113
		// (get) Token: 0x06001848 RID: 6216 RVA: 0x00058E39 File Offset: 0x00057039
		// (set) Token: 0x06001849 RID: 6217 RVA: 0x00058E41 File Offset: 0x00057041
		[DataSourceProperty]
		public bool PlayerCanChangeClanName
		{
			get
			{
				return this._playerCanChangeClanName;
			}
			set
			{
				if (value != this._playerCanChangeClanName)
				{
					this._playerCanChangeClanName = value;
					base.OnPropertyChangedWithValue(value, "PlayerCanChangeClanName");
				}
			}
		}

		// Token: 0x17000842 RID: 2114
		// (get) Token: 0x0600184A RID: 6218 RVA: 0x00058E5F File Offset: 0x0005705F
		// (set) Token: 0x0600184B RID: 6219 RVA: 0x00058E67 File Offset: 0x00057067
		[DataSourceProperty]
		public bool CanChooseBanner
		{
			get
			{
				return this._canChooseBanner;
			}
			set
			{
				if (value != this._canChooseBanner)
				{
					this._canChooseBanner = value;
					base.OnPropertyChangedWithValue(value, "CanChooseBanner");
				}
			}
		}

		// Token: 0x17000843 RID: 2115
		// (get) Token: 0x0600184C RID: 6220 RVA: 0x00058E85 File Offset: 0x00057085
		// (set) Token: 0x0600184D RID: 6221 RVA: 0x00058E8D File Offset: 0x0005708D
		[DataSourceProperty]
		public bool IsRenownProgressComplete
		{
			get
			{
				return this._isRenownProgressComplete;
			}
			set
			{
				if (value != this._isRenownProgressComplete)
				{
					this._isRenownProgressComplete = value;
					base.OnPropertyChangedWithValue(value, "IsRenownProgressComplete");
				}
			}
		}

		// Token: 0x17000844 RID: 2116
		// (get) Token: 0x0600184E RID: 6222 RVA: 0x00058EAB File Offset: 0x000570AB
		// (set) Token: 0x0600184F RID: 6223 RVA: 0x00058EB3 File Offset: 0x000570B3
		[DataSourceProperty]
		public string DoneLbl
		{
			get
			{
				return this._doneLbl;
			}
			set
			{
				if (value != this._doneLbl)
				{
					this._doneLbl = value;
					base.OnPropertyChangedWithValue<string>(value, "DoneLbl");
				}
			}
		}

		// Token: 0x17000845 RID: 2117
		// (get) Token: 0x06001850 RID: 6224 RVA: 0x00058ED6 File Offset: 0x000570D6
		// (set) Token: 0x06001851 RID: 6225 RVA: 0x00058EDE File Offset: 0x000570DE
		[DataSourceProperty]
		public string CurrentRenownText
		{
			get
			{
				return this._currentRenownText;
			}
			set
			{
				if (value != this._currentRenownText)
				{
					this._currentRenownText = value;
					base.OnPropertyChangedWithValue<string>(value, "CurrentRenownText");
				}
			}
		}

		// Token: 0x17000846 RID: 2118
		// (get) Token: 0x06001852 RID: 6226 RVA: 0x00058F01 File Offset: 0x00057101
		// (set) Token: 0x06001853 RID: 6227 RVA: 0x00058F09 File Offset: 0x00057109
		[DataSourceProperty]
		public string KingdomActionText
		{
			get
			{
				return this._kingdomActionText;
			}
			set
			{
				if (value != this._kingdomActionText)
				{
					this._kingdomActionText = value;
					base.OnPropertyChangedWithValue<string>(value, "KingdomActionText");
				}
			}
		}

		// Token: 0x17000847 RID: 2119
		// (get) Token: 0x06001854 RID: 6228 RVA: 0x00058F2C File Offset: 0x0005712C
		// (set) Token: 0x06001855 RID: 6229 RVA: 0x00058F34 File Offset: 0x00057134
		[DataSourceProperty]
		public int NextTierRenown
		{
			get
			{
				return this._nextTierRenown;
			}
			set
			{
				if (value != this._nextTierRenown)
				{
					this._nextTierRenown = value;
					base.OnPropertyChangedWithValue(value, "NextTierRenown");
				}
			}
		}

		// Token: 0x17000848 RID: 2120
		// (get) Token: 0x06001856 RID: 6230 RVA: 0x00058F52 File Offset: 0x00057152
		// (set) Token: 0x06001857 RID: 6231 RVA: 0x00058F5A File Offset: 0x0005715A
		[DataSourceProperty]
		public int CurrentTier
		{
			get
			{
				return this._currentTier;
			}
			set
			{
				if (value != this._currentTier)
				{
					this._currentTier = value;
					base.OnPropertyChangedWithValue(value, "CurrentTier");
				}
			}
		}

		// Token: 0x17000849 RID: 2121
		// (get) Token: 0x06001858 RID: 6232 RVA: 0x00058F78 File Offset: 0x00057178
		// (set) Token: 0x06001859 RID: 6233 RVA: 0x00058F80 File Offset: 0x00057180
		[DataSourceProperty]
		public int MinRenownForCurrentTier
		{
			get
			{
				return this._minRenownForCurrentTier;
			}
			set
			{
				if (value != this._minRenownForCurrentTier)
				{
					this._minRenownForCurrentTier = value;
					base.OnPropertyChangedWithValue(value, "MinRenownForCurrentTier");
				}
			}
		}

		// Token: 0x1700084A RID: 2122
		// (get) Token: 0x0600185A RID: 6234 RVA: 0x00058F9E File Offset: 0x0005719E
		// (set) Token: 0x0600185B RID: 6235 RVA: 0x00058FA6 File Offset: 0x000571A6
		[DataSourceProperty]
		public int NextTier
		{
			get
			{
				return this._nextTier;
			}
			set
			{
				if (value != this._nextTier)
				{
					this._nextTier = value;
					base.OnPropertyChangedWithValue(value, "NextTier");
				}
			}
		}

		// Token: 0x1700084B RID: 2123
		// (get) Token: 0x0600185C RID: 6236 RVA: 0x00058FC4 File Offset: 0x000571C4
		// (set) Token: 0x0600185D RID: 6237 RVA: 0x00058FCC File Offset: 0x000571CC
		[DataSourceProperty]
		public int CurrentRenown
		{
			get
			{
				return this._currentRenown;
			}
			set
			{
				if (value != this._currentRenown)
				{
					this._currentRenown = value;
					base.OnPropertyChangedWithValue(value, "CurrentRenown");
				}
			}
		}

		// Token: 0x1700084C RID: 2124
		// (get) Token: 0x0600185E RID: 6238 RVA: 0x00058FEA File Offset: 0x000571EA
		// (set) Token: 0x0600185F RID: 6239 RVA: 0x00058FF2 File Offset: 0x000571F2
		[DataSourceProperty]
		public string MembersText
		{
			get
			{
				return this._membersText;
			}
			set
			{
				if (value != this._membersText)
				{
					this._membersText = value;
					base.OnPropertyChangedWithValue<string>(value, "MembersText");
				}
			}
		}

		// Token: 0x1700084D RID: 2125
		// (get) Token: 0x06001860 RID: 6240 RVA: 0x00059015 File Offset: 0x00057215
		// (set) Token: 0x06001861 RID: 6241 RVA: 0x0005901D File Offset: 0x0005721D
		[DataSourceProperty]
		public string PartiesText
		{
			get
			{
				return this._partiesText;
			}
			set
			{
				if (value != this._partiesText)
				{
					this._partiesText = value;
					base.OnPropertyChangedWithValue<string>(value, "PartiesText");
				}
			}
		}

		// Token: 0x1700084E RID: 2126
		// (get) Token: 0x06001862 RID: 6242 RVA: 0x00059040 File Offset: 0x00057240
		// (set) Token: 0x06001863 RID: 6243 RVA: 0x00059048 File Offset: 0x00057248
		[DataSourceProperty]
		public string FiefsText
		{
			get
			{
				return this._fiefsText;
			}
			set
			{
				if (value != this._fiefsText)
				{
					this._fiefsText = value;
					base.OnPropertyChangedWithValue<string>(value, "FiefsText");
				}
			}
		}

		// Token: 0x1700084F RID: 2127
		// (get) Token: 0x06001864 RID: 6244 RVA: 0x0005906B File Offset: 0x0005726B
		// (set) Token: 0x06001865 RID: 6245 RVA: 0x00059073 File Offset: 0x00057273
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
					base.OnPropertyChanged("OtherText");
				}
			}
		}

		// Token: 0x17000850 RID: 2128
		// (get) Token: 0x06001866 RID: 6246 RVA: 0x00059095 File Offset: 0x00057295
		// (set) Token: 0x06001867 RID: 6247 RVA: 0x0005909D File Offset: 0x0005729D
		[DataSourceProperty]
		public BasicTooltipViewModel RenownHint
		{
			get
			{
				return this._renownHint;
			}
			set
			{
				if (value != this._renownHint)
				{
					this._renownHint = value;
					base.OnPropertyChangedWithValue<BasicTooltipViewModel>(value, "RenownHint");
				}
			}
		}

		// Token: 0x17000851 RID: 2129
		// (get) Token: 0x06001868 RID: 6248 RVA: 0x000590BB File Offset: 0x000572BB
		// (set) Token: 0x06001869 RID: 6249 RVA: 0x000590C3 File Offset: 0x000572C3
		[DataSourceProperty]
		public HintViewModel ClanBannerHint
		{
			get
			{
				return this._clanBannerHint;
			}
			set
			{
				if (value != this._clanBannerHint)
				{
					this._clanBannerHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "ClanBannerHint");
				}
			}
		}

		// Token: 0x17000852 RID: 2130
		// (get) Token: 0x0600186A RID: 6250 RVA: 0x000590E1 File Offset: 0x000572E1
		// (set) Token: 0x0600186B RID: 6251 RVA: 0x000590E9 File Offset: 0x000572E9
		[DataSourceProperty]
		public HintViewModel ChangeClanNameHint
		{
			get
			{
				return this._changeClanNameHint;
			}
			set
			{
				if (value != this._changeClanNameHint)
				{
					this._changeClanNameHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "ChangeClanNameHint");
				}
			}
		}

		// Token: 0x17000853 RID: 2131
		// (get) Token: 0x0600186C RID: 6252 RVA: 0x00059107 File Offset: 0x00057307
		// (set) Token: 0x0600186D RID: 6253 RVA: 0x0005910F File Offset: 0x0005730F
		[DataSourceProperty]
		public BasicTooltipViewModel KingdomActionDisabledReasonHint
		{
			get
			{
				return this._kingdomActionDisabledReasonHint;
			}
			set
			{
				if (value != this._kingdomActionDisabledReasonHint)
				{
					this._kingdomActionDisabledReasonHint = value;
					base.OnPropertyChangedWithValue<BasicTooltipViewModel>(value, "KingdomActionDisabledReasonHint");
				}
			}
		}

		// Token: 0x17000854 RID: 2132
		// (get) Token: 0x0600186E RID: 6254 RVA: 0x0005912D File Offset: 0x0005732D
		// (set) Token: 0x0600186F RID: 6255 RVA: 0x00059135 File Offset: 0x00057335
		[DataSourceProperty]
		public BasicTooltipViewModel GoldChangeTooltip
		{
			get
			{
				return this._goldChangeTooltip;
			}
			set
			{
				if (value != this._goldChangeTooltip)
				{
					this._goldChangeTooltip = value;
					base.OnPropertyChangedWithValue<BasicTooltipViewModel>(value, "GoldChangeTooltip");
				}
			}
		}

		// Token: 0x17000855 RID: 2133
		// (get) Token: 0x06001870 RID: 6256 RVA: 0x00059153 File Offset: 0x00057353
		// (set) Token: 0x06001871 RID: 6257 RVA: 0x0005915B File Offset: 0x0005735B
		[DataSourceProperty]
		public string CurrentGoldText
		{
			get
			{
				return this._currentGoldText;
			}
			set
			{
				if (value != this._currentGoldText)
				{
					this._currentGoldText = value;
					base.OnPropertyChangedWithValue<string>(value, "CurrentGoldText");
				}
			}
		}

		// Token: 0x17000856 RID: 2134
		// (get) Token: 0x06001872 RID: 6258 RVA: 0x0005917E File Offset: 0x0005737E
		// (set) Token: 0x06001873 RID: 6259 RVA: 0x00059186 File Offset: 0x00057386
		[DataSourceProperty]
		public int CurrentGold
		{
			get
			{
				return this._currentGold;
			}
			set
			{
				if (value != this._currentGold)
				{
					this._currentGold = value;
					base.OnPropertyChangedWithValue(value, "CurrentGold");
				}
			}
		}

		// Token: 0x17000857 RID: 2135
		// (get) Token: 0x06001874 RID: 6260 RVA: 0x000591A4 File Offset: 0x000573A4
		// (set) Token: 0x06001875 RID: 6261 RVA: 0x000591AC File Offset: 0x000573AC
		[DataSourceProperty]
		public string ExpenseText
		{
			get
			{
				return this._expenseText;
			}
			set
			{
				if (value != this._expenseText)
				{
					this._expenseText = value;
					base.OnPropertyChangedWithValue<string>(value, "ExpenseText");
				}
			}
		}

		// Token: 0x17000858 RID: 2136
		// (get) Token: 0x06001876 RID: 6262 RVA: 0x000591CF File Offset: 0x000573CF
		// (set) Token: 0x06001877 RID: 6263 RVA: 0x000591D7 File Offset: 0x000573D7
		[DataSourceProperty]
		public string TotalIncomeText
		{
			get
			{
				return this._totalIncomeText;
			}
			set
			{
				if (value != this._totalIncomeText)
				{
					this._totalIncomeText = value;
					base.OnPropertyChangedWithValue<string>(value, "TotalIncomeText");
				}
			}
		}

		// Token: 0x17000859 RID: 2137
		// (get) Token: 0x06001878 RID: 6264 RVA: 0x000591FA File Offset: 0x000573FA
		// (set) Token: 0x06001879 RID: 6265 RVA: 0x00059202 File Offset: 0x00057402
		[DataSourceProperty]
		public string FinanceText
		{
			get
			{
				return this._financeText;
			}
			set
			{
				if (value != this._financeText)
				{
					this._financeText = value;
					base.OnPropertyChangedWithValue<string>(value, "FinanceText");
				}
			}
		}

		// Token: 0x1700085A RID: 2138
		// (get) Token: 0x0600187A RID: 6266 RVA: 0x00059225 File Offset: 0x00057425
		// (set) Token: 0x0600187B RID: 6267 RVA: 0x0005922D File Offset: 0x0005742D
		[DataSourceProperty]
		public int TotalIncome
		{
			get
			{
				return this._totalIncome;
			}
			set
			{
				if (value != this._totalIncome)
				{
					this._totalIncome = value;
					base.OnPropertyChangedWithValue(value, "TotalIncome");
				}
			}
		}

		// Token: 0x1700085B RID: 2139
		// (get) Token: 0x0600187C RID: 6268 RVA: 0x0005924B File Offset: 0x0005744B
		// (set) Token: 0x0600187D RID: 6269 RVA: 0x00059253 File Offset: 0x00057453
		[DataSourceProperty]
		public string TotalExpensesText
		{
			get
			{
				return this._totalExpensesText;
			}
			set
			{
				if (value != this._totalExpensesText)
				{
					this._totalExpensesText = value;
					base.OnPropertyChangedWithValue<string>(value, "TotalExpensesText");
				}
			}
		}

		// Token: 0x1700085C RID: 2140
		// (get) Token: 0x0600187E RID: 6270 RVA: 0x00059276 File Offset: 0x00057476
		// (set) Token: 0x0600187F RID: 6271 RVA: 0x0005927E File Offset: 0x0005747E
		[DataSourceProperty]
		public int TotalExpenses
		{
			get
			{
				return this._totalExpenses;
			}
			set
			{
				if (value != this._totalExpenses)
				{
					this._totalExpenses = value;
					base.OnPropertyChangedWithValue(value, "TotalExpenses");
				}
			}
		}

		// Token: 0x1700085D RID: 2141
		// (get) Token: 0x06001880 RID: 6272 RVA: 0x0005929C File Offset: 0x0005749C
		// (set) Token: 0x06001881 RID: 6273 RVA: 0x000592A4 File Offset: 0x000574A4
		[DataSourceProperty]
		public string DailyChangeText
		{
			get
			{
				return this._dailyChangeText;
			}
			set
			{
				if (value != this._dailyChangeText)
				{
					this._dailyChangeText = value;
					base.OnPropertyChangedWithValue<string>(value, "DailyChangeText");
				}
			}
		}

		// Token: 0x1700085E RID: 2142
		// (get) Token: 0x06001882 RID: 6274 RVA: 0x000592C7 File Offset: 0x000574C7
		// (set) Token: 0x06001883 RID: 6275 RVA: 0x000592CF File Offset: 0x000574CF
		[DataSourceProperty]
		public int DailyChange
		{
			get
			{
				return this._dailyChange;
			}
			set
			{
				if (value != this._dailyChange)
				{
					this._dailyChange = value;
					base.OnPropertyChangedWithValue(value, "DailyChange");
				}
			}
		}

		// Token: 0x1700085F RID: 2143
		// (get) Token: 0x06001884 RID: 6276 RVA: 0x000592ED File Offset: 0x000574ED
		// (set) Token: 0x06001885 RID: 6277 RVA: 0x000592F5 File Offset: 0x000574F5
		[DataSourceProperty]
		public string ExpectedGoldText
		{
			get
			{
				return this._expectedGoldText;
			}
			set
			{
				if (value != this._expectedGoldText)
				{
					this._expectedGoldText = value;
					base.OnPropertyChangedWithValue<string>(value, "ExpectedGoldText");
				}
			}
		}

		// Token: 0x17000860 RID: 2144
		// (get) Token: 0x06001886 RID: 6278 RVA: 0x00059318 File Offset: 0x00057518
		// (set) Token: 0x06001887 RID: 6279 RVA: 0x00059320 File Offset: 0x00057520
		[DataSourceProperty]
		public int ExpectedGold
		{
			get
			{
				return this._expectedGold;
			}
			set
			{
				if (value != this._expectedGold)
				{
					this._expectedGold = value;
					base.OnPropertyChangedWithValue(value, "ExpectedGold");
				}
			}
		}

		// Token: 0x17000861 RID: 2145
		// (get) Token: 0x06001888 RID: 6280 RVA: 0x0005933E File Offset: 0x0005753E
		// (set) Token: 0x06001889 RID: 6281 RVA: 0x00059346 File Offset: 0x00057546
		[DataSourceProperty]
		public string DailyChangeValueText
		{
			get
			{
				return this._dailyChangeValueText;
			}
			set
			{
				if (value != this._dailyChangeValueText)
				{
					this._dailyChangeValueText = value;
					base.OnPropertyChangedWithValue<string>(value, "DailyChangeValueText");
				}
			}
		}

		// Token: 0x17000862 RID: 2146
		// (get) Token: 0x0600188A RID: 6282 RVA: 0x00059369 File Offset: 0x00057569
		// (set) Token: 0x0600188B RID: 6283 RVA: 0x00059371 File Offset: 0x00057571
		[DataSourceProperty]
		public string TotalExpensesValueText
		{
			get
			{
				return this._totalExpensesValueText;
			}
			set
			{
				if (value != this._totalExpensesValueText)
				{
					this._totalExpensesValueText = value;
					base.OnPropertyChangedWithValue<string>(value, "TotalExpensesValueText");
				}
			}
		}

		// Token: 0x17000863 RID: 2147
		// (get) Token: 0x0600188C RID: 6284 RVA: 0x00059394 File Offset: 0x00057594
		// (set) Token: 0x0600188D RID: 6285 RVA: 0x0005939C File Offset: 0x0005759C
		[DataSourceProperty]
		public string TotalIncomeValueText
		{
			get
			{
				return this._totalIncomeValueText;
			}
			set
			{
				if (value != this._totalIncomeValueText)
				{
					this._totalIncomeValueText = value;
					base.OnPropertyChangedWithValue<string>(value, "TotalIncomeValueText");
				}
			}
		}

		// Token: 0x0600188E RID: 6286 RVA: 0x000593BF File Offset: 0x000575BF
		public void SetDoneInputKey(HotKey hotkey)
		{
			this.DoneInputKey = InputKeyItemVM.CreateFromHotKey(hotkey, true);
			this.CardSelectionPopup.SetDoneInputKey(hotkey);
		}

		// Token: 0x0600188F RID: 6287 RVA: 0x000593DA File Offset: 0x000575DA
		public void SetPreviousTabInputKey(HotKey hotkey)
		{
			this.PreviousTabInputKey = InputKeyItemVM.CreateFromHotKey(hotkey, true);
		}

		// Token: 0x06001890 RID: 6288 RVA: 0x000593E9 File Offset: 0x000575E9
		public void SetNextTabInputKey(HotKey hotkey)
		{
			this.NextTabInputKey = InputKeyItemVM.CreateFromHotKey(hotkey, true);
		}

		// Token: 0x17000864 RID: 2148
		// (get) Token: 0x06001891 RID: 6289 RVA: 0x000593F8 File Offset: 0x000575F8
		// (set) Token: 0x06001892 RID: 6290 RVA: 0x00059400 File Offset: 0x00057600
		public InputKeyItemVM DoneInputKey
		{
			get
			{
				return this._doneInputKey;
			}
			set
			{
				if (value != this._doneInputKey)
				{
					this._doneInputKey = value;
					base.OnPropertyChangedWithValue<InputKeyItemVM>(value, "DoneInputKey");
				}
			}
		}

		// Token: 0x17000865 RID: 2149
		// (get) Token: 0x06001893 RID: 6291 RVA: 0x0005941E File Offset: 0x0005761E
		// (set) Token: 0x06001894 RID: 6292 RVA: 0x00059426 File Offset: 0x00057626
		public InputKeyItemVM PreviousTabInputKey
		{
			get
			{
				return this._previousTabInputKey;
			}
			set
			{
				if (value != this._previousTabInputKey)
				{
					this._previousTabInputKey = value;
					base.OnPropertyChangedWithValue<InputKeyItemVM>(value, "PreviousTabInputKey");
				}
			}
		}

		// Token: 0x17000866 RID: 2150
		// (get) Token: 0x06001895 RID: 6293 RVA: 0x00059444 File Offset: 0x00057644
		// (set) Token: 0x06001896 RID: 6294 RVA: 0x0005944C File Offset: 0x0005764C
		public InputKeyItemVM NextTabInputKey
		{
			get
			{
				return this._nextTabInputKey;
			}
			set
			{
				if (value != this._nextTabInputKey)
				{
					this._nextTabInputKey = value;
					base.OnPropertyChangedWithValue<InputKeyItemVM>(value, "NextTabInputKey");
				}
			}
		}

		// Token: 0x17000867 RID: 2151
		// (get) Token: 0x06001897 RID: 6295 RVA: 0x0005946A File Offset: 0x0005766A
		// (set) Token: 0x06001898 RID: 6296 RVA: 0x00059472 File Offset: 0x00057672
		[DataSourceProperty]
		public ElementNotificationVM TutorialNotification
		{
			get
			{
				return this._tutorialNotification;
			}
			set
			{
				if (value != this._tutorialNotification)
				{
					this._tutorialNotification = value;
					base.OnPropertyChangedWithValue<ElementNotificationVM>(value, "TutorialNotification");
				}
			}
		}

		// Token: 0x06001899 RID: 6297 RVA: 0x00059490 File Offset: 0x00057690
		private void OnTutorialNotificationElementIDChange(TutorialNotificationElementChangeEvent obj)
		{
			if (obj.NewNotificationElementID != this._latestTutorialElementID)
			{
				if (this._latestTutorialElementID != null)
				{
					this.TutorialNotification.ElementID = string.Empty;
				}
				this._latestTutorialElementID = obj.NewNotificationElementID;
				if (this._latestTutorialElementID != null)
				{
					this.TutorialNotification.ElementID = this._latestTutorialElementID;
					if (this._latestTutorialElementID == "RoleAssignmentWidget")
					{
						this.SetSelectedCategory(1);
					}
				}
			}
		}

		// Token: 0x04000B71 RID: 2929
		private readonly Action _onClose;

		// Token: 0x04000B72 RID: 2930
		private readonly Action _openBannerEditor;

		// Token: 0x04000B73 RID: 2931
		private readonly Action<Hero> _openPartyAsManage;

		// Token: 0x04000B74 RID: 2932
		private readonly Action<Hero> _showHeroOnMap;

		// Token: 0x04000B75 RID: 2933
		private readonly Clan _clan;

		// Token: 0x04000B76 RID: 2934
		private PlayerUpdateTracker _playerUpdateTracker;

		// Token: 0x04000B77 RID: 2935
		private readonly int _categoryCount;

		// Token: 0x04000B78 RID: 2936
		private int _currentCategory;

		// Token: 0x04000B79 RID: 2937
		private ClanMembersVM _clanMembers;

		// Token: 0x04000B7A RID: 2938
		private ClanPartiesVM _clanParties;

		// Token: 0x04000B7B RID: 2939
		private ClanFiefsVM _clanFiefs;

		// Token: 0x04000B7C RID: 2940
		private ClanIncomeVM _clanIncome;

		// Token: 0x04000B7D RID: 2941
		private HeroVM _leader;

		// Token: 0x04000B7E RID: 2942
		private ImageIdentifierVM _clanBanner;

		// Token: 0x04000B7F RID: 2943
		private ClanCardSelectionPopupVM _cardSelectionPopup;

		// Token: 0x04000B80 RID: 2944
		private bool _canSwitchTabs;

		// Token: 0x04000B81 RID: 2945
		private bool _isPartiesSelected;

		// Token: 0x04000B82 RID: 2946
		private bool _isMembersSelected;

		// Token: 0x04000B83 RID: 2947
		private bool _isFiefsSelected;

		// Token: 0x04000B84 RID: 2948
		private bool _isIncomeSelected;

		// Token: 0x04000B85 RID: 2949
		private bool _canChooseBanner;

		// Token: 0x04000B86 RID: 2950
		private bool _isRenownProgressComplete;

		// Token: 0x04000B87 RID: 2951
		private bool _playerCanChangeClanName;

		// Token: 0x04000B88 RID: 2952
		private bool _clanIsInAKingdom;

		// Token: 0x04000B89 RID: 2953
		private string _doneLbl;

		// Token: 0x04000B8A RID: 2954
		private bool _isKingdomActionEnabled;

		// Token: 0x04000B8B RID: 2955
		private string _name;

		// Token: 0x04000B8C RID: 2956
		private string _kingdomActionText;

		// Token: 0x04000B8D RID: 2957
		private string _leaderText;

		// Token: 0x04000B8E RID: 2958
		private int _minRenownForCurrentTier;

		// Token: 0x04000B8F RID: 2959
		private int _currentRenown;

		// Token: 0x04000B90 RID: 2960
		private int _currentTier = -1;

		// Token: 0x04000B91 RID: 2961
		private int _nextTierRenown;

		// Token: 0x04000B92 RID: 2962
		private int _nextTier;

		// Token: 0x04000B93 RID: 2963
		private string _currentRenownText;

		// Token: 0x04000B94 RID: 2964
		private string _membersText;

		// Token: 0x04000B95 RID: 2965
		private string _partiesText;

		// Token: 0x04000B96 RID: 2966
		private string _fiefsText;

		// Token: 0x04000B97 RID: 2967
		private string _incomeText;

		// Token: 0x04000B98 RID: 2968
		private BasicTooltipViewModel _renownHint;

		// Token: 0x04000B99 RID: 2969
		private BasicTooltipViewModel _kingdomActionDisabledReasonHint;

		// Token: 0x04000B9A RID: 2970
		private HintViewModel _clanBannerHint;

		// Token: 0x04000B9B RID: 2971
		private HintViewModel _changeClanNameHint;

		// Token: 0x04000B9C RID: 2972
		private string _financeText;

		// Token: 0x04000B9D RID: 2973
		private string _currentGoldText;

		// Token: 0x04000B9E RID: 2974
		private int _currentGold;

		// Token: 0x04000B9F RID: 2975
		private string _totalIncomeText;

		// Token: 0x04000BA0 RID: 2976
		private int _totalIncome;

		// Token: 0x04000BA1 RID: 2977
		private string _totalIncomeValueText;

		// Token: 0x04000BA2 RID: 2978
		private string _totalExpensesText;

		// Token: 0x04000BA3 RID: 2979
		private int _totalExpenses;

		// Token: 0x04000BA4 RID: 2980
		private string _totalExpensesValueText;

		// Token: 0x04000BA5 RID: 2981
		private string _dailyChangeText;

		// Token: 0x04000BA6 RID: 2982
		private int _dailyChange;

		// Token: 0x04000BA7 RID: 2983
		private string _dailyChangeValueText;

		// Token: 0x04000BA8 RID: 2984
		private string _expectedGoldText;

		// Token: 0x04000BA9 RID: 2985
		private int _expectedGold;

		// Token: 0x04000BAA RID: 2986
		private string _expenseText;

		// Token: 0x04000BAB RID: 2987
		private BasicTooltipViewModel _goldChangeTooltip;

		// Token: 0x04000BAC RID: 2988
		private InputKeyItemVM _doneInputKey;

		// Token: 0x04000BAD RID: 2989
		private InputKeyItemVM _previousTabInputKey;

		// Token: 0x04000BAE RID: 2990
		private InputKeyItemVM _nextTabInputKey;

		// Token: 0x04000BAF RID: 2991
		private ElementNotificationVM _tutorialNotification;

		// Token: 0x04000BB0 RID: 2992
		private string _latestTutorialElementID;
	}
}
