using System;
using TaleWorlds.DotNet;
using TaleWorlds.Library;

namespace TaleWorlds.Engine
{
	// Token: 0x02000024 RID: 36
	[ApplicationInterfaceBase]
	internal interface ISkeleton
	{
		// Token: 0x06000212 RID: 530
		[EngineMethod("create_from_model", false)]
		Skeleton CreateFromModel(string skeletonModelName);

		// Token: 0x06000213 RID: 531
		[EngineMethod("create_from_model_with_null_anim_tree", false)]
		Skeleton CreateFromModelWithNullAnimTree(UIntPtr entityPointer, string skeletonModelName, float scale);

		// Token: 0x06000214 RID: 532
		[EngineMethod("freeze", false)]
		void Freeze(UIntPtr skeletonPointer, bool isFrozen);

		// Token: 0x06000215 RID: 533
		[EngineMethod("is_frozen", false)]
		bool IsFrozen(UIntPtr skeletonPointer);

		// Token: 0x06000216 RID: 534
		[EngineMethod("add_mesh_to_bone", false)]
		void AddMeshToBone(UIntPtr skeletonPointer, UIntPtr multiMeshPointer, sbyte bone_index);

		// Token: 0x06000217 RID: 535
		[EngineMethod("get_bone_child_count", false)]
		sbyte GetBoneChildCount(Skeleton skeleton, sbyte boneIndex);

		// Token: 0x06000218 RID: 536
		[EngineMethod("get_bone_child_at_index", false)]
		sbyte GetBoneChildAtIndex(Skeleton skeleton, sbyte boneIndex, sbyte childIndex);

		// Token: 0x06000219 RID: 537
		[EngineMethod("get_bone_name", false)]
		string GetBoneName(Skeleton skeleton, sbyte boneIndex);

		// Token: 0x0600021A RID: 538
		[EngineMethod("get_name", false)]
		string GetName(Skeleton skeleton);

		// Token: 0x0600021B RID: 539
		[EngineMethod("get_parent_bone_index", false)]
		sbyte GetParentBoneIndex(Skeleton skeleton, sbyte boneIndex);

		// Token: 0x0600021C RID: 540
		[EngineMethod("set_bone_local_frame", false)]
		void SetBoneLocalFrame(UIntPtr skeletonPointer, sbyte boneIndex, ref MatrixFrame localFrame);

		// Token: 0x0600021D RID: 541
		[EngineMethod("get_bone_count", false)]
		sbyte GetBoneCount(UIntPtr skeletonPointer);

		// Token: 0x0600021E RID: 542
		[EngineMethod("force_update_bone_frames", false)]
		void ForceUpdateBoneFrames(UIntPtr skeletonPointer);

		// Token: 0x0600021F RID: 543
		[EngineMethod("get_bone_entitial_frame_with_index", false)]
		void GetBoneEntitialFrameWithIndex(UIntPtr skeletonPointer, sbyte boneIndex, ref MatrixFrame outEntitialFrame);

		// Token: 0x06000220 RID: 544
		[EngineMethod("get_bone_entitial_frame_with_name", false)]
		void GetBoneEntitialFrameWithName(UIntPtr skeletonPointer, string boneName, ref MatrixFrame outEntitialFrame);

		// Token: 0x06000221 RID: 545
		[EngineMethod("add_prefab_entity_to_bone", false)]
		void AddPrefabEntityToBone(UIntPtr skeletonPointer, string prefab_name, sbyte boneIndex);

		// Token: 0x06000222 RID: 546
		[EngineMethod("get_skeleton_bone_mapping", false)]
		sbyte GetSkeletonBoneMapping(UIntPtr skeletonPointer, sbyte boneIndex);

		// Token: 0x06000223 RID: 547
		[EngineMethod("add_mesh", false)]
		void AddMesh(UIntPtr skeletonPointer, UIntPtr mesnPointer);

		// Token: 0x06000224 RID: 548
		[EngineMethod("clear_meshes", false)]
		void ClearMeshes(UIntPtr skeletonPointer, bool clearBoneComponents);

		// Token: 0x06000225 RID: 549
		[EngineMethod("get_bone_body", false)]
		void GetBoneBody(UIntPtr skeletonPointer, sbyte boneIndex, ref CapsuleData data);

		// Token: 0x06000226 RID: 550
		[EngineMethod("get_current_ragdoll_state", false)]
		RagdollState GetCurrentRagdollState(UIntPtr skeletonPointer);

		// Token: 0x06000227 RID: 551
		[EngineMethod("activate_ragdoll", false)]
		void ActivateRagdoll(UIntPtr skeletonPointer);

		// Token: 0x06000228 RID: 552
		[EngineMethod("skeleton_model_exist", false)]
		bool SkeletonModelExist(string skeletonModelName);

		// Token: 0x06000229 RID: 553
		[EngineMethod("get_component_at_index", false)]
		GameEntityComponent GetComponentAtIndex(UIntPtr skeletonPointer, GameEntity.ComponentType componentType, int index);

		// Token: 0x0600022A RID: 554
		[EngineMethod("get_bone_entitial_frame", false)]
		void GetBoneEntitialFrame(UIntPtr skeletonPointer, sbyte boneIndex, ref MatrixFrame outFrame);

		// Token: 0x0600022B RID: 555
		[EngineMethod("get_bone_component_count", false)]
		int GetBoneComponentCount(UIntPtr skeletonPointer, sbyte boneIndex);

		// Token: 0x0600022C RID: 556
		[EngineMethod("add_component_to_bone", false)]
		void AddComponentToBone(UIntPtr skeletonPointer, sbyte boneIndex, GameEntityComponent component);

		// Token: 0x0600022D RID: 557
		[EngineMethod("get_bone_component_at_index", false)]
		GameEntityComponent GetBoneComponentAtIndex(UIntPtr skeletonPointer, sbyte boneIndex, int componentIndex);

		// Token: 0x0600022E RID: 558
		[EngineMethod("has_bone_component", false)]
		bool HasBoneComponent(UIntPtr skeletonPointer, sbyte boneIndex, GameEntityComponent component);

		// Token: 0x0600022F RID: 559
		[EngineMethod("remove_bone_component", false)]
		void RemoveBoneComponent(UIntPtr skeletonPointer, sbyte boneIndex, GameEntityComponent component);

		// Token: 0x06000230 RID: 560
		[EngineMethod("clear_meshes_at_bone", false)]
		void ClearMeshesAtBone(UIntPtr skeletonPointer, sbyte boneIndex);

		// Token: 0x06000231 RID: 561
		[EngineMethod("get_component_count", false)]
		int GetComponentCount(UIntPtr skeletonPointer, GameEntity.ComponentType componentType);

		// Token: 0x06000232 RID: 562
		[EngineMethod("set_use_precise_bounding_volume", false)]
		void SetUsePreciseBoundingVolume(UIntPtr skeletonPointer, bool value);

		// Token: 0x06000233 RID: 563
		[EngineMethod("tick_animations", false)]
		void TickAnimations(UIntPtr skeletonPointer, ref MatrixFrame globalFrame, float dt, bool tickAnimsForChildren);

		// Token: 0x06000234 RID: 564
		[EngineMethod("tick_animations_and_force_update", false)]
		void TickAnimationsAndForceUpdate(UIntPtr skeletonPointer, ref MatrixFrame globalFrame, float dt, bool tickAnimsForChildren);

		// Token: 0x06000235 RID: 565
		[EngineMethod("get_skeleton_animation_parameter_at_channel", false)]
		float GetSkeletonAnimationParameterAtChannel(UIntPtr skeletonPointer, int channelNo);

		// Token: 0x06000236 RID: 566
		[EngineMethod("set_skeleton_animation_parameter_at_channel", false)]
		void SetSkeletonAnimationParameterAtChannel(UIntPtr skeletonPointer, int channelNo, float parameter);

		// Token: 0x06000237 RID: 567
		[EngineMethod("get_skeleton_animation_speed_at_channel", false)]
		float GetSkeletonAnimationSpeedAtChannel(UIntPtr skeletonPointer, int channelNo);

		// Token: 0x06000238 RID: 568
		[EngineMethod("set_skeleton_animation_speed_at_channel", false)]
		void SetSkeletonAnimationSpeedAtChannel(UIntPtr skeletonPointer, int channelNo, float speed);

		// Token: 0x06000239 RID: 569
		[EngineMethod("set_up_to_date", false)]
		void SetSkeletonUptoDate(UIntPtr skeletonPointer, bool value);

		// Token: 0x0600023A RID: 570
		[EngineMethod("get_bone_entitial_rest_frame", false)]
		void GetBoneEntitialRestFrame(UIntPtr skeletonPointer, sbyte boneIndex, bool useBoneMapping, ref MatrixFrame outFrame);

		// Token: 0x0600023B RID: 571
		[EngineMethod("get_bone_local_rest_frame", false)]
		void GetBoneLocalRestFrame(UIntPtr skeletonPointer, sbyte boneIndex, bool useBoneMapping, ref MatrixFrame outFrame);

		// Token: 0x0600023C RID: 572
		[EngineMethod("get_bone_entitial_frame_at_channel", false)]
		void GetBoneEntitialFrameAtChannel(UIntPtr skeletonPointer, int channelNo, sbyte boneIndex, ref MatrixFrame outFrame);

		// Token: 0x0600023D RID: 573
		[EngineMethod("get_animation_at_channel", false)]
		string GetAnimationAtChannel(UIntPtr skeletonPointer, int channelNo);

		// Token: 0x0600023E RID: 574
		[EngineMethod("get_animation_index_at_channel", false)]
		int GetAnimationIndexAtChannel(UIntPtr skeletonPointer, int channelNo);

		// Token: 0x0600023F RID: 575
		[EngineMethod("reset_cloths", false)]
		void ResetCloths(UIntPtr skeletonPointer);

		// Token: 0x06000240 RID: 576
		[EngineMethod("reset_frames", false)]
		void ResetFrames(UIntPtr skeletonPointer);

		// Token: 0x06000241 RID: 577
		[EngineMethod("clear_components", false)]
		void ClearComponents(UIntPtr skeletonPointer);

		// Token: 0x06000242 RID: 578
		[EngineMethod("add_component", false)]
		void AddComponent(UIntPtr skeletonPointer, UIntPtr componentPointer);

		// Token: 0x06000243 RID: 579
		[EngineMethod("has_component", false)]
		bool HasComponent(UIntPtr skeletonPointer, UIntPtr componentPointer);

		// Token: 0x06000244 RID: 580
		[EngineMethod("remove_component", false)]
		void RemoveComponent(UIntPtr SkeletonPointer, UIntPtr componentPointer);

		// Token: 0x06000245 RID: 581
		[EngineMethod("update_entitial_frames_from_local_frames", false)]
		void UpdateEntitialFramesFromLocalFrames(UIntPtr skeletonPointer);

		// Token: 0x06000246 RID: 582
		[EngineMethod("get_all_meshes", false)]
		void GetAllMeshes(Skeleton skeleton, NativeObjectArray nativeObjectArray);

		// Token: 0x06000247 RID: 583
		[EngineMethod("get_bone_index_from_name", false)]
		sbyte GetBoneIndexFromName(string skeletonModelName, string boneName);
	}
}
