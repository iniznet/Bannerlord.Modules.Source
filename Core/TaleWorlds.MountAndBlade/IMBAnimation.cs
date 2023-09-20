using System;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000191 RID: 401
	[ScriptingInterfaceBase]
	internal interface IMBAnimation
	{
		// Token: 0x060015F0 RID: 5616
		[EngineMethod("get_id_with_index", false)]
		string GetIDWithIndex(int index);

		// Token: 0x060015F1 RID: 5617
		[EngineMethod("get_index_with_id", false)]
		int GetIndexWithID(string id);

		// Token: 0x060015F2 RID: 5618
		[EngineMethod("get_displacement_vector", false)]
		Vec3 GetDisplacementVector(int actionSetNo, int actionIndex);

		// Token: 0x060015F3 RID: 5619
		[EngineMethod("check_animation_clip_exists", false)]
		bool CheckAnimationClipExists(int actionSetNo, int actionIndex);

		// Token: 0x060015F4 RID: 5620
		[EngineMethod("prefetch_animation_clip", false)]
		void PrefetchAnimationClip(int actionSetNo, int actionIndex);

		// Token: 0x060015F5 RID: 5621
		[EngineMethod("get_animation_index_of_action_code", false)]
		int AnimationIndexOfActionCode(int actionSetNo, int actionIndex);

		// Token: 0x060015F6 RID: 5622
		[EngineMethod("get_animation_flags", false)]
		AnimFlags GetAnimationFlags(int actionSetNo, int actionIndex);

		// Token: 0x060015F7 RID: 5623
		[EngineMethod("get_action_type", false)]
		Agent.ActionCodeType GetActionType(int actionIndex);

		// Token: 0x060015F8 RID: 5624
		[EngineMethod("get_animation_duration", false)]
		float GetAnimationDuration(int animationIndex);

		// Token: 0x060015F9 RID: 5625
		[EngineMethod("get_animation_parameter1", false)]
		float GetAnimationParameter1(int animationIndex);

		// Token: 0x060015FA RID: 5626
		[EngineMethod("get_animation_parameter2", false)]
		float GetAnimationParameter2(int animationIndex);

		// Token: 0x060015FB RID: 5627
		[EngineMethod("get_animation_parameter3", false)]
		float GetAnimationParameter3(int animationIndex);

		// Token: 0x060015FC RID: 5628
		[EngineMethod("get_action_animation_duration", false)]
		float GetActionAnimationDuration(int actionSetNo, int actionIndex);

		// Token: 0x060015FD RID: 5629
		[EngineMethod("get_animation_name", false)]
		string GetAnimationName(int actionSetNo, int actionIndex);

		// Token: 0x060015FE RID: 5630
		[EngineMethod("get_animation_continue_to_action", false)]
		int GetAnimationContinueToAction(int actionSetNo, int actionIndex);

		// Token: 0x060015FF RID: 5631
		[EngineMethod("get_animation_blend_in_period", false)]
		float GetAnimationBlendInPeriod(int animationIndex);

		// Token: 0x06001600 RID: 5632
		[EngineMethod("get_action_blend_out_start_progress", false)]
		float GetActionBlendOutStartProgress(int actionSetNo, int actionIndex);

		// Token: 0x06001601 RID: 5633
		[EngineMethod("get_action_code_with_name", false)]
		int GetActionCodeWithName(string name);

		// Token: 0x06001602 RID: 5634
		[EngineMethod("get_action_name_with_code", false)]
		string GetActionNameWithCode(int index);

		// Token: 0x06001603 RID: 5635
		[EngineMethod("get_num_action_codes", false)]
		int GetNumActionCodes();

		// Token: 0x06001604 RID: 5636
		[EngineMethod("get_num_animations", false)]
		int GetNumAnimations();

		// Token: 0x06001605 RID: 5637
		[EngineMethod("is_any_animation_loading_from_disk", false)]
		bool IsAnyAnimationLoadingFromDisk();
	}
}
