using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Conversation;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem.Settlements.Locations;
using TaleWorlds.Core;

namespace SandBox
{
	// Token: 0x0200000C RID: 12
	public class CampaignMissionManager : CampaignMission.ICampaignMissionManager
	{
		// Token: 0x06000082 RID: 130 RVA: 0x00005278 File Offset: 0x00003478
		IMission CampaignMission.ICampaignMissionManager.OpenSiegeMissionWithDeployment(string scene, float[] wallHitPointsPercentages, bool hasAnySiegeTower, List<MissionSiegeWeapon> siegeWeaponsOfAttackers, List<MissionSiegeWeapon> siegeWeaponsOfDefenders, bool isPlayerAttacker, int upgradeLevel, bool isSallyOut, bool isReliefForceAttack)
		{
			return SandBoxMissions.OpenSiegeMissionWithDeployment(scene, wallHitPointsPercentages, hasAnySiegeTower, siegeWeaponsOfAttackers, siegeWeaponsOfDefenders, isPlayerAttacker, upgradeLevel, isSallyOut, isReliefForceAttack);
		}

		// Token: 0x06000083 RID: 131 RVA: 0x00005299 File Offset: 0x00003499
		IMission CampaignMission.ICampaignMissionManager.OpenSiegeMissionNoDeployment(string scene, bool isSallyOut, bool isReliefForceAttack)
		{
			return SandBoxMissions.OpenSiegeMissionNoDeployment(scene, isSallyOut, isReliefForceAttack);
		}

		// Token: 0x06000084 RID: 132 RVA: 0x000052A3 File Offset: 0x000034A3
		IMission CampaignMission.ICampaignMissionManager.OpenSiegeLordsHallFightMission(string scene, FlattenedTroopRoster attackerPriorityList)
		{
			return SandBoxMissions.OpenSiegeLordsHallFightMission(scene, attackerPriorityList);
		}

		// Token: 0x06000085 RID: 133 RVA: 0x000052AC File Offset: 0x000034AC
		IMission CampaignMission.ICampaignMissionManager.OpenBattleMission(MissionInitializerRecord rec)
		{
			return SandBoxMissions.OpenBattleMission(rec);
		}

		// Token: 0x06000086 RID: 134 RVA: 0x000052B4 File Offset: 0x000034B4
		IMission CampaignMission.ICampaignMissionManager.OpenCaravanBattleMission(MissionInitializerRecord rec, bool isCaravan)
		{
			return SandBoxMissions.OpenCaravanBattleMission(rec, isCaravan);
		}

		// Token: 0x06000087 RID: 135 RVA: 0x000052BD File Offset: 0x000034BD
		IMission CampaignMission.ICampaignMissionManager.OpenBattleMission(string scene)
		{
			return SandBoxMissions.OpenBattleMission(scene);
		}

		// Token: 0x06000088 RID: 136 RVA: 0x000052C5 File Offset: 0x000034C5
		IMission CampaignMission.ICampaignMissionManager.OpenAlleyFightMission(string scene, int upgradeLevel, Location location, TroopRoster playerSideTroops, TroopRoster rivalSideTroops)
		{
			return SandBoxMissions.OpenAlleyFightMission(scene, upgradeLevel, location, playerSideTroops, rivalSideTroops);
		}

		// Token: 0x06000089 RID: 137 RVA: 0x000052D3 File Offset: 0x000034D3
		IMission CampaignMission.ICampaignMissionManager.OpenCombatMissionWithDialogue(string scene, CharacterObject characterToTalkTo, CharacterObject allyTroopsWithFixedTeam, int upgradeLevel)
		{
			return SandBoxMissions.OpenCombatMissionWithDialogue(scene, characterToTalkTo, allyTroopsWithFixedTeam, upgradeLevel);
		}

		// Token: 0x0600008A RID: 138 RVA: 0x000052DF File Offset: 0x000034DF
		IMission CampaignMission.ICampaignMissionManager.OpenBattleMissionWhileEnteringSettlement(string scene, int upgradeLevel, int numberOfMaxTroopToBeSpawnedForPlayer, int numberOfMaxTroopToBeSpawnedForOpponent)
		{
			return SandBoxMissions.OpenBattleMissionWhileEnteringSettlement(scene, upgradeLevel, numberOfMaxTroopToBeSpawnedForPlayer, numberOfMaxTroopToBeSpawnedForOpponent);
		}

		// Token: 0x0600008B RID: 139 RVA: 0x000052EB File Offset: 0x000034EB
		IMission CampaignMission.ICampaignMissionManager.OpenConversatonTestMission(string scene)
		{
			return TestMissions.OpenFacialAnimTestMission(scene, "");
		}

		// Token: 0x0600008C RID: 140 RVA: 0x000052F8 File Offset: 0x000034F8
		IMission CampaignMission.ICampaignMissionManager.OpenHideoutBattleMission(string scene, FlattenedTroopRoster playerTroops)
		{
			return SandBoxMissions.OpenHideoutBattleMission(scene, playerTroops);
		}

		// Token: 0x0600008D RID: 141 RVA: 0x00005301 File Offset: 0x00003501
		IMission CampaignMission.ICampaignMissionManager.OpenTownCenterMission(string scene, int townUpgradeLevel, Location location, CharacterObject talkToChar, string playerSpawnTag)
		{
			return SandBoxMissions.OpenTownCenterMission(scene, townUpgradeLevel, location, talkToChar, playerSpawnTag);
		}

		// Token: 0x0600008E RID: 142 RVA: 0x0000530F File Offset: 0x0000350F
		IMission CampaignMission.ICampaignMissionManager.OpenCastleCourtyardMission(string scene, int castleUpgradeLevel, Location location, CharacterObject talkToChar)
		{
			return SandBoxMissions.OpenCastleCourtyardMission(scene, castleUpgradeLevel, location, talkToChar);
		}

		// Token: 0x0600008F RID: 143 RVA: 0x0000531B File Offset: 0x0000351B
		IMission CampaignMission.ICampaignMissionManager.OpenVillageMission(string scene, Location location, CharacterObject talkToChar)
		{
			return SandBoxMissions.OpenVillageMission(scene, location, talkToChar, null);
		}

		// Token: 0x06000090 RID: 144 RVA: 0x00005326 File Offset: 0x00003526
		IMission CampaignMission.ICampaignMissionManager.OpenIndoorMission(string scene, int upgradeLevel, Location location, CharacterObject talkToChar)
		{
			return SandBoxMissions.OpenIndoorMission(scene, upgradeLevel, location, talkToChar);
		}

		// Token: 0x06000091 RID: 145 RVA: 0x00005332 File Offset: 0x00003532
		IMission CampaignMission.ICampaignMissionManager.OpenPrisonBreakMission(string scene, Location location, CharacterObject prisonerCharacter, CharacterObject companionCharacter)
		{
			return SandBoxMissions.OpenPrisonBreakMission(scene, location, prisonerCharacter, companionCharacter);
		}

		// Token: 0x06000092 RID: 146 RVA: 0x0000533E File Offset: 0x0000353E
		IMission CampaignMission.ICampaignMissionManager.OpenArenaStartMission(string scene, Location location, CharacterObject talkToChar)
		{
			return SandBoxMissions.OpenArenaStartMission(scene, location, talkToChar, "");
		}

		// Token: 0x06000093 RID: 147 RVA: 0x0000534D File Offset: 0x0000354D
		public IMission OpenArenaDuelMission(string scene, Location location, CharacterObject duelCharacter, bool requireCivilianEquipment, bool spawnBOthSidesWithHorse, Action<CharacterObject> onDuelEndAction, float customAgentHealth)
		{
			return SandBoxMissions.OpenArenaDuelMission(scene, location, duelCharacter, requireCivilianEquipment, spawnBOthSidesWithHorse, onDuelEndAction, customAgentHealth, "");
		}

		// Token: 0x06000094 RID: 148 RVA: 0x00005364 File Offset: 0x00003564
		IMission CampaignMission.ICampaignMissionManager.OpenConversationMission(ConversationCharacterData playerCharacterData, ConversationCharacterData conversationPartnerData, string specialScene, string sceneLevels)
		{
			return SandBoxMissions.OpenConversationMission(playerCharacterData, conversationPartnerData, specialScene, sceneLevels);
		}

		// Token: 0x06000095 RID: 149 RVA: 0x00005370 File Offset: 0x00003570
		IMission CampaignMission.ICampaignMissionManager.OpenMeetingMission(string scene, CharacterObject character)
		{
			return SandBoxMissions.OpenMeetingMission(scene, character);
		}

		// Token: 0x06000096 RID: 150 RVA: 0x00005379 File Offset: 0x00003579
		IMission CampaignMission.ICampaignMissionManager.OpenEquipmentTestMission(string scene)
		{
			return TestMissions.OpenEquipmentTestMission(scene);
		}

		// Token: 0x06000097 RID: 151 RVA: 0x00005381 File Offset: 0x00003581
		IMission CampaignMission.ICampaignMissionManager.OpenRetirementMission(string scene, Location location, CharacterObject talkToChar, string sceneLevels)
		{
			return SandBoxMissions.OpenRetirementMission(scene, location, talkToChar, sceneLevels);
		}
	}
}
