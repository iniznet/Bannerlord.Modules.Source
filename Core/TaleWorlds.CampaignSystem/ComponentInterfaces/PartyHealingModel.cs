using System;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;

namespace TaleWorlds.CampaignSystem.ComponentInterfaces
{
	// Token: 0x02000177 RID: 375
	public abstract class PartyHealingModel : GameModel
	{
		// Token: 0x06001917 RID: 6423
		public abstract float GetSurgeryChance(PartyBase party, CharacterObject character);

		// Token: 0x06001918 RID: 6424
		public abstract float GetSurvivalChance(PartyBase party, CharacterObject agentCharacter, DamageTypes damageType, PartyBase enemyParty = null);

		// Token: 0x06001919 RID: 6425
		public abstract int GetSkillXpFromHealingTroop(PartyBase party);

		// Token: 0x0600191A RID: 6426
		public abstract ExplainedNumber GetDailyHealingForRegulars(MobileParty party, bool includeDescriptions = false);

		// Token: 0x0600191B RID: 6427
		public abstract ExplainedNumber GetDailyHealingHpForHeroes(MobileParty party, bool includeDescriptions = false);

		// Token: 0x0600191C RID: 6428
		public abstract int GetHeroesEffectedHealingAmount(Hero hero, float healingRate);

		// Token: 0x0600191D RID: 6429
		public abstract float GetSiegeBombardmentHitSurgeryChance(PartyBase party);

		// Token: 0x0600191E RID: 6430
		public abstract int GetBattleEndHealingAmount(MobileParty party, CharacterObject character);
	}
}
