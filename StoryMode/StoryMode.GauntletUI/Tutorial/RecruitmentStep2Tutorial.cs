using System;
using StoryMode.ViewModelCollection.Tutorial;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;

namespace StoryMode.GauntletUI.Tutorial
{
	// Token: 0x02000014 RID: 20
	public class RecruitmentStep2Tutorial : TutorialItemBase
	{
		// Token: 0x0600005C RID: 92 RVA: 0x00002A47 File Offset: 0x00000C47
		public RecruitmentStep2Tutorial()
		{
			base.Type = "RecruitmentTutorialStep2";
			base.Placement = TutorialItemVM.ItemPlacements.Right;
			base.HighlightedVisualElementID = "AvailableTroops";
			base.MouseRequired = true;
		}

		// Token: 0x0600005D RID: 93 RVA: 0x00002A73 File Offset: 0x00000C73
		public override bool IsConditionsMetForCompletion()
		{
			return this._playerRecruitedTroop;
		}

		// Token: 0x0600005E RID: 94 RVA: 0x00002A7B File Offset: 0x00000C7B
		public override void OnPlayerStartRecruitment(CharacterObject obj)
		{
			this._playerRecruitedTroop = true;
		}

		// Token: 0x0600005F RID: 95 RVA: 0x00002A84 File Offset: 0x00000C84
		public override TutorialContexts GetTutorialsRelevantContext()
		{
			return 5;
		}

		// Token: 0x06000060 RID: 96 RVA: 0x00002A87 File Offset: 0x00000C87
		public override bool IsConditionsMetForActivation()
		{
			return TutorialHelper.PlayerCanRecruit && TutorialHelper.CurrentContext == 5;
		}

		// Token: 0x04000015 RID: 21
		private bool _playerRecruitedTroop;
	}
}
