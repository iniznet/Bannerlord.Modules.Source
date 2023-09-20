using System;
using SandBox;
using SandBox.Conversation.MissionLogics;
using SandBox.Missions.MissionLogics;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Settlements.Locations;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Source.Missions;
using TaleWorlds.MountAndBlade.Source.Missions.Handlers;

namespace StoryMode.Missions
{
	[MissionManager]
	public static class StoryModeMissions
	{
		[MissionMethod]
		public static Mission OpenTrainingFieldMission(string scene, Location location, CharacterObject talkToChar = null, string sceneLevels = null)
		{
			return MissionState.OpenNew("TrainingField", SandBoxMissions.CreateSandBoxTrainingMissionInitializerRecord(scene, sceneLevels, false), (Mission mission) => new MissionBehavior[]
			{
				new MissionOptionsComponent(),
				new CampaignMissionComponent(),
				new MissionBasicTeamLogic(),
				new TrainingFieldMissionController(),
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
				new AgentHumanAILogic(),
				new MissionCrimeHandler(),
				new MissionDebugHandler(),
				new MissionHardBorderPlacer(),
				new MissionBoundaryPlacer(),
				new MissionBoundaryCrossingHandler(),
				new VisualTrackerMissionBehavior(),
				new EquipmentControllerLeaveLogic()
			}, true, true);
		}
	}
}
