using System;
using System.Linq;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.Election;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.KingdomManagement.Clans
{
	// Token: 0x02000072 RID: 114
	public class KingdomClanVM : KingdomCategoryVM
	{
		// Token: 0x060009FF RID: 2559 RVA: 0x00028488 File Offset: 0x00026688
		public KingdomClanVM(Action<KingdomDecision> forceDecide)
		{
			this._forceDecide = forceDecide;
			this.SupportHint = new HintViewModel();
			this.ExpelHint = new HintViewModel();
			this._clans = new MBBindingList<KingdomClanItemVM>();
			base.IsAcceptableItemSelected = false;
			this.RefreshClanList();
			base.NotificationCount = 0;
			this.SupportCost = Campaign.Current.Models.DiplomacyModel.GetInfluenceCostOfSupportingClan();
			this.ExpelCost = Campaign.Current.Models.DiplomacyModel.GetInfluenceCostOfExpellingClan();
			TextObject textObject;
			this.CanSupportCurrentClan = this.GetCanSupportCurrentClanWithReason(this.SupportCost, out textObject);
			this.SupportHint.HintText = textObject;
			TextObject textObject2;
			this.CanExpelCurrentClan = this.GetCanExpelCurrentClanWithReason(this._isThereAPendingDecisionToExpelThisClan, this.ExpelCost, out textObject2);
			this.ExpelHint.HintText = textObject2;
			this.ClanSortController = new KingdomClanSortControllerVM(ref this._clans);
			CampaignEvents.ClanChangedKingdom.AddNonSerializedListener(this, new Action<Clan, Kingdom, Kingdom, ChangeKingdomAction.ChangeKingdomActionDetail, bool>(this.OnClanChangedKingdom));
			this.RefreshValues();
		}

		// Token: 0x06000A00 RID: 2560 RVA: 0x00028580 File Offset: 0x00026780
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.SupportText = new TextObject("{=N63XYX2r}Support", null).ToString();
			this.NameText = GameTexts.FindText("str_scoreboard_header", "name").ToString();
			this.InfluenceText = GameTexts.FindText("str_influence", null).ToString();
			this.FiefsText = GameTexts.FindText("str_fiefs", null).ToString();
			this.MembersText = GameTexts.FindText("str_members", null).ToString();
			this.BannerText = GameTexts.FindText("str_banner", null).ToString();
			this.TypeText = GameTexts.FindText("str_sort_by_type_label", null).ToString();
			base.CategoryNameText = new TextObject("{=j4F7tTzy}Clan", null).ToString();
			base.NoItemSelectedText = GameTexts.FindText("str_kingdom_no_clan_selected", null).ToString();
			this.SupportActionExplanationText = GameTexts.FindText("str_support_clan_action_explanation", null).ToString();
			this.ExpelActionExplanationText = GameTexts.FindText("str_expel_clan_action_explanation", null).ToString();
		}

		// Token: 0x06000A01 RID: 2561 RVA: 0x0002868C File Offset: 0x0002688C
		private void SetCurrentSelectedClan(KingdomClanItemVM clan)
		{
			if (clan != this.CurrentSelectedClan)
			{
				if (this.CurrentSelectedClan != null)
				{
					this.CurrentSelectedClan.IsSelected = false;
				}
				this.CurrentSelectedClan = clan;
				this.CurrentSelectedClan.IsSelected = true;
				this.SupportCost = Campaign.Current.Models.DiplomacyModel.GetInfluenceCostOfSupportingClan();
				this._isThereAPendingDecisionToExpelThisClan = Clan.PlayerClan.Kingdom.UnresolvedDecisions.Any(delegate(KingdomDecision x)
				{
					ExpelClanFromKingdomDecision expelClanFromKingdomDecision;
					return (expelClanFromKingdomDecision = x as ExpelClanFromKingdomDecision) != null && expelClanFromKingdomDecision.ClanToExpel == this.CurrentSelectedClan.Clan && !x.ShouldBeCancelled();
				});
				TextObject textObject;
				this.CanExpelCurrentClan = this.GetCanExpelCurrentClanWithReason(this._isThereAPendingDecisionToExpelThisClan, this.ExpelCost, out textObject);
				this.ExpelHint.HintText = textObject;
				if (this._isThereAPendingDecisionToExpelThisClan)
				{
					this.ExpelActionText = GameTexts.FindText("str_resolve", null).ToString();
					this.ExpelActionExplanationText = GameTexts.FindText("str_resolve_explanation", null).ToString();
					this.ExpelCost = 0;
					return;
				}
				this.ExpelActionText = GameTexts.FindText("str_policy_propose", null).ToString();
				this.ExpelCost = Campaign.Current.Models.DiplomacyModel.GetInfluenceCostOfExpellingClan();
				TextObject textObject2;
				this.CanSupportCurrentClan = this.GetCanSupportCurrentClanWithReason(this.SupportCost, out textObject2);
				this.SupportHint.HintText = textObject2;
				this.ExpelActionExplanationText = GameTexts.FindText("str_expel_clan_action_explanation", null).SetTextVariable("SUPPORT", this.CalculateExpelLikelihood(this.CurrentSelectedClan)).ToString();
				base.IsAcceptableItemSelected = this.CurrentSelectedClan != null;
			}
		}

		// Token: 0x06000A02 RID: 2562 RVA: 0x000287FC File Offset: 0x000269FC
		private bool GetCanSupportCurrentClanWithReason(int supportCost, out TextObject disabledReason)
		{
			TextObject textObject;
			if (!CampaignUIHelper.GetMapScreenActionIsEnabledWithReason(out textObject))
			{
				disabledReason = textObject;
				return false;
			}
			if (Hero.MainHero.Clan.Influence < (float)supportCost)
			{
				disabledReason = GameTexts.FindText("str_warning_you_dont_have_enough_influence", null);
				return false;
			}
			if (this.CurrentSelectedClan.Clan == Clan.PlayerClan)
			{
				disabledReason = GameTexts.FindText("str_cannot_support_your_clan", null);
				return false;
			}
			if (Hero.MainHero.Clan.IsUnderMercenaryService)
			{
				disabledReason = GameTexts.FindText("str_mercenaries_cannot_support_clans", null);
				return false;
			}
			disabledReason = TextObject.Empty;
			return true;
		}

		// Token: 0x06000A03 RID: 2563 RVA: 0x00028884 File Offset: 0x00026A84
		private bool GetCanExpelCurrentClanWithReason(bool isThereAPendingDecision, int expelCost, out TextObject disabledReason)
		{
			TextObject textObject;
			if (!CampaignUIHelper.GetMapScreenActionIsEnabledWithReason(out textObject))
			{
				disabledReason = textObject;
				return false;
			}
			if (Hero.MainHero.Clan.IsUnderMercenaryService)
			{
				disabledReason = GameTexts.FindText("str_mercenaries_cannot_expel_clans", null);
				return false;
			}
			if (!isThereAPendingDecision)
			{
				if (Hero.MainHero.Clan.Influence < (float)expelCost)
				{
					disabledReason = GameTexts.FindText("str_warning_you_dont_have_enough_influence", null);
					return false;
				}
				if (this.CurrentSelectedClan.Clan == Clan.PlayerClan)
				{
					disabledReason = GameTexts.FindText("str_cannot_expel_your_clan", null);
					return false;
				}
				Clan clan = this.CurrentSelectedClan.Clan;
				Kingdom kingdom = this.CurrentSelectedClan.Clan.Kingdom;
				if (clan == ((kingdom != null) ? kingdom.RulingClan : null))
				{
					disabledReason = GameTexts.FindText("str_cannot_expel_ruling_clan", null);
					return false;
				}
			}
			disabledReason = TextObject.Empty;
			return true;
		}

		// Token: 0x06000A04 RID: 2564 RVA: 0x00028948 File Offset: 0x00026B48
		public void RefreshClan()
		{
			this.RefreshClanList();
			foreach (KingdomClanItemVM kingdomClanItemVM in this.Clans)
			{
				kingdomClanItemVM.Refresh();
			}
		}

		// Token: 0x06000A05 RID: 2565 RVA: 0x00028998 File Offset: 0x00026B98
		public void SelectClan(Clan clan)
		{
			foreach (KingdomClanItemVM kingdomClanItemVM in this.Clans)
			{
				if (kingdomClanItemVM.Clan == clan)
				{
					this.OnClanSelection(kingdomClanItemVM);
					break;
				}
			}
		}

		// Token: 0x06000A06 RID: 2566 RVA: 0x000289F0 File Offset: 0x00026BF0
		private void OnClanSelection(KingdomClanItemVM clan)
		{
			if (this._currentSelectedClan != clan)
			{
				this.SetCurrentSelectedClan(clan);
			}
		}

		// Token: 0x06000A07 RID: 2567 RVA: 0x00028A04 File Offset: 0x00026C04
		private void ExecuteExpelCurrentClan()
		{
			if (Hero.MainHero.Clan.Influence >= (float)this.ExpelCost)
			{
				KingdomDecision kingdomDecision = new ExpelClanFromKingdomDecision(Clan.PlayerClan, this._currentSelectedClan.Clan);
				Clan.PlayerClan.Kingdom.AddDecision(kingdomDecision, false);
				this._forceDecide(kingdomDecision);
			}
		}

		// Token: 0x06000A08 RID: 2568 RVA: 0x00028A5C File Offset: 0x00026C5C
		private void ExecuteSupport()
		{
			if (Hero.MainHero.Clan.Influence >= (float)this.SupportCost)
			{
				this._currentSelectedClan.Clan.OnSupportedByClan(Hero.MainHero.Clan);
				Clan clan = this._currentSelectedClan.Clan;
				this.RefreshClan();
				this.SelectClan(clan);
			}
		}

		// Token: 0x06000A09 RID: 2569 RVA: 0x00028AB4 File Offset: 0x00026CB4
		private int CalculateExpelLikelihood(KingdomClanItemVM clan)
		{
			return MathF.Round(new KingdomElection(new ExpelClanFromKingdomDecision(Clan.PlayerClan, clan.Clan)).GetLikelihoodForSponsor(Clan.PlayerClan) * 100f);
		}

		// Token: 0x06000A0A RID: 2570 RVA: 0x00028AE0 File Offset: 0x00026CE0
		private void RefreshClanList()
		{
			this.Clans.Clear();
			foreach (Clan clan in Clan.PlayerClan.Kingdom.Clans)
			{
				this.Clans.Add(new KingdomClanItemVM(clan, new Action<KingdomClanItemVM>(this.OnClanSelection)));
			}
			if (this.Clans.Count > 0)
			{
				this.SetCurrentSelectedClan(this.Clans.FirstOrDefault<KingdomClanItemVM>());
			}
			if (this.ClanSortController != null)
			{
				this.ClanSortController.SortByCurrentState();
			}
		}

		// Token: 0x06000A0B RID: 2571 RVA: 0x00028B90 File Offset: 0x00026D90
		public override void OnFinalize()
		{
			base.OnFinalize();
			CampaignEvents.ClanChangedKingdom.ClearListeners(this);
		}

		// Token: 0x06000A0C RID: 2572 RVA: 0x00028BA3 File Offset: 0x00026DA3
		private void OnClanChangedKingdom(Clan clan, Kingdom oldKingdom, Kingdom newKingdom, ChangeKingdomAction.ChangeKingdomActionDetail detail, bool showNotification)
		{
			if (clan != Clan.PlayerClan && (oldKingdom == Clan.PlayerClan.Kingdom || newKingdom == Clan.PlayerClan.Kingdom))
			{
				this.RefreshClanList();
			}
		}

		// Token: 0x17000339 RID: 825
		// (get) Token: 0x06000A0D RID: 2573 RVA: 0x00028BCD File Offset: 0x00026DCD
		// (set) Token: 0x06000A0E RID: 2574 RVA: 0x00028BD5 File Offset: 0x00026DD5
		[DataSourceProperty]
		public KingdomClanSortControllerVM ClanSortController
		{
			get
			{
				return this._clanSortController;
			}
			set
			{
				if (value != this._clanSortController)
				{
					this._clanSortController = value;
					base.OnPropertyChangedWithValue<KingdomClanSortControllerVM>(value, "ClanSortController");
				}
			}
		}

		// Token: 0x1700033A RID: 826
		// (get) Token: 0x06000A0F RID: 2575 RVA: 0x00028BF3 File Offset: 0x00026DF3
		// (set) Token: 0x06000A10 RID: 2576 RVA: 0x00028BFB File Offset: 0x00026DFB
		[DataSourceProperty]
		public KingdomClanItemVM CurrentSelectedClan
		{
			get
			{
				return this._currentSelectedClan;
			}
			set
			{
				if (value != this._currentSelectedClan)
				{
					this._currentSelectedClan = value;
					base.OnPropertyChangedWithValue<KingdomClanItemVM>(value, "CurrentSelectedClan");
				}
			}
		}

		// Token: 0x1700033B RID: 827
		// (get) Token: 0x06000A11 RID: 2577 RVA: 0x00028C19 File Offset: 0x00026E19
		// (set) Token: 0x06000A12 RID: 2578 RVA: 0x00028C21 File Offset: 0x00026E21
		[DataSourceProperty]
		public string ExpelActionExplanationText
		{
			get
			{
				return this._expelActionExplanationText;
			}
			set
			{
				if (value != this._expelActionExplanationText)
				{
					this._expelActionExplanationText = value;
					base.OnPropertyChangedWithValue<string>(value, "ExpelActionExplanationText");
				}
			}
		}

		// Token: 0x1700033C RID: 828
		// (get) Token: 0x06000A13 RID: 2579 RVA: 0x00028C44 File Offset: 0x00026E44
		// (set) Token: 0x06000A14 RID: 2580 RVA: 0x00028C4C File Offset: 0x00026E4C
		[DataSourceProperty]
		public string SupportActionExplanationText
		{
			get
			{
				return this._supportActionExplanationText;
			}
			set
			{
				if (value != this._supportActionExplanationText)
				{
					this._supportActionExplanationText = value;
					base.OnPropertyChangedWithValue<string>(value, "SupportActionExplanationText");
				}
			}
		}

		// Token: 0x1700033D RID: 829
		// (get) Token: 0x06000A15 RID: 2581 RVA: 0x00028C6F File Offset: 0x00026E6F
		// (set) Token: 0x06000A16 RID: 2582 RVA: 0x00028C77 File Offset: 0x00026E77
		[DataSourceProperty]
		public string BannerText
		{
			get
			{
				return this._bannerText;
			}
			set
			{
				if (value != this._bannerText)
				{
					this._bannerText = value;
					base.OnPropertyChangedWithValue<string>(value, "BannerText");
				}
			}
		}

		// Token: 0x1700033E RID: 830
		// (get) Token: 0x06000A17 RID: 2583 RVA: 0x00028C9A File Offset: 0x00026E9A
		// (set) Token: 0x06000A18 RID: 2584 RVA: 0x00028CA2 File Offset: 0x00026EA2
		[DataSourceProperty]
		public string TypeText
		{
			get
			{
				return this._typeText;
			}
			set
			{
				if (value != this._typeText)
				{
					this._typeText = value;
					base.OnPropertyChangedWithValue<string>(value, "TypeText");
				}
			}
		}

		// Token: 0x1700033F RID: 831
		// (get) Token: 0x06000A19 RID: 2585 RVA: 0x00028CC5 File Offset: 0x00026EC5
		// (set) Token: 0x06000A1A RID: 2586 RVA: 0x00028CCD File Offset: 0x00026ECD
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

		// Token: 0x17000340 RID: 832
		// (get) Token: 0x06000A1B RID: 2587 RVA: 0x00028CF0 File Offset: 0x00026EF0
		// (set) Token: 0x06000A1C RID: 2588 RVA: 0x00028CF8 File Offset: 0x00026EF8
		[DataSourceProperty]
		public string InfluenceText
		{
			get
			{
				return this._influenceText;
			}
			set
			{
				if (value != this._influenceText)
				{
					this._influenceText = value;
					base.OnPropertyChangedWithValue<string>(value, "InfluenceText");
				}
			}
		}

		// Token: 0x17000341 RID: 833
		// (get) Token: 0x06000A1D RID: 2589 RVA: 0x00028D1B File Offset: 0x00026F1B
		// (set) Token: 0x06000A1E RID: 2590 RVA: 0x00028D23 File Offset: 0x00026F23
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

		// Token: 0x17000342 RID: 834
		// (get) Token: 0x06000A1F RID: 2591 RVA: 0x00028D46 File Offset: 0x00026F46
		// (set) Token: 0x06000A20 RID: 2592 RVA: 0x00028D4E File Offset: 0x00026F4E
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

		// Token: 0x17000343 RID: 835
		// (get) Token: 0x06000A21 RID: 2593 RVA: 0x00028D71 File Offset: 0x00026F71
		// (set) Token: 0x06000A22 RID: 2594 RVA: 0x00028D79 File Offset: 0x00026F79
		[DataSourceProperty]
		public MBBindingList<KingdomClanItemVM> Clans
		{
			get
			{
				return this._clans;
			}
			set
			{
				if (value != this._clans)
				{
					this._clans = value;
					base.OnPropertyChangedWithValue<MBBindingList<KingdomClanItemVM>>(value, "Clans");
				}
			}
		}

		// Token: 0x17000344 RID: 836
		// (get) Token: 0x06000A23 RID: 2595 RVA: 0x00028D97 File Offset: 0x00026F97
		// (set) Token: 0x06000A24 RID: 2596 RVA: 0x00028D9F File Offset: 0x00026F9F
		[DataSourceProperty]
		public bool CanSupportCurrentClan
		{
			get
			{
				return this._canSupportCurrentClan;
			}
			set
			{
				if (value != this._canSupportCurrentClan)
				{
					this._canSupportCurrentClan = value;
					base.OnPropertyChangedWithValue(value, "CanSupportCurrentClan");
				}
			}
		}

		// Token: 0x17000345 RID: 837
		// (get) Token: 0x06000A25 RID: 2597 RVA: 0x00028DBD File Offset: 0x00026FBD
		// (set) Token: 0x06000A26 RID: 2598 RVA: 0x00028DC5 File Offset: 0x00026FC5
		[DataSourceProperty]
		public bool CanExpelCurrentClan
		{
			get
			{
				return this._canExpelCurrentClan;
			}
			set
			{
				if (value != this._canExpelCurrentClan)
				{
					this._canExpelCurrentClan = value;
					base.OnPropertyChangedWithValue(value, "CanExpelCurrentClan");
				}
			}
		}

		// Token: 0x17000346 RID: 838
		// (get) Token: 0x06000A27 RID: 2599 RVA: 0x00028DE3 File Offset: 0x00026FE3
		// (set) Token: 0x06000A28 RID: 2600 RVA: 0x00028DEB File Offset: 0x00026FEB
		[DataSourceProperty]
		public string SupportText
		{
			get
			{
				return this._supportText;
			}
			set
			{
				if (value != this._supportText)
				{
					this._supportText = value;
					base.OnPropertyChangedWithValue<string>(value, "SupportText");
				}
			}
		}

		// Token: 0x17000347 RID: 839
		// (get) Token: 0x06000A29 RID: 2601 RVA: 0x00028E0E File Offset: 0x0002700E
		// (set) Token: 0x06000A2A RID: 2602 RVA: 0x00028E16 File Offset: 0x00027016
		[DataSourceProperty]
		public string ExpelActionText
		{
			get
			{
				return this._expelActionText;
			}
			set
			{
				if (value != this._expelActionText)
				{
					this._expelActionText = value;
					base.OnPropertyChangedWithValue<string>(value, "ExpelActionText");
				}
			}
		}

		// Token: 0x17000348 RID: 840
		// (get) Token: 0x06000A2B RID: 2603 RVA: 0x00028E39 File Offset: 0x00027039
		// (set) Token: 0x06000A2C RID: 2604 RVA: 0x00028E41 File Offset: 0x00027041
		[DataSourceProperty]
		public int SupportCost
		{
			get
			{
				return this._supportCost;
			}
			set
			{
				if (value != this._supportCost)
				{
					this._supportCost = value;
					base.OnPropertyChangedWithValue(value, "SupportCost");
				}
			}
		}

		// Token: 0x17000349 RID: 841
		// (get) Token: 0x06000A2D RID: 2605 RVA: 0x00028E5F File Offset: 0x0002705F
		// (set) Token: 0x06000A2E RID: 2606 RVA: 0x00028E67 File Offset: 0x00027067
		[DataSourceProperty]
		public int ExpelCost
		{
			get
			{
				return this._expelCost;
			}
			set
			{
				if (value != this._expelCost)
				{
					this._expelCost = value;
					base.OnPropertyChangedWithValue(value, "ExpelCost");
				}
			}
		}

		// Token: 0x1700034A RID: 842
		// (get) Token: 0x06000A2F RID: 2607 RVA: 0x00028E85 File Offset: 0x00027085
		// (set) Token: 0x06000A30 RID: 2608 RVA: 0x00028E8D File Offset: 0x0002708D
		[DataSourceProperty]
		public HintViewModel ExpelHint
		{
			get
			{
				return this._expelHint;
			}
			set
			{
				if (value != this._expelHint)
				{
					this._expelHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "ExpelHint");
				}
			}
		}

		// Token: 0x1700034B RID: 843
		// (get) Token: 0x06000A31 RID: 2609 RVA: 0x00028EAB File Offset: 0x000270AB
		// (set) Token: 0x06000A32 RID: 2610 RVA: 0x00028EB3 File Offset: 0x000270B3
		[DataSourceProperty]
		public HintViewModel SupportHint
		{
			get
			{
				return this._supportHint;
			}
			set
			{
				if (value != this._supportHint)
				{
					this._supportHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "SupportHint");
				}
			}
		}

		// Token: 0x04000484 RID: 1156
		private Action<KingdomDecision> _forceDecide;

		// Token: 0x04000485 RID: 1157
		private bool _isThereAPendingDecisionToExpelThisClan;

		// Token: 0x04000486 RID: 1158
		private MBBindingList<KingdomClanItemVM> _clans;

		// Token: 0x04000487 RID: 1159
		private HintViewModel _expelHint;

		// Token: 0x04000488 RID: 1160
		private HintViewModel _supportHint;

		// Token: 0x04000489 RID: 1161
		private string _bannerText;

		// Token: 0x0400048A RID: 1162
		private string _nameText;

		// Token: 0x0400048B RID: 1163
		private string _influenceText;

		// Token: 0x0400048C RID: 1164
		private string _membersText;

		// Token: 0x0400048D RID: 1165
		private string _fiefsText;

		// Token: 0x0400048E RID: 1166
		private string _typeText;

		// Token: 0x0400048F RID: 1167
		private string _expelActionText;

		// Token: 0x04000490 RID: 1168
		private string _expelActionExplanationText;

		// Token: 0x04000491 RID: 1169
		private string _supportActionExplanationText;

		// Token: 0x04000492 RID: 1170
		private int _expelCost;

		// Token: 0x04000493 RID: 1171
		private string _supportText;

		// Token: 0x04000494 RID: 1172
		private int _supportCost;

		// Token: 0x04000495 RID: 1173
		private bool _canSupportCurrentClan;

		// Token: 0x04000496 RID: 1174
		private bool _canExpelCurrentClan;

		// Token: 0x04000497 RID: 1175
		private KingdomClanItemVM _currentSelectedClan;

		// Token: 0x04000498 RID: 1176
		private KingdomClanSortControllerVM _clanSortController;
	}
}
