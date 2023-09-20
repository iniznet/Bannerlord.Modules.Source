using System;
using StoryMode.ViewModelCollection.Tutorial;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.ViewModelCollection.GameMenu.Overlay;
using TaleWorlds.Core;

namespace StoryMode.GauntletUI.Tutorial
{
	public class CrimeTutorial : TutorialItemBase
	{
		public CrimeTutorial()
		{
			base.Type = "CrimeTutorial";
			base.Placement = TutorialItemVM.ItemPlacements.Top;
			base.HighlightedVisualElementID = "CrimeLabel";
			base.MouseRequired = false;
		}

		public override TutorialContexts GetTutorialsRelevantContext()
		{
			return 4;
		}

		public override void OnCrimeValueInspectedInSettlementOverlay(SettlementMenuOverlayVM.CrimeValueInspectedInSettlementOverlayEvent obj)
		{
			this._inspectedCrimeValueItem = true;
		}

		public override bool IsConditionsMetForActivation()
		{
			if (TutorialHelper.TownMenuIsOpen)
			{
				IFaction mapFaction = Settlement.CurrentSettlement.MapFaction;
				return mapFaction != null && mapFaction.MainHeroCrimeRating > 0f;
			}
			return false;
		}

		public override bool IsConditionsMetForCompletion()
		{
			return this._inspectedCrimeValueItem;
		}

		private bool _inspectedCrimeValueItem;
	}
}
