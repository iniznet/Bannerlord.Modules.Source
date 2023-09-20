using System;
using StoryMode.StoryModeObjects;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameComponents;

namespace StoryMode.GameComponents
{
	public class StoryModeHeroDeathProbabilityCalculationModel : DefaultHeroDeathProbabilityCalculationModel
	{
		public override float CalculateHeroDeathProbability(Hero hero)
		{
			if (hero == StoryModeHeroes.ElderBrother && !StoryModeManager.Current.MainStoryLine.IsCompleted)
			{
				return 0f;
			}
			return base.CalculateHeroDeathProbability(hero);
		}
	}
}
