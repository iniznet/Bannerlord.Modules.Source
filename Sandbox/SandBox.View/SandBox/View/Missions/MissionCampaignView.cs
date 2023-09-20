using System;
using System.Collections.Generic;
using SandBox.BoardGames.MissionLogics;
using SandBox.View.Map;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.View.MissionViews;
using TaleWorlds.MountAndBlade.View.Screens;
using TaleWorlds.ScreenSystem;

namespace SandBox.View.Missions
{
	public class MissionCampaignView : MissionView
	{
		public override void OnMissionScreenPreLoad()
		{
			this._mapScreen = MapScreen.Instance;
			if (this._mapScreen != null && base.Mission.NeedsMemoryCleanup && ScreenManager.ScreenTypeExistsAtList(this._mapScreen))
			{
				this._mapScreen.ClearGPUMemory();
				Utilities.ClearShaderMemory();
			}
		}

		public override void OnMissionScreenFinalize()
		{
			MapScreen mapScreen = this._mapScreen;
			if (((mapScreen != null) ? mapScreen.BannerTexturedMaterialCache : null) != null)
			{
				this._mapScreen.BannerTexturedMaterialCache.Clear();
			}
			Game.Current.EventManager.TriggerEvent<TutorialContextChangedEvent>(new TutorialContextChangedEvent(0));
		}

		[CommandLineFunctionality.CommandLineArgumentFunction("get_face_and_helmet_info_of_followed_agent", "mission")]
		public static string GetFaceAndHelmetInfoOfFollowedAgent(List<string> strings)
		{
			MissionScreen missionScreen = ScreenManager.TopScreen as MissionScreen;
			if (missionScreen == null)
			{
				return "Only works at missions";
			}
			Agent lastFollowedAgent = missionScreen.LastFollowedAgent;
			if (lastFollowedAgent == null)
			{
				return "An agent needs to be focussed.";
			}
			string text = "";
			text += lastFollowedAgent.BodyPropertiesValue.ToString();
			EquipmentElement equipmentFromSlot = lastFollowedAgent.SpawnEquipment.GetEquipmentFromSlot(5);
			if (!equipmentFromSlot.IsEmpty)
			{
				text = text + "\n Armor Name: " + equipmentFromSlot.Item.Name.ToString();
				text = text + "\n Mesh Name: " + equipmentFromSlot.Item.MultiMeshName;
			}
			if (lastFollowedAgent.Character != null)
			{
				CharacterObject characterObject = lastFollowedAgent.Character as CharacterObject;
				if (characterObject != null)
				{
					text = text + "\n Troop Id: " + characterObject.StringId;
				}
			}
			Input.SetClipboardText(text);
			return "Copied to clipboard:\n" + text;
		}

		public override void EarlyStart()
		{
			base.EarlyStart();
			this._missionMainAgentController = Mission.Current.GetMissionBehavior<MissionMainAgentController>();
			MissionBoardGameLogic missionBehavior = Mission.Current.GetMissionBehavior<MissionBoardGameLogic>();
			if (missionBehavior != null)
			{
				missionBehavior.GameStarted += this._missionMainAgentController.Disable;
				missionBehavior.GameEnded += this._missionMainAgentController.Enable;
			}
		}

		public override void OnRenderingStarted()
		{
			Game.Current.EventManager.TriggerEvent<TutorialContextChangedEvent>(new TutorialContextChangedEvent(8));
		}

		private MapScreen _mapScreen;

		private MissionMainAgentController _missionMainAgentController;
	}
}
