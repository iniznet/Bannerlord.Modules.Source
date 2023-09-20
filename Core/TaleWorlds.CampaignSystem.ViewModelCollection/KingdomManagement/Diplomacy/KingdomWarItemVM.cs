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
	// Token: 0x02000060 RID: 96
	public class KingdomWarItemVM : KingdomDiplomacyItemVM
	{
		// Token: 0x06000847 RID: 2119 RVA: 0x000231A8 File Offset: 0x000213A8
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

		// Token: 0x06000848 RID: 2120 RVA: 0x00023398 File Offset: 0x00021598
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.UpdateDiplomacyProperties();
		}

		// Token: 0x06000849 RID: 2121 RVA: 0x000233A6 File Offset: 0x000215A6
		protected override void OnSelect()
		{
			this.UpdateDiplomacyProperties();
			this._onSelect(this);
			base.IsSelected = true;
		}

		// Token: 0x0600084A RID: 2122 RVA: 0x000233C4 File Offset: 0x000215C4
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

		// Token: 0x0600084B RID: 2123 RVA: 0x000236E6 File Offset: 0x000218E6
		protected override void ExecuteAction()
		{
			this._onAction(this);
		}

		// Token: 0x1700028C RID: 652
		// (get) Token: 0x0600084C RID: 2124 RVA: 0x000236F4 File Offset: 0x000218F4
		// (set) Token: 0x0600084D RID: 2125 RVA: 0x000236FC File Offset: 0x000218FC
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

		// Token: 0x1700028D RID: 653
		// (get) Token: 0x0600084E RID: 2126 RVA: 0x0002371F File Offset: 0x0002191F
		// (set) Token: 0x0600084F RID: 2127 RVA: 0x00023727 File Offset: 0x00021927
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

		// Token: 0x1700028E RID: 654
		// (get) Token: 0x06000850 RID: 2128 RVA: 0x0002374A File Offset: 0x0002194A
		// (set) Token: 0x06000851 RID: 2129 RVA: 0x00023752 File Offset: 0x00021952
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

		// Token: 0x1700028F RID: 655
		// (get) Token: 0x06000852 RID: 2130 RVA: 0x00023770 File Offset: 0x00021970
		// (set) Token: 0x06000853 RID: 2131 RVA: 0x00023778 File Offset: 0x00021978
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

		// Token: 0x17000290 RID: 656
		// (get) Token: 0x06000854 RID: 2132 RVA: 0x00023796 File Offset: 0x00021996
		// (set) Token: 0x06000855 RID: 2133 RVA: 0x0002379E File Offset: 0x0002199E
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

		// Token: 0x17000291 RID: 657
		// (get) Token: 0x06000856 RID: 2134 RVA: 0x000237BC File Offset: 0x000219BC
		// (set) Token: 0x06000857 RID: 2135 RVA: 0x000237C4 File Offset: 0x000219C4
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

		// Token: 0x17000292 RID: 658
		// (get) Token: 0x06000858 RID: 2136 RVA: 0x000237E2 File Offset: 0x000219E2
		// (set) Token: 0x06000859 RID: 2137 RVA: 0x000237EA File Offset: 0x000219EA
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

		// Token: 0x040003B2 RID: 946
		private readonly Action<KingdomWarItemVM> _onSelect;

		// Token: 0x040003B3 RID: 947
		private readonly StanceLink _war;

		// Token: 0x040003B4 RID: 948
		private readonly Action<KingdomWarItemVM> _onAction;

		// Token: 0x040003B5 RID: 949
		private List<Hero> _prisonersCapturedByFaction1;

		// Token: 0x040003B6 RID: 950
		private List<Hero> _prisonersCapturedByFaction2;

		// Token: 0x040003B7 RID: 951
		private List<Settlement> _townsCapturedByFaction1;

		// Token: 0x040003B8 RID: 952
		private List<Settlement> _townsCapturedByFaction2;

		// Token: 0x040003B9 RID: 953
		private List<Settlement> _castlesCapturedByFaction1;

		// Token: 0x040003BA RID: 954
		private List<Settlement> _castlesCapturedByFaction2;

		// Token: 0x040003BB RID: 955
		private List<Settlement> _raidsMadeByFaction1;

		// Token: 0x040003BC RID: 956
		private List<Settlement> _raidsMadeByFaction2;

		// Token: 0x040003BD RID: 957
		private string _warName;

		// Token: 0x040003BE RID: 958
		private string _numberOfDaysSinceWarBegan;

		// Token: 0x040003BF RID: 959
		private int _score;

		// Token: 0x040003C0 RID: 960
		private bool _isBehaviorSelectionEnabled;

		// Token: 0x040003C1 RID: 961
		private int _casualtiesOfFaction1;

		// Token: 0x040003C2 RID: 962
		private int _casualtiesOfFaction2;

		// Token: 0x040003C3 RID: 963
		private MBBindingList<KingdomWarLogItemVM> _warLog;
	}
}
