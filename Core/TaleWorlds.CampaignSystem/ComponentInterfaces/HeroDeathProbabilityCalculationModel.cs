using System;
using TaleWorlds.Core;

namespace TaleWorlds.CampaignSystem.ComponentInterfaces
{
	// Token: 0x020001AC RID: 428
	public abstract class HeroDeathProbabilityCalculationModel : GameModel
	{
		// Token: 0x06001ABB RID: 6843
		public abstract float CalculateHeroDeathProbability(Hero hero);
	}
}
