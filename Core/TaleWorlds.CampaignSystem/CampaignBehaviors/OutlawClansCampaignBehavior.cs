using System;

namespace TaleWorlds.CampaignSystem.CampaignBehaviors
{
	public class OutlawClansCampaignBehavior : CampaignBehaviorBase
	{
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

		public override void RegisterEvents()
		{
			CampaignEvents.OnNewGameCreatedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(this.OnNewGameCreated));
		}

		private void OnNewGameCreated(CampaignGameStarter starter)
		{
			OutlawClansCampaignBehavior.MakeOutlawFactionsEnemyToKingdomFactions();
		}

		public override void SyncData(IDataStore dataStore)
		{
		}
	}
}
