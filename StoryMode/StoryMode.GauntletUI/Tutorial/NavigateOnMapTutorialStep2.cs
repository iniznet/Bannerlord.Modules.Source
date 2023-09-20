using System;
using StoryMode.ViewModelCollection.Tutorial;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;

namespace StoryMode.GauntletUI.Tutorial
{
	// Token: 0x02000024 RID: 36
	public class NavigateOnMapTutorialStep2 : TutorialItemBase
	{
		// Token: 0x060000AF RID: 175 RVA: 0x00003578 File Offset: 0x00001778
		public NavigateOnMapTutorialStep2()
		{
			base.Type = "NavigateOnMapTutorialStep2";
			base.Placement = TutorialItemVM.ItemPlacements.TopRight;
			base.HighlightedVisualElementID = "village_ES3_2";
			base.MouseRequired = true;
		}

		// Token: 0x060000B0 RID: 176 RVA: 0x000035A4 File Offset: 0x000017A4
		public override TutorialContexts GetTutorialsRelevantContext()
		{
			return 4;
		}

		// Token: 0x060000B1 RID: 177 RVA: 0x000035A7 File Offset: 0x000017A7
		public override bool IsConditionsMetForActivation()
		{
			return TutorialHelper.CurrentContext == 4;
		}

		// Token: 0x060000B2 RID: 178 RVA: 0x000035B1 File Offset: 0x000017B1
		public override bool IsConditionsMetForCompletion()
		{
			MobileParty mainParty = MobileParty.MainParty;
			string text;
			if (mainParty == null)
			{
				text = null;
			}
			else
			{
				Settlement targetSettlement = mainParty.TargetSettlement;
				text = ((targetSettlement != null) ? targetSettlement.StringId : null);
			}
			return text == "village_ES3_2";
		}

		// Token: 0x04000031 RID: 49
		private const string TargetQuestVillage = "village_ES3_2";
	}
}
