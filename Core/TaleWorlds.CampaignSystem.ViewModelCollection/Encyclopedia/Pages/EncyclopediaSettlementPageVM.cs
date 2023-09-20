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
	[EncyclopediaViewModel(typeof(Settlement))]
	public class EncyclopediaSettlementPageVM : EncyclopediaContentPageVM
	{
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

		public override string GetName()
		{
			return this._settlement.Name.ToString();
		}

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

		public override string GetNavigationBarURL()
		{
			return HyperlinkTexts.GetGenericHyperlinkText("Home", GameTexts.FindText("str_encyclopedia_home", null).ToString()) + " \\ " + HyperlinkTexts.GetGenericHyperlinkText("ListPage-Settlements", GameTexts.FindText("str_encyclopedia_settlements", null).ToString()) + " \\ " + this.GetName();
		}

		public void ExecuteBoundSettlementLink()
		{
			if (this.HasBoundSettlement)
			{
				Campaign.Current.EncyclopediaManager.GoToLink(this._settlement.Village.Bound.EncyclopediaLink);
			}
		}

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

		private void OnTutorialNotificationElementIDChange(TutorialNotificationElementChangeEvent evnt)
		{
			this.IsTrackerButtonHighlightEnabled = evnt.NewNotificationElementID == "EncyclopediaItemTrackButton";
		}

		public override void OnFinalize()
		{
			base.OnFinalize();
			Game.Current.EventManager.UnregisterEvent<TutorialNotificationElementChangeEvent>(new Action<TutorialNotificationElementChangeEvent>(this.OnTutorialNotificationElementIDChange));
		}

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

		private readonly Settlement _settlement;

		private int _settlementType;

		private MBBindingList<EncyclopediaHistoryEventVM> _history;

		private MBBindingList<EncyclopediaSettlementVM> _settlements;

		private EncyclopediaSettlementVM _boundSettlement;

		private MBBindingList<HeroVM> _notableCharacters;

		private EncyclopediaFactionVM _ownerBanner;

		private HintViewModel _showInMapHint;

		private BasicTooltipViewModel _militasHint;

		private BasicTooltipViewModel _prosperityHint;

		private BasicTooltipViewModel _loyaltyHint;

		private BasicTooltipViewModel _securityHint;

		private BasicTooltipViewModel _wallsHint;

		private BasicTooltipViewModel _garrisonHint;

		private BasicTooltipViewModel _foodHint;

		private HeroVM _owner;

		private string _ownerText;

		private string _militasText;

		private string _garrisonText;

		private string _prosperityText;

		private string _loyaltyText;

		private string _securityText;

		private string _wallsText;

		private string _foodText;

		private string _nameText;

		private string _cultureText;

		private string _villagesText;

		private string _notableCharactersText;

		private string _settlementPath;

		private string _settlementName;

		private string _informationText;

		private string _settlementImageID;

		private string _boundSettlementText;

		private string _trackText;

		private double _settlementCropPosition;

		private bool _isFortification;

		private bool _isVisualTrackerSelected;

		private bool _hasBoundSettlement;

		private bool _isTrackerButtonHighlightEnabled;

		private enum SettlementTypes
		{
			Town,
			LoneVillage,
			VillageWithCastle
		}
	}
}
