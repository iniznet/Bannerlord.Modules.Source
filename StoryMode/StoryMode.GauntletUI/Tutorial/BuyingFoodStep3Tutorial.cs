using System;
using System.Collections.Generic;
using StoryMode.ViewModelCollection.Tutorial;
using TaleWorlds.CampaignSystem;
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
			return this._purchasedFoodCount >= TutorialHelper.BuyGrainAmount;
		}

		public override bool IsConditionsMetForActivation()
		{
			return TutorialHelper.BuyingFoodBaseConditions && TutorialHelper.CurrentContext == 2;
		}

		public override void OnPlayerInventoryExchange(List<ValueTuple<ItemRosterElement, int>> purchasedItems, List<ValueTuple<ItemRosterElement, int>> soldItems, bool isTrading)
		{
			for (int i = 0; i < purchasedItems.Count; i++)
			{
				ValueTuple<ItemRosterElement, int> valueTuple = purchasedItems[i];
				if (valueTuple.Item1.EquipmentElement.Item == DefaultItems.Grain)
				{
					this._purchasedFoodCount += valueTuple.Item1.Amount;
				}
			}
		}

		public override TutorialContexts GetTutorialsRelevantContext()
		{
			return 2;
		}

		private int _purchasedFoodCount;
	}
}
