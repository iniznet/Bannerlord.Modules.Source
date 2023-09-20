using System;
using TaleWorlds.Core;

namespace TaleWorlds.CampaignSystem.ComponentInterfaces
{
	// Token: 0x02000190 RID: 400
	public abstract class RansomValueCalculationModel : GameModel
	{
		// Token: 0x060019EE RID: 6638
		public abstract int PrisonerRansomValue(CharacterObject prisoner, Hero sellerHero = null);
	}
}
