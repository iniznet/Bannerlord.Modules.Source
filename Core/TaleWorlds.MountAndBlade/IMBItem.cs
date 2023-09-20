using System;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000193 RID: 403
	[ScriptingInterfaceBase]
	internal interface IMBItem
	{
		// Token: 0x06001606 RID: 5638
		[EngineMethod("get_item_usage_index", false)]
		int GetItemUsageIndex(string itemusagename);

		// Token: 0x06001607 RID: 5639
		[EngineMethod("get_item_holster_index", false)]
		int GetItemHolsterIndex(string itemholstername);

		// Token: 0x06001608 RID: 5640
		[EngineMethod("get_item_is_passive_usage", false)]
		bool GetItemIsPassiveUsage(string itemUsageName);

		// Token: 0x06001609 RID: 5641
		[EngineMethod("get_holster_frame_by_index", false)]
		void GetHolsterFrameByIndex(int index, ref MatrixFrame outFrame);

		// Token: 0x0600160A RID: 5642
		[EngineMethod("get_item_usage_set_flags", false)]
		int GetItemUsageSetFlags(string ItemUsageName);

		// Token: 0x0600160B RID: 5643
		[EngineMethod("get_item_usage_reload_action_code", false)]
		int GetItemUsageReloadActionCode(string itemUsageName, int usageDirection, bool isMounted, int leftHandUsageSetIndex, bool isLeftStance);

		// Token: 0x0600160C RID: 5644
		[EngineMethod("get_item_usage_strike_type", false)]
		int GetItemUsageStrikeType(string itemUsageName, int usageDirection, bool isMounted, int leftHandUsageSetIndex, bool isLeftStance);

		// Token: 0x0600160D RID: 5645
		[EngineMethod("get_missile_range", false)]
		float GetMissileRange(float shot_speed, float z_diff);
	}
}
