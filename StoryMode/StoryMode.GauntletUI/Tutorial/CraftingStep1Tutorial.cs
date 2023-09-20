using System;
using StoryMode.ViewModelCollection.Tutorial;
using TaleWorlds.CampaignSystem.ViewModelCollection.WeaponCrafting.WeaponDesign;
using TaleWorlds.Core;

namespace StoryMode.GauntletUI.Tutorial
{
	public class CraftingStep1Tutorial : TutorialItemBase
	{
		public CraftingStep1Tutorial()
		{
			base.Type = "CraftingStep1Tutorial";
			base.Placement = TutorialItemVM.ItemPlacements.Top;
			base.HighlightedVisualElementID = "FreeModeClassSelectionButton";
			base.MouseRequired = false;
		}

		public override TutorialContexts GetTutorialsRelevantContext()
		{
			return 14;
		}

		public override void OnCraftingWeaponClassSelectionOpened(CraftingWeaponClassSelectionOpenedEvent obj)
		{
			this._craftingCategorySelectionOpened = obj.IsOpen;
		}

		public override void OnCraftingOrderSelectionOpened(CraftingOrderSelectionOpenedEvent obj)
		{
			this._craftingOrderSelectionOpened = obj.IsOpen;
		}

		public override void OnCraftingOnWeaponResultPopupOpened(CraftingWeaponResultPopupToggledEvent obj)
		{
			this._craftingOrderResultOpened = obj.IsOpen;
		}

		public override bool IsConditionsMetForActivation()
		{
			return !this._craftingOrderSelectionOpened && !this._craftingOrderResultOpened;
		}

		public override bool IsConditionsMetForCompletion()
		{
			return this._craftingCategorySelectionOpened;
		}

		private bool _craftingCategorySelectionOpened;

		private bool _craftingOrderSelectionOpened;

		private bool _craftingOrderResultOpened;
	}
}
