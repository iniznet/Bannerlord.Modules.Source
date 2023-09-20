using System;
using SandBox.Conversation.MissionLogics;
using SandBox.Missions.MissionLogics;
using SandBox.Missions.MissionLogics.Test;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Conversation;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Source.Missions;

namespace SandBox
{
	[MissionManager]
	public static class TestMissions
	{
		[MissionMethod]
		public static Mission OpenEquipmentTestMission(string scene)
		{
			return MissionState.OpenNew("EquipmentTest", new MissionInitializerRecord(scene), (Mission missionController) => new MissionBehavior[]
			{
				new MissionOptionsComponent(),
				new BasicLeaveMissionLogic(false, 0),
				new EquipmentTestMissionController(),
				new MissionHardBorderPlacer(),
				new MissionBoundaryPlacer(),
				new MissionBoundaryCrossingHandler(),
				new EquipmentControllerLeaveLogic()
			}, true, true);
		}

		[MissionMethod(UsableByEditor = true)]
		public static Mission OpenFacialAnimTestMission(string scene, string sceneLevels = "")
		{
			string text = "FacialAnimTest";
			MissionInitializerRecord missionInitializerRecord;
			missionInitializerRecord..ctor(scene);
			missionInitializerRecord.PlayingInCampaignMode = Campaign.Current.GameMode == 1;
			missionInitializerRecord.AtmosphereOnCampaign = ((Campaign.Current.GameMode == 1) ? Campaign.Current.Models.MapWeatherModel.GetAtmosphereModel(CampaignTime.Now, MobileParty.MainParty.GetLogicalPosition()) : null);
			missionInitializerRecord.SceneLevels = sceneLevels;
			return MissionState.OpenNew(text, missionInitializerRecord, (Mission missionController) => new MissionBehavior[]
			{
				new CampaignMissionComponent(),
				new FacialAnimationTestController(),
				new MissionConversationLogic(),
				new MissionOptionsComponent(),
				new AgentHumanAILogic(),
				new MissionHardBorderPlacer(),
				new MissionBoundaryPlacer(),
				new MissionBoundaryCrossingHandler(),
				new EquipmentControllerLeaveLogic()
			}, true, true);
		}

		[MissionMethod(UsableByEditor = true)]
		public static void OpenConversationMissionForEditor(string scene, string sceneLevels)
		{
			string text = "Conversation";
			MissionInitializerRecord missionInitializerRecord;
			missionInitializerRecord..ctor(scene);
			missionInitializerRecord.PlayingInCampaignMode = false;
			missionInitializerRecord.AtmosphereOnCampaign = null;
			missionInitializerRecord.SceneLevels = sceneLevels;
			MissionState.OpenNew(text, missionInitializerRecord, (Mission mission) => new MissionBehavior[]
			{
				new CampaignMissionComponent(),
				new MissionOptionsComponent(),
				new EquipmentControllerLeaveLogic(),
				new MissionConversationLogic(),
				new ConversationMissionLogic(new ConversationCharacterData(Game.Current.ObjectManager.GetObject<CharacterObject>("crazy_man"), null, false, false, false, false, false, false), new ConversationCharacterData(Game.Current.ObjectManager.GetObject<CharacterObject>("crazy_man"), null, false, false, false, false, false, false))
			}, true, true);
		}

		[MissionMethod(UsableByEditor = true)]
		public static Mission OpenSimpleMountedPlayerMission(string scene, string sceneLevels)
		{
			string text = "SimpleMountedPlayer";
			MissionInitializerRecord missionInitializerRecord;
			missionInitializerRecord..ctor(scene);
			missionInitializerRecord.PlayingInCampaignMode = Campaign.Current.GameMode == 1;
			missionInitializerRecord.AtmosphereOnCampaign = ((Campaign.Current.GameMode == 1) ? Campaign.Current.Models.MapWeatherModel.GetAtmosphereModel(CampaignTime.Now, MobileParty.MainParty.GetLogicalPosition()) : null);
			missionInitializerRecord.SceneLevels = sceneLevels;
			return MissionState.OpenNew(text, missionInitializerRecord, (Mission missionController) => new MissionBehavior[]
			{
				new MissionOptionsComponent(),
				new SimpleMountedPlayerMissionController(),
				new EquipmentControllerLeaveLogic()
			}, true, true);
		}
	}
}
