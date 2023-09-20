using System;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.Source.Missions;

namespace TaleWorlds.MountAndBlade.View
{
	public class SimpleSceneTestWithMission
	{
		public SimpleSceneTestWithMission(string sceneName, DecalAtlasGroup atlasGroup = 0)
		{
			this._sceneName = sceneName;
			this._customDecalGroup = atlasGroup;
			this._mission = this.OpenSceneWithMission(this._sceneName, "");
		}

		public bool LoadingFinished()
		{
			return this._mission.IsLoadingFinished && Utilities.GetNumberOfShaderCompilationsInProgress() == 0;
		}

		private Mission OpenSceneWithMission(string scene, string sceneLevels = "")
		{
			LoadingWindow.DisableGlobalLoadingWindow();
			string text = "SimpleSceneTestWithMission";
			MissionInitializerRecord missionInitializerRecord;
			missionInitializerRecord..ctor(scene);
			missionInitializerRecord.PlayingInCampaignMode = false;
			missionInitializerRecord.AtmosphereOnCampaign = AtmosphereInfo.GetInvalidAtmosphereInfo();
			missionInitializerRecord.DecalAtlasGroup = this._customDecalGroup;
			missionInitializerRecord.SceneLevels = sceneLevels;
			return MissionState.OpenNew(text, missionInitializerRecord, (Mission missionController) => new MissionBehavior[]
			{
				new MissionOptionsComponent(),
				new BasicLeaveMissionLogic(false, 0),
				new MissionHardBorderPlacer(),
				new MissionBoundaryPlacer(),
				new MissionBoundaryCrossingHandler(),
				new EquipmentControllerLeaveLogic()
			}, true, true);
		}

		private Mission _mission;

		private string _sceneName;

		private DecalAtlasGroup _customDecalGroup;
	}
}
