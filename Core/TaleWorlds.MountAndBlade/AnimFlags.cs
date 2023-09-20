using System;
using TaleWorlds.DotNet;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000187 RID: 391
	[EngineStruct("Anim_flags")]
	[Flags]
	public enum AnimFlags : ulong
	{
		// Token: 0x040006DA RID: 1754
		amf_priority_continue = 1UL,
		// Token: 0x040006DB RID: 1755
		amf_priority_jump = 2UL,
		// Token: 0x040006DC RID: 1756
		amf_priority_ride = 2UL,
		// Token: 0x040006DD RID: 1757
		amf_priority_crouch = 2UL,
		// Token: 0x040006DE RID: 1758
		amf_priority_attack = 10UL,
		// Token: 0x040006DF RID: 1759
		amf_priority_cancel = 12UL,
		// Token: 0x040006E0 RID: 1760
		amf_priority_defend = 14UL,
		// Token: 0x040006E1 RID: 1761
		amf_priority_defend_parry = 15UL,
		// Token: 0x040006E2 RID: 1762
		amf_priority_throw = 15UL,
		// Token: 0x040006E3 RID: 1763
		amf_priority_blocked = 15UL,
		// Token: 0x040006E4 RID: 1764
		amf_priority_parried = 15UL,
		// Token: 0x040006E5 RID: 1765
		amf_priority_kick = 33UL,
		// Token: 0x040006E6 RID: 1766
		amf_priority_jump_end = 33UL,
		// Token: 0x040006E7 RID: 1767
		amf_priority_reload = 60UL,
		// Token: 0x040006E8 RID: 1768
		amf_priority_mount = 64UL,
		// Token: 0x040006E9 RID: 1769
		amf_priority_equip = 70UL,
		// Token: 0x040006EA RID: 1770
		amf_priority_rear = 74UL,
		// Token: 0x040006EB RID: 1771
		amf_priority_upperbody_while_kick = 75UL,
		// Token: 0x040006EC RID: 1772
		amf_priority_striked = 80UL,
		// Token: 0x040006ED RID: 1773
		amf_priority_fall_from_horse = 81UL,
		// Token: 0x040006EE RID: 1774
		amf_priority_die = 95UL,
		// Token: 0x040006EF RID: 1775
		amf_priority_mask = 255UL,
		// Token: 0x040006F0 RID: 1776
		anf_disable_agent_agent_collisions = 256UL,
		// Token: 0x040006F1 RID: 1777
		anf_ignore_all_collisions = 512UL,
		// Token: 0x040006F2 RID: 1778
		anf_ignore_static_body_collisions = 1024UL,
		// Token: 0x040006F3 RID: 1779
		anf_use_last_step_point_as_data = 2048UL,
		// Token: 0x040006F4 RID: 1780
		anf_make_bodyfall_sound = 4096UL,
		// Token: 0x040006F5 RID: 1781
		anf_client_prediction = 8192UL,
		// Token: 0x040006F6 RID: 1782
		anf_keep = 16384UL,
		// Token: 0x040006F7 RID: 1783
		anf_restart = 32768UL,
		// Token: 0x040006F8 RID: 1784
		anf_client_owner_prediction = 65536UL,
		// Token: 0x040006F9 RID: 1785
		anf_make_walk_sound = 131072UL,
		// Token: 0x040006FA RID: 1786
		anf_disable_hand_ik = 262144UL,
		// Token: 0x040006FB RID: 1787
		anf_stick_item_to_left_hand = 524288UL,
		// Token: 0x040006FC RID: 1788
		anf_blends_according_to_look_slope = 1048576UL,
		// Token: 0x040006FD RID: 1789
		anf_synch_with_horse = 2097152UL,
		// Token: 0x040006FE RID: 1790
		anf_use_left_hand_during_attack = 4194304UL,
		// Token: 0x040006FF RID: 1791
		anf_lock_camera = 8388608UL,
		// Token: 0x04000700 RID: 1792
		anf_lock_movement = 16777216UL,
		// Token: 0x04000701 RID: 1793
		anf_synch_with_movement = 33554432UL,
		// Token: 0x04000702 RID: 1794
		anf_enable_hand_spring_ik = 67108864UL,
		// Token: 0x04000703 RID: 1795
		anf_enable_hand_blend_ik = 134217728UL,
		// Token: 0x04000704 RID: 1796
		anf_synch_with_ladder_movement = 268435456UL,
		// Token: 0x04000705 RID: 1797
		anf_do_not_keep_track_of_sound = 536870912UL,
		// Token: 0x04000706 RID: 1798
		anf_reset_camera_height = 1073741824UL,
		// Token: 0x04000707 RID: 1799
		anf_disable_alternative_randomization = 2147483648UL,
		// Token: 0x04000708 RID: 1800
		anf_disable_auto_increment_progress = 4294967296UL,
		// Token: 0x04000709 RID: 1801
		anf_switch_item_between_hands = 8589934592UL,
		// Token: 0x0400070A RID: 1802
		anf_enforce_lowerbody = 68719476736UL,
		// Token: 0x0400070B RID: 1803
		anf_enforce_all = 137438953472UL,
		// Token: 0x0400070C RID: 1804
		anf_cyclic = 274877906944UL,
		// Token: 0x0400070D RID: 1805
		anf_enforce_root_rotation = 549755813888UL,
		// Token: 0x0400070E RID: 1806
		anf_allow_head_movement = 1099511627776UL,
		// Token: 0x0400070F RID: 1807
		anf_disable_foot_ik = 2199023255552UL,
		// Token: 0x04000710 RID: 1808
		anf_affected_by_movement = 4398046511104UL,
		// Token: 0x04000711 RID: 1809
		anf_update_bounding_volume = 8796093022208UL,
		// Token: 0x04000712 RID: 1810
		anf_align_with_ground = 17592186044416UL,
		// Token: 0x04000713 RID: 1811
		anf_ignore_slope = 35184372088832UL,
		// Token: 0x04000714 RID: 1812
		anf_displace_position = 70368744177664UL,
		// Token: 0x04000715 RID: 1813
		anf_enable_left_hand_ik = 140737488355328UL,
		// Token: 0x04000716 RID: 1814
		anf_ignore_scale_on_root_position = 281474976710656UL,
		// Token: 0x04000717 RID: 1815
		anf_blend_main_item_bone_entitially = 562949953421312UL,
		// Token: 0x04000718 RID: 1816
		anf_animation_layer_flags_mask = 4503530907893760UL,
		// Token: 0x04000719 RID: 1817
		anf_animation_layer_flags_bits = 36UL,
		// Token: 0x0400071A RID: 1818
		anf_randomization_weight_1 = 1152921504606846976UL,
		// Token: 0x0400071B RID: 1819
		anf_randomization_weight_2 = 2305843009213693952UL,
		// Token: 0x0400071C RID: 1820
		anf_randomization_weight_4 = 4611686018427387904UL,
		// Token: 0x0400071D RID: 1821
		anf_randomization_weight_8 = 9223372036854775808UL,
		// Token: 0x0400071E RID: 1822
		anf_randomization_weight_mask = 17293822569102704640UL
	}
}
