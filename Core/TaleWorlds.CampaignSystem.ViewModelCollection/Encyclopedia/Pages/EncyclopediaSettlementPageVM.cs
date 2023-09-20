using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem.Encyclopedia;
using TaleWorlds.CampaignSystem.LogEntries;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.ViewModelCollection.Encyclopedia.Items;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Core.ViewModelCollection.Tutorial;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.Encyclopedia.Pages
{
	// Token: 0x020000B9 RID: 185
	[EncyclopediaViewModel(typeof(Settlement))]
	public class EncyclopediaSettlementPageVM : EncyclopediaContentPageVM
	{
		// Token: 0x0600121B RID: 4635 RVA: 0x00046DA0 File Offset: 0x00044FA0
		public EncyclopediaSettlementPageVM(EncyclopediaPageArgs args)
			: base(args)
		{
			this._settlement = base.Obj as Settlement;
			this.NotableCharacters = new MBBindingList<HeroVM>();
			this.Settlements = new MBBindingList<EncyclopediaSettlementVM>();
			this.History = new MBBindingList<EncyclopediaHistoryEventVM>();
			this._isVisualTrackerSelected = Campaign.Current.VisualTrackerManager.CheckTracked(this._settlement);
			this.IsFortification = this._settlement.IsFortification;
			this.SettlementImageID = this._settlement.SettlementComponent.WaitMeshName;
			base.IsBookmarked = Campaign.Current.EncyclopediaManager.ViewDataTracker.IsEncyclopediaBookmarked(this._settlement);
			Game.Current.EventManager.RegisterEvent<TutorialNotificationElementChangeEvent>(new Action<TutorialNotificationElementChangeEvent>(this.OnTutorialNotificationElementIDChange));
			this.RefreshValues();
			TextObject textObject;
			if (CampaignUIHelper.IsSettlementInformationHidden(this._settlement, out textObject))
			{
				Game.Current.EventManager.TriggerEvent<EncyclopediaPageChangedEvent>(new EncyclopediaPageChangedEvent(EncyclopediaPages.Settlement, true));
			}
		}

		// Token: 0x0600121C RID: 4636 RVA: 0x00046E90 File Offset: 0x00045090
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.SettlementName = this._settlement.Name.ToString();
			this.SettlementsText = GameTexts.FindText("str_villages", null).ToString();
			this.NotableCharactersText = GameTexts.FindText("str_notable_characters", null).ToString();
			this.OwnerText = GameTexts.FindText("str_owner", null).ToString();
			this.TrackText = GameTexts.FindText("str_settlement_track", null).ToString();
			this.ShowInMapHint = new HintViewModel(GameTexts.FindText("str_show_on_map", null), null);
			this.InformationText = this._settlement.EncyclopediaText.ToString();
			base.UpdateBookmarkHintText();
			this.Refresh();
		}

		// Token: 0x0600121D RID: 4637 RVA: 0x00046F4C File Offset: 0x0004514C
		public override void Refresh()
		{
			base.IsLoadingOver = false;
			SettlementComponent settlementComponent = this._settlement.SettlementComponent;
			this.NotableCharacters.Clear();
			this.Settlements.Clear();
			this.History.Clear();
			this.IsFortification = this._settlement.IsFortification;
			if (this._settlement.IsFortification)
			{
				this.SettlementType = 0;
				EncyclopediaPage pageOf = Campaign.Current.EncyclopediaManager.GetPageOf(typeof(Settlement));
				using (List<Village>.Enumerator enumerator = this._settlement.BoundVillages.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						Village village = enumerator.Current;
						if (pageOf.IsValidEncyclopediaItem(village.Owner.Settlement))
						{
							this.Settlements.Add(new EncyclopediaSettlementVM(village.Owner.Settlement));
						}
					}
					goto IL_F2;
				}
			}
			if (this._settlement.IsVillage)
			{
				this.SettlementType = 1;
			}
			IL_F2:
			if (!this._settlement.IsCastle)
			{
				EncyclopediaPage pageOf2 = Campaign.Current.EncyclopediaManager.GetPageOf(typeof(Hero));
				foreach (Hero hero in this._settlement.Notables)
				{
					if (pageOf2.IsValidEncyclopediaItem(hero))
					{
						this.NotableCharacters.Add(new HeroVM(hero, false));
					}
				}
			}
			GameTexts.SetVariable("STR1", GameTexts.FindText("str_enc_sf_culture", null).ToString());
			GameTexts.SetVariable("STR2", this._settlement.Culture.Name.ToString());
			this.CultureText = GameTexts.FindText("str_STR1_space_STR2", null).ToString();
			this.OwnerText = GameTexts.FindText("str_owner", null).ToString();
			this.Owner = new HeroVM(this._settlement.OwnerClan.Leader, false);
			this.OwnerBanner = new EncyclopediaFactionVM(this._settlement.OwnerClan);
			this.SettlementPath = settlementComponent.BackgroundMeshName;
			this.SettlementCropPosition = (double)settlementComponent.BackgroundCropPosition;
			this.HasBoundSettlement = this._settlement.IsVillage;
			this.BoundSettlement = (this.HasBoundSettlement ? new EncyclopediaSettlementVM(this._settlement.Village.Bound) : null);
			this.BoundSettlementText = "";
			if (this.HasBoundSettlement)
			{
				GameTexts.SetVariable("SETTLEMENT_LINK", this._settlement.Village.Bound.EncyclopediaLinkWithName);
				this.BoundSettlementText = GameTexts.FindText("str_bound_settlement_encyclopedia", null).ToString();
			}
			int num = (int)this._settlement.Militia;
			TextObject textObject;
			bool flag = CampaignUIHelper.IsSettlementInformationHidden(this._settlement, out textObject);
			string text = GameTexts.FindText("str_missing_info_indicator", null).ToString();
			this.MilitasText = (flag ? text : num.ToString());
			if (this._settlement.IsFortification)
			{
				this.FoodHint = new BasicTooltipViewModel(() => CampaignUIHelper.GetTownFoodTooltip(this._settlement.Town));
				this.LoyaltyHint = new BasicTooltipViewModel(() => CampaignUIHelper.GetTownLoyaltyTooltip(this._settlement.Town));
				this.MilitasHint = new BasicTooltipViewModel(() => CampaignUIHelper.GetTownMilitiaTooltip(this._settlement.Town));
				this.ProsperityHint = new BasicTooltipViewModel(() => CampaignUIHelper.GetTownProsperityTooltip(this._settlement.Town));
				this.WallsHint = new BasicTooltipViewModel(() => CampaignUIHelper.GetTownWallsTooltip(this._settlement.Town));
				this.GarrisonHint = new BasicTooltipViewModel(() => CampaignUIHelper.GetTownGarrisonTooltip(this._settlement.Town));
				this.SecurityHint = new BasicTooltipViewModel(() => CampaignUIHelper.GetTownSecurityTooltip(this._settlement.Town));
				this.ProsperityText = (flag ? text : ((int)this._settlement.Town.Prosperity).ToString());
				this.LoyaltyText = (flag ? text : ((int)this._settlement.Town.Loyalty).ToString());
				this.SecurityText = (flag ? text : ((int)this._settlement.Town.Security).ToString());
				string text2;
				if (!flag)
				{
					MobileParty garrisonParty = this._settlement.Town.GarrisonParty;
					text2 = ((garrisonParty != null) ? garrisonParty.Party.NumberOfAllMembers.ToString() : null) ?? "0";
				}
				else
				{
					text2 = text;
				}
				this.GarrisonText = text2;
				this.FoodText = (flag ? text : ((int)this._settlement.Town.FoodStocks).ToString());
				this.WallsText = (flag ? text : this._settlement.Town.GetWallLevel().ToString());
			}
			else
			{
				this.MilitasHint = new BasicTooltipViewModel(() => CampaignUIHelper.GetVillageMilitiaTooltip(this._settlement.Village));
				this.ProsperityHint = new BasicTooltipViewModel(() => CampaignUIHelper.GetVillageProsperityTooltip(this._settlement.Village));
				this.LoyaltyHint = new BasicTooltipViewModel();
				this.WallsHint = new BasicTooltipViewModel();
				this.ProsperityText = (flag ? text : ((int)this._settlement.Village.Hearth).ToString());
				this.LoyaltyText = "-";
				this.SecurityText = "-";
				this.FoodText = "-";
				this.GarrisonText = "-";
				this.WallsText = "-";
			}
			this.NameText = this._settlement.Name.ToString();
			for (int i = Campaign.Current.LogEntryHistory.GameActionLogs.Count - 1; i >= 0; i--)
			{
				IEncyclopediaLog encyclopediaLog;
				if ((encyclopediaLog = Campaign.Current.LogEntryHistory.GameActionLogs[i] as IEncyclopediaLog) != null && encyclopediaLog.IsVisibleInEncyclopediaPageOf<Settlement>(this._settlement))
				{
					this.History.Add(new EncyclopediaHistoryEventVM(encyclopediaLog));
				}
			}
			this.IsVisualTrackerSelected = Campaign.Current.VisualTrackerManager.CheckTracked(this._settlement);
			base.IsLoadingOver = true;
		}

		// Token: 0x0600121E RID: 4638 RVA: 0x00047544 File Offset: 0x00045744
		public override string GetName()
		{
			return this._settlement.Name.ToString();
		}

		// Token: 0x0600121F RID: 4639 RVA: 0x00047558 File Offset: 0x00045758
		public void ExecuteTrack()
		{
			if (!this.IsVisualTrackerSelected)
			{
				Campaign.Current.VisualTrackerManager.RegisterObject(this._settlement);
				this.IsVisualTrackerSelected = true;
			}
			else
			{
				Campaign.Current.VisualTrackerManager.RemoveTrackedObject(this._settlement, false);
				this.IsVisualTrackerSelected = false;
			}
			Game.Current.EventManager.TriggerEvent<PlayerToggleTrackSettlementFromEncyclopediaEvent>(new PlayerToggleTrackSettlementFromEncyclopediaEvent(this._settlement, this.IsVisualTrackerSelected));
		}

		// Token: 0x06001220 RID: 4640 RVA: 0x000475C8 File Offset: 0x000457C8
		public override string GetNavigationBarURL()
		{
			return HyperlinkTexts.GetGenericHyperlinkText("Home", GameTexts.FindText("str_encyclopedia_home", null).ToString()) + " \\ " + HyperlinkTexts.GetGenericHyperlinkText("ListPage-Settlements", GameTexts.FindText("str_encyclopedia_settlements", null).ToString()) + " \\ " + this.GetName();
		}

		// Token: 0x06001221 RID: 4641 RVA: 0x0004762D File Offset: 0x0004582D
		public void ExecuteBoundSettlementLink()
		{
			if (this.HasBoundSettlement)
			{
				Campaign.Current.EncyclopediaManager.GoToLink(this._settlement.Village.Bound.EncyclopediaLink);
			}
		}

		// Token: 0x06001222 RID: 4642 RVA: 0x0004765C File Offset: 0x0004585C
		public override void ExecuteSwitchBookmarkedState()
		{
			base.ExecuteSwitchBookmarkedState();
			if (base.IsBookmarked)
			{
				Campaign.Current.EncyclopediaManager.ViewDataTracker.AddEncyclopediaBookmarkToItem(this._settlement);
				return;
			}
			Campaign.Current.EncyclopediaManager.ViewDataTracker.RemoveEncyclopediaBookmarkFromItem(this._settlement);
		}

		// Token: 0x06001223 RID: 4643 RVA: 0x000476AC File Offset: 0x000458AC
		private void OnTutorialNotificationElementIDChange(TutorialNotificationElementChangeEvent evnt)
		{
			this.IsTrackerButtonHighlightEnabled = evnt.NewNotificationElementID == "EncyclopediaItemTrackButton";
		}

		// Token: 0x06001224 RID: 4644 RVA: 0x000476C4 File Offset: 0x000458C4
		public override void OnFinalize()
		{
			base.OnFinalize();
			Game.Current.EventManager.UnregisterEvent<TutorialNotificationElementChangeEvent>(new Action<TutorialNotificationElementChangeEvent>(this.OnTutorialNotificationElementIDChange));
		}

		// Token: 0x17000608 RID: 1544
		// (get) Token: 0x06001225 RID: 4645 RVA: 0x000476E7 File Offset: 0x000458E7
		// (set) Token: 0x06001226 RID: 4646 RVA: 0x000476EF File Offset: 0x000458EF
		[DataSourceProperty]
		public EncyclopediaFactionVM OwnerBanner
		{
			get
			{
				return this._ownerBanner;
			}
			set
			{
				if (value != this._ownerBanner)
				{
					this._ownerBanner = value;
					base.OnPropertyChangedWithValue<EncyclopediaFactionVM>(value, "OwnerBanner");
				}
			}
		}

		// Token: 0x17000609 RID: 1545
		// (get) Token: 0x06001227 RID: 4647 RVA: 0x0004770D File Offset: 0x0004590D
		// (set) Token: 0x06001228 RID: 4648 RVA: 0x00047715 File Offset: 0x00045915
		[DataSourceProperty]
		public EncyclopediaSettlementVM BoundSettlement
		{
			get
			{
				return this._boundSettlement;
			}
			set
			{
				if (value != this._boundSettlement)
				{
					this._boundSettlement = value;
					base.OnPropertyChangedWithValue<EncyclopediaSettlementVM>(value, "BoundSettlement");
				}
			}
		}

		// Token: 0x1700060A RID: 1546
		// (get) Token: 0x06001229 RID: 4649 RVA: 0x00047733 File Offset: 0x00045933
		// (set) Token: 0x0600122A RID: 4650 RVA: 0x0004773B File Offset: 0x0004593B
		[DataSourceProperty]
		public bool IsFortification
		{
			get
			{
				return this._isFortification;
			}
			set
			{
				if (value != this._isFortification)
				{
					this._isFortification = value;
					base.OnPropertyChangedWithValue(value, "IsFortification");
				}
			}
		}

		// Token: 0x1700060B RID: 1547
		// (get) Token: 0x0600122B RID: 4651 RVA: 0x00047759 File Offset: 0x00045959
		// (set) Token: 0x0600122C RID: 4652 RVA: 0x00047761 File Offset: 0x00045961
		[DataSourceProperty]
		public bool IsTrackerButtonHighlightEnabled
		{
			get
			{
				return this._isTrackerButtonHighlightEnabled;
			}
			set
			{
				if (value != this._isTrackerButtonHighlightEnabled)
				{
					this._isTrackerButtonHighlightEnabled = value;
					base.OnPropertyChangedWithValue(value, "IsTrackerButtonHighlightEnabled");
				}
			}
		}

		// Token: 0x1700060C RID: 1548
		// (get) Token: 0x0600122D RID: 4653 RVA: 0x0004777F File Offset: 0x0004597F
		// (set) Token: 0x0600122E RID: 4654 RVA: 0x00047787 File Offset: 0x00045987
		[DataSourceProperty]
		public bool HasBoundSettlement
		{
			get
			{
				return this._hasBoundSettlement;
			}
			set
			{
				if (value != this._hasBoundSettlement)
				{
					this._hasBoundSettlement = value;
					base.OnPropertyChangedWithValue(value, "HasBoundSettlement");
				}
			}
		}

		// Token: 0x1700060D RID: 1549
		// (get) Token: 0x0600122F RID: 4655 RVA: 0x000477A5 File Offset: 0x000459A5
		// (set) Token: 0x06001230 RID: 4656 RVA: 0x000477AD File Offset: 0x000459AD
		[DataSourceProperty]
		public double SettlementCropPosition
		{
			get
			{
				return this._settlementCropPosition;
			}
			set
			{
				if (value != this._settlementCropPosition)
				{
					this._settlementCropPosition = value;
					base.OnPropertyChangedWithValue(value, "SettlementCropPosition");
				}
			}
		}

		// Token: 0x1700060E RID: 1550
		// (get) Token: 0x06001231 RID: 4657 RVA: 0x000477CB File Offset: 0x000459CB
		// (set) Token: 0x06001232 RID: 4658 RVA: 0x000477D3 File Offset: 0x000459D3
		[DataSourceProperty]
		public string BoundSettlementText
		{
			get
			{
				return this._boundSettlementText;
			}
			set
			{
				if (value != this._boundSettlementText)
				{
					this._boundSettlementText = value;
					base.OnPropertyChangedWithValue<string>(value, "BoundSettlementText");
				}
			}
		}

		// Token: 0x1700060F RID: 1551
		// (get) Token: 0x06001233 RID: 4659 RVA: 0x000477F6 File Offset: 0x000459F6
		// (set) Token: 0x06001234 RID: 4660 RVA: 0x000477FE File Offset: 0x000459FE
		[DataSourceProperty]
		public string TrackText
		{
			get
			{
				return this._trackText;
			}
			set
			{
				if (value != this._trackText)
				{
					this._trackText = value;
					base.OnPropertyChangedWithValue<string>(value, "TrackText");
				}
			}
		}

		// Token: 0x17000610 RID: 1552
		// (get) Token: 0x06001235 RID: 4661 RVA: 0x00047821 File Offset: 0x00045A21
		// (set) Token: 0x06001236 RID: 4662 RVA: 0x00047829 File Offset: 0x00045A29
		[DataSourceProperty]
		public string SettlementPath
		{
			get
			{
				return this._settlementPath;
			}
			set
			{
				if (value != this._settlementPath)
				{
					this._settlementPath = value;
					base.OnPropertyChangedWithValue<string>(value, "SettlementPath");
				}
			}
		}

		// Token: 0x17000611 RID: 1553
		// (get) Token: 0x06001237 RID: 4663 RVA: 0x0004784C File Offset: 0x00045A4C
		// (set) Token: 0x06001238 RID: 4664 RVA: 0x00047854 File Offset: 0x00045A54
		[DataSourceProperty]
		public string SettlementName
		{
			get
			{
				return this._settlementName;
			}
			set
			{
				if (value != this._settlementName)
				{
					this._settlementName = value;
					base.OnPropertyChangedWithValue<string>(value, "SettlementName");
				}
			}
		}

		// Token: 0x17000612 RID: 1554
		// (get) Token: 0x06001239 RID: 4665 RVA: 0x00047877 File Offset: 0x00045A77
		// (set) Token: 0x0600123A RID: 4666 RVA: 0x0004787F File Offset: 0x00045A7F
		[DataSourceProperty]
		public string InformationText
		{
			get
			{
				return this._informationText;
			}
			set
			{
				if (value != this._informationText)
				{
					this._informationText = value;
					base.OnPropertyChangedWithValue<string>(value, "InformationText");
				}
			}
		}

		// Token: 0x17000613 RID: 1555
		// (get) Token: 0x0600123B RID: 4667 RVA: 0x000478A2 File Offset: 0x00045AA2
		// (set) Token: 0x0600123C RID: 4668 RVA: 0x000478AA File Offset: 0x00045AAA
		[DataSourceProperty]
		public HeroVM Owner
		{
			get
			{
				return this._owner;
			}
			set
			{
				if (value != this._owner)
				{
					this._owner = value;
					base.OnPropertyChangedWithValue<HeroVM>(value, "Owner");
				}
			}
		}

		// Token: 0x17000614 RID: 1556
		// (get) Token: 0x0600123D RID: 4669 RVA: 0x000478C8 File Offset: 0x00045AC8
		// (set) Token: 0x0600123E RID: 4670 RVA: 0x000478D0 File Offset: 0x00045AD0
		[DataSourceProperty]
		public string SettlementsText
		{
			get
			{
				return this._villagesText;
			}
			set
			{
				if (value != this._villagesText)
				{
					this._villagesText = value;
					base.OnPropertyChanged("VillagesText");
				}
			}
		}

		// Token: 0x17000615 RID: 1557
		// (get) Token: 0x0600123F RID: 4671 RVA: 0x000478F2 File Offset: 0x00045AF2
		// (set) Token: 0x06001240 RID: 4672 RVA: 0x000478FA File Offset: 0x00045AFA
		[DataSourceProperty]
		public string SettlementImageID
		{
			get
			{
				return this._settlementImageID;
			}
			set
			{
				if (value != this._settlementImageID)
				{
					this._settlementImageID = value;
					base.OnPropertyChangedWithValue<string>(value, "SettlementImageID");
				}
			}
		}

		// Token: 0x17000616 RID: 1558
		// (get) Token: 0x06001241 RID: 4673 RVA: 0x0004791D File Offset: 0x00045B1D
		// (set) Token: 0x06001242 RID: 4674 RVA: 0x00047925 File Offset: 0x00045B25
		[DataSourceProperty]
		public string NotableCharactersText
		{
			get
			{
				return this._notableCharactersText;
			}
			set
			{
				if (value != this._notableCharactersText)
				{
					this._notableCharactersText = value;
					base.OnPropertyChangedWithValue<string>(value, "NotableCharactersText");
				}
			}
		}

		// Token: 0x17000617 RID: 1559
		// (get) Token: 0x06001243 RID: 4675 RVA: 0x00047948 File Offset: 0x00045B48
		// (set) Token: 0x06001244 RID: 4676 RVA: 0x00047950 File Offset: 0x00045B50
		[DataSourceProperty]
		public int SettlementType
		{
			get
			{
				return this._settlementType;
			}
			set
			{
				if (value != this._settlementType)
				{
					this._settlementType = value;
					base.OnPropertyChangedWithValue(value, "SettlementType");
				}
			}
		}

		// Token: 0x17000618 RID: 1560
		// (get) Token: 0x06001245 RID: 4677 RVA: 0x0004796E File Offset: 0x00045B6E
		// (set) Token: 0x06001246 RID: 4678 RVA: 0x00047976 File Offset: 0x00045B76
		[DataSourceProperty]
		public MBBindingList<EncyclopediaHistoryEventVM> History
		{
			get
			{
				return this._history;
			}
			set
			{
				if (value != this._history)
				{
					this._history = value;
					base.OnPropertyChangedWithValue<MBBindingList<EncyclopediaHistoryEventVM>>(value, "History");
				}
			}
		}

		// Token: 0x17000619 RID: 1561
		// (get) Token: 0x06001247 RID: 4679 RVA: 0x00047994 File Offset: 0x00045B94
		// (set) Token: 0x06001248 RID: 4680 RVA: 0x0004799C File Offset: 0x00045B9C
		[DataSourceProperty]
		public MBBindingList<EncyclopediaSettlementVM> Settlements
		{
			get
			{
				return this._settlements;
			}
			set
			{
				if (value != this._settlements)
				{
					this._settlements = value;
					base.OnPropertyChanged("Villages");
				}
			}
		}

		// Token: 0x1700061A RID: 1562
		// (get) Token: 0x06001249 RID: 4681 RVA: 0x000479B9 File Offset: 0x00045BB9
		// (set) Token: 0x0600124A RID: 4682 RVA: 0x000479C1 File Offset: 0x00045BC1
		[DataSourceProperty]
		public MBBindingList<HeroVM> NotableCharacters
		{
			get
			{
				return this._notableCharacters;
			}
			set
			{
				if (value != this._notableCharacters)
				{
					this._notableCharacters = value;
					base.OnPropertyChangedWithValue<MBBindingList<HeroVM>>(value, "NotableCharacters");
				}
			}
		}

		// Token: 0x1700061B RID: 1563
		// (get) Token: 0x0600124B RID: 4683 RVA: 0x000479DF File Offset: 0x00045BDF
		// (set) Token: 0x0600124C RID: 4684 RVA: 0x000479E7 File Offset: 0x00045BE7
		[DataSourceProperty]
		public HintViewModel ShowInMapHint
		{
			get
			{
				return this._showInMapHint;
			}
			set
			{
				if (value != this._showInMapHint)
				{
					this._showInMapHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "ShowInMapHint");
				}
			}
		}

		// Token: 0x1700061C RID: 1564
		// (get) Token: 0x0600124D RID: 4685 RVA: 0x00047A05 File Offset: 0x00045C05
		// (set) Token: 0x0600124E RID: 4686 RVA: 0x00047A0D File Offset: 0x00045C0D
		[DataSourceProperty]
		public BasicTooltipViewModel MilitasHint
		{
			get
			{
				return this._militasHint;
			}
			set
			{
				if (value != this._militasHint)
				{
					this._militasHint = value;
					base.OnPropertyChangedWithValue<BasicTooltipViewModel>(value, "MilitasHint");
				}
			}
		}

		// Token: 0x1700061D RID: 1565
		// (get) Token: 0x0600124F RID: 4687 RVA: 0x00047A2B File Offset: 0x00045C2B
		// (set) Token: 0x06001250 RID: 4688 RVA: 0x00047A33 File Offset: 0x00045C33
		[DataSourceProperty]
		public BasicTooltipViewModel FoodHint
		{
			get
			{
				return this._foodHint;
			}
			set
			{
				if (value != this._foodHint)
				{
					this._foodHint = value;
					base.OnPropertyChangedWithValue<BasicTooltipViewModel>(value, "FoodHint");
				}
			}
		}

		// Token: 0x1700061E RID: 1566
		// (get) Token: 0x06001251 RID: 4689 RVA: 0x00047A51 File Offset: 0x00045C51
		// (set) Token: 0x06001252 RID: 4690 RVA: 0x00047A59 File Offset: 0x00045C59
		[DataSourceProperty]
		public BasicTooltipViewModel GarrisonHint
		{
			get
			{
				return this._garrisonHint;
			}
			set
			{
				if (value != this._garrisonHint)
				{
					this._garrisonHint = value;
					base.OnPropertyChangedWithValue<BasicTooltipViewModel>(value, "GarrisonHint");
				}
			}
		}

		// Token: 0x1700061F RID: 1567
		// (get) Token: 0x06001253 RID: 4691 RVA: 0x00047A77 File Offset: 0x00045C77
		// (set) Token: 0x06001254 RID: 4692 RVA: 0x00047A7F File Offset: 0x00045C7F
		[DataSourceProperty]
		public BasicTooltipViewModel ProsperityHint
		{
			get
			{
				return this._prosperityHint;
			}
			set
			{
				if (value != this._prosperityHint)
				{
					this._prosperityHint = value;
					base.OnPropertyChangedWithValue<BasicTooltipViewModel>(value, "ProsperityHint");
				}
			}
		}

		// Token: 0x17000620 RID: 1568
		// (get) Token: 0x06001255 RID: 4693 RVA: 0x00047A9D File Offset: 0x00045C9D
		// (set) Token: 0x06001256 RID: 4694 RVA: 0x00047AA5 File Offset: 0x00045CA5
		[DataSourceProperty]
		public BasicTooltipViewModel LoyaltyHint
		{
			get
			{
				return this._loyaltyHint;
			}
			set
			{
				if (value != this._loyaltyHint)
				{
					this._loyaltyHint = value;
					base.OnPropertyChangedWithValue<BasicTooltipViewModel>(value, "LoyaltyHint");
				}
			}
		}

		// Token: 0x17000621 RID: 1569
		// (get) Token: 0x06001257 RID: 4695 RVA: 0x00047AC3 File Offset: 0x00045CC3
		// (set) Token: 0x06001258 RID: 4696 RVA: 0x00047ACB File Offset: 0x00045CCB
		[DataSourceProperty]
		public BasicTooltipViewModel SecurityHint
		{
			get
			{
				return this._securityHint;
			}
			set
			{
				if (value != this._securityHint)
				{
					this._securityHint = value;
					base.OnPropertyChangedWithValue<BasicTooltipViewModel>(value, "SecurityHint");
				}
			}
		}

		// Token: 0x17000622 RID: 1570
		// (get) Token: 0x06001259 RID: 4697 RVA: 0x00047AE9 File Offset: 0x00045CE9
		// (set) Token: 0x0600125A RID: 4698 RVA: 0x00047AF1 File Offset: 0x00045CF1
		[DataSourceProperty]
		public BasicTooltipViewModel WallsHint
		{
			get
			{
				return this._wallsHint;
			}
			set
			{
				if (value != this._wallsHint)
				{
					this._wallsHint = value;
					base.OnPropertyChangedWithValue<BasicTooltipViewModel>(value, "WallsHint");
				}
			}
		}

		// Token: 0x17000623 RID: 1571
		// (get) Token: 0x0600125B RID: 4699 RVA: 0x00047B0F File Offset: 0x00045D0F
		// (set) Token: 0x0600125C RID: 4700 RVA: 0x00047B17 File Offset: 0x00045D17
		[DataSourceProperty]
		public string MilitasText
		{
			get
			{
				return this._militasText;
			}
			set
			{
				if (value != this._militasText)
				{
					this._militasText = value;
					base.OnPropertyChangedWithValue<string>(value, "MilitasText");
				}
			}
		}

		// Token: 0x17000624 RID: 1572
		// (get) Token: 0x0600125D RID: 4701 RVA: 0x00047B3A File Offset: 0x00045D3A
		// (set) Token: 0x0600125E RID: 4702 RVA: 0x00047B42 File Offset: 0x00045D42
		[DataSourceProperty]
		public string ProsperityText
		{
			get
			{
				return this._prosperityText;
			}
			set
			{
				if (value != this._prosperityText)
				{
					this._prosperityText = value;
					base.OnPropertyChangedWithValue<string>(value, "ProsperityText");
				}
			}
		}

		// Token: 0x17000625 RID: 1573
		// (get) Token: 0x0600125F RID: 4703 RVA: 0x00047B65 File Offset: 0x00045D65
		// (set) Token: 0x06001260 RID: 4704 RVA: 0x00047B6D File Offset: 0x00045D6D
		[DataSourceProperty]
		public string LoyaltyText
		{
			get
			{
				return this._loyaltyText;
			}
			set
			{
				if (value != this._loyaltyText)
				{
					this._loyaltyText = value;
					base.OnPropertyChangedWithValue<string>(value, "LoyaltyText");
				}
			}
		}

		// Token: 0x17000626 RID: 1574
		// (get) Token: 0x06001261 RID: 4705 RVA: 0x00047B90 File Offset: 0x00045D90
		// (set) Token: 0x06001262 RID: 4706 RVA: 0x00047B98 File Offset: 0x00045D98
		[DataSourceProperty]
		public string SecurityText
		{
			get
			{
				return this._securityText;
			}
			set
			{
				if (value != this._securityText)
				{
					this._securityText = value;
					base.OnPropertyChangedWithValue<string>(value, "SecurityText");
				}
			}
		}

		// Token: 0x17000627 RID: 1575
		// (get) Token: 0x06001263 RID: 4707 RVA: 0x00047BBB File Offset: 0x00045DBB
		// (set) Token: 0x06001264 RID: 4708 RVA: 0x00047BC3 File Offset: 0x00045DC3
		[DataSourceProperty]
		public string WallsText
		{
			get
			{
				return this._wallsText;
			}
			set
			{
				if (value != this._wallsText)
				{
					this._wallsText = value;
					base.OnPropertyChangedWithValue<string>(value, "WallsText");
				}
			}
		}

		// Token: 0x17000628 RID: 1576
		// (get) Token: 0x06001265 RID: 4709 RVA: 0x00047BE6 File Offset: 0x00045DE6
		// (set) Token: 0x06001266 RID: 4710 RVA: 0x00047BEE File Offset: 0x00045DEE
		[DataSourceProperty]
		public string FoodText
		{
			get
			{
				return this._foodText;
			}
			set
			{
				if (value != this._foodText)
				{
					this._foodText = value;
					base.OnPropertyChangedWithValue<string>(value, "FoodText");
				}
			}
		}

		// Token: 0x17000629 RID: 1577
		// (get) Token: 0x06001267 RID: 4711 RVA: 0x00047C11 File Offset: 0x00045E11
		// (set) Token: 0x06001268 RID: 4712 RVA: 0x00047C19 File Offset: 0x00045E19
		[DataSourceProperty]
		public string GarrisonText
		{
			get
			{
				return this._garrisonText;
			}
			set
			{
				if (value != this._garrisonText)
				{
					this._garrisonText = value;
					base.OnPropertyChangedWithValue<string>(value, "GarrisonText");
				}
			}
		}

		// Token: 0x1700062A RID: 1578
		// (get) Token: 0x06001269 RID: 4713 RVA: 0x00047C3C File Offset: 0x00045E3C
		// (set) Token: 0x0600126A RID: 4714 RVA: 0x00047C44 File Offset: 0x00045E44
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

		// Token: 0x1700062B RID: 1579
		// (get) Token: 0x0600126B RID: 4715 RVA: 0x00047C67 File Offset: 0x00045E67
		// (set) Token: 0x0600126C RID: 4716 RVA: 0x00047C6F File Offset: 0x00045E6F
		[DataSourceProperty]
		public string CultureText
		{
			get
			{
				return this._cultureText;
			}
			set
			{
				if (value != this._cultureText)
				{
					this._cultureText = value;
					base.OnPropertyChangedWithValue<string>(value, "CultureText");
				}
			}
		}

		// Token: 0x1700062C RID: 1580
		// (get) Token: 0x0600126D RID: 4717 RVA: 0x00047C92 File Offset: 0x00045E92
		// (set) Token: 0x0600126E RID: 4718 RVA: 0x00047C9A File Offset: 0x00045E9A
		[DataSourceProperty]
		public string OwnerText
		{
			get
			{
				return this._ownerText;
			}
			set
			{
				if (value != this._ownerText)
				{
					this._ownerText = value;
					base.OnPropertyChangedWithValue<string>(value, "OwnerText");
				}
			}
		}

		// Token: 0x1700062D RID: 1581
		// (get) Token: 0x0600126F RID: 4719 RVA: 0x00047CBD File Offset: 0x00045EBD
		// (set) Token: 0x06001270 RID: 4720 RVA: 0x00047CC5 File Offset: 0x00045EC5
		[DataSourceProperty]
		public bool IsVisualTrackerSelected
		{
			get
			{
				return this._isVisualTrackerSelected;
			}
			set
			{
				if (value != this._isVisualTrackerSelected)
				{
					this._isVisualTrackerSelected = value;
					base.OnPropertyChangedWithValue(value, "IsVisualTrackerSelected");
				}
			}
		}

		// Token: 0x0400086E RID: 2158
		private readonly Settlement _settlement;

		// Token: 0x0400086F RID: 2159
		private int _settlementType;

		// Token: 0x04000870 RID: 2160
		private MBBindingList<EncyclopediaHistoryEventVM> _history;

		// Token: 0x04000871 RID: 2161
		private MBBindingList<EncyclopediaSettlementVM> _settlements;

		// Token: 0x04000872 RID: 2162
		private EncyclopediaSettlementVM _boundSettlement;

		// Token: 0x04000873 RID: 2163
		private MBBindingList<HeroVM> _notableCharacters;

		// Token: 0x04000874 RID: 2164
		private EncyclopediaFactionVM _ownerBanner;

		// Token: 0x04000875 RID: 2165
		private HintViewModel _showInMapHint;

		// Token: 0x04000876 RID: 2166
		private BasicTooltipViewModel _militasHint;

		// Token: 0x04000877 RID: 2167
		private BasicTooltipViewModel _prosperityHint;

		// Token: 0x04000878 RID: 2168
		private BasicTooltipViewModel _loyaltyHint;

		// Token: 0x04000879 RID: 2169
		private BasicTooltipViewModel _securityHint;

		// Token: 0x0400087A RID: 2170
		private BasicTooltipViewModel _wallsHint;

		// Token: 0x0400087B RID: 2171
		private BasicTooltipViewModel _garrisonHint;

		// Token: 0x0400087C RID: 2172
		private BasicTooltipViewModel _foodHint;

		// Token: 0x0400087D RID: 2173
		private HeroVM _owner;

		// Token: 0x0400087E RID: 2174
		private string _ownerText;

		// Token: 0x0400087F RID: 2175
		private string _militasText;

		// Token: 0x04000880 RID: 2176
		private string _garrisonText;

		// Token: 0x04000881 RID: 2177
		private string _prosperityText;

		// Token: 0x04000882 RID: 2178
		private string _loyaltyText;

		// Token: 0x04000883 RID: 2179
		private string _securityText;

		// Token: 0x04000884 RID: 2180
		private string _wallsText;

		// Token: 0x04000885 RID: 2181
		private string _foodText;

		// Token: 0x04000886 RID: 2182
		private string _nameText;

		// Token: 0x04000887 RID: 2183
		private string _cultureText;

		// Token: 0x04000888 RID: 2184
		private string _villagesText;

		// Token: 0x04000889 RID: 2185
		private string _notableCharactersText;

		// Token: 0x0400088A RID: 2186
		private string _settlementPath;

		// Token: 0x0400088B RID: 2187
		private string _settlementName;

		// Token: 0x0400088C RID: 2188
		private string _informationText;

		// Token: 0x0400088D RID: 2189
		private string _settlementImageID;

		// Token: 0x0400088E RID: 2190
		private string _boundSettlementText;

		// Token: 0x0400088F RID: 2191
		private string _trackText;

		// Token: 0x04000890 RID: 2192
		private double _settlementCropPosition;

		// Token: 0x04000891 RID: 2193
		private bool _isFortification;

		// Token: 0x04000892 RID: 2194
		private bool _isVisualTrackerSelected;

		// Token: 0x04000893 RID: 2195
		private bool _hasBoundSettlement;

		// Token: 0x04000894 RID: 2196
		private bool _isTrackerButtonHighlightEnabled;

		// Token: 0x020001F6 RID: 502
		private enum SettlementTypes
		{
			// Token: 0x04001041 RID: 4161
			Town,
			// Token: 0x04001042 RID: 4162
			LoneVillage,
			// Token: 0x04001043 RID: 4163
			VillageWithCastle
		}
	}
}
