using System;
using StoryMode.ViewModelCollection.Tutorial;
using TaleWorlds.CampaignSystem.ViewModelCollection.Encyclopedia;
using TaleWorlds.Core;

namespace StoryMode.GauntletUI.Tutorial
{
	public class EncyclopediaFogOfWarTutorial : TutorialItemBase
	{
		public EncyclopediaFogOfWarTutorial()
		{
			base.Type = "EncyclopediaFogOfWarTutorial";
			base.Placement = TutorialItemVM.ItemPlacements.Right;
			base.HighlightedVisualElementID = "";
			base.MouseRequired = false;
		}

		public override TutorialContexts GetTutorialsRelevantContext()
		{
			if (!this._registeredEvents && TutorialHelper.CurrentContext == 9)
			{
				Game.Current.EventManager.RegisterEvent<EncyclopediaPageChangedEvent>(new Action<EncyclopediaPageChangedEvent>(this.OnLimitedInformationPageOpened));
				this._registeredEvents = true;
			}
			else if (this._registeredEvents && TutorialHelper.CurrentContext != 9)
			{
				Game.Current.EventManager.UnregisterEvent<EncyclopediaPageChangedEvent>(new Action<EncyclopediaPageChangedEvent>(this.OnLimitedInformationPageOpened));
				this._registeredEvents = false;
			}
			return 9;
		}

		public override void OnTutorialContextChanged(TutorialContextChangedEvent evnt)
		{
			base.OnTutorialContextChanged(evnt);
			if (this._registeredEvents && evnt.NewContext != 9)
			{
				Game.Current.EventManager.UnregisterEvent<EncyclopediaPageChangedEvent>(new Action<EncyclopediaPageChangedEvent>(this.OnLimitedInformationPageOpened));
				this._registeredEvents = false;
			}
		}

		public override bool IsConditionsMetForActivation()
		{
			if (!this._registeredEvents)
			{
				Game.Current.EventManager.RegisterEvent<EncyclopediaPageChangedEvent>(new Action<EncyclopediaPageChangedEvent>(this.OnLimitedInformationPageOpened));
				this._registeredEvents = true;
			}
			return this._isActive;
		}

		public override bool IsConditionsMetForCompletion()
		{
			if (!this._lastActiveState && this._isActive)
			{
				this._activatedPage = TutorialHelper.CurrentEncyclopediaPage;
			}
			if (this._lastActiveState && this._isActive && this._activatedPage != TutorialHelper.CurrentEncyclopediaPage)
			{
				Game.Current.EventManager.UnregisterEvent<EncyclopediaPageChangedEvent>(new Action<EncyclopediaPageChangedEvent>(this.OnLimitedInformationPageOpened));
				return true;
			}
			this._lastActiveState = this._isActive;
			return false;
		}

		private void OnLimitedInformationPageOpened(EncyclopediaPageChangedEvent evnt)
		{
			if (evnt.NewPageHasHiddenInformation)
			{
				this._isActive = true;
			}
		}

		private EncyclopediaPages _activatedPage;

		private bool _registeredEvents;

		private bool _lastActiveState;

		private bool _isActive;
	}
}
