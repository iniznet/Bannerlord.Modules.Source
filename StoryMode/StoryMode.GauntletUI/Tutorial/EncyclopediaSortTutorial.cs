using System;
using StoryMode.ViewModelCollection.Tutorial;
using TaleWorlds.CampaignSystem.ViewModelCollection.Encyclopedia;
using TaleWorlds.Core;

namespace StoryMode.GauntletUI.Tutorial
{
	public class EncyclopediaSortTutorial : TutorialItemBase
	{
		public EncyclopediaSortTutorial()
		{
			base.Type = "EncyclopediaSortTutorial";
			base.Placement = TutorialItemVM.ItemPlacements.Right;
			base.HighlightedVisualElementID = "EncyclopediaSortButton";
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
				Game.Current.EventManager.RegisterEvent<OnEncyclopediaListSortedEvent>(new Action<OnEncyclopediaListSortedEvent>(this.OnSortClicked));
			}
			else if (!this._isActive && isActive)
			{
				Game.Current.EventManager.UnregisterEvent<OnEncyclopediaListSortedEvent>(new Action<OnEncyclopediaListSortedEvent>(this.OnSortClicked));
			}
			return this._isActive;
		}

		private void OnSortClicked(OnEncyclopediaListSortedEvent evnt)
		{
			this._isSortClicked = true;
		}

		public override bool IsConditionsMetForCompletion()
		{
			if (this._isActive && this._isSortClicked)
			{
				Game.Current.EventManager.UnregisterEvent<OnEncyclopediaListSortedEvent>(new Action<OnEncyclopediaListSortedEvent>(this.OnSortClicked));
				return true;
			}
			return false;
		}

		private bool _isActive;

		private bool _isSortClicked;
	}
}
