using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem.GameComponents;
using TaleWorlds.Core;
using TaleWorlds.LinQuick;

namespace StoryMode.GameComponents
{
	public class StoryModeBannerItemModel : DefaultBannerItemModel
	{
		public override IEnumerable<ItemObject> GetPossibleRewardBannerItems()
		{
			if (!StoryModeManager.Current.MainStoryLine.TutorialPhase.IsCompleted)
			{
				return new List<ItemObject>();
			}
			return LinQuick.WhereQ<ItemObject>(base.GetPossibleRewardBannerItems(), (ItemObject i) => !this.IsItemDragonBanner(i));
		}

		public override bool CanBannerBeUpdated(ItemObject item)
		{
			return !this.IsItemDragonBanner(item) && base.CanBannerBeUpdated(item);
		}

		private bool IsItemDragonBanner(ItemObject item)
		{
			return item.StringId == "dragon_banner" || item.StringId == "dragon_banner_center" || item.StringId == "dragon_banner_dragonhead" || item.StringId == "dragon_banner_handle";
		}
	}
}
