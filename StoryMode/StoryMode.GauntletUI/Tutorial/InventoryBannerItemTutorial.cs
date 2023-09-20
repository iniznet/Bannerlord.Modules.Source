using System;
using StoryMode.ViewModelCollection.Tutorial;
using TaleWorlds.CampaignSystem.ViewModelCollection.Inventory;
using TaleWorlds.Core;

namespace StoryMode.GauntletUI.Tutorial
{
	// Token: 0x02000038 RID: 56
	public class InventoryBannerItemTutorial : TutorialItemBase
	{
		// Token: 0x06000119 RID: 281 RVA: 0x0000446C File Offset: 0x0000266C
		public InventoryBannerItemTutorial()
		{
			base.Type = "InventoryBannerItemTutorial";
			base.Placement = TutorialItemVM.ItemPlacements.Center;
			base.HighlightedVisualElementID = "InventoryOtherBannerItems";
			base.MouseRequired = false;
		}

		// Token: 0x0600011A RID: 282 RVA: 0x00004498 File Offset: 0x00002698
		public override TutorialContexts GetTutorialsRelevantContext()
		{
			return 2;
		}

		// Token: 0x0600011B RID: 283 RVA: 0x0000449C File Offset: 0x0000269C
		public override void OnInventoryItemInspected(InventoryItemInspectedEvent obj)
		{
			if (obj.Item.EquipmentElement.Item.IsBannerItem && obj.ItemSide == null)
			{
				this._inspectedOtherBannerItem = true;
			}
		}

		// Token: 0x0600011C RID: 284 RVA: 0x000044D5 File Offset: 0x000026D5
		public override bool IsConditionsMetForActivation()
		{
			return TutorialHelper.CurrentInventoryScreenIncludesBannerItem;
		}

		// Token: 0x0600011D RID: 285 RVA: 0x000044DC File Offset: 0x000026DC
		public override bool IsConditionsMetForCompletion()
		{
			return this._inspectedOtherBannerItem;
		}

		// Token: 0x04000057 RID: 87
		private bool _inspectedOtherBannerItem;
	}
}
