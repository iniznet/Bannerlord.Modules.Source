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
	public class DeclareWarDecisionItemVM : DecisionItemBaseVM
	{
		private Kingdom _sourceFaction
		{
			get
			{
				return Hero.MainHero.Clan.Kingdom;
			}
		}

		public IFaction TargetFaction
		{
			get
			{
				return (this._decision as DeclareWarDecision).FactionToDeclareWarOn;
			}
		}

		public DeclareWarDecisionItemVM(DeclareWarDecision decision, Action onDecisionOver)
			: base(decision, onDecisionOver)
		{
			this._declareWarDecision = decision;
			base.DecisionType = 4;
		}

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

		private readonly DeclareWarDecision _declareWarDecision;

		private string _nameText;

		private string _warDescriptionText;

		private ImageIdentifierVM _sourceFactionBanner;

		private ImageIdentifierVM _targetFactionBanner;

		private string _leaderText;

		private HeroVM _sourceFactionLeader;

		private HeroVM _targetFactionLeader;

		private MBBindingList<KingdomWarComparableStatVM> _comparedStats;
	}
}
