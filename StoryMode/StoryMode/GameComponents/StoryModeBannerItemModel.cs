using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem.GameComponents;
using TaleWorlds.Core;
using TaleWorlds.LinQuick;

namespace StoryMode.GameComponents
{
	// Token: 0x0200003B RID: 59
	public class StoryModeBannerItemModel : DefaultBannerItemModel
	{
		// Token: 0x060003AB RID: 939 RVA: 0x00017136 File Offset: 0x00015336
		public override IEnumerable<ItemObject> GetPossibleRewardBannerItems()
		{
			if (!StoryModeManager.Current.MainStoryLine.TutorialPhase.IsCompleted)
			{
				return new List<ItemObject>();
			}
			return LinQuick.WhereQ<ItemObject>(base.GetPossibleRewardBannerItems(), (ItemObject i) => !this.IsItemDragonBanner(i));
		}

		// Token: 0x060003AC RID: 940 RVA: 0x0001716B File Offset: 0x0001536B
		public override bool CanBannerBeUpdated(ItemObject item)
		{
			return !this.IsItemDragonBanner(item) && base.CanBannerBeUpdated(item);
		}

		// Token: 0x060003AD RID: 941 RVA: 0x00017180 File Offset: 0x00015380
		private bool IsItemDragonBanner(ItemObject item)
		{
			return item.StringId == "dragon_banner" || item.StringId == "dragon_banner_center" || item.StringId == "dragon_banner_dragonhead" || item.StringId == "dragon_banner_handle";
		}
	}
}
