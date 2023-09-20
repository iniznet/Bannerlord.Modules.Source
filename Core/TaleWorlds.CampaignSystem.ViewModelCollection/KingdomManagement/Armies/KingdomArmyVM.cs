using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.KingdomManagement.Armies
{
	public class KingdomArmyVM : KingdomCategoryVM
	{
		public KingdomArmyVM(Action onManageArmy, Action refreshDecision, Action<Army> showArmyOnMap)
		{
			this._onManageArmy = onManageArmy;
			this._refreshDecision = refreshDecision;
			this._showArmyOnMap = showArmyOnMap;
			this._viewDataTracker = Campaign.Current.GetCampaignBehavior<IViewDataTracker>();
			this._armies = new MBBindingList<KingdomArmyItemVM>();
			this.PlayerHasArmy = MobileParty.MainParty.Army != null;
			this.ChangeLeaderCost = Campaign.Current.Models.DiplomacyModel.GetInfluenceCostOfChangingLeaderOfArmy();
			this.DisbandCost = Campaign.Current.Models.DiplomacyModel.GetInfluenceCostOfDisbandingArmy();
			this.CreateArmyHint = new HintViewModel();
			this.DisbandHint = new HintViewModel();
			this.ManageArmyHint = new HintViewModel();
			base.IsAcceptableItemSelected = false;
			this.RefreshArmyList();
			this.ArmySortController = new KingdomArmySortControllerVM(ref this._armies);
			this.RefreshValues();
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
			this.ArmyNameText = GameTexts.FindText("str_sort_by_army_name_label", null).ToString();
			this.LeaderText = GameTexts.FindText("str_sort_by_leader_name_label", null).ToString();
			this.StrengthText = GameTexts.FindText("str_men_numbersign", null).ToString();
			this.LocationText = GameTexts.FindText("str_tooltip_label_location", null).ToString();
			base.NoItemSelectedText = GameTexts.FindText("str_kingdom_no_army_selected", null).ToString();
			this.DisbandActionExplanationText = GameTexts.FindText("str_kingdom_disband_army_explanation", null).ToString();
			this.ManageActionExplanationText = GameTexts.FindText("str_kingdom_manage_army_explanation", null).ToString();
			this.ManageText = GameTexts.FindText("str_manage", null).ToString();
			this.CreateArmyText = (this.PlayerHasArmy ? new TextObject("{=DAmdTxuC}Army Manage", null).ToString() : new TextObject("{=lc9s4rLZ}Create Army", null).ToString());
			base.CategoryNameText = new TextObject("{=j12VrGKz}Army", null).ToString();
			this.ChangeLeaderText = new TextObject("{=NcYbdiyT}Change Leader", null).ToString();
			this.PartiesText = new TextObject("{=t3tq0eoW}Parties", null).ToString();
			this.DisbandText = new TextObject("{=xXSFaGW8}Disband", null).ToString();
			this.ShowOnMapText = GameTexts.FindText("str_show_on_map", null).ToString();
			this.CreateArmyText = new TextObject("{=lc9s4rLZ}Create Army", null).ToString();
			this.Armies.ApplyActionOnAllItems(delegate(KingdomArmyItemVM x)
			{
				x.RefreshValues();
			});
			KingdomArmyItemVM currentSelectedArmy = this.CurrentSelectedArmy;
			if (currentSelectedArmy == null)
			{
				return;
			}
			currentSelectedArmy.RefreshValues();
		}

		public void RefreshArmyList()
		{
			base.NotificationCount = this._viewDataTracker.NumOfKingdomArmyNotifications;
			this._kingdom = Hero.MainHero.MapFaction as Kingdom;
			if (this._kingdom != null)
			{
				this.Armies.Clear();
				using (List<Army>.Enumerator enumerator = this._kingdom.Armies.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						Army army = enumerator.Current;
						this.Armies.Add(new KingdomArmyItemVM(army, new Action<KingdomArmyItemVM>(this.OnSelection)));
					}
					goto IL_A0;
				}
			}
			Debug.FailedAssert("Kingdom screen can't open if you're not in kingdom", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.CampaignSystem.ViewModelCollection\\KingdomManagement\\Armies\\KingdomArmyVM.cs", "RefreshArmyList", 81);
			IL_A0:
			this.RefreshCanManageArmy();
			if (this.Armies.Count == 0 && this.CurrentSelectedArmy != null)
			{
				this.OnSelection(null);
				return;
			}
			if (this.Armies.Count > 0)
			{
				this.OnSelection(this.Armies[0]);
				this.CurrentSelectedArmy.IsSelected = true;
			}
		}

		private void ExecuteManageArmy()
		{
			this._onManageArmy();
		}

		private void ExecuteShowOnMap()
		{
			if (this.CurrentSelectedArmy != null)
			{
				this._showArmyOnMap(this.CurrentSelectedArmy.Army);
			}
		}

		private void RefreshCurrentArmyVisuals(KingdomArmyItemVM item)
		{
			if (item != null)
			{
				if (this.CurrentSelectedArmy != null)
				{
					this.CurrentSelectedArmy.IsSelected = false;
				}
				this.CanManageCurrentArmy = false;
				this.CurrentSelectedArmy = item;
				base.NotificationCount = this._viewDataTracker.NumOfKingdomArmyNotifications;
				this.DisbandCost = Campaign.Current.Models.DiplomacyModel.GetInfluenceCostOfDisbandingArmy();
				this.ChangeLeaderCost = Campaign.Current.Models.DiplomacyModel.GetInfluenceCostOfChangingLeaderOfArmy();
				TextObject textObject;
				this.CanDisbandCurrentArmy = this.GetCanDisbandCurrentArmyWithReason(item, this.DisbandCost, out textObject);
				this.DisbandHint.HintText = textObject;
				this.DisbandActionExplanationText = GameTexts.FindText("str_kingdom_disband_army_explanation", null).ToString();
				if (this.CurrentSelectedArmy != null)
				{
					this.CanShowLocationOfCurrentArmy = this.CurrentSelectedArmy.Army.AiBehaviorObject is Settlement || this.CurrentSelectedArmy.Army.AiBehaviorObject is MobileParty;
					TextObject textObject2;
					this.CanManageCurrentArmy = this.GetCanManageCurrentArmyWithReason(out textObject2);
					this.ManageArmyHint.HintText = textObject2;
				}
			}
		}

		private bool GetCanManageCurrentArmyWithReason(out TextObject disabledReason)
		{
			if (Hero.MainHero.IsPrisoner)
			{
				disabledReason = GameTexts.FindText("str_action_disabled_reason_prisoner", null);
				return false;
			}
			if (PlayerEncounter.Current != null)
			{
				if (PlayerEncounter.EncounterSettlement == null)
				{
					disabledReason = GameTexts.FindText("str_action_disabled_reason_encounter", null);
					return false;
				}
				Village village = PlayerEncounter.EncounterSettlement.Village;
				if (village != null && village.VillageState == Village.VillageStates.BeingRaided && MobileParty.MainParty.MapEvent != null && MobileParty.MainParty.MapEvent.IsRaid)
				{
					disabledReason = GameTexts.FindText("str_action_disabled_reason_raid", null);
					return false;
				}
			}
			KingdomArmyItemVM currentSelectedArmy = this.CurrentSelectedArmy;
			if (currentSelectedArmy == null || !currentSelectedArmy.IsMainArmy)
			{
				disabledReason = TextObject.Empty;
				return false;
			}
			disabledReason = TextObject.Empty;
			return true;
		}

		private bool GetCanDisbandCurrentArmyWithReason(KingdomArmyItemVM armyItem, int disbandCost, out TextObject disabledReason)
		{
			TextObject textObject;
			if (!CampaignUIHelper.GetMapScreenActionIsEnabledWithReason(out textObject))
			{
				disabledReason = textObject;
				return false;
			}
			if (Clan.PlayerClan.IsUnderMercenaryService)
			{
				disabledReason = GameTexts.FindText("str_cannot_disband_army_while_mercenary", null);
				return false;
			}
			if (Clan.PlayerClan.Influence < (float)disbandCost)
			{
				disabledReason = GameTexts.FindText("str_warning_you_dont_have_enough_influence", null);
				return false;
			}
			if (armyItem.Army.LeaderParty.MapEvent != null)
			{
				disabledReason = GameTexts.FindText("str_cannot_disband_army_while_in_event", null);
				return false;
			}
			if (armyItem.Army.Parties.Contains(MobileParty.MainParty))
			{
				disabledReason = GameTexts.FindText("str_cannot_disband_army_while_in_that_army", null);
				return false;
			}
			disabledReason = TextObject.Empty;
			return true;
		}

		public void SelectArmy(Army army)
		{
			foreach (KingdomArmyItemVM kingdomArmyItemVM in this.Armies)
			{
				if (kingdomArmyItemVM.Army == army)
				{
					this.OnSelection(kingdomArmyItemVM);
					break;
				}
			}
		}

		private void OnSelection(KingdomArmyItemVM item)
		{
			if (this.CurrentSelectedArmy != item)
			{
				this.RefreshCurrentArmyVisuals(item);
				this.CurrentSelectedArmy = item;
				base.IsAcceptableItemSelected = item != null;
			}
		}

		private void ExecuteDisbandCurrentArmy()
		{
			if (this.CurrentSelectedArmy != null && Hero.MainHero.Clan.Influence >= (float)this.DisbandCost)
			{
				InformationManager.ShowInquiry(new InquiryData(GameTexts.FindText("str_disband_army", null).ToString(), new TextObject("{=zrhr4rDA}Are you sure you want to disband this army? This will result in relation loss.", null).ToString(), true, true, GameTexts.FindText("str_yes", null).ToString(), GameTexts.FindText("str_no", null).ToString(), new Action(this.DisbandCurrentArmy), null, "", 0f, null, null, null), false, false);
			}
		}

		private void DisbandCurrentArmy()
		{
			if (this.CurrentSelectedArmy != null && Hero.MainHero.Clan.Influence >= (float)this.DisbandCost)
			{
				DisbandArmyAction.ApplyByReleasedByPlayerAfterBattle(this.CurrentSelectedArmy.Army);
				this.RefreshArmyList();
			}
		}

		private void RefreshCanManageArmy()
		{
			TextObject textObject;
			bool mapScreenActionIsEnabledWithReason = CampaignUIHelper.GetMapScreenActionIsEnabledWithReason(out textObject);
			this.PlayerHasArmy = MobileParty.MainParty.Army != null;
			bool flag = this._kingdom != null;
			bool isUnderMercenaryService = Clan.PlayerClan.IsUnderMercenaryService;
			bool flag2 = this.PlayerHasArmy && MobileParty.MainParty.Army.LeaderParty == MobileParty.MainParty;
			this.CanCreateArmy = mapScreenActionIsEnabledWithReason && flag && !isUnderMercenaryService && !this.PlayerHasArmy;
			if (!flag)
			{
				this.CreateArmyHint.HintText = new TextObject("{=XSQ0Y9gy}You need to be a part of a kingdom to create an army.", null);
				return;
			}
			if (isUnderMercenaryService)
			{
				this.CreateArmyHint.HintText = new TextObject("{=aRhQzJca}Mercenaries cannot create or manage armies.", null);
				return;
			}
			if (this.PlayerHasArmy && !flag2)
			{
				this.CreateArmyHint.HintText = new TextObject("{=NAA4pajB}You need to leave your current army to create a new one.", null);
				return;
			}
			if (!mapScreenActionIsEnabledWithReason)
			{
				this.CreateArmyHint.HintText = textObject;
				return;
			}
			this.CreateArmyHint.HintText = TextObject.Empty;
		}

		[DataSourceProperty]
		public KingdomArmySortControllerVM ArmySortController
		{
			get
			{
				return this._armySortController;
			}
			set
			{
				if (value != this._armySortController)
				{
					this._armySortController = value;
					base.OnPropertyChangedWithValue<KingdomArmySortControllerVM>(value, "ArmySortController");
				}
			}
		}

		[DataSourceProperty]
		public string CreateArmyText
		{
			get
			{
				return this._createArmyText;
			}
			set
			{
				if (value != this._createArmyText)
				{
					this._createArmyText = value;
					base.OnPropertyChangedWithValue<string>(value, "CreateArmyText");
				}
			}
		}

		[DataSourceProperty]
		public string DisbandActionExplanationText
		{
			get
			{
				return this._disbandActionExplanationText;
			}
			set
			{
				if (value != this._disbandActionExplanationText)
				{
					this._disbandActionExplanationText = value;
					base.OnPropertyChangedWithValue<string>(value, "DisbandActionExplanationText");
				}
			}
		}

		[DataSourceProperty]
		public string ManageActionExplanationText
		{
			get
			{
				return this._manageActionExplanationText;
			}
			set
			{
				if (value != this._manageActionExplanationText)
				{
					this._manageActionExplanationText = value;
					base.OnPropertyChangedWithValue<string>(value, "ManageActionExplanationText");
				}
			}
		}

		[DataSourceProperty]
		public KingdomArmyItemVM CurrentSelectedArmy
		{
			get
			{
				return this._currentSelectedArmy;
			}
			set
			{
				if (value != this._currentSelectedArmy)
				{
					this._currentSelectedArmy = value;
					base.OnPropertyChangedWithValue<KingdomArmyItemVM>(value, "CurrentSelectedArmy");
				}
			}
		}

		[DataSourceProperty]
		public HintViewModel CreateArmyHint
		{
			get
			{
				return this._createArmyHint;
			}
			set
			{
				if (value != this._createArmyHint)
				{
					this._createArmyHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "CreateArmyHint");
				}
			}
		}

		[DataSourceProperty]
		public HintViewModel ManageArmyHint
		{
			get
			{
				return this._manageArmyHint;
			}
			set
			{
				if (value != this._manageArmyHint)
				{
					this._manageArmyHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "ManageArmyHint");
				}
			}
		}

		[DataSourceProperty]
		public bool PlayerHasArmy
		{
			get
			{
				return this._playerHasArmy;
			}
			set
			{
				if (value != this._playerHasArmy)
				{
					this._playerHasArmy = value;
					base.OnPropertyChangedWithValue(value, "PlayerHasArmy");
				}
			}
		}

		[DataSourceProperty]
		public bool CanCreateArmy
		{
			get
			{
				return this._canCreateArmy;
			}
			set
			{
				if (value != this._canCreateArmy)
				{
					this._canCreateArmy = value;
					base.OnPropertyChangedWithValue(value, "CanCreateArmy");
				}
			}
		}

		[DataSourceProperty]
		public string LeaderText
		{
			get
			{
				return this._categoryLeaderName;
			}
			set
			{
				if (value != this._categoryLeaderName)
				{
					this._categoryLeaderName = value;
					base.OnPropertyChanged("CategoryLeaderName");
				}
			}
		}

		[DataSourceProperty]
		public string ShowOnMapText
		{
			get
			{
				return this._showOnMapText;
			}
			set
			{
				if (value != this._showOnMapText)
				{
					this._showOnMapText = value;
					base.OnPropertyChangedWithValue<string>(value, "ShowOnMapText");
				}
			}
		}

		[DataSourceProperty]
		public string ArmyNameText
		{
			get
			{
				return this._categoryLordCount;
			}
			set
			{
				if (value != this._categoryLordCount)
				{
					this._categoryLordCount = value;
					base.OnPropertyChanged("CategoryLordCount");
				}
			}
		}

		[DataSourceProperty]
		public string StrengthText
		{
			get
			{
				return this._categoryStrength;
			}
			set
			{
				if (value != this._categoryStrength)
				{
					this._categoryStrength = value;
					base.OnPropertyChanged("CategoryStrength");
				}
			}
		}

		[DataSourceProperty]
		public string PartiesText
		{
			get
			{
				return this._categoryParties;
			}
			set
			{
				if (value != this._categoryParties)
				{
					this._categoryParties = value;
					base.OnPropertyChangedWithValue<string>(value, "PartiesText");
				}
			}
		}

		[DataSourceProperty]
		public string LocationText
		{
			get
			{
				return this._categoryObjective;
			}
			set
			{
				if (value != this._categoryObjective)
				{
					this._categoryObjective = value;
					base.OnPropertyChanged("CategoryObjective");
				}
			}
		}

		[DataSourceProperty]
		public MBBindingList<KingdomArmyItemVM> Armies
		{
			get
			{
				return this._armies;
			}
			set
			{
				if (value != this._armies)
				{
					this._armies = value;
					base.OnPropertyChangedWithValue<MBBindingList<KingdomArmyItemVM>>(value, "Armies");
				}
			}
		}

		[DataSourceProperty]
		public bool CanDisbandCurrentArmy
		{
			get
			{
				return this._canDisbandCurrentArmy;
			}
			set
			{
				if (value != this._canDisbandCurrentArmy)
				{
					this._canDisbandCurrentArmy = value;
					base.OnPropertyChangedWithValue(value, "CanDisbandCurrentArmy");
				}
			}
		}

		[DataSourceProperty]
		public bool CanManageCurrentArmy
		{
			get
			{
				return this._canManageCurrentArmy;
			}
			set
			{
				if (value != this._canManageCurrentArmy)
				{
					this._canManageCurrentArmy = value;
					base.OnPropertyChangedWithValue(value, "CanManageCurrentArmy");
				}
			}
		}

		[DataSourceProperty]
		public bool CanChangeLeaderOfCurrentArmy
		{
			get
			{
				return this._canChangeLeaderOfCurrentArmy;
			}
			set
			{
				if (value != this._canChangeLeaderOfCurrentArmy)
				{
					this._canChangeLeaderOfCurrentArmy = value;
					base.OnPropertyChangedWithValue(value, "CanChangeLeaderOfCurrentArmy");
				}
			}
		}

		[DataSourceProperty]
		public bool CanShowLocationOfCurrentArmy
		{
			get
			{
				return this._canShowLocationOfCurrentArmy;
			}
			set
			{
				if (value != this._canShowLocationOfCurrentArmy)
				{
					this._canShowLocationOfCurrentArmy = value;
					base.OnPropertyChangedWithValue(value, "CanShowLocationOfCurrentArmy");
				}
			}
		}

		[DataSourceProperty]
		public string DisbandText
		{
			get
			{
				return this._disbandText;
			}
			set
			{
				if (value != this._disbandText)
				{
					this._disbandText = value;
					base.OnPropertyChangedWithValue<string>(value, "DisbandText");
				}
			}
		}

		[DataSourceProperty]
		public string ManageText
		{
			get
			{
				return this._manageText;
			}
			set
			{
				if (value != this._manageText)
				{
					this._manageText = value;
					base.OnPropertyChangedWithValue<string>(value, "ManageText");
				}
			}
		}

		[DataSourceProperty]
		public int DisbandCost
		{
			get
			{
				return this._disbandCost;
			}
			set
			{
				if (value != this._disbandCost)
				{
					this._disbandCost = value;
					base.OnPropertyChangedWithValue(value, "DisbandCost");
				}
			}
		}

		[DataSourceProperty]
		public string ChangeLeaderText
		{
			get
			{
				return this._changeLeaderText;
			}
			set
			{
				if (value != this._changeLeaderText)
				{
					this._changeLeaderText = value;
					base.OnPropertyChangedWithValue<string>(value, "ChangeLeaderText");
				}
			}
		}

		[DataSourceProperty]
		public int ChangeLeaderCost
		{
			get
			{
				return this._changeLeaderCost;
			}
			set
			{
				if (value != this._changeLeaderCost)
				{
					this._changeLeaderCost = value;
					base.OnPropertyChangedWithValue(value, "ChangeLeaderCost");
				}
			}
		}

		[DataSourceProperty]
		public HintViewModel DisbandHint
		{
			get
			{
				return this._disbandHint;
			}
			set
			{
				if (value != this._disbandHint)
				{
					this._disbandHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "DisbandHint");
				}
			}
		}

		private readonly Action _onManageArmy;

		private readonly Action _refreshDecision;

		private readonly Action<Army> _showArmyOnMap;

		private readonly IViewDataTracker _viewDataTracker;

		private Kingdom _kingdom;

		private MBBindingList<KingdomArmyItemVM> _armies;

		private KingdomArmyItemVM _currentSelectedArmy;

		private HintViewModel _disbandHint;

		private string _categoryLeaderName;

		private string _categoryLordCount;

		private string _categoryStrength;

		private string _categoryObjective;

		private string _categoryParties;

		private string _createArmyText;

		private string _disbandText;

		private string _manageText;

		private string _changeLeaderText;

		private string _showOnMapText;

		private string _disbandActionExplanationText;

		private string _manageActionExplanationText;

		private bool _canCreateArmy;

		private bool _playerHasArmy;

		private HintViewModel _createArmyHint;

		private HintViewModel _manageArmyHint;

		private bool _canChangeLeaderOfCurrentArmy;

		private bool _canDisbandCurrentArmy;

		private bool _canShowLocationOfCurrentArmy;

		private bool _canManageCurrentArmy;

		private int _disbandCost;

		private int _changeLeaderCost;

		private KingdomArmySortControllerVM _armySortController;
	}
}
