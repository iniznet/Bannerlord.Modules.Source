using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameComponents;
using TaleWorlds.CampaignSystem.Settlements;

namespace StoryMode.GameComponents
{
	// Token: 0x02000043 RID: 67
	public class StoryModeNotableSpawnModel : DefaultNotableSpawnModel
	{
		// Token: 0x060003BF RID: 959 RVA: 0x0001747F File Offset: 0x0001567F
		public override int GetTargetNotableCountForSettlement(Settlement settlement, Occupation occupation)
		{
			if (!StoryModeManager.Current.MainStoryLine.TutorialPhase.IsCompleted && settlement.StringId == "village_ES3_2")
			{
				return 0;
			}
			return base.GetTargetNotableCountForSettlement(settlement, occupation);
		}
	}
}
