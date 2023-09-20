using System;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000199 RID: 409
	[ScriptingInterfaceBase]
	internal interface IMBSkeletonExtensions
	{
		// Token: 0x060016C8 RID: 5832
		[EngineMethod("create_agent_skeleton", false)]
		Skeleton CreateAgentSkeleton(string skeletonName, bool isHumanoid, int actionSetIndex, string monsterUsageSetName, ref AnimationSystemData animationSystemData);

		// Token: 0x060016C9 RID: 5833
		[EngineMethod("create_simple_skeleton", false)]
		Skeleton CreateSimpleSkeleton(string skeletonName);

		// Token: 0x060016CA RID: 5834
		[EngineMethod("create_with_action_set", false)]
		Skeleton CreateWithActionSet(ref AnimationSystemData animationSystemData);

		// Token: 0x060016CB RID: 5835
		[EngineMethod("get_skeleton_face_animation_time", false)]
		float GetSkeletonFaceAnimationTime(UIntPtr entityId);

		// Token: 0x060016CC RID: 5836
		[EngineMethod("set_skeleton_face_animation_time", false)]
		void SetSkeletonFaceAnimationTime(UIntPtr entityId, float time);

		// Token: 0x060016CD RID: 5837
		[EngineMethod("get_skeleton_face_animation_name", false)]
		string GetSkeletonFaceAnimationName(UIntPtr entityId);

		// Token: 0x060016CE RID: 5838
		[EngineMethod("get_bone_entitial_frame_at_animation_progress", false)]
		void GetBoneEntitialFrameAtAnimationProgress(UIntPtr skeletonPointer, sbyte boneIndex, int animationIndex, float progress, ref MatrixFrame outFrame);

		// Token: 0x060016CF RID: 5839
		[EngineMethod("get_bone_entitial_frame", false)]
		void GetBoneEntitialFrame(UIntPtr skeletonPointer, sbyte bone, bool useBoneMapping, bool forceToUpdate, ref MatrixFrame outFrame);

		// Token: 0x060016D0 RID: 5840
		[EngineMethod("set_animation_at_channel", false)]
		void SetAnimationAtChannel(UIntPtr skeletonPointer, int animationIndex, int channelNo, float animationSpeedMultiplier, float blendInPeriod, float startProgress);

		// Token: 0x060016D1 RID: 5841
		[EngineMethod("get_action_at_channel", false)]
		int GetActionAtChannel(UIntPtr skeletonPointer, int channelNo);

		// Token: 0x060016D2 RID: 5842
		[EngineMethod("set_facial_animation_of_channel", false)]
		void SetFacialAnimationOfChannel(UIntPtr skeletonPointer, int channel, string facialAnimationName, bool playSound, bool loop);

		// Token: 0x060016D3 RID: 5843
		[EngineMethod("set_agent_action_channel", false)]
		void SetAgentActionChannel(UIntPtr skeletonPointer, int actionChannelNo, int actionIndex, float channelParameter, float blendPeriodOverride, bool forceFaceMorphRestart);

		// Token: 0x060016D4 RID: 5844
		[EngineMethod("does_action_continue_with_current_action_at_channel", false)]
		bool DoesActionContinueWithCurrentActionAtChannel(UIntPtr skeletonPointer, int actionChannelNo, int actionIndex);

		// Token: 0x060016D5 RID: 5845
		[EngineMethod("tick_action_channels", false)]
		void TickActionChannels(UIntPtr skeletonPointer);
	}
}
