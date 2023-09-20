using System;
using StoryMode.ViewModelCollection.Tutorial;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;

namespace StoryMode.GauntletUI.Tutorial
{
	// Token: 0x0200000C RID: 12
	public class ChoosingSkillFocusStep1Tutorial : TutorialItemBase
	{
		// Token: 0x06000034 RID: 52 RVA: 0x0000250E File Offset: 0x0000070E
		public ChoosingSkillFocusStep1Tutorial()
		{
			base.Type = "ChoosingSkillFocusStep1";
			base.Placement = TutorialItemVM.ItemPlacements.Right;
			base.HighlightedVisualElementID = "CharacterButton";
			base.MouseRequired = true;
		}

		// Token: 0x06000035 RID: 53 RVA: 0x0000253A File Offset: 0x0000073A
		public override bool IsConditionsMetForCompletion()
		{
			return this._characterWindowOpened;
		}

		// Token: 0x06000036 RID: 54 RVA: 0x00002542 File Offset: 0x00000742
		public override void OnTutorialContextChanged(TutorialContextChangedEvent obj)
		{
			this._characterWindowOpened = obj.NewContext == 3;
		}

		// Token: 0x06000037 RID: 55 RVA: 0x00002553 File Offset: 0x00000753
		public override TutorialContexts GetTutorialsRelevantContext()
		{
			return 4;
		}

		// Token: 0x06000038 RID: 56 RVA: 0x00002556 File Offset: 0x00000756
		public override bool IsConditionsMetForActivation()
		{
			return Settlement.CurrentSettlement == null && Hero.MainHero.HeroDeveloper.UnspentFocusPoints > 1 && (TutorialHelper.PlayerIsInAnySettlement || TutorialHelper.PlayerIsSafeOnMap) && TutorialHelper.CurrentContext == 4;
		}

		// Token: 0x0400000D RID: 13
		private bool _characterWindowOpened;
	}
}
