using System;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x0200018E RID: 398
	[ScriptingInterfaceBase]
	internal interface IMBActionSet
	{
		// Token: 0x060014CE RID: 5326
		[EngineMethod("get_index_with_id", false)]
		int GetIndexWithID(string id);

		// Token: 0x060014CF RID: 5327
		[EngineMethod("get_name_with_index", false)]
		string GetNameWithIndex(int index);

		// Token: 0x060014D0 RID: 5328
		[EngineMethod("get_skeleton_name", false)]
		string GetSkeletonName(int index);

		// Token: 0x060014D1 RID: 5329
		[EngineMethod("get_number_of_action_sets", false)]
		int GetNumberOfActionSets();

		// Token: 0x060014D2 RID: 5330
		[EngineMethod("get_number_of_monster_usage_sets", false)]
		int GetNumberOfMonsterUsageSets();

		// Token: 0x060014D3 RID: 5331
		[EngineMethod("get_animation_name", false)]
		string GetAnimationName(int index, int actionNo);

		// Token: 0x060014D4 RID: 5332
		[EngineMethod("are_actions_alternatives", false)]
		bool AreActionsAlternatives(int index, int actionNo1, int actionNo2);

		// Token: 0x060014D5 RID: 5333
		[EngineMethod("get_bone_index_with_id", false)]
		sbyte GetBoneIndexWithId(string actionSetId, string boneId);

		// Token: 0x060014D6 RID: 5334
		[EngineMethod("get_bone_has_parent_bone", false)]
		bool GetBoneHasParentBone(string actionSetId, sbyte boneIndex);
	}
}
