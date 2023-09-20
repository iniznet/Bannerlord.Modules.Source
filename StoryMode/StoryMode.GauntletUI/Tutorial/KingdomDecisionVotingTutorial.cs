using System;
using StoryMode.ViewModelCollection.Tutorial;
using TaleWorlds.CampaignSystem.ViewModelCollection.KingdomManagement.Decisions;
using TaleWorlds.Core;

namespace StoryMode.GauntletUI.Tutorial
{
	// Token: 0x0200003B RID: 59
	public class KingdomDecisionVotingTutorial : TutorialItemBase
	{
		// Token: 0x06000128 RID: 296 RVA: 0x00004592 File Offset: 0x00002792
		public KingdomDecisionVotingTutorial()
		{
			base.Type = "KingdomDecisionVotingTutorial";
			base.Placement = TutorialItemVM.ItemPlacements.Left;
			base.HighlightedVisualElementID = "DecisionOptions";
			base.MouseRequired = false;
		}

		// Token: 0x06000129 RID: 297 RVA: 0x000045BE File Offset: 0x000027BE
		public override TutorialContexts GetTutorialsRelevantContext()
		{
			return 7;
		}

		// Token: 0x0600012A RID: 298 RVA: 0x000045C1 File Offset: 0x000027C1
		public override void OnPlayerSelectedAKingdomDecisionOption(PlayerSelectedAKingdomDecisionOptionEvent obj)
		{
			this._playerSelectedAnOption = true;
		}

		// Token: 0x0600012B RID: 299 RVA: 0x000045CA File Offset: 0x000027CA
		public override bool IsConditionsMetForActivation()
		{
			return TutorialHelper.IsKingdomDecisionPanelActiveAndHasOptions;
		}

		// Token: 0x0600012C RID: 300 RVA: 0x000045D1 File Offset: 0x000027D1
		public override bool IsConditionsMetForCompletion()
		{
			return this._playerSelectedAnOption;
		}

		// Token: 0x0400005A RID: 90
		private bool _playerSelectedAnOption;
	}
}
