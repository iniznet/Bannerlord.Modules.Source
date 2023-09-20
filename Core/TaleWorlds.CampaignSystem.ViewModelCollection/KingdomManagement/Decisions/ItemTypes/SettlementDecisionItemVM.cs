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
	// Token: 0x0200006E RID: 110
	public class SettlementDecisionItemVM : DecisionItemBaseVM
	{
		// Token: 0x17000303 RID: 771
		// (get) Token: 0x0600097A RID: 2426 RVA: 0x0002703C File Offset: 0x0002523C
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

		// Token: 0x0600097B RID: 2427 RVA: 0x000270A6 File Offset: 0x000252A6
		public SettlementDecisionItemVM(Settlement settlement, KingdomDecision decision, Action onDecisionOver)
			: base(decision, onDecisionOver)
		{
			this._settlement = settlement;
			base.DecisionType = 1;
		}

		// Token: 0x0600097C RID: 2428 RVA: 0x000270C0 File Offset: 0x000252C0
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

		// Token: 0x17000304 RID: 772
		// (get) Token: 0x0600097D RID: 2429 RVA: 0x00027588 File Offset: 0x00025788
		// (set) Token: 0x0600097E RID: 2430 RVA: 0x00027590 File Offset: 0x00025790
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

		// Token: 0x17000305 RID: 773
		// (get) Token: 0x0600097F RID: 2431 RVA: 0x000275AE File Offset: 0x000257AE
		// (set) Token: 0x06000980 RID: 2432 RVA: 0x000275B6 File Offset: 0x000257B6
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

		// Token: 0x17000306 RID: 774
		// (get) Token: 0x06000981 RID: 2433 RVA: 0x000275D4 File Offset: 0x000257D4
		// (set) Token: 0x06000982 RID: 2434 RVA: 0x000275DC File Offset: 0x000257DC
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

		// Token: 0x17000307 RID: 775
		// (get) Token: 0x06000983 RID: 2435 RVA: 0x000275FF File Offset: 0x000257FF
		// (set) Token: 0x06000984 RID: 2436 RVA: 0x00027607 File Offset: 0x00025807
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

		// Token: 0x17000308 RID: 776
		// (get) Token: 0x06000985 RID: 2437 RVA: 0x0002762A File Offset: 0x0002582A
		// (set) Token: 0x06000986 RID: 2438 RVA: 0x00027632 File Offset: 0x00025832
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

		// Token: 0x17000309 RID: 777
		// (get) Token: 0x06000987 RID: 2439 RVA: 0x00027655 File Offset: 0x00025855
		// (set) Token: 0x06000988 RID: 2440 RVA: 0x0002765D File Offset: 0x0002585D
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

		// Token: 0x1700030A RID: 778
		// (get) Token: 0x06000989 RID: 2441 RVA: 0x00027680 File Offset: 0x00025880
		// (set) Token: 0x0600098A RID: 2442 RVA: 0x00027688 File Offset: 0x00025888
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

		// Token: 0x1700030B RID: 779
		// (get) Token: 0x0600098B RID: 2443 RVA: 0x000276AB File Offset: 0x000258AB
		// (set) Token: 0x0600098C RID: 2444 RVA: 0x000276B3 File Offset: 0x000258B3
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

		// Token: 0x1700030C RID: 780
		// (get) Token: 0x0600098D RID: 2445 RVA: 0x000276D1 File Offset: 0x000258D1
		// (set) Token: 0x0600098E RID: 2446 RVA: 0x000276D9 File Offset: 0x000258D9
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

		// Token: 0x1700030D RID: 781
		// (get) Token: 0x0600098F RID: 2447 RVA: 0x000276FC File Offset: 0x000258FC
		// (set) Token: 0x06000990 RID: 2448 RVA: 0x00027704 File Offset: 0x00025904
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

		// Token: 0x1700030E RID: 782
		// (get) Token: 0x06000991 RID: 2449 RVA: 0x00027727 File Offset: 0x00025927
		// (set) Token: 0x06000992 RID: 2450 RVA: 0x0002772F File Offset: 0x0002592F
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

		// Token: 0x1700030F RID: 783
		// (get) Token: 0x06000993 RID: 2451 RVA: 0x00027752 File Offset: 0x00025952
		// (set) Token: 0x06000994 RID: 2452 RVA: 0x0002775A File Offset: 0x0002595A
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

		// Token: 0x17000310 RID: 784
		// (get) Token: 0x06000995 RID: 2453 RVA: 0x00027778 File Offset: 0x00025978
		// (set) Token: 0x06000996 RID: 2454 RVA: 0x00027780 File Offset: 0x00025980
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

		// Token: 0x17000311 RID: 785
		// (get) Token: 0x06000997 RID: 2455 RVA: 0x0002779E File Offset: 0x0002599E
		// (set) Token: 0x06000998 RID: 2456 RVA: 0x000277A6 File Offset: 0x000259A6
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

		// Token: 0x17000312 RID: 786
		// (get) Token: 0x06000999 RID: 2457 RVA: 0x000277C4 File Offset: 0x000259C4
		// (set) Token: 0x0600099A RID: 2458 RVA: 0x000277CC File Offset: 0x000259CC
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

		// Token: 0x17000313 RID: 787
		// (get) Token: 0x0600099B RID: 2459 RVA: 0x000277EA File Offset: 0x000259EA
		// (set) Token: 0x0600099C RID: 2460 RVA: 0x000277F2 File Offset: 0x000259F2
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

		// Token: 0x17000314 RID: 788
		// (get) Token: 0x0600099D RID: 2461 RVA: 0x00027810 File Offset: 0x00025A10
		// (set) Token: 0x0600099E RID: 2462 RVA: 0x00027818 File Offset: 0x00025A18
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

		// Token: 0x17000315 RID: 789
		// (get) Token: 0x0600099F RID: 2463 RVA: 0x00027836 File Offset: 0x00025A36
		// (set) Token: 0x060009A0 RID: 2464 RVA: 0x0002783E File Offset: 0x00025A3E
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

		// Token: 0x17000316 RID: 790
		// (get) Token: 0x060009A1 RID: 2465 RVA: 0x0002785C File Offset: 0x00025A5C
		// (set) Token: 0x060009A2 RID: 2466 RVA: 0x00027864 File Offset: 0x00025A64
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

		// Token: 0x17000317 RID: 791
		// (get) Token: 0x060009A3 RID: 2467 RVA: 0x00027882 File Offset: 0x00025A82
		// (set) Token: 0x060009A4 RID: 2468 RVA: 0x0002788A File Offset: 0x00025A8A
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

		// Token: 0x17000318 RID: 792
		// (get) Token: 0x060009A5 RID: 2469 RVA: 0x000278A8 File Offset: 0x00025AA8
		// (set) Token: 0x060009A6 RID: 2470 RVA: 0x000278B0 File Offset: 0x00025AB0
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

		// Token: 0x17000319 RID: 793
		// (get) Token: 0x060009A7 RID: 2471 RVA: 0x000278D3 File Offset: 0x00025AD3
		// (set) Token: 0x060009A8 RID: 2472 RVA: 0x000278DB File Offset: 0x00025ADB
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

		// Token: 0x1700031A RID: 794
		// (get) Token: 0x060009A9 RID: 2473 RVA: 0x000278FE File Offset: 0x00025AFE
		// (set) Token: 0x060009AA RID: 2474 RVA: 0x00027906 File Offset: 0x00025B06
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

		// Token: 0x1700031B RID: 795
		// (get) Token: 0x060009AB RID: 2475 RVA: 0x00027929 File Offset: 0x00025B29
		// (set) Token: 0x060009AC RID: 2476 RVA: 0x00027931 File Offset: 0x00025B31
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

		// Token: 0x1700031C RID: 796
		// (get) Token: 0x060009AD RID: 2477 RVA: 0x00027954 File Offset: 0x00025B54
		// (set) Token: 0x060009AE RID: 2478 RVA: 0x0002795C File Offset: 0x00025B5C
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

		// Token: 0x1700031D RID: 797
		// (get) Token: 0x060009AF RID: 2479 RVA: 0x0002797F File Offset: 0x00025B7F
		// (set) Token: 0x060009B0 RID: 2480 RVA: 0x00027987 File Offset: 0x00025B87
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

		// Token: 0x1700031E RID: 798
		// (get) Token: 0x060009B1 RID: 2481 RVA: 0x000279AA File Offset: 0x00025BAA
		// (set) Token: 0x060009B2 RID: 2482 RVA: 0x000279B2 File Offset: 0x00025BB2
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

		// Token: 0x1700031F RID: 799
		// (get) Token: 0x060009B3 RID: 2483 RVA: 0x000279D5 File Offset: 0x00025BD5
		// (set) Token: 0x060009B4 RID: 2484 RVA: 0x000279DD File Offset: 0x00025BDD
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

		// Token: 0x17000320 RID: 800
		// (get) Token: 0x060009B5 RID: 2485 RVA: 0x00027A00 File Offset: 0x00025C00
		// (set) Token: 0x060009B6 RID: 2486 RVA: 0x00027A08 File Offset: 0x00025C08
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

		// Token: 0x17000321 RID: 801
		// (get) Token: 0x060009B7 RID: 2487 RVA: 0x00027A2B File Offset: 0x00025C2B
		// (set) Token: 0x060009B8 RID: 2488 RVA: 0x00027A33 File Offset: 0x00025C33
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

		// Token: 0x17000322 RID: 802
		// (get) Token: 0x060009B9 RID: 2489 RVA: 0x00027A51 File Offset: 0x00025C51
		// (set) Token: 0x060009BA RID: 2490 RVA: 0x00027A59 File Offset: 0x00025C59
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

		// Token: 0x04000443 RID: 1091
		private SettlementClaimantDecision _settlementDecision;

		// Token: 0x04000444 RID: 1092
		private SettlementClaimantPreliminaryDecision _settlementPreliminaryDecision;

		// Token: 0x04000445 RID: 1093
		private Settlement _settlement;

		// Token: 0x04000446 RID: 1094
		private string _settlementName;

		// Token: 0x04000447 RID: 1095
		private HeroVM _governor;

		// Token: 0x04000448 RID: 1096
		private MBBindingList<EncyclopediaSettlementVM> _boundVillages;

		// Token: 0x04000449 RID: 1097
		private MBBindingList<HeroVM> _notableCharacters;

		// Token: 0x0400044A RID: 1098
		private BasicTooltipViewModel _militasHint;

		// Token: 0x0400044B RID: 1099
		private BasicTooltipViewModel _prosperityHint;

		// Token: 0x0400044C RID: 1100
		private BasicTooltipViewModel _loyaltyHint;

		// Token: 0x0400044D RID: 1101
		private BasicTooltipViewModel _securityHint;

		// Token: 0x0400044E RID: 1102
		private BasicTooltipViewModel _wallsHint;

		// Token: 0x0400044F RID: 1103
		private BasicTooltipViewModel _garrisonHint;

		// Token: 0x04000450 RID: 1104
		private BasicTooltipViewModel _foodHint;

		// Token: 0x04000451 RID: 1105
		private HeroVM _owner;

		// Token: 0x04000452 RID: 1106
		private string _ownerText;

		// Token: 0x04000453 RID: 1107
		private string _militasText;

		// Token: 0x04000454 RID: 1108
		private string _garrisonText;

		// Token: 0x04000455 RID: 1109
		private string _prosperityText;

		// Token: 0x04000456 RID: 1110
		private string _loyaltyText;

		// Token: 0x04000457 RID: 1111
		private string _securityText;

		// Token: 0x04000458 RID: 1112
		private string _wallsText;

		// Token: 0x04000459 RID: 1113
		private string _foodText;

		// Token: 0x0400045A RID: 1114
		private string _descriptorText;

		// Token: 0x0400045B RID: 1115
		private string _villagesText;

		// Token: 0x0400045C RID: 1116
		private string _notableCharactersText;

		// Token: 0x0400045D RID: 1117
		private string _settlementPath;

		// Token: 0x0400045E RID: 1118
		private string _informationText;

		// Token: 0x0400045F RID: 1119
		private string _settlementImageID;

		// Token: 0x04000460 RID: 1120
		private string _boundSettlementText;

		// Token: 0x04000461 RID: 1121
		private string _detailsText;

		// Token: 0x04000462 RID: 1122
		private double _settlementCropPosition;

		// Token: 0x04000463 RID: 1123
		private bool _hasBoundSettlement;

		// Token: 0x04000464 RID: 1124
		private bool _hasNotables;
	}
}
