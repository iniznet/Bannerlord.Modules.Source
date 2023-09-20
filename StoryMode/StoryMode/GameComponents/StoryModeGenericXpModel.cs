using System;
using StoryMode.Extensions;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameComponents;

namespace StoryMode.GameComponents
{
	public class StoryModeGenericXpModel : DefaultGenericXpModel
	{
		public override float GetXpMultiplier(Hero hero)
		{
			if (((hero != null) ? hero.CurrentSettlement : null) != null && hero.CurrentSettlement.IsTrainingField())
			{
				return 0f;
			}
			return base.GetXpMultiplier(hero);
		}
	}
}
