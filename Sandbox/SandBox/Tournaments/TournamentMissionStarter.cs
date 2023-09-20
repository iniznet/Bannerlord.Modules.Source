using System;
using System.Collections.Generic;
using SandBox.Missions.MissionLogics;
using SandBox.Missions.MissionLogics.Arena;
using SandBox.Tournaments.MissionLogics;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.TournamentGames;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Source.Missions;

namespace SandBox.Tournaments
{
	// Token: 0x02000019 RID: 25
	[MissionManager]
	public static class TournamentMissionStarter
	{
		// Token: 0x060000ED RID: 237 RVA: 0x00007538 File Offset: 0x00005738
		[MissionMethod]
		public static Mission OpenTournamentArcheryMission(string scene, TournamentGame tournamentGame, Settlement settlement, CultureObject culture, bool isPlayerParticipating)
		{
			return MissionState.OpenNew("TournamentArchery", SandBoxMissions.CreateSandBoxMissionInitializerRecord(scene, "", false, 3), delegate(Mission missionController)
			{
				TournamentArcheryMissionController tournamentArcheryMissionController = new TournamentArcheryMissionController(culture);
				return new MissionBehavior[]
				{
					new CampaignMissionComponent(),
					new EquipmentControllerLeaveLogic(),
					tournamentArcheryMissionController,
					new TournamentBehavior(tournamentGame, settlement, tournamentArcheryMissionController, isPlayerParticipating),
					new AgentVictoryLogic(),
					new MissionAgentPanicHandler(),
					new AgentHumanAILogic(),
					new ArenaAgentStateDeciderLogic(),
					new BasicLeaveMissionLogic(true),
					new MissionHardBorderPlacer(),
					new MissionBoundaryPlacer(),
					new MissionOptionsComponent()
				};
			}, true, true);
		}

		// Token: 0x060000EE RID: 238 RVA: 0x00007590 File Offset: 0x00005790
		[MissionMethod]
		public static Mission OpenTournamentFightMission(string scene, TournamentGame tournamentGame, Settlement settlement, CultureObject culture, bool isPlayerParticipating)
		{
			return MissionState.OpenNew("TournamentFight", SandBoxMissions.CreateSandBoxMissionInitializerRecord(scene, "", false, 3), delegate(Mission missionController)
			{
				TournamentFightMissionController tournamentFightMissionController = new TournamentFightMissionController(culture);
				return new MissionBehavior[]
				{
					new CampaignMissionComponent(),
					new EquipmentControllerLeaveLogic(),
					tournamentFightMissionController,
					new TournamentBehavior(tournamentGame, settlement, tournamentFightMissionController, isPlayerParticipating),
					new AgentVictoryLogic(),
					new MissionAgentPanicHandler(),
					new AgentHumanAILogic(),
					new ArenaAgentStateDeciderLogic(),
					new MissionHardBorderPlacer(),
					new MissionBoundaryPlacer(),
					new MissionOptionsComponent(),
					new HighlightsController(),
					new SandboxHighlightsController()
				};
			}, true, true);
		}

		// Token: 0x060000EF RID: 239 RVA: 0x000075E8 File Offset: 0x000057E8
		[MissionMethod]
		public static Mission OpenTournamentHorseRaceMission(string scene, TournamentGame tournamentGame, Settlement settlement, CultureObject culture, bool isPlayerParticipating)
		{
			return MissionState.OpenNew("TournamentHorseRace", SandBoxMissions.CreateSandBoxMissionInitializerRecord(scene, "", false, 3), delegate(Mission missionController)
			{
				TownHorseRaceMissionController townHorseRaceMissionController = new TownHorseRaceMissionController(culture);
				return new MissionBehavior[]
				{
					new CampaignMissionComponent(),
					new EquipmentControllerLeaveLogic(),
					townHorseRaceMissionController,
					new TournamentBehavior(tournamentGame, settlement, townHorseRaceMissionController, isPlayerParticipating),
					new AgentVictoryLogic(),
					new MissionAgentPanicHandler(),
					new AgentHumanAILogic(),
					new ArenaAgentStateDeciderLogic(),
					new MissionDebugHandler(),
					new MissionHardBorderPlacer(),
					new MissionBoundaryPlacer(),
					new MissionOptionsComponent()
				};
			}, true, true);
		}

		// Token: 0x060000F0 RID: 240 RVA: 0x00007640 File Offset: 0x00005840
		[MissionMethod]
		public static Mission OpenTournamentJoustingMission(string scene, TournamentGame tournamentGame, Settlement settlement, CultureObject culture, bool isPlayerParticipating)
		{
			return MissionState.OpenNew("TournamentJousting", SandBoxMissions.CreateSandBoxMissionInitializerRecord(scene, "", false, 3), delegate(Mission missionController)
			{
				TournamentJoustingMissionController tournamentJoustingMissionController = new TournamentJoustingMissionController(culture);
				return new MissionBehavior[]
				{
					new CampaignMissionComponent(),
					new EquipmentControllerLeaveLogic(),
					tournamentJoustingMissionController,
					new TournamentBehavior(tournamentGame, settlement, tournamentJoustingMissionController, isPlayerParticipating),
					new AgentVictoryLogic(),
					new MissionAgentPanicHandler(),
					new AgentHumanAILogic(),
					new ArenaAgentStateDeciderLogic(),
					new MissionHardBorderPlacer(),
					new MissionBoundaryPlacer(),
					new MissionBoundaryCrossingHandler(),
					new MissionOptionsComponent()
				};
			}, true, true);
		}

		// Token: 0x060000F1 RID: 241 RVA: 0x00007695 File Offset: 0x00005895
		[MissionMethod]
		public static Mission OpenBattleChallengeMission(string scene, IList<Hero> priorityCharsAttacker, IList<Hero> priorityCharsDefender)
		{
			return null;
		}
	}
}
