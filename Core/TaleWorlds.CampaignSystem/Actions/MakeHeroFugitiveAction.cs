using System;
using TaleWorlds.Core;

namespace TaleWorlds.CampaignSystem.Actions
{
	public static class MakeHeroFugitiveAction
	{
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

		public static void Apply(Hero fugitive)
		{
			MakeHeroFugitiveAction.ApplyInternal(fugitive);
		}
	}
}
