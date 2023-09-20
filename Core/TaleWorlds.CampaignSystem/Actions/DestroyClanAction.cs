using System;
using System.Collections.Generic;
using System.Linq;
using Helpers;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Party.PartyComponents;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;

namespace TaleWorlds.CampaignSystem.Actions
{
	// Token: 0x02000438 RID: 1080
	public static class DestroyClanAction
	{
		// Token: 0x06003ED6 RID: 16086 RVA: 0x0012C468 File Offset: 0x0012A668
		private static void ApplyInternal(Clan destroyedClan, DestroyClanAction.DestroyClanActionDetails details)
		{
			destroyedClan.DeactivateClan();
			foreach (WarPartyComponent warPartyComponent in destroyedClan.WarPartyComponents.ToList<WarPartyComponent>())
			{
				PartyBase partyBase = null;
				if (warPartyComponent.MobileParty.MapEvent != null)
				{
					partyBase = warPartyComponent.MobileParty.MapEventSide.OtherSide.LeaderParty;
					if (warPartyComponent.MobileParty.MapEvent != MobileParty.MainParty.MapEvent)
					{
						warPartyComponent.MobileParty.MapEventSide = null;
					}
				}
				DestroyPartyAction.Apply(partyBase, warPartyComponent.MobileParty);
			}
			List<Hero> list = destroyedClan.Heroes.Where((Hero x) => x.IsAlive).ToList<Hero>();
			for (int i = 0; i < list.Count; i++)
			{
				Hero hero = list[i];
				if (details != DestroyClanAction.DestroyClanActionDetails.ClanLeaderDeath || hero != destroyedClan.Leader)
				{
					KillCharacterAction.ApplyByRemove(hero, false, true);
				}
			}
			if (details != DestroyClanAction.DestroyClanActionDetails.ClanLeaderDeath && destroyedClan.Leader.IsAlive && destroyedClan.Leader.DeathMark == KillCharacterAction.KillCharacterActionDetail.None)
			{
				KillCharacterAction.ApplyByRemove(destroyedClan.Leader, false, true);
			}
			if (!destroyedClan.Settlements.IsEmpty<Settlement>())
			{
				Clan clan = FactionHelper.ChooseHeirClanForFiefs(destroyedClan);
				foreach (Settlement settlement in destroyedClan.Settlements.ToList<Settlement>())
				{
					if (settlement.IsTown || settlement.IsCastle)
					{
						Hero randomElementWithPredicate = clan.Lords.GetRandomElementWithPredicate((Hero x) => !x.IsChild && x.IsAlive);
						ChangeOwnerOfSettlementAction.ApplyByDestroyClan(settlement, randomElementWithPredicate);
					}
				}
			}
			FactionManager.Instance.RemoveFactionsFromCampaignWars(destroyedClan);
			if (destroyedClan.Kingdom != null)
			{
				ChangeKingdomAction.ApplyByLeaveKingdomByClanDestruction(destroyedClan, true);
			}
			if (destroyedClan.IsRebelClan)
			{
				Campaign.Current.CampaignObjectManager.RemoveClan(destroyedClan);
			}
			CampaignEventDispatcher.Instance.OnClanDestroyed(destroyedClan);
		}

		// Token: 0x06003ED7 RID: 16087 RVA: 0x0012C684 File Offset: 0x0012A884
		public static void Apply(Clan destroyedClan)
		{
			DestroyClanAction.ApplyInternal(destroyedClan, DestroyClanAction.DestroyClanActionDetails.Default);
		}

		// Token: 0x06003ED8 RID: 16088 RVA: 0x0012C68D File Offset: 0x0012A88D
		public static void ApplyByFailedRebellion(Clan failedRebellionClan)
		{
			DestroyClanAction.ApplyInternal(failedRebellionClan, DestroyClanAction.DestroyClanActionDetails.RebellionFailure);
		}

		// Token: 0x06003ED9 RID: 16089 RVA: 0x0012C696 File Offset: 0x0012A896
		public static void ApplyByClanLeaderDeath(Clan destroyedClan)
		{
			DestroyClanAction.ApplyInternal(destroyedClan, DestroyClanAction.DestroyClanActionDetails.ClanLeaderDeath);
		}

		// Token: 0x0200075C RID: 1884
		private enum DestroyClanActionDetails
		{
			// Token: 0x04001E3C RID: 7740
			Default,
			// Token: 0x04001E3D RID: 7741
			RebellionFailure,
			// Token: 0x04001E3E RID: 7742
			ClanLeaderDeath
		}
	}
}
