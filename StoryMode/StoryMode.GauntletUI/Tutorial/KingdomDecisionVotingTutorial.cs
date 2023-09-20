using System;
using StoryMode.ViewModelCollection.Tutorial;
using TaleWorlds.CampaignSystem.ViewModelCollection.KingdomManagement.Decisions;
using TaleWorlds.Core;

namespace StoryMode.GauntletUI.Tutorial
{
	public class KingdomDecisionVotingTutorial : TutorialItemBase
	{
		public KingdomDecisionVotingTutorial()
		{
			base.Type = "KingdomDecisionVotingTutorial";
			base.Placement = TutorialItemVM.ItemPlacements.Left;
			base.HighlightedVisualElementID = "DecisionOptions";
			base.MouseRequired = false;
		}

		public override TutorialContexts GetTutorialsRelevantContext()
		{
			return 7;
		}

		public override void OnPlayerSelectedAKingdomDecisionOption(PlayerSelectedAKingdomDecisionOptionEvent obj)
		{
			this._playerSelectedAnOption = true;
		}

		public override bool IsConditionsMetForActivation()
		{
			return TutorialHelper.IsKingdomDecisionPanelActiveAndHasOptions;
		}

		public override bool IsConditionsMetForCompletion()
		{
			return this._playerSelectedAnOption;
		}

		private bool _playerSelectedAnOption;
	}
}
