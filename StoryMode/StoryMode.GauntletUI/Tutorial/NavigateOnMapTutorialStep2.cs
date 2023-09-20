using System;
using StoryMode.ViewModelCollection.Tutorial;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;

namespace StoryMode.GauntletUI.Tutorial
{
	public class NavigateOnMapTutorialStep2 : TutorialItemBase
	{
		public NavigateOnMapTutorialStep2()
		{
			base.Type = "NavigateOnMapTutorialStep2";
			base.Placement = TutorialItemVM.ItemPlacements.TopRight;
			base.HighlightedVisualElementID = "village_ES3_2";
			base.MouseRequired = true;
		}

		public override TutorialContexts GetTutorialsRelevantContext()
		{
			return 4;
		}

		public override bool IsConditionsMetForActivation()
		{
			return TutorialHelper.CurrentContext == 4;
		}

		public override bool IsConditionsMetForCompletion()
		{
			MobileParty mainParty = MobileParty.MainParty;
			string text;
			if (mainParty == null)
			{
				text = null;
			}
			else
			{
				Settlement targetSettlement = mainParty.TargetSettlement;
				text = ((targetSettlement != null) ? targetSettlement.StringId : null);
			}
			return text == "village_ES3_2";
		}

		private const string TargetQuestVillage = "village_ES3_2";
	}
}
