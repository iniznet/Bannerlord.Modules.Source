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
	// Token: 0x02000014 RID: 20
	public class MissionCampaignView : MissionView
	{
		// Token: 0x06000072 RID: 114 RVA: 0x00005120 File Offset: 0x00003320
		public override void OnMissionScreenPreLoad()
		{
			this._mapScreen = MapScreen.Instance;
			if (this._mapScreen != null && base.Mission.NeedsMemoryCleanup && ScreenManager.ScreenTypeExistsAtList(this._mapScreen))
			{
				this._mapScreen.ClearGPUMemory();
				Utilities.ClearShaderMemory();
			}
		}

		// Token: 0x06000073 RID: 115 RVA: 0x0000515F File Offset: 0x0000335F
		public override void OnMissionScreenFinalize()
		{
			MapScreen mapScreen = this._mapScreen;
			if (((mapScreen != null) ? mapScreen.BannerTexturedMaterialCache : null) != null)
			{
				this._mapScreen.BannerTexturedMaterialCache.Clear();
			}
			Game.Current.EventManager.TriggerEvent<TutorialContextChangedEvent>(new TutorialContextChangedEvent(0));
		}

		// Token: 0x06000074 RID: 116 RVA: 0x0000519C File Offset: 0x0000339C
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
			TaleWorlds.InputSystem.Input.SetClipboardText(text);
			return "Copied to clipboard:\n" + text;
		}

		// Token: 0x06000075 RID: 117 RVA: 0x00005278 File Offset: 0x00003478
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

		// Token: 0x06000076 RID: 118 RVA: 0x000052D7 File Offset: 0x000034D7
		public override void OnRenderingStarted()
		{
			Game.Current.EventManager.TriggerEvent<TutorialContextChangedEvent>(new TutorialContextChangedEvent(8));
		}

		// Token: 0x0400003A RID: 58
		private MapScreen _mapScreen;

		// Token: 0x0400003B RID: 59
		private MissionMainAgentController _missionMainAgentController;
	}
}
