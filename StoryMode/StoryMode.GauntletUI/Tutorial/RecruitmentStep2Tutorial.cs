using System;
using StoryMode.ViewModelCollection.Tutorial;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;

namespace StoryMode.GauntletUI.Tutorial
{
	public class RecruitmentStep2Tutorial : TutorialItemBase
	{
		public RecruitmentStep2Tutorial()
		{
			base.Type = "RecruitmentTutorialStep2";
			base.Placement = TutorialItemVM.ItemPlacements.Right;
			base.HighlightedVisualElementID = "AvailableTroops";
			base.MouseRequired = true;
		}

		public override bool IsConditionsMetForCompletion()
		{
			return this._playerRecruitedTroop;
		}

		public override void OnPlayerStartRecruitment(CharacterObject obj)
		{
			this._playerRecruitedTroop = true;
		}

		public override TutorialContexts GetTutorialsRelevantContext()
		{
			return 5;
		}

		public override bool IsConditionsMetForActivation()
		{
			return TutorialHelper.PlayerCanRecruit && TutorialHelper.CurrentContext == 5;
		}

		private bool _playerRecruitedTroop;
	}
}
