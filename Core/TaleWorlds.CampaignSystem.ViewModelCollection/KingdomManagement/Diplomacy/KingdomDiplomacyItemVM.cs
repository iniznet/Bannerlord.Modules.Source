using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.KingdomManagement.Diplomacy
{
	public abstract class KingdomDiplomacyItemVM : KingdomItemVM
	{
		protected KingdomDiplomacyItemVM(IFaction faction1, IFaction faction2)
		{
			this._playerKingdom = Hero.MainHero.MapFaction;
			if (faction1 == this._playerKingdom || faction2 == this._playerKingdom)
			{
				this.Faction1 = this._playerKingdom;
				this.Faction2 = ((faction1 != this._playerKingdom) ? faction1 : faction2);
			}
			else
			{
				this.Faction1 = faction1;
				this.Faction2 = faction2;
			}
			this._faction1Color = Color.FromUint(this.Faction1.Color).ToString();
			this._faction2Color = Color.FromUint(this.Faction2.Color).ToString();
			this.Stats = new MBBindingList<KingdomWarComparableStatVM>();
			this.PopulateSettlements();
		}

		protected virtual void UpdateDiplomacyProperties()
		{
			this.Stats.Clear();
			this.Faction1Visual = new ImageIdentifierVM(BannerCode.CreateFrom(this.Faction1.Banner), true);
			this.Faction2Visual = new ImageIdentifierVM(BannerCode.CreateFrom(this.Faction2.Banner), true);
			int dailyTributePaid = this._playerKingdom.GetStanceWith(this.Faction2).GetDailyTributePaid(this._playerKingdom);
			TextObject textObject = new TextObject("{=SDhQWonF}Paying {DENAR}{GOLD_ICON} as tribute per day.", null);
			textObject.SetTextVariable("DENAR", MathF.Abs(dailyTributePaid));
			this.Faction1TributeText = ((dailyTributePaid > 0) ? textObject.ToString() : string.Empty);
			this.Faction2TributeText = ((dailyTributePaid < 0) ? textObject.ToString() : string.Empty);
			this.Faction1Name = this.Faction1.Name.ToString();
			this.Faction2Name = this.Faction2.Name.ToString();
			TextObject textObject2 = new TextObject("{=OyyJSyIX}{FACTION_1} is paying {DENAR}{GOLD_ICON} as tribute to {FACTION_2}", null);
			TextObject textObject3 = textObject2.CopyTextObject();
			this.Faction1TributeHint = ((dailyTributePaid > 0) ? new HintViewModel(textObject2.SetTextVariable("DENAR", MathF.Abs(dailyTributePaid)).SetTextVariable("FACTION_1", this.Faction1Name).SetTextVariable("FACTION_2", this.Faction2Name), null) : new HintViewModel());
			this.Faction2TributeHint = ((dailyTributePaid < 0) ? new HintViewModel(textObject3.SetTextVariable("DENAR", MathF.Abs(dailyTributePaid)).SetTextVariable("FACTION_1", this.Faction2Name).SetTextVariable("FACTION_2", this.Faction1Name), null) : new HintViewModel());
			this.Faction1Leader = new HeroVM(this.Faction1.Leader, false);
			this.Faction2Leader = new HeroVM(this.Faction2.Leader, false);
			this.Faction1OwnedClans = new MBBindingList<KingdomDiplomacyFactionItemVM>();
			if (this.Faction1.IsKingdomFaction)
			{
				foreach (Clan clan in (this.Faction1 as Kingdom).Clans)
				{
					this.Faction1OwnedClans.Add(new KingdomDiplomacyFactionItemVM(clan));
				}
			}
			this.Faction2OwnedClans = new MBBindingList<KingdomDiplomacyFactionItemVM>();
			if (this.Faction2.IsKingdomFaction)
			{
				foreach (Clan clan2 in (this.Faction2 as Kingdom).Clans)
				{
					this.Faction2OwnedClans.Add(new KingdomDiplomacyFactionItemVM(clan2));
				}
			}
			this.Faction2OtherWars = new MBBindingList<KingdomDiplomacyFactionItemVM>();
			foreach (StanceLink stanceLink in this.Faction2.Stances)
			{
				if (stanceLink.IsAtWar && stanceLink.Faction1 != this.Faction1 && stanceLink.Faction2 != this.Faction1 && (stanceLink.Faction1.IsKingdomFaction || stanceLink.Faction1.Leader == Hero.MainHero) && (stanceLink.Faction2.IsKingdomFaction || stanceLink.Faction2.Leader == Hero.MainHero) && !stanceLink.Faction1.IsRebelClan && !stanceLink.Faction2.IsRebelClan && !stanceLink.Faction1.IsBanditFaction && !stanceLink.Faction2.IsBanditFaction)
				{
					this.Faction2OtherWars.Add(new KingdomDiplomacyFactionItemVM((stanceLink.Faction1 == this.Faction2) ? stanceLink.Faction2 : stanceLink.Faction1));
				}
			}
			this.IsFaction2OtherWarsVisible = this.Faction2OtherWars.Count > 0;
		}

		private void PopulateSettlements()
		{
			this._faction1Towns = new List<Settlement>();
			this._faction1Castles = new List<Settlement>();
			this._faction2Towns = new List<Settlement>();
			this._faction2Castles = new List<Settlement>();
			foreach (Settlement settlement in this.Faction1.Settlements)
			{
				if (settlement.IsTown)
				{
					this._faction1Towns.Add(settlement);
				}
				else if (settlement.IsCastle)
				{
					this._faction1Castles.Add(settlement);
				}
			}
			foreach (Settlement settlement2 in this.Faction2.Settlements)
			{
				if (settlement2.IsTown)
				{
					this._faction2Towns.Add(settlement2);
				}
				else if (settlement2.IsCastle)
				{
					this._faction2Castles.Add(settlement2);
				}
			}
		}

		protected abstract void ExecuteAction();

		[DataSourceProperty]
		public MBBindingList<KingdomDiplomacyFactionItemVM> Faction1OwnedClans
		{
			get
			{
				return this._faction1OwnedClans;
			}
			set
			{
				if (value != this._faction1OwnedClans)
				{
					this._faction1OwnedClans = value;
					base.OnPropertyChangedWithValue<MBBindingList<KingdomDiplomacyFactionItemVM>>(value, "Faction1OwnedClans");
				}
			}
		}

		[DataSourceProperty]
		public MBBindingList<KingdomDiplomacyFactionItemVM> Faction2OwnedClans
		{
			get
			{
				return this._faction2OwnedClans;
			}
			set
			{
				if (value != this._faction2OwnedClans)
				{
					this._faction2OwnedClans = value;
					base.OnPropertyChangedWithValue<MBBindingList<KingdomDiplomacyFactionItemVM>>(value, "Faction2OwnedClans");
				}
			}
		}

		[DataSourceProperty]
		public MBBindingList<KingdomDiplomacyFactionItemVM> Faction2OtherWars
		{
			get
			{
				return this._faction2OtherWars;
			}
			set
			{
				if (value != this._faction2OtherWars)
				{
					this._faction2OtherWars = value;
					base.OnPropertyChangedWithValue<MBBindingList<KingdomDiplomacyFactionItemVM>>(value, "Faction2OtherWars");
				}
			}
		}

		[DataSourceProperty]
		public MBBindingList<KingdomWarComparableStatVM> Stats
		{
			get
			{
				return this._stats;
			}
			set
			{
				if (value != this._stats)
				{
					this._stats = value;
					base.OnPropertyChangedWithValue<MBBindingList<KingdomWarComparableStatVM>>(value, "Stats");
				}
			}
		}

		[DataSourceProperty]
		public ImageIdentifierVM Faction1Visual
		{
			get
			{
				return this._faction1Visual;
			}
			set
			{
				if (value != this._faction1Visual)
				{
					this._faction1Visual = value;
					base.OnPropertyChangedWithValue<ImageIdentifierVM>(value, "Faction1Visual");
				}
			}
		}

		[DataSourceProperty]
		public ImageIdentifierVM Faction2Visual
		{
			get
			{
				return this._faction2Visual;
			}
			set
			{
				if (value != this._faction2Visual)
				{
					this._faction2Visual = value;
					base.OnPropertyChangedWithValue<ImageIdentifierVM>(value, "Faction2Visual");
				}
			}
		}

		[DataSourceProperty]
		public string Faction1Name
		{
			get
			{
				return this._faction1Name;
			}
			set
			{
				if (value != this._faction1Name)
				{
					this._faction1Name = value;
					base.OnPropertyChangedWithValue<string>(value, "Faction1Name");
				}
			}
		}

		[DataSourceProperty]
		public string Faction2Name
		{
			get
			{
				return this._faction2Name;
			}
			set
			{
				if (value != this._faction2Name)
				{
					this._faction2Name = value;
					base.OnPropertyChangedWithValue<string>(value, "Faction2Name");
				}
			}
		}

		[DataSourceProperty]
		public string Faction1TributeText
		{
			get
			{
				return this._faction1TributeText;
			}
			set
			{
				if (value != this._faction1TributeText)
				{
					this._faction1TributeText = value;
					base.OnPropertyChangedWithValue<string>(value, "Faction1TributeText");
				}
			}
		}

		[DataSourceProperty]
		public string Faction2TributeText
		{
			get
			{
				return this._faction2TributeText;
			}
			set
			{
				if (value != this._faction2TributeText)
				{
					this._faction2TributeText = value;
					base.OnPropertyChangedWithValue<string>(value, "Faction2TributeText");
				}
			}
		}

		[DataSourceProperty]
		public HintViewModel Faction1TributeHint
		{
			get
			{
				return this._faction1TributeHint;
			}
			set
			{
				if (value != this._faction1TributeHint)
				{
					this._faction1TributeHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "Faction1TributeHint");
				}
			}
		}

		[DataSourceProperty]
		public HintViewModel Faction2TributeHint
		{
			get
			{
				return this._faction2TributeHint;
			}
			set
			{
				if (value != this._faction2TributeHint)
				{
					this._faction2TributeHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "Faction2TributeHint");
				}
			}
		}

		[DataSourceProperty]
		public bool IsFaction2OtherWarsVisible
		{
			get
			{
				return this._isFaction2OtherWarsVisible;
			}
			set
			{
				if (value != this._isFaction2OtherWarsVisible)
				{
					this._isFaction2OtherWarsVisible = value;
					base.OnPropertyChangedWithValue(value, "IsFaction2OtherWarsVisible");
				}
			}
		}

		[DataSourceProperty]
		public HeroVM Faction1Leader
		{
			get
			{
				return this._faction1Leader;
			}
			set
			{
				if (value != this._faction1Leader)
				{
					this._faction1Leader = value;
					base.OnPropertyChangedWithValue<HeroVM>(value, "Faction1Leader");
				}
			}
		}

		[DataSourceProperty]
		public HeroVM Faction2Leader
		{
			get
			{
				return this._faction2Leader;
			}
			set
			{
				if (value != this._faction2Leader)
				{
					this._faction2Leader = value;
					base.OnPropertyChangedWithValue<HeroVM>(value, "Faction2Leader");
				}
			}
		}

		public readonly IFaction Faction1;

		public readonly IFaction Faction2;

		protected readonly string _faction1Color;

		protected readonly string _faction2Color;

		protected readonly IFaction _playerKingdom;

		protected List<Settlement> _faction1Towns;

		protected List<Settlement> _faction2Towns;

		protected List<Settlement> _faction1Castles;

		protected List<Settlement> _faction2Castles;

		private MBBindingList<KingdomWarComparableStatVM> _stats;

		private ImageIdentifierVM _faction1Visual;

		private ImageIdentifierVM _faction2Visual;

		private HeroVM _faction1Leader;

		private HeroVM _faction2Leader;

		private string _faction1Name;

		private string _faction2Name;

		private string _faction1TributeText;

		private string _faction2TributeText;

		private HintViewModel _faction1TributeHint;

		private HintViewModel _faction2TributeHint;

		private bool _isFaction2OtherWarsVisible;

		private MBBindingList<KingdomDiplomacyFactionItemVM> _faction1OwnedClans;

		private MBBindingList<KingdomDiplomacyFactionItemVM> _faction2OwnedClans;

		private MBBindingList<KingdomDiplomacyFactionItemVM> _faction2OtherWars;
	}
}
