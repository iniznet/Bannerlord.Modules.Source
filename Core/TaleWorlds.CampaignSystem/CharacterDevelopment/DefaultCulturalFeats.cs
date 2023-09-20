using System;
using TaleWorlds.Core;

namespace TaleWorlds.CampaignSystem.CharacterDevelopment
{
	// Token: 0x0200034E RID: 846
	public class DefaultCulturalFeats
	{
		// Token: 0x17000B3C RID: 2876
		// (get) Token: 0x06002F48 RID: 12104 RVA: 0x000C1E5C File Offset: 0x000C005C
		private static DefaultCulturalFeats Instance
		{
			get
			{
				return Campaign.Current.DefaultFeats;
			}
		}

		// Token: 0x06002F49 RID: 12105 RVA: 0x000C1E68 File Offset: 0x000C0068
		public DefaultCulturalFeats()
		{
			this.RegisterAll();
		}

		// Token: 0x06002F4A RID: 12106 RVA: 0x000C1E78 File Offset: 0x000C0078
		private void RegisterAll()
		{
			this._aseraiTraderFeat = this.Create("aserai_cheaper_caravans");
			this._aseraiDesertSpeedFeat = this.Create("aserai_desert_speed");
			this._aseraiWageFeat = this.Create("aserai_increased_wages");
			this._battaniaForestSpeedFeat = this.Create("battanian_forest_speed");
			this._battaniaMilitiaFeat = this.Create("battanian_militia_production");
			this._battaniaConstructionFeat = this.Create("battanian_slower_construction");
			this._empireGarrisonWageFeat = this.Create("empire_decreased_garrison_wage");
			this._empireArmyInfluenceFeat = this.Create("empire_army_influence");
			this._empireVillageHearthFeat = this.Create("empire_slower_hearth_production");
			this._khuzaitCheaperRecruitsFeat = this.Create("khuzait_cheaper_recruits_mounted");
			this._khuzaitAnimalProductionFeat = this.Create("khuzait_increased_animal_production");
			this._khuzaitDecreasedTaxFeat = this.Create("khuzait_decreased_town_tax");
			this._sturgianCheaperRecruitsFeat = this.Create("sturgian_cheaper_recruits_infantry");
			this._sturgianArmyCohesionFeat = this.Create("sturgian_decreased_cohesion_rate");
			this._sturgianDecisionPenaltyFeat = this.Create("sturgian_increased_decision_penalty");
			this._vlandianRenownIncomeFeat = this.Create("vlandian_renown_mercenary_income");
			this._vlandianVillageProductionFeat = this.Create("vlandian_villages_production_bonus");
			this._vlandianArmyInfluenceCostFeat = this.Create("vlandian_increased_army_influence_cost");
			this.InitializeAll();
		}

		// Token: 0x06002F4B RID: 12107 RVA: 0x000C1FBD File Offset: 0x000C01BD
		private FeatObject Create(string stringId)
		{
			return Game.Current.ObjectManager.RegisterPresumedObject<FeatObject>(new FeatObject(stringId));
		}

		// Token: 0x06002F4C RID: 12108 RVA: 0x000C1FD4 File Offset: 0x000C01D4
		private void InitializeAll()
		{
			this._aseraiTraderFeat.Initialize("{=!}aserai_cheaper_caravans", "{=7kGGgkro}Caravans are 30% cheaper to build. 10% less trade penalty.", 0.7f, true, FeatObject.AdditionType.AddFactor);
			this._aseraiDesertSpeedFeat.Initialize("{=!}aserai_desert_speed", "{=6aFTN1Nb}No speed penalty on desert.", 1f, true, FeatObject.AdditionType.AddFactor);
			this._aseraiWageFeat.Initialize("{=!}aserai_increased_wages", "{=GacrZ1Jl}Daily wages of troops in the party are increased by 5%.", 0.05f, false, FeatObject.AdditionType.AddFactor);
			this._battaniaForestSpeedFeat.Initialize("{=!}battanian_forest_speed", "{=38W2WloI}50% less speed penalty and 15% sight range bonus in forests.", 0.5f, true, FeatObject.AdditionType.AddFactor);
			this._battaniaMilitiaFeat.Initialize("{=!}battanian_militia_production", "{=1qUFMK28}Towns owned by Battanian rulers have +1 militia production.", 1f, true, FeatObject.AdditionType.Add);
			this._battaniaConstructionFeat.Initialize("{=!}battanian_slower_construction", "{=ruP9jbSq}10% slower build rate for town projects in settlements.", -0.1f, false, FeatObject.AdditionType.AddFactor);
			this._empireGarrisonWageFeat.Initialize("{=!}empire_decreased_garrison_wage", "{=a2eM0QUb}20% less garrison troop wage.", -0.2f, true, FeatObject.AdditionType.AddFactor);
			this._empireArmyInfluenceFeat.Initialize("{=!}empire_army_influence", "{=xgPNGOa8}Being in army brings 25% more influence.", 0.25f, true, FeatObject.AdditionType.AddFactor);
			this._empireVillageHearthFeat.Initialize("{=!}empire_slower_hearth_production", "{=UWiqIFUb}Village hearths increase 20% less.", -0.2f, false, FeatObject.AdditionType.AddFactor);
			this._khuzaitCheaperRecruitsFeat.Initialize("{=!}khuzait_cheaper_recruits_mounted", "{=JUpZuals}Recruiting and upgrading mounted troops are 10% cheaper.", -0.1f, true, FeatObject.AdditionType.AddFactor);
			this._khuzaitAnimalProductionFeat.Initialize("{=!}khuzait_increased_animal_production", "{=Xaw2CoCG}25% production bonus to horse, mule, cow and sheep in villages owned by Khuzait rulers.", 0.25f, true, FeatObject.AdditionType.AddFactor);
			this._khuzaitDecreasedTaxFeat.Initialize("{=!}khuzait_decreased_town_tax", "{=8PsaGhI8}20% less tax income from towns.", -0.2f, false, FeatObject.AdditionType.AddFactor);
			this._sturgianCheaperRecruitsFeat.Initialize("{=!}sturgian_cheaper_recruits_infantry", "{=CJ5pLHaL}Recruiting and upgrading infantry troops are 25% cheaper.", -0.25f, true, FeatObject.AdditionType.AddFactor);
			this._sturgianArmyCohesionFeat.Initialize("{=!}sturgian_decreased_cohesion_rate", "{=QiHaWd75}Armies lose 20% less daily cohesion.", -0.2f, true, FeatObject.AdditionType.AddFactor);
			this._sturgianDecisionPenaltyFeat.Initialize("{=!}sturgian_increased_decision_penalty", "{=fB7kS9Cx}20% more relationship penalty from kingdom decisions.", 0.2f, false, FeatObject.AdditionType.AddFactor);
			this._vlandianRenownIncomeFeat.Initialize("{=!}vlandian_renown_mercenary_income", "{=ppdrgOL8}5% more renown from the battles, 15% more income while serving as a mercenary.", 0.05f, true, FeatObject.AdditionType.AddFactor);
			this._vlandianVillageProductionFeat.Initialize("{=!}vlandian_villages_production_bonus", "{=3GsZXXOi}10% production bonus to villages that are bound to castles.", 0.1f, true, FeatObject.AdditionType.AddFactor);
			this._vlandianArmyInfluenceCostFeat.Initialize("{=!}vlandian_increased_army_influence_cost", "{=O1XCNeZr}Recruiting lords to armies costs 20% more influence.", 0.2f, false, FeatObject.AdditionType.AddFactor);
		}

		// Token: 0x17000B3D RID: 2877
		// (get) Token: 0x06002F4D RID: 12109 RVA: 0x000C21D9 File Offset: 0x000C03D9
		public static FeatObject AseraiTraderFeat
		{
			get
			{
				return DefaultCulturalFeats.Instance._aseraiTraderFeat;
			}
		}

		// Token: 0x17000B3E RID: 2878
		// (get) Token: 0x06002F4E RID: 12110 RVA: 0x000C21E5 File Offset: 0x000C03E5
		public static FeatObject AseraiDesertFeat
		{
			get
			{
				return DefaultCulturalFeats.Instance._aseraiDesertSpeedFeat;
			}
		}

		// Token: 0x17000B3F RID: 2879
		// (get) Token: 0x06002F4F RID: 12111 RVA: 0x000C21F1 File Offset: 0x000C03F1
		public static FeatObject AseraiIncreasedWageFeat
		{
			get
			{
				return DefaultCulturalFeats.Instance._aseraiWageFeat;
			}
		}

		// Token: 0x17000B40 RID: 2880
		// (get) Token: 0x06002F50 RID: 12112 RVA: 0x000C21FD File Offset: 0x000C03FD
		public static FeatObject BattanianForestSpeedFeat
		{
			get
			{
				return DefaultCulturalFeats.Instance._battaniaForestSpeedFeat;
			}
		}

		// Token: 0x17000B41 RID: 2881
		// (get) Token: 0x06002F51 RID: 12113 RVA: 0x000C2209 File Offset: 0x000C0409
		public static FeatObject BattanianMilitiaFeat
		{
			get
			{
				return DefaultCulturalFeats.Instance._battaniaMilitiaFeat;
			}
		}

		// Token: 0x17000B42 RID: 2882
		// (get) Token: 0x06002F52 RID: 12114 RVA: 0x000C2215 File Offset: 0x000C0415
		public static FeatObject BattanianConstructionFeat
		{
			get
			{
				return DefaultCulturalFeats.Instance._battaniaConstructionFeat;
			}
		}

		// Token: 0x17000B43 RID: 2883
		// (get) Token: 0x06002F53 RID: 12115 RVA: 0x000C2221 File Offset: 0x000C0421
		public static FeatObject EmpireGarrisonWageFeat
		{
			get
			{
				return DefaultCulturalFeats.Instance._empireGarrisonWageFeat;
			}
		}

		// Token: 0x17000B44 RID: 2884
		// (get) Token: 0x06002F54 RID: 12116 RVA: 0x000C222D File Offset: 0x000C042D
		public static FeatObject EmpireArmyInfluenceFeat
		{
			get
			{
				return DefaultCulturalFeats.Instance._empireArmyInfluenceFeat;
			}
		}

		// Token: 0x17000B45 RID: 2885
		// (get) Token: 0x06002F55 RID: 12117 RVA: 0x000C2239 File Offset: 0x000C0439
		public static FeatObject EmpireVillageHearthFeat
		{
			get
			{
				return DefaultCulturalFeats.Instance._empireVillageHearthFeat;
			}
		}

		// Token: 0x17000B46 RID: 2886
		// (get) Token: 0x06002F56 RID: 12118 RVA: 0x000C2245 File Offset: 0x000C0445
		public static FeatObject KhuzaitRecruitUpgradeFeat
		{
			get
			{
				return DefaultCulturalFeats.Instance._khuzaitCheaperRecruitsFeat;
			}
		}

		// Token: 0x17000B47 RID: 2887
		// (get) Token: 0x06002F57 RID: 12119 RVA: 0x000C2251 File Offset: 0x000C0451
		public static FeatObject KhuzaitAnimalProductionFeat
		{
			get
			{
				return DefaultCulturalFeats.Instance._khuzaitAnimalProductionFeat;
			}
		}

		// Token: 0x17000B48 RID: 2888
		// (get) Token: 0x06002F58 RID: 12120 RVA: 0x000C225D File Offset: 0x000C045D
		public static FeatObject KhuzaitDecreasedTaxFeat
		{
			get
			{
				return DefaultCulturalFeats.Instance._khuzaitDecreasedTaxFeat;
			}
		}

		// Token: 0x17000B49 RID: 2889
		// (get) Token: 0x06002F59 RID: 12121 RVA: 0x000C2269 File Offset: 0x000C0469
		public static FeatObject SturgianRecruitUpgradeFeat
		{
			get
			{
				return DefaultCulturalFeats.Instance._sturgianCheaperRecruitsFeat;
			}
		}

		// Token: 0x17000B4A RID: 2890
		// (get) Token: 0x06002F5A RID: 12122 RVA: 0x000C2275 File Offset: 0x000C0475
		public static FeatObject SturgianArmyCohesionFeat
		{
			get
			{
				return DefaultCulturalFeats.Instance._sturgianArmyCohesionFeat;
			}
		}

		// Token: 0x17000B4B RID: 2891
		// (get) Token: 0x06002F5B RID: 12123 RVA: 0x000C2281 File Offset: 0x000C0481
		public static FeatObject SturgianDecisionPenaltyFeat
		{
			get
			{
				return DefaultCulturalFeats.Instance._sturgianDecisionPenaltyFeat;
			}
		}

		// Token: 0x17000B4C RID: 2892
		// (get) Token: 0x06002F5C RID: 12124 RVA: 0x000C228D File Offset: 0x000C048D
		public static FeatObject VlandianRenownMercenaryFeat
		{
			get
			{
				return DefaultCulturalFeats.Instance._vlandianRenownIncomeFeat;
			}
		}

		// Token: 0x17000B4D RID: 2893
		// (get) Token: 0x06002F5D RID: 12125 RVA: 0x000C2299 File Offset: 0x000C0499
		public static FeatObject VlandianCastleVillageProductionFeat
		{
			get
			{
				return DefaultCulturalFeats.Instance._vlandianVillageProductionFeat;
			}
		}

		// Token: 0x17000B4E RID: 2894
		// (get) Token: 0x06002F5E RID: 12126 RVA: 0x000C22A5 File Offset: 0x000C04A5
		public static FeatObject VlandianArmyInfluenceFeat
		{
			get
			{
				return DefaultCulturalFeats.Instance._vlandianArmyInfluenceCostFeat;
			}
		}

		// Token: 0x04000E18 RID: 3608
		private FeatObject _aseraiTraderFeat;

		// Token: 0x04000E19 RID: 3609
		private FeatObject _aseraiDesertSpeedFeat;

		// Token: 0x04000E1A RID: 3610
		private FeatObject _aseraiWageFeat;

		// Token: 0x04000E1B RID: 3611
		private FeatObject _battaniaForestSpeedFeat;

		// Token: 0x04000E1C RID: 3612
		private FeatObject _battaniaMilitiaFeat;

		// Token: 0x04000E1D RID: 3613
		private FeatObject _battaniaConstructionFeat;

		// Token: 0x04000E1E RID: 3614
		private FeatObject _empireGarrisonWageFeat;

		// Token: 0x04000E1F RID: 3615
		private FeatObject _empireArmyInfluenceFeat;

		// Token: 0x04000E20 RID: 3616
		private FeatObject _empireVillageHearthFeat;

		// Token: 0x04000E21 RID: 3617
		private FeatObject _khuzaitCheaperRecruitsFeat;

		// Token: 0x04000E22 RID: 3618
		private FeatObject _khuzaitAnimalProductionFeat;

		// Token: 0x04000E23 RID: 3619
		private FeatObject _khuzaitDecreasedTaxFeat;

		// Token: 0x04000E24 RID: 3620
		private FeatObject _sturgianCheaperRecruitsFeat;

		// Token: 0x04000E25 RID: 3621
		private FeatObject _sturgianArmyCohesionFeat;

		// Token: 0x04000E26 RID: 3622
		private FeatObject _sturgianDecisionPenaltyFeat;

		// Token: 0x04000E27 RID: 3623
		private FeatObject _vlandianRenownIncomeFeat;

		// Token: 0x04000E28 RID: 3624
		private FeatObject _vlandianVillageProductionFeat;

		// Token: 0x04000E29 RID: 3625
		private FeatObject _vlandianArmyInfluenceCostFeat;
	}
}
