using System;
using System.Linq;
using TaleWorlds.CampaignSystem.Election;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.ViewModelCollection.KingdomManagement.Diplomacy;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.KingdomManagement.Decisions.ItemTypes
{
	// Token: 0x0200006C RID: 108
	public class MakePeaceDecisionItemVM : DecisionItemBaseVM
	{
		// Token: 0x170002F4 RID: 756
		// (get) Token: 0x0600095C RID: 2396 RVA: 0x00026A99 File Offset: 0x00024C99
		private Kingdom _sourceFaction
		{
			get
			{
				return Hero.MainHero.Clan.Kingdom;
			}
		}

		// Token: 0x170002F5 RID: 757
		// (get) Token: 0x0600095D RID: 2397 RVA: 0x00026AAA File Offset: 0x00024CAA
		public IFaction TargetFaction
		{
			get
			{
				return (this._decision as MakePeaceKingdomDecision).FactionToMakePeaceWith;
			}
		}

		// Token: 0x0600095E RID: 2398 RVA: 0x00026ABC File Offset: 0x00024CBC
		public MakePeaceDecisionItemVM(MakePeaceKingdomDecision decision, Action onDecisionOver)
			: base(decision, onDecisionOver)
		{
			this._makePeaceDecision = decision;
			base.DecisionType = 5;
		}

		// Token: 0x0600095F RID: 2399 RVA: 0x00026AD4 File Offset: 0x00024CD4
		protected override void InitValues()
		{
			base.InitValues();
			TextObject textObject = GameTexts.FindText("str_kingdom_decision_make_peace", null);
			this.NameText = textObject.ToString();
			TextObject textObject2 = GameTexts.FindText("str_kingdom_decision_make_peace_desc", null);
			textObject2.SetTextVariable("FACTION", this.TargetFaction.Name);
			this.PeaceDescriptionText = textObject2.ToString();
			this.SourceFactionBanner = new ImageIdentifierVM(BannerCode.CreateFrom(this._sourceFaction.Banner), true);
			this.TargetFactionBanner = new ImageIdentifierVM(BannerCode.CreateFrom(this.TargetFaction.Banner), true);
			this.LeaderText = GameTexts.FindText("str_leader", null).ToString();
			this.SourceFactionLeader = new HeroVM(this._sourceFaction.Leader, false);
			this.TargetFactionLeader = new HeroVM(this.TargetFaction.Leader, false);
			this.ComparedStats = new MBBindingList<KingdomWarComparableStatVM>();
			Kingdom targetFaction = this.TargetFaction as Kingdom;
			string text = Color.FromUint(this._sourceFaction.Color).ToString();
			string text2 = Color.FromUint(targetFaction.Color).ToString();
			StanceLink stanceLink = this._sourceFaction.Stances.First((StanceLink s) => s.IsAtWar && (s.Faction2 == this.TargetFaction || s.Faction1 == this.TargetFaction));
			KingdomWarComparableStatVM kingdomWarComparableStatVM = new KingdomWarComparableStatVM((int)this._sourceFaction.TotalStrength, (int)targetFaction.TotalStrength, GameTexts.FindText("str_strength", null), text, text2, 10000, null, null);
			this.ComparedStats.Add(kingdomWarComparableStatVM);
			int num = targetFaction.Heroes.Count(delegate(Hero x)
			{
				if (x.IsPrisoner)
				{
					PartyBase partyBelongedToAsPrisoner = x.PartyBelongedToAsPrisoner;
					return ((partyBelongedToAsPrisoner != null) ? partyBelongedToAsPrisoner.MapFaction : null) == this._sourceFaction;
				}
				return false;
			});
			int num2 = this._sourceFaction.Heroes.Count(delegate(Hero x)
			{
				if (x.IsPrisoner)
				{
					PartyBase partyBelongedToAsPrisoner2 = x.PartyBelongedToAsPrisoner;
					return ((partyBelongedToAsPrisoner2 != null) ? partyBelongedToAsPrisoner2.MapFaction : null) == targetFaction;
				}
				return false;
			});
			KingdomWarComparableStatVM kingdomWarComparableStatVM2 = new KingdomWarComparableStatVM(num, num2, GameTexts.FindText("str_party_category_prisoners_tooltip", null), text, text2, 10, null, null);
			this.ComparedStats.Add(kingdomWarComparableStatVM2);
			KingdomWarComparableStatVM kingdomWarComparableStatVM3 = new KingdomWarComparableStatVM(stanceLink.GetCasualties(targetFaction), stanceLink.GetCasualties(this._sourceFaction), GameTexts.FindText("str_war_casualties_inflicted", null), text, text2, 5000, null, null);
			this.ComparedStats.Add(kingdomWarComparableStatVM3);
			KingdomWarComparableStatVM kingdomWarComparableStatVM4 = new KingdomWarComparableStatVM(stanceLink.GetSuccessfulSieges(this._sourceFaction), stanceLink.GetSuccessfulSieges(targetFaction), GameTexts.FindText("str_war_successful_sieges", null), text, text2, 5, null, null);
			this.ComparedStats.Add(kingdomWarComparableStatVM4);
			KingdomWarComparableStatVM kingdomWarComparableStatVM5 = new KingdomWarComparableStatVM(stanceLink.GetSuccessfulRaids(this._sourceFaction), stanceLink.GetSuccessfulRaids(targetFaction), GameTexts.FindText("str_war_successful_raids", null), text, text2, 10, null, null);
			this.ComparedStats.Add(kingdomWarComparableStatVM5);
		}

		// Token: 0x170002F6 RID: 758
		// (get) Token: 0x06000960 RID: 2400 RVA: 0x00026D97 File Offset: 0x00024F97
		// (set) Token: 0x06000961 RID: 2401 RVA: 0x00026D9F File Offset: 0x00024F9F
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

		// Token: 0x170002F7 RID: 759
		// (get) Token: 0x06000962 RID: 2402 RVA: 0x00026DC2 File Offset: 0x00024FC2
		// (set) Token: 0x06000963 RID: 2403 RVA: 0x00026DCA File Offset: 0x00024FCA
		[DataSourceProperty]
		public string PeaceDescriptionText
		{
			get
			{
				return this._peaceDescriptionText;
			}
			set
			{
				if (value != this._peaceDescriptionText)
				{
					this._peaceDescriptionText = value;
					base.OnPropertyChangedWithValue<string>(value, "PeaceDescriptionText");
				}
			}
		}

		// Token: 0x170002F8 RID: 760
		// (get) Token: 0x06000964 RID: 2404 RVA: 0x00026DED File Offset: 0x00024FED
		// (set) Token: 0x06000965 RID: 2405 RVA: 0x00026DF5 File Offset: 0x00024FF5
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

		// Token: 0x170002F9 RID: 761
		// (get) Token: 0x06000966 RID: 2406 RVA: 0x00026E13 File Offset: 0x00025013
		// (set) Token: 0x06000967 RID: 2407 RVA: 0x00026E1B File Offset: 0x0002501B
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

		// Token: 0x170002FA RID: 762
		// (get) Token: 0x06000968 RID: 2408 RVA: 0x00026E39 File Offset: 0x00025039
		// (set) Token: 0x06000969 RID: 2409 RVA: 0x00026E41 File Offset: 0x00025041
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

		// Token: 0x170002FB RID: 763
		// (get) Token: 0x0600096A RID: 2410 RVA: 0x00026E5F File Offset: 0x0002505F
		// (set) Token: 0x0600096B RID: 2411 RVA: 0x00026E67 File Offset: 0x00025067
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

		// Token: 0x170002FC RID: 764
		// (get) Token: 0x0600096C RID: 2412 RVA: 0x00026E8A File Offset: 0x0002508A
		// (set) Token: 0x0600096D RID: 2413 RVA: 0x00026E92 File Offset: 0x00025092
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

		// Token: 0x170002FD RID: 765
		// (get) Token: 0x0600096E RID: 2414 RVA: 0x00026EB0 File Offset: 0x000250B0
		// (set) Token: 0x0600096F RID: 2415 RVA: 0x00026EB8 File Offset: 0x000250B8
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

		// Token: 0x04000436 RID: 1078
		private readonly MakePeaceKingdomDecision _makePeaceDecision;

		// Token: 0x04000437 RID: 1079
		private string _nameText;

		// Token: 0x04000438 RID: 1080
		private string _peaceDescriptionText;

		// Token: 0x04000439 RID: 1081
		private ImageIdentifierVM _sourceFactionBanner;

		// Token: 0x0400043A RID: 1082
		private ImageIdentifierVM _targetFactionBanner;

		// Token: 0x0400043B RID: 1083
		private string _leaderText;

		// Token: 0x0400043C RID: 1084
		private HeroVM _sourceFactionLeader;

		// Token: 0x0400043D RID: 1085
		private HeroVM _targetFactionLeader;

		// Token: 0x0400043E RID: 1086
		private MBBindingList<KingdomWarComparableStatVM> _comparedStats;
	}
}
