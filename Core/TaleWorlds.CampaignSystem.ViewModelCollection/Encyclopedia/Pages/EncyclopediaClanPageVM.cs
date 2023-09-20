using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem.Encyclopedia;
using TaleWorlds.CampaignSystem.LogEntries;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.ViewModelCollection.Encyclopedia.Items;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Generic;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.Encyclopedia.Pages
{
	[EncyclopediaViewModel(typeof(Clan))]
	public class EncyclopediaClanPageVM : EncyclopediaContentPageVM
	{
		public EncyclopediaClanPageVM(EncyclopediaPageArgs args)
			: base(args)
		{
			this._faction = base.Obj as IFaction;
			this._clan = this._faction as Clan;
			this.Members = new MBBindingList<HeroVM>();
			this.Enemies = new MBBindingList<EncyclopediaFactionVM>();
			this.Settlements = new MBBindingList<EncyclopediaSettlementVM>();
			this.History = new MBBindingList<EncyclopediaHistoryEventVM>();
			this.ClanInfo = new MBBindingList<StringPairItemVM>();
			base.IsBookmarked = Campaign.Current.EncyclopediaManager.ViewDataTracker.IsEncyclopediaBookmarked(this._clan);
			this.RefreshValues();
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
			this.StrengthHint = new HintViewModel(GameTexts.FindText("str_strength", null), null);
			this.ProsperityHint = new HintViewModel(GameTexts.FindText("str_prosperity", null), null);
			this.MembersText = GameTexts.FindText("str_members", null).ToString();
			this.AlliesText = new TextObject("{=bfQLwMUp}Clans", null).ToString();
			this.EnemiesText = new TextObject("{=zZlWRZjO}Wars", null).ToString();
			this.SettlementsText = GameTexts.FindText("str_settlements", null).ToString();
			this.VillagesText = GameTexts.FindText("str_villages", null).ToString();
			this.DestroyedText = new TextObject("{=w8Yzf0F0}Destroyed", null).ToString();
			this.PartOfText = GameTexts.FindText("str_encyclopedia_clan_part_of_kingdom", null).ToString();
			this.LeaderText = GameTexts.FindText("str_leader", null).ToString();
			this.InfoText = GameTexts.FindText("str_info", null).ToString();
			base.UpdateBookmarkHintText();
			this.Refresh();
		}

		public override void Refresh()
		{
			this.Members.Clear();
			this.Enemies.Clear();
			this.Settlements.Clear();
			this.History.Clear();
			this.ClanInfo.Clear();
			TextObject encyclopediaText = this._faction.EncyclopediaText;
			this.InformationText = ((encyclopediaText != null) ? encyclopediaText.ToString() : null);
			this.Leader = new HeroVM(this._faction.Leader, true);
			this.NameText = this._clan.Name.ToString();
			this.HasParentKingdom = this._clan.Kingdom != null;
			this.ParentKingdom = (this.HasParentKingdom ? new EncyclopediaFactionVM(((Clan)this._faction).Kingdom) : null);
			if (this._faction.IsKingdomFaction)
			{
				this.DescriptorText = GameTexts.FindText("str_kingdom_faction", null).ToString();
			}
			else if (this._faction.IsBanditFaction)
			{
				this.DescriptorText = GameTexts.FindText("str_bandit_faction", null).ToString();
			}
			else if (this._faction.IsMinorFaction)
			{
				this.DescriptorText = GameTexts.FindText("str_minor_faction", null).ToString();
			}
			int num = 0;
			float num2 = 0f;
			EncyclopediaPage pageOf = Campaign.Current.EncyclopediaManager.GetPageOf(typeof(Hero));
			IEnumerable<Hero> heroes = this._faction.Heroes;
			Clan clan = this._clan;
			foreach (Hero hero in heroes.Union((clan != null) ? clan.Companions : null))
			{
				if (pageOf.IsValidEncyclopediaItem(hero))
				{
					if (hero != this.Leader.Hero)
					{
						this.Members.Add(new HeroVM(hero, true));
					}
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
				if ((encyclopediaLog = Campaign.Current.LogEntryHistory.GameActionLogs[i] as IEncyclopediaLog) != null && ((this._faction.IsKingdomFaction && encyclopediaLog.IsVisibleInEncyclopediaPageOf<Kingdom>((Kingdom)this._faction)) || (this._faction.IsClan && encyclopediaLog.IsVisibleInEncyclopediaPageOf<Clan>((Clan)this._faction))))
				{
					this.History.Add(new EncyclopediaHistoryEventVM(encyclopediaLog));
				}
			}
			EncyclopediaPage pageOf2 = Campaign.Current.EncyclopediaManager.GetPageOf(typeof(Clan));
			foreach (IFaction faction in Campaign.Current.Factions.OrderBy((IFaction x) => !x.IsKingdomFaction).ThenBy((IFaction f) => f.Name.ToString()))
			{
				IFaction mapFaction = faction.MapFaction;
				if (pageOf2.IsValidEncyclopediaItem(mapFaction) && mapFaction != this._faction.MapFaction && mapFaction != this._faction && !mapFaction.IsBanditFaction && FactionManager.IsAtWarAgainstFaction(this._faction.MapFaction, mapFaction) && !this.Enemies.Any((EncyclopediaFactionVM x) => x.Faction == mapFaction))
				{
					this.Enemies.Add(new EncyclopediaFactionVM(mapFaction));
				}
			}
			EncyclopediaPage pageOf3 = Campaign.Current.EncyclopediaManager.GetPageOf(typeof(Settlement));
			foreach (Settlement settlement in from s in Settlement.All
				orderby s.IsVillage, s.IsCastle, s.IsTown
				select s)
			{
				if ((settlement.MapFaction == this._faction || (settlement.OwnerClan == this._faction && settlement.OwnerClan.Leader != null)) && pageOf3.IsValidEncyclopediaItem(settlement) && (settlement.IsTown || settlement.IsCastle))
				{
					this.Settlements.Add(new EncyclopediaSettlementVM(settlement));
				}
			}
			GameTexts.SetVariable("LEFT", new TextObject("{=tTLvo8sM}Clan Tier", null).ToString());
			this.ClanInfo.Add(new StringPairItemVM(GameTexts.FindText("str_LEFT_colon", null).ToString(), this._clan.Tier.ToString(), null));
			GameTexts.SetVariable("LEFT", new TextObject("{=ODEnkg0o}Clan Strength", null).ToString());
			this.ClanInfo.Add(new StringPairItemVM(GameTexts.FindText("str_LEFT_colon", null).ToString(), this._clan.TotalStrength.ToString("F0"), null));
			GameTexts.SetVariable("LEFT", GameTexts.FindText("str_wealth", null).ToString());
			this.ClanInfo.Add(new StringPairItemVM(GameTexts.FindText("str_LEFT_colon", null).ToString(), CampaignUIHelper.GetClanWealthStatusText(this._clan), null));
			this.IsClanDestroyed = this._clan.IsEliminated;
		}

		public override string GetName()
		{
			return this._clan.Name.ToString();
		}

		public override string GetNavigationBarURL()
		{
			return HyperlinkTexts.GetGenericHyperlinkText("Home", GameTexts.FindText("str_encyclopedia_home", null).ToString()) + " \\ " + HyperlinkTexts.GetGenericHyperlinkText("ListPage-Clans", GameTexts.FindText("str_encyclopedia_clans", null).ToString()) + " \\ " + this.GetName();
		}

		public override void ExecuteSwitchBookmarkedState()
		{
			base.ExecuteSwitchBookmarkedState();
			if (base.IsBookmarked)
			{
				Campaign.Current.EncyclopediaManager.ViewDataTracker.AddEncyclopediaBookmarkToItem(this._clan);
				return;
			}
			Campaign.Current.EncyclopediaManager.ViewDataTracker.RemoveEncyclopediaBookmarkFromItem(this._clan);
		}

		[DataSourceProperty]
		public MBBindingList<StringPairItemVM> ClanInfo
		{
			get
			{
				return this._clanInfo;
			}
			set
			{
				if (value != this._clanInfo)
				{
					this._clanInfo = value;
					base.OnPropertyChangedWithValue<MBBindingList<StringPairItemVM>>(value, "ClanInfo");
				}
			}
		}

		[DataSourceProperty]
		public MBBindingList<HeroVM> Members
		{
			get
			{
				return this._members;
			}
			set
			{
				if (value != this._members)
				{
					this._members = value;
					base.OnPropertyChangedWithValue<MBBindingList<HeroVM>>(value, "Members");
				}
			}
		}

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
		public EncyclopediaFactionVM ParentKingdom
		{
			get
			{
				return this._parentKingdom;
			}
			set
			{
				if (value != this._parentKingdom)
				{
					this._parentKingdom = value;
					base.OnPropertyChangedWithValue<EncyclopediaFactionVM>(value, "ParentKingdom");
				}
			}
		}

		[DataSourceProperty]
		public bool HasParentKingdom
		{
			get
			{
				return this._hasParentKingdom;
			}
			set
			{
				if (value != this._hasParentKingdom)
				{
					this._hasParentKingdom = value;
					base.OnPropertyChangedWithValue(value, "HasParentKingdom");
				}
			}
		}

		[DataSourceProperty]
		public bool IsClanDestroyed
		{
			get
			{
				return this._isClanDestroyed;
			}
			set
			{
				if (value != this._isClanDestroyed)
				{
					this._isClanDestroyed = value;
					base.OnPropertyChangedWithValue(value, "IsClanDestroyed");
				}
			}
		}

		[DataSourceProperty]
		public string DestroyedText
		{
			get
			{
				return this._destroyedText;
			}
			set
			{
				if (value != this._destroyedText)
				{
					this._destroyedText = value;
					base.OnPropertyChangedWithValue<string>(value, "DestroyedText");
				}
			}
		}

		[DataSourceProperty]
		public string PartOfText
		{
			get
			{
				return this._partOfText;
			}
			set
			{
				if (value != this._partOfText)
				{
					this._partOfText = value;
					base.OnPropertyChangedWithValue<string>(value, "PartOfText");
				}
			}
		}

		[DataSourceProperty]
		public string TierText
		{
			get
			{
				return this._tierText;
			}
			set
			{
				if (value != this._tierText)
				{
					this._tierText = value;
					base.OnPropertyChangedWithValue<string>(value, "TierText");
				}
			}
		}

		[DataSourceProperty]
		public string InfoText
		{
			get
			{
				return this._infoText;
			}
			set
			{
				if (value != this._infoText)
				{
					this._infoText = value;
					base.OnPropertyChangedWithValue<string>(value, "InfoText");
				}
			}
		}

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

		[DataSourceProperty]
		public string AlliesText
		{
			get
			{
				return this._alliesText;
			}
			set
			{
				if (value != this._alliesText)
				{
					this._alliesText = value;
					base.OnPropertyChangedWithValue<string>(value, "AlliesText");
				}
			}
		}

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

		private readonly IFaction _faction;

		private readonly Clan _clan;

		private MBBindingList<StringPairItemVM> _clanInfo;

		private MBBindingList<HeroVM> _members;

		private MBBindingList<EncyclopediaFactionVM> _enemies;

		private MBBindingList<EncyclopediaSettlementVM> _settlements;

		private MBBindingList<EncyclopediaHistoryEventVM> _history;

		private HeroVM _leader;

		private ImageIdentifierVM _banner;

		private string _membersText;

		private string _enemiesText;

		private string _alliesText;

		private string _settlementsText;

		private string _villagesText;

		private string _leaderText;

		private string _descriptorText;

		private string _informationText;

		private string _prosperityText;

		private string _strengthText;

		private string _destroyedText;

		private string _partOfText;

		private string _tierText;

		private string _infoText;

		private HintViewModel _prosperityHint;

		private HintViewModel _strengthHint;

		private EncyclopediaFactionVM _parentKingdom;

		private string _nameText;

		private bool _hasParentKingdom;

		private bool _isClanDestroyed;
	}
}
