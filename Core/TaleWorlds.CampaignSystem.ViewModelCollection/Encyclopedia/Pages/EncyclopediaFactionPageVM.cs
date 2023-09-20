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
	[EncyclopediaViewModel(typeof(Kingdom))]
	public class EncyclopediaFactionPageVM : EncyclopediaContentPageVM
	{
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

		public override string GetName()
		{
			return this._faction.Name.ToString();
		}

		public override string GetNavigationBarURL()
		{
			return HyperlinkTexts.GetGenericHyperlinkText("Home", GameTexts.FindText("str_encyclopedia_home", null).ToString()) + " \\ " + HyperlinkTexts.GetGenericHyperlinkText("ListPage-Kingdoms", GameTexts.FindText("str_encyclopedia_kingdoms", null).ToString()) + " \\ " + this.GetName();
		}

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

		private Kingdom _faction;

		private MBBindingList<EncyclopediaFactionVM> _clans;

		private MBBindingList<EncyclopediaFactionVM> _enemies;

		private MBBindingList<EncyclopediaSettlementVM> _settlements;

		private MBBindingList<EncyclopediaHistoryEventVM> _history;

		private HeroVM _leader;

		private ImageIdentifierVM _banner;

		private string _membersText;

		private string _enemiesText;

		private string _clansText;

		private string _settlementsText;

		private string _villagesText;

		private string _leaderText;

		private string _descriptorText;

		private string _prosperityText;

		private string _strengthText;

		private string _informationText;

		private HintViewModel _prosperityHint;

		private HintViewModel _strengthHint;

		private string _nameText;
	}
}
