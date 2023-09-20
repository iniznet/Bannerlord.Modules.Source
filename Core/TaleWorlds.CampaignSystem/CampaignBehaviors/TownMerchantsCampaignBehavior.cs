using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem.AgentOrigins;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Settlements.Locations;
using TaleWorlds.Core;

namespace TaleWorlds.CampaignSystem.CampaignBehaviors
{
	// Token: 0x020003D8 RID: 984
	public class TownMerchantsCampaignBehavior : CampaignBehaviorBase
	{
		// Token: 0x06003B94 RID: 15252 RVA: 0x00119991 File Offset: 0x00117B91
		public override void RegisterEvents()
		{
			CampaignEvents.LocationCharactersAreReadyToSpawnEvent.AddNonSerializedListener(this, new Action<Dictionary<string, int>>(this.LocationCharactersAreReadyToSpawn));
		}

		// Token: 0x06003B95 RID: 15253 RVA: 0x001199AA File Offset: 0x00117BAA
		public override void SyncData(IDataStore dataStore)
		{
		}

		// Token: 0x06003B96 RID: 15254 RVA: 0x001199AC File Offset: 0x00117BAC
		private void LocationCharactersAreReadyToSpawn(Dictionary<string, int> unusedUsablePointCount)
		{
			Location locationWithId = PlayerEncounter.LocationEncounter.Settlement.LocationComplex.GetLocationWithId("center");
			if (CampaignMission.Current.Location == locationWithId && Campaign.Current.IsDay)
			{
				this.AddTradersToCenter(unusedUsablePointCount);
			}
		}

		// Token: 0x06003B97 RID: 15255 RVA: 0x001199F4 File Offset: 0x00117BF4
		private void AddTradersToCenter(Dictionary<string, int> unusedUsablePointCount)
		{
			Location locationWithId = Settlement.CurrentSettlement.LocationComplex.GetLocationWithId("center");
			int num;
			if (unusedUsablePointCount.TryGetValue("sp_merchant", out num))
			{
				locationWithId.AddLocationCharacters(new CreateLocationCharacterDelegate(TownMerchantsCampaignBehavior.CreateMerchant), Settlement.CurrentSettlement.Culture, LocationCharacter.CharacterRelations.Neutral, num);
			}
			if (unusedUsablePointCount.TryGetValue("sp_horse_merchant", out num))
			{
				locationWithId.AddLocationCharacters(new CreateLocationCharacterDelegate(TownMerchantsCampaignBehavior.CreateHorseTrader), Settlement.CurrentSettlement.Culture, LocationCharacter.CharacterRelations.Neutral, num);
			}
			if (unusedUsablePointCount.TryGetValue("sp_armorer", out num))
			{
				locationWithId.AddLocationCharacters(new CreateLocationCharacterDelegate(TownMerchantsCampaignBehavior.CreateArmorer), Settlement.CurrentSettlement.Culture, LocationCharacter.CharacterRelations.Neutral, num);
			}
			if (unusedUsablePointCount.TryGetValue("sp_weaponsmith", out num))
			{
				locationWithId.AddLocationCharacters(new CreateLocationCharacterDelegate(TownMerchantsCampaignBehavior.CreateWeaponsmith), Settlement.CurrentSettlement.Culture, LocationCharacter.CharacterRelations.Neutral, num);
			}
			if (unusedUsablePointCount.TryGetValue("sp_blacksmith", out num))
			{
				locationWithId.AddLocationCharacters(new CreateLocationCharacterDelegate(TownMerchantsCampaignBehavior.CreateBlacksmith), Settlement.CurrentSettlement.Culture, LocationCharacter.CharacterRelations.Neutral, num);
			}
		}

		// Token: 0x06003B98 RID: 15256 RVA: 0x00119AF8 File Offset: 0x00117CF8
		private static LocationCharacter CreateBlacksmith(CultureObject culture, LocationCharacter.CharacterRelations relation)
		{
			CharacterObject blacksmith = culture.Blacksmith;
			Monster monsterWithSuffix = FaceGen.GetMonsterWithSuffix(blacksmith.Race, "_settlement");
			int num;
			int num2;
			Campaign.Current.Models.AgeModel.GetAgeLimitForLocation(blacksmith, out num, out num2, "");
			return new LocationCharacter(new AgentData(new SimpleAgentOrigin(blacksmith, -1, null, default(UniqueTroopDescriptor))).Monster(monsterWithSuffix).Age(MBRandom.RandomInt(num, num2)), new LocationCharacter.AddBehaviorsDelegate(SandBoxManager.Instance.AgentBehaviorManager.AddWandererBehaviors), "sp_blacksmith", true, relation, null, true, false, null, false, false, true);
		}

		// Token: 0x06003B99 RID: 15257 RVA: 0x00119B90 File Offset: 0x00117D90
		private static LocationCharacter CreateMerchant(CultureObject culture, LocationCharacter.CharacterRelations relation)
		{
			CharacterObject merchant = culture.Merchant;
			Monster monsterWithSuffix = FaceGen.GetMonsterWithSuffix(merchant.Race, "_settlement");
			int num;
			int num2;
			Campaign.Current.Models.AgeModel.GetAgeLimitForLocation(merchant, out num, out num2, "");
			AgentData agentData = new AgentData(new SimpleAgentOrigin(merchant, -1, null, default(UniqueTroopDescriptor))).Monster(monsterWithSuffix).Age(MBRandom.RandomInt(num, num2));
			return new LocationCharacter(agentData, new LocationCharacter.AddBehaviorsDelegate(SandBoxManager.Instance.AgentBehaviorManager.AddWandererBehaviors), "sp_merchant", true, relation, ActionSetCode.GenerateActionSetNameWithSuffix(agentData.AgentMonster, agentData.AgentIsFemale, "_seller"), true, false, null, false, false, true);
		}

		// Token: 0x06003B9A RID: 15258 RVA: 0x00119C40 File Offset: 0x00117E40
		private static LocationCharacter CreateHorseTrader(CultureObject culture, LocationCharacter.CharacterRelations relation)
		{
			CharacterObject horseMerchant = culture.HorseMerchant;
			Monster monsterWithSuffix = FaceGen.GetMonsterWithSuffix(horseMerchant.Race, "_settlement");
			int num;
			int num2;
			Campaign.Current.Models.AgeModel.GetAgeLimitForLocation(horseMerchant, out num, out num2, "");
			AgentData agentData = new AgentData(new SimpleAgentOrigin(horseMerchant, -1, null, default(UniqueTroopDescriptor))).Monster(monsterWithSuffix).Age(MBRandom.RandomInt(num, num2));
			return new LocationCharacter(agentData, new LocationCharacter.AddBehaviorsDelegate(SandBoxManager.Instance.AgentBehaviorManager.AddWandererBehaviors), "sp_horse_merchant", true, relation, ActionSetCode.GenerateActionSetNameWithSuffix(agentData.AgentMonster, agentData.AgentIsFemale, "_seller"), true, false, null, false, false, true);
		}

		// Token: 0x06003B9B RID: 15259 RVA: 0x00119CF0 File Offset: 0x00117EF0
		private static LocationCharacter CreateArmorer(CultureObject culture, LocationCharacter.CharacterRelations relation)
		{
			CharacterObject armorer = culture.Armorer;
			Monster monsterWithSuffix = FaceGen.GetMonsterWithSuffix(armorer.Race, "_settlement");
			int num;
			int num2;
			Campaign.Current.Models.AgeModel.GetAgeLimitForLocation(armorer, out num, out num2, "");
			AgentData agentData = new AgentData(new SimpleAgentOrigin(armorer, -1, null, default(UniqueTroopDescriptor))).Monster(monsterWithSuffix).Age(MBRandom.RandomInt(num, num2));
			return new LocationCharacter(agentData, new LocationCharacter.AddBehaviorsDelegate(SandBoxManager.Instance.AgentBehaviorManager.AddWandererBehaviors), "sp_armorer", true, relation, ActionSetCode.GenerateActionSetNameWithSuffix(agentData.AgentMonster, agentData.AgentIsFemale, "_seller"), true, false, null, false, false, true);
		}

		// Token: 0x06003B9C RID: 15260 RVA: 0x00119DA0 File Offset: 0x00117FA0
		private static LocationCharacter CreateWeaponsmith(CultureObject culture, LocationCharacter.CharacterRelations relation)
		{
			CharacterObject weaponsmith = culture.Weaponsmith;
			Monster monsterWithSuffix = FaceGen.GetMonsterWithSuffix(weaponsmith.Race, "_settlement");
			int num;
			int num2;
			Campaign.Current.Models.AgeModel.GetAgeLimitForLocation(weaponsmith, out num, out num2, "");
			AgentData agentData = new AgentData(new SimpleAgentOrigin(weaponsmith, -1, null, default(UniqueTroopDescriptor))).Monster(monsterWithSuffix).Age(MBRandom.RandomInt(num, num2));
			return new LocationCharacter(agentData, new LocationCharacter.AddBehaviorsDelegate(SandBoxManager.Instance.AgentBehaviorManager.AddWandererBehaviors), "sp_weaponsmith", true, relation, ActionSetCode.GenerateActionSetNameWithSuffix(agentData.AgentMonster, agentData.AgentIsFemale, "_weaponsmith"), true, false, null, false, false, true);
		}
	}
}
