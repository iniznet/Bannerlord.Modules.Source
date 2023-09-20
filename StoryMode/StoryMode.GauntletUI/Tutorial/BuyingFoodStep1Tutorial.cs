using System;
using StoryMode.ViewModelCollection.Tutorial;
using TaleWorlds.Core;

namespace StoryMode.GauntletUI.Tutorial
{
	public class BuyingFoodStep1Tutorial : TutorialItemBase
	{
		public BuyingFoodStep1Tutorial()
		{
			base.Type = "GetSuppliesTutorialStep1";
			base.Placement = TutorialItemVM.ItemPlacements.Right;
			base.HighlightedVisualElementID = "storymode_tutorial_village_buy";
			base.MouseRequired = true;
		}

		public override bool IsConditionsMetForCompletion()
		{
			return this._contextChangedToInventory;
		}

		public override bool IsConditionsMetForActivation()
		{
			return !TutorialHelper.IsCharacterPopUpWindowOpen && TutorialHelper.BuyingFoodBaseConditions && TutorialHelper.CurrentContext == 4;
		}

		public override void OnTutorialContextChanged(TutorialContextChangedEvent obj)
		{
			this._contextChangedToInventory = obj.NewContext == 2;
		}

		public override TutorialContexts GetTutorialsRelevantContext()
		{
			return 4;
		}

		private bool _contextChangedToInventory;
	}
}
