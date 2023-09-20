using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.KingdomManagement.Diplomacy
{
	// Token: 0x0200005B RID: 91
	public abstract class KingdomDiplomacyItemVM : KingdomItemVM
	{
		// Token: 0x060007C4 RID: 1988 RVA: 0x00021428 File Offset: 0x0001F628
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

		// Token: 0x060007C5 RID: 1989 RVA: 0x000214E8 File Offset: 0x0001F6E8
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

		// Token: 0x060007C6 RID: 1990 RVA: 0x000218C8 File Offset: 0x0001FAC8
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

		// Token: 0x060007C7 RID: 1991
		protected abstract void ExecuteAction();

		// Token: 0x1700025C RID: 604
		// (get) Token: 0x060007C8 RID: 1992 RVA: 0x000219DC File Offset: 0x0001FBDC
		// (set) Token: 0x060007C9 RID: 1993 RVA: 0x000219E4 File Offset: 0x0001FBE4
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

		// Token: 0x1700025D RID: 605
		// (get) Token: 0x060007CA RID: 1994 RVA: 0x00021A02 File Offset: 0x0001FC02
		// (set) Token: 0x060007CB RID: 1995 RVA: 0x00021A0A File Offset: 0x0001FC0A
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

		// Token: 0x1700025E RID: 606
		// (get) Token: 0x060007CC RID: 1996 RVA: 0x00021A28 File Offset: 0x0001FC28
		// (set) Token: 0x060007CD RID: 1997 RVA: 0x00021A30 File Offset: 0x0001FC30
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

		// Token: 0x1700025F RID: 607
		// (get) Token: 0x060007CE RID: 1998 RVA: 0x00021A4E File Offset: 0x0001FC4E
		// (set) Token: 0x060007CF RID: 1999 RVA: 0x00021A56 File Offset: 0x0001FC56
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

		// Token: 0x17000260 RID: 608
		// (get) Token: 0x060007D0 RID: 2000 RVA: 0x00021A74 File Offset: 0x0001FC74
		// (set) Token: 0x060007D1 RID: 2001 RVA: 0x00021A7C File Offset: 0x0001FC7C
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

		// Token: 0x17000261 RID: 609
		// (get) Token: 0x060007D2 RID: 2002 RVA: 0x00021A9A File Offset: 0x0001FC9A
		// (set) Token: 0x060007D3 RID: 2003 RVA: 0x00021AA2 File Offset: 0x0001FCA2
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

		// Token: 0x17000262 RID: 610
		// (get) Token: 0x060007D4 RID: 2004 RVA: 0x00021AC0 File Offset: 0x0001FCC0
		// (set) Token: 0x060007D5 RID: 2005 RVA: 0x00021AC8 File Offset: 0x0001FCC8
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

		// Token: 0x17000263 RID: 611
		// (get) Token: 0x060007D6 RID: 2006 RVA: 0x00021AEB File Offset: 0x0001FCEB
		// (set) Token: 0x060007D7 RID: 2007 RVA: 0x00021AF3 File Offset: 0x0001FCF3
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

		// Token: 0x17000264 RID: 612
		// (get) Token: 0x060007D8 RID: 2008 RVA: 0x00021B16 File Offset: 0x0001FD16
		// (set) Token: 0x060007D9 RID: 2009 RVA: 0x00021B1E File Offset: 0x0001FD1E
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

		// Token: 0x17000265 RID: 613
		// (get) Token: 0x060007DA RID: 2010 RVA: 0x00021B41 File Offset: 0x0001FD41
		// (set) Token: 0x060007DB RID: 2011 RVA: 0x00021B49 File Offset: 0x0001FD49
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

		// Token: 0x17000266 RID: 614
		// (get) Token: 0x060007DC RID: 2012 RVA: 0x00021B6C File Offset: 0x0001FD6C
		// (set) Token: 0x060007DD RID: 2013 RVA: 0x00021B74 File Offset: 0x0001FD74
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

		// Token: 0x17000267 RID: 615
		// (get) Token: 0x060007DE RID: 2014 RVA: 0x00021B92 File Offset: 0x0001FD92
		// (set) Token: 0x060007DF RID: 2015 RVA: 0x00021B9A File Offset: 0x0001FD9A
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

		// Token: 0x17000268 RID: 616
		// (get) Token: 0x060007E0 RID: 2016 RVA: 0x00021BB8 File Offset: 0x0001FDB8
		// (set) Token: 0x060007E1 RID: 2017 RVA: 0x00021BC0 File Offset: 0x0001FDC0
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

		// Token: 0x17000269 RID: 617
		// (get) Token: 0x060007E2 RID: 2018 RVA: 0x00021BDE File Offset: 0x0001FDDE
		// (set) Token: 0x060007E3 RID: 2019 RVA: 0x00021BE6 File Offset: 0x0001FDE6
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

		// Token: 0x1700026A RID: 618
		// (get) Token: 0x060007E4 RID: 2020 RVA: 0x00021C04 File Offset: 0x0001FE04
		// (set) Token: 0x060007E5 RID: 2021 RVA: 0x00021C0C File Offset: 0x0001FE0C
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

		// Token: 0x04000370 RID: 880
		public readonly IFaction Faction1;

		// Token: 0x04000371 RID: 881
		public readonly IFaction Faction2;

		// Token: 0x04000372 RID: 882
		protected readonly string _faction1Color;

		// Token: 0x04000373 RID: 883
		protected readonly string _faction2Color;

		// Token: 0x04000374 RID: 884
		protected readonly IFaction _playerKingdom;

		// Token: 0x04000375 RID: 885
		protected List<Settlement> _faction1Towns;

		// Token: 0x04000376 RID: 886
		protected List<Settlement> _faction2Towns;

		// Token: 0x04000377 RID: 887
		protected List<Settlement> _faction1Castles;

		// Token: 0x04000378 RID: 888
		protected List<Settlement> _faction2Castles;

		// Token: 0x04000379 RID: 889
		private MBBindingList<KingdomWarComparableStatVM> _stats;

		// Token: 0x0400037A RID: 890
		private ImageIdentifierVM _faction1Visual;

		// Token: 0x0400037B RID: 891
		private ImageIdentifierVM _faction2Visual;

		// Token: 0x0400037C RID: 892
		private HeroVM _faction1Leader;

		// Token: 0x0400037D RID: 893
		private HeroVM _faction2Leader;

		// Token: 0x0400037E RID: 894
		private string _faction1Name;

		// Token: 0x0400037F RID: 895
		private string _faction2Name;

		// Token: 0x04000380 RID: 896
		private string _faction1TributeText;

		// Token: 0x04000381 RID: 897
		private string _faction2TributeText;

		// Token: 0x04000382 RID: 898
		private HintViewModel _faction1TributeHint;

		// Token: 0x04000383 RID: 899
		private HintViewModel _faction2TributeHint;

		// Token: 0x04000384 RID: 900
		private bool _isFaction2OtherWarsVisible;

		// Token: 0x04000385 RID: 901
		private MBBindingList<KingdomDiplomacyFactionItemVM> _faction1OwnedClans;

		// Token: 0x04000386 RID: 902
		private MBBindingList<KingdomDiplomacyFactionItemVM> _faction2OwnedClans;

		// Token: 0x04000387 RID: 903
		private MBBindingList<KingdomDiplomacyFactionItemVM> _faction2OtherWars;
	}
}
