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
	public class ExpelClanDecisionItemVM : DecisionItemBaseVM
	{
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

		public Clan Clan
		{
			get
			{
				return this.ExpelDecision.ClanToExpel;
			}
		}

		public ExpelClanDecisionItemVM(ExpelClanFromKingdomDecision decision, Action onDecisionOver)
			: base(decision, onDecisionOver)
		{
			base.DecisionType = 2;
		}

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

		private ExpelClanFromKingdomDecision _expelDecision;

		private MBBindingList<HeroVM> _members;

		private MBBindingList<EncyclopediaSettlementVM> _fiefs;

		private HeroVM _leader;

		private string _nameText;

		private string _membersText;

		private string _settlementsText;

		private string _leaderText;

		private string _informationText;

		private string _prosperityText;

		private string _strengthText;

		private BasicTooltipViewModel _prosperityHint;

		private BasicTooltipViewModel _strengthHint;
	}
}
