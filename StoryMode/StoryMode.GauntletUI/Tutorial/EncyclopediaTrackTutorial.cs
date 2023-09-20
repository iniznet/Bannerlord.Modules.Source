using System;
using StoryMode.ViewModelCollection.Tutorial;
using TaleWorlds.CampaignSystem.ViewModelCollection.Encyclopedia;
using TaleWorlds.Core;

namespace StoryMode.GauntletUI.Tutorial
{
	public class EncyclopediaTrackTutorial : TutorialItemBase
	{
		public EncyclopediaTrackTutorial()
		{
			base.Type = "EncyclopediaTrackTutorial";
			base.Placement = TutorialItemVM.ItemPlacements.Right;
			base.HighlightedVisualElementID = "EncyclopediaItemTrackButton";
			base.MouseRequired = false;
		}

		public override TutorialContexts GetTutorialsRelevantContext()
		{
			return 9;
		}

		public override bool IsConditionsMetForActivation()
		{
			bool isActive = this._isActive;
			this._isActive = TutorialHelper.CurrentEncyclopediaPage == 9;
			if (!isActive && this._isActive)
			{
				Game.Current.EventManager.RegisterEvent<PlayerToggleTrackSettlementFromEncyclopediaEvent>(new Action<PlayerToggleTrackSettlementFromEncyclopediaEvent>(this.OnTrackToggledFromEncyclopedia));
			}
			else if (!this._isActive && isActive)
			{
				Game.Current.EventManager.UnregisterEvent<PlayerToggleTrackSettlementFromEncyclopediaEvent>(new Action<PlayerToggleTrackSettlementFromEncyclopediaEvent>(this.OnTrackToggledFromEncyclopedia));
			}
			return this._isActive;
		}

		public override bool IsConditionsMetForCompletion()
		{
			if (this._isActive)
			{
				bool flag = false;
				if (this._isActive)
				{
					if (TutorialHelper.CurrentContext != 9)
					{
						flag = true;
					}
					if (TutorialHelper.CurrentEncyclopediaPage != 12 && TutorialHelper.CurrentEncyclopediaPage != 9)
					{
						flag = true;
					}
					if (this._usedTrackFromEncyclopedia)
					{
						flag = true;
					}
				}
				if (flag)
				{
					Game.Current.EventManager.UnregisterEvent<PlayerToggleTrackSettlementFromEncyclopediaEvent>(new Action<PlayerToggleTrackSettlementFromEncyclopediaEvent>(this.OnTrackToggledFromEncyclopedia));
					return true;
				}
			}
			return false;
		}

		private void OnTrackToggledFromEncyclopedia(PlayerToggleTrackSettlementFromEncyclopediaEvent callback)
		{
			this._usedTrackFromEncyclopedia = true;
		}

		private bool _isActive;

		private bool _usedTrackFromEncyclopedia;
	}
}
