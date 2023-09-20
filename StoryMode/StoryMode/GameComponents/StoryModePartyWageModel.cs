using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameComponents;

namespace StoryMode.GameComponents
{
	// Token: 0x02000045 RID: 69
	public class StoryModePartyWageModel : DefaultPartyWageModel
	{
		// Token: 0x060003C3 RID: 963 RVA: 0x00017546 File Offset: 0x00015746
		public override int GetTroopRecruitmentCost(CharacterObject troop, Hero buyerHero, bool withoutItemCost = false)
		{
			if (StoryModeManager.Current.MainStoryLine.TutorialPhase.IsCompleted)
			{
				return base.GetTroopRecruitmentCost(troop, buyerHero, withoutItemCost);
			}
			if (!(troop.StringId == "tutorial_placeholder_volunteer"))
			{
				return base.GetTroopRecruitmentCost(troop, buyerHero, withoutItemCost);
			}
			return 50;
		}

		// Token: 0x04000182 RID: 386
		private const int StoryModeTutorialTroopCost = 50;
	}
}
