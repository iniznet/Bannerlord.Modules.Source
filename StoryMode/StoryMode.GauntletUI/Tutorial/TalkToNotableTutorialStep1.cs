using System;
using StoryMode.ViewModelCollection.Tutorial;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;

namespace StoryMode.GauntletUI.Tutorial
{
	// Token: 0x02000027 RID: 39
	public class TalkToNotableTutorialStep1 : TutorialItemBase
	{
		// Token: 0x060000BD RID: 189 RVA: 0x000036EF File Offset: 0x000018EF
		public TalkToNotableTutorialStep1()
		{
			base.Type = "TalkToNotableTutorialStep1";
			base.Placement = TutorialItemVM.ItemPlacements.TopRight;
			base.HighlightedVisualElementID = "ApplicableNotable";
			base.MouseRequired = true;
		}

		// Token: 0x060000BE RID: 190 RVA: 0x0000371B File Offset: 0x0000191B
		public override TutorialContexts GetTutorialsRelevantContext()
		{
			return 4;
		}

		// Token: 0x060000BF RID: 191 RVA: 0x0000371E File Offset: 0x0000191E
		public override bool IsConditionsMetForActivation()
		{
			return !TutorialHelper.IsCharacterPopUpWindowOpen && TutorialHelper.CurrentContext == 4 && TutorialHelper.VillageMenuIsOpen;
		}

		// Token: 0x060000C0 RID: 192 RVA: 0x00003736 File Offset: 0x00001936
		public override bool IsConditionsMetForCompletion()
		{
			return this._wantedCharacterPopupOpened;
		}

		// Token: 0x060000C1 RID: 193 RVA: 0x0000373E File Offset: 0x0000193E
		public override void OnCharacterPortraitPopUpOpened(CharacterObject obj)
		{
			this._wantedCharacterPopupOpened = obj != null && obj.IsHero && obj.HeroObject.IsNotable;
		}

		// Token: 0x04000034 RID: 52
		private bool _wantedCharacterPopupOpened;
	}
}
