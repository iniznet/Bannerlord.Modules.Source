using System;
using StoryMode.ViewModelCollection.Tutorial;
using TaleWorlds.CampaignSystem.ViewModelCollection.Inventory;
using TaleWorlds.Core;

namespace StoryMode.GauntletUI.Tutorial
{
	public class BuyingFoodStep2Tutorial : TutorialItemBase
	{
		public BuyingFoodStep2Tutorial()
		{
			base.Type = "GetSuppliesTutorialStep2";
			base.Placement = TutorialItemVM.ItemPlacements.Right;
			base.HighlightedVisualElementID = "InventoryMicsFilter";
			base.MouseRequired = true;
		}

		public override bool IsConditionsMetForCompletion()
		{
			return this._filterChangedToMisc;
		}

		public override bool IsConditionsMetForActivation()
		{
			return TutorialHelper.BuyingFoodBaseConditions && TutorialHelper.CurrentContext == 2;
		}

		public override void OnInventoryFilterChanged(InventoryFilterChangedEvent obj)
		{
			this._filterChangedToMisc = obj.NewFilter == 5;
		}

		public override TutorialContexts GetTutorialsRelevantContext()
		{
			return 2;
		}

		private bool _filterChangedToMisc;
	}
}
