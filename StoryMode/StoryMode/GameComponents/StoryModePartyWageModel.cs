using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameComponents;

namespace StoryMode.GameComponents
{
	public class StoryModePartyWageModel : DefaultPartyWageModel
	{
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

		private const int StoryModeTutorialTroopCost = 50;
	}
}
