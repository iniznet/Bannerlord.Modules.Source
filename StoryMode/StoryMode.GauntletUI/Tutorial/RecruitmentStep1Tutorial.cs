using System;
using StoryMode.ViewModelCollection.Tutorial;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;

namespace StoryMode.GauntletUI.Tutorial
{
	public class RecruitmentStep1Tutorial : TutorialItemBase
	{
		public RecruitmentStep1Tutorial()
		{
			base.Type = "RecruitmentTutorialStep1";
			base.Placement = TutorialItemVM.ItemPlacements.Right;
			base.HighlightedVisualElementID = "storymode_tutorial_village_recruit";
			base.MouseRequired = true;
		}

		public override bool IsConditionsMetForCompletion()
		{
			return this._recruitmentOpened;
		}

		public override void OnTutorialContextChanged(TutorialContextChangedEvent obj)
		{
			this._recruitmentOpened = obj.NewContext == 5;
		}

		public override TutorialContexts GetTutorialsRelevantContext()
		{
			return 4;
		}

		public override bool IsConditionsMetForActivation()
		{
			return !TutorialHelper.IsCharacterPopUpWindowOpen && TutorialHelper.CurrentContext == 4 && TutorialHelper.PlayerCanRecruit && !Settlement.CurrentSettlement.MapFaction.IsAtWarWith(MobileParty.MainParty.MapFaction);
		}

		private bool _recruitmentOpened;
	}
}
