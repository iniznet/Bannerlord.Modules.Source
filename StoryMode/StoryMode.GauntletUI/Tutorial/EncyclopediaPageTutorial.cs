using System;
using StoryMode.ViewModelCollection.Tutorial;
using TaleWorlds.CampaignSystem.ViewModelCollection.Encyclopedia;
using TaleWorlds.Core;

namespace StoryMode.GauntletUI.Tutorial
{
	// Token: 0x0200002D RID: 45
	public class EncyclopediaPageTutorial : TutorialItemBase
	{
		// Token: 0x060000DD RID: 221 RVA: 0x00003C0B File Offset: 0x00001E0B
		public EncyclopediaPageTutorial(string type, EncyclopediaPages activationPage, EncyclopediaPages alternateActivationPage)
		{
			base.Type = type;
			base.Placement = TutorialItemVM.ItemPlacements.Right;
			base.HighlightedVisualElementID = "";
			base.MouseRequired = false;
			this._activationPage = activationPage;
			this._alternateActivationPage = alternateActivationPage;
		}

		// Token: 0x060000DE RID: 222 RVA: 0x00003C41 File Offset: 0x00001E41
		public override TutorialContexts GetTutorialsRelevantContext()
		{
			return 9;
		}

		// Token: 0x060000DF RID: 223 RVA: 0x00003C48 File Offset: 0x00001E48
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

		// Token: 0x060000E0 RID: 224 RVA: 0x00003C98 File Offset: 0x00001E98
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

		// Token: 0x0400003D RID: 61
		private bool _isActive;

		// Token: 0x0400003E RID: 62
		private readonly EncyclopediaPages _activationPage;

		// Token: 0x0400003F RID: 63
		private readonly EncyclopediaPages _alternateActivationPage;

		// Token: 0x04000040 RID: 64
		private EncyclopediaPages _lastActivatedPage;
	}
}
