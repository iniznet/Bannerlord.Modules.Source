using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem.Conversation;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem.Settlements.Locations;
using TaleWorlds.Core;

namespace TaleWorlds.CampaignSystem
{
	// Token: 0x02000050 RID: 80
	public static class CampaignMission
	{
		// Token: 0x17000198 RID: 408
		// (get) Token: 0x06000796 RID: 1942 RVA: 0x00020A65 File Offset: 0x0001EC65
		// (set) Token: 0x06000797 RID: 1943 RVA: 0x00020A6C File Offset: 0x0001EC6C
		public static ICampaignMission Current { get; set; }

		// Token: 0x06000798 RID: 1944 RVA: 0x00020A74 File Offset: 0x0001EC74
		public static IMission OpenBattleMission(string scene)
		{
			return Campaign.Current.CampaignMissionManager.OpenBattleMission(scene);
		}

		// Token: 0x06000799 RID: 1945 RVA: 0x00020A86 File Offset: 0x0001EC86
		public static IMission OpenAlleyFightMission(string scene, int upgradeLevel, Location location, TroopRoster playerSideTroops, TroopRoster rivalSideTroops)
		{
			return Campaign.Current.CampaignMissionManager.OpenAlleyFightMission(scene, upgradeLevel, location, playerSideTroops, rivalSideTroops);
		}

		// Token: 0x0600079A RID: 1946 RVA: 0x00020A9D File Offset: 0x0001EC9D
		public static IMission OpenCombatMissionWithDialogue(string scene, CharacterObject characterToTalkTo, CharacterObject allyTroopsWithFixedTeam, int upgradeLevel)
		{
			return Campaign.Current.CampaignMissionManager.OpenCombatMissionWithDialogue(scene, characterToTalkTo, allyTroopsWithFixedTeam, upgradeLevel);
		}

		// Token: 0x0600079B RID: 1947 RVA: 0x00020AB2 File Offset: 0x0001ECB2
		public static IMission OpenBattleMissionWhileEnteringSettlement(string scene, int upgradeLevel, int numberOfMaxTroopToBeSpawnedForPlayer, int numberOfMaxTroopToBeSpawnedForOpponent)
		{
			return Campaign.Current.CampaignMissionManager.OpenBattleMissionWhileEnteringSettlement(scene, upgradeLevel, numberOfMaxTroopToBeSpawnedForPlayer, numberOfMaxTroopToBeSpawnedForOpponent);
		}

		// Token: 0x0600079C RID: 1948 RVA: 0x00020AC7 File Offset: 0x0001ECC7
		public static IMission OpenHideoutBattleMission(string scene, FlattenedTroopRoster playerTroops)
		{
			return Campaign.Current.CampaignMissionManager.OpenHideoutBattleMission(scene, playerTroops);
		}

		// Token: 0x0600079D RID: 1949 RVA: 0x00020ADC File Offset: 0x0001ECDC
		public static IMission OpenSiegeMissionWithDeployment(string scene, float[] wallHitPointsPercentages, bool hasAnySiegeTower, List<MissionSiegeWeapon> siegeWeaponsOfAttackers, List<MissionSiegeWeapon> siegeWeaponsOfDefenders, bool isPlayerAttacker, int upgradeLevel = 0, bool isSallyOut = false, bool isReliefForceAttack = false)
		{
			return Campaign.Current.CampaignMissionManager.OpenSiegeMissionWithDeployment(scene, wallHitPointsPercentages, hasAnySiegeTower, siegeWeaponsOfAttackers, siegeWeaponsOfDefenders, isPlayerAttacker, upgradeLevel, isSallyOut, isReliefForceAttack);
		}

		// Token: 0x0600079E RID: 1950 RVA: 0x00020B06 File Offset: 0x0001ED06
		public static IMission OpenSiegeMissionNoDeployment(string scene, bool isSallyOut = false, bool isReliefForceAttack = false)
		{
			return Campaign.Current.CampaignMissionManager.OpenSiegeMissionNoDeployment(scene, isSallyOut, isReliefForceAttack);
		}

		// Token: 0x0600079F RID: 1951 RVA: 0x00020B1A File Offset: 0x0001ED1A
		public static IMission OpenSiegeLordsHallFightMission(string scene, FlattenedTroopRoster attackerPriorityList)
		{
			return Campaign.Current.CampaignMissionManager.OpenSiegeLordsHallFightMission(scene, attackerPriorityList);
		}

		// Token: 0x060007A0 RID: 1952 RVA: 0x00020B2D File Offset: 0x0001ED2D
		public static IMission OpenBattleMission(MissionInitializerRecord rec)
		{
			return Campaign.Current.CampaignMissionManager.OpenBattleMission(rec);
		}

		// Token: 0x060007A1 RID: 1953 RVA: 0x00020B3F File Offset: 0x0001ED3F
		public static IMission OpenCaravanBattleMission(MissionInitializerRecord rec, bool isCaravan)
		{
			return Campaign.Current.CampaignMissionManager.OpenCaravanBattleMission(rec, isCaravan);
		}

		// Token: 0x060007A2 RID: 1954 RVA: 0x00020B52 File Offset: 0x0001ED52
		public static IMission OpenTownCenterMission(string scene, Location location, CharacterObject talkToChar, int townUpgradeLevel, string playerSpawnTag)
		{
			return Campaign.Current.CampaignMissionManager.OpenTownCenterMission(scene, townUpgradeLevel, location, talkToChar, playerSpawnTag);
		}

		// Token: 0x060007A3 RID: 1955 RVA: 0x00020B69 File Offset: 0x0001ED69
		public static IMission OpenCastleCourtyardMission(string scene, Location location, CharacterObject talkToChar, int castleUpgradeLevel)
		{
			return Campaign.Current.CampaignMissionManager.OpenCastleCourtyardMission(scene, castleUpgradeLevel, location, talkToChar);
		}

		// Token: 0x060007A4 RID: 1956 RVA: 0x00020B7E File Offset: 0x0001ED7E
		public static IMission OpenVillageMission(string scene, Location location, CharacterObject talkToChar)
		{
			return Campaign.Current.CampaignMissionManager.OpenVillageMission(scene, location, talkToChar);
		}

		// Token: 0x060007A5 RID: 1957 RVA: 0x00020B92 File Offset: 0x0001ED92
		public static IMission OpenIndoorMission(string scene, int upgradeLevel, Location location, CharacterObject talkToChar)
		{
			return Campaign.Current.CampaignMissionManager.OpenIndoorMission(scene, upgradeLevel, location, talkToChar);
		}

		// Token: 0x060007A6 RID: 1958 RVA: 0x00020BA7 File Offset: 0x0001EDA7
		public static IMission OpenPrisonBreakMission(string scene, Location location, CharacterObject prisonerCharacter, CharacterObject companionCharacter = null)
		{
			return Campaign.Current.CampaignMissionManager.OpenPrisonBreakMission(scene, location, prisonerCharacter, companionCharacter);
		}

		// Token: 0x060007A7 RID: 1959 RVA: 0x00020BBC File Offset: 0x0001EDBC
		public static IMission OpenArenaStartMission(string scene, Location location, CharacterObject talkToChar)
		{
			return Campaign.Current.CampaignMissionManager.OpenArenaStartMission(scene, location, talkToChar);
		}

		// Token: 0x060007A8 RID: 1960 RVA: 0x00020BD0 File Offset: 0x0001EDD0
		public static IMission OpenArenaDuelMission(string scene, Location location, CharacterObject talkToChar, bool requireCivilianEquipment, bool spawnBothSidesWithHorse, Action<CharacterObject> onDuelEnd, float customAgentHealth)
		{
			return Campaign.Current.CampaignMissionManager.OpenArenaDuelMission(scene, location, talkToChar, requireCivilianEquipment, spawnBothSidesWithHorse, onDuelEnd, customAgentHealth);
		}

		// Token: 0x060007A9 RID: 1961 RVA: 0x00020BEB File Offset: 0x0001EDEB
		public static IMission OpenConversationMission(ConversationCharacterData playerCharacterData, ConversationCharacterData conversationPartnerData, string specialScene = "", string sceneLevels = "")
		{
			return Campaign.Current.CampaignMissionManager.OpenConversationMission(playerCharacterData, conversationPartnerData, specialScene, sceneLevels);
		}

		// Token: 0x060007AA RID: 1962 RVA: 0x00020C00 File Offset: 0x0001EE00
		public static IMission OpenConversatonTestMission(string scene)
		{
			return Campaign.Current.CampaignMissionManager.OpenConversatonTestMission(scene);
		}

		// Token: 0x060007AB RID: 1963 RVA: 0x00020C12 File Offset: 0x0001EE12
		public static IMission OpenRetirementMission(string scene, Location location, CharacterObject talkToChar = null, string sceneLevels = null)
		{
			return Campaign.Current.CampaignMissionManager.OpenRetirementMission(scene, location, talkToChar, sceneLevels);
		}

		// Token: 0x02000497 RID: 1175
		public interface ICampaignMissionManager
		{
			// Token: 0x0600405E RID: 16478
			IMission OpenSiegeMissionWithDeployment(string scene, float[] wallHitPointsPercentages, bool hasAnySiegeTower, List<MissionSiegeWeapon> siegeWeaponsOfAttackers, List<MissionSiegeWeapon> siegeWeaponsOfDefenders, bool isPlayerAttacker, int upgradeLevel = 0, bool isSallyOut = false, bool isReliefForceAttack = false);

			// Token: 0x0600405F RID: 16479
			IMission OpenSiegeMissionNoDeployment(string scene, bool isSallyOut = false, bool isReliefForceAttack = false);

			// Token: 0x06004060 RID: 16480
			IMission OpenSiegeLordsHallFightMission(string scene, FlattenedTroopRoster attackerPriorityList);

			// Token: 0x06004061 RID: 16481
			IMission OpenBattleMission(MissionInitializerRecord rec);

			// Token: 0x06004062 RID: 16482
			IMission OpenCaravanBattleMission(MissionInitializerRecord rec, bool isCaravan);

			// Token: 0x06004063 RID: 16483
			IMission OpenBattleMission(string scene);

			// Token: 0x06004064 RID: 16484
			IMission OpenHideoutBattleMission(string scene, FlattenedTroopRoster playerTroops);

			// Token: 0x06004065 RID: 16485
			IMission OpenTownCenterMission(string scene, int townUpgradeLevel, Location location, CharacterObject talkToChar, string playerSpawnTag);

			// Token: 0x06004066 RID: 16486
			IMission OpenCastleCourtyardMission(string scene, int castleUpgradeLevel, Location location, CharacterObject talkToChar);

			// Token: 0x06004067 RID: 16487
			IMission OpenVillageMission(string scene, Location location, CharacterObject talkToChar);

			// Token: 0x06004068 RID: 16488
			IMission OpenIndoorMission(string scene, int upgradeLevel, Location location, CharacterObject talkToChar);

			// Token: 0x06004069 RID: 16489
			IMission OpenPrisonBreakMission(string scene, Location location, CharacterObject prisonerCharacter, CharacterObject companionCharacter = null);

			// Token: 0x0600406A RID: 16490
			IMission OpenArenaStartMission(string scene, Location location, CharacterObject talkToChar);

			// Token: 0x0600406B RID: 16491
			IMission OpenArenaDuelMission(string scene, Location location, CharacterObject duelCharacter, bool requireCivilianEquipment, bool spawnBOthSidesWithHorse, Action<CharacterObject> onDuelEndAction, float customAgentHealth);

			// Token: 0x0600406C RID: 16492
			IMission OpenConversationMission(ConversationCharacterData playerCharacterData, ConversationCharacterData conversationPartnerData, string specialScene = "", string sceneLevels = "");

			// Token: 0x0600406D RID: 16493
			IMission OpenMeetingMission(string scene, CharacterObject character);

			// Token: 0x0600406E RID: 16494
			IMission OpenEquipmentTestMission(string scene);

			// Token: 0x0600406F RID: 16495
			IMission OpenConversatonTestMission(string scene);

			// Token: 0x06004070 RID: 16496
			IMission OpenAlleyFightMission(string scene, int upgradeLevel, Location location, TroopRoster playerSideTroops, TroopRoster rivalSideTroops);

			// Token: 0x06004071 RID: 16497
			IMission OpenCombatMissionWithDialogue(string scene, CharacterObject characterToTalkTo, CharacterObject allyTroopsWithFixedTeam, int upgradeLevel);

			// Token: 0x06004072 RID: 16498
			IMission OpenBattleMissionWhileEnteringSettlement(string scene, int upgradeLevel, int numberOfMaxTroopToBeSpawnedForPlayer, int numberOfMaxTroopToBeSpawnedForOpponent);

			// Token: 0x06004073 RID: 16499
			IMission OpenRetirementMission(string scene, Location location, CharacterObject talkToChar = null, string sceneLevels = null);
		}
	}
}
