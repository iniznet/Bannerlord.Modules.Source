using System;
using StoryMode.ViewModelCollection.Tutorial;
using TaleWorlds.CampaignSystem.ViewModelCollection.Encyclopedia;
using TaleWorlds.Core;

namespace StoryMode.GauntletUI.Tutorial
{
	// Token: 0x02000032 RID: 50
	public class EncyclopediaFogOfWarTutorial : TutorialItemBase
	{
		// Token: 0x060000F5 RID: 245 RVA: 0x000040C7 File Offset: 0x000022C7
		public EncyclopediaFogOfWarTutorial()
		{
			base.Type = "EncyclopediaFogOfWarTutorial";
			base.Placement = TutorialItemVM.ItemPlacements.Right;
			base.HighlightedVisualElementID = "";
			base.MouseRequired = false;
		}

		// Token: 0x060000F6 RID: 246 RVA: 0x000040F4 File Offset: 0x000022F4
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

		// Token: 0x060000F7 RID: 247 RVA: 0x0000416B File Offset: 0x0000236B
		public override void OnTutorialContextChanged(TutorialContextChangedEvent evnt)
		{
			base.OnTutorialContextChanged(evnt);
			if (this._registeredEvents && evnt.NewContext != 9)
			{
				Game.Current.EventManager.UnregisterEvent<EncyclopediaPageChangedEvent>(new Action<EncyclopediaPageChangedEvent>(this.OnLimitedInformationPageOpened));
				this._registeredEvents = false;
			}
		}

		// Token: 0x060000F8 RID: 248 RVA: 0x000041A8 File Offset: 0x000023A8
		public override bool IsConditionsMetForActivation()
		{
			if (!this._registeredEvents)
			{
				Game.Current.EventManager.RegisterEvent<EncyclopediaPageChangedEvent>(new Action<EncyclopediaPageChangedEvent>(this.OnLimitedInformationPageOpened));
				this._registeredEvents = true;
			}
			return this._isActive;
		}

		// Token: 0x060000F9 RID: 249 RVA: 0x000041DC File Offset: 0x000023DC
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

		// Token: 0x060000FA RID: 250 RVA: 0x0000424B File Offset: 0x0000244B
		private void OnLimitedInformationPageOpened(EncyclopediaPageChangedEvent evnt)
		{
			if (evnt.NewPageHasHiddenInformation)
			{
				this._isActive = true;
			}
		}

		// Token: 0x04000049 RID: 73
		private EncyclopediaPages _activatedPage;

		// Token: 0x0400004A RID: 74
		private bool _registeredEvents;

		// Token: 0x0400004B RID: 75
		private bool _lastActiveState;

		// Token: 0x0400004C RID: 76
		private bool _isActive;
	}
}
