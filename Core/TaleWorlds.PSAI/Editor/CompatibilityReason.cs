﻿using System;

namespace psai.Editor
{
	public enum CompatibilityReason
	{
		not_set,
		target_theme_will_never_interrupt_source,
		manual_setting_within_same_hierarchy,
		manual_setting_of_parent_entity,
		inherited_from_parent_hierarchy,
		target_segment_and_source_segment_are_both_only_usable_at_end,
		target_segment_is_of_a_different_group_and_is_only_usable_at_end,
		target_segment_is_a_pure_bridge_segment_within_the_same_group,
		target_segment_is_a_manual_bridge_segment_for_the_source_group,
		target_segment_is_an_automatic_bridge_segment,
		target_group_contains_at_least_one_bridge_segment,
		anything_may_be_played_after_a_pure_end_segment,
		default_behavior_of_psai,
		default_compatibility_of_the_target_segment_as_a_follower
	}
}
