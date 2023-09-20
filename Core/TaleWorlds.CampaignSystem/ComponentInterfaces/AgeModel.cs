using System;
using TaleWorlds.Core;

namespace TaleWorlds.CampaignSystem.ComponentInterfaces
{
	// Token: 0x020001B3 RID: 435
	public abstract class AgeModel : GameModel
	{
		// Token: 0x170006F5 RID: 1781
		// (get) Token: 0x06001ADA RID: 6874
		public abstract int BecomeInfantAge { get; }

		// Token: 0x170006F6 RID: 1782
		// (get) Token: 0x06001ADB RID: 6875
		public abstract int BecomeChildAge { get; }

		// Token: 0x170006F7 RID: 1783
		// (get) Token: 0x06001ADC RID: 6876
		public abstract int BecomeTeenagerAge { get; }

		// Token: 0x170006F8 RID: 1784
		// (get) Token: 0x06001ADD RID: 6877
		public abstract int HeroComesOfAge { get; }

		// Token: 0x170006F9 RID: 1785
		// (get) Token: 0x06001ADE RID: 6878
		public abstract int BecomeOldAge { get; }

		// Token: 0x170006FA RID: 1786
		// (get) Token: 0x06001ADF RID: 6879
		public abstract int MaxAge { get; }

		// Token: 0x06001AE0 RID: 6880
		public abstract void GetAgeLimitForLocation(CharacterObject character, out int minimumAge, out int maximumAge, string additionalTags = "");

		// Token: 0x06001AE1 RID: 6881
		public abstract float GetSkillScalingModifierForAge(Hero hero, SkillObject skill, bool isByNaturalGrowth);
	}
}
