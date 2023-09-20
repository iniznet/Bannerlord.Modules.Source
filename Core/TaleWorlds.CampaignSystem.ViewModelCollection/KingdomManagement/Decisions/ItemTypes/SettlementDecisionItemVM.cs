using System;
using TaleWorlds.CampaignSystem.Election;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.ViewModelCollection.Encyclopedia.Items;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.KingdomManagement.Decisions.ItemTypes
{
	public class SettlementDecisionItemVM : DecisionItemBaseVM
	{
		public Settlement Settlement
		{
			get
			{
				if (this._settlementDecision == null && this._settlementPreliminaryDecision == null)
				{
					SettlementClaimantDecision settlementClaimantDecision;
					SettlementClaimantPreliminaryDecision settlementClaimantPreliminaryDecision;
					if ((settlementClaimantDecision = this._decision as SettlementClaimantDecision) != null)
					{
						this._settlementDecision = settlementClaimantDecision;
					}
					else if ((settlementClaimantPreliminaryDecision = this._decision as SettlementClaimantPreliminaryDecision) != null)
					{
						this._settlementPreliminaryDecision = settlementClaimantPreliminaryDecision;
					}
				}
				if (this._settlementDecision == null)
				{
					return this._settlementPreliminaryDecision.Settlement;
				}
				return this._settlementDecision.Settlement;
			}
		}

		public SettlementDecisionItemVM(Settlement settlement, KingdomDecision decision, Action onDecisionOver)
			: base(decision, onDecisionOver)
		{
			this._settlement = settlement;
			base.DecisionType = 1;
		}

		protected override void InitValues()
		{
			base.InitValues();
			base.DecisionType = 1;
			this.SettlementImageID = ((this.Settlement.SettlementComponent != null) ? this.Settlement.SettlementComponent.WaitMeshName : "");
			this.BoundVillages = new MBBindingList<EncyclopediaSettlementVM>();
			this.NotableCharacters = new MBBindingList<HeroVM>();
			this.SettlementName = this.Settlement.Name.ToString();
			Town town = this.Settlement.Town;
			this.Governor = new HeroVM((town != null) ? town.Governor : null, false);
			foreach (Village village in this.Settlement.BoundVillages)
			{
				this.BoundVillages.Add(new EncyclopediaSettlementVM(village.Settlement));
			}
			Town town2 = this.Settlement.Town;
			this.WallsText = town2.GetWallLevel().ToString();
			this.WallsHint = new BasicTooltipViewModel(() => CampaignUIHelper.GetTownWallsTooltip(this.Settlement.Town));
			this.HasNotables = this.Settlement.Notables.Count > 0;
			if (!this.Settlement.IsCastle)
			{
				Campaign.Current.EncyclopediaManager.GetPageOf(typeof(Hero));
				foreach (Hero hero in this.Settlement.Notables)
				{
					this.NotableCharacters.Add(new HeroVM(hero, false));
				}
			}
			this.DescriptorText = this.Settlement.Culture.Name.ToString();
			this.DetailsText = GameTexts.FindText("str_people_encyclopedia_details", null).ToString();
			this.OwnerText = GameTexts.FindText("str_owner", null).ToString();
			this.Owner = new HeroVM(this.Settlement.OwnerClan.Leader, false);
			SettlementComponent settlementComponent = this.Settlement.SettlementComponent;
			this.SettlementPath = settlementComponent.BackgroundMeshName;
			this.SettlementCropPosition = (double)settlementComponent.BackgroundCropPosition;
			this.NotableCharactersText = GameTexts.FindText("str_notable_characters", null).ToString();
			this.BoundSettlementText = GameTexts.FindText("str_villages", null).ToString();
			if (this.HasBoundSettlement)
			{
				GameTexts.SetVariable("SETTLEMENT_LINK", this.Settlement.Village.Bound.EncyclopediaLinkWithName);
				this.BoundSettlementText = GameTexts.FindText("str_bound_settlement_encyclopedia", null).ToString();
			}
			this.MilitasText = ((int)this.Settlement.Militia).ToString();
			this.MilitasHint = new BasicTooltipViewModel(() => CampaignUIHelper.GetTownMilitiaTooltip(this.Settlement.Town));
			this.ProsperityText = (this.Settlement.IsTown ? ((int)this.Settlement.Town.Prosperity).ToString() : ((int)this.Settlement.Prosperity).ToString());
			if (this.Settlement.IsTown)
			{
				this.ProsperityHint = new BasicTooltipViewModel(() => CampaignUIHelper.GetTownProsperityTooltip(this.Settlement.Town));
			}
			else
			{
				this.ProsperityHint = new BasicTooltipViewModel(() => GameTexts.FindText("str_prosperity", null).ToString());
			}
			if (this.Settlement.Town != null)
			{
				this.LoyaltyHint = new BasicTooltipViewModel(() => CampaignUIHelper.GetTownLoyaltyTooltip(this.Settlement.Town));
				this.LoyaltyText = string.Format("{0:0.#}", this.Settlement.Town.Loyalty);
				this.SecurityHint = new BasicTooltipViewModel(() => CampaignUIHelper.GetTownSecurityTooltip(this.Settlement.Town));
				this.SecurityText = string.Format("{0:0.#}", this.Settlement.Town.Security);
			}
			else
			{
				this.LoyaltyText = "-";
				this.SecurityText = "-";
			}
			Town town3 = this.Settlement.Town;
			this.FoodText = ((town3 != null) ? town3.FoodStocks.ToString("0.0") : null);
			if (this.Settlement.IsFortification)
			{
				this.FoodHint = new BasicTooltipViewModel(() => CampaignUIHelper.GetTownFoodTooltip(this.Settlement.Town));
				MobileParty garrisonParty = this.Settlement.Town.GarrisonParty;
				this.GarrisonText = ((garrisonParty != null) ? garrisonParty.Party.NumberOfAllMembers.ToString() : null) ?? "0";
				this.GarrisonHint = new BasicTooltipViewModel(() => CampaignUIHelper.GetTownGarrisonTooltip(this.Settlement.Town));
				return;
			}
			this.FoodHint = new BasicTooltipViewModel();
			this.GarrisonText = "-";
		}

		[DataSourceProperty]
		public bool HasBoundSettlement
		{
			get
			{
				return this._hasBoundSettlement;
			}
			set
			{
				if (value != this._hasBoundSettlement)
				{
					this._hasBoundSettlement = value;
					base.OnPropertyChangedWithValue(value, "HasBoundSettlement");
				}
			}
		}

		[DataSourceProperty]
		public double SettlementCropPosition
		{
			get
			{
				return this._settlementCropPosition;
			}
			set
			{
				if (value != this._settlementCropPosition)
				{
					this._settlementCropPosition = value;
					base.OnPropertyChangedWithValue(value, "SettlementCropPosition");
				}
			}
		}

		[DataSourceProperty]
		public string BoundSettlementText
		{
			get
			{
				return this._boundSettlementText;
			}
			set
			{
				if (value != this._boundSettlementText)
				{
					this._boundSettlementText = value;
					base.OnPropertyChangedWithValue<string>(value, "BoundSettlementText");
				}
			}
		}

		[DataSourceProperty]
		public string DetailsText
		{
			get
			{
				return this._detailsText;
			}
			set
			{
				if (value != this._detailsText)
				{
					this._detailsText = value;
					base.OnPropertyChangedWithValue<string>(value, "DetailsText");
				}
			}
		}

		[DataSourceProperty]
		public string SettlementPath
		{
			get
			{
				return this._settlementPath;
			}
			set
			{
				if (value != this._settlementPath)
				{
					this._settlementPath = value;
					base.OnPropertyChangedWithValue<string>(value, "SettlementPath");
				}
			}
		}

		[DataSourceProperty]
		public string SettlementName
		{
			get
			{
				return this._settlementName;
			}
			set
			{
				if (value != this._settlementName)
				{
					this._settlementName = value;
					base.OnPropertyChangedWithValue<string>(value, "SettlementName");
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
		public HeroVM Owner
		{
			get
			{
				return this._owner;
			}
			set
			{
				if (value != this._owner)
				{
					this._owner = value;
					base.OnPropertyChangedWithValue<HeroVM>(value, "Owner");
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
		public string SettlementImageID
		{
			get
			{
				return this._settlementImageID;
			}
			set
			{
				if (value != this._settlementImageID)
				{
					this._settlementImageID = value;
					base.OnPropertyChangedWithValue<string>(value, "SettlementImageID");
				}
			}
		}

		[DataSourceProperty]
		public string NotableCharactersText
		{
			get
			{
				return this._notableCharactersText;
			}
			set
			{
				if (value != this._notableCharactersText)
				{
					this._notableCharactersText = value;
					base.OnPropertyChangedWithValue<string>(value, "NotableCharactersText");
				}
			}
		}

		[DataSourceProperty]
		public MBBindingList<EncyclopediaSettlementVM> BoundVillages
		{
			get
			{
				return this._boundVillages;
			}
			set
			{
				if (value != this._boundVillages)
				{
					this._boundVillages = value;
					base.OnPropertyChangedWithValue<MBBindingList<EncyclopediaSettlementVM>>(value, "BoundVillages");
				}
			}
		}

		[DataSourceProperty]
		public MBBindingList<HeroVM> NotableCharacters
		{
			get
			{
				return this._notableCharacters;
			}
			set
			{
				if (value != this._notableCharacters)
				{
					this._notableCharacters = value;
					base.OnPropertyChangedWithValue<MBBindingList<HeroVM>>(value, "NotableCharacters");
				}
			}
		}

		[DataSourceProperty]
		public BasicTooltipViewModel MilitasHint
		{
			get
			{
				return this._militasHint;
			}
			set
			{
				if (value != this._militasHint)
				{
					this._militasHint = value;
					base.OnPropertyChangedWithValue<BasicTooltipViewModel>(value, "MilitasHint");
				}
			}
		}

		[DataSourceProperty]
		public BasicTooltipViewModel FoodHint
		{
			get
			{
				return this._foodHint;
			}
			set
			{
				if (value != this._foodHint)
				{
					this._foodHint = value;
					base.OnPropertyChangedWithValue<BasicTooltipViewModel>(value, "FoodHint");
				}
			}
		}

		[DataSourceProperty]
		public BasicTooltipViewModel GarrisonHint
		{
			get
			{
				return this._garrisonHint;
			}
			set
			{
				if (value != this._garrisonHint)
				{
					this._garrisonHint = value;
					base.OnPropertyChangedWithValue<BasicTooltipViewModel>(value, "GarrisonHint");
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
		public BasicTooltipViewModel LoyaltyHint
		{
			get
			{
				return this._loyaltyHint;
			}
			set
			{
				if (value != this._loyaltyHint)
				{
					this._loyaltyHint = value;
					base.OnPropertyChangedWithValue<BasicTooltipViewModel>(value, "LoyaltyHint");
				}
			}
		}

		[DataSourceProperty]
		public BasicTooltipViewModel SecurityHint
		{
			get
			{
				return this._securityHint;
			}
			set
			{
				if (value != this._securityHint)
				{
					this._securityHint = value;
					base.OnPropertyChangedWithValue<BasicTooltipViewModel>(value, "SecurityHint");
				}
			}
		}

		[DataSourceProperty]
		public BasicTooltipViewModel WallsHint
		{
			get
			{
				return this._wallsHint;
			}
			set
			{
				if (value != this._wallsHint)
				{
					this._wallsHint = value;
					base.OnPropertyChangedWithValue<BasicTooltipViewModel>(value, "WallsHint");
				}
			}
		}

		[DataSourceProperty]
		public string MilitasText
		{
			get
			{
				return this._militasText;
			}
			set
			{
				if (value != this._militasText)
				{
					this._militasText = value;
					base.OnPropertyChangedWithValue<string>(value, "MilitasText");
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
		public string LoyaltyText
		{
			get
			{
				return this._loyaltyText;
			}
			set
			{
				if (value != this._loyaltyText)
				{
					this._loyaltyText = value;
					base.OnPropertyChangedWithValue<string>(value, "LoyaltyText");
				}
			}
		}

		[DataSourceProperty]
		public string SecurityText
		{
			get
			{
				return this._securityText;
			}
			set
			{
				if (value != this._securityText)
				{
					this._securityText = value;
					base.OnPropertyChangedWithValue<string>(value, "SecurityText");
				}
			}
		}

		[DataSourceProperty]
		public string WallsText
		{
			get
			{
				return this._wallsText;
			}
			set
			{
				if (value != this._wallsText)
				{
					this._wallsText = value;
					base.OnPropertyChangedWithValue<string>(value, "WallsText");
				}
			}
		}

		[DataSourceProperty]
		public string FoodText
		{
			get
			{
				return this._foodText;
			}
			set
			{
				if (value != this._foodText)
				{
					this._foodText = value;
					base.OnPropertyChangedWithValue<string>(value, "FoodText");
				}
			}
		}

		[DataSourceProperty]
		public string GarrisonText
		{
			get
			{
				return this._garrisonText;
			}
			set
			{
				if (value != this._garrisonText)
				{
					this._garrisonText = value;
					base.OnPropertyChangedWithValue<string>(value, "GarrisonText");
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
		public string OwnerText
		{
			get
			{
				return this._ownerText;
			}
			set
			{
				if (value != this._ownerText)
				{
					this._ownerText = value;
					base.OnPropertyChangedWithValue<string>(value, "OwnerText");
				}
			}
		}

		[DataSourceProperty]
		public HeroVM Governor
		{
			get
			{
				return this._governor;
			}
			set
			{
				if (value != this._governor)
				{
					this._governor = value;
					base.OnPropertyChangedWithValue<HeroVM>(value, "Governor");
				}
			}
		}

		[DataSourceProperty]
		public bool HasNotables
		{
			get
			{
				return this._hasNotables;
			}
			set
			{
				if (value != this._hasNotables)
				{
					this._hasNotables = value;
					base.OnPropertyChangedWithValue(value, "HasNotables");
				}
			}
		}

		private SettlementClaimantDecision _settlementDecision;

		private SettlementClaimantPreliminaryDecision _settlementPreliminaryDecision;

		private Settlement _settlement;

		private string _settlementName;

		private HeroVM _governor;

		private MBBindingList<EncyclopediaSettlementVM> _boundVillages;

		private MBBindingList<HeroVM> _notableCharacters;

		private BasicTooltipViewModel _militasHint;

		private BasicTooltipViewModel _prosperityHint;

		private BasicTooltipViewModel _loyaltyHint;

		private BasicTooltipViewModel _securityHint;

		private BasicTooltipViewModel _wallsHint;

		private BasicTooltipViewModel _garrisonHint;

		private BasicTooltipViewModel _foodHint;

		private HeroVM _owner;

		private string _ownerText;

		private string _militasText;

		private string _garrisonText;

		private string _prosperityText;

		private string _loyaltyText;

		private string _securityText;

		private string _wallsText;

		private string _foodText;

		private string _descriptorText;

		private string _villagesText;

		private string _notableCharactersText;

		private string _settlementPath;

		private string _informationText;

		private string _settlementImageID;

		private string _boundSettlementText;

		private string _detailsText;

		private double _settlementCropPosition;

		private bool _hasBoundSettlement;

		private bool _hasNotables;
	}
}
