using System;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x0200018F RID: 399
	[ScriptingInterfaceBase]
	internal interface IMBAgentVisuals
	{
		// Token: 0x060014D7 RID: 5335
		[EngineMethod("validate_agent_visuals_reseted", false)]
		void ValidateAgentVisualsReseted(UIntPtr scenePointer, UIntPtr agentRendererSceneControllerPointer);

		// Token: 0x060014D8 RID: 5336
		[EngineMethod("create_agent_renderer_scene_controller", false)]
		UIntPtr CreateAgentRendererSceneController(UIntPtr scenePointer, int maxRenderCount);

		// Token: 0x060014D9 RID: 5337
		[EngineMethod("destruct_agent_renderer_scene_controller", false)]
		void DestructAgentRendererSceneController(UIntPtr scenePointer, UIntPtr agentRendererSceneControllerPointer, bool deleteThisFrame);

		// Token: 0x060014DA RID: 5338
		[EngineMethod("set_do_timer_based_skeleton_forced_updates", false)]
		void SetDoTimerBasedForcedSkeletonUpdates(UIntPtr agentRendererSceneControllerPointer, bool value);

		// Token: 0x060014DB RID: 5339
		[EngineMethod("set_enforced_visibility_for_all_agents", false)]
		void SetEnforcedVisibilityForAllAgents(UIntPtr scenePointer, UIntPtr agentRendererSceneControllerPointer);

		// Token: 0x060014DC RID: 5340
		[EngineMethod("create_agent_visuals", false)]
		MBAgentVisuals CreateAgentVisuals(UIntPtr scenePtr, string ownerName, Vec3 eyeOffset);

		// Token: 0x060014DD RID: 5341
		[EngineMethod("tick", false)]
		void Tick(UIntPtr agentVisualsId, UIntPtr parentAgentVisualsId, float dt, bool entityMoving, float speed);

		// Token: 0x060014DE RID: 5342
		[EngineMethod("set_entity", false)]
		void SetEntity(UIntPtr agentVisualsId, UIntPtr entityPtr);

		// Token: 0x060014DF RID: 5343
		[EngineMethod("set_skeleton", false)]
		void SetSkeleton(UIntPtr agentVisualsId, UIntPtr skeletonPtr);

		// Token: 0x060014E0 RID: 5344
		[EngineMethod("fill_entity_with_body_meshes_without_agent_visuals", false)]
		void FillEntityWithBodyMeshesWithoutAgentVisuals(UIntPtr entityPoinbter, ref SkinGenerationParams skinParams, ref BodyProperties bodyProperties, MetaMesh glovesMesh);

		// Token: 0x060014E1 RID: 5345
		[EngineMethod("add_skin_meshes_to_agent_visuals", false)]
		void AddSkinMeshesToAgentEntity(UIntPtr agentVisualsId, ref SkinGenerationParams skinParams, ref BodyProperties bodyProperties, bool useGPUMorph, bool useFaceCache);

		// Token: 0x060014E2 RID: 5346
		[EngineMethod("set_lod_atlas_shading_index", false)]
		void SetLodAtlasShadingIndex(UIntPtr agentVisualsId, int index, bool useTeamColor, uint teamColor1, uint teamColor2);

		// Token: 0x060014E3 RID: 5347
		[EngineMethod("set_face_generation_params", false)]
		void SetFaceGenerationParams(UIntPtr agentVisualsId, FaceGenerationParams faceGenerationParams);

		// Token: 0x060014E4 RID: 5348
		[EngineMethod("start_rhubarb_record", false)]
		void StartRhubarbRecord(UIntPtr agentVisualsId, string path, int soundId);

		// Token: 0x060014E5 RID: 5349
		[EngineMethod("clear_visual_components", false)]
		void ClearVisualComponents(UIntPtr agentVisualsId, bool removeSkeleton);

		// Token: 0x060014E6 RID: 5350
		[EngineMethod("lazy_update_agent_renderer_data", false)]
		void LazyUpdateAgentRendererData(UIntPtr agentVisualsId);

		// Token: 0x060014E7 RID: 5351
		[EngineMethod("add_mesh", false)]
		void AddMesh(UIntPtr agentVisualsId, UIntPtr meshPointer);

		// Token: 0x060014E8 RID: 5352
		[EngineMethod("remove_mesh", false)]
		void RemoveMesh(UIntPtr agentVisualsPtr, UIntPtr meshPointer);

		// Token: 0x060014E9 RID: 5353
		[EngineMethod("add_multi_mesh", false)]
		void AddMultiMesh(UIntPtr agentVisualsPtr, UIntPtr multiMeshPointer, int bodyMeshIndex);

		// Token: 0x060014EA RID: 5354
		[EngineMethod("add_horse_reins_cloth_mesh", false)]
		void AddHorseReinsClothMesh(UIntPtr agentVisualsPtr, UIntPtr reinMeshPointer, UIntPtr ropeMeshPointer);

		// Token: 0x060014EB RID: 5355
		[EngineMethod("update_skeleton_scale", false)]
		void UpdateSkeletonScale(UIntPtr agentVisualsId, int bodyDeformType);

		// Token: 0x060014EC RID: 5356
		[EngineMethod("apply_skeleton_scale", false)]
		void ApplySkeletonScale(UIntPtr agentVisualsId, Vec3 mountSitBoneScale, float mountRadiusAdder, byte boneCount, sbyte[] boneIndices, Vec3[] boneScales);

		// Token: 0x060014ED RID: 5357
		[EngineMethod("batch_last_lod_meshes", false)]
		void BatchLastLodMeshes(UIntPtr agentVisualsPtr);

		// Token: 0x060014EE RID: 5358
		[EngineMethod("remove_multi_mesh", false)]
		void RemoveMultiMesh(UIntPtr agentVisualsPtr, UIntPtr multiMeshPointer, int bodyMeshIndex);

		// Token: 0x060014EF RID: 5359
		[EngineMethod("add_weapon_to_agent_entity", false)]
		void AddWeaponToAgentEntity(UIntPtr agentVisualsPtr, int slotIndex, in WeaponData agentEntityData, WeaponStatsData[] weaponStatsData, int weaponStatsDataLength, in WeaponData agentEntityAmmoData, WeaponStatsData[] ammoWeaponStatsData, int ammoWeaponStatsDataLength, GameEntity cachedEntity);

		// Token: 0x060014F0 RID: 5360
		[EngineMethod("update_quiver_mesh_of_weapon_in_slot", false)]
		void UpdateQuiverMeshesWithoutAgent(UIntPtr agentVisualsId, int weaponIndex, int ammoCountToShow);

		// Token: 0x060014F1 RID: 5361
		[EngineMethod("set_wielded_weapon_indices", false)]
		void SetWieldedWeaponIndices(UIntPtr agentVisualsId, int slotIndexRightHand, int slotIndexLeftHand);

		// Token: 0x060014F2 RID: 5362
		[EngineMethod("clear_all_weapon_meshes", false)]
		void ClearAllWeaponMeshes(UIntPtr agentVisualsPtr);

		// Token: 0x060014F3 RID: 5363
		[EngineMethod("clear_weapon_meshes", false)]
		void ClearWeaponMeshes(UIntPtr agentVisualsPtr, int weaponVisualIndex);

		// Token: 0x060014F4 RID: 5364
		[EngineMethod("make_voice", false)]
		void MakeVoice(UIntPtr agentVisualsPtr, int voiceId, ref Vec3 position);

		// Token: 0x060014F5 RID: 5365
		[EngineMethod("set_setup_morph_node", false)]
		void SetSetupMorphNode(UIntPtr agentVisualsPtr, bool value);

		// Token: 0x060014F6 RID: 5366
		[EngineMethod("use_scaled_weapons", false)]
		void UseScaledWeapons(UIntPtr agentVisualsPtr, bool value);

		// Token: 0x060014F7 RID: 5367
		[EngineMethod("set_cloth_component_keep_state_of_all_meshes", false)]
		void SetClothComponentKeepStateOfAllMeshes(UIntPtr agentVisualsPtr, bool keepState);

		// Token: 0x060014F8 RID: 5368
		[EngineMethod("get_current_helmet_scaling_factor", false)]
		Vec3 GetCurrentHelmetScalingFactor(UIntPtr agentVisualsPtr);

		// Token: 0x060014F9 RID: 5369
		[EngineMethod("set_voice_definition_index", false)]
		void SetVoiceDefinitionIndex(UIntPtr agentVisualsPtr, int voiceDefinitionIndex, float voicePitch);

		// Token: 0x060014FA RID: 5370
		[EngineMethod("set_agent_lod_make_zero_or_max", false)]
		void SetAgentLodMakeZeroOrMax(UIntPtr agentVisualsPtr, bool makeZero);

		// Token: 0x060014FB RID: 5371
		[EngineMethod("set_agent_local_speed", false)]
		void SetAgentLocalSpeed(UIntPtr agentVisualsPtr, Vec2 speed);

		// Token: 0x060014FC RID: 5372
		[EngineMethod("set_look_direction", false)]
		void SetLookDirection(UIntPtr agentVisualsPtr, Vec3 direction);

		// Token: 0x060014FD RID: 5373
		[EngineMethod("reset", false)]
		void Reset(UIntPtr agentVisualsPtr);

		// Token: 0x060014FE RID: 5374
		[EngineMethod("reset_next_frame", false)]
		void ResetNextFrame(UIntPtr agentVisualsPtr);

		// Token: 0x060014FF RID: 5375
		[EngineMethod("set_frame", false)]
		void SetFrame(UIntPtr agentVisualsPtr, ref MatrixFrame frame);

		// Token: 0x06001500 RID: 5376
		[EngineMethod("get_frame", false)]
		void GetFrame(UIntPtr agentVisualsPtr, ref MatrixFrame outFrame);

		// Token: 0x06001501 RID: 5377
		[EngineMethod("get_global_frame", false)]
		void GetGlobalFrame(UIntPtr agentVisualsPtr, ref MatrixFrame outFrame);

		// Token: 0x06001502 RID: 5378
		[EngineMethod("set_visible", false)]
		void SetVisible(UIntPtr agentVisualsPtr, bool value);

		// Token: 0x06001503 RID: 5379
		[EngineMethod("get_visible", false)]
		bool GetVisible(UIntPtr agentVisualsPtr);

		// Token: 0x06001504 RID: 5380
		[EngineMethod("get_skeleton", false)]
		Skeleton GetSkeleton(UIntPtr agentVisualsPtr);

		// Token: 0x06001505 RID: 5381
		[EngineMethod("get_entity", false)]
		GameEntity GetEntity(UIntPtr agentVisualsPtr);

		// Token: 0x06001506 RID: 5382
		[EngineMethod("is_valid", false)]
		bool IsValid(UIntPtr agentVisualsPtr);

		// Token: 0x06001507 RID: 5383
		[EngineMethod("get_global_stable_eye_point", false)]
		Vec3 GetGlobalStableEyePoint(UIntPtr agentVisualsPtr, bool isHumanoid);

		// Token: 0x06001508 RID: 5384
		[EngineMethod("get_global_stable_neck_point", false)]
		Vec3 GetGlobalStableNeckPoint(UIntPtr agentVisualsPtr, bool isHumanoid);

		// Token: 0x06001509 RID: 5385
		[EngineMethod("get_bone_entitial_frame", false)]
		void GetBoneEntitialFrame(UIntPtr agentVisualsPtr, sbyte bone, bool useBoneMapping, ref MatrixFrame outFrame);

		// Token: 0x0600150A RID: 5386
		[EngineMethod("get_current_ragdoll_state", false)]
		RagdollState GetCurrentRagdollState(UIntPtr agentVisualsPtr);

		// Token: 0x0600150B RID: 5387
		[EngineMethod("get_real_bone_index", false)]
		sbyte GetRealBoneIndex(UIntPtr agentVisualsPtr, HumanBone boneType);

		// Token: 0x0600150C RID: 5388
		[EngineMethod("add_prefab_to_agent_visual_bone_by_bone_type", false)]
		CompositeComponent AddPrefabToAgentVisualBoneByBoneType(UIntPtr agentVisualsPtr, string prefabName, HumanBone boneType);

		// Token: 0x0600150D RID: 5389
		[EngineMethod("add_prefab_to_agent_visual_bone_by_real_bone_index", false)]
		CompositeComponent AddPrefabToAgentVisualBoneByRealBoneIndex(UIntPtr agentVisualsPtr, string prefabName, sbyte realBoneIndex);

		// Token: 0x0600150E RID: 5390
		[EngineMethod("get_attached_weapon_entity", false)]
		GameEntity GetAttachedWeaponEntity(UIntPtr agentVisualsPtr, int attachedWeaponIndex);

		// Token: 0x0600150F RID: 5391
		[EngineMethod("create_particle_system_attached_to_bone", false)]
		void CreateParticleSystemAttachedToBone(UIntPtr agentVisualsPtr, int runtimeParticleindex, sbyte boneIndex, ref MatrixFrame boneLocalParticleFrame);

		// Token: 0x06001510 RID: 5392
		[EngineMethod("check_resources", false)]
		bool CheckResources(UIntPtr agentVisualsPtr, bool addToQueue);

		// Token: 0x06001511 RID: 5393
		[EngineMethod("add_child_entity", false)]
		bool AddChildEntity(UIntPtr agentVisualsPtr, UIntPtr EntityId);

		// Token: 0x06001512 RID: 5394
		[EngineMethod("set_cloth_wind_to_weapon_at_index", false)]
		void SetClothWindToWeaponAtIndex(UIntPtr agentVisualsPtr, Vec3 windDirection, bool isLocal, int index);

		// Token: 0x06001513 RID: 5395
		[EngineMethod("remove_child_entity", false)]
		void RemoveChildEntity(UIntPtr agentVisualsPtr, UIntPtr EntityId, int removeReason);

		// Token: 0x06001514 RID: 5396
		[EngineMethod("disable_contour", false)]
		void DisableContour(UIntPtr agentVisualsPtr);

		// Token: 0x06001515 RID: 5397
		[EngineMethod("set_as_contour_entity", false)]
		void SetAsContourEntity(UIntPtr agentVisualsPtr, uint color);

		// Token: 0x06001516 RID: 5398
		[EngineMethod("set_contour_state", false)]
		void SetContourState(UIntPtr agentVisualsPtr, bool alwaysVisible);

		// Token: 0x06001517 RID: 5399
		[EngineMethod("set_enable_occlusion_culling", false)]
		void SetEnableOcclusionCulling(UIntPtr agentVisualsPtr, bool enable);

		// Token: 0x06001518 RID: 5400
		[EngineMethod("get_bone_type_data", false)]
		void GetBoneTypeData(UIntPtr pointer, sbyte boneIndex, ref BoneBodyTypeData boneBodyTypeData);
	}
}
