using System;
using StoryMode.ViewModelCollection.Tutorial;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;

namespace StoryMode.GauntletUI.Tutorial
{
	// Token: 0x02000028 RID: 40
	public class TalkToNotableTutorialStep2 : TutorialItemBase
	{
		// Token: 0x060000C2 RID: 194 RVA: 0x0000375F File Offset: 0x0000195F
		public TalkToNotableTutorialStep2()
		{
			base.Type = "TalkToNotableTutorialStep2";
			base.Placement = TutorialItemVM.ItemPlacements.Right;
			base.HighlightedVisualElementID = "OverlayTalkButton";
			base.MouseRequired = true;
		}

		// Token: 0x060000C3 RID: 195 RVA: 0x0000378B File Offset: 0x0000198B
		public override bool IsConditionsMetForCompletion()
		{
			return this._startedTalkingWithNotable;
		}

		// Token: 0x060000C4 RID: 196 RVA: 0x00003793 File Offset: 0x00001993
		public override void OnPlayerStartTalkFromMenuOverlay(Hero hero)
		{
			this._startedTalkingWithNotable = hero.IsNotable && !hero.IsPlayerCompanion;
		}

		// Token: 0x060000C5 RID: 197 RVA: 0x000037AF File Offset: 0x000019AF
		public override TutorialContexts GetTutorialsRelevantContext()
		{
			return 4;
		}

		// Token: 0x060000C6 RID: 198 RVA: 0x000037B2 File Offset: 0x000019B2
		public override bool IsConditionsMetForActivation()
		{
			return TutorialHelper.CurrentContext == 4 && TutorialHelper.IsCharacterPopUpWindowOpen;
		}

		// Token: 0x04000035 RID: 53
		private bool _startedTalkingWithNotable;
	}
}
