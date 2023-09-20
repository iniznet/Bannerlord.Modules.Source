using System;

namespace TaleWorlds.CampaignSystem.CampaignBehaviors
{
	// Token: 0x020003B3 RID: 947
	public class OutlawClansCampaignBehavior : CampaignBehaviorBase
	{
		// Token: 0x06003884 RID: 14468 RVA: 0x00101328 File Offset: 0x000FF528
		private static void MakeOutlawFactionsEnemyToKingdomFactions()
		{
			foreach (Clan clan in Clan.All)
			{
				if (clan.IsMinorFaction && clan.IsOutlaw)
				{
					foreach (Kingdom kingdom in Kingdom.All)
					{
						if (kingdom.Culture == clan.Culture)
						{
							FactionManager.DeclareWar(kingdom, clan, true);
						}
					}
				}
			}
		}

		// Token: 0x06003885 RID: 14469 RVA: 0x001013D4 File Offset: 0x000FF5D4
		public override void RegisterEvents()
		{
			CampaignEvents.OnNewGameCreatedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(this.OnNewGameCreated));
		}

		// Token: 0x06003886 RID: 14470 RVA: 0x001013ED File Offset: 0x000FF5ED
		private void OnNewGameCreated(CampaignGameStarter starter)
		{
			OutlawClansCampaignBehavior.MakeOutlawFactionsEnemyToKingdomFactions();
		}

		// Token: 0x06003887 RID: 14471 RVA: 0x001013F4 File Offset: 0x000FF5F4
		public override void SyncData(IDataStore dataStore)
		{
		}
	}
}
