using System;
using StoryMode.ViewModelCollection.Tutorial;
using TaleWorlds.Core;

namespace StoryMode.GauntletUI.Tutorial
{
	// Token: 0x0200002C RID: 44
	public class EncyclopediaHomeTutorial : TutorialItemBase
	{
		// Token: 0x060000D9 RID: 217 RVA: 0x00003BA4 File Offset: 0x00001DA4
		public EncyclopediaHomeTutorial()
		{
			base.Type = "EncyclopediaHomeTutorial";
			base.Placement = TutorialItemVM.ItemPlacements.Right;
			base.HighlightedVisualElementID = "";
			base.MouseRequired = false;
		}

		// Token: 0x060000DA RID: 218 RVA: 0x00003BD0 File Offset: 0x00001DD0
		public override TutorialContexts GetTutorialsRelevantContext()
		{
			return 9;
		}

		// Token: 0x060000DB RID: 219 RVA: 0x00003BD4 File Offset: 0x00001DD4
		public override bool IsConditionsMetForActivation()
		{
			this._isActive = GauntletTutorialSystem.Current.CurrentEncyclopediaPageContext == 1;
			return this._isActive;
		}

		// Token: 0x060000DC RID: 220 RVA: 0x00003BEF File Offset: 0x00001DEF
		public override bool IsConditionsMetForCompletion()
		{
			return this._isActive && GauntletTutorialSystem.Current.CurrentEncyclopediaPageContext != 1;
		}

		// Token: 0x0400003C RID: 60
		private bool _isActive;
	}
}
