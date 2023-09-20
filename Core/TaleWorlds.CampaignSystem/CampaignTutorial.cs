using System;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem
{
	// Token: 0x02000070 RID: 112
	public class CampaignTutorial
	{
		// Token: 0x170003A3 RID: 931
		// (get) Token: 0x06000EB4 RID: 3764 RVA: 0x000449E1 File Offset: 0x00042BE1
		public TextObject Description
		{
			get
			{
				return GameTexts.FindText("str_campaign_tutorial_description", this.TutorialTypeId);
			}
		}

		// Token: 0x170003A4 RID: 932
		// (get) Token: 0x06000EB5 RID: 3765 RVA: 0x000449F3 File Offset: 0x00042BF3
		public TextObject Title
		{
			get
			{
				return GameTexts.FindText("str_campaign_tutorial_title", this.TutorialTypeId);
			}
		}

		// Token: 0x06000EB6 RID: 3766 RVA: 0x00044A05 File Offset: 0x00042C05
		public CampaignTutorial(string tutorialType, int priority)
		{
			this.TutorialTypeId = tutorialType;
			this.Priority = priority;
		}

		// Token: 0x04000462 RID: 1122
		public readonly string TutorialTypeId;

		// Token: 0x04000463 RID: 1123
		public readonly int Priority;
	}
}
