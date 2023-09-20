using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.CampaignBehaviors;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.KingdomManagement.Armies
{
	// Token: 0x02000076 RID: 118
	public class KingdomArmyVM : KingdomCategoryVM
	{
		// Token: 0x06000A75 RID: 2677 RVA: 0x00029898 File Offset: 0x00027A98
		public KingdomArmyVM(Action onManageArmy, Action refreshDecision, Action<Army> showArmyOnMap)
		{
			this._onManageArmy = onManageArmy;
			this._refreshDecision = refreshDecision;
			this._showArmyOnMap = showArmyOnMap;
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

		// Token: 0x06000A76 RID: 2678 RVA: 0x00029958 File Offset: 0x00027B58
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

		// Token: 0x06000A77 RID: 2679 RVA: 0x00029B0C File Offset: 0x00027D0C
		public void RefreshArmyList()
		{
			base.NotificationCount = PlayerUpdateTracker.Current.NumKingdomArmyNotifications;
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
					goto IL_9F;
				}
			}
			Debug.FailedAssert("Kingdom screen can't open if you're not in kingdom", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.CampaignSystem.ViewModelCollection\\KingdomManagement\\Armies\\KingdomArmyVM.cs", "RefreshArmyList", 79);
			IL_9F:
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

		// Token: 0x06000A78 RID: 2680 RVA: 0x00029C18 File Offset: 0x00027E18
		private void ExecuteManageArmy()
		{
			this._onManageArmy();
		}

		// Token: 0x06000A79 RID: 2681 RVA: 0x00029C25 File Offset: 0x00027E25
		private void ExecuteShowOnMap()
		{
			if (this.CurrentSelectedArmy != null)
			{
				this._showArmyOnMap(this.CurrentSelectedArmy.Army);
			}
		}

		// Token: 0x06000A7A RID: 2682 RVA: 0x00029C48 File Offset: 0x00027E48
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
				base.NotificationCount = PlayerUpdateTracker.Current.NumKingdomArmyNotifications;
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

		// Token: 0x06000A7B RID: 2683 RVA: 0x00029D54 File Offset: 0x00027F54
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

		// Token: 0x06000A7C RID: 2684 RVA: 0x00029E08 File Offset: 0x00028008
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

		// Token: 0x06000A7D RID: 2685 RVA: 0x00029EAC File Offset: 0x000280AC
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

		// Token: 0x06000A7E RID: 2686 RVA: 0x00029F04 File Offset: 0x00028104
		private void OnSelection(KingdomArmyItemVM item)
		{
			if (this.CurrentSelectedArmy != item)
			{
				this.RefreshCurrentArmyVisuals(item);
				this.CurrentSelectedArmy = item;
				base.IsAcceptableItemSelected = item != null;
			}
		}

		// Token: 0x06000A7F RID: 2687 RVA: 0x00029F28 File Offset: 0x00028128
		private void ExecuteDisbandCurrentArmy()
		{
			if (this.CurrentSelectedArmy != null && Hero.MainHero.Clan.Influence >= (float)this.DisbandCost)
			{
				InformationManager.ShowInquiry(new InquiryData(GameTexts.FindText("str_disband_army", null).ToString(), new TextObject("{=zrhr4rDA}Are you sure you want to disband this army? This will result in relation loss.", null).ToString(), true, true, GameTexts.FindText("str_yes", null).ToString(), GameTexts.FindText("str_no", null).ToString(), new Action(this.DisbandCurrentArmy), null, "", 0f, null, null, null), false, false);
			}
		}

		// Token: 0x06000A80 RID: 2688 RVA: 0x00029FC0 File Offset: 0x000281C0
		private void DisbandCurrentArmy()
		{
			if (this.CurrentSelectedArmy != null && Hero.MainHero.Clan.Influence >= (float)this.DisbandCost)
			{
				DisbandArmyAction.ApplyByReleasedByPlayerAfterBattle(this.CurrentSelectedArmy.Army);
				this.RefreshArmyList();
			}
		}

		// Token: 0x06000A81 RID: 2689 RVA: 0x00029FF8 File Offset: 0x000281F8
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

		// Token: 0x17000363 RID: 867
		// (get) Token: 0x06000A82 RID: 2690 RVA: 0x0002A0E9 File Offset: 0x000282E9
		// (set) Token: 0x06000A83 RID: 2691 RVA: 0x0002A0F1 File Offset: 0x000282F1
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

		// Token: 0x17000364 RID: 868
		// (get) Token: 0x06000A84 RID: 2692 RVA: 0x0002A10F File Offset: 0x0002830F
		// (set) Token: 0x06000A85 RID: 2693 RVA: 0x0002A117 File Offset: 0x00028317
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

		// Token: 0x17000365 RID: 869
		// (get) Token: 0x06000A86 RID: 2694 RVA: 0x0002A13A File Offset: 0x0002833A
		// (set) Token: 0x06000A87 RID: 2695 RVA: 0x0002A142 File Offset: 0x00028342
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

		// Token: 0x17000366 RID: 870
		// (get) Token: 0x06000A88 RID: 2696 RVA: 0x0002A165 File Offset: 0x00028365
		// (set) Token: 0x06000A89 RID: 2697 RVA: 0x0002A16D File Offset: 0x0002836D
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

		// Token: 0x17000367 RID: 871
		// (get) Token: 0x06000A8A RID: 2698 RVA: 0x0002A190 File Offset: 0x00028390
		// (set) Token: 0x06000A8B RID: 2699 RVA: 0x0002A198 File Offset: 0x00028398
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

		// Token: 0x17000368 RID: 872
		// (get) Token: 0x06000A8C RID: 2700 RVA: 0x0002A1B6 File Offset: 0x000283B6
		// (set) Token: 0x06000A8D RID: 2701 RVA: 0x0002A1BE File Offset: 0x000283BE
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

		// Token: 0x17000369 RID: 873
		// (get) Token: 0x06000A8E RID: 2702 RVA: 0x0002A1DC File Offset: 0x000283DC
		// (set) Token: 0x06000A8F RID: 2703 RVA: 0x0002A1E4 File Offset: 0x000283E4
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

		// Token: 0x1700036A RID: 874
		// (get) Token: 0x06000A90 RID: 2704 RVA: 0x0002A202 File Offset: 0x00028402
		// (set) Token: 0x06000A91 RID: 2705 RVA: 0x0002A20A File Offset: 0x0002840A
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

		// Token: 0x1700036B RID: 875
		// (get) Token: 0x06000A92 RID: 2706 RVA: 0x0002A228 File Offset: 0x00028428
		// (set) Token: 0x06000A93 RID: 2707 RVA: 0x0002A230 File Offset: 0x00028430
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

		// Token: 0x1700036C RID: 876
		// (get) Token: 0x06000A94 RID: 2708 RVA: 0x0002A24E File Offset: 0x0002844E
		// (set) Token: 0x06000A95 RID: 2709 RVA: 0x0002A256 File Offset: 0x00028456
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

		// Token: 0x1700036D RID: 877
		// (get) Token: 0x06000A96 RID: 2710 RVA: 0x0002A278 File Offset: 0x00028478
		// (set) Token: 0x06000A97 RID: 2711 RVA: 0x0002A280 File Offset: 0x00028480
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

		// Token: 0x1700036E RID: 878
		// (get) Token: 0x06000A98 RID: 2712 RVA: 0x0002A2A3 File Offset: 0x000284A3
		// (set) Token: 0x06000A99 RID: 2713 RVA: 0x0002A2AB File Offset: 0x000284AB
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

		// Token: 0x1700036F RID: 879
		// (get) Token: 0x06000A9A RID: 2714 RVA: 0x0002A2CD File Offset: 0x000284CD
		// (set) Token: 0x06000A9B RID: 2715 RVA: 0x0002A2D5 File Offset: 0x000284D5
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

		// Token: 0x17000370 RID: 880
		// (get) Token: 0x06000A9C RID: 2716 RVA: 0x0002A2F7 File Offset: 0x000284F7
		// (set) Token: 0x06000A9D RID: 2717 RVA: 0x0002A2FF File Offset: 0x000284FF
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

		// Token: 0x17000371 RID: 881
		// (get) Token: 0x06000A9E RID: 2718 RVA: 0x0002A322 File Offset: 0x00028522
		// (set) Token: 0x06000A9F RID: 2719 RVA: 0x0002A32A File Offset: 0x0002852A
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

		// Token: 0x17000372 RID: 882
		// (get) Token: 0x06000AA0 RID: 2720 RVA: 0x0002A34C File Offset: 0x0002854C
		// (set) Token: 0x06000AA1 RID: 2721 RVA: 0x0002A354 File Offset: 0x00028554
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

		// Token: 0x17000373 RID: 883
		// (get) Token: 0x06000AA2 RID: 2722 RVA: 0x0002A372 File Offset: 0x00028572
		// (set) Token: 0x06000AA3 RID: 2723 RVA: 0x0002A37A File Offset: 0x0002857A
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

		// Token: 0x17000374 RID: 884
		// (get) Token: 0x06000AA4 RID: 2724 RVA: 0x0002A398 File Offset: 0x00028598
		// (set) Token: 0x06000AA5 RID: 2725 RVA: 0x0002A3A0 File Offset: 0x000285A0
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

		// Token: 0x17000375 RID: 885
		// (get) Token: 0x06000AA6 RID: 2726 RVA: 0x0002A3BE File Offset: 0x000285BE
		// (set) Token: 0x06000AA7 RID: 2727 RVA: 0x0002A3C6 File Offset: 0x000285C6
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

		// Token: 0x17000376 RID: 886
		// (get) Token: 0x06000AA8 RID: 2728 RVA: 0x0002A3E4 File Offset: 0x000285E4
		// (set) Token: 0x06000AA9 RID: 2729 RVA: 0x0002A3EC File Offset: 0x000285EC
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

		// Token: 0x17000377 RID: 887
		// (get) Token: 0x06000AAA RID: 2730 RVA: 0x0002A40A File Offset: 0x0002860A
		// (set) Token: 0x06000AAB RID: 2731 RVA: 0x0002A412 File Offset: 0x00028612
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

		// Token: 0x17000378 RID: 888
		// (get) Token: 0x06000AAC RID: 2732 RVA: 0x0002A435 File Offset: 0x00028635
		// (set) Token: 0x06000AAD RID: 2733 RVA: 0x0002A43D File Offset: 0x0002863D
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

		// Token: 0x17000379 RID: 889
		// (get) Token: 0x06000AAE RID: 2734 RVA: 0x0002A460 File Offset: 0x00028660
		// (set) Token: 0x06000AAF RID: 2735 RVA: 0x0002A468 File Offset: 0x00028668
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

		// Token: 0x1700037A RID: 890
		// (get) Token: 0x06000AB0 RID: 2736 RVA: 0x0002A486 File Offset: 0x00028686
		// (set) Token: 0x06000AB1 RID: 2737 RVA: 0x0002A48E File Offset: 0x0002868E
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

		// Token: 0x1700037B RID: 891
		// (get) Token: 0x06000AB2 RID: 2738 RVA: 0x0002A4B1 File Offset: 0x000286B1
		// (set) Token: 0x06000AB3 RID: 2739 RVA: 0x0002A4B9 File Offset: 0x000286B9
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

		// Token: 0x1700037C RID: 892
		// (get) Token: 0x06000AB4 RID: 2740 RVA: 0x0002A4D7 File Offset: 0x000286D7
		// (set) Token: 0x06000AB5 RID: 2741 RVA: 0x0002A4DF File Offset: 0x000286DF
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

		// Token: 0x040004B9 RID: 1209
		private readonly Action _onManageArmy;

		// Token: 0x040004BA RID: 1210
		private readonly Action _refreshDecision;

		// Token: 0x040004BB RID: 1211
		private readonly Action<Army> _showArmyOnMap;

		// Token: 0x040004BC RID: 1212
		private Kingdom _kingdom;

		// Token: 0x040004BD RID: 1213
		private MBBindingList<KingdomArmyItemVM> _armies;

		// Token: 0x040004BE RID: 1214
		private KingdomArmyItemVM _currentSelectedArmy;

		// Token: 0x040004BF RID: 1215
		private HintViewModel _disbandHint;

		// Token: 0x040004C0 RID: 1216
		private string _categoryLeaderName;

		// Token: 0x040004C1 RID: 1217
		private string _categoryLordCount;

		// Token: 0x040004C2 RID: 1218
		private string _categoryStrength;

		// Token: 0x040004C3 RID: 1219
		private string _categoryObjective;

		// Token: 0x040004C4 RID: 1220
		private string _categoryParties;

		// Token: 0x040004C5 RID: 1221
		private string _createArmyText;

		// Token: 0x040004C6 RID: 1222
		private string _disbandText;

		// Token: 0x040004C7 RID: 1223
		private string _manageText;

		// Token: 0x040004C8 RID: 1224
		private string _changeLeaderText;

		// Token: 0x040004C9 RID: 1225
		private string _showOnMapText;

		// Token: 0x040004CA RID: 1226
		private string _disbandActionExplanationText;

		// Token: 0x040004CB RID: 1227
		private string _manageActionExplanationText;

		// Token: 0x040004CC RID: 1228
		private bool _canCreateArmy;

		// Token: 0x040004CD RID: 1229
		private bool _playerHasArmy;

		// Token: 0x040004CE RID: 1230
		private HintViewModel _createArmyHint;

		// Token: 0x040004CF RID: 1231
		private HintViewModel _manageArmyHint;

		// Token: 0x040004D0 RID: 1232
		private bool _canChangeLeaderOfCurrentArmy;

		// Token: 0x040004D1 RID: 1233
		private bool _canDisbandCurrentArmy;

		// Token: 0x040004D2 RID: 1234
		private bool _canShowLocationOfCurrentArmy;

		// Token: 0x040004D3 RID: 1235
		private bool _canManageCurrentArmy;

		// Token: 0x040004D4 RID: 1236
		private int _disbandCost;

		// Token: 0x040004D5 RID: 1237
		private int _changeLeaderCost;

		// Token: 0x040004D6 RID: 1238
		private KingdomArmySortControllerVM _armySortController;
	}
}
