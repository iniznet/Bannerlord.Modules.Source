using System;
using TaleWorlds.CampaignSystem.Party;

namespace TaleWorlds.CampaignSystem.Actions
{
	// Token: 0x0200045A RID: 1114
	public static class TransferPrisonerAction
	{
		// Token: 0x06003F6A RID: 16234 RVA: 0x0012FFD2 File Offset: 0x0012E1D2
		private static void ApplyInternal(CharacterObject prisonerTroop, PartyBase prisonerOwnerParty, PartyBase newParty)
		{
			if (prisonerTroop.HeroObject == Hero.MainHero)
			{
				PlayerCaptivity.CaptorParty = newParty;
				return;
			}
			prisonerOwnerParty.PrisonRoster.AddToCounts(prisonerTroop, -1, false, 0, 0, true, -1);
			newParty.AddPrisoner(prisonerTroop, 1);
		}

		// Token: 0x06003F6B RID: 16235 RVA: 0x00130004 File Offset: 0x0012E204
		public static void Apply(CharacterObject prisonerTroop, PartyBase prisonerOwnerParty, PartyBase newParty)
		{
			TransferPrisonerAction.ApplyInternal(prisonerTroop, prisonerOwnerParty, newParty);
		}
	}
}
