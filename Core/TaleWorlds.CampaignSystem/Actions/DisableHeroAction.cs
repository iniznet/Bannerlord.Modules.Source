using System;
using TaleWorlds.Core;

namespace TaleWorlds.CampaignSystem.Actions
{
	// Token: 0x0200043B RID: 1083
	public static class DisableHeroAction
	{
		// Token: 0x06003EE0 RID: 16096 RVA: 0x0012C7E4 File Offset: 0x0012A9E4
		private static void ApplyInternal(Hero hero)
		{
			if (hero.IsAlive)
			{
				if (hero.PartyBelongedTo != null)
				{
					if (hero.PartyBelongedTo.LeaderHero == hero)
					{
						DestroyPartyAction.Apply(null, hero.PartyBelongedTo);
					}
					else
					{
						hero.PartyBelongedTo.MemberRoster.RemoveTroop(hero.CharacterObject, 1, default(UniqueTroopDescriptor), 0);
					}
				}
				if (hero.StayingInSettlement != null)
				{
					hero.ChangeState(Hero.CharacterStates.Disabled);
					hero.StayingInSettlement = null;
				}
				if (hero.CurrentSettlement != null)
				{
					LeaveSettlementAction.ApplyForCharacterOnly(hero);
				}
				if (hero.IsPrisoner)
				{
					EndCaptivityAction.ApplyByEscape(hero, null);
				}
				hero.ChangeState(Hero.CharacterStates.Disabled);
			}
		}

		// Token: 0x06003EE1 RID: 16097 RVA: 0x0012C878 File Offset: 0x0012AA78
		public static void Apply(Hero hero)
		{
			DisableHeroAction.ApplyInternal(hero);
		}
	}
}
