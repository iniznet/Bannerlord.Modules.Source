using System;
using StoryMode.ViewModelCollection.Tutorial;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;

namespace StoryMode.GauntletUI.Tutorial
{
	public class TalkToNotableTutorialStep1 : TutorialItemBase
	{
		public TalkToNotableTutorialStep1()
		{
			base.Type = "TalkToNotableTutorialStep1";
			base.Placement = TutorialItemVM.ItemPlacements.TopRight;
			base.HighlightedVisualElementID = "ApplicableNotable";
			base.MouseRequired = true;
		}

		public override TutorialContexts GetTutorialsRelevantContext()
		{
			return 4;
		}

		public override bool IsConditionsMetForActivation()
		{
			return !TutorialHelper.IsCharacterPopUpWindowOpen && TutorialHelper.CurrentContext == 4 && TutorialHelper.VillageMenuIsOpen;
		}

		public override bool IsConditionsMetForCompletion()
		{
			return this._wantedCharacterPopupOpened;
		}

		public override void OnCharacterPortraitPopUpOpened(CharacterObject obj)
		{
			this._wantedCharacterPopupOpened = obj != null && obj.IsHero && obj.HeroObject.IsNotable;
		}

		private bool _wantedCharacterPopupOpened;
	}
}
