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
			bool flag;
			if (obj == null)
			{
				flag = false;
			}
			else
			{
				Hero heroObject = obj.HeroObject;
				bool? flag2 = ((heroObject != null) ? new bool?(heroObject.IsHeadman) : null);
				bool flag3 = true;
				flag = (flag2.GetValueOrDefault() == flag3) & (flag2 != null);
			}
			this._wantedCharacterPopupOpened = flag;
		}

		private bool _wantedCharacterPopupOpened;
	}
}
