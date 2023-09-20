using System;
using StoryMode.ViewModelCollection.Tutorial;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;

namespace StoryMode.GauntletUI.Tutorial
{
	// Token: 0x02000013 RID: 19
	public class RecruitmentStep1Tutorial : TutorialItemBase
	{
		// Token: 0x06000057 RID: 87 RVA: 0x000029C9 File Offset: 0x00000BC9
		public RecruitmentStep1Tutorial()
		{
			base.Type = "RecruitmentTutorialStep1";
			base.Placement = TutorialItemVM.ItemPlacements.Right;
			base.HighlightedVisualElementID = "storymode_tutorial_village_recruit";
			base.MouseRequired = true;
		}

		// Token: 0x06000058 RID: 88 RVA: 0x000029F5 File Offset: 0x00000BF5
		public override bool IsConditionsMetForCompletion()
		{
			return this._recruitmentOpened;
		}

		// Token: 0x06000059 RID: 89 RVA: 0x000029FD File Offset: 0x00000BFD
		public override void OnTutorialContextChanged(TutorialContextChangedEvent obj)
		{
			this._recruitmentOpened = obj.NewContext == 5;
		}

		// Token: 0x0600005A RID: 90 RVA: 0x00002A0E File Offset: 0x00000C0E
		public override TutorialContexts GetTutorialsRelevantContext()
		{
			return 4;
		}

		// Token: 0x0600005B RID: 91 RVA: 0x00002A11 File Offset: 0x00000C11
		public override bool IsConditionsMetForActivation()
		{
			return !TutorialHelper.IsCharacterPopUpWindowOpen && TutorialHelper.CurrentContext == 4 && TutorialHelper.PlayerCanRecruit && !Settlement.CurrentSettlement.MapFaction.IsAtWarWith(MobileParty.MainParty.MapFaction);
		}

		// Token: 0x04000014 RID: 20
		private bool _recruitmentOpened;
	}
}
