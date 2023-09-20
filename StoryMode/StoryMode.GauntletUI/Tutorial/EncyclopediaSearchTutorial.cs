using System;
using StoryMode.ViewModelCollection.Tutorial;
using TaleWorlds.CampaignSystem.ViewModelCollection.Encyclopedia;
using TaleWorlds.Core;

namespace StoryMode.GauntletUI.Tutorial
{
	// Token: 0x0200002F RID: 47
	public class EncyclopediaSearchTutorial : TutorialItemBase
	{
		// Token: 0x060000E6 RID: 230 RVA: 0x00003E02 File Offset: 0x00002002
		public EncyclopediaSearchTutorial()
		{
			base.Type = "EncyclopediaSearchTutorial";
			base.Placement = TutorialItemVM.ItemPlacements.Right;
			base.HighlightedVisualElementID = "EncyclopediaSearchButton";
			base.MouseRequired = false;
		}

		// Token: 0x060000E7 RID: 231 RVA: 0x00003E2E File Offset: 0x0000202E
		public override TutorialContexts GetTutorialsRelevantContext()
		{
			return 9;
		}

		// Token: 0x060000E8 RID: 232 RVA: 0x00003E34 File Offset: 0x00002034
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

		// Token: 0x060000E9 RID: 233 RVA: 0x00003EAD File Offset: 0x000020AD
		private void OnEncyclopediaSearchBarUsed(OnEncyclopediaSearchActivatedEvent evnt)
		{
			this._isSearchButtonPressed = true;
		}

		// Token: 0x060000EA RID: 234 RVA: 0x00003EB6 File Offset: 0x000020B6
		public override bool IsConditionsMetForCompletion()
		{
			if (this._isActive && this._isSearchButtonPressed)
			{
				Game.Current.EventManager.UnregisterEvent<OnEncyclopediaSearchActivatedEvent>(new Action<OnEncyclopediaSearchActivatedEvent>(this.OnEncyclopediaSearchBarUsed));
				return true;
			}
			return false;
		}

		// Token: 0x04000043 RID: 67
		private bool _isActive;

		// Token: 0x04000044 RID: 68
		private bool _isSearchButtonPressed;
	}
}
