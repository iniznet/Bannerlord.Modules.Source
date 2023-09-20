using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Conversation;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem.Settlements.Locations;
using TaleWorlds.Core;

namespace SandBox
{
	public class CampaignMissionManager : CampaignMission.ICampaignMissionManager
	{
		IMission CampaignMission.ICampaignMissionManager.OpenSiegeMissionWithDeployment(string scene, float[] wallHitPointsPercentages, bool hasAnySiegeTower, List<MissionSiegeWeapon> siegeWeaponsOfAttackers, List<MissionSiegeWeapon> siegeWeaponsOfDefenders, bool isPlayerAttacker, int upgradeLevel, bool isSallyOut, bool isReliefForceAttack)
		{
			return SandBoxMissions.OpenSiegeMissionWithDeployment(scene, wallHitPointsPercentages, hasAnySiegeTower, siegeWeaponsOfAttackers, siegeWeaponsOfDefenders, isPlayerAttacker, upgradeLevel, isSallyOut, isReliefForceAttack);
		}

		IMission CampaignMission.ICampaignMissionManager.OpenSiegeMissionNoDeployment(string scene, bool isSallyOut, bool isReliefForceAttack)
		{
			return SandBoxMissions.OpenSiegeMissionNoDeployment(scene, isSallyOut, isReliefForceAttack);
		}

		IMission CampaignMission.ICampaignMissionManager.OpenSiegeLordsHallFightMission(string scene, FlattenedTroopRoster attackerPriorityList)
		{
			return SandBoxMissions.OpenSiegeLordsHallFightMission(scene, attackerPriorityList);
		}

		IMission CampaignMission.ICampaignMissionManager.OpenBattleMission(MissionInitializerRecord rec)
		{
			return SandBoxMissions.OpenBattleMission(rec);
		}

		IMission CampaignMission.ICampaignMissionManager.OpenCaravanBattleMission(MissionInitializerRecord rec, bool isCaravan)
		{
			return SandBoxMissions.OpenCaravanBattleMission(rec, isCaravan);
		}

		IMission CampaignMission.ICampaignMissionManager.OpenBattleMission(string scene)
		{
			return SandBoxMissions.OpenBattleMission(scene);
		}

		IMission CampaignMission.ICampaignMissionManager.OpenAlleyFightMission(string scene, int upgradeLevel, Location location, TroopRoster playerSideTroops, TroopRoster rivalSideTroops)
		{
			return SandBoxMissions.OpenAlleyFightMission(scene, upgradeLevel, location, playerSideTroops, rivalSideTroops);
		}

		IMission CampaignMission.ICampaignMissionManager.OpenCombatMissionWithDialogue(string scene, CharacterObject characterToTalkTo, CharacterObject allyTroopsWithFixedTeam, int upgradeLevel)
		{
			return SandBoxMissions.OpenCombatMissionWithDialogue(scene, characterToTalkTo, allyTroopsWithFixedTeam, upgradeLevel);
		}

		IMission CampaignMission.ICampaignMissionManager.OpenBattleMissionWhileEnteringSettlement(string scene, int upgradeLevel, int numberOfMaxTroopToBeSpawnedForPlayer, int numberOfMaxTroopToBeSpawnedForOpponent)
		{
			return SandBoxMissions.OpenBattleMissionWhileEnteringSettlement(scene, upgradeLevel, numberOfMaxTroopToBeSpawnedForPlayer, numberOfMaxTroopToBeSpawnedForOpponent);
		}

		IMission CampaignMission.ICampaignMissionManager.OpenConversatonTestMission(string scene)
		{
			return TestMissions.OpenFacialAnimTestMission(scene, "");
		}

		IMission CampaignMission.ICampaignMissionManager.OpenHideoutBattleMission(string scene, FlattenedTroopRoster playerTroops)
		{
			return SandBoxMissions.OpenHideoutBattleMission(scene, playerTroops);
		}

		IMission CampaignMission.ICampaignMissionManager.OpenTownCenterMission(string scene, int townUpgradeLevel, Location location, CharacterObject talkToChar, string playerSpawnTag)
		{
			return SandBoxMissions.OpenTownCenterMission(scene, townUpgradeLevel, location, talkToChar, playerSpawnTag);
		}

		IMission CampaignMission.ICampaignMissionManager.OpenCastleCourtyardMission(string scene, int castleUpgradeLevel, Location location, CharacterObject talkToChar)
		{
			return SandBoxMissions.OpenCastleCourtyardMission(scene, castleUpgradeLevel, location, talkToChar);
		}

		IMission CampaignMission.ICampaignMissionManager.OpenVillageMission(string scene, Location location, CharacterObject talkToChar)
		{
			return SandBoxMissions.OpenVillageMission(scene, location, talkToChar, null);
		}

		IMission CampaignMission.ICampaignMissionManager.OpenIndoorMission(string scene, int upgradeLevel, Location location, CharacterObject talkToChar)
		{
			return SandBoxMissions.OpenIndoorMission(scene, upgradeLevel, location, talkToChar);
		}

		IMission CampaignMission.ICampaignMissionManager.OpenPrisonBreakMission(string scene, Location location, CharacterObject prisonerCharacter, CharacterObject companionCharacter)
		{
			return SandBoxMissions.OpenPrisonBreakMission(scene, location, prisonerCharacter, companionCharacter);
		}

		IMission CampaignMission.ICampaignMissionManager.OpenArenaStartMission(string scene, Location location, CharacterObject talkToChar)
		{
			return SandBoxMissions.OpenArenaStartMission(scene, location, talkToChar, "");
		}

		public IMission OpenArenaDuelMission(string scene, Location location, CharacterObject duelCharacter, bool requireCivilianEquipment, bool spawnBOthSidesWithHorse, Action<CharacterObject> onDuelEndAction, float customAgentHealth)
		{
			return SandBoxMissions.OpenArenaDuelMission(scene, location, duelCharacter, requireCivilianEquipment, spawnBOthSidesWithHorse, onDuelEndAction, customAgentHealth, "");
		}

		IMission CampaignMission.ICampaignMissionManager.OpenConversationMission(ConversationCharacterData playerCharacterData, ConversationCharacterData conversationPartnerData, string specialScene, string sceneLevels)
		{
			return SandBoxMissions.OpenConversationMission(playerCharacterData, conversationPartnerData, specialScene, sceneLevels);
		}

		IMission CampaignMission.ICampaignMissionManager.OpenMeetingMission(string scene, CharacterObject character)
		{
			return SandBoxMissions.OpenMeetingMission(scene, character);
		}

		IMission CampaignMission.ICampaignMissionManager.OpenEquipmentTestMission(string scene)
		{
			return TestMissions.OpenEquipmentTestMission(scene);
		}

		IMission CampaignMission.ICampaignMissionManager.OpenRetirementMission(string scene, Location location, CharacterObject talkToChar, string sceneLevels)
		{
			return SandBoxMissions.OpenRetirementMission(scene, location, talkToChar, sceneLevels);
		}
	}
}
