using System;
using StoryMode.ViewModelCollection.Tutorial;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;

namespace StoryMode.GauntletUI.Tutorial
{
	public class TalkToNotableTutorialStep2 : TutorialItemBase
	{
		public TalkToNotableTutorialStep2()
		{
			base.Type = "TalkToNotableTutorialStep2";
			base.Placement = TutorialItemVM.ItemPlacements.Right;
			base.HighlightedVisualElementID = "OverlayTalkButton";
			base.MouseRequired = true;
		}

		public override bool IsConditionsMetForCompletion()
		{
			return this._hasTalkedToNotable;
		}

		public override void OnPlayerStartTalkFromMenuOverlay(Hero hero)
		{
			this._hasTalkedToNotable = hero.IsHeadman;
		}

		public override TutorialContexts GetTutorialsRelevantContext()
		{
			return 4;
		}

		public override bool IsConditionsMetForActivation()
		{
			return TutorialHelper.CurrentContext == 4 && TutorialHelper.IsCharacterPopUpWindowOpen;
		}

		private bool _hasTalkedToNotable;
	}
}
