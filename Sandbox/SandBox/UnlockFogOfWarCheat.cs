using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Localization;

namespace SandBox
{
	public class UnlockFogOfWarCheat : GameplayCheatItem
	{
		public override void ExecuteCheat()
		{
			foreach (Hero hero in Hero.AllAliveHeroes)
			{
				hero.IsKnownToPlayer = true;
			}
		}

		public override TextObject GetName()
		{
			return new TextObject("{=jPtG0Pu1}Unlock Fog of War", null);
		}
	}
}
