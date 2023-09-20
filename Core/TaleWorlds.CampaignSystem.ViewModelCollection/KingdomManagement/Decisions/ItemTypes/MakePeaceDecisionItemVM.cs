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
	public class MakePeaceDecisionItemVM : DecisionItemBaseVM
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
				return (this._decision as MakePeaceKingdomDecision).FactionToMakePeaceWith;
			}
		}

		public MakePeaceDecisionItemVM(MakePeaceKingdomDecision decision, Action onDecisionOver)
			: base(decision, onDecisionOver)
		{
			this._makePeaceDecision = decision;
			base.DecisionType = 5;
		}

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

		private readonly MakePeaceKingdomDecision _makePeaceDecision;

		private string _nameText;

		private string _peaceDescriptionText;

		private ImageIdentifierVM _sourceFactionBanner;

		private ImageIdentifierVM _targetFactionBanner;

		private string _leaderText;

		private HeroVM _sourceFactionLeader;

		private HeroVM _targetFactionLeader;

		private MBBindingList<KingdomWarComparableStatVM> _comparedStats;
	}
}
