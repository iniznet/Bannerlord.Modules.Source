using System;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.ObjectSystem;

namespace TaleWorlds.MountAndBlade
{
	public class MBEditor
	{
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

		[MBCallback]
		internal static void DestroyEditor(Scene scene)
		{
			MBAgentRendererSceneController.DestructAgentRendererSceneController(MBEditor._editorScene, MBEditor._agentRendererSceneController, false);
			MBEditor._editorScene.ClearAll();
			MBEditor._editorScene = null;
			MBEditor._agentRendererSceneController = null;
		}

		public static bool IsEditModeOn
		{
			get
			{
				return MBAPI.IMBEditor.IsEditMode();
			}
		}

		public static bool EditModeEnabled
		{
			get
			{
				return MBAPI.IMBEditor.IsEditModeEnabled();
			}
		}

		public static void UpdateSceneTree()
		{
			MBAPI.IMBEditor.UpdateSceneTree();
		}

		public static bool IsEntitySelected(GameEntity entity)
		{
			return MBAPI.IMBEditor.IsEntitySelected(entity.Pointer);
		}

		public static void RenderEditorMesh(MetaMesh mesh, MatrixFrame frame)
		{
			MBAPI.IMBEditor.RenderEditorMesh(mesh.Pointer, ref frame);
		}

		public static void EnterEditMode(SceneView sceneView, MatrixFrame initialCameraFrame, float initialCameraElevation, float initialCameraBearing)
		{
			MBAPI.IMBEditor.EnterEditMode(sceneView.Pointer, ref initialCameraFrame, initialCameraElevation, initialCameraBearing);
		}

		public static void TickEditMode(float dt)
		{
			MBAPI.IMBEditor.TickEditMode(dt);
		}

		public static void LeaveEditMode()
		{
			MBAPI.IMBEditor.LeaveEditMode();
			MBAgentRendererSceneController.DestructAgentRendererSceneController(MBEditor._editorScene, MBEditor._agentRendererSceneController, false);
			MBEditor._agentRendererSceneController = null;
			MBEditor._editorScene = null;
		}

		public static void EnterEditMissionMode(Mission mission)
		{
			MBAPI.IMBEditor.EnterEditMissionMode(mission.Pointer);
			MBEditor._isEditorMissionOn = true;
		}

		public static void LeaveEditMissionMode()
		{
			MBAPI.IMBEditor.LeaveEditMissionMode();
			MBEditor._isEditorMissionOn = false;
		}

		public static bool IsEditorMissionOn()
		{
			return MBEditor._isEditorMissionOn && MBEditor.IsEditModeOn;
		}

		public static void ActivateSceneEditorPresentation()
		{
			Monster.GetBoneIndexWithId = new Func<string, string, sbyte>(MBActionSet.GetBoneIndexWithId);
			Monster.GetBoneHasParentBone = new Func<string, sbyte, bool>(MBActionSet.GetBoneHasParentBone);
			MBObjectManager.Init();
			MBObjectManager.Instance.RegisterType<Monster>("Monster", "Monsters", 2U, true, false);
			MBObjectManager.Instance.LoadXML("Monsters", true);
			MBAPI.IMBEditor.ActivateSceneEditorPresentation();
		}

		public static void DeactivateSceneEditorPresentation()
		{
			MBAPI.IMBEditor.DeactivateSceneEditorPresentation();
			MBObjectManager.Instance.Destroy();
		}

		public static void TickSceneEditorPresentation(float dt)
		{
			MBAPI.IMBEditor.TickSceneEditorPresentation(dt);
			LoadingWindow.DisableGlobalLoadingWindow();
		}

		public static SceneView GetEditorSceneView()
		{
			return MBAPI.IMBEditor.GetEditorSceneView();
		}

		public static bool HelpersEnabled()
		{
			return MBAPI.IMBEditor.HelpersEnabled();
		}

		public static bool BorderHelpersEnabled()
		{
			return MBAPI.IMBEditor.BorderHelpersEnabled();
		}

		public static void ZoomToPosition(Vec3 pos)
		{
			MBAPI.IMBEditor.ZoomToPosition(pos);
		}

		public static bool IsReplayManagerReplaying()
		{
			return MBAPI.IMBEditor.IsReplayManagerReplaying();
		}

		public static bool IsReplayManagerRendering()
		{
			return MBAPI.IMBEditor.IsReplayManagerRendering();
		}

		public static bool IsReplayManagerRecording()
		{
			return MBAPI.IMBEditor.IsReplayManagerRecording();
		}

		public static void AddEditorWarning(string msg)
		{
			MBAPI.IMBEditor.AddEditorWarning(msg);
		}

		public static void AddEntityWarning(GameEntity entityId, string msg)
		{
			MBAPI.IMBEditor.AddEntityWarning(entityId.Pointer, msg);
		}

		public static string GetAllPrefabsAndChildWithTag(string tag)
		{
			return MBAPI.IMBEditor.GetAllPrefabsAndChildWithTag(tag);
		}

		public static void ExitEditMode()
		{
			MBAPI.IMBEditor.ExitEditMode();
		}

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

		public static void SetLevelVisibility(List<string> levels)
		{
		}

		public static Scene _editorScene;

		private static MBAgentRendererSceneController _agentRendererSceneController;

		public static bool _isEditorMissionOn;
	}
}
