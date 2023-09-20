using System;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x020001A4 RID: 420
	[ScriptingInterfaceBase]
	internal interface IMBMapScene
	{
		// Token: 0x06001715 RID: 5909
		[EngineMethod("get_accessible_point_near_position", false)]
		Vec3 GetAccessiblePointNearPosition(UIntPtr scenePointer, Vec2 position, float radius);

		// Token: 0x06001716 RID: 5910
		[EngineMethod("remove_zero_corner_bodies", false)]
		void RemoveZeroCornerBodies(UIntPtr scenePointer);

		// Token: 0x06001717 RID: 5911
		[EngineMethod("load_atmosphere_data", false)]
		void LoadAtmosphereData(UIntPtr scenePointer);

		// Token: 0x06001718 RID: 5912
		[EngineMethod("get_face_index_for_multiple_positions", false)]
		void GetFaceIndexForMultiplePositions(UIntPtr scenePointer, int movedPartyCount, float[] positionArray, PathFaceRecord[] resultArray, bool check_if_disabled, bool check_height);

		// Token: 0x06001719 RID: 5913
		[EngineMethod("set_sound_parameters", false)]
		void SetSoundParameters(UIntPtr scenePointer, float tod, int season, float cameraHeight);

		// Token: 0x0600171A RID: 5914
		[EngineMethod("tick_step_sound", false)]
		void TickStepSound(UIntPtr scenePointer, UIntPtr visualsPointer, int faceIndexterrainType, int soundType);

		// Token: 0x0600171B RID: 5915
		[EngineMethod("tick_ambient_sounds", false)]
		void TickAmbientSounds(UIntPtr scenePointer, int terrainType);

		// Token: 0x0600171C RID: 5916
		[EngineMethod("tick_visuals", false)]
		void TickVisuals(UIntPtr scenePointer, float tod, UIntPtr[] ticked_map_meshes, int tickedMapMeshesCount);

		// Token: 0x0600171D RID: 5917
		[EngineMethod("validate_terrain_sound_ids", false)]
		void ValidateTerrainSoundIds();

		// Token: 0x0600171E RID: 5918
		[EngineMethod("set_political_color", false)]
		void SetPoliticalColor(UIntPtr scenePointer, string value);

		// Token: 0x0600171F RID: 5919
		[EngineMethod("set_frame_for_atmosphere", false)]
		void SetFrameForAtmosphere(UIntPtr scenePointer, float tod, float cameraElevation, bool forceLoadTextures);

		// Token: 0x06001720 RID: 5920
		[EngineMethod("get_color_grade_grid_data", false)]
		void GetColorGradeGridData(UIntPtr scenePointer, byte[] snowData, string textureName);

		// Token: 0x06001721 RID: 5921
		[EngineMethod("get_battle_scene_index_map_resolution", false)]
		void GetBattleSceneIndexMapResolution(UIntPtr scenePointer, ref int width, ref int height);

		// Token: 0x06001722 RID: 5922
		[EngineMethod("get_battle_scene_index_map", false)]
		void GetBattleSceneIndexMap(UIntPtr scenePointer, byte[] indexData);

		// Token: 0x06001723 RID: 5923
		[EngineMethod("set_terrain_dynamic_params", false)]
		void SetTerrainDynamicParams(UIntPtr scenePointer, Vec3 dynamic_params);

		// Token: 0x06001724 RID: 5924
		[EngineMethod("set_season_time_factor", false)]
		void SetSeasonTimeFactor(UIntPtr scenePointer, float seasonTimeFactor);

		// Token: 0x06001725 RID: 5925
		[EngineMethod("get_season_time_factor", false)]
		float GetSeasonTimeFactor(UIntPtr scenePointer);

		// Token: 0x06001726 RID: 5926
		[EngineMethod("get_mouse_visible", false)]
		bool GetMouseVisible();

		// Token: 0x06001727 RID: 5927
		[EngineMethod("send_mouse_key_down_event", false)]
		void SendMouseKeyEvent(int keyId, bool isDown);

		// Token: 0x06001728 RID: 5928
		[EngineMethod("set_mouse_visible", false)]
		void SetMouseVisible(bool value);

		// Token: 0x06001729 RID: 5929
		[EngineMethod("set_mouse_pos", false)]
		void SetMousePos(int posX, int posY);
	}
}
