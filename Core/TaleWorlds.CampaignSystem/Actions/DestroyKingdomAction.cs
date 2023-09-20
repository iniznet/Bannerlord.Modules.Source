using System;
using System.Linq;

namespace TaleWorlds.CampaignSystem.Actions
{
	// Token: 0x02000439 RID: 1081
	public static class DestroyKingdomAction
	{
		// Token: 0x06003EDA RID: 16090 RVA: 0x0012C6A0 File Offset: 0x0012A8A0
		private static void ApplyInternal(Kingdom destroyedKingdom, bool isKingdomLeaderDeath = false)
		{
			destroyedKingdom.DeactivateKingdom();
			foreach (Clan clan in destroyedKingdom.Clans.ToList<Clan>())
			{
				if (!clan.IsEliminated)
				{
					if (isKingdomLeaderDeath)
					{
						DestroyClanAction.ApplyByClanLeaderDeath(clan);
					}
					else
					{
						DestroyClanAction.Apply(clan);
					}
					destroyedKingdom.RemoveClanInternal(clan);
				}
			}
			Campaign.Current.FactionManager.RemoveFactionsFromCampaignWars(destroyedKingdom);
			CampaignEventDispatcher.Instance.OnKingdomDestroyed(destroyedKingdom);
		}

		// Token: 0x06003EDB RID: 16091 RVA: 0x0012C734 File Offset: 0x0012A934
		public static void Apply(Kingdom destroyedKingdom)
		{
			DestroyKingdomAction.ApplyInternal(destroyedKingdom, false);
		}

		// Token: 0x06003EDC RID: 16092 RVA: 0x0012C73D File Offset: 0x0012A93D
		public static void ApplyByKingdomLeaderDeath(Kingdom destroyedKingdom)
		{
			DestroyKingdomAction.ApplyInternal(destroyedKingdom, true);
		}
	}
}
