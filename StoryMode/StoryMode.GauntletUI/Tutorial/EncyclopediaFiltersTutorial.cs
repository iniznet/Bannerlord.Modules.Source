using System;
using StoryMode.ViewModelCollection.Tutorial;
using TaleWorlds.CampaignSystem.ViewModelCollection.Encyclopedia;
using TaleWorlds.Core;

namespace StoryMode.GauntletUI.Tutorial
{
	public class EncyclopediaFiltersTutorial : TutorialItemBase
	{
		public EncyclopediaFiltersTutorial()
		{
			base.Type = "EncyclopediaFiltersTutorial";
			base.Placement = TutorialItemVM.ItemPlacements.Right;
			base.HighlightedVisualElementID = "EncyclopediaFiltersContainer";
			base.MouseRequired = false;
		}

		public override TutorialContexts GetTutorialsRelevantContext()
		{
			return 9;
		}

		public override bool IsConditionsMetForActivation()
		{
			bool isActive = this._isActive;
			EncyclopediaPages currentEncyclopediaPage = TutorialHelper.CurrentEncyclopediaPage;
			if (currentEncyclopediaPage - 2 <= 5)
			{
				this._isActive = true;
			}
			else
			{
				this._isActive = false;
			}
			if (!isActive && this._isActive)
			{
				Game.Current.EventManager.RegisterEvent<OnEncyclopediaFilterActivatedEvent>(new Action<OnEncyclopediaFilterActivatedEvent>(this.OnFilterClicked));
			}
			else if (!this._isActive && isActive)
			{
				Game.Current.EventManager.UnregisterEvent<OnEncyclopediaFilterActivatedEvent>(new Action<OnEncyclopediaFilterActivatedEvent>(this.OnFilterClicked));
			}
			return this._isActive;
		}

		private void OnFilterClicked(OnEncyclopediaFilterActivatedEvent evnt)
		{
			this._isAnyFilterSelected = true;
		}

		public override bool IsConditionsMetForCompletion()
		{
			if (this._isActive && this._isAnyFilterSelected)
			{
				Game.Current.EventManager.UnregisterEvent<OnEncyclopediaFilterActivatedEvent>(new Action<OnEncyclopediaFilterActivatedEvent>(this.OnFilterClicked));
				return true;
			}
			return false;
		}

		private bool _isActive;

		private bool _isAnyFilterSelected;
	}
}
