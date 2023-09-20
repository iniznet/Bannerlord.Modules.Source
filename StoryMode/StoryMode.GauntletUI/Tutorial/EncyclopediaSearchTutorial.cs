using System;
using StoryMode.ViewModelCollection.Tutorial;
using TaleWorlds.CampaignSystem.ViewModelCollection.Encyclopedia;
using TaleWorlds.Core;

namespace StoryMode.GauntletUI.Tutorial
{
	public class EncyclopediaSearchTutorial : TutorialItemBase
	{
		public EncyclopediaSearchTutorial()
		{
			base.Type = "EncyclopediaSearchTutorial";
			base.Placement = TutorialItemVM.ItemPlacements.Right;
			base.HighlightedVisualElementID = "EncyclopediaSearchButton";
			base.MouseRequired = false;
		}

		public override TutorialContexts GetTutorialsRelevantContext()
		{
			return 9;
		}

		public override bool IsConditionsMetForActivation()
		{
			bool isActive = this._isActive;
			this._isActive = TutorialHelper.CurrentContext == 9;
			if (!isActive && this._isActive)
			{
				Game.Current.EventManager.RegisterEvent<OnEncyclopediaSearchActivatedEvent>(new Action<OnEncyclopediaSearchActivatedEvent>(this.OnEncyclopediaSearchBarUsed));
			}
			else if (!this._isActive && isActive)
			{
				Game.Current.EventManager.UnregisterEvent<OnEncyclopediaSearchActivatedEvent>(new Action<OnEncyclopediaSearchActivatedEvent>(this.OnEncyclopediaSearchBarUsed));
			}
			return this._isActive;
		}

		private void OnEncyclopediaSearchBarUsed(OnEncyclopediaSearchActivatedEvent evnt)
		{
			this._isSearchButtonPressed = true;
		}

		public override bool IsConditionsMetForCompletion()
		{
			if (this._isActive && this._isSearchButtonPressed)
			{
				Game.Current.EventManager.UnregisterEvent<OnEncyclopediaSearchActivatedEvent>(new Action<OnEncyclopediaSearchActivatedEvent>(this.OnEncyclopediaSearchBarUsed));
				return true;
			}
			return false;
		}

		private bool _isActive;

		private bool _isSearchButtonPressed;
	}
}
