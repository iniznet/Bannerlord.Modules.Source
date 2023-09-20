using System;
using TaleWorlds.Core;

namespace TaleWorlds.CampaignSystem.CharacterDevelopment
{
	public class DefaultCulturalFeats
	{
		private static DefaultCulturalFeats Instance
		{
			get
			{
				return Campaign.Current.DefaultFeats;
			}
		}

		public DefaultCulturalFeats()
		{
			this.RegisterAll();
		}

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

		private FeatObject Create(string stringId)
		{
			return Game.Current.ObjectManager.RegisterPresumedObject<FeatObject>(new FeatObject(stringId));
		}

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

		public static FeatObject AseraiTraderFeat
		{
			get
			{
				return DefaultCulturalFeats.Instance._aseraiTraderFeat;
			}
		}

		public static FeatObject AseraiDesertFeat
		{
			get
			{
				return DefaultCulturalFeats.Instance._aseraiDesertSpeedFeat;
			}
		}

		public static FeatObject AseraiIncreasedWageFeat
		{
			get
			{
				return DefaultCulturalFeats.Instance._aseraiWageFeat;
			}
		}

		public static FeatObject BattanianForestSpeedFeat
		{
			get
			{
				return DefaultCulturalFeats.Instance._battaniaForestSpeedFeat;
			}
		}

		public static FeatObject BattanianMilitiaFeat
		{
			get
			{
				return DefaultCulturalFeats.Instance._battaniaMilitiaFeat;
			}
		}

		public static FeatObject BattanianConstructionFeat
		{
			get
			{
				return DefaultCulturalFeats.Instance._battaniaConstructionFeat;
			}
		}

		public static FeatObject EmpireGarrisonWageFeat
		{
			get
			{
				return DefaultCulturalFeats.Instance._empireGarrisonWageFeat;
			}
		}

		public static FeatObject EmpireArmyInfluenceFeat
		{
			get
			{
				return DefaultCulturalFeats.Instance._empireArmyInfluenceFeat;
			}
		}

		public static FeatObject EmpireVillageHearthFeat
		{
			get
			{
				return DefaultCulturalFeats.Instance._empireVillageHearthFeat;
			}
		}

		public static FeatObject KhuzaitRecruitUpgradeFeat
		{
			get
			{
				return DefaultCulturalFeats.Instance._khuzaitCheaperRecruitsFeat;
			}
		}

		public static FeatObject KhuzaitAnimalProductionFeat
		{
			get
			{
				return DefaultCulturalFeats.Instance._khuzaitAnimalProductionFeat;
			}
		}

		public static FeatObject KhuzaitDecreasedTaxFeat
		{
			get
			{
				return DefaultCulturalFeats.Instance._khuzaitDecreasedTaxFeat;
			}
		}

		public static FeatObject SturgianRecruitUpgradeFeat
		{
			get
			{
				return DefaultCulturalFeats.Instance._sturgianCheaperRecruitsFeat;
			}
		}

		public static FeatObject SturgianArmyCohesionFeat
		{
			get
			{
				return DefaultCulturalFeats.Instance._sturgianArmyCohesionFeat;
			}
		}

		public static FeatObject SturgianDecisionPenaltyFeat
		{
			get
			{
				return DefaultCulturalFeats.Instance._sturgianDecisionPenaltyFeat;
			}
		}

		public static FeatObject VlandianRenownMercenaryFeat
		{
			get
			{
				return DefaultCulturalFeats.Instance._vlandianRenownIncomeFeat;
			}
		}

		public static FeatObject VlandianCastleVillageProductionFeat
		{
			get
			{
				return DefaultCulturalFeats.Instance._vlandianVillageProductionFeat;
			}
		}

		public static FeatObject VlandianArmyInfluenceFeat
		{
			get
			{
				return DefaultCulturalFeats.Instance._vlandianArmyInfluenceCostFeat;
			}
		}

		private FeatObject _aseraiTraderFeat;

		private FeatObject _aseraiDesertSpeedFeat;

		private FeatObject _aseraiWageFeat;

		private FeatObject _battaniaForestSpeedFeat;

		private FeatObject _battaniaMilitiaFeat;

		private FeatObject _battaniaConstructionFeat;

		private FeatObject _empireGarrisonWageFeat;

		private FeatObject _empireArmyInfluenceFeat;

		private FeatObject _empireVillageHearthFeat;

		private FeatObject _khuzaitCheaperRecruitsFeat;

		private FeatObject _khuzaitAnimalProductionFeat;

		private FeatObject _khuzaitDecreasedTaxFeat;

		private FeatObject _sturgianCheaperRecruitsFeat;

		private FeatObject _sturgianArmyCohesionFeat;

		private FeatObject _sturgianDecisionPenaltyFeat;

		private FeatObject _vlandianRenownIncomeFeat;

		private FeatObject _vlandianVillageProductionFeat;

		private FeatObject _vlandianArmyInfluenceCostFeat;
	}
}
