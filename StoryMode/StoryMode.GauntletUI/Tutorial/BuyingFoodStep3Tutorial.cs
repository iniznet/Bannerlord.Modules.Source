using System;
using StoryMode.ViewModelCollection.Tutorial;
using TaleWorlds.CampaignSystem.Inventory;
using TaleWorlds.Core;

namespace StoryMode.GauntletUI.Tutorial
{
	// Token: 0x02000008 RID: 8
	public class BuyingFoodStep3Tutorial : TutorialItemBase
	{
		// Token: 0x06000020 RID: 32 RVA: 0x0000234B File Offset: 0x0000054B
		public BuyingFoodStep3Tutorial()
		{
			base.Type = "GetSuppliesTutorialStep3";
			base.Placement = TutorialItemVM.ItemPlacements.Right;
			base.HighlightedVisualElementID = "TransferButtonOnlyFood";
			base.MouseRequired = true;
		}

		// Token: 0x06000021 RID: 33 RVA: 0x00002377 File Offset: 0x00000577
		public override bool IsConditionsMetForCompletion()
		{
			return this._foodItemTransfered;
		}

		// Token: 0x06000022 RID: 34 RVA: 0x0000237F File Offset: 0x0000057F
		public override bool IsConditionsMetForActivation()
		{
			return TutorialHelper.BuyingFoodBaseConditions && TutorialHelper.CurrentContext == 2;
		}

		// Token: 0x06000023 RID: 35 RVA: 0x00002392 File Offset: 0x00000592
		public override TutorialContexts GetTutorialsRelevantContext()
		{
			return 2;
		}

		// Token: 0x06000024 RID: 36 RVA: 0x00002395 File Offset: 0x00000595
		public override void OnInventoryTransferItem(InventoryTransferItemEvent obj)
		{
			this._foodItemTransfered = obj.IsBuyForPlayer && obj.Item.IsFood;
		}

		// Token: 0x04000009 RID: 9
		private bool _foodItemTransfered;
	}
}
