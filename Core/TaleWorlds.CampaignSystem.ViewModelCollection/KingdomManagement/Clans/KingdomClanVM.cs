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
	public class KingdomClanVM : KingdomCategoryVM
	{
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

		public void RefreshClan()
		{
			this.RefreshClanList();
			foreach (KingdomClanItemVM kingdomClanItemVM in this.Clans)
			{
				kingdomClanItemVM.Refresh();
			}
		}

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

		private void OnClanSelection(KingdomClanItemVM clan)
		{
			if (this._currentSelectedClan != clan)
			{
				this.SetCurrentSelectedClan(clan);
			}
		}

		private void ExecuteExpelCurrentClan()
		{
			if (Hero.MainHero.Clan.Influence >= (float)this.ExpelCost)
			{
				KingdomDecision kingdomDecision = new ExpelClanFromKingdomDecision(Clan.PlayerClan, this._currentSelectedClan.Clan);
				Clan.PlayerClan.Kingdom.AddDecision(kingdomDecision, false);
				this._forceDecide(kingdomDecision);
			}
		}

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

		private int CalculateExpelLikelihood(KingdomClanItemVM clan)
		{
			return MathF.Round(new KingdomElection(new ExpelClanFromKingdomDecision(Clan.PlayerClan, clan.Clan)).GetLikelihoodForSponsor(Clan.PlayerClan) * 100f);
		}

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

		public override void OnFinalize()
		{
			base.OnFinalize();
			CampaignEvents.ClanChangedKingdom.ClearListeners(this);
		}

		private void OnClanChangedKingdom(Clan clan, Kingdom oldKingdom, Kingdom newKingdom, ChangeKingdomAction.ChangeKingdomActionDetail detail, bool showNotification)
		{
			if (clan != Clan.PlayerClan && (oldKingdom == Clan.PlayerClan.Kingdom || newKingdom == Clan.PlayerClan.Kingdom))
			{
				this.RefreshClanList();
			}
		}

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

		private Action<KingdomDecision> _forceDecide;

		private bool _isThereAPendingDecisionToExpelThisClan;

		private MBBindingList<KingdomClanItemVM> _clans;

		private HintViewModel _expelHint;

		private HintViewModel _supportHint;

		private string _bannerText;

		private string _nameText;

		private string _influenceText;

		private string _membersText;

		private string _fiefsText;

		private string _typeText;

		private string _expelActionText;

		private string _expelActionExplanationText;

		private string _supportActionExplanationText;

		private int _expelCost;

		private string _supportText;

		private int _supportCost;

		private bool _canSupportCurrentClan;

		private bool _canExpelCurrentClan;

		private KingdomClanItemVM _currentSelectedClan;

		private KingdomClanSortControllerVM _clanSortController;
	}
}
