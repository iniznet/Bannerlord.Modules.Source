using System;
using StoryMode.ViewModelCollection.Tutorial;
using TaleWorlds.CampaignSystem.ViewModelCollection.Inventory;
using TaleWorlds.Core;

namespace StoryMode.GauntletUI.Tutorial
{
	public class InventoryBannerItemTutorial : TutorialItemBase
	{
		public InventoryBannerItemTutorial()
		{
			base.Type = "InventoryBannerItemTutorial";
			base.Placement = TutorialItemVM.ItemPlacements.Center;
			base.HighlightedVisualElementID = "InventoryOtherBannerItems";
			base.MouseRequired = false;
		}

		public override TutorialContexts GetTutorialsRelevantContext()
		{
			return 2;
		}

		public override void OnInventoryItemInspected(InventoryItemInspectedEvent obj)
		{
			if (obj.Item.EquipmentElement.Item.IsBannerItem && obj.ItemSide == null)
			{
				this._inspectedOtherBannerItem = true;
			}
		}

		public override bool IsConditionsMetForActivation()
		{
			return TutorialHelper.CurrentInventoryScreenIncludesBannerItem;
		}

		public override bool IsConditionsMetForCompletion()
		{
			return this._inspectedOtherBannerItem;
		}

		private bool _inspectedOtherBannerItem;
	}
}
