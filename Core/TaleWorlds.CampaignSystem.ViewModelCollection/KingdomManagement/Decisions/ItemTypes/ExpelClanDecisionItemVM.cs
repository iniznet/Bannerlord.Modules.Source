using System;
using System.Linq;
using TaleWorlds.CampaignSystem.Election;
using TaleWorlds.CampaignSystem.Encyclopedia;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.ViewModelCollection.Encyclopedia.Items;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.KingdomManagement.Decisions.ItemTypes
{
	// Token: 0x02000069 RID: 105
	public class ExpelClanDecisionItemVM : DecisionItemBaseVM
	{
		// Token: 0x170002DA RID: 730
		// (get) Token: 0x06000923 RID: 2339 RVA: 0x00025F54 File Offset: 0x00024154
		public ExpelClanFromKingdomDecision ExpelDecision
		{
			get
			{
				ExpelClanFromKingdomDecision expelClanFromKingdomDecision;
				if ((expelClanFromKingdomDecision = this._expelDecision) == null)
				{
					expelClanFromKingdomDecision = (this._expelDecision = this._decision as ExpelClanFromKingdomDecision);
				}
				return expelClanFromKingdomDecision;
			}
		}

		// Token: 0x170002DB RID: 731
		// (get) Token: 0x06000924 RID: 2340 RVA: 0x00025F7F File Offset: 0x0002417F
		public Clan Clan
		{
			get
			{
				return this.ExpelDecision.ClanToExpel;
			}
		}

		// Token: 0x06000925 RID: 2341 RVA: 0x00025F8C File Offset: 0x0002418C
		public ExpelClanDecisionItemVM(ExpelClanFromKingdomDecision decision, Action onDecisionOver)
			: base(decision, onDecisionOver)
		{
			base.DecisionType = 2;
		}

		// Token: 0x06000926 RID: 2342 RVA: 0x00025FA0 File Offset: 0x000241A0
		protected override void InitValues()
		{
			base.InitValues();
			base.DecisionType = 2;
			this.Members = new MBBindingList<HeroVM>();
			this.Fiefs = new MBBindingList<EncyclopediaSettlementVM>();
			GameTexts.SetVariable("RENOWN", this.Clan.Renown);
			string text = "STR1";
			TextObject encyclopediaText = this.Clan.EncyclopediaText;
			GameTexts.SetVariable(text, (encyclopediaText != null) ? encyclopediaText.ToString() : null);
			GameTexts.SetVariable("STR2", GameTexts.FindText("str_encyclopedia_renown", null).ToString());
			this.InformationText = GameTexts.FindText("str_STR1_space_STR2", null).ToString();
			this.Leader = new HeroVM(this.Clan.Leader, false);
			this.LeaderText = GameTexts.FindText("str_leader", null).ToString();
			this.MembersText = GameTexts.FindText("str_members", null).ToString();
			this.SettlementsText = GameTexts.FindText("str_fiefs", null).ToString();
			this.NameText = this.Clan.Name.ToString();
			int num = 0;
			float num2 = 0f;
			EncyclopediaPage pageOf = Campaign.Current.EncyclopediaManager.GetPageOf(typeof(Hero));
			foreach (Hero hero in this.Clan.Heroes)
			{
				if (hero.IsAlive && hero.Age >= 18f && pageOf.IsValidEncyclopediaItem(hero))
				{
					if (hero != this.Leader.Hero)
					{
						this.Members.Add(new HeroVM(hero, false));
					}
					num += hero.Gold;
				}
			}
			foreach (Hero hero2 in this.Clan.Companions)
			{
				if (hero2.IsAlive && hero2.Age >= 18f && pageOf.IsValidEncyclopediaItem(hero2))
				{
					if (hero2 != this.Leader.Hero)
					{
						this.Members.Add(new HeroVM(hero2, false));
					}
					num += hero2.Gold;
				}
			}
			foreach (MobileParty mobileParty in MobileParty.AllLordParties)
			{
				if (mobileParty.ActualClan == this.Clan && !mobileParty.IsDisbanding)
				{
					num2 += mobileParty.Party.TotalStrength;
				}
			}
			this.ProsperityText = num.ToString();
			this.ProsperityHint = new BasicTooltipViewModel(() => CampaignUIHelper.GetClanProsperityTooltip(this.Clan));
			this.StrengthText = num2.ToString();
			this.StrengthHint = new BasicTooltipViewModel(() => CampaignUIHelper.GetClanStrengthTooltip(this.Clan));
			foreach (Town town in from s in this.Clan.Fiefs
				orderby s.IsCastle, s.IsTown
				select s)
			{
				if (town.Settlement.OwnerClan == this.Clan)
				{
					this.Fiefs.Add(new EncyclopediaSettlementVM(town.Settlement));
				}
			}
		}

		// Token: 0x170002DC RID: 732
		// (get) Token: 0x06000927 RID: 2343 RVA: 0x00026348 File Offset: 0x00024548
		// (set) Token: 0x06000928 RID: 2344 RVA: 0x00026350 File Offset: 0x00024550
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

		// Token: 0x170002DD RID: 733
		// (get) Token: 0x06000929 RID: 2345 RVA: 0x0002636E File Offset: 0x0002456E
		// (set) Token: 0x0600092A RID: 2346 RVA: 0x00026376 File Offset: 0x00024576
		[DataSourceProperty]
		public MBBindingList<EncyclopediaSettlementVM> Fiefs
		{
			get
			{
				return this._fiefs;
			}
			set
			{
				if (value != this._fiefs)
				{
					this._fiefs = value;
					base.OnPropertyChangedWithValue<MBBindingList<EncyclopediaSettlementVM>>(value, "Fiefs");
				}
			}
		}

		// Token: 0x170002DE RID: 734
		// (get) Token: 0x0600092B RID: 2347 RVA: 0x00026394 File Offset: 0x00024594
		// (set) Token: 0x0600092C RID: 2348 RVA: 0x0002639C File Offset: 0x0002459C
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

		// Token: 0x170002DF RID: 735
		// (get) Token: 0x0600092D RID: 2349 RVA: 0x000263BA File Offset: 0x000245BA
		// (set) Token: 0x0600092E RID: 2350 RVA: 0x000263C2 File Offset: 0x000245C2
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

		// Token: 0x170002E0 RID: 736
		// (get) Token: 0x0600092F RID: 2351 RVA: 0x000263E5 File Offset: 0x000245E5
		// (set) Token: 0x06000930 RID: 2352 RVA: 0x000263ED File Offset: 0x000245ED
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

		// Token: 0x170002E1 RID: 737
		// (get) Token: 0x06000931 RID: 2353 RVA: 0x00026410 File Offset: 0x00024610
		// (set) Token: 0x06000932 RID: 2354 RVA: 0x00026418 File Offset: 0x00024618
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

		// Token: 0x170002E2 RID: 738
		// (get) Token: 0x06000933 RID: 2355 RVA: 0x0002643B File Offset: 0x0002463B
		// (set) Token: 0x06000934 RID: 2356 RVA: 0x00026443 File Offset: 0x00024643
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

		// Token: 0x170002E3 RID: 739
		// (get) Token: 0x06000935 RID: 2357 RVA: 0x00026466 File Offset: 0x00024666
		// (set) Token: 0x06000936 RID: 2358 RVA: 0x0002646E File Offset: 0x0002466E
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

		// Token: 0x170002E4 RID: 740
		// (get) Token: 0x06000937 RID: 2359 RVA: 0x00026491 File Offset: 0x00024691
		// (set) Token: 0x06000938 RID: 2360 RVA: 0x00026499 File Offset: 0x00024699
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

		// Token: 0x170002E5 RID: 741
		// (get) Token: 0x06000939 RID: 2361 RVA: 0x000264BC File Offset: 0x000246BC
		// (set) Token: 0x0600093A RID: 2362 RVA: 0x000264C4 File Offset: 0x000246C4
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

		// Token: 0x170002E6 RID: 742
		// (get) Token: 0x0600093B RID: 2363 RVA: 0x000264E7 File Offset: 0x000246E7
		// (set) Token: 0x0600093C RID: 2364 RVA: 0x000264EF File Offset: 0x000246EF
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

		// Token: 0x170002E7 RID: 743
		// (get) Token: 0x0600093D RID: 2365 RVA: 0x0002650D File Offset: 0x0002470D
		// (set) Token: 0x0600093E RID: 2366 RVA: 0x00026515 File Offset: 0x00024715
		[DataSourceProperty]
		public BasicTooltipViewModel StrengthHint
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
					base.OnPropertyChangedWithValue<BasicTooltipViewModel>(value, "StrengthHint");
				}
			}
		}

		// Token: 0x0400041D RID: 1053
		private ExpelClanFromKingdomDecision _expelDecision;

		// Token: 0x0400041E RID: 1054
		private MBBindingList<HeroVM> _members;

		// Token: 0x0400041F RID: 1055
		private MBBindingList<EncyclopediaSettlementVM> _fiefs;

		// Token: 0x04000420 RID: 1056
		private HeroVM _leader;

		// Token: 0x04000421 RID: 1057
		private string _nameText;

		// Token: 0x04000422 RID: 1058
		private string _membersText;

		// Token: 0x04000423 RID: 1059
		private string _settlementsText;

		// Token: 0x04000424 RID: 1060
		private string _leaderText;

		// Token: 0x04000425 RID: 1061
		private string _informationText;

		// Token: 0x04000426 RID: 1062
		private string _prosperityText;

		// Token: 0x04000427 RID: 1063
		private string _strengthText;

		// Token: 0x04000428 RID: 1064
		private BasicTooltipViewModel _prosperityHint;

		// Token: 0x04000429 RID: 1065
		private BasicTooltipViewModel _strengthHint;
	}
}
