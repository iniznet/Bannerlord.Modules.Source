using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem.Election;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;
using TaleWorlds.Library.EventSystem;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.KingdomManagement.Decisions
{
	public class DecisionOptionVM : ViewModel
	{
		public DecisionOutcome Option { get; private set; }

		public KingdomDecision Decision { get; private set; }

		public DecisionOptionVM(DecisionOutcome option, KingdomDecision decision, KingdomElection kingdomDecisionMaker, Action<DecisionOptionVM> onSelect, Action<DecisionOptionVM> onSupportStrengthChange)
		{
			this._onSelect = onSelect;
			this._onSupportStrengthChange = onSupportStrengthChange;
			this._kingdomDecisionMaker = kingdomDecisionMaker;
			this.Decision = decision;
			this.CurrentSupportWeight = Supporter.SupportWeights.Choose;
			this.OptionHint = new HintViewModel();
			this.IsPlayerSupporter = !this._kingdomDecisionMaker.IsPlayerChooser;
			this.SupportersOfThisOption = new MBBindingList<DecisionSupporterVM>();
			this.Option = option;
			if (option != null)
			{
				Clan sponsorClan = option.SponsorClan;
				if (((sponsorClan != null) ? sponsorClan.Leader : null) != null)
				{
					this.Sponsor = new HeroVM(option.SponsorClan.Leader, false);
				}
				List<Supporter> supporterList = option.SupporterList;
				if (supporterList != null && supporterList.Count > 0)
				{
					foreach (Supporter supporter in option.SupporterList)
					{
						if (supporter.SupportWeight > Supporter.SupportWeights.StayNeutral)
						{
							if (supporter.Clan != option.SponsorClan)
							{
								this.SupportersOfThisOption.Add(new DecisionSupporterVM(supporter.Name, supporter.ImagePath, supporter.Clan, supporter.SupportWeight));
							}
							else
							{
								this.SponsorWeightImagePath = DecisionSupporterVM.GetSupporterWeightImagePath(supporter.SupportWeight);
							}
						}
					}
				}
				this.IsOptionForAbstain = false;
			}
			else
			{
				this.IsOptionForAbstain = true;
			}
			this.RefreshValues();
			this.RefreshSupportOptionEnabled();
			this.RefreshCanChooseOption();
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
			if (this.Option != null)
			{
				this.Name = this.Option.GetDecisionTitle().ToString();
				this.Description = this.Option.GetDecisionDescription().ToString();
			}
			else
			{
				this.Name = GameTexts.FindText("str_abstain", null).ToString();
				this.Description = GameTexts.FindText("str_kingdom_decision_abstain_desc", null).ToString();
			}
			MBBindingList<DecisionSupporterVM> supportersOfThisOption = this.SupportersOfThisOption;
			if (supportersOfThisOption == null)
			{
				return;
			}
			supportersOfThisOption.ApplyActionOnAllItems(delegate(DecisionSupporterVM x)
			{
				x.RefreshValues();
			});
		}

		private void ExecuteShowSupporterTooltip()
		{
			DecisionOutcome option = this.Option;
			if (option != null && option.SupporterList.Count > 0)
			{
				List<TooltipProperty> list = new List<TooltipProperty>();
				this._kingdomDecisionMaker.DetermineOfficialSupport();
				foreach (Supporter supporter in this.Option.SupporterList)
				{
					if (supporter.SupportWeight > Supporter.SupportWeights.StayNeutral && !supporter.IsPlayer)
					{
						int influenceCost = this.Decision.GetInfluenceCost(this.Option, supporter.Clan, supporter.SupportWeight);
						GameTexts.SetVariable("AMOUNT", influenceCost);
						GameTexts.SetVariable("INFLUENCE_ICON", "{=!}<img src=\"General\\Icons\\Influence@2x\" extend=\"7\">");
						list.Add(new TooltipProperty(supporter.Name.ToString(), GameTexts.FindText("str_amount_with_influence_icon", null).ToString(), 0, false, TooltipProperty.TooltipPropertyFlags.None));
					}
				}
				InformationManager.ShowTooltip(typeof(List<TooltipProperty>), new object[] { list });
			}
		}

		private void ExecuteHideSupporterTooltip()
		{
			MBInformationManager.HideInformations();
		}

		private void RefreshSupportOptionEnabled()
		{
			int influenceCostOfOutcome = this._kingdomDecisionMaker.GetInfluenceCostOfOutcome(this.Option, Clan.PlayerClan, Supporter.SupportWeights.SlightlyFavor);
			int influenceCostOfOutcome2 = this._kingdomDecisionMaker.GetInfluenceCostOfOutcome(this.Option, Clan.PlayerClan, Supporter.SupportWeights.StronglyFavor);
			int influenceCostOfOutcome3 = this._kingdomDecisionMaker.GetInfluenceCostOfOutcome(this.Option, Clan.PlayerClan, Supporter.SupportWeights.FullyPush);
			this.SupportOption1Text = influenceCostOfOutcome.ToString();
			this.SupportOption2Text = influenceCostOfOutcome2.ToString();
			this.SupportOption3Text = influenceCostOfOutcome3.ToString();
			this.IsSupportOption1Enabled = (float)influenceCostOfOutcome <= Clan.PlayerClan.Influence && !this.IsOptionForAbstain;
			this.IsSupportOption2Enabled = (float)influenceCostOfOutcome2 <= Clan.PlayerClan.Influence && !this.IsOptionForAbstain;
			this.IsSupportOption3Enabled = (float)influenceCostOfOutcome3 <= Clan.PlayerClan.Influence && !this.IsOptionForAbstain;
		}

		private void OnSupportStrengthChange(int index)
		{
			if (!this.IsOptionForAbstain)
			{
				switch (index)
				{
				case 0:
					this.CurrentSupportWeight = Supporter.SupportWeights.SlightlyFavor;
					break;
				case 1:
					this.CurrentSupportWeight = Supporter.SupportWeights.StronglyFavor;
					break;
				case 2:
					this.CurrentSupportWeight = Supporter.SupportWeights.FullyPush;
					break;
				}
				this._kingdomDecisionMaker.OnPlayerSupport((!this.IsOptionForAbstain) ? this.Option : null, this.CurrentSupportWeight);
				this._onSupportStrengthChange(this);
			}
		}

		public void AfterKingChooseOutcome()
		{
			this._hasKingChoosen = true;
			this.RefreshCanChooseOption();
		}

		private void RefreshCanChooseOption()
		{
			if (this._hasKingChoosen)
			{
				this.CanBeChosen = false;
				return;
			}
			if (this.IsOptionForAbstain)
			{
				this.CanBeChosen = true;
				return;
			}
			if (this.IsPlayerSupporter)
			{
				this.CanBeChosen = (float)this._kingdomDecisionMaker.GetInfluenceCostOfOutcome(this.Option, Clan.PlayerClan, Supporter.SupportWeights.SlightlyFavor) <= Clan.PlayerClan.Influence;
			}
			else
			{
				int influenceCostOfOutcome = this._kingdomDecisionMaker.GetInfluenceCostOfOutcome(this.Option, Clan.PlayerClan, Supporter.SupportWeights.Choose);
				this.CanBeChosen = (float)influenceCostOfOutcome <= Clan.PlayerClan.Influence || influenceCostOfOutcome == 0;
			}
			this.OptionHint.HintText = (this.CanBeChosen ? TextObject.Empty : new TextObject("{=Xmw93W6a}Not Enough Influence", null));
		}

		private void ExecuteSelection()
		{
			this._onSelect(this);
			Game game = Game.Current;
			if (game == null)
			{
				return;
			}
			EventManager eventManager = game.EventManager;
			if (eventManager == null)
			{
				return;
			}
			eventManager.TriggerEvent<PlayerSelectedAKingdomDecisionOptionEvent>(new PlayerSelectedAKingdomDecisionOptionEvent(this.Option));
		}

		[DataSourceProperty]
		public HintViewModel OptionHint
		{
			get
			{
				return this._optionHint;
			}
			set
			{
				if (value != this._optionHint)
				{
					this._optionHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "OptionHint");
				}
			}
		}

		[DataSourceProperty]
		public MBBindingList<DecisionSupporterVM> SupportersOfThisOption
		{
			get
			{
				return this._supportersOfThisOption;
			}
			set
			{
				if (value != this._supportersOfThisOption)
				{
					this._supportersOfThisOption = value;
					base.OnPropertyChangedWithValue<MBBindingList<DecisionSupporterVM>>(value, "SupportersOfThisOption");
				}
			}
		}

		[DataSourceProperty]
		public HeroVM Sponsor
		{
			get
			{
				return this._sponsor;
			}
			set
			{
				if (value != this._sponsor)
				{
					this._sponsor = value;
					base.OnPropertyChangedWithValue<HeroVM>(value, "Sponsor");
				}
			}
		}

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

		[DataSourceProperty]
		public string SponsorWeightImagePath
		{
			get
			{
				return this._sponsorWeightImagePath;
			}
			set
			{
				if (value != this._sponsorWeightImagePath)
				{
					this._sponsorWeightImagePath = value;
					base.OnPropertyChangedWithValue<string>(value, "SponsorWeightImagePath");
				}
			}
		}

		[DataSourceProperty]
		public bool CanBeChosen
		{
			get
			{
				return this._canBeChosen;
			}
			set
			{
				if (value != this._canBeChosen)
				{
					this._canBeChosen = value;
					base.OnPropertyChangedWithValue(value, "CanBeChosen");
				}
			}
		}

		[DataSourceProperty]
		public bool IsKingsOutcome
		{
			get
			{
				return this._isKingsOutcome;
			}
			set
			{
				if (value != this._isKingsOutcome)
				{
					this._isKingsOutcome = value;
					base.OnPropertyChangedWithValue(value, "IsKingsOutcome");
				}
			}
		}

		[DataSourceProperty]
		public bool IsPlayerSupporter
		{
			get
			{
				return this._isPlayerSupporter;
			}
			set
			{
				if (value != this._isPlayerSupporter)
				{
					this._isPlayerSupporter = value;
					base.OnPropertyChangedWithValue(value, "IsPlayerSupporter");
				}
			}
		}

		[DataSourceProperty]
		public bool IsHighlightEnabled
		{
			get
			{
				return this._isHighlightEnabled;
			}
			set
			{
				if (value != this._isHighlightEnabled)
				{
					this._isHighlightEnabled = value;
					base.OnPropertyChangedWithValue(value, "IsHighlightEnabled");
				}
			}
		}

		[DataSourceProperty]
		public int WinPercentage
		{
			get
			{
				return this._winPercentage;
			}
			set
			{
				if (value != this._winPercentage)
				{
					this._winPercentage = value;
					base.OnPropertyChangedWithValue(value, "WinPercentage");
					GameTexts.SetVariable("NUMBER", value);
					this.WinPercentageStr = GameTexts.FindText("str_NUMBER_percent", null).ToString();
				}
			}
		}

		[DataSourceProperty]
		public string WinPercentageStr
		{
			get
			{
				return this._winPercentageStr;
			}
			set
			{
				if (value != this._winPercentageStr)
				{
					this._winPercentageStr = value;
					base.OnPropertyChangedWithValue<string>(value, "WinPercentageStr");
				}
			}
		}

		[DataSourceProperty]
		public string Description
		{
			get
			{
				return this._description;
			}
			set
			{
				if (value != this._description)
				{
					this._description = value;
					base.OnPropertyChangedWithValue<string>(value, "Description");
				}
			}
		}

		[DataSourceProperty]
		public int InitialPercentage
		{
			get
			{
				return this._initialPercentage;
			}
			set
			{
				if (value != this._initialPercentage)
				{
					this._initialPercentage = value;
					base.OnPropertyChangedWithValue(value, "InitialPercentage");
				}
			}
		}

		[DataSourceProperty]
		public int InfluenceCost
		{
			get
			{
				return this._influenceCost;
			}
			set
			{
				if (value != this._influenceCost)
				{
					this._influenceCost = value;
					base.OnPropertyChangedWithValue(value, "InfluenceCost");
				}
			}
		}

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

		[DataSourceProperty]
		public bool IsOptionForAbstain
		{
			get
			{
				return this._isOptionForAbstain;
			}
			set
			{
				if (value != this._isOptionForAbstain)
				{
					this._isOptionForAbstain = value;
					base.OnPropertyChangedWithValue(value, "IsOptionForAbstain");
				}
			}
		}

		[DataSourceProperty]
		public Supporter.SupportWeights CurrentSupportWeight
		{
			get
			{
				return this._currentSupportWeight;
			}
			set
			{
				if (value != this._currentSupportWeight)
				{
					this._currentSupportWeight = value;
					base.OnPropertyChanged("CurrentSupportWeight");
					this.CurrentSupportWeightIndex = (int)value;
				}
			}
		}

		[DataSourceProperty]
		public int CurrentSupportWeightIndex
		{
			get
			{
				return this._currentSupportWeightIndex;
			}
			set
			{
				if (value != this._currentSupportWeightIndex)
				{
					this._currentSupportWeightIndex = value;
					base.OnPropertyChangedWithValue(value, "CurrentSupportWeightIndex");
				}
			}
		}

		[DataSourceProperty]
		public string SupportOption1Text
		{
			get
			{
				return this._supportOption1Text;
			}
			set
			{
				if (value != this._supportOption1Text)
				{
					this._supportOption1Text = value;
					base.OnPropertyChangedWithValue<string>(value, "SupportOption1Text");
				}
			}
		}

		[DataSourceProperty]
		public string SupportOption2Text
		{
			get
			{
				return this._supportOption2Text;
			}
			set
			{
				if (value != this._supportOption2Text)
				{
					this._supportOption2Text = value;
					base.OnPropertyChangedWithValue<string>(value, "SupportOption2Text");
				}
			}
		}

		[DataSourceProperty]
		public string SupportOption3Text
		{
			get
			{
				return this._supportOption3Text;
			}
			set
			{
				if (value != this._supportOption3Text)
				{
					this._supportOption3Text = value;
					base.OnPropertyChangedWithValue<string>(value, "SupportOption3Text");
				}
			}
		}

		[DataSourceProperty]
		public bool IsSupportOption1Enabled
		{
			get
			{
				return this._isSupportOption1Enabled;
			}
			set
			{
				if (value != this._isSupportOption1Enabled)
				{
					this._isSupportOption1Enabled = value;
					base.OnPropertyChangedWithValue(value, "IsSupportOption1Enabled");
				}
			}
		}

		[DataSourceProperty]
		public bool IsSupportOption2Enabled
		{
			get
			{
				return this._isSupportOption2Enabled;
			}
			set
			{
				if (value != this._isSupportOption2Enabled)
				{
					this._isSupportOption2Enabled = value;
					base.OnPropertyChangedWithValue(value, "IsSupportOption2Enabled");
				}
			}
		}

		[DataSourceProperty]
		public bool IsSupportOption3Enabled
		{
			get
			{
				return this._isSupportOption3Enabled;
			}
			set
			{
				if (value != this._isSupportOption3Enabled)
				{
					this._isSupportOption3Enabled = value;
					base.OnPropertyChangedWithValue(value, "IsSupportOption3Enabled");
				}
			}
		}

		private readonly Action<DecisionOptionVM> _onSelect;

		private readonly Action<DecisionOptionVM> _onSupportStrengthChange;

		private readonly KingdomElection _kingdomDecisionMaker;

		private bool _hasKingChoosen;

		private MBBindingList<DecisionSupporterVM> _supportersOfThisOption;

		private HeroVM _sponsor;

		private bool _isOptionForAbstain;

		private bool _isPlayerSupporter;

		private bool _isSelected;

		private bool _canBeChosen;

		private bool _isKingsOutcome;

		private bool _isHighlightEnabled;

		private int _winPercentage = -1;

		private int _influenceCost;

		private int _initialPercentage = -99;

		private int _currentSupportWeightIndex;

		private string _name;

		private string _description;

		private string _winPercentageStr;

		private string _sponsorWeightImagePath;

		private Supporter.SupportWeights _currentSupportWeight;

		private string _supportOption1Text;

		private bool _isSupportOption1Enabled;

		private string _supportOption2Text;

		private bool _isSupportOption2Enabled;

		private string _supportOption3Text;

		private bool _isSupportOption3Enabled;

		private HintViewModel _optionHint;
	}
}
