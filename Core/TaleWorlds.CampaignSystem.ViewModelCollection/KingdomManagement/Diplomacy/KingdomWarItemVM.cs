using System;
using System.Collections.Generic;
using Helpers;
using TaleWorlds.CampaignSystem.LogEntries;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.KingdomManagement.Diplomacy
{
	public class KingdomWarItemVM : KingdomDiplomacyItemVM
	{
		public KingdomWarItemVM(StanceLink war, Action<KingdomWarItemVM> onSelect, Action<KingdomWarItemVM> onAction)
			: base(war.Faction1, war.Faction2)
		{
			this._war = war;
			this._onSelect = onSelect;
			this._onAction = onAction;
			this.IsBehaviorSelectionEnabled = this.Faction1.IsKingdomFaction && this.Faction1.Leader == Hero.MainHero;
			this._prisonersCapturedByFaction1 = DiplomacyHelper.GetPrisonersOfWarTakenByFaction(this.Faction1, this.Faction2);
			this._prisonersCapturedByFaction2 = DiplomacyHelper.GetPrisonersOfWarTakenByFaction(this.Faction2, this.Faction1);
			this._townsCapturedByFaction1 = DiplomacyHelper.GetSuccessfullSiegesInWarForFaction(this.Faction1, war, (Settlement x) => x.IsTown);
			this._townsCapturedByFaction2 = DiplomacyHelper.GetSuccessfullSiegesInWarForFaction(this.Faction2, war, (Settlement x) => x.IsTown);
			this._castlesCapturedByFaction1 = DiplomacyHelper.GetSuccessfullSiegesInWarForFaction(this.Faction1, war, (Settlement x) => x.IsCastle);
			this._castlesCapturedByFaction2 = DiplomacyHelper.GetSuccessfullSiegesInWarForFaction(this.Faction2, war, (Settlement x) => x.IsCastle);
			this._raidsMadeByFaction1 = DiplomacyHelper.GetRaidsInWar(this.Faction1, war, null);
			this._raidsMadeByFaction2 = DiplomacyHelper.GetRaidsInWar(this.Faction2, war, null);
			this.RefreshValues();
			this.WarLog = new MBBindingList<KingdomWarLogItemVM>();
			foreach (ValueTuple<LogEntry, IFaction, IFaction> valueTuple in DiplomacyHelper.GetLogsForWar(war))
			{
				LogEntry item = valueTuple.Item1;
				IFaction item2 = valueTuple.Item2;
				IEncyclopediaLog encyclopediaLog;
				if ((encyclopediaLog = item as IEncyclopediaLog) != null)
				{
					this.WarLog.Add(new KingdomWarLogItemVM(encyclopediaLog, item2));
				}
			}
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
			this.UpdateDiplomacyProperties();
		}

		protected override void OnSelect()
		{
			this.UpdateDiplomacyProperties();
			this._onSelect(this);
			base.IsSelected = true;
		}

		protected override void UpdateDiplomacyProperties()
		{
			base.UpdateDiplomacyProperties();
			GameTexts.SetVariable("FACTION_1_NAME", this.Faction1.Name.ToString());
			GameTexts.SetVariable("FACTION_2_NAME", this.Faction2.Name.ToString());
			this.WarName = GameTexts.FindText("str_war_faction_versus_faction", null).ToString();
			StanceLink stanceWith = this.Faction1.GetStanceWith(this.Faction2);
			this.Score = stanceWith.GetSuccessfulSieges(this.Faction1) + stanceWith.GetSuccessfulRaids(this.Faction1);
			this.CasualtiesOfFaction1 = stanceWith.GetCasualties(this.Faction1);
			this.CasualtiesOfFaction2 = stanceWith.GetCasualties(this.Faction2);
			int num = MathF.Ceiling(this._war.WarStartDate.ElapsedDaysUntilNow + 0.01f);
			TextObject textObject = GameTexts.FindText("str_for_DAY_days", null);
			textObject.SetTextVariable("DAY", num.ToString());
			textObject.SetTextVariable("DAY_IS_PLURAL", (num > 1) ? 1 : 0);
			this.NumberOfDaysSinceWarBegan = textObject.ToString();
			base.Stats.Add(new KingdomWarComparableStatVM((int)this.Faction1.TotalStrength, (int)this.Faction2.TotalStrength, GameTexts.FindText("str_total_strength", null), this._faction1Color, this._faction2Color, 10000, null, null));
			base.Stats.Add(new KingdomWarComparableStatVM(stanceWith.GetCasualties(this.Faction2), stanceWith.GetCasualties(this.Faction1), GameTexts.FindText("str_war_casualties_inflicted", null), this._faction1Color, this._faction2Color, 5000, null, null));
			base.Stats.Add(new KingdomWarComparableStatVM(this._prisonersCapturedByFaction1.Count, this._prisonersCapturedByFaction2.Count, GameTexts.FindText("str_party_category_prisoners_tooltip", null), this._faction1Color, this._faction2Color, 10, new BasicTooltipViewModel(() => CampaignUIHelper.GetWarPrisonersTooltip(this._prisonersCapturedByFaction1, this.Faction1.Name)), new BasicTooltipViewModel(() => CampaignUIHelper.GetWarPrisonersTooltip(this._prisonersCapturedByFaction2, this.Faction2.Name))));
			base.Stats.Add(new KingdomWarComparableStatVM(this._faction1Towns.Count, this._faction2Towns.Count, GameTexts.FindText("str_towns", null), this._faction1Color, this._faction2Color, 25, new BasicTooltipViewModel(() => CampaignUIHelper.GetWarSuccessfulSiegesTooltip(this._townsCapturedByFaction1, this.Faction1.Name, true)), new BasicTooltipViewModel(() => CampaignUIHelper.GetWarSuccessfulSiegesTooltip(this._townsCapturedByFaction2, this.Faction2.Name, true))));
			base.Stats.Add(new KingdomWarComparableStatVM(this._faction1Castles.Count, this._faction2Castles.Count, GameTexts.FindText("str_castles", null), this._faction1Color, this._faction2Color, 25, new BasicTooltipViewModel(() => CampaignUIHelper.GetWarSuccessfulSiegesTooltip(this._castlesCapturedByFaction1, this.Faction1.Name, false)), new BasicTooltipViewModel(() => CampaignUIHelper.GetWarSuccessfulSiegesTooltip(this._castlesCapturedByFaction2, this.Faction2.Name, false))));
			base.Stats.Add(new KingdomWarComparableStatVM(stanceWith.GetSuccessfulRaids(this.Faction1), stanceWith.GetSuccessfulRaids(this.Faction2), GameTexts.FindText("str_war_successful_raids", null), this._faction1Color, this._faction2Color, 10, new BasicTooltipViewModel(() => CampaignUIHelper.GetWarSuccessfulRaidsTooltip(this._raidsMadeByFaction1, this.Faction1.Name)), new BasicTooltipViewModel(() => CampaignUIHelper.GetWarSuccessfulRaidsTooltip(this._raidsMadeByFaction2, this.Faction2.Name))));
		}

		protected override void ExecuteAction()
		{
			this._onAction(this);
		}

		[DataSourceProperty]
		public string WarName
		{
			get
			{
				return this._warName;
			}
			set
			{
				if (value != this._warName)
				{
					this._warName = value;
					base.OnPropertyChangedWithValue<string>(value, "WarName");
				}
			}
		}

		[DataSourceProperty]
		public string NumberOfDaysSinceWarBegan
		{
			get
			{
				return this._numberOfDaysSinceWarBegan;
			}
			set
			{
				if (value != this._numberOfDaysSinceWarBegan)
				{
					this._numberOfDaysSinceWarBegan = value;
					base.OnPropertyChangedWithValue<string>(value, "NumberOfDaysSinceWarBegan");
				}
			}
		}

		[DataSourceProperty]
		public bool IsBehaviorSelectionEnabled
		{
			get
			{
				return this._isBehaviorSelectionEnabled;
			}
			set
			{
				if (value != this._isBehaviorSelectionEnabled)
				{
					this._isBehaviorSelectionEnabled = value;
					base.OnPropertyChangedWithValue(value, "IsBehaviorSelectionEnabled");
				}
			}
		}

		[DataSourceProperty]
		public int Score
		{
			get
			{
				return this._score;
			}
			set
			{
				if (value != this._score)
				{
					this._score = value;
					base.OnPropertyChangedWithValue(value, "Score");
				}
			}
		}

		[DataSourceProperty]
		public int CasualtiesOfFaction1
		{
			get
			{
				return this._casualtiesOfFaction1;
			}
			set
			{
				if (value != this._casualtiesOfFaction1)
				{
					this._casualtiesOfFaction1 = value;
					base.OnPropertyChangedWithValue(value, "CasualtiesOfFaction1");
				}
			}
		}

		[DataSourceProperty]
		public int CasualtiesOfFaction2
		{
			get
			{
				return this._casualtiesOfFaction2;
			}
			set
			{
				if (value != this._casualtiesOfFaction2)
				{
					this._casualtiesOfFaction2 = value;
					base.OnPropertyChangedWithValue(value, "CasualtiesOfFaction2");
				}
			}
		}

		[DataSourceProperty]
		public MBBindingList<KingdomWarLogItemVM> WarLog
		{
			get
			{
				return this._warLog;
			}
			set
			{
				if (value != this._warLog)
				{
					this._warLog = value;
					base.OnPropertyChangedWithValue<MBBindingList<KingdomWarLogItemVM>>(value, "WarLog");
				}
			}
		}

		private readonly Action<KingdomWarItemVM> _onSelect;

		private readonly StanceLink _war;

		private readonly Action<KingdomWarItemVM> _onAction;

		private List<Hero> _prisonersCapturedByFaction1;

		private List<Hero> _prisonersCapturedByFaction2;

		private List<Settlement> _townsCapturedByFaction1;

		private List<Settlement> _townsCapturedByFaction2;

		private List<Settlement> _castlesCapturedByFaction1;

		private List<Settlement> _castlesCapturedByFaction2;

		private List<Settlement> _raidsMadeByFaction1;

		private List<Settlement> _raidsMadeByFaction2;

		private string _warName;

		private string _numberOfDaysSinceWarBegan;

		private int _score;

		private bool _isBehaviorSelectionEnabled;

		private int _casualtiesOfFaction1;

		private int _casualtiesOfFaction2;

		private MBBindingList<KingdomWarLogItemVM> _warLog;
	}
}
