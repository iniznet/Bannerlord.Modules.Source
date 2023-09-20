using System;
using TaleWorlds.Core;

namespace TaleWorlds.CampaignSystem.Actions
{
	// Token: 0x0200044B RID: 1099
	public static class MakeHeroFugitiveAction
	{
		// Token: 0x06003F31 RID: 16177 RVA: 0x0012E594 File Offset: 0x0012C794
		private static void ApplyInternal(Hero fugitive)
		{
			if (fugitive.IsAlive)
			{
				if (fugitive.PartyBelongedTo != null)
				{
					if (fugitive.PartyBelongedTo.LeaderHero == fugitive)
					{
						DestroyPartyAction.Apply(null, fugitive.PartyBelongedTo);
					}
					else
					{
						fugitive.PartyBelongedTo.MemberRoster.RemoveTroop(fugitive.CharacterObject, 1, default(UniqueTroopDescriptor), 0);
					}
				}
				if (fugitive.CurrentSettlement != null)
				{
					LeaveSettlementAction.ApplyForCharacterOnly(fugitive);
				}
				fugitive.ChangeState(Hero.CharacterStates.Fugitive);
				CampaignEventDispatcher.Instance.OnCharacterBecameFugitive(fugitive);
			}
		}

		// Token: 0x06003F32 RID: 16178 RVA: 0x0012E60E File Offset: 0x0012C80E
		public static void Apply(Hero fugitive)
		{
			MakeHeroFugitiveAction.ApplyInternal(fugitive);
		}
	}
}
