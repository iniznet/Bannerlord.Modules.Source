using System;
using StoryMode.ViewModelCollection.Tutorial;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ViewModelCollection.CharacterDeveloper;
using TaleWorlds.Core;

namespace StoryMode.GauntletUI.Tutorial
{
	// Token: 0x0200000D RID: 13
	public class ChoosingSkillFocusStep2Tutorial : TutorialItemBase
	{
		// Token: 0x06000039 RID: 57 RVA: 0x00002589 File Offset: 0x00000789
		public ChoosingSkillFocusStep2Tutorial()
		{
			base.Type = "ChoosingSkillFocusStep2";
			base.Placement = TutorialItemVM.ItemPlacements.TopRight;
			base.HighlightedVisualElementID = "AddFocusButton";
			base.MouseRequired = true;
		}

		// Token: 0x0600003A RID: 58 RVA: 0x000025B5 File Offset: 0x000007B5
		public override bool IsConditionsMetForCompletion()
		{
			return this._focusAdded;
		}

		// Token: 0x0600003B RID: 59 RVA: 0x000025BD File Offset: 0x000007BD
		public override void OnFocusAddedByPlayer(FocusAddedByPlayerEvent obj)
		{
			this._focusAdded = true;
		}

		// Token: 0x0600003C RID: 60 RVA: 0x000025C6 File Offset: 0x000007C6
		public override TutorialContexts GetTutorialsRelevantContext()
		{
			return 3;
		}

		// Token: 0x0600003D RID: 61 RVA: 0x000025C9 File Offset: 0x000007C9
		public override bool IsConditionsMetForActivation()
		{
			return Hero.MainHero.HeroDeveloper.UnspentFocusPoints > 1 && TutorialHelper.CurrentContext == 3;
		}

		// Token: 0x0400000E RID: 14
		private bool _focusAdded;
	}
}
