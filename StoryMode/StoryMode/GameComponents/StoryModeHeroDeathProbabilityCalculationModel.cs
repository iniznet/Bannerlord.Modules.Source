using System;
using StoryMode.StoryModeObjects;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameComponents;

namespace StoryMode.GameComponents
{
	// Token: 0x02000041 RID: 65
	public class StoryModeHeroDeathProbabilityCalculationModel : DefaultHeroDeathProbabilityCalculationModel
	{
		// Token: 0x060003BA RID: 954 RVA: 0x0001735F File Offset: 0x0001555F
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
