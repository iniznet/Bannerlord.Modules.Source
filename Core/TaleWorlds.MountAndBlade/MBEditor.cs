using System;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.ObjectSystem;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x020001AF RID: 431
	public class MBEditor
	{
		// Token: 0x0600191B RID: 6427 RVA: 0x0005ABC8 File Offset: 0x00058DC8
		[MBCallback]
		internal static void SetEditorScene(Scene scene)
		{
			if (MBEditor._editorScene != null)
			{
				if (MBEditor._agentRendererSceneController != null)
				{
					MBAgentRendererSceneController.DestructAgentRendererSceneController(MBEditor._editorScene, MBEditor._agentRendererSceneController, false);
				}
				MBEditor._editorScene.ClearAll();
			}
			MBEditor._editorScene = scene;
			MBEditor._agentRendererSceneController = MBAgentRendererSceneController.CreateNewAgentRendererSceneController(MBEditor._editorScene, 32);
		}

		// Token: 0x0600191C RID: 6428 RVA: 0x0005AC1A File Offset: 0x00058E1A
		[MBCallback]
		internal static void CloseEditorScene()
		{
			if (MBEditor._agentRendererSceneController != null)
			{
				MBAgentRendererSceneController.DestructAgentRendererSceneController(MBEditor._editorScene, MBEditor._agentRendererSceneController, false);
			}
			MBEditor._agentRendererSceneController = null;
			MBEditor._editorScene = null;
		}

		// Token: 0x0600191D RID: 6429 RVA: 0x0005AC3F File Offset: 0x00058E3F
		[MBCallback]
		internal static void DestroyEditor(Scene scene)
		{
			MBAgentRendererSceneController.DestructAgentRendererSceneController(MBEditor._editorScene, MBEditor._agentRendererSceneController, false);
			MBEditor._editorScene.ClearAll();
			MBEditor._editorScene = null;
			MBEditor._agentRendererSceneController = null;
		}

		// Token: 0x170004FE RID: 1278
		// (get) Token: 0x0600191E RID: 6430 RVA: 0x0005AC67 File Offset: 0x00058E67
		public static bool IsEditModeOn
		{
			get
			{
				return MBAPI.IMBEditor.IsEditMode();
			}
		}

		// Token: 0x170004FF RID: 1279
		// (get) Token: 0x0600191F RID: 6431 RVA: 0x0005AC73 File Offset: 0x00058E73
		public static bool EditModeEnabled
		{
			get
			{
				return MBAPI.IMBEditor.IsEditModeEnabled();
			}
		}

		// Token: 0x06001920 RID: 6432 RVA: 0x0005AC7F File Offset: 0x00058E7F
		public static void UpdateSceneTree()
		{
			MBAPI.IMBEditor.UpdateSceneTree();
		}

		// Token: 0x06001921 RID: 6433 RVA: 0x0005AC8B File Offset: 0x00058E8B
		public static bool IsEntitySelected(GameEntity entity)
		{
			return MBAPI.IMBEditor.IsEntitySelected(entity.Pointer);
		}

		// Token: 0x06001922 RID: 6434 RVA: 0x0005AC9D File Offset: 0x00058E9D
		public static void RenderEditorMesh(MetaMesh mesh, MatrixFrame frame)
		{
			MBAPI.IMBEditor.RenderEditorMesh(mesh.Pointer, ref frame);
		}

		// Token: 0x06001923 RID: 6435 RVA: 0x0005ACB1 File Offset: 0x00058EB1
		public static void EnterEditMode(SceneView sceneView, MatrixFrame initialCameraFrame, float initialCameraElevation, float initialCameraBearing)
		{
			MBAPI.IMBEditor.EnterEditMode(sceneView.Pointer, ref initialCameraFrame, initialCameraElevation, initialCameraBearing);
		}

		// Token: 0x06001924 RID: 6436 RVA: 0x0005ACC7 File Offset: 0x00058EC7
		public static void TickEditMode(float dt)
		{
			MBAPI.IMBEditor.TickEditMode(dt);
		}

		// Token: 0x06001925 RID: 6437 RVA: 0x0005ACD4 File Offset: 0x00058ED4
		public static void LeaveEditMode()
		{
			MBAPI.IMBEditor.LeaveEditMode();
			MBAgentRendererSceneController.DestructAgentRendererSceneController(MBEditor._editorScene, MBEditor._agentRendererSceneController, false);
			MBEditor._agentRendererSceneController = null;
			MBEditor._editorScene = null;
		}

		// Token: 0x06001926 RID: 6438 RVA: 0x0005ACFC File Offset: 0x00058EFC
		public static void EnterEditMissionMode(Mission mission)
		{
			MBAPI.IMBEditor.EnterEditMissionMode(mission.Pointer);
			MBEditor._isEditorMissionOn = true;
		}

		// Token: 0x06001927 RID: 6439 RVA: 0x0005AD14 File Offset: 0x00058F14
		public static void LeaveEditMissionMode()
		{
			MBAPI.IMBEditor.LeaveEditMissionMode();
			MBEditor._isEditorMissionOn = false;
		}

		// Token: 0x06001928 RID: 6440 RVA: 0x0005AD26 File Offset: 0x00058F26
		public static bool IsEditorMissionOn()
		{
			return MBEditor._isEditorMissionOn && MBEditor.IsEditModeOn;
		}

		// Token: 0x06001929 RID: 6441 RVA: 0x0005AD38 File Offset: 0x00058F38
		public static void ActivateSceneEditorPresentation()
		{
			Monster.GetBoneIndexWithId = new Func<string, string, sbyte>(MBActionSet.GetBoneIndexWithId);
			Monster.GetBoneHasParentBone = new Func<string, sbyte, bool>(MBActionSet.GetBoneHasParentBone);
			MBObjectManager.Init();
			MBObjectManager.Instance.RegisterType<Monster>("Monster", "Monsters", 2U, true, false);
			MBObjectManager.Instance.LoadXML("Monsters", true);
			MBAPI.IMBEditor.ActivateSceneEditorPresentation();
		}

		// Token: 0x0600192A RID: 6442 RVA: 0x0005AD9E File Offset: 0x00058F9E
		public static void DeactivateSceneEditorPresentation()
		{
			MBAPI.IMBEditor.DeactivateSceneEditorPresentation();
			MBObjectManager.Instance.Destroy();
		}

		// Token: 0x0600192B RID: 6443 RVA: 0x0005ADB4 File Offset: 0x00058FB4
		public static void TickSceneEditorPresentation(float dt)
		{
			MBAPI.IMBEditor.TickSceneEditorPresentation(dt);
			LoadingWindow.DisableGlobalLoadingWindow();
		}

		// Token: 0x0600192C RID: 6444 RVA: 0x0005ADC6 File Offset: 0x00058FC6
		public static SceneView GetEditorSceneView()
		{
			return MBAPI.IMBEditor.GetEditorSceneView();
		}

		// Token: 0x0600192D RID: 6445 RVA: 0x0005ADD2 File Offset: 0x00058FD2
		public static bool HelpersEnabled()
		{
			return MBAPI.IMBEditor.HelpersEnabled();
		}

		// Token: 0x0600192E RID: 6446 RVA: 0x0005ADDE File Offset: 0x00058FDE
		public static bool BorderHelpersEnabled()
		{
			return MBAPI.IMBEditor.BorderHelpersEnabled();
		}

		// Token: 0x0600192F RID: 6447 RVA: 0x0005ADEA File Offset: 0x00058FEA
		public static void ZoomToPosition(Vec3 pos)
		{
			MBAPI.IMBEditor.ZoomToPosition(pos);
		}

		// Token: 0x06001930 RID: 6448 RVA: 0x0005ADF7 File Offset: 0x00058FF7
		public static bool IsReplayManagerReplaying()
		{
			return MBAPI.IMBEditor.IsReplayManagerReplaying();
		}

		// Token: 0x06001931 RID: 6449 RVA: 0x0005AE03 File Offset: 0x00059003
		public static bool IsReplayManagerRendering()
		{
			return MBAPI.IMBEditor.IsReplayManagerRendering();
		}

		// Token: 0x06001932 RID: 6450 RVA: 0x0005AE0F File Offset: 0x0005900F
		public static bool IsReplayManagerRecording()
		{
			return MBAPI.IMBEditor.IsReplayManagerRecording();
		}

		// Token: 0x06001933 RID: 6451 RVA: 0x0005AE1B File Offset: 0x0005901B
		public static void AddEditorWarning(string msg)
		{
			MBAPI.IMBEditor.AddEditorWarning(msg);
		}

		// Token: 0x06001934 RID: 6452 RVA: 0x0005AE28 File Offset: 0x00059028
		public static void AddEntityWarning(GameEntity entityId, string msg)
		{
			MBAPI.IMBEditor.AddEntityWarning(entityId.Pointer, msg);
		}

		// Token: 0x06001935 RID: 6453 RVA: 0x0005AE3B File Offset: 0x0005903B
		public static string GetAllPrefabsAndChildWithTag(string tag)
		{
			return MBAPI.IMBEditor.GetAllPrefabsAndChildWithTag(tag);
		}

		// Token: 0x06001936 RID: 6454 RVA: 0x0005AE48 File Offset: 0x00059048
		public static void ExitEditMode()
		{
			MBAPI.IMBEditor.ExitEditMode();
		}

		// Token: 0x06001937 RID: 6455 RVA: 0x0005AE54 File Offset: 0x00059054
		public static void SetUpgradeLevelVisibility(List<string> levels)
		{
			string text = "";
			for (int i = 0; i < levels.Count - 1; i++)
			{
				text = text + levels[i] + "|";
			}
			text += levels[levels.Count - 1];
			MBAPI.IMBEditor.SetUpgradeLevelVisibility(text);
		}

		// Token: 0x040007B7 RID: 1975
		public static Scene _editorScene;

		// Token: 0x040007B8 RID: 1976
		private static MBAgentRendererSceneController _agentRendererSceneController;

		// Token: 0x040007B9 RID: 1977
		public static bool _isEditorMissionOn;
	}
}
