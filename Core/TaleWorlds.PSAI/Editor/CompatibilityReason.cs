using System;

namespace psai.Editor
{
	// Token: 0x02000006 RID: 6
	public enum CompatibilityReason
	{
		// Token: 0x04000024 RID: 36
		not_set,
		// Token: 0x04000025 RID: 37
		target_theme_will_never_interrupt_source,
		// Token: 0x04000026 RID: 38
		manual_setting_within_same_hierarchy,
		// Token: 0x04000027 RID: 39
		manual_setting_of_parent_entity,
		// Token: 0x04000028 RID: 40
		inherited_from_parent_hierarchy,
		// Token: 0x04000029 RID: 41
		target_segment_and_source_segment_are_both_only_usable_at_end,
		// Token: 0x0400002A RID: 42
		target_segment_is_of_a_different_group_and_is_only_usable_at_end,
		// Token: 0x0400002B RID: 43
		target_segment_is_a_pure_bridge_segment_within_the_same_group,
		// Token: 0x0400002C RID: 44
		target_segment_is_a_manual_bridge_segment_for_the_source_group,
		// Token: 0x0400002D RID: 45
		target_segment_is_an_automatic_bridge_segment,
		// Token: 0x0400002E RID: 46
		target_group_contains_at_least_one_bridge_segment,
		// Token: 0x0400002F RID: 47
		anything_may_be_played_after_a_pure_end_segment,
		// Token: 0x04000030 RID: 48
		default_behavior_of_psai,
		// Token: 0x04000031 RID: 49
		default_compatibility_of_the_target_segment_as_a_follower
	}
}
