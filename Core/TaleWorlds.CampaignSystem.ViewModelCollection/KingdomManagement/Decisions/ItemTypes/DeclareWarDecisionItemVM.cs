using System;
using System.Linq;
using TaleWorlds.CampaignSystem.Election;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.ViewModelCollection.KingdomManagement.Diplomacy;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.KingdomManagement.Decisions.ItemTypes
{
	// Token: 0x02000068 RID: 104
	public class DeclareWarDecisionItemVM : DecisionItemBaseVM
	{
		// Token: 0x170002D0 RID: 720
		// (get) Token: 0x0600090F RID: 2319 RVA: 0x00025B29 File Offset: 0x00023D29
		private Kingdom _sourceFaction
		{
			get
			{
				return Hero.MainHero.Clan.Kingdom;
			}
		}

		// Token: 0x170002D1 RID: 721
		// (get) Token: 0x06000910 RID: 2320 RVA: 0x00025B3A File Offset: 0x00023D3A
		public IFaction TargetFaction
		{
			get
			{
				return (this._decision as DeclareWarDecision).FactionToDeclareWarOn;
			}
		}

		// Token: 0x06000911 RID: 2321 RVA: 0x00025B4C File Offset: 0x00023D4C
		public DeclareWarDecisionItemVM(DeclareWarDecision decision, Action onDecisionOver)
			: base(decision, onDecisionOver)
		{
			this._declareWarDecision = decision;
			base.DecisionType = 4;
		}

		// Token: 0x06000912 RID: 2322 RVA: 0x00025B64 File Offset: 0x00023D64
		protected override void InitValues()
		{
			base.InitValues();
			TextObject textObject = GameTexts.FindText("str_kingdom_decision_declare_war", null);
			this.NameText = textObject.ToString();
			TextObject textObject2 = GameTexts.FindText("str_kingdom_decision_declare_war_desc", null);
			textObject2.SetTextVariable("FACTION", this.TargetFaction.Name);
			this.WarDescriptionText = textObject2.ToString();
			this.SourceFactionBanner = new ImageIdentifierVM(BannerCode.CreateFrom(this._sourceFaction.Banner), true);
			this.TargetFactionBanner = new ImageIdentifierVM(BannerCode.CreateFrom(this.TargetFaction.Banner), true);
			this.LeaderText = GameTexts.FindText("str_leader", null).ToString();
			this.SourceFactionLeader = new HeroVM(this._sourceFaction.Leader, false);
			this.TargetFactionLeader = new HeroVM(this.TargetFaction.Leader, false);
			this.ComparedStats = new MBBindingList<KingdomWarComparableStatVM>();
			Kingdom kingdom = this.TargetFaction as Kingdom;
			string text = Color.FromUint(this._sourceFaction.Color).ToString();
			string text2 = Color.FromUint(kingdom.Color).ToString();
			KingdomWarComparableStatVM kingdomWarComparableStatVM = new KingdomWarComparableStatVM((int)this._sourceFaction.TotalStrength, (int)kingdom.TotalStrength, GameTexts.FindText("str_strength", null), text, text2, 10000, null, null);
			this.ComparedStats.Add(kingdomWarComparableStatVM);
			KingdomWarComparableStatVM kingdomWarComparableStatVM2 = new KingdomWarComparableStatVM(this._sourceFaction.Armies.Count, kingdom.Armies.Count, GameTexts.FindText("str_armies", null), text, text2, 5, null, null);
			this.ComparedStats.Add(kingdomWarComparableStatVM2);
			int num = this._sourceFaction.Settlements.Count((Settlement settlement) => settlement.IsTown);
			int num2 = kingdom.Settlements.Count((Settlement settlement) => settlement.IsTown);
			KingdomWarComparableStatVM kingdomWarComparableStatVM3 = new KingdomWarComparableStatVM(num, num2, GameTexts.FindText("str_towns", null), text, text2, 50, null, null);
			this.ComparedStats.Add(kingdomWarComparableStatVM3);
			int num3 = this._sourceFaction.Settlements.Count((Settlement settlement) => settlement.IsCastle);
			int num4 = this.TargetFaction.Settlements.Count((Settlement settlement) => settlement.IsCastle);
			KingdomWarComparableStatVM kingdomWarComparableStatVM4 = new KingdomWarComparableStatVM(num3, num4, GameTexts.FindText("str_castles", null), text, text2, 50, null, null);
			this.ComparedStats.Add(kingdomWarComparableStatVM4);
		}

		// Token: 0x170002D2 RID: 722
		// (get) Token: 0x06000913 RID: 2323 RVA: 0x00025E14 File Offset: 0x00024014
		// (set) Token: 0x06000914 RID: 2324 RVA: 0x00025E1C File Offset: 0x0002401C
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

		// Token: 0x170002D3 RID: 723
		// (get) Token: 0x06000915 RID: 2325 RVA: 0x00025E3F File Offset: 0x0002403F
		// (set) Token: 0x06000916 RID: 2326 RVA: 0x00025E47 File Offset: 0x00024047
		[DataSourceProperty]
		public string WarDescriptionText
		{
			get
			{
				return this._warDescriptionText;
			}
			set
			{
				if (value != this._warDescriptionText)
				{
					this._warDescriptionText = value;
					base.OnPropertyChangedWithValue<string>(value, "WarDescriptionText");
				}
			}
		}

		// Token: 0x170002D4 RID: 724
		// (get) Token: 0x06000917 RID: 2327 RVA: 0x00025E6A File Offset: 0x0002406A
		// (set) Token: 0x06000918 RID: 2328 RVA: 0x00025E72 File Offset: 0x00024072
		[DataSourceProperty]
		public ImageIdentifierVM SourceFactionBanner
		{
			get
			{
				return this._sourceFactionBanner;
			}
			set
			{
				if (value != this._sourceFactionBanner)
				{
					this._sourceFactionBanner = value;
					base.OnPropertyChangedWithValue<ImageIdentifierVM>(value, "SourceFactionBanner");
				}
			}
		}

		// Token: 0x170002D5 RID: 725
		// (get) Token: 0x06000919 RID: 2329 RVA: 0x00025E90 File Offset: 0x00024090
		// (set) Token: 0x0600091A RID: 2330 RVA: 0x00025E98 File Offset: 0x00024098
		[DataSourceProperty]
		public ImageIdentifierVM TargetFactionBanner
		{
			get
			{
				return this._targetFactionBanner;
			}
			set
			{
				if (value != this._targetFactionBanner)
				{
					this._targetFactionBanner = value;
					base.OnPropertyChangedWithValue<ImageIdentifierVM>(value, "TargetFactionBanner");
				}
			}
		}

		// Token: 0x170002D6 RID: 726
		// (get) Token: 0x0600091B RID: 2331 RVA: 0x00025EB6 File Offset: 0x000240B6
		// (set) Token: 0x0600091C RID: 2332 RVA: 0x00025EBE File Offset: 0x000240BE
		[DataSourceProperty]
		public MBBindingList<KingdomWarComparableStatVM> ComparedStats
		{
			get
			{
				return this._comparedStats;
			}
			set
			{
				if (value != this._comparedStats)
				{
					this._comparedStats = value;
					base.OnPropertyChangedWithValue<MBBindingList<KingdomWarComparableStatVM>>(value, "ComparedStats");
				}
			}
		}

		// Token: 0x170002D7 RID: 727
		// (get) Token: 0x0600091D RID: 2333 RVA: 0x00025EDC File Offset: 0x000240DC
		// (set) Token: 0x0600091E RID: 2334 RVA: 0x00025EE4 File Offset: 0x000240E4
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

		// Token: 0x170002D8 RID: 728
		// (get) Token: 0x0600091F RID: 2335 RVA: 0x00025F07 File Offset: 0x00024107
		// (set) Token: 0x06000920 RID: 2336 RVA: 0x00025F0F File Offset: 0x0002410F
		[DataSourceProperty]
		public HeroVM SourceFactionLeader
		{
			get
			{
				return this._sourceFactionLeader;
			}
			set
			{
				if (value != this._sourceFactionLeader)
				{
					this._sourceFactionLeader = value;
					base.OnPropertyChangedWithValue<HeroVM>(value, "SourceFactionLeader");
				}
			}
		}

		// Token: 0x170002D9 RID: 729
		// (get) Token: 0x06000921 RID: 2337 RVA: 0x00025F2D File Offset: 0x0002412D
		// (set) Token: 0x06000922 RID: 2338 RVA: 0x00025F35 File Offset: 0x00024135
		[DataSourceProperty]
		public HeroVM TargetFactionLeader
		{
			get
			{
				return this._targetFactionLeader;
			}
			set
			{
				if (value != this._targetFactionLeader)
				{
					this._targetFactionLeader = value;
					base.OnPropertyChangedWithValue<HeroVM>(value, "TargetFactionLeader");
				}
			}
		}

		// Token: 0x04000414 RID: 1044
		private readonly DeclareWarDecision _declareWarDecision;

		// Token: 0x04000415 RID: 1045
		private string _nameText;

		// Token: 0x04000416 RID: 1046
		private string _warDescriptionText;

		// Token: 0x04000417 RID: 1047
		private ImageIdentifierVM _sourceFactionBanner;

		// Token: 0x04000418 RID: 1048
		private ImageIdentifierVM _targetFactionBanner;

		// Token: 0x04000419 RID: 1049
		private string _leaderText;

		// Token: 0x0400041A RID: 1050
		private HeroVM _sourceFactionLeader;

		// Token: 0x0400041B RID: 1051
		private HeroVM _targetFactionLeader;

		// Token: 0x0400041C RID: 1052
		private MBBindingList<KingdomWarComparableStatVM> _comparedStats;
	}
}
