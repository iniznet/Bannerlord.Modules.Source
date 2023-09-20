using System;
using StoryMode.ViewModelCollection.Tutorial;
using TaleWorlds.CampaignSystem.ViewModelCollection.Encyclopedia;
using TaleWorlds.Core;

namespace StoryMode.GauntletUI.Tutorial
{
	public class EncyclopediaPageTutorial : TutorialItemBase
	{
		public EncyclopediaPageTutorial(string type, EncyclopediaPages activationPage, EncyclopediaPages alternateActivationPage)
		{
			base.Type = type;
			base.Placement = TutorialItemVM.ItemPlacements.Right;
			base.HighlightedVisualElementID = "";
			base.MouseRequired = false;
			this._activationPage = activationPage;
			this._alternateActivationPage = alternateActivationPage;
		}

		public override TutorialContexts GetTutorialsRelevantContext()
		{
			return 9;
		}

		public override bool IsConditionsMetForActivation()
		{
			EncyclopediaPages currentEncyclopediaPageContext = GauntletTutorialSystem.Current.CurrentEncyclopediaPageContext;
			bool isActive = this._isActive;
			this._isActive = currentEncyclopediaPageContext == this._activationPage || currentEncyclopediaPageContext == this._alternateActivationPage;
			if (!isActive && this._isActive)
			{
				this._lastActivatedPage = currentEncyclopediaPageContext;
			}
			return this._isActive;
		}

		public override bool IsConditionsMetForCompletion()
		{
			if (!this._isActive)
			{
				return false;
			}
			EncyclopediaPages currentEncyclopediaPageContext = GauntletTutorialSystem.Current.CurrentEncyclopediaPageContext;
			if (this._lastActivatedPage == this._alternateActivationPage)
			{
				return currentEncyclopediaPageContext != this._alternateActivationPage;
			}
			return currentEncyclopediaPageContext != 9 && currentEncyclopediaPageContext != 2;
		}

		private bool _isActive;

		private readonly EncyclopediaPages _activationPage;

		private readonly EncyclopediaPages _alternateActivationPage;

		private EncyclopediaPages _lastActivatedPage;
	}
}
