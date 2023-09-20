using System;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;
using TaleWorlds.Library.Information;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.Map.MapBar
{
	public class MapInfoVM : ViewModel
	{
		public MapInfoVM()
		{
			this.DenarTooltip = CampaignUIHelper.GetDenarTooltip();
			this.HealthHint = new BasicTooltipViewModel(() => CampaignUIHelper.GetPlayerHitpointsTooltip());
			this.InfluenceHint = new BasicTooltipViewModel(() => CampaignUIHelper.GetInfluenceTooltip(Clan.PlayerClan));
			this.AvailableTroopsHint = new BasicTooltipViewModel(() => CampaignUIHelper.GetMainPartyHealthTooltip());
			this.ExtendHint = new HintViewModel(GameTexts.FindText("str_map_extend_bar_hint", null), null);
			this.SpeedHint = new BasicTooltipViewModel(() => CampaignUIHelper.GetPartySpeedTooltip());
			this.ViewDistanceHint = new BasicTooltipViewModel(() => CampaignUIHelper.GetViewDistanceTooltip());
			this.TroopWageHint = new BasicTooltipViewModel(() => CampaignUIHelper.GetPartyWageTooltip());
			this.MoraleHint = new BasicTooltipViewModel(() => CampaignUIHelper.GetPartyMoraleTooltip(MobileParty.MainParty));
			this.DailyConsumptionHint = new BasicTooltipViewModel(() => CampaignUIHelper.GetPartyFoodTooltip(MobileParty.MainParty));
			this._viewDataTracker = Campaign.Current.GetCampaignBehavior<IViewDataTracker>();
			this.IsInfoBarExtended = this._viewDataTracker.GetMapBarExtendedState();
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
			this.UpdatePlayerInfo(true);
		}

		public void Tick()
		{
			this.IsMainHeroSick = Hero.MainHero != null && Hero.IsMainHeroIll;
			Hero mainHero = Hero.MainHero;
			this.IsInfoBarEnabled = mainHero != null && mainHero.IsAlive;
		}

		public void Refresh()
		{
			this.UpdatePlayerInfo(false);
		}

		private void UpdatePlayerInfo(bool updateForced)
		{
			int totalWage = MobileParty.MainParty.TotalWage;
			ExplainedNumber explainedNumber = Campaign.Current.Models.ClanFinanceModel.CalculateClanGoldChange(Clan.PlayerClan, true, false, true);
			this.IsDenarTooltipWarning = (float)Hero.MainHero.Gold + explainedNumber.ResultNumber < 0f;
			this.IsInfluenceTooltipWarning = Hero.MainHero.Clan.Influence < -100f;
			this.IsMoraleTooltipWarning = MobileParty.MainParty.Morale < (float)Campaign.Current.Models.PartyDesertionModel.GetMoraleThresholdForTroopDesertion(MobileParty.MainParty);
			int numDaysForFoodToLast = MobileParty.MainParty.GetNumDaysForFoodToLast();
			this.IsDailyConsumptionTooltipWarning = numDaysForFoodToLast < 1;
			this.IsAvailableTroopsTooltipWarning = PartyBase.MainParty.PartySizeLimit < PartyBase.MainParty.NumberOfAllMembers || PartyBase.MainParty.PrisonerSizeLimit < PartyBase.MainParty.NumberOfPrisoners;
			this.IsHealthTooltipWarning = Hero.MainHero.IsWounded;
			if (this.Denars != Hero.MainHero.Gold || updateForced)
			{
				this.Denars = Hero.MainHero.Gold;
				this.DenarsWithAbbrText = CampaignUIHelper.GetAbbreviatedValueTextFromValue(this.Denars);
			}
			if (this.Influence != (int)Hero.MainHero.Clan.Influence || updateForced)
			{
				this.Influence = (int)Hero.MainHero.Clan.Influence;
				this.InfluenceWithAbbrText = CampaignUIHelper.GetAbbreviatedValueTextFromValue(this.Influence);
			}
			this.Morale = (int)MobileParty.MainParty.Morale;
			this.TotalFood = (int)((MobileParty.MainParty.Food > 0f) ? MobileParty.MainParty.Food : 0f);
			this.TotalTroops = PartyBase.MainParty.MemberRoster.TotalManCount;
			this.AvailableTroopsText = CampaignUIHelper.GetPartyNameplateText(PartyBase.MainParty);
			int num = (int)MathF.Clamp((float)(Hero.MainHero.HitPoints * 100 / CharacterObject.PlayerCharacter.MaxHitPoints()), 1f, 100f);
			if (this.Health != num || updateForced)
			{
				this.Health = num;
				GameTexts.SetVariable("NUMBER", this.Health);
				this.HealthTextWithPercentage = GameTexts.FindText("str_NUMBER_percent", null).ToString();
			}
			float num2 = MathF.Round(MobileParty.MainParty.Morale, 1);
			if (this._latestMorale != num2 || updateForced)
			{
				this._latestMorale = num2;
				MBTextManager.SetTextVariable("BASE_EFFECT", num2.ToString("0.0"), false);
			}
			float num3 = (MobileParty.MainParty.CurrentNavigationFace.IsValid() ? MobileParty.MainParty.Speed : 0f);
			if (this._latestSpeed != num3 || updateForced)
			{
				this._latestSpeed = num3;
				this.Speed = CampaignUIHelper.FloatToString(num3);
			}
			float seeingRange = MobileParty.MainParty.SeeingRange;
			if (this._latestSeeingRange != seeingRange || updateForced)
			{
				this._latestSeeingRange = seeingRange;
				this.ViewDistance = CampaignUIHelper.FloatToString(seeingRange);
			}
			if (this._latestTotalWage != totalWage || updateForced)
			{
				this._latestTotalWage = totalWage;
				this.TroopWage = totalWage.ToString();
			}
		}

		[DataSourceProperty]
		public bool IsHealthTooltipWarning
		{
			get
			{
				return this._isHealthTooltipWarning;
			}
			set
			{
				if (value != this._isHealthTooltipWarning)
				{
					this._isHealthTooltipWarning = value;
					base.OnPropertyChangedWithValue(value, "IsHealthTooltipWarning");
				}
			}
		}

		[DataSourceProperty]
		public bool IsMainHeroSick
		{
			get
			{
				return this._isMainHeroSick;
			}
			set
			{
				if (value != this._isMainHeroSick)
				{
					this._isMainHeroSick = value;
					base.OnPropertyChangedWithValue(value, "IsMainHeroSick");
				}
			}
		}

		[DataSourceProperty]
		public HintViewModel ExtendHint
		{
			get
			{
				return this._extendHint;
			}
			set
			{
				if (value != this._extendHint)
				{
					this._extendHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "ExtendHint");
				}
			}
		}

		[DataSourceProperty]
		public TooltipTriggerVM DenarTooltip
		{
			get
			{
				return this._denarTooltip;
			}
			set
			{
				if (value != this._denarTooltip)
				{
					this._denarTooltip = value;
					base.OnPropertyChangedWithValue<TooltipTriggerVM>(value, "DenarTooltip");
				}
			}
		}

		[DataSourceProperty]
		public BasicTooltipViewModel InfluenceHint
		{
			get
			{
				return this._influenceHint;
			}
			set
			{
				if (value != this._influenceHint)
				{
					this._influenceHint = value;
					base.OnPropertyChangedWithValue<BasicTooltipViewModel>(value, "InfluenceHint");
				}
			}
		}

		[DataSourceProperty]
		public BasicTooltipViewModel AvailableTroopsHint
		{
			get
			{
				return this._availableTroopsHint;
			}
			set
			{
				if (value != this._availableTroopsHint)
				{
					this._availableTroopsHint = value;
					base.OnPropertyChangedWithValue<BasicTooltipViewModel>(value, "AvailableTroopsHint");
				}
			}
		}

		[DataSourceProperty]
		public BasicTooltipViewModel HealthHint
		{
			get
			{
				return this._healthHint;
			}
			set
			{
				if (value != this._healthHint)
				{
					this._healthHint = value;
					base.OnPropertyChangedWithValue<BasicTooltipViewModel>(value, "HealthHint");
				}
			}
		}

		[DataSourceProperty]
		public BasicTooltipViewModel DailyConsumptionHint
		{
			get
			{
				return this._dailyConsumptionHint;
			}
			set
			{
				if (value != this._dailyConsumptionHint)
				{
					this._dailyConsumptionHint = value;
					base.OnPropertyChangedWithValue<BasicTooltipViewModel>(value, "DailyConsumptionHint");
				}
			}
		}

		[DataSourceProperty]
		public BasicTooltipViewModel MoraleHint
		{
			get
			{
				return this._moraleHint;
			}
			set
			{
				if (value != this._moraleHint)
				{
					this._moraleHint = value;
					base.OnPropertyChangedWithValue<BasicTooltipViewModel>(value, "MoraleHint");
				}
			}
		}

		[DataSourceProperty]
		public BasicTooltipViewModel SpeedHint
		{
			get
			{
				return this._speedHint;
			}
			set
			{
				if (value != this._speedHint)
				{
					this._speedHint = value;
					base.OnPropertyChangedWithValue<BasicTooltipViewModel>(value, "SpeedHint");
				}
			}
		}

		[DataSourceProperty]
		public BasicTooltipViewModel ViewDistanceHint
		{
			get
			{
				return this._viewDistanceHint;
			}
			set
			{
				if (value != this._viewDistanceHint)
				{
					this._viewDistanceHint = value;
					base.OnPropertyChangedWithValue<BasicTooltipViewModel>(value, "ViewDistanceHint");
				}
			}
		}

		[DataSourceProperty]
		public BasicTooltipViewModel TrainingFactorHint
		{
			get
			{
				return this._trainingFactorHint;
			}
			set
			{
				if (value != this._trainingFactorHint)
				{
					this._trainingFactorHint = value;
					base.OnPropertyChangedWithValue<BasicTooltipViewModel>(value, "TrainingFactorHint");
				}
			}
		}

		[DataSourceProperty]
		public BasicTooltipViewModel TroopWageHint
		{
			get
			{
				return this._troopWageHint;
			}
			set
			{
				if (value != this._troopWageHint)
				{
					this._troopWageHint = value;
					base.OnPropertyChangedWithValue<BasicTooltipViewModel>(value, "TroopWageHint");
				}
			}
		}

		[DataSourceProperty]
		public bool IsDenarTooltipWarning
		{
			get
			{
				return this._isDenarTooltipWarning;
			}
			set
			{
				if (value != this._isDenarTooltipWarning)
				{
					this._isDenarTooltipWarning = value;
					base.OnPropertyChangedWithValue(value, "IsDenarTooltipWarning");
				}
			}
		}

		[DataSourceProperty]
		public bool IsInfluenceTooltipWarning
		{
			get
			{
				return this._isInfluenceTooltipWarning;
			}
			set
			{
				if (value != this._isInfluenceTooltipWarning)
				{
					this._isInfluenceTooltipWarning = value;
					base.OnPropertyChangedWithValue(value, "IsInfluenceTooltipWarning");
				}
			}
		}

		[DataSourceProperty]
		public bool IsMoraleTooltipWarning
		{
			get
			{
				return this._isMoraleTooltipWarning;
			}
			set
			{
				if (value != this._isMoraleTooltipWarning)
				{
					this._isMoraleTooltipWarning = value;
					base.OnPropertyChangedWithValue(value, "IsMoraleTooltipWarning");
				}
			}
		}

		[DataSourceProperty]
		public bool IsDailyConsumptionTooltipWarning
		{
			get
			{
				return this._isDailyConsumptionTooltipWarning;
			}
			set
			{
				if (value != this._isDailyConsumptionTooltipWarning)
				{
					this._isDailyConsumptionTooltipWarning = value;
					base.OnPropertyChangedWithValue(value, "IsDailyConsumptionTooltipWarning");
				}
			}
		}

		[DataSourceProperty]
		public bool IsAvailableTroopsTooltipWarning
		{
			get
			{
				return this._isAvailableTroopsTooltipWarning;
			}
			set
			{
				if (value != this._isAvailableTroopsTooltipWarning)
				{
					this._isAvailableTroopsTooltipWarning = value;
					base.OnPropertyChangedWithValue(value, "IsAvailableTroopsTooltipWarning");
				}
			}
		}

		[DataSourceProperty]
		public string DenarsWithAbbrText
		{
			get
			{
				return this._denarsWithAbbrText;
			}
			set
			{
				if (value != this._denarsWithAbbrText)
				{
					this._denarsWithAbbrText = value;
					base.OnPropertyChangedWithValue<string>(value, "DenarsWithAbbrText");
				}
			}
		}

		[DataSourceProperty]
		public int Denars
		{
			get
			{
				return this._denars;
			}
			set
			{
				if (value != this._denars)
				{
					this._denars = value;
					base.OnPropertyChangedWithValue(value, "Denars");
				}
			}
		}

		[DataSourceProperty]
		public int Influence
		{
			get
			{
				return this._influence;
			}
			set
			{
				if (value != this._influence)
				{
					this._influence = value;
					base.OnPropertyChangedWithValue(value, "Influence");
				}
			}
		}

		[DataSourceProperty]
		public string InfluenceWithAbbrText
		{
			get
			{
				return this._influenceWithAbbrText;
			}
			set
			{
				if (value != this._influenceWithAbbrText)
				{
					this._influenceWithAbbrText = value;
					base.OnPropertyChangedWithValue<string>(value, "InfluenceWithAbbrText");
				}
			}
		}

		[DataSourceProperty]
		public int Morale
		{
			get
			{
				return this._morale;
			}
			set
			{
				if (value != this._morale)
				{
					this._morale = value;
					base.OnPropertyChangedWithValue(value, "Morale");
				}
			}
		}

		[DataSourceProperty]
		public int TotalFood
		{
			get
			{
				return this._totalFood;
			}
			set
			{
				if (value != this._totalFood)
				{
					this._totalFood = value;
					base.OnPropertyChangedWithValue(value, "TotalFood");
				}
			}
		}

		[DataSourceProperty]
		public int Health
		{
			get
			{
				return this._health;
			}
			set
			{
				if (value != this._health)
				{
					this._health = value;
					base.OnPropertyChangedWithValue(value, "Health");
				}
			}
		}

		[DataSourceProperty]
		public string HealthTextWithPercentage
		{
			get
			{
				return this._healthTextWithPercentage;
			}
			set
			{
				if (value != this._healthTextWithPercentage)
				{
					this._healthTextWithPercentage = value;
					base.OnPropertyChangedWithValue<string>(value, "HealthTextWithPercentage");
				}
			}
		}

		[DataSourceProperty]
		public string AvailableTroopsText
		{
			get
			{
				return this._availableTroopsText;
			}
			set
			{
				if (value != this._availableTroopsText)
				{
					this._availableTroopsText = value;
					base.OnPropertyChangedWithValue<string>(value, "AvailableTroopsText");
				}
			}
		}

		[DataSourceProperty]
		public int TotalTroops
		{
			get
			{
				return this._totalTroops;
			}
			set
			{
				if (value != this._totalTroops)
				{
					this._totalTroops = value;
					base.OnPropertyChangedWithValue(value, "TotalTroops");
				}
			}
		}

		[DataSourceProperty]
		public string Speed
		{
			get
			{
				return this._speed;
			}
			set
			{
				if (value != this._speed)
				{
					this._speed = value;
					base.OnPropertyChangedWithValue<string>(value, "Speed");
				}
			}
		}

		[DataSourceProperty]
		public string ViewDistance
		{
			get
			{
				return this._viewDistance;
			}
			set
			{
				if (value != this._viewDistance)
				{
					this._viewDistance = value;
					base.OnPropertyChangedWithValue<string>(value, "ViewDistance");
				}
			}
		}

		[DataSourceProperty]
		public string TrainingFactor
		{
			get
			{
				return this._trainingFactor;
			}
			set
			{
				if (value != this._trainingFactor)
				{
					this._trainingFactor = value;
					base.OnPropertyChangedWithValue<string>(value, "TrainingFactor");
				}
			}
		}

		[DataSourceProperty]
		public string TroopWage
		{
			get
			{
				return this._troopWage;
			}
			set
			{
				if (value != this._troopWage)
				{
					this._troopWage = value;
					base.OnPropertyChangedWithValue<string>(value, "TroopWage");
				}
			}
		}

		[DataSourceProperty]
		public bool IsInfoBarExtended
		{
			get
			{
				return this._isInfoBarExtended;
			}
			set
			{
				if (value != this._isInfoBarExtended)
				{
					this._isInfoBarExtended = value;
					this._viewDataTracker.SetMapBarExtendedState(value);
					base.OnPropertyChangedWithValue(value, "IsInfoBarExtended");
				}
			}
		}

		[DataSourceProperty]
		public bool IsInfoBarEnabled
		{
			get
			{
				return this._isInfoBarEnabled;
			}
			set
			{
				if (value != this._isInfoBarEnabled)
				{
					this._isInfoBarEnabled = value;
					base.OnPropertyChangedWithValue(value, "IsInfoBarEnabled");
				}
			}
		}

		private int _latestTotalWage = -1;

		private float _latestSeeingRange = -1f;

		private float _latestSpeed = -1f;

		private float _latestMorale = -1f;

		private IViewDataTracker _viewDataTracker;

		private string _speed;

		private string _viewDistance;

		private string _trainingFactor;

		private string _troopWage;

		private string _healthTextWithPercentage;

		private string _denarsWithAbbrText = "";

		private string _influenceWithAbbrText = "";

		private string _availableTroopsText;

		private int _denars = -1;

		private int _influence = -1;

		private int _morale = -1;

		private int _totalFood;

		private int _health;

		private int _totalTroops;

		private bool _isInfoBarExtended;

		private bool _isInfoBarEnabled;

		private bool _isDenarTooltipWarning;

		private bool _isHealthTooltipWarning;

		private bool _isInfluenceTooltipWarning;

		private bool _isMoraleTooltipWarning;

		private bool _isDailyConsumptionTooltipWarning;

		private bool _isAvailableTroopsTooltipWarning;

		private bool _isMainHeroSick;

		private TooltipTriggerVM _denarTooltip;

		private BasicTooltipViewModel _influenceHint;

		private BasicTooltipViewModel _availableTroopsHint;

		private BasicTooltipViewModel _healthHint;

		private BasicTooltipViewModel _dailyConsumptionHint;

		private BasicTooltipViewModel _moraleHint;

		private BasicTooltipViewModel _trainingFactorHint;

		private BasicTooltipViewModel _troopWageHint;

		private BasicTooltipViewModel _speedHint;

		private BasicTooltipViewModel _viewDistanceHint;

		private HintViewModel _extendHint;
	}
}
