using System;
using StoryMode.Extensions;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameComponents;

namespace StoryMode.GameComponents
{
	// Token: 0x02000040 RID: 64
	public class StoryModeGenericXpModel : DefaultGenericXpModel
	{
		// Token: 0x060003B8 RID: 952 RVA: 0x0001732D File Offset: 0x0001552D
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
