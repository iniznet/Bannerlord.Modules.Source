using System;
using StoryMode.ViewModelCollection.Tutorial;
using TaleWorlds.Core;

namespace StoryMode.GauntletUI.Tutorial
{
	public class EncyclopediaHomeTutorial : TutorialItemBase
	{
		public EncyclopediaHomeTutorial()
		{
			base.Type = "EncyclopediaHomeTutorial";
			base.Placement = TutorialItemVM.ItemPlacements.Right;
			base.HighlightedVisualElementID = "";
			base.MouseRequired = false;
		}

		public override TutorialContexts GetTutorialsRelevantContext()
		{
			return 9;
		}

		public override bool IsConditionsMetForActivation()
		{
			this._isActive = GauntletTutorialSystem.Current.CurrentEncyclopediaPageContext == 1;
			return this._isActive;
		}

		public override bool IsConditionsMetForCompletion()
		{
			return this._isActive && GauntletTutorialSystem.Current.CurrentEncyclopediaPageContext != 1;
		}

		private bool _isActive;
	}
}
