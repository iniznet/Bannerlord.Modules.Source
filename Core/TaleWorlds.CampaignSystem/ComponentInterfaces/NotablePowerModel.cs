using System;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.ComponentInterfaces
{
	// Token: 0x020001B6 RID: 438
	public abstract class NotablePowerModel : GameModel
	{
		// Token: 0x17000700 RID: 1792
		// (get) Token: 0x06001AEE RID: 6894
		public abstract int RegularNotableMaxPowerLevel { get; }

		// Token: 0x17000701 RID: 1793
		// (get) Token: 0x06001AEF RID: 6895
		public abstract int NotableDisappearPowerLimit { get; }

		// Token: 0x06001AF0 RID: 6896
		public abstract ExplainedNumber CalculateDailyPowerChangeForHero(Hero hero, bool includeDescriptions = false);

		// Token: 0x06001AF1 RID: 6897
		public abstract TextObject GetPowerRankName(Hero hero);

		// Token: 0x06001AF2 RID: 6898
		public abstract float GetInfluenceBonusToClan(Hero hero);

		// Token: 0x06001AF3 RID: 6899
		public abstract int GetInitialPower();
	}
}
