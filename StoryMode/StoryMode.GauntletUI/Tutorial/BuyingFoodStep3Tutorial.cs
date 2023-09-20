using System;
using StoryMode.ViewModelCollection.Tutorial;
using TaleWorlds.CampaignSystem.Inventory;
using TaleWorlds.Core;

namespace StoryMode.GauntletUI.Tutorial
{
	public class BuyingFoodStep3Tutorial : TutorialItemBase
	{
		public BuyingFoodStep3Tutorial()
		{
			base.Type = "GetSuppliesTutorialStep3";
			base.Placement = TutorialItemVM.ItemPlacements.Right;
			base.HighlightedVisualElementID = "TransferButtonOnlyFood";
			base.MouseRequired = true;
		}

		public override bool IsConditionsMetForCompletion()
		{
			return this._foodItemTransfered;
		}

		public override bool IsConditionsMetForActivation()
		{
			return TutorialHelper.BuyingFoodBaseConditions && TutorialHelper.CurrentContext == 2;
		}

		public override TutorialContexts GetTutorialsRelevantContext()
		{
			return 2;
		}

		public override void OnInventoryTransferItem(InventoryTransferItemEvent obj)
		{
			this._foodItemTransfered = obj.IsBuyForPlayer && obj.Item.IsFood;
		}

		private bool _foodItemTransfered;
	}
}
