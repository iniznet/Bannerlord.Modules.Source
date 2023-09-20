using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem.Encyclopedia;
using TaleWorlds.CampaignSystem.LogEntries;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.ViewModelCollection.Encyclopedia.Items;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.Encyclopedia.Pages
{
	// Token: 0x020000B3 RID: 179
	[EncyclopediaViewModel(typeof(Kingdom))]
	public class EncyclopediaFactionPageVM : EncyclopediaContentPageVM
	{
		// Token: 0x06001181 RID: 4481 RVA: 0x000450AC File Offset: 0x000432AC
		public EncyclopediaFactionPageVM(EncyclopediaPageArgs args)
			: base(args)
		{
			this._faction = base.Obj as Kingdom;
			this.Clans = new MBBindingList<EncyclopediaFactionVM>();
			this.Enemies = new MBBindingList<EncyclopediaFactionVM>();
			this.Settlements = new MBBindingList<EncyclopediaSettlementVM>();
			this.History = new MBBindingList<EncyclopediaHistoryEventVM>();
			base.IsBookmarked = Campaign.Current.EncyclopediaManager.ViewDataTracker.IsEncyclopediaBookmarked(this._faction);
			this.RefreshValues();
		}

		// Token: 0x06001182 RID: 4482 RVA: 0x00045124 File Offset: 0x00043324
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.StrengthHint = new HintViewModel(GameTexts.FindText("str_strength", null), null);
			this.ProsperityHint = new HintViewModel(GameTexts.FindText("str_prosperity", null), null);
			this.MembersText = GameTexts.FindText("str_members", null).ToString();
			this.ClansText = new TextObject("{=bfQLwMUp}Clans", null).ToString();
			this.EnemiesText = new TextObject("{=zZlWRZjO}Wars", null).ToString();
			this.SettlementsText = new TextObject("{=LBNzsqyb}Fiefs", null).ToString();
			this.VillagesText = GameTexts.FindText("str_villages", null).ToString();
			TextObject encyclopediaText = this._faction.EncyclopediaText;
			this.InformationText = ((encyclopediaText != null) ? encyclopediaText.ToString() : null) ?? string.Empty;
			base.UpdateBookmarkHintText();
			this.Refresh();
		}

		// Token: 0x06001183 RID: 4483 RVA: 0x00045208 File Offset: 0x00043408
		public override void Refresh()
		{
			base.IsLoadingOver = false;
			this.Clans.Clear();
			this.Enemies.Clear();
			this.Settlements.Clear();
			this.History.Clear();
			this.Leader = new HeroVM(this._faction.Leader, false);
			this.LeaderText = GameTexts.FindText("str_leader", null).ToString();
			this.NameText = this._faction.Name.ToString();
			this.DescriptorText = GameTexts.FindText("str_kingdom_faction", null).ToString();
			int num = 0;
			float num2 = 0f;
			EncyclopediaPage pageOf = Campaign.Current.EncyclopediaManager.GetPageOf(typeof(Hero));
			foreach (Hero hero in this._faction.Lords)
			{
				if (pageOf.IsValidEncyclopediaItem(hero))
				{
					num += hero.Gold;
				}
			}
			this.Banner = new ImageIdentifierVM(BannerCode.CreateFrom(this._faction.Banner), true);
			foreach (MobileParty mobileParty in MobileParty.AllLordParties)
			{
				if (mobileParty.MapFaction == this._faction && !mobileParty.IsDisbanding)
				{
					num2 += mobileParty.Party.TotalStrength;
				}
			}
			this.ProsperityText = num.ToString();
			this.StrengthText = num2.ToString();
			for (int i = Campaign.Current.LogEntryHistory.GameActionLogs.Count - 1; i >= 0; i--)
			{
				IEncyclopediaLog encyclopediaLog;
				if ((encyclopediaLog = Campaign.Current.LogEntryHistory.GameActionLogs[i] as IEncyclopediaLog) != null && encyclopediaLog.IsVisibleInEncyclopediaPageOf<Kingdom>(this._faction))
				{
					this.History.Add(new EncyclopediaHistoryEventVM(encyclopediaLog));
				}
			}
			EncyclopediaPage pageOf2 = Campaign.Current.EncyclopediaManager.GetPageOf(typeof(Clan));
			using (IEnumerator<IFaction> enumerator3 = Campaign.Current.Factions.OrderBy((IFaction x) => !x.IsKingdomFaction).ThenBy((IFaction f) => f.Name.ToString()).GetEnumerator())
			{
				while (enumerator3.MoveNext())
				{
					IFaction factionObject = enumerator3.Current;
					if (pageOf2.IsValidEncyclopediaItem(factionObject) && factionObject != this._faction && !factionObject.IsBanditFaction && FactionManager.IsAtWarAgainstFaction(this._faction, factionObject.MapFaction) && !this.Enemies.Any((EncyclopediaFactionVM x) => x.Faction == factionObject.MapFaction))
					{
						this.Enemies.Add(new EncyclopediaFactionVM(factionObject.MapFaction));
					}
				}
			}
			foreach (Clan clan in Campaign.Current.Clans.Where((Clan c) => c.Kingdom == this._faction))
			{
				this.Clans.Add(new EncyclopediaFactionVM(clan));
			}
			EncyclopediaPage pageOf3 = Campaign.Current.EncyclopediaManager.GetPageOf(typeof(Settlement));
			foreach (Settlement settlement in from s in Settlement.All
				where s.IsTown || s.IsCastle
				orderby s.IsCastle, s.IsTown
				select s)
			{
				if ((settlement.MapFaction == this._faction || (settlement.OwnerClan == this._faction.RulingClan && settlement.OwnerClan.Leader != null)) && pageOf3.IsValidEncyclopediaItem(settlement))
				{
					this.Settlements.Add(new EncyclopediaSettlementVM(settlement));
				}
			}
			base.IsLoadingOver = true;
		}

		// Token: 0x06001184 RID: 4484 RVA: 0x000456C4 File Offset: 0x000438C4
		public override string GetName()
		{
			return this._faction.Name.ToString();
		}

		// Token: 0x06001185 RID: 4485 RVA: 0x000456D8 File Offset: 0x000438D8
		public override string GetNavigationBarURL()
		{
			return HyperlinkTexts.GetGenericHyperlinkText("Home", GameTexts.FindText("str_encyclopedia_home", null).ToString()) + " \\ " + HyperlinkTexts.GetGenericHyperlinkText("ListPage-Kingdoms", GameTexts.FindText("str_encyclopedia_kingdoms", null).ToString()) + " \\ " + this.GetName();
		}

		// Token: 0x06001186 RID: 4486 RVA: 0x00045740 File Offset: 0x00043940
		public override void ExecuteSwitchBookmarkedState()
		{
			base.ExecuteSwitchBookmarkedState();
			if (base.IsBookmarked)
			{
				Campaign.Current.EncyclopediaManager.ViewDataTracker.AddEncyclopediaBookmarkToItem(this._faction);
				return;
			}
			Campaign.Current.EncyclopediaManager.ViewDataTracker.RemoveEncyclopediaBookmarkFromItem(this._faction);
		}

		// Token: 0x170005C9 RID: 1481
		// (get) Token: 0x06001187 RID: 4487 RVA: 0x00045790 File Offset: 0x00043990
		// (set) Token: 0x06001188 RID: 4488 RVA: 0x00045798 File Offset: 0x00043998
		[DataSourceProperty]
		public MBBindingList<EncyclopediaFactionVM> Clans
		{
			get
			{
				return this._clans;
			}
			set
			{
				if (value != this._clans)
				{
					this._clans = value;
					base.OnPropertyChangedWithValue<MBBindingList<EncyclopediaFactionVM>>(value, "Clans");
				}
			}
		}

		// Token: 0x170005CA RID: 1482
		// (get) Token: 0x06001189 RID: 4489 RVA: 0x000457B6 File Offset: 0x000439B6
		// (set) Token: 0x0600118A RID: 4490 RVA: 0x000457BE File Offset: 0x000439BE
		[DataSourceProperty]
		public MBBindingList<EncyclopediaFactionVM> Enemies
		{
			get
			{
				return this._enemies;
			}
			set
			{
				if (value != this._enemies)
				{
					this._enemies = value;
					base.OnPropertyChangedWithValue<MBBindingList<EncyclopediaFactionVM>>(value, "Enemies");
				}
			}
		}

		// Token: 0x170005CB RID: 1483
		// (get) Token: 0x0600118B RID: 4491 RVA: 0x000457DC File Offset: 0x000439DC
		// (set) Token: 0x0600118C RID: 4492 RVA: 0x000457E4 File Offset: 0x000439E4
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
					base.OnPropertyChangedWithValue<MBBindingList<EncyclopediaSettlementVM>>(value, "Settlements");
				}
			}
		}

		// Token: 0x170005CC RID: 1484
		// (get) Token: 0x0600118D RID: 4493 RVA: 0x00045802 File Offset: 0x00043A02
		// (set) Token: 0x0600118E RID: 4494 RVA: 0x0004580A File Offset: 0x00043A0A
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

		// Token: 0x170005CD RID: 1485
		// (get) Token: 0x0600118F RID: 4495 RVA: 0x00045828 File Offset: 0x00043A28
		// (set) Token: 0x06001190 RID: 4496 RVA: 0x00045830 File Offset: 0x00043A30
		[DataSourceProperty]
		public HeroVM Leader
		{
			get
			{
				return this._leader;
			}
			set
			{
				if (value != this._leader)
				{
					this._leader = value;
					base.OnPropertyChangedWithValue<HeroVM>(value, "Leader");
				}
			}
		}

		// Token: 0x170005CE RID: 1486
		// (get) Token: 0x06001191 RID: 4497 RVA: 0x0004584E File Offset: 0x00043A4E
		// (set) Token: 0x06001192 RID: 4498 RVA: 0x00045856 File Offset: 0x00043A56
		[DataSourceProperty]
		public ImageIdentifierVM Banner
		{
			get
			{
				return this._banner;
			}
			set
			{
				if (value != this._banner)
				{
					this._banner = value;
					base.OnPropertyChangedWithValue<ImageIdentifierVM>(value, "Banner");
				}
			}
		}

		// Token: 0x170005CF RID: 1487
		// (get) Token: 0x06001193 RID: 4499 RVA: 0x00045874 File Offset: 0x00043A74
		// (set) Token: 0x06001194 RID: 4500 RVA: 0x0004587C File Offset: 0x00043A7C
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

		// Token: 0x170005D0 RID: 1488
		// (get) Token: 0x06001195 RID: 4501 RVA: 0x0004589F File Offset: 0x00043A9F
		// (set) Token: 0x06001196 RID: 4502 RVA: 0x000458A7 File Offset: 0x00043AA7
		[DataSourceProperty]
		public string MembersText
		{
			get
			{
				return this._membersText;
			}
			set
			{
				if (value != this._membersText)
				{
					this._membersText = value;
					base.OnPropertyChangedWithValue<string>(value, "MembersText");
				}
			}
		}

		// Token: 0x170005D1 RID: 1489
		// (get) Token: 0x06001197 RID: 4503 RVA: 0x000458CA File Offset: 0x00043ACA
		// (set) Token: 0x06001198 RID: 4504 RVA: 0x000458D2 File Offset: 0x00043AD2
		[DataSourceProperty]
		public string EnemiesText
		{
			get
			{
				return this._enemiesText;
			}
			set
			{
				if (value != this._enemiesText)
				{
					this._enemiesText = value;
					base.OnPropertyChangedWithValue<string>(value, "EnemiesText");
				}
			}
		}

		// Token: 0x170005D2 RID: 1490
		// (get) Token: 0x06001199 RID: 4505 RVA: 0x000458F5 File Offset: 0x00043AF5
		// (set) Token: 0x0600119A RID: 4506 RVA: 0x000458FD File Offset: 0x00043AFD
		[DataSourceProperty]
		public string ClansText
		{
			get
			{
				return this._clansText;
			}
			set
			{
				if (value != this._clansText)
				{
					this._clansText = value;
					base.OnPropertyChangedWithValue<string>(value, "ClansText");
				}
			}
		}

		// Token: 0x170005D3 RID: 1491
		// (get) Token: 0x0600119B RID: 4507 RVA: 0x00045920 File Offset: 0x00043B20
		// (set) Token: 0x0600119C RID: 4508 RVA: 0x00045928 File Offset: 0x00043B28
		[DataSourceProperty]
		public string SettlementsText
		{
			get
			{
				return this._settlementsText;
			}
			set
			{
				if (value != this._settlementsText)
				{
					this._settlementsText = value;
					base.OnPropertyChangedWithValue<string>(value, "SettlementsText");
				}
			}
		}

		// Token: 0x170005D4 RID: 1492
		// (get) Token: 0x0600119D RID: 4509 RVA: 0x0004594B File Offset: 0x00043B4B
		// (set) Token: 0x0600119E RID: 4510 RVA: 0x00045953 File Offset: 0x00043B53
		[DataSourceProperty]
		public string VillagesText
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
					base.OnPropertyChangedWithValue<string>(value, "VillagesText");
				}
			}
		}

		// Token: 0x170005D5 RID: 1493
		// (get) Token: 0x0600119F RID: 4511 RVA: 0x00045976 File Offset: 0x00043B76
		// (set) Token: 0x060011A0 RID: 4512 RVA: 0x0004597E File Offset: 0x00043B7E
		[DataSourceProperty]
		public string LeaderText
		{
			get
			{
				return this._leaderText;
			}
			set
			{
				if (value != this._leaderText)
				{
					this._leaderText = value;
					base.OnPropertyChangedWithValue<string>(value, "LeaderText");
				}
			}
		}

		// Token: 0x170005D6 RID: 1494
		// (get) Token: 0x060011A1 RID: 4513 RVA: 0x000459A1 File Offset: 0x00043BA1
		// (set) Token: 0x060011A2 RID: 4514 RVA: 0x000459A9 File Offset: 0x00043BA9
		[DataSourceProperty]
		public string DescriptorText
		{
			get
			{
				return this._descriptorText;
			}
			set
			{
				if (value != this._descriptorText)
				{
					this._descriptorText = value;
					base.OnPropertyChangedWithValue<string>(value, "DescriptorText");
				}
			}
		}

		// Token: 0x170005D7 RID: 1495
		// (get) Token: 0x060011A3 RID: 4515 RVA: 0x000459CC File Offset: 0x00043BCC
		// (set) Token: 0x060011A4 RID: 4516 RVA: 0x000459D4 File Offset: 0x00043BD4
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

		// Token: 0x170005D8 RID: 1496
		// (get) Token: 0x060011A5 RID: 4517 RVA: 0x000459F7 File Offset: 0x00043BF7
		// (set) Token: 0x060011A6 RID: 4518 RVA: 0x000459FF File Offset: 0x00043BFF
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

		// Token: 0x170005D9 RID: 1497
		// (get) Token: 0x060011A7 RID: 4519 RVA: 0x00045A22 File Offset: 0x00043C22
		// (set) Token: 0x060011A8 RID: 4520 RVA: 0x00045A2A File Offset: 0x00043C2A
		[DataSourceProperty]
		public string StrengthText
		{
			get
			{
				return this._strengthText;
			}
			set
			{
				if (value != this._strengthText)
				{
					this._strengthText = value;
					base.OnPropertyChangedWithValue<string>(value, "StrengthText");
				}
			}
		}

		// Token: 0x170005DA RID: 1498
		// (get) Token: 0x060011A9 RID: 4521 RVA: 0x00045A4D File Offset: 0x00043C4D
		// (set) Token: 0x060011AA RID: 4522 RVA: 0x00045A55 File Offset: 0x00043C55
		[DataSourceProperty]
		public HintViewModel ProsperityHint
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
					base.OnPropertyChangedWithValue<HintViewModel>(value, "ProsperityHint");
				}
			}
		}

		// Token: 0x170005DB RID: 1499
		// (get) Token: 0x060011AB RID: 4523 RVA: 0x00045A73 File Offset: 0x00043C73
		// (set) Token: 0x060011AC RID: 4524 RVA: 0x00045A7B File Offset: 0x00043C7B
		[DataSourceProperty]
		public HintViewModel StrengthHint
		{
			get
			{
				return this._strengthHint;
			}
			set
			{
				if (value != this._strengthHint)
				{
					this._strengthHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "StrengthHint");
				}
			}
		}

		// Token: 0x04000828 RID: 2088
		private Kingdom _faction;

		// Token: 0x04000829 RID: 2089
		private MBBindingList<EncyclopediaFactionVM> _clans;

		// Token: 0x0400082A RID: 2090
		private MBBindingList<EncyclopediaFactionVM> _enemies;

		// Token: 0x0400082B RID: 2091
		private MBBindingList<EncyclopediaSettlementVM> _settlements;

		// Token: 0x0400082C RID: 2092
		private MBBindingList<EncyclopediaHistoryEventVM> _history;

		// Token: 0x0400082D RID: 2093
		private HeroVM _leader;

		// Token: 0x0400082E RID: 2094
		private ImageIdentifierVM _banner;

		// Token: 0x0400082F RID: 2095
		private string _membersText;

		// Token: 0x04000830 RID: 2096
		private string _enemiesText;

		// Token: 0x04000831 RID: 2097
		private string _clansText;

		// Token: 0x04000832 RID: 2098
		private string _settlementsText;

		// Token: 0x04000833 RID: 2099
		private string _villagesText;

		// Token: 0x04000834 RID: 2100
		private string _leaderText;

		// Token: 0x04000835 RID: 2101
		private string _descriptorText;

		// Token: 0x04000836 RID: 2102
		private string _prosperityText;

		// Token: 0x04000837 RID: 2103
		private string _strengthText;

		// Token: 0x04000838 RID: 2104
		private string _informationText;

		// Token: 0x04000839 RID: 2105
		private HintViewModel _prosperityHint;

		// Token: 0x0400083A RID: 2106
		private HintViewModel _strengthHint;

		// Token: 0x0400083B RID: 2107
		private string _nameText;
	}
}
