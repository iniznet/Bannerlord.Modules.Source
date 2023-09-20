using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameComponents;
using TaleWorlds.CampaignSystem.Settlements;

namespace StoryMode.GameComponents
{
	public class StoryModeNotableSpawnModel : DefaultNotableSpawnModel
	{
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
