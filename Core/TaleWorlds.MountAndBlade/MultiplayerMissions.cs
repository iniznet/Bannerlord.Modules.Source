using System;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade.Source.Missions;

namespace TaleWorlds.MountAndBlade
{
	[MissionManager]
	public static class MultiplayerMissions
	{
		[MissionMethod]
		public static void OpenFreeForAllMission(string scene)
		{
			MissionState.OpenNew("MultiplayerFreeForAll", new MissionInitializerRecord(scene), delegate(Mission missionController)
			{
				if (GameNetwork.IsServer)
				{
					return new MissionBehavior[]
					{
						MissionLobbyComponent.CreateBehavior(),
						new MissionMultiplayerFFA(),
						new MissionMultiplayerFFAClient(),
						new MultiplayerTimerComponent(),
						new MultiplayerMissionAgentVisualSpawnComponent(),
						new ConsoleMatchStartEndHandler(),
						new SpawnComponent(new FFASpawnFrameBehavior(), new WarmupSpawningBehavior()),
						new MissionLobbyEquipmentNetworkComponent(),
						new MultiplayerTeamSelectComponent(),
						new MissionHardBorderPlacer(),
						new MissionBoundaryPlacer(),
						new MissionBoundaryCrossingHandler(),
						new MultiplayerPollComponent(),
						new MultiplayerAdminComponent(),
						new MultiplayerGameNotificationsComponent(),
						new MissionOptionsComponent(),
						new MissionScoreboardComponent(new FFAScoreboardData()),
						new MissionAgentPanicHandler(),
						new AgentHumanAILogic(),
						new EquipmentControllerLeaveLogic(),
						new MultiplayerPreloadHelper()
					};
				}
				return new MissionBehavior[]
				{
					MissionLobbyComponent.CreateBehavior(),
					new MissionMultiplayerFFAClient(),
					new MultiplayerAchievementComponent(),
					new MultiplayerTimerComponent(),
					new MultiplayerMissionAgentVisualSpawnComponent(),
					new ConsoleMatchStartEndHandler(),
					new MissionLobbyEquipmentNetworkComponent(),
					new MultiplayerTeamSelectComponent(),
					new MissionHardBorderPlacer(),
					new MissionBoundaryPlacer(),
					new MissionBoundaryCrossingHandler(),
					new MultiplayerPollComponent(),
					new MultiplayerGameNotificationsComponent(),
					new MissionOptionsComponent(),
					new MissionScoreboardComponent(new FFAScoreboardData()),
					new MissionMatchHistoryComponent(),
					new EquipmentControllerLeaveLogic(),
					new MissionRecentPlayersComponent(),
					new MultiplayerPreloadHelper()
				};
			}, true, true);
		}

		[MissionMethod]
		public static void OpenTeamDeathmatchMission(string scene)
		{
			MissionState.OpenNew("MultiplayerTeamDeathmatch", new MissionInitializerRecord(scene), delegate(Mission missionController)
			{
				if (GameNetwork.IsServer)
				{
					return new MissionBehavior[]
					{
						MissionLobbyComponent.CreateBehavior(),
						new MissionMultiplayerTeamDeathmatch(),
						new MissionMultiplayerTeamDeathmatchClient(),
						new MultiplayerTimerComponent(),
						new MultiplayerMissionAgentVisualSpawnComponent(),
						new ConsoleMatchStartEndHandler(),
						new SpawnComponent(new TeamDeathmatchSpawnFrameBehavior(), new TeamDeathmatchSpawningBehavior()),
						new MissionLobbyEquipmentNetworkComponent(),
						new MultiplayerTeamSelectComponent(),
						new MissionHardBorderPlacer(),
						new MissionBoundaryPlacer(),
						new MissionBoundaryCrossingHandler(),
						new MultiplayerPollComponent(),
						new MultiplayerAdminComponent(),
						new MultiplayerGameNotificationsComponent(),
						new MissionOptionsComponent(),
						new MissionScoreboardComponent(new TDMScoreboardData()),
						new MissionAgentPanicHandler(),
						new AgentHumanAILogic(),
						new EquipmentControllerLeaveLogic(),
						new MultiplayerPreloadHelper()
					};
				}
				return new MissionBehavior[]
				{
					MissionLobbyComponent.CreateBehavior(),
					new MissionMultiplayerTeamDeathmatchClient(),
					new MultiplayerAchievementComponent(),
					new MultiplayerTimerComponent(),
					new MultiplayerMissionAgentVisualSpawnComponent(),
					new ConsoleMatchStartEndHandler(),
					new MissionLobbyEquipmentNetworkComponent(),
					new MultiplayerTeamSelectComponent(),
					new MissionHardBorderPlacer(),
					new MissionBoundaryPlacer(),
					new MissionBoundaryCrossingHandler(),
					new MultiplayerPollComponent(),
					new MultiplayerGameNotificationsComponent(),
					new MissionOptionsComponent(),
					new MissionScoreboardComponent(new TDMScoreboardData()),
					new MissionMatchHistoryComponent(),
					new EquipmentControllerLeaveLogic(),
					new MissionRecentPlayersComponent(),
					new MultiplayerPreloadHelper()
				};
			}, true, true);
		}

		[MissionMethod]
		public static void OpenDuelMission(string scene)
		{
			MissionState.OpenNew("MultiplayerDuel", new MissionInitializerRecord(scene), delegate(Mission missionController)
			{
				if (GameNetwork.IsServer)
				{
					return new MissionBehavior[]
					{
						MissionLobbyComponent.CreateBehavior(),
						new MissionMultiplayerDuel(),
						new MissionMultiplayerGameModeDuelClient(),
						new MultiplayerTimerComponent(),
						new MultiplayerMissionAgentVisualSpawnComponent(),
						new ConsoleMatchStartEndHandler(),
						new SpawnComponent(new DuelSpawnFrameBehavior(), new DuelSpawningBehavior()),
						new MissionLobbyEquipmentNetworkComponent(),
						new MissionHardBorderPlacer(),
						new MissionBoundaryPlacer(),
						new MissionBoundaryCrossingHandler(),
						new MultiplayerPollComponent(),
						new MultiplayerAdminComponent(),
						new MultiplayerGameNotificationsComponent(),
						new MissionOptionsComponent(),
						new MissionScoreboardComponent(new DuelScoreboardData()),
						new MissionAgentPanicHandler(),
						new AgentHumanAILogic(),
						new EquipmentControllerLeaveLogic(),
						new MultiplayerPreloadHelper()
					};
				}
				return new MissionBehavior[]
				{
					MissionLobbyComponent.CreateBehavior(),
					new MissionMultiplayerGameModeDuelClient(),
					new MultiplayerAchievementComponent(),
					new MultiplayerTimerComponent(),
					new MultiplayerMissionAgentVisualSpawnComponent(),
					new ConsoleMatchStartEndHandler(),
					new MissionLobbyEquipmentNetworkComponent(),
					new MissionHardBorderPlacer(),
					new MissionBoundaryPlacer(),
					new MissionBoundaryCrossingHandler(),
					new MultiplayerPollComponent(),
					new MultiplayerGameNotificationsComponent(),
					new MissionOptionsComponent(),
					new MissionScoreboardComponent(new DuelScoreboardData()),
					new MissionMatchHistoryComponent(),
					new EquipmentControllerLeaveLogic(),
					new MissionRecentPlayersComponent(),
					new MultiplayerPreloadHelper()
				};
			}, true, true);
		}

		[MissionMethod]
		public static void OpenSiegeMission(string scene)
		{
			MissionState.OpenNew("MultiplayerSiege", new MissionInitializerRecord(scene)
			{
				SceneUpgradeLevel = 3,
				SceneLevels = ""
			}, delegate(Mission missionController)
			{
				if (GameNetwork.IsServer)
				{
					return new MissionBehavior[]
					{
						MissionLobbyComponent.CreateBehavior(),
						new MissionMultiplayerSiege(),
						new MultiplayerWarmupComponent(),
						new MissionMultiplayerSiegeClient(),
						new MultiplayerTimerComponent(),
						new MultiplayerMissionAgentVisualSpawnComponent(),
						new ConsoleMatchStartEndHandler(),
						new SpawnComponent(new SiegeSpawnFrameBehavior(), new SiegeSpawningBehavior()),
						new MissionLobbyEquipmentNetworkComponent(),
						new MultiplayerTeamSelectComponent(),
						new MissionHardBorderPlacer(),
						new MissionBoundaryPlacer(),
						new MissionBoundaryCrossingHandler(),
						new MultiplayerPollComponent(),
						new MultiplayerAdminComponent(),
						new MultiplayerGameNotificationsComponent(),
						new MissionOptionsComponent(),
						new MissionScoreboardComponent(new SiegeScoreboardData()),
						new MissionAgentPanicHandler(),
						new AgentHumanAILogic(),
						new EquipmentControllerLeaveLogic(),
						new MultiplayerPreloadHelper()
					};
				}
				return new MissionBehavior[]
				{
					MissionLobbyComponent.CreateBehavior(),
					new MultiplayerWarmupComponent(),
					new MissionMultiplayerSiegeClient(),
					new MultiplayerAchievementComponent(),
					new MultiplayerTimerComponent(),
					new MultiplayerMissionAgentVisualSpawnComponent(),
					new ConsoleMatchStartEndHandler(),
					new MissionLobbyEquipmentNetworkComponent(),
					new MultiplayerTeamSelectComponent(),
					new MissionHardBorderPlacer(),
					new MissionBoundaryPlacer(),
					new MissionBoundaryCrossingHandler(),
					new MultiplayerPollComponent(),
					new MultiplayerGameNotificationsComponent(),
					new MissionOptionsComponent(),
					new MissionScoreboardComponent(new SiegeScoreboardData()),
					new MissionMatchHistoryComponent(),
					new EquipmentControllerLeaveLogic(),
					new MissionRecentPlayersComponent(),
					new MultiplayerPreloadHelper()
				};
			}, true, true);
		}

		[MissionMethod]
		public static void OpenBattleMission(string scene)
		{
			MissionState.OpenNew("MultiplayerBattle", new MissionInitializerRecord(scene), delegate(Mission missionController)
			{
				if (GameNetwork.IsServer)
				{
					return new MissionBehavior[]
					{
						MissionLobbyComponent.CreateBehavior(),
						new MultiplayerRoundController(),
						new MissionMultiplayerFlagDomination(MissionLobbyComponent.MultiplayerGameType.Battle),
						new MultiplayerWarmupComponent(),
						new MissionMultiplayerGameModeFlagDominationClient(),
						new MultiplayerTimerComponent(),
						new MultiplayerMissionAgentVisualSpawnComponent(),
						new ConsoleMatchStartEndHandler(),
						new SpawnComponent(new FlagDominationSpawnFrameBehavior(), new FlagDominationSpawningBehavior()),
						new MissionLobbyEquipmentNetworkComponent(),
						new MultiplayerTeamSelectComponent(),
						new MissionHardBorderPlacer(),
						new MissionBoundaryPlacer(),
						new AgentVictoryLogic(),
						new AgentHumanAILogic(),
						new MissionBoundaryCrossingHandler(),
						new MultiplayerPollComponent(),
						new MultiplayerAdminComponent(),
						new MultiplayerGameNotificationsComponent(),
						new MissionOptionsComponent(),
						new MissionScoreboardComponent(new BattleScoreboardData()),
						new EquipmentControllerLeaveLogic(),
						new MultiplayerPreloadHelper()
					};
				}
				return new MissionBehavior[]
				{
					MissionLobbyComponent.CreateBehavior(),
					new MultiplayerRoundComponent(),
					new MultiplayerWarmupComponent(),
					new MissionMultiplayerGameModeFlagDominationClient(),
					new MultiplayerTimerComponent(),
					new MultiplayerMissionAgentVisualSpawnComponent(),
					new ConsoleMatchStartEndHandler(),
					new MissionLobbyEquipmentNetworkComponent(),
					new MultiplayerTeamSelectComponent(),
					new MissionHardBorderPlacer(),
					new MissionBoundaryPlacer(),
					new AgentVictoryLogic(),
					new MissionBoundaryCrossingHandler(),
					new MultiplayerPollComponent(),
					new MultiplayerGameNotificationsComponent(),
					new MissionOptionsComponent(),
					new MissionScoreboardComponent(new BattleScoreboardData()),
					new MissionMatchHistoryComponent(),
					new EquipmentControllerLeaveLogic(),
					new MultiplayerPreloadHelper()
				};
			}, true, true);
		}

		[MissionMethod]
		public static void OpenCaptainMission(string scene)
		{
			MissionState.OpenNew("MultiplayerCaptain", new MissionInitializerRecord(scene), delegate(Mission missionController)
			{
				if (GameNetwork.IsServer)
				{
					return new MissionBehavior[]
					{
						MissionLobbyComponent.CreateBehavior(),
						new MissionMultiplayerFlagDomination(MissionLobbyComponent.MultiplayerGameType.Captain),
						new MultiplayerRoundController(),
						new MultiplayerWarmupComponent(),
						new MissionMultiplayerGameModeFlagDominationClient(),
						new MultiplayerTimerComponent(),
						new MultiplayerMissionAgentVisualSpawnComponent(),
						new ConsoleMatchStartEndHandler(),
						new SpawnComponent(new FlagDominationSpawnFrameBehavior(), new FlagDominationSpawningBehavior()),
						new MissionLobbyEquipmentNetworkComponent(),
						new MultiplayerTeamSelectComponent(),
						new MissionHardBorderPlacer(),
						new MissionBoundaryPlacer(),
						new AgentVictoryLogic(),
						new AgentHumanAILogic(),
						new MissionAgentPanicHandler(),
						new MissionBoundaryCrossingHandler(),
						new MultiplayerPollComponent(),
						new MultiplayerAdminComponent(),
						new MultiplayerGameNotificationsComponent(),
						new MissionOptionsComponent(),
						new MissionScoreboardComponent(new CaptainScoreboardData()),
						new EquipmentControllerLeaveLogic(),
						new MultiplayerPreloadHelper()
					};
				}
				return new MissionBehavior[]
				{
					MissionLobbyComponent.CreateBehavior(),
					new MultiplayerAchievementComponent(),
					new MultiplayerWarmupComponent(),
					new MissionMultiplayerGameModeFlagDominationClient(),
					new MultiplayerRoundComponent(),
					new MultiplayerTimerComponent(),
					new MultiplayerMissionAgentVisualSpawnComponent(),
					new ConsoleMatchStartEndHandler(),
					new MissionLobbyEquipmentNetworkComponent(),
					new MultiplayerTeamSelectComponent(),
					new MissionHardBorderPlacer(),
					new MissionBoundaryPlacer(),
					new AgentVictoryLogic(),
					new MissionBoundaryCrossingHandler(),
					new MultiplayerPollComponent(),
					new MultiplayerGameNotificationsComponent(),
					new MissionOptionsComponent(),
					new MissionScoreboardComponent(new CaptainScoreboardData()),
					new MissionMatchHistoryComponent(),
					new EquipmentControllerLeaveLogic(),
					new MissionRecentPlayersComponent(),
					new MultiplayerPreloadHelper()
				};
			}, true, true);
		}

		[MissionMethod]
		public static void OpenSkirmishMission(string scene)
		{
			MissionState.OpenNew("MultiplayerSkirmish", new MissionInitializerRecord(scene), delegate(Mission missionController)
			{
				if (GameNetwork.IsServer)
				{
					return new MissionBehavior[]
					{
						MissionLobbyComponent.CreateBehavior(),
						new MissionMultiplayerFlagDomination(MissionLobbyComponent.MultiplayerGameType.Skirmish),
						new MultiplayerRoundController(),
						new MultiplayerWarmupComponent(),
						new MissionMultiplayerGameModeFlagDominationClient(),
						new MultiplayerTimerComponent(),
						new MultiplayerMissionAgentVisualSpawnComponent(),
						new ConsoleMatchStartEndHandler(),
						new SpawnComponent(new FlagDominationSpawnFrameBehavior(), new FlagDominationSpawningBehavior()),
						new MissionLobbyEquipmentNetworkComponent(),
						new MultiplayerTeamSelectComponent(),
						new MissionHardBorderPlacer(),
						new MissionBoundaryPlacer(),
						new AgentVictoryLogic(),
						new MissionAgentPanicHandler(),
						new AgentHumanAILogic(),
						new MissionBoundaryCrossingHandler(),
						new MultiplayerPollComponent(),
						new MultiplayerAdminComponent(),
						new MultiplayerGameNotificationsComponent(),
						new MissionOptionsComponent(),
						new MissionScoreboardComponent(new SkirmishScoreboardData()),
						new EquipmentControllerLeaveLogic(),
						new VoiceChatHandler(),
						new MultiplayerPreloadHelper()
					};
				}
				return new MissionBehavior[]
				{
					MissionLobbyComponent.CreateBehavior(),
					new MultiplayerAchievementComponent(),
					new MultiplayerWarmupComponent(),
					new MissionMultiplayerGameModeFlagDominationClient(),
					new MultiplayerRoundComponent(),
					new MultiplayerTimerComponent(),
					new MultiplayerMissionAgentVisualSpawnComponent(),
					new ConsoleMatchStartEndHandler(),
					new MissionLobbyEquipmentNetworkComponent(),
					new MultiplayerTeamSelectComponent(),
					new MissionHardBorderPlacer(),
					new MissionBoundaryPlacer(),
					new AgentVictoryLogic(),
					new MissionBoundaryCrossingHandler(),
					new MultiplayerPollComponent(),
					new MultiplayerGameNotificationsComponent(),
					new MissionOptionsComponent(),
					new MissionScoreboardComponent(new SkirmishScoreboardData()),
					new MissionMatchHistoryComponent(),
					new EquipmentControllerLeaveLogic(),
					new MissionRecentPlayersComponent(),
					new VoiceChatHandler(),
					new MultiplayerPreloadHelper()
				};
			}, true, true);
		}
	}
}
