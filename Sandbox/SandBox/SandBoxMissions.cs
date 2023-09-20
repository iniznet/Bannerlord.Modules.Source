using System;
using System.Collections.Generic;
using System.Linq;
using Helpers;
using SandBox.Conversation.MissionLogics;
using SandBox.Missions.AgentBehaviors;
using SandBox.Missions.AgentControllers;
using SandBox.Missions.Handlers;
using SandBox.Missions.MissionLogics;
using SandBox.Missions.MissionLogics.Arena;
using SandBox.Missions.MissionLogics.Towns;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Conversation;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Settlements.Locations;
using TaleWorlds.CampaignSystem.TroopSuppliers;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.LinQuick;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Missions.Handlers;
using TaleWorlds.MountAndBlade.Source.Missions;
using TaleWorlds.MountAndBlade.Source.Missions.Handlers;
using TaleWorlds.MountAndBlade.Source.Missions.Handlers.Logic;

namespace SandBox
{
	[MissionManager]
	public static class SandBoxMissions
	{
		public static MissionInitializerRecord CreateSandBoxMissionInitializerRecord(string sceneName, string sceneLevels = "", bool doNotUseLoadingScreen = false, DecalAtlasGroup decalAtlasGroup = 0)
		{
			MissionInitializerRecord missionInitializerRecord;
			missionInitializerRecord..ctor(sceneName);
			missionInitializerRecord.DamageToPlayerMultiplier = Campaign.Current.Models.DifficultyModel.GetDamageToPlayerMultiplier();
			missionInitializerRecord.DamageToFriendsMultiplier = Campaign.Current.Models.DifficultyModel.GetPlayerTroopsReceivedDamageMultiplier();
			missionInitializerRecord.DamageFromPlayerToFriendsMultiplier = Campaign.Current.Models.DifficultyModel.GetPlayerTroopsReceivedDamageMultiplier();
			missionInitializerRecord.PlayingInCampaignMode = Campaign.Current.GameMode == 1;
			missionInitializerRecord.AtmosphereOnCampaign = ((Campaign.Current.GameMode == 1) ? Campaign.Current.Models.MapWeatherModel.GetAtmosphereModel(CampaignTime.Now, MobileParty.MainParty.GetLogicalPosition()) : null);
			missionInitializerRecord.TerrainType = Campaign.Current.MapSceneWrapper.GetFaceTerrainType(MobileParty.MainParty.CurrentNavigationFace);
			missionInitializerRecord.SceneLevels = sceneLevels;
			missionInitializerRecord.DoNotUseLoadingScreen = doNotUseLoadingScreen;
			missionInitializerRecord.AtlasGroup = decalAtlasGroup;
			return missionInitializerRecord;
		}

		public static MissionInitializerRecord CreateSandBoxTrainingMissionInitializerRecord(string sceneName, string sceneLevels = "", bool doNotUseLoadingScreen = false)
		{
			MissionInitializerRecord missionInitializerRecord;
			missionInitializerRecord..ctor(sceneName);
			missionInitializerRecord.DamageToPlayerMultiplier = Campaign.Current.Models.DifficultyModel.GetDamageToPlayerMultiplier();
			missionInitializerRecord.DamageToFriendsMultiplier = Campaign.Current.Models.DifficultyModel.GetPlayerTroopsReceivedDamageMultiplier();
			missionInitializerRecord.DamageFromPlayerToFriendsMultiplier = 1f;
			missionInitializerRecord.PlayingInCampaignMode = Campaign.Current.GameMode == 1;
			missionInitializerRecord.AtmosphereOnCampaign = ((Campaign.Current.GameMode == 1) ? Campaign.Current.Models.MapWeatherModel.GetAtmosphereModel(CampaignTime.Now, MobileParty.MainParty.GetLogicalPosition()) : null);
			missionInitializerRecord.TerrainType = Campaign.Current.MapSceneWrapper.GetFaceTerrainType(MobileParty.MainParty.CurrentNavigationFace);
			missionInitializerRecord.SceneLevels = sceneLevels;
			missionInitializerRecord.DoNotUseLoadingScreen = doNotUseLoadingScreen;
			return missionInitializerRecord;
		}

		[MissionMethod]
		public static Mission OpenTownCenterMission(string scene, int townUpgradeLevel, Location location, CharacterObject talkToChar, string playerSpawnTag)
		{
			string civilianUpgradeLevelTag = Campaign.Current.Models.LocationModel.GetCivilianUpgradeLevelTag(townUpgradeLevel);
			return SandBoxMissions.OpenTownCenterMission(scene, civilianUpgradeLevelTag, location, talkToChar, playerSpawnTag);
		}

		[MissionMethod]
		public static Mission OpenTownCenterMission(string scene, string sceneLevels, Location location, CharacterObject talkToChar, string playerSpawnTag)
		{
			return MissionState.OpenNew("TownCenter", SandBoxMissions.CreateSandBoxMissionInitializerRecord(scene, sceneLevels, false, 3), (Mission mission) => new MissionBehavior[]
			{
				new MissionOptionsComponent(),
				new CampaignMissionComponent(),
				new MissionBasicTeamLogic(),
				new MissionSettlementPrepareLogic(),
				new TownCenterMissionController(),
				new MissionAgentLookHandler(),
				new SandBoxMissionHandler(),
				new WorkshopMissionHandler(SandBoxMissions.GetCurrentTown()),
				new BasicLeaveMissionLogic(),
				new LeaveMissionLogic(),
				new BattleAgentLogic(),
				new MountAgentLogic(),
				new NotableSpawnPointHandler(),
				new MissionAgentPanicHandler(),
				new AgentHumanAILogic(),
				new MissionAlleyHandler(),
				new MissionCrimeHandler(),
				new MissionConversationLogic(talkToChar),
				new MissionAgentHandler(location, playerSpawnTag),
				new HeroSkillHandler(),
				new MissionFightHandler(),
				new MissionFacialAnimationHandler(),
				new MissionDebugHandler(),
				new MissionHardBorderPlacer(),
				new MissionBoundaryPlacer(),
				new MissionBoundaryCrossingHandler(),
				new VisualTrackerMissionBehavior(),
				new EquipmentControllerLeaveLogic()
			}, true, true);
		}

		[MissionMethod]
		public static Mission OpenCastleCourtyardMission(string scene, int castleUpgradeLevel, Location location, CharacterObject talkToChar)
		{
			string civilianUpgradeLevelTag = Campaign.Current.Models.LocationModel.GetCivilianUpgradeLevelTag(castleUpgradeLevel);
			return SandBoxMissions.OpenCastleCourtyardMission(scene, civilianUpgradeLevelTag, location, talkToChar);
		}

		[MissionMethod]
		public static Mission OpenCastleCourtyardMission(string scene, string sceneLevels, Location location, CharacterObject talkToChar)
		{
			return MissionState.OpenNew("TownCenter", SandBoxMissions.CreateSandBoxMissionInitializerRecord(scene, sceneLevels, false, 3), delegate(Mission mission)
			{
				List<MissionBehavior> list = new List<MissionBehavior>();
				list.Add(new MissionOptionsComponent());
				list.Add(new CampaignMissionComponent());
				list.Add(new MissionBasicTeamLogic());
				list.Add(new MissionSettlementPrepareLogic());
				list.Add(new TownCenterMissionController());
				list.Add(new MissionAgentLookHandler());
				list.Add(new SandBoxMissionHandler());
				list.Add(new BasicLeaveMissionLogic());
				list.Add(new LeaveMissionLogic());
				list.Add(new BattleAgentLogic());
				list.Add(new MountAgentLogic());
				Settlement currentTown = SandBoxMissions.GetCurrentTown();
				if (currentTown != null)
				{
					list.Add(new WorkshopMissionHandler(currentTown));
				}
				list.Add(new MissionAgentPanicHandler());
				list.Add(new AgentHumanAILogic());
				list.Add(new MissionConversationLogic(talkToChar));
				list.Add(new MissionAgentHandler(location, null));
				list.Add(new HeroSkillHandler());
				list.Add(new MissionFightHandler());
				list.Add(new MissionFacialAnimationHandler());
				list.Add(new MissionDebugHandler());
				list.Add(new MissionHardBorderPlacer());
				list.Add(new MissionBoundaryPlacer());
				list.Add(new EquipmentControllerLeaveLogic());
				list.Add(new MissionBoundaryCrossingHandler());
				list.Add(new VisualTrackerMissionBehavior());
				return list.ToArray();
			}, true, true);
		}

		[MissionMethod]
		public static Mission OpenIndoorMission(string scene, int townUpgradeLevel, Location location, CharacterObject talkToChar)
		{
			string civilianUpgradeLevelTag = Campaign.Current.Models.LocationModel.GetCivilianUpgradeLevelTag(townUpgradeLevel);
			return SandBoxMissions.OpenIndoorMission(scene, location, talkToChar, civilianUpgradeLevelTag);
		}

		[MissionMethod]
		public static Mission OpenIndoorMission(string scene, Location location, CharacterObject talkToChar = null, string sceneLevels = "")
		{
			return MissionState.OpenNew("Indoor", SandBoxMissions.CreateSandBoxMissionInitializerRecord(scene, sceneLevels, true, 3), (Mission mission) => new MissionBehavior[]
			{
				new MissionOptionsComponent(),
				new CampaignMissionComponent(),
				new MissionBasicTeamLogic(),
				new BasicLeaveMissionLogic(),
				new LeaveMissionLogic(),
				new SandBoxMissionHandler(),
				new MissionAgentLookHandler(),
				new MissionConversationLogic(talkToChar),
				new MissionAgentHandler(location, null),
				new HeroSkillHandler(),
				new MissionFightHandler(),
				new BattleAgentLogic(),
				new MountAgentLogic(),
				new AgentHumanAILogic(),
				new MissionCrimeHandler(),
				new MissionFacialAnimationHandler(),
				new MissionDebugHandler(),
				new LocationItemSpawnHandler(),
				new IndoorMissionController(),
				new VisualTrackerMissionBehavior(),
				new EquipmentControllerLeaveLogic(),
				new BattleSurgeonLogic()
			}, true, true);
		}

		[MissionMethod]
		public static Mission OpenPrisonBreakMission(string scene, Location location, CharacterObject prisonerCharacter, CharacterObject companionCharacter = null)
		{
			Mission mission2 = MissionState.OpenNew("PrisonBreak", SandBoxMissions.CreateSandBoxMissionInitializerRecord(scene, "prison_break", true, 3), delegate(Mission mission)
			{
				List<MissionBehavior> list = new List<MissionBehavior>();
				list.Add(new MissionOptionsComponent());
				list.Add(new CampaignMissionComponent());
				list.Add(new MissionBasicTeamLogic());
				list.Add(new BasicLeaveMissionLogic());
				list.Add(new LeaveMissionLogic());
				list.Add(new SandBoxMissionHandler());
				list.Add(new MissionAgentLookHandler());
				list.Add(new MissionAgentHandler(location, "sp_prison_break"));
				list.Add(new HeroSkillHandler());
				list.Add(new MissionFightHandler());
				list.Add(new BattleAgentLogic());
				list.Add(new AgentHumanAILogic());
				list.Add(new MissionCrimeHandler());
				list.Add(new MissionFacialAnimationHandler());
				list.Add(new LocationItemSpawnHandler());
				list.Add(new PrisonBreakMissionController(prisonerCharacter, companionCharacter));
				list.Add(new VisualTrackerMissionBehavior());
				list.Add(new EquipmentControllerLeaveLogic());
				list.Add(new BattleSurgeonLogic());
				if (Game.Current.IsDevelopmentMode)
				{
					list.Add(new MissionDebugHandler());
				}
				return list.ToArray();
			}, true, true);
			mission2.ForceNoFriendlyFire = true;
			return mission2;
		}

		[MissionMethod]
		public static Mission OpenVillageMission(string scene, Location location, CharacterObject talkToChar = null, string sceneLevels = null)
		{
			return MissionState.OpenNew("Village", SandBoxMissions.CreateSandBoxMissionInitializerRecord(scene, sceneLevels, false, 3), (Mission mission) => new MissionBehavior[]
			{
				new MissionOptionsComponent(),
				new CampaignMissionComponent(),
				new MissionBasicTeamLogic(),
				new VillageMissionController(),
				new NotableSpawnPointHandler(),
				new BasicLeaveMissionLogic(),
				new LeaveMissionLogic(),
				new MissionAgentLookHandler(),
				new SandBoxMissionHandler(),
				new MissionConversationLogic(talkToChar),
				new MissionFightHandler(),
				new MissionAgentHandler(location, null),
				new MissionAlleyHandler(),
				new HeroSkillHandler(),
				new MissionFacialAnimationHandler(),
				new MissionAgentPanicHandler(),
				new BattleAgentLogic(),
				new MountAgentLogic(),
				new AgentHumanAILogic(),
				new MissionCrimeHandler(),
				new MissionDebugHandler(),
				new MissionHardBorderPlacer(),
				new MissionBoundaryPlacer(),
				new EquipmentControllerLeaveLogic(),
				new MissionBoundaryCrossingHandler(),
				new VisualTrackerMissionBehavior(),
				new BattleSurgeonLogic()
			}, true, true);
		}

		[MissionMethod]
		public static Mission OpenArenaStartMission(string scene, Location location, CharacterObject talkToChar = null, string sceneLevels = "")
		{
			return MissionState.OpenNew("ArenaPracticeFight", SandBoxMissions.CreateSandBoxMissionInitializerRecord(scene, sceneLevels, false, 3), (Mission mission) => new MissionBehavior[]
			{
				new MissionOptionsComponent(),
				new EquipmentControllerLeaveLogic(),
				new ArenaPracticeFightMissionController(),
				new BasicLeaveMissionLogic(),
				new MissionConversationLogic(talkToChar),
				new HeroSkillHandler(),
				new MissionFacialAnimationHandler(),
				new MissionDebugHandler(),
				new MissionAgentPanicHandler(),
				new AgentHumanAILogic(),
				new ArenaAgentStateDeciderLogic(),
				new VisualTrackerMissionBehavior(),
				new CampaignMissionComponent(),
				new MissionAgentHandler(location, null)
			}, true, true);
		}

		[MissionMethod]
		public static Mission OpenRetirementMission(string scene, Location location, CharacterObject talkToChar = null, string sceneLevels = null)
		{
			return MissionState.OpenNew("Retirement", SandBoxMissions.CreateSandBoxMissionInitializerRecord(scene, sceneLevels, false, 3), (Mission mission) => new MissionBehavior[]
			{
				new MissionOptionsComponent(),
				new CampaignMissionComponent(),
				new MissionBasicTeamLogic(),
				new VillageMissionController(),
				new NotableSpawnPointHandler(),
				new BasicLeaveMissionLogic(),
				new MissionAgentLookHandler(),
				new MissionConversationLogic(talkToChar),
				new MissionFightHandler(),
				new MissionAgentHandler(location, null),
				new MissionAlleyHandler(),
				new HeroSkillHandler(),
				new MissionFacialAnimationHandler(),
				new MissionAgentPanicHandler(),
				new MountAgentLogic(),
				new AgentHumanAILogic(),
				new MissionCrimeHandler(),
				new MissionHardBorderPlacer(),
				new MissionBoundaryPlacer(),
				new EquipmentControllerLeaveLogic(),
				new MissionBoundaryCrossingHandler(),
				new VisualTrackerMissionBehavior(),
				new BattleSurgeonLogic(),
				new RetirementMissionLogic(),
				new LeaveMissionLogic()
			}, true, true);
		}

		[MissionMethod]
		public static Mission OpenArenaDuelMission(string scene, Location location, CharacterObject duelCharacter, bool requireCivilianEquipment, bool spawnBOthSidesWithHorse, Action<CharacterObject> onDuelEnd, float customAgentHealth, string sceneLevels = "")
		{
			return MissionState.OpenNew("ArenaDuelMission", SandBoxMissions.CreateSandBoxMissionInitializerRecord(scene, sceneLevels, false, 3), (Mission mission) => new MissionBehavior[]
			{
				new MissionOptionsComponent(),
				new ArenaDuelMissionController(duelCharacter, requireCivilianEquipment, spawnBOthSidesWithHorse, onDuelEnd, customAgentHealth),
				new MissionFacialAnimationHandler(),
				new MissionDebugHandler(),
				new MissionAgentPanicHandler(),
				new AgentHumanAILogic(),
				new ArenaAgentStateDeciderLogic(),
				new VisualTrackerMissionBehavior(),
				new CampaignMissionComponent(),
				new EquipmentControllerLeaveLogic(),
				new MissionAgentHandler(location, null)
			}, true, true);
		}

		[MissionMethod]
		public static Mission OpenArenaDuelMission(string scene, Location location)
		{
			return MissionState.OpenNew("ArenaDuel", SandBoxMissions.CreateSandBoxMissionInitializerRecord(scene, "", false, 3), (Mission mission) => new MissionBehavior[]
			{
				new MissionOptionsComponent(),
				new CampaignMissionComponent(),
				new ArenaDuelMissionBehavior(),
				new BasicLeaveMissionLogic(),
				new MissionAgentHandler(location, null),
				new HeroSkillHandler(),
				new MissionFacialAnimationHandler(),
				new MissionDebugHandler(),
				new MissionAgentPanicHandler(),
				new AgentHumanAILogic(),
				new EquipmentControllerLeaveLogic(),
				new ArenaAgentStateDeciderLogic()
			}, true, true);
		}

		[MissionMethod]
		public static Mission OpenBattleMission(MissionInitializerRecord rec)
		{
			bool isPlayerSergeant = MobileParty.MainParty.MapEvent.IsPlayerSergeant();
			bool isPlayerInArmy = MobileParty.MainParty.Army != null;
			List<string> heroesOnPlayerSideByPriority = HeroHelper.OrderHeroesOnPlayerSideByPriority();
			bool isPlayerAttacker = !Extensions.IsEmpty<MapEventParty>(MobileParty.MainParty.MapEvent.AttackerSide.Parties.Where((MapEventParty p) => p.Party == MobileParty.MainParty.Party));
			if (isPlayerInArmy)
			{
				bool flag = MobileParty.MainParty.Army.LeaderParty == MobileParty.MainParty;
			}
			return MissionState.OpenNew("Battle", rec, delegate(Mission mission)
			{
				MissionBehavior[] array = new MissionBehavior[29];
				array[0] = SandBoxMissions.CreateCampaignMissionAgentSpawnLogic(0, null, null);
				array[1] = new BattlePowerCalculationLogic();
				array[2] = new BattleSpawnLogic("battle_set");
				array[3] = new SandBoxBattleMissionSpawnHandler();
				array[4] = new CampaignMissionComponent();
				array[5] = new BattleAgentLogic();
				array[6] = new MountAgentLogic();
				array[7] = new BannerBearerLogic();
				array[8] = new MissionOptionsComponent();
				array[9] = new BattleEndLogic();
				array[10] = new BattleReinforcementsSpawnController();
				array[11] = new MissionCombatantsLogic(MobileParty.MainParty.MapEvent.InvolvedParties, PartyBase.MainParty, MobileParty.MainParty.MapEvent.GetLeaderParty(0), MobileParty.MainParty.MapEvent.GetLeaderParty(1), 1, isPlayerSergeant);
				array[12] = new BattleObserverMissionLogic();
				array[13] = new AgentHumanAILogic();
				array[14] = new AgentVictoryLogic();
				array[15] = new BattleSurgeonLogic();
				array[16] = new MissionAgentPanicHandler();
				array[17] = new BattleMissionAgentInteractionLogic();
				array[18] = new AgentMoraleInteractionLogic();
				array[19] = new AssignPlayerRoleInTeamMissionController(!isPlayerSergeant, isPlayerSergeant, isPlayerInArmy, heroesOnPlayerSideByPriority, 8);
				int num = 20;
				Hero leaderHero = MapEvent.PlayerMapEvent.AttackerSide.LeaderParty.LeaderHero;
				TextObject textObject = ((leaderHero != null) ? leaderHero.Name : null);
				Hero leaderHero2 = MapEvent.PlayerMapEvent.DefenderSide.LeaderParty.LeaderHero;
				array[num] = new SandboxGeneralsAndCaptainsAssignmentLogic(textObject, (leaderHero2 != null) ? leaderHero2.Name : null, null, null, true);
				array[21] = new EquipmentControllerLeaveLogic();
				array[22] = new MissionHardBorderPlacer();
				array[23] = new MissionBoundaryPlacer();
				array[24] = new MissionBoundaryCrossingHandler();
				array[25] = new HighlightsController();
				array[26] = new BattleHighlightsController();
				array[27] = new DeploymentMissionController(isPlayerAttacker);
				array[28] = new BattleDeploymentHandler(isPlayerAttacker);
				return array;
			}, true, true);
		}

		[MissionMethod]
		public static Mission OpenCaravanBattleMission(MissionInitializerRecord rec, bool isCaravan)
		{
			bool isPlayerAttacker = !Extensions.IsEmpty<MapEventParty>(MobileParty.MainParty.MapEvent.AttackerSide.Parties.Where((MapEventParty p) => p.Party == MobileParty.MainParty.Party));
			bool isPlayerSergeant = MobileParty.MainParty.MapEvent.IsPlayerSergeant();
			bool isPlayerInArmy = MobileParty.MainParty.Army != null;
			return MissionState.OpenNew("Battle", rec, delegate(Mission mission)
			{
				MissionBehavior[] array = new MissionBehavior[32];
				array[0] = new MissionOptionsComponent();
				array[1] = new CampaignMissionComponent();
				array[2] = new BattleEndLogic();
				array[3] = new BattleReinforcementsSpawnController();
				array[4] = new BannerBearerLogic();
				array[5] = new MissionCombatantsLogic(MobileParty.MainParty.MapEvent.InvolvedParties, PartyBase.MainParty, MobileParty.MainParty.MapEvent.GetLeaderParty(0), MobileParty.MainParty.MapEvent.GetLeaderParty(1), 1, isPlayerSergeant);
				array[6] = new BattleSpawnLogic("battle_set");
				array[7] = new AgentHumanAILogic();
				array[8] = SandBoxMissions.CreateCampaignMissionAgentSpawnLogic(0, null, null);
				array[9] = new BattlePowerCalculationLogic();
				array[10] = new SandBoxBattleMissionSpawnHandler();
				array[11] = new BattleObserverMissionLogic();
				array[12] = new BattleAgentLogic();
				array[13] = new MountAgentLogic();
				array[14] = new AgentVictoryLogic();
				array[15] = new MissionDebugHandler();
				array[16] = new MissionAgentPanicHandler();
				array[17] = new MissionHardBorderPlacer();
				array[18] = new MissionBoundaryPlacer();
				array[19] = new MissionBoundaryCrossingHandler();
				array[20] = new BattleMissionAgentInteractionLogic();
				array[21] = new AgentMoraleInteractionLogic();
				array[22] = new HighlightsController();
				array[23] = new BattleHighlightsController();
				array[24] = new AssignPlayerRoleInTeamMissionController(!isPlayerSergeant, isPlayerSergeant, isPlayerInArmy, null, 8);
				int num = 25;
				Hero leaderHero = MapEvent.PlayerMapEvent.AttackerSide.LeaderParty.LeaderHero;
				TextObject textObject = ((leaderHero != null) ? leaderHero.Name : null);
				Hero leaderHero2 = MapEvent.PlayerMapEvent.DefenderSide.LeaderParty.LeaderHero;
				array[num] = new SandboxGeneralsAndCaptainsAssignmentLogic(textObject, (leaderHero2 != null) ? leaderHero2.Name : null, null, null, true);
				array[26] = new EquipmentControllerLeaveLogic();
				array[27] = new MissionCaravanOrVillagerTacticsHandler();
				array[28] = new CaravanBattleMissionHandler(MathF.Min(MapEvent.PlayerMapEvent.InvolvedParties.Where((PartyBase ip) => ip.Side == 1).Sum((PartyBase ip) => ip.MobileParty.Party.MemberRoster.TotalManCount - ip.MobileParty.Party.MemberRoster.TotalWounded), MapEvent.PlayerMapEvent.InvolvedParties.Where((PartyBase ip) => ip.Side == 0).Sum((PartyBase ip) => ip.MobileParty.Party.MemberRoster.TotalManCount - ip.MobileParty.Party.MemberRoster.TotalWounded)), MapEvent.PlayerMapEvent.InvolvedParties.Any((PartyBase ip) => (ip.MobileParty.IsCaravan || ip.MobileParty.IsVillager) && (ip.Culture.StringId == "aserai" || ip.Culture.StringId == "khuzait")), isCaravan);
				array[29] = new BattleDeploymentHandler(isPlayerAttacker);
				array[30] = new DeploymentMissionController(isPlayerAttacker);
				array[31] = new BattleSurgeonLogic();
				return array;
			}, true, true);
		}

		[MissionMethod]
		public static Mission OpenAlleyFightMission(MissionInitializerRecord rec, Location location, TroopRoster playerSideTroops, TroopRoster rivalSideTroops)
		{
			return MissionState.OpenNew("AlleyFight", rec, delegate(Mission mission)
			{
				List<MissionBehavior> list = new List<MissionBehavior>();
				list.Add(new MissionOptionsComponent());
				list.Add(new BattleEndLogic());
				list.Add(new AgentHumanAILogic());
				list.Add(new BattlePowerCalculationLogic());
				list.Add(new CampaignMissionComponent());
				list.Add(new AlleyFightMissionHandler(playerSideTroops, rivalSideTroops));
				list.Add(new BattleObserverMissionLogic());
				list.Add(new AgentVictoryLogic());
				list.Add(new MissionHardBorderPlacer());
				list.Add(new MissionAgentHandler(location, null));
				list.Add(new MissionFightHandler());
				list.Add(new MissionBoundaryPlacer());
				list.Add(new MissionBoundaryCrossingHandler());
				list.Add(new BattleMissionAgentInteractionLogic());
				list.Add(new HighlightsController());
				list.Add(new BattleHighlightsController());
				list.Add(new EquipmentControllerLeaveLogic());
				Settlement currentTown = SandBoxMissions.GetCurrentTown();
				if (currentTown != null)
				{
					list.Add(new WorkshopMissionHandler(currentTown));
				}
				return list.ToArray();
			}, true, true);
		}

		[MissionMethod]
		public static Mission OpenCombatMissionWithDialogue(MissionInitializerRecord rec, CharacterObject characterToTalkTo, CharacterObject allyTroopsWithFixedTeam)
		{
			return MissionState.OpenNew("CombatWithDialogue", rec, delegate(Mission mission)
			{
				IMissionTroopSupplier[] array = new IMissionTroopSupplier[]
				{
					new PartyGroupTroopSupplier(PlayerEncounter.Battle, 0, null, null),
					new PartyGroupTroopSupplier(PlayerEncounter.Battle, 1, null, null)
				};
				List<MissionBehavior> list = new List<MissionBehavior>();
				list.Add(new MissionOptionsComponent());
				list.Add(new CampaignMissionComponent());
				list.Add(new BattleEndLogic());
				list.Add(new MissionCombatantsLogic(MobileParty.MainParty.MapEvent.InvolvedParties, PartyBase.MainParty, MobileParty.MainParty.MapEvent.GetLeaderParty(0), MobileParty.MainParty.MapEvent.GetLeaderParty(1), 0, false));
				list.Add(new BattleSpawnLogic("battle_set"));
				list.Add(new MissionAgentPanicHandler());
				list.Add(new AgentHumanAILogic());
				list.Add(new CombatMissionWithDialogueController(array, characterToTalkTo, allyTroopsWithFixedTeam));
				list.Add(new MissionConversationLogic(null));
				list.Add(new BattleObserverMissionLogic());
				list.Add(new BattleAgentLogic());
				list.Add(new AgentVictoryLogic());
				list.Add(new MissionDebugHandler());
				list.Add(new MissionHardBorderPlacer());
				list.Add(new MissionBoundaryPlacer());
				list.Add(new MissionBoundaryCrossingHandler());
				list.Add(new BattleMissionAgentInteractionLogic());
				list.Add(new HighlightsController());
				list.Add(new BattleHighlightsController());
				list.Add(new EquipmentControllerLeaveLogic());
				list.Add(new BattleSurgeonLogic());
				Settlement currentTown = SandBoxMissions.GetCurrentTown();
				if (currentTown != null)
				{
					list.Add(new WorkshopMissionHandler(currentTown));
				}
				return list.ToArray();
			}, true, true);
		}

		[MissionMethod]
		public static Mission OpenBattleMissionWhileEnteringSettlement(string scene, int upgradeLevel, int numberOfMaxTroopToBeSpawnedForPlayer, int numberOfMaxTroopToBeSpawnedForOpponent)
		{
			string text = "EnteringSettlementBattle";
			MissionInitializerRecord missionInitializerRecord;
			missionInitializerRecord..ctor(scene);
			missionInitializerRecord.PlayingInCampaignMode = Campaign.Current.GameMode == 1;
			missionInitializerRecord.AtmosphereOnCampaign = ((Campaign.Current.GameMode == 1) ? Campaign.Current.Models.MapWeatherModel.GetAtmosphereModel(CampaignTime.Now, MobileParty.MainParty.GetLogicalPosition()) : null);
			missionInitializerRecord.AtlasGroup = 3;
			missionInitializerRecord.SceneLevels = Campaign.Current.Models.LocationModel.GetCivilianUpgradeLevelTag(upgradeLevel);
			return MissionState.OpenNew(text, missionInitializerRecord, delegate(Mission mission)
			{
				IMissionTroopSupplier[] array = new IMissionTroopSupplier[]
				{
					new PartyGroupTroopSupplier(PlayerEncounter.Battle, 0, null, null),
					new PartyGroupTroopSupplier(PlayerEncounter.Battle, 1, null, null)
				};
				List<MissionBehavior> list = new List<MissionBehavior>();
				list.Add(new MissionOptionsComponent());
				list.Add(new CampaignMissionComponent());
				list.Add(new BattleEndLogic());
				list.Add(new MissionCombatantsLogic(MobileParty.MainParty.MapEvent.InvolvedParties, PartyBase.MainParty, MobileParty.MainParty.MapEvent.GetLeaderParty(0), MobileParty.MainParty.MapEvent.GetLeaderParty(1), 0, false));
				list.Add(new BattleSpawnLogic("battle_set"));
				list.Add(new MissionAgentPanicHandler());
				list.Add(new AgentHumanAILogic());
				list.Add(new BattleObserverMissionLogic());
				list.Add(new WhileEnteringSettlementBattleMissionController(array, numberOfMaxTroopToBeSpawnedForPlayer, numberOfMaxTroopToBeSpawnedForOpponent));
				list.Add(new MissionFightHandler());
				list.Add(new BattleAgentLogic());
				list.Add(new MountAgentLogic());
				list.Add(new AgentVictoryLogic());
				list.Add(new MissionDebugHandler());
				list.Add(new MissionHardBorderPlacer());
				list.Add(new MissionBoundaryPlacer());
				list.Add(new MissionBoundaryCrossingHandler());
				list.Add(new BattleMissionAgentInteractionLogic());
				list.Add(new HighlightsController());
				list.Add(new BattleHighlightsController());
				list.Add(new EquipmentControllerLeaveLogic());
				list.Add(new BattleSurgeonLogic());
				Settlement currentTown = SandBoxMissions.GetCurrentTown();
				if (currentTown != null)
				{
					list.Add(new WorkshopMissionHandler(currentTown));
				}
				return list.ToArray();
			}, true, true);
		}

		[MissionMethod]
		public static Mission OpenBattleMission(string scene)
		{
			return SandBoxMissions.OpenBattleMission(SandBoxMissions.CreateSandBoxMissionInitializerRecord(scene, "", false, 2));
		}

		[MissionMethod]
		public static Mission OpenAlleyFightMission(string scene, int upgradeLevel, Location location, TroopRoster playerSideTroops, TroopRoster rivalSideTroops)
		{
			return SandBoxMissions.OpenAlleyFightMission(SandBoxMissions.CreateSandBoxMissionInitializerRecord(scene, Campaign.Current.Models.LocationModel.GetCivilianUpgradeLevelTag(upgradeLevel), false, 3), location, playerSideTroops, rivalSideTroops);
		}

		[MissionMethod]
		public static Mission OpenCombatMissionWithDialogue(string scene, CharacterObject characterToTalkTo, CharacterObject allyTroopsWithFixedTeam, int upgradeLevel)
		{
			return SandBoxMissions.OpenCombatMissionWithDialogue(SandBoxMissions.CreateSandBoxMissionInitializerRecord(scene, Campaign.Current.Models.LocationModel.GetCivilianUpgradeLevelTag(upgradeLevel), false, 3), characterToTalkTo, allyTroopsWithFixedTeam);
		}

		[MissionMethod]
		public static Mission OpenHideoutBattleMission(string scene, FlattenedTroopRoster playerTroops)
		{
			int firstPhaseEnemySideTroopCount;
			FlattenedTroopRoster banditPriorityList = SandBoxMissions.GetPriorityListForHideoutMission(MapEvent.PlayerMapEvent, 0, out firstPhaseEnemySideTroopCount);
			FlattenedTroopRoster playerPriorityList = playerTroops ?? MobilePartyHelper.GetStrongestAndPriorTroops(MobileParty.MainParty, Campaign.Current.Models.BanditDensityModel.GetPlayerMaximumTroopCountForHideoutMission(MobileParty.MainParty), true).ToFlattenedRoster();
			int firstPhasePlayerSideTroopCount = playerPriorityList.Count<FlattenedTroopRosterElement>();
			return MissionState.OpenNew("HideoutBattle", SandBoxMissions.CreateSandBoxMissionInitializerRecord(scene, "", false, 3), delegate(Mission mission)
			{
				IMissionTroopSupplier[] array = new IMissionTroopSupplier[]
				{
					new PartyGroupTroopSupplier(MapEvent.PlayerMapEvent, 0, banditPriorityList, null),
					new PartyGroupTroopSupplier(MapEvent.PlayerMapEvent, 1, playerPriorityList, null)
				};
				return new MissionBehavior[]
				{
					new MissionOptionsComponent(),
					new CampaignMissionComponent(),
					new BattleEndLogic(),
					new MissionCombatantsLogic(MobileParty.MainParty.MapEvent.InvolvedParties, PartyBase.MainParty, MobileParty.MainParty.MapEvent.GetLeaderParty(0), MobileParty.MainParty.MapEvent.GetLeaderParty(1), 0, false),
					new AgentHumanAILogic(),
					new HideoutCinematicController(),
					new MissionConversationLogic(),
					new HideoutMissionController(array, PartyBase.MainParty.Side, firstPhaseEnemySideTroopCount, firstPhasePlayerSideTroopCount),
					new BattleObserverMissionLogic(),
					new BattleAgentLogic(),
					new MountAgentLogic(),
					new AgentVictoryLogic(),
					new MissionDebugHandler(),
					new MissionAgentPanicHandler(),
					new MissionHardBorderPlacer(),
					new MissionBoundaryPlacer(),
					new MissionBoundaryCrossingHandler(),
					new AgentMoraleInteractionLogic(),
					new HighlightsController(),
					new BattleHighlightsController(),
					new EquipmentControllerLeaveLogic(),
					new BattleSurgeonLogic()
				};
			}, true, true);
		}

		private static FlattenedTroopRoster GetPriorityListForHideoutMission(MapEvent playerMapEvent, BattleSideEnum side, out int firstPhaseTroopCount)
		{
			List<MapEventParty> list = LinQuick.WhereQ<MapEventParty>(playerMapEvent.PartiesOnSide(side), (MapEventParty x) => x.Party.IsMobile).ToList<MapEventParty>();
			int num = LinQuick.SumQ<MapEventParty>(list, (MapEventParty x) => x.Party.MemberRoster.TotalHealthyCount);
			firstPhaseTroopCount = MathF.Min(MathF.Floor((float)num * Campaign.Current.Models.BanditDensityModel.SpawnPercentageForFirstFightInHideoutMission), Campaign.Current.Models.BanditDensityModel.NumberOfMaximumTroopCountForFirstFightInHideout);
			int num2 = num - firstPhaseTroopCount;
			FlattenedTroopRoster flattenedTroopRoster = new FlattenedTroopRoster(num);
			foreach (MapEventParty mapEventParty in list)
			{
				flattenedTroopRoster.Add(mapEventParty.Party.MemberRoster.GetTroopRoster());
			}
			flattenedTroopRoster.RemoveIf((FlattenedTroopRosterElement x) => x.IsWounded);
			int count = flattenedTroopRoster.RemoveIf((FlattenedTroopRosterElement x) => x.Troop.IsHero || x.Troop.Culture.BanditBoss == x.Troop).ToList<FlattenedTroopRosterElement>().Count;
			int num3 = 0;
			int num4 = num2 - count;
			if (num4 > 0)
			{
				IEnumerable<FlattenedTroopRosterElement> selectedRegularTroops = flattenedTroopRoster.OrderByDescending((FlattenedTroopRosterElement x) => x.Troop.Level).Take(num4);
				flattenedTroopRoster.RemoveIf((FlattenedTroopRosterElement x) => selectedRegularTroops.Contains(x));
				num3 += selectedRegularTroops.Count<FlattenedTroopRosterElement>();
			}
			Debug.Print("Picking bandit troops for hideout mission...", 0, 9, 256UL);
			Debug.Print("- First phase troop count: " + firstPhaseTroopCount, 0, 9, 256UL);
			Debug.Print("- Second phase boss troop count: " + count, 0, 9, 256UL);
			Debug.Print("- Second phase regular troop count: " + num3, 0, 9, 256UL);
			return flattenedTroopRoster;
		}

		[MissionMethod]
		public static Mission OpenAmbushMission(string scene, MissionResult oldResult)
		{
			Debug.FailedAssert("This mission was broken", "C:\\Develop\\MB3\\Source\\Bannerlord\\SandBox\\Missions\\SandBoxMissions.cs", "OpenAmbushMission", 858);
			return MissionState.OpenNew("Ambush", SandBoxMissions.CreateSandBoxMissionInitializerRecord(scene, "", false, 3), (Mission mission) => new MissionBehavior[]
			{
				new MissionOptionsComponent(),
				new CampaignMissionComponent(),
				new BattleEndLogic(),
				new BattleReinforcementsSpawnController(),
				new MissionCombatantsLogic(MobileParty.MainParty.MapEvent.InvolvedParties, PartyBase.MainParty, MobileParty.MainParty.MapEvent.GetLeaderParty(0), MobileParty.MainParty.MapEvent.GetLeaderParty(1), 1, false),
				new BattleObserverMissionLogic(),
				new BattleAgentLogic(),
				new MountAgentLogic(),
				new AgentVictoryLogic(),
				new AgentHumanAILogic(),
				new MissionAgentPanicHandler(),
				new MissionHardBorderPlacer(),
				new MissionBoundaryPlacer(),
				new MissionBoundaryCrossingHandler(),
				new AgentMoraleInteractionLogic(),
				new HighlightsController(),
				new BattleHighlightsController(),
				new EquipmentControllerLeaveLogic(),
				new BattleSurgeonLogic()
			}, true, true);
		}

		[MissionMethod]
		public static Mission OpenCampMission(string scene)
		{
			return MissionState.OpenNew("Camp", SandBoxMissions.CreateSandBoxMissionInitializerRecord(scene, "", false, 3), (Mission mission) => new MissionBehavior[]
			{
				new MissionOptionsComponent(),
				new CampaignMissionComponent(),
				new BattleEndLogic(),
				new MissionCombatantsLogic(MobileParty.MainParty.MapEvent.InvolvedParties, PartyBase.MainParty, MobileParty.MainParty.MapEvent.GetLeaderParty(0), MobileParty.MainParty.MapEvent.GetLeaderParty(1), 0, false),
				new BasicLeaveMissionLogic(),
				new MissionHardBorderPlacer(),
				new MissionBoundaryPlacer(),
				new MissionBoundaryCrossingHandler(),
				new EquipmentControllerLeaveLogic()
			}, true, true);
		}

		[MissionMethod]
		public static Mission OpenSiegeMissionWithDeployment(string scene, float[] wallHitPointPercentages, bool hasAnySiegeTower, List<MissionSiegeWeapon> siegeWeaponsOfAttackers, List<MissionSiegeWeapon> siegeWeaponsOfDefenders, bool isPlayerAttacker, int sceneUpgradeLevel = 0, bool isSallyOut = false, bool isReliefForceAttack = false)
		{
			string text = Campaign.Current.Models.LocationModel.GetUpgradeLevelTag(sceneUpgradeLevel);
			text += " siege";
			bool isPlayerSergeant = MobileParty.MainParty.MapEvent.IsPlayerSergeant();
			bool isPlayerInArmy = MobileParty.MainParty.Army != null;
			List<string> heroesOnPlayerSideByPriority = HeroHelper.OrderHeroesOnPlayerSideByPriority();
			return MissionState.OpenNew("SiegeMissionWithDeployment", SandBoxMissions.CreateSandBoxMissionInitializerRecord(scene, text, false, 3), delegate(Mission mission)
			{
				List<MissionBehavior> list = new List<MissionBehavior>();
				list.Add(new BattleSpawnLogic(isSallyOut ? "sally_out_set" : (isReliefForceAttack ? "relief_force_attack_set" : "battle_set")));
				list.Add(new MissionOptionsComponent());
				list.Add(new CampaignMissionComponent());
				BattleEndLogic battleEndLogic = new BattleEndLogic();
				if (MobileParty.MainParty.MapEvent.PlayerSide == 1)
				{
					battleEndLogic.EnableEnemyDefenderPullBack(Campaign.Current.Models.SiegeLordsHallFightModel.DefenderTroopNumberForSuccessfulPullBack);
				}
				list.Add(battleEndLogic);
				list.Add(new BattleReinforcementsSpawnController());
				list.Add(new MissionCombatantsLogic(MobileParty.MainParty.MapEvent.InvolvedParties, PartyBase.MainParty, MobileParty.MainParty.MapEvent.GetLeaderParty(0), MobileParty.MainParty.MapEvent.GetLeaderParty(1), isSallyOut ? 3 : 2, isPlayerSergeant));
				list.Add(new SiegeMissionPreparationHandler(isSallyOut, isReliefForceAttack, wallHitPointPercentages, hasAnySiegeTower));
				list.Add(new CampaignSiegeStateHandler());
				Settlement currentTown = SandBoxMissions.GetCurrentTown();
				if (currentTown != null)
				{
					list.Add(new WorkshopMissionHandler(currentTown));
				}
				Mission.BattleSizeType battleSizeType = 1;
				if (isSallyOut)
				{
					battleSizeType = 2;
					FlattenedTroopRoster priorityTroopsForSallyOutAmbush = Campaign.Current.Models.SiegeEventModel.GetPriorityTroopsForSallyOutAmbush();
					list.Add(new SandBoxSallyOutMissionController());
					list.Add(SandBoxMissions.CreateCampaignMissionAgentSpawnLogic(battleSizeType, priorityTroopsForSallyOutAmbush, null));
				}
				else
				{
					if (isReliefForceAttack)
					{
						list.Add(new SandBoxSallyOutMissionController());
					}
					else
					{
						list.Add(new SandBoxSiegeMissionSpawnHandler());
					}
					list.Add(SandBoxMissions.CreateCampaignMissionAgentSpawnLogic(battleSizeType, null, null));
				}
				list.Add(new BattlePowerCalculationLogic());
				list.Add(new BattleObserverMissionLogic());
				list.Add(new BattleAgentLogic());
				list.Add(new BattleSurgeonLogic());
				list.Add(new MountAgentLogic());
				list.Add(new BannerBearerLogic());
				list.Add(new AgentHumanAILogic());
				list.Add(new AmmoSupplyLogic(new List<BattleSideEnum> { 0 }));
				list.Add(new AgentVictoryLogic());
				list.Add(new AssignPlayerRoleInTeamMissionController(!isPlayerSergeant, isPlayerSergeant, isPlayerInArmy, heroesOnPlayerSideByPriority, 8));
				List<MissionBehavior> list2 = list;
				Hero leaderHero = MapEvent.PlayerMapEvent.AttackerSide.LeaderParty.LeaderHero;
				TextObject textObject = ((leaderHero != null) ? leaderHero.Name : null);
				Hero leaderHero2 = MapEvent.PlayerMapEvent.DefenderSide.LeaderParty.LeaderHero;
				list2.Add(new SandboxGeneralsAndCaptainsAssignmentLogic(textObject, (leaderHero2 != null) ? leaderHero2.Name : null, null, null, false));
				list.Add(new MissionAgentPanicHandler());
				list.Add(new MissionBoundaryPlacer());
				list.Add(new MissionBoundaryCrossingHandler());
				list.Add(new AgentMoraleInteractionLogic());
				list.Add(new HighlightsController());
				list.Add(new BattleHighlightsController());
				list.Add(new EquipmentControllerLeaveLogic());
				if (isSallyOut)
				{
					list.Add(new MissionSiegeEnginesLogic(new List<MissionSiegeWeapon>(), siegeWeaponsOfAttackers));
				}
				else
				{
					list.Add(new MissionSiegeEnginesLogic(siegeWeaponsOfDefenders, siegeWeaponsOfAttackers));
				}
				list.Add(new SiegeDeploymentHandler(isPlayerAttacker));
				list.Add(new SiegeDeploymentMissionController(isPlayerAttacker));
				return list.ToArray();
			}, true, true);
		}

		[MissionMethod]
		public static Mission OpenSiegeMissionNoDeployment(string scene, bool isSallyOut = false, bool isReliefForceAttack = false)
		{
			string text = Campaign.Current.Models.LocationModel.GetUpgradeLevelTag(3);
			text += " siege";
			bool isPlayerSergeant = MobileParty.MainParty.MapEvent.IsPlayerSergeant();
			bool isPlayerInArmy = MobileParty.MainParty.Army != null;
			List<string> heroesOnPlayerSideByPriority = HeroHelper.OrderHeroesOnPlayerSideByPriority();
			return MissionState.OpenNew("SiegeMissionNoDeployment", SandBoxMissions.CreateSandBoxMissionInitializerRecord(scene, text, false, 3), delegate(Mission mission)
			{
				List<MissionBehavior> list = new List<MissionBehavior>();
				list.Add(new MissionOptionsComponent());
				list.Add(new BattleSpawnLogic(isSallyOut ? "sally_out_set" : (isReliefForceAttack ? "relief_force_attack_set" : "battle_set")));
				list.Add(new CampaignMissionComponent());
				BattleEndLogic battleEndLogic = new BattleEndLogic();
				if (!isSallyOut && !isReliefForceAttack && MobileParty.MainParty.MapEvent.PlayerSide == 1)
				{
					battleEndLogic.EnableEnemyDefenderPullBack(Campaign.Current.Models.SiegeLordsHallFightModel.DefenderTroopNumberForSuccessfulPullBack);
				}
				list.Add(battleEndLogic);
				list.Add(new BattleReinforcementsSpawnController());
				list.Add(new MissionCombatantsLogic(MobileParty.MainParty.MapEvent.InvolvedParties, PartyBase.MainParty, MobileParty.MainParty.MapEvent.GetLeaderParty(0), MobileParty.MainParty.MapEvent.GetLeaderParty(1), 1, isPlayerSergeant));
				list.Add(new CampaignSiegeStateHandler());
				Mission.BattleSizeType battleSizeType = (isSallyOut ? 2 : 1);
				list.Add(SandBoxMissions.CreateCampaignMissionAgentSpawnLogic(battleSizeType, null, null));
				list.Add(new BattlePowerCalculationLogic());
				list.Add(new SandBoxBattleMissionSpawnHandler());
				Settlement currentTown = SandBoxMissions.GetCurrentTown();
				if (currentTown != null)
				{
					list.Add(new WorkshopMissionHandler(currentTown));
				}
				list.Add(new BattleObserverMissionLogic());
				list.Add(new BattleAgentLogic());
				list.Add(new BattleSurgeonLogic());
				list.Add(new MountAgentLogic());
				list.Add(new AgentVictoryLogic());
				list.Add(new AmmoSupplyLogic(new List<BattleSideEnum> { 0 }));
				list.Add(new MissionAgentPanicHandler());
				list.Add(new MissionHardBorderPlacer());
				list.Add(new MissionBoundaryPlacer());
				list.Add(new EquipmentControllerLeaveLogic());
				list.Add(new MissionBoundaryCrossingHandler());
				list.Add(new AgentHumanAILogic());
				list.Add(new AgentMoraleInteractionLogic());
				list.Add(new HighlightsController());
				list.Add(new BattleHighlightsController());
				list.Add(new AssignPlayerRoleInTeamMissionController(!isPlayerSergeant, isPlayerSergeant, isPlayerInArmy, heroesOnPlayerSideByPriority, 8));
				List<MissionBehavior> list2 = list;
				Hero leaderHero = MapEvent.PlayerMapEvent.AttackerSide.LeaderParty.LeaderHero;
				TextObject textObject = ((leaderHero != null) ? leaderHero.Name : null);
				Hero leaderHero2 = MapEvent.PlayerMapEvent.DefenderSide.LeaderParty.LeaderHero;
				list2.Add(new SandboxGeneralsAndCaptainsAssignmentLogic(textObject, (leaderHero2 != null) ? leaderHero2.Name : null, null, null, false));
				return list.ToArray();
			}, true, true);
		}

		[MissionMethod]
		public static Mission OpenSiegeLordsHallFightMission(string scene, FlattenedTroopRoster attackerPriorityList)
		{
			int remainingDefenderArcherCount = Campaign.Current.Models.SiegeLordsHallFightModel.MaxDefenderArcherCount;
			FlattenedTroopRoster defenderPriorityList = Campaign.Current.Models.SiegeLordsHallFightModel.GetPriorityListForLordsHallFightMission(MapEvent.PlayerMapEvent, 0, Campaign.Current.Models.SiegeLordsHallFightModel.MaxDefenderSideTroopCount);
			int attackerSideTroopCountMax = MathF.Min(Campaign.Current.Models.SiegeLordsHallFightModel.MaxAttackerSideTroopCount, attackerPriorityList.Troops.Count<CharacterObject>());
			int defenderSideTroopCountMax = MathF.Min(Campaign.Current.Models.SiegeLordsHallFightModel.MaxDefenderSideTroopCount, defenderPriorityList.Troops.Count<CharacterObject>());
			MissionInitializerRecord missionInitializerRecord = SandBoxMissions.CreateSandBoxMissionInitializerRecord(scene, "siege", false, 3);
			Func<UniqueTroopDescriptor, MapEventParty, bool> <>9__1;
			return MissionState.OpenNew("SiegeLordsHallFightMission", missionInitializerRecord, delegate(Mission mission)
			{
				IMissionTroopSupplier[] array = new IMissionTroopSupplier[2];
				IMissionTroopSupplier[] array2 = array;
				int num = 0;
				MapEvent playerMapEvent = MapEvent.PlayerMapEvent;
				BattleSideEnum battleSideEnum = 0;
				FlattenedTroopRoster defenderPriorityList2 = defenderPriorityList;
				Func<UniqueTroopDescriptor, MapEventParty, bool> func;
				if ((func = <>9__1) == null)
				{
					func = (<>9__1 = delegate(UniqueTroopDescriptor uniqueTroopDescriptor, MapEventParty mapEventParty)
					{
						bool flag = true;
						if (mapEventParty.GetTroop(uniqueTroopDescriptor).IsRanged)
						{
							if (remainingDefenderArcherCount > 0)
							{
								int remainingDefenderArcherCount2 = remainingDefenderArcherCount;
								remainingDefenderArcherCount = remainingDefenderArcherCount2 - 1;
							}
							else
							{
								flag = false;
							}
						}
						return flag;
					});
				}
				array2[num] = new PartyGroupTroopSupplier(playerMapEvent, battleSideEnum, defenderPriorityList2, func);
				array[1] = new PartyGroupTroopSupplier(MapEvent.PlayerMapEvent, 1, attackerPriorityList, null);
				return new MissionBehavior[]
				{
					new MissionOptionsComponent(),
					new CampaignMissionComponent(),
					new BattleEndLogic(),
					new MissionCombatantsLogic(MobileParty.MainParty.MapEvent.InvolvedParties, PartyBase.MainParty, MobileParty.MainParty.MapEvent.GetLeaderParty(0), MobileParty.MainParty.MapEvent.GetLeaderParty(1), 0, false),
					new CampaignSiegeStateHandler(),
					new AgentHumanAILogic(),
					new LordsHallFightMissionController(array, Campaign.Current.Models.SiegeLordsHallFightModel.AreaLostRatio, Campaign.Current.Models.SiegeLordsHallFightModel.AttackerDefenderTroopCountRatio, attackerSideTroopCountMax, defenderSideTroopCountMax, PartyBase.MainParty.Side),
					new BattleObserverMissionLogic(),
					new BattleAgentLogic(),
					new AgentVictoryLogic(),
					new AmmoSupplyLogic(new List<BattleSideEnum> { 0 }),
					new MissionHardBorderPlacer(),
					new MissionBoundaryPlacer(),
					new MissionBoundaryCrossingHandler(),
					new EquipmentControllerLeaveLogic(),
					new BattleMissionAgentInteractionLogic(),
					new HighlightsController(),
					new BattleHighlightsController(),
					new BattleSurgeonLogic()
				};
			}, true, true);
		}

		[MissionMethod]
		public static Mission OpenAmbushMissionForTutorial(string scene, bool isPlayerAttacker)
		{
			return MissionState.OpenNew("AmbushMissionForTutorial", SandBoxMissions.CreateSandBoxMissionInitializerRecord(scene, "", false, 3), delegate(Mission missionController)
			{
				List<MissionBehavior> list = new List<MissionBehavior>();
				list.Add(new MissionOptionsComponent());
				list.Add(new AmbushMissionController(isPlayerAttacker));
				AmbushBattleDeploymentHandler ambushBattleDeploymentHandler = new AmbushBattleDeploymentHandler(isPlayerAttacker);
				list.Add(ambushBattleDeploymentHandler);
				list.Add(new BasicLeaveMissionLogic());
				list.Add(new MissionAgentPanicHandler());
				list.Add(new MissionBoundaryPlacer());
				list.Add(new AgentMoraleInteractionLogic());
				list.Add(new EquipmentControllerLeaveLogic());
				return list.ToArray();
			}, true, true);
		}

		[MissionMethod]
		public static Mission OpenVillageBattleMission(string scene)
		{
			bool isPlayerSergeant = MobileParty.MainParty.MapEvent.IsPlayerSergeant();
			bool isPlayerInArmy = MobileParty.MainParty.Army != null;
			return MissionState.OpenNew("VillageBattle", SandBoxMissions.CreateSandBoxMissionInitializerRecord(scene, "", false, 3), delegate(Mission mission)
			{
				MissionBehavior[] array = new MissionBehavior[17];
				array[0] = new MissionOptionsComponent();
				array[1] = new CampaignMissionComponent();
				array[2] = new BattleEndLogic();
				array[3] = new BattleReinforcementsSpawnController();
				array[4] = new MissionCombatantsLogic(MobileParty.MainParty.MapEvent.InvolvedParties, PartyBase.MainParty, MobileParty.MainParty.MapEvent.GetLeaderParty(0), MobileParty.MainParty.MapEvent.GetLeaderParty(1), 1, isPlayerSergeant);
				array[5] = new AgentHumanAILogic();
				array[6] = new MissionAgentPanicHandler();
				array[7] = new MissionHardBorderPlacer();
				array[8] = new MissionBoundaryPlacer();
				array[9] = new MissionBoundaryCrossingHandler();
				array[10] = new AgentMoraleInteractionLogic();
				array[11] = new HighlightsController();
				array[12] = new BattleHighlightsController();
				array[13] = new EquipmentControllerLeaveLogic();
				array[14] = new AssignPlayerRoleInTeamMissionController(!isPlayerSergeant, isPlayerSergeant, isPlayerInArmy, null, 8);
				int num = 15;
				Hero leaderHero = MapEvent.PlayerMapEvent.AttackerSide.LeaderParty.LeaderHero;
				TextObject textObject = ((leaderHero != null) ? leaderHero.Name : null);
				Hero leaderHero2 = MapEvent.PlayerMapEvent.DefenderSide.LeaderParty.LeaderHero;
				array[num] = new SandboxGeneralsAndCaptainsAssignmentLogic(textObject, (leaderHero2 != null) ? leaderHero2.Name : null, null, null, true);
				array[16] = new BattleSurgeonLogic();
				return array;
			}, true, true);
		}

		[MissionMethod]
		public static Mission OpenConversationMission(ConversationCharacterData playerCharacterData, ConversationCharacterData conversationPartnerData, string specialScene = "", string sceneLevels = "")
		{
			string text = (Extensions.IsEmpty<char>(specialScene) ? PlayerEncounter.GetConversationSceneForMapPosition(PartyBase.MainParty.Position2D) : specialScene);
			return MissionState.OpenNew("Conversation", SandBoxMissions.CreateSandBoxMissionInitializerRecord(text, sceneLevels, true, 3), (Mission mission) => new MissionBehavior[]
			{
				new CampaignMissionComponent(),
				new MissionConversationLogic(),
				new MissionOptionsComponent(),
				new ConversationMissionLogic(playerCharacterData, conversationPartnerData),
				new MissionDebugHandler(),
				new EquipmentControllerLeaveLogic()
			}, true, false);
		}

		[MissionMethod]
		public static Mission OpenMeetingMission(string scene, CharacterObject character)
		{
			Debug.FailedAssert("This mission was broken", "C:\\Develop\\MB3\\Source\\Bannerlord\\SandBox\\Missions\\SandBoxMissions.cs", "OpenMeetingMission", 1281);
			return MissionState.OpenNew("Conversation", SandBoxMissions.CreateSandBoxMissionInitializerRecord(scene, "", false, 3), (Mission mission) => new MissionBehavior[]
			{
				new CampaignMissionComponent(),
				new MissionSettlementPrepareLogic(),
				new MissionOptionsComponent(),
				new MissionConversationLogic(),
				new EquipmentControllerLeaveLogic()
			}, true, false);
		}

		private static Settlement GetCurrentTown()
		{
			if (Settlement.CurrentSettlement != null && Settlement.CurrentSettlement.IsTown)
			{
				return Settlement.CurrentSettlement;
			}
			if (MapEvent.PlayerMapEvent != null && MapEvent.PlayerMapEvent.MapEventSettlement != null && MapEvent.PlayerMapEvent.MapEventSettlement.IsTown)
			{
				return MapEvent.PlayerMapEvent.MapEventSettlement;
			}
			return null;
		}

		private static MissionAgentSpawnLogic CreateCampaignMissionAgentSpawnLogic(Mission.BattleSizeType battleSizeType, FlattenedTroopRoster priorTroopsForDefenders = null, FlattenedTroopRoster priorTroopsForAttackers = null)
		{
			return new MissionAgentSpawnLogic(new IMissionTroopSupplier[]
			{
				new PartyGroupTroopSupplier(MapEvent.PlayerMapEvent, 0, priorTroopsForDefenders, null),
				new PartyGroupTroopSupplier(MapEvent.PlayerMapEvent, 1, priorTroopsForAttackers, null)
			}, PartyBase.MainParty.Side, battleSizeType);
		}
	}
}
