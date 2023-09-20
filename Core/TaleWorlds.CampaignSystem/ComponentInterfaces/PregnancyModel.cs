using System;
using TaleWorlds.Core;

namespace TaleWorlds.CampaignSystem.ComponentInterfaces
{
	// Token: 0x020001B5 RID: 437
	public abstract class PregnancyModel : GameModel
	{
		// Token: 0x06001AE7 RID: 6887
		public abstract float GetDailyChanceOfPregnancyForHero(Hero hero);

		// Token: 0x170006FB RID: 1787
		// (get) Token: 0x06001AE8 RID: 6888
		public abstract float PregnancyDurationInDays { get; }

		// Token: 0x170006FC RID: 1788
		// (get) Token: 0x06001AE9 RID: 6889
		public abstract float MaternalMortalityProbabilityInLabor { get; }

		// Token: 0x170006FD RID: 1789
		// (get) Token: 0x06001AEA RID: 6890
		public abstract float StillbirthProbability { get; }

		// Token: 0x170006FE RID: 1790
		// (get) Token: 0x06001AEB RID: 6891
		public abstract float DeliveringFemaleOffspringProbability { get; }

		// Token: 0x170006FF RID: 1791
		// (get) Token: 0x06001AEC RID: 6892
		public abstract float DeliveringTwinsProbability { get; }
	}
}
