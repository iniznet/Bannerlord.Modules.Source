using System;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000194 RID: 404
	[ScriptingInterfaceBase]
	internal interface IMBEditor
	{
		// Token: 0x0600160E RID: 5646
		[EngineMethod("is_edit_mode", false)]
		bool IsEditMode();

		// Token: 0x0600160F RID: 5647
		[EngineMethod("is_edit_mode_enabled", false)]
		bool IsEditModeEnabled();

		// Token: 0x06001610 RID: 5648
		[EngineMethod("update_scene_tree", false)]
		void UpdateSceneTree();

		// Token: 0x06001611 RID: 5649
		[EngineMethod("is_entity_selected", false)]
		bool IsEntitySelected(UIntPtr entityId);

		// Token: 0x06001612 RID: 5650
		[EngineMethod("add_editor_warning", false)]
		void AddEditorWarning(string msg);

		// Token: 0x06001613 RID: 5651
		[EngineMethod("render_editor_mesh", false)]
		void RenderEditorMesh(UIntPtr metaMeshId, ref MatrixFrame frame);

		// Token: 0x06001614 RID: 5652
		[EngineMethod("enter_edit_mode", false)]
		void EnterEditMode(UIntPtr sceneWidgetPointer, ref MatrixFrame initialCameraFrame, float initialCameraElevation, float initialCameraBearing);

		// Token: 0x06001615 RID: 5653
		[EngineMethod("tick_edit_mode", false)]
		void TickEditMode(float dt);

		// Token: 0x06001616 RID: 5654
		[EngineMethod("leave_edit_mode", false)]
		void LeaveEditMode();

		// Token: 0x06001617 RID: 5655
		[EngineMethod("enter_edit_mission_mode", false)]
		void EnterEditMissionMode(UIntPtr missionPointer);

		// Token: 0x06001618 RID: 5656
		[EngineMethod("leave_edit_mission_mode", false)]
		void LeaveEditMissionMode();

		// Token: 0x06001619 RID: 5657
		[EngineMethod("activate_scene_editor_presentation", false)]
		void ActivateSceneEditorPresentation();

		// Token: 0x0600161A RID: 5658
		[EngineMethod("deactivate_scene_editor_presentation", false)]
		void DeactivateSceneEditorPresentation();

		// Token: 0x0600161B RID: 5659
		[EngineMethod("tick_scene_editor_presentation", false)]
		void TickSceneEditorPresentation(float dt);

		// Token: 0x0600161C RID: 5660
		[EngineMethod("get_editor_scene_view", false)]
		SceneView GetEditorSceneView();

		// Token: 0x0600161D RID: 5661
		[EngineMethod("helpers_enabled", false)]
		bool HelpersEnabled();

		// Token: 0x0600161E RID: 5662
		[EngineMethod("border_helpers_enabled", false)]
		bool BorderHelpersEnabled();

		// Token: 0x0600161F RID: 5663
		[EngineMethod("zoom_to_position", false)]
		void ZoomToPosition(Vec3 pos);

		// Token: 0x06001620 RID: 5664
		[EngineMethod("add_entity_warning", false)]
		void AddEntityWarning(UIntPtr entityId, string msg);

		// Token: 0x06001621 RID: 5665
		[EngineMethod("get_all_prefabs_and_child_with_tag", false)]
		string GetAllPrefabsAndChildWithTag(string tag);

		// Token: 0x06001622 RID: 5666
		[EngineMethod("set_upgrade_level_visibility", false)]
		void SetUpgradeLevelVisibility(string cumulated_string);

		// Token: 0x06001623 RID: 5667
		[EngineMethod("exit_edit_mode", false)]
		void ExitEditMode();

		// Token: 0x06001624 RID: 5668
		[EngineMethod("is_replay_manager_recording", false)]
		bool IsReplayManagerRecording();

		// Token: 0x06001625 RID: 5669
		[EngineMethod("is_replay_manager_rendering", false)]
		bool IsReplayManagerRendering();

		// Token: 0x06001626 RID: 5670
		[EngineMethod("is_replay_manager_replaying", false)]
		bool IsReplayManagerReplaying();
	}
}
