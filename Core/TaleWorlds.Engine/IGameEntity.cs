using System;
using TaleWorlds.Library;

namespace TaleWorlds.Engine
{
	// Token: 0x02000026 RID: 38
	[ApplicationInterfaceBase]
	internal interface IGameEntity
	{
		// Token: 0x0600024A RID: 586
		[EngineMethod("get_scene", false)]
		Scene GetScene(UIntPtr entityId);

		// Token: 0x0600024B RID: 587
		[EngineMethod("get_scene_pointer", false)]
		UIntPtr GetScenePointer(UIntPtr entityId);

		// Token: 0x0600024C RID: 588
		[EngineMethod("get_first_mesh", false)]
		Mesh GetFirstMesh(UIntPtr entityId);

		// Token: 0x0600024D RID: 589
		[EngineMethod("create_from_prefab", false)]
		GameEntity CreateFromPrefab(UIntPtr scenePointer, string prefabid, bool callScriptCallbacks);

		// Token: 0x0600024E RID: 590
		[EngineMethod("call_script_callbacks", false)]
		void CallScriptCallbacks(UIntPtr entityPointer);

		// Token: 0x0600024F RID: 591
		[EngineMethod("create_from_prefab_with_initial_frame", false)]
		GameEntity CreateFromPrefabWithInitialFrame(UIntPtr scenePointer, string prefabid, ref MatrixFrame frame);

		// Token: 0x06000250 RID: 592
		[EngineMethod("add_component", false)]
		void AddComponent(UIntPtr pointer, UIntPtr componentPointer);

		// Token: 0x06000251 RID: 593
		[EngineMethod("remove_component", false)]
		bool RemoveComponent(UIntPtr pointer, UIntPtr componentPointer);

		// Token: 0x06000252 RID: 594
		[EngineMethod("has_component", false)]
		bool HasComponent(UIntPtr pointer, UIntPtr componentPointer);

		// Token: 0x06000253 RID: 595
		[EngineMethod("update_global_bounds", false)]
		void UpdateGlobalBounds(UIntPtr entityPointer);

		// Token: 0x06000254 RID: 596
		[EngineMethod("validate_bounding_box", false)]
		void ValidateBoundingBox(UIntPtr entityPointer);

		// Token: 0x06000255 RID: 597
		[EngineMethod("clear_components", false)]
		void ClearComponents(UIntPtr entityId);

		// Token: 0x06000256 RID: 598
		[EngineMethod("clear_only_own_components", false)]
		void ClearOnlyOwnComponents(UIntPtr entityId);

		// Token: 0x06000257 RID: 599
		[EngineMethod("clear_entity_components", false)]
		void ClearEntityComponents(UIntPtr entityId, bool resetAll, bool removeScripts, bool deleteChildEntities);

		// Token: 0x06000258 RID: 600
		[EngineMethod("update_visibility_mask", false)]
		void UpdateVisibilityMask(UIntPtr entityPtr);

		// Token: 0x06000259 RID: 601
		[EngineMethod("check_resources", false)]
		bool CheckResources(UIntPtr entityId, bool addToQueue, bool checkFaceResources);

		// Token: 0x0600025A RID: 602
		[EngineMethod("set_mobility", false)]
		void SetMobility(UIntPtr entityId, int mobility);

		// Token: 0x0600025B RID: 603
		[EngineMethod("add_mesh", false)]
		void AddMesh(UIntPtr entityId, UIntPtr mesh, bool recomputeBoundingBox);

		// Token: 0x0600025C RID: 604
		[EngineMethod("add_multi_mesh_to_skeleton", false)]
		void AddMultiMeshToSkeleton(UIntPtr gameEntity, UIntPtr multiMesh);

		// Token: 0x0600025D RID: 605
		[EngineMethod("add_multi_mesh_to_skeleton_bone", false)]
		void AddMultiMeshToSkeletonBone(UIntPtr gameEntity, UIntPtr multiMesh, sbyte boneIndex);

		// Token: 0x0600025E RID: 606
		[EngineMethod("set_as_replay_entity", false)]
		void SetAsReplayEntity(UIntPtr gameEntity);

		// Token: 0x0600025F RID: 607
		[EngineMethod("set_cloth_max_distance_multiplier", false)]
		void SetClothMaxDistanceMultiplier(UIntPtr gameEntity, float multiplier);

		// Token: 0x06000260 RID: 608
		[EngineMethod("set_previous_frame_invalid", false)]
		void SetPreviousFrameInvalid(UIntPtr gameEntity);

		// Token: 0x06000261 RID: 609
		[EngineMethod("remove_multi_mesh_from_skeleton", false)]
		void RemoveMultiMeshFromSkeleton(UIntPtr gameEntity, UIntPtr multiMesh);

		// Token: 0x06000262 RID: 610
		[EngineMethod("remove_multi_mesh_from_skeleton_bone", false)]
		void RemoveMultiMeshFromSkeletonBone(UIntPtr gameEntity, UIntPtr multiMesh, sbyte boneIndex);

		// Token: 0x06000263 RID: 611
		[EngineMethod("remove_component_with_mesh", false)]
		bool RemoveComponentWithMesh(UIntPtr entityId, UIntPtr mesh);

		// Token: 0x06000264 RID: 612
		[EngineMethod("get_guid", false)]
		string GetGuid(UIntPtr entityId);

		// Token: 0x06000265 RID: 613
		[EngineMethod("is_guid_valid", false)]
		bool IsGuidValid(UIntPtr entityId);

		// Token: 0x06000266 RID: 614
		[EngineMethod("add_sphere_as_body", false)]
		void AddSphereAsBody(UIntPtr entityId, Vec3 center, float radius, uint bodyFlags);

		// Token: 0x06000267 RID: 615
		[EngineMethod("get_quick_bone_entitial_frame", false)]
		void GetQuickBoneEntitialFrame(UIntPtr entityId, sbyte index, ref MatrixFrame frame);

		// Token: 0x06000268 RID: 616
		[EngineMethod("create_empty", false)]
		GameEntity CreateEmpty(UIntPtr scenePointer, bool isModifiableFromEditor, UIntPtr entityId);

		// Token: 0x06000269 RID: 617
		[EngineMethod("create_empty_without_scene", false)]
		GameEntity CreateEmptyWithoutScene();

		// Token: 0x0600026A RID: 618
		[EngineMethod("remove", false)]
		void Remove(UIntPtr entityId, int removeReason);

		// Token: 0x0600026B RID: 619
		[EngineMethod("find_with_name", false)]
		GameEntity FindWithName(UIntPtr scenePointer, string name);

		// Token: 0x0600026C RID: 620
		[EngineMethod("get_frame", false)]
		void GetFrame(UIntPtr entityId, ref MatrixFrame outFrame);

		// Token: 0x0600026D RID: 621
		[EngineMethod("set_frame", false)]
		void SetFrame(UIntPtr entityId, ref MatrixFrame frame);

		// Token: 0x0600026E RID: 622
		[EngineMethod("set_cloth_component_keep_state", false)]
		void SetClothComponentKeepState(UIntPtr entityId, UIntPtr metaMesh, bool keepState);

		// Token: 0x0600026F RID: 623
		[EngineMethod("set_cloth_component_keep_state_of_all_meshes", false)]
		void SetClothComponentKeepStateOfAllMeshes(UIntPtr entityId, bool keepState);

		// Token: 0x06000270 RID: 624
		[EngineMethod("update_triad_frame_for_editor", false)]
		void UpdateTriadFrameForEditor(UIntPtr meshPointer);

		// Token: 0x06000271 RID: 625
		[EngineMethod("get_global_frame", false)]
		void GetGlobalFrame(UIntPtr meshPointer, out MatrixFrame outFrame);

		// Token: 0x06000272 RID: 626
		[EngineMethod("set_global_frame", false)]
		void SetGlobalFrame(UIntPtr entityId, in MatrixFrame frame);

		// Token: 0x06000273 RID: 627
		[EngineMethod("get_previous_global_frame", false)]
		void GetPreviousGlobalFrame(UIntPtr entityPtr, out MatrixFrame frame);

		// Token: 0x06000274 RID: 628
		[EngineMethod("has_physics_body", false)]
		bool HasPhysicsBody(UIntPtr entityId);

		// Token: 0x06000275 RID: 629
		[EngineMethod("set_local_position", false)]
		void SetLocalPosition(UIntPtr entityId, Vec3 position);

		// Token: 0x06000276 RID: 630
		[EngineMethod("get_entity_flags", false)]
		uint GetEntityFlags(UIntPtr entityId);

		// Token: 0x06000277 RID: 631
		[EngineMethod("set_entity_flags", false)]
		void SetEntityFlags(UIntPtr entityId, uint entityFlags);

		// Token: 0x06000278 RID: 632
		[EngineMethod("get_entity_visibility_flags", false)]
		uint GetEntityVisibilityFlags(UIntPtr entityId);

		// Token: 0x06000279 RID: 633
		[EngineMethod("set_entity_visibility_flags", false)]
		void SetEntityVisibilityFlags(UIntPtr entityId, uint entityVisibilityFlags);

		// Token: 0x0600027A RID: 634
		[EngineMethod("get_body_flags", false)]
		uint GetBodyFlags(UIntPtr entityId);

		// Token: 0x0600027B RID: 635
		[EngineMethod("set_body_flags", false)]
		void SetBodyFlags(UIntPtr entityId, uint bodyFlags);

		// Token: 0x0600027C RID: 636
		[EngineMethod("get_physics_desc_body_flags", false)]
		uint GetPhysicsDescBodyFlags(UIntPtr entityId);

		// Token: 0x0600027D RID: 637
		[EngineMethod("get_center_of_mass", false)]
		Vec3 GetCenterOfMass(UIntPtr entityId);

		// Token: 0x0600027E RID: 638
		[EngineMethod("get_mass", false)]
		float GetMass(UIntPtr entityId);

		// Token: 0x0600027F RID: 639
		[EngineMethod("set_mass", false)]
		void SetMass(UIntPtr entityId, float mass);

		// Token: 0x06000280 RID: 640
		[EngineMethod("set_mass_space_inertia", false)]
		void SetMassSpaceInertia(UIntPtr entityId, ref Vec3 inertia);

		// Token: 0x06000281 RID: 641
		[EngineMethod("set_damping", false)]
		void SetDamping(UIntPtr entityId, float linearDamping, float angularDamping);

		// Token: 0x06000282 RID: 642
		[EngineMethod("disable_gravity", false)]
		void DisableGravity(UIntPtr entityId);

		// Token: 0x06000283 RID: 643
		[EngineMethod("set_body_flags_recursive", false)]
		void SetBodyFlagsRecursive(UIntPtr entityId, uint bodyFlags);

		// Token: 0x06000284 RID: 644
		[EngineMethod("get_global_scale", false)]
		Vec3 GetGlobalScale(GameEntity entity);

		// Token: 0x06000285 RID: 645
		[EngineMethod("get_body_shape", false)]
		PhysicsShape GetBodyShape(GameEntity entity);

		// Token: 0x06000286 RID: 646
		[EngineMethod("set_body_shape", false)]
		void SetBodyShape(UIntPtr entityId, UIntPtr shape);

		// Token: 0x06000287 RID: 647
		[EngineMethod("add_physics", false)]
		void AddPhysics(UIntPtr entityId, UIntPtr body, float mass, ref Vec3 localCenterOfMass, ref Vec3 initialVelocity, ref Vec3 initialAngularVelocity, int physicsMaterial, bool isStatic, int collisionGroupID);

		// Token: 0x06000288 RID: 648
		[EngineMethod("remove_physics", false)]
		void RemovePhysics(UIntPtr entityId, bool clearingTheScene);

		// Token: 0x06000289 RID: 649
		[EngineMethod("set_physics_state", false)]
		void SetPhysicsState(UIntPtr entityId, bool isEnabled, bool setChildren);

		// Token: 0x0600028A RID: 650
		[EngineMethod("get_physics_state", false)]
		bool GetPhysicsState(UIntPtr entityId);

		// Token: 0x0600028B RID: 651
		[EngineMethod("add_distance_joint", false)]
		void AddDistanceJoint(UIntPtr entityId, UIntPtr otherEntityId, float minDistance, float maxDistance);

		// Token: 0x0600028C RID: 652
		[EngineMethod("has_physics_definition", false)]
		bool HasPhysicsDefinition(UIntPtr entityId, int excludeFlags);

		// Token: 0x0600028D RID: 653
		[EngineMethod("remove_engine_physics", false)]
		void RemoveEnginePhysics(UIntPtr entityId);

		// Token: 0x0600028E RID: 654
		[EngineMethod("is_engine_body_sleeping", false)]
		bool IsEngineBodySleeping(UIntPtr entityId);

		// Token: 0x0600028F RID: 655
		[EngineMethod("enable_dynamic_body", false)]
		void EnableDynamicBody(UIntPtr entityId);

		// Token: 0x06000290 RID: 656
		[EngineMethod("disable_dynamic_body_simulation", false)]
		void DisableDynamicBodySimulation(UIntPtr entityId);

		// Token: 0x06000291 RID: 657
		[EngineMethod("apply_local_impulse_to_dynamic_body", false)]
		void ApplyLocalImpulseToDynamicBody(UIntPtr entityId, ref Vec3 localPosition, ref Vec3 impulse);

		// Token: 0x06000292 RID: 658
		[EngineMethod("apply_acceleration_to_dynamic_body", false)]
		void ApplyAccelerationToDynamicBody(UIntPtr entityId, ref Vec3 acceleration);

		// Token: 0x06000293 RID: 659
		[EngineMethod("apply_force_to_dynamic_body", false)]
		void ApplyForceToDynamicBody(UIntPtr entityId, ref Vec3 force);

		// Token: 0x06000294 RID: 660
		[EngineMethod("apply_local_force_to_dynamic_body", false)]
		void ApplyLocalForceToDynamicBody(UIntPtr entityId, ref Vec3 localPosition, ref Vec3 force);

		// Token: 0x06000295 RID: 661
		[EngineMethod("add_child", false)]
		void AddChild(UIntPtr parententity, UIntPtr childentity, bool autoLocalizeFrame);

		// Token: 0x06000296 RID: 662
		[EngineMethod("remove_child", false)]
		void RemoveChild(UIntPtr parentEntity, UIntPtr childEntity, bool keepPhysics, bool keepScenePointer, bool callScriptCallbacks, int removeReason);

		// Token: 0x06000297 RID: 663
		[EngineMethod("get_child_count", false)]
		int GetChildCount(UIntPtr entityId);

		// Token: 0x06000298 RID: 664
		[EngineMethod("get_child", false)]
		GameEntity GetChild(UIntPtr entityId, int childIndex);

		// Token: 0x06000299 RID: 665
		[EngineMethod("get_parent", false)]
		GameEntity GetParent(UIntPtr entityId);

		// Token: 0x0600029A RID: 666
		[EngineMethod("has_complex_anim_tree", false)]
		bool HasComplexAnimTree(UIntPtr entityId);

		// Token: 0x0600029B RID: 667
		[EngineMethod("get_script_component", false)]
		ScriptComponentBehavior GetScriptComponent(UIntPtr entityId);

		// Token: 0x0600029C RID: 668
		[EngineMethod("get_script_component_count", false)]
		int GetScriptComponentCount(UIntPtr entityId);

		// Token: 0x0600029D RID: 669
		[EngineMethod("get_script_component_at_index", false)]
		ScriptComponentBehavior GetScriptComponentAtIndex(UIntPtr entityId, int index);

		// Token: 0x0600029E RID: 670
		[EngineMethod("set_entity_env_map_visibility", false)]
		void SetEntityEnvMapVisibility(UIntPtr entityId, bool value);

		// Token: 0x0600029F RID: 671
		[EngineMethod("create_and_add_script_component", false)]
		void CreateAndAddScriptComponent(UIntPtr entityId, string name);

		// Token: 0x060002A0 RID: 672
		[EngineMethod("remove_script_component", false)]
		void RemoveScriptComponent(UIntPtr entityId, UIntPtr scriptComponentPtr, int removeReason);

		// Token: 0x060002A1 RID: 673
		[EngineMethod("prefab_exists", false)]
		bool PrefabExists(string prefabName);

		// Token: 0x060002A2 RID: 674
		[EngineMethod("is_ghost_object", false)]
		bool IsGhostObject(UIntPtr entityId);

		// Token: 0x060002A3 RID: 675
		[EngineMethod("has_script_component", false)]
		bool HasScriptComponent(UIntPtr entityId, string scName);

		// Token: 0x060002A4 RID: 676
		[EngineMethod("has_scene", false)]
		bool HasScene(UIntPtr entityId);

		// Token: 0x060002A5 RID: 677
		[EngineMethod("get_name", false)]
		string GetName(UIntPtr entityId);

		// Token: 0x060002A6 RID: 678
		[EngineMethod("get_first_entity_with_tag", false)]
		GameEntity GetFirstEntityWithTag(UIntPtr scenePointer, string tag);

		// Token: 0x060002A7 RID: 679
		[EngineMethod("get_next_entity_with_tag", false)]
		GameEntity GetNextEntityWithTag(UIntPtr currententityId, string tag);

		// Token: 0x060002A8 RID: 680
		[EngineMethod("get_first_entity_with_tag_expression", false)]
		GameEntity GetFirstEntityWithTagExpression(UIntPtr scenePointer, string tagExpression);

		// Token: 0x060002A9 RID: 681
		[EngineMethod("get_next_entity_with_tag_expression", false)]
		GameEntity GetNextEntityWithTagExpression(UIntPtr currententityId, string tagExpression);

		// Token: 0x060002AA RID: 682
		[EngineMethod("get_next_prefab", false)]
		GameEntity GetNextPrefab(UIntPtr currentPrefab);

		// Token: 0x060002AB RID: 683
		[EngineMethod("copy_from_prefab", false)]
		GameEntity CopyFromPrefab(UIntPtr prefab);

		// Token: 0x060002AC RID: 684
		[EngineMethod("set_upgrade_level_mask", false)]
		void SetUpgradeLevelMask(UIntPtr prefab, uint mask);

		// Token: 0x060002AD RID: 685
		[EngineMethod("get_upgrade_level_mask", false)]
		uint GetUpgradeLevelMask(UIntPtr prefab);

		// Token: 0x060002AE RID: 686
		[EngineMethod("get_upgrade_level_mask_cumulative", false)]
		uint GetUpgradeLevelMaskCumulative(UIntPtr prefab);

		// Token: 0x060002AF RID: 687
		[EngineMethod("get_old_prefab_name", false)]
		string GetOldPrefabName(UIntPtr prefab);

		// Token: 0x060002B0 RID: 688
		[EngineMethod("get_prefab_name", false)]
		string GetPrefabName(UIntPtr prefab);

		// Token: 0x060002B1 RID: 689
		[EngineMethod("copy_script_component_from_another_entity", false)]
		void CopyScriptComponentFromAnotherEntity(UIntPtr prefab, UIntPtr other_prefab, string script_name);

		// Token: 0x060002B2 RID: 690
		[EngineMethod("add_multi_mesh", false)]
		void AddMultiMesh(UIntPtr entityId, UIntPtr multiMeshPtr, bool updateVisMask);

		// Token: 0x060002B3 RID: 691
		[EngineMethod("remove_multi_mesh", false)]
		bool RemoveMultiMesh(UIntPtr entityId, UIntPtr multiMeshPtr);

		// Token: 0x060002B4 RID: 692
		[EngineMethod("get_component_count", false)]
		int GetComponentCount(UIntPtr entityId, GameEntity.ComponentType componentType);

		// Token: 0x060002B5 RID: 693
		[EngineMethod("get_component_at_index", false)]
		GameEntityComponent GetComponentAtIndex(UIntPtr entityId, GameEntity.ComponentType componentType, int index);

		// Token: 0x060002B6 RID: 694
		[EngineMethod("add_all_meshes_of_game_entity", false)]
		void AddAllMeshesOfGameEntity(UIntPtr entityId, UIntPtr copiedEntityId);

		// Token: 0x060002B7 RID: 695
		[EngineMethod("set_frame_changed", false)]
		void SetFrameChanged(UIntPtr entityId);

		// Token: 0x060002B8 RID: 696
		[EngineMethod("is_visible_include_parents", false)]
		bool IsVisibleIncludeParents(UIntPtr entityId);

		// Token: 0x060002B9 RID: 697
		[EngineMethod("get_visibility_level_mask_including_parents", false)]
		uint GetVisibilityLevelMaskIncludingParents(UIntPtr entityId);

		// Token: 0x060002BA RID: 698
		[EngineMethod("get_edit_mode_level_visibility", false)]
		bool GetEditModeLevelVisibility(UIntPtr entityId);

		// Token: 0x060002BB RID: 699
		[EngineMethod("get_visibility_exclude_parents", false)]
		bool GetVisibilityExcludeParents(UIntPtr entityId);

		// Token: 0x060002BC RID: 700
		[EngineMethod("set_visibility_exclude_parents", false)]
		void SetVisibilityExcludeParents(UIntPtr entityId, bool visibility);

		// Token: 0x060002BD RID: 701
		[EngineMethod("set_alpha", false)]
		void SetAlpha(UIntPtr entityId, float alpha);

		// Token: 0x060002BE RID: 702
		[EngineMethod("set_ready_to_render", false)]
		void SetReadyToRender(UIntPtr entityId, bool ready);

		// Token: 0x060002BF RID: 703
		[EngineMethod("add_particle_system_component", false)]
		void AddParticleSystemComponent(UIntPtr entityId, string particleid);

		// Token: 0x060002C0 RID: 704
		[EngineMethod("remove_all_particle_systems", false)]
		void RemoveAllParticleSystems(UIntPtr entityId);

		// Token: 0x060002C1 RID: 705
		[EngineMethod("get_tags", false)]
		string GetTags(UIntPtr entityId);

		// Token: 0x060002C2 RID: 706
		[EngineMethod("has_tag", false)]
		bool HasTag(UIntPtr entityId, string tag);

		// Token: 0x060002C3 RID: 707
		[EngineMethod("add_tag", false)]
		void AddTag(UIntPtr entityId, string tag);

		// Token: 0x060002C4 RID: 708
		[EngineMethod("remove_tag", false)]
		void RemoveTag(UIntPtr entityId, string tag);

		// Token: 0x060002C5 RID: 709
		[EngineMethod("add_light", false)]
		bool AddLight(UIntPtr entityId, UIntPtr lightPointer);

		// Token: 0x060002C6 RID: 710
		[EngineMethod("get_light", false)]
		Light GetLight(UIntPtr entityId);

		// Token: 0x060002C7 RID: 711
		[EngineMethod("set_material_for_all_meshes", false)]
		void SetMaterialForAllMeshes(UIntPtr entityId, UIntPtr materialPointer);

		// Token: 0x060002C8 RID: 712
		[EngineMethod("set_name", false)]
		void SetName(UIntPtr entityId, string name);

		// Token: 0x060002C9 RID: 713
		[EngineMethod("set_vector_argument", false)]
		void SetVectorArgument(UIntPtr entityId, float vectorArgument0, float vectorArgument1, float vectorArgument2, float vectorArgument3);

		// Token: 0x060002CA RID: 714
		[EngineMethod("set_factor2_color", false)]
		void SetFactor2Color(UIntPtr entityId, uint factor2Color);

		// Token: 0x060002CB RID: 715
		[EngineMethod("set_factor_color", false)]
		void SetFactorColor(UIntPtr entityId, uint factorColor);

		// Token: 0x060002CC RID: 716
		[EngineMethod("get_factor_color", false)]
		uint GetFactorColor(UIntPtr entityId);

		// Token: 0x060002CD RID: 717
		[EngineMethod("set_animation_sound_activation", false)]
		void SetAnimationSoundActivation(UIntPtr entityId, bool activate);

		// Token: 0x060002CE RID: 718
		[EngineMethod("copy_components_to_skeleton", false)]
		void CopyComponentsToSkeleton(UIntPtr entityId);

		// Token: 0x060002CF RID: 719
		[EngineMethod("get_bounding_box_min", false)]
		Vec3 GetBoundingBoxMin(UIntPtr entityId);

		// Token: 0x060002D0 RID: 720
		[EngineMethod("get_bounding_box_max", false)]
		Vec3 GetBoundingBoxMax(UIntPtr entityId);

		// Token: 0x060002D1 RID: 721
		[EngineMethod("has_frame_changed", false)]
		bool HasFrameChanged(UIntPtr entityId);

		// Token: 0x060002D2 RID: 722
		[EngineMethod("set_external_references_usage", false)]
		void SetExternalReferencesUsage(UIntPtr entityId, bool value);

		// Token: 0x060002D3 RID: 723
		[EngineMethod("set_morph_frame_of_components", false)]
		void SetMorphFrameOfComponents(UIntPtr entityId, float value);

		// Token: 0x060002D4 RID: 724
		[EngineMethod("add_edit_data_user_to_all_meshes", false)]
		void AddEditDataUserToAllMeshes(UIntPtr entityId, bool entity_components, bool skeleton_components);

		// Token: 0x060002D5 RID: 725
		[EngineMethod("release_edit_data_user_to_all_meshes", false)]
		void ReleaseEditDataUserToAllMeshes(UIntPtr entityId, bool entity_components, bool skeleton_components);

		// Token: 0x060002D6 RID: 726
		[EngineMethod("get_camera_params_from_camera_script", false)]
		void GetCameraParamsFromCameraScript(UIntPtr entityId, UIntPtr camPtr, ref Vec3 dof_params);

		// Token: 0x060002D7 RID: 727
		[EngineMethod("get_mesh_bended_position", false)]
		void GetMeshBendedPosition(UIntPtr entityId, ref MatrixFrame worldSpacePosition, ref MatrixFrame output);

		// Token: 0x060002D8 RID: 728
		[EngineMethod("break_prefab", false)]
		void BreakPrefab(UIntPtr entityId);

		// Token: 0x060002D9 RID: 729
		[EngineMethod("set_anim_tree_channel_parameter", false)]
		void SetAnimTreeChannelParameter(UIntPtr entityId, float phase, int channel_no);

		// Token: 0x060002DA RID: 730
		[EngineMethod("add_mesh_to_bone", false)]
		void AddMeshToBone(UIntPtr entityId, UIntPtr multiMeshPointer, sbyte boneIndex);

		// Token: 0x060002DB RID: 731
		[EngineMethod("activate_ragdoll", false)]
		void ActivateRagdoll(UIntPtr entityId);

		// Token: 0x060002DC RID: 732
		[EngineMethod("freeze", false)]
		void Freeze(UIntPtr entityId, bool isFrozen);

		// Token: 0x060002DD RID: 733
		[EngineMethod("is_frozen", false)]
		bool IsFrozen(UIntPtr entityId);

		// Token: 0x060002DE RID: 734
		[EngineMethod("get_bone_count", false)]
		sbyte GetBoneCount(UIntPtr entityId);

		// Token: 0x060002DF RID: 735
		[EngineMethod("get_bone_entitial_frame_with_index", false)]
		void GetBoneEntitialFrameWithIndex(UIntPtr entityId, sbyte boneIndex, ref MatrixFrame outEntitialFrame);

		// Token: 0x060002E0 RID: 736
		[EngineMethod("get_bone_entitial_frame_with_name", false)]
		void GetBoneEntitialFrameWithName(UIntPtr entityId, string boneName, ref MatrixFrame outEntitialFrame);

		// Token: 0x060002E1 RID: 737
		[EngineMethod("disable_contour", false)]
		void DisableContour(UIntPtr entityId);

		// Token: 0x060002E2 RID: 738
		[EngineMethod("set_as_contour_entity", false)]
		void SetAsContourEntity(UIntPtr entityId, uint color);

		// Token: 0x060002E3 RID: 739
		[EngineMethod("set_contour_state", false)]
		void SetContourState(UIntPtr entityId, bool alwaysVisible);

		// Token: 0x060002E4 RID: 740
		[EngineMethod("recompute_bounding_box", false)]
		void RecomputeBoundingBox(GameEntity entity);

		// Token: 0x060002E5 RID: 741
		[EngineMethod("set_boundingbox_dirty", false)]
		void SetBoundingboxDirty(UIntPtr entityId);

		// Token: 0x060002E6 RID: 742
		[EngineMethod("get_global_box_max", false)]
		Vec3 GetGlobalBoxMax(UIntPtr entityId);

		// Token: 0x060002E7 RID: 743
		[EngineMethod("get_global_box_min", false)]
		Vec3 GetGlobalBoxMin(UIntPtr entityId);

		// Token: 0x060002E8 RID: 744
		[EngineMethod("get_radius", false)]
		float GetRadius(UIntPtr entityId);

		// Token: 0x060002E9 RID: 745
		[EngineMethod("change_meta_mesh_or_remove_it_if_not_exists", false)]
		void ChangeMetaMeshOrRemoveItIfNotExists(UIntPtr entityId, UIntPtr entityMetaMeshPointer, UIntPtr newMetaMeshPointer);

		// Token: 0x060002EA RID: 746
		[EngineMethod("set_skeleton", false)]
		void SetSkeleton(UIntPtr entityId, UIntPtr skeletonPointer);

		// Token: 0x060002EB RID: 747
		[EngineMethod("get_skeleton", false)]
		Skeleton GetSkeleton(UIntPtr entityId);

		// Token: 0x060002EC RID: 748
		[EngineMethod("delete_all_children", false)]
		void RemoveAllChildren(UIntPtr entityId);

		// Token: 0x060002ED RID: 749
		[EngineMethod("check_point_with_oriented_bounding_box", false)]
		bool CheckPointWithOrientedBoundingBox(UIntPtr entityId, Vec3 point);

		// Token: 0x060002EE RID: 750
		[EngineMethod("resume_particle_system", false)]
		void ResumeParticleSystem(UIntPtr entityId, bool doChildren);

		// Token: 0x060002EF RID: 751
		[EngineMethod("pause_particle_system", false)]
		void PauseParticleSystem(UIntPtr entityId, bool doChildren);

		// Token: 0x060002F0 RID: 752
		[EngineMethod("burst_entity_particle", false)]
		void BurstEntityParticle(UIntPtr entityId, bool doChildren);

		// Token: 0x060002F1 RID: 753
		[EngineMethod("set_runtime_emission_rate_multiplier", false)]
		void SetRuntimeEmissionRateMultiplier(UIntPtr entityId, float emission_rate_multiplier);

		// Token: 0x060002F2 RID: 754
		[EngineMethod("has_body", false)]
		bool HasBody(UIntPtr entityId);

		// Token: 0x060002F3 RID: 755
		[EngineMethod("attach_nav_mesh_faces_to_entity", false)]
		void AttachNavigationMeshFaces(UIntPtr entityId, int faceGroupId, bool isConnected, bool isBlocker, bool autoLocalize);

		// Token: 0x060002F4 RID: 756
		[EngineMethod("set_enforced_maximum_lod_level", false)]
		void SetEnforcedMaximumLodLevel(UIntPtr entityId, int lodLevel);

		// Token: 0x060002F5 RID: 757
		[EngineMethod("get_lod_level_for_distance_sq", false)]
		float GetLodLevelForDistanceSq(UIntPtr entityId, float distanceSquared);

		// Token: 0x060002F6 RID: 758
		[EngineMethod("is_entity_selected_on_editor", false)]
		bool IsEntitySelectedOnEditor(UIntPtr entityId);

		// Token: 0x060002F7 RID: 759
		[EngineMethod("select_entity_on_editor", false)]
		void SelectEntityOnEditor(UIntPtr entityId);

		// Token: 0x060002F8 RID: 760
		[EngineMethod("deselect_entity_on_editor", false)]
		void DeselectEntityOnEditor(UIntPtr entityId);

		// Token: 0x060002F9 RID: 761
		[EngineMethod("set_as_predisplay_entity", false)]
		void SetAsPredisplayEntity(UIntPtr entityId);

		// Token: 0x060002FA RID: 762
		[EngineMethod("remove_from_predisplay_entity", false)]
		void RemoveFromPredisplayEntity(UIntPtr entityId);

		// Token: 0x060002FB RID: 763
		[EngineMethod("get_physics_min_max", false)]
		void GetPhysicsMinMax(UIntPtr entityId, bool includeChildren, ref Vec3 bbmin, ref Vec3 bbmax, bool returnLocal);

		// Token: 0x060002FC RID: 764
		[EngineMethod("is_dynamic_body_stationary", false)]
		bool IsDynamicBodyStationary(UIntPtr entityId);

		// Token: 0x060002FD RID: 765
		[EngineMethod("set_cull_mode", false)]
		void SetCullMode(UIntPtr entityPtr, MBMeshCullingMode cullMode);

		// Token: 0x060002FE RID: 766
		[EngineMethod("get_linear_velocity", false)]
		Vec3 GetLinearVelocity(UIntPtr entityPtr);
	}
}
