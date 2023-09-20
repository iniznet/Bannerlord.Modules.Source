using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.Extensions;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;

namespace TaleWorlds.CampaignSystem.CampaignBehaviors
{
	// Token: 0x020003DA RID: 986
	public class TradeCampaignBehavior : CampaignBehaviorBase
	{
		// Token: 0x06003BA6 RID: 15270 RVA: 0x0011A243 File Offset: 0x00118443
		public void OnNewGameCreated(CampaignGameStarter campaignGameStarter)
		{
			this.InitializeMarkets();
		}

		// Token: 0x06003BA7 RID: 15271 RVA: 0x0011A24C File Offset: 0x0011844C
		public override void RegisterEvents()
		{
			CampaignEvents.DailyTickTownEvent.AddNonSerializedListener(this, new Action<Town>(this.DailyTickTown));
			CampaignEvents.OnNewGameCreatedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(this.OnNewGameCreated));
			CampaignEvents.OnNewGameCreatedPartialFollowUpEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter, int>(this.OnNewGameCreatedPartialFollowUp));
			CampaignEvents.OnSessionLaunchedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(this.OnSessionLaunched));
		}

		// Token: 0x06003BA8 RID: 15272 RVA: 0x0011A2B8 File Offset: 0x001184B8
		private void OnNewGameCreatedPartialFollowUp(CampaignGameStarter campaignGameStarter, int i)
		{
			if (i == 2)
			{
				this.InitializeTrade();
			}
			if (i % 10 == 0)
			{
				foreach (Settlement settlement in Settlement.All)
				{
					if (settlement.IsTown)
					{
						this.UpdateMarketStores(settlement.Town);
					}
				}
			}
		}

		// Token: 0x06003BA9 RID: 15273 RVA: 0x0011A328 File Offset: 0x00118528
		private void OnSessionLaunched(CampaignGameStarter campaignGameStarter)
		{
			foreach (Town town in Town.AllTowns)
			{
				this.UpdateMarketStores(town);
			}
		}

		// Token: 0x06003BAA RID: 15274 RVA: 0x0011A37C File Offset: 0x0011857C
		public override void SyncData(IDataStore dataStore)
		{
			dataStore.SyncData<Dictionary<ItemCategory, float>>("_numberOfTotalItemsAtGameWorld", ref this._numberOfTotalItemsAtGameWorld);
		}

		// Token: 0x06003BAB RID: 15275 RVA: 0x0011A390 File Offset: 0x00118590
		private void InitializeTrade()
		{
			this._numberOfTotalItemsAtGameWorld = new Dictionary<ItemCategory, float>();
			Campaign.Current.Settlements.Where((Settlement settlement) => settlement.IsTown).ToList<Settlement>();
			foreach (Hero hero in Hero.AllAliveHeroes)
			{
				if (hero.CharacterObject.Occupation == Occupation.Lord && hero.Clan != Clan.PlayerClan)
				{
					Clan clan = hero.Clan;
					int num;
					if (((clan != null) ? clan.Leader : null) == hero)
					{
						num = 50000 + 10000 * hero.Clan.Tier + ((hero == hero.MapFaction.Leader) ? 50000 : 0);
					}
					else
					{
						num = 10000;
					}
					GiveGoldAction.ApplyBetweenCharacters(null, hero, num, false);
				}
			}
		}

		// Token: 0x06003BAC RID: 15276 RVA: 0x0011A490 File Offset: 0x00118690
		public void DailyTickTown(Town town)
		{
			this.UpdateMarketStores(town);
		}

		// Token: 0x06003BAD RID: 15277 RVA: 0x0011A499 File Offset: 0x00118699
		private void UpdateMarketStores(Town town)
		{
			town.MarketData.UpdateStores();
		}

		// Token: 0x06003BAE RID: 15278 RVA: 0x0011A4A8 File Offset: 0x001186A8
		private void InitializeMarkets()
		{
			foreach (Town town in Town.AllTowns)
			{
				foreach (ItemCategory itemCategory in ItemCategories.All)
				{
					if (itemCategory.IsValid)
					{
						town.MarketData.AddDemand(itemCategory, 3f);
						town.MarketData.AddSupply(itemCategory, 2f);
					}
				}
			}
		}

		// Token: 0x0400122C RID: 4652
		private Dictionary<ItemCategory, float> _numberOfTotalItemsAtGameWorld;

		// Token: 0x0400122D RID: 4653
		public const float MaximumTaxRatioForVillages = 1f;

		// Token: 0x0400122E RID: 4654
		public const float MaximumTaxRatioForTowns = 0.5f;

		// Token: 0x02000737 RID: 1847
		public enum TradeGoodType
		{
			// Token: 0x04001DBA RID: 7610
			Grain,
			// Token: 0x04001DBB RID: 7611
			Wood,
			// Token: 0x04001DBC RID: 7612
			Meat,
			// Token: 0x04001DBD RID: 7613
			Wool,
			// Token: 0x04001DBE RID: 7614
			Cheese,
			// Token: 0x04001DBF RID: 7615
			Iron,
			// Token: 0x04001DC0 RID: 7616
			Salt,
			// Token: 0x04001DC1 RID: 7617
			Spice,
			// Token: 0x04001DC2 RID: 7618
			Raw_Silk,
			// Token: 0x04001DC3 RID: 7619
			Fish,
			// Token: 0x04001DC4 RID: 7620
			Flax,
			// Token: 0x04001DC5 RID: 7621
			Grape,
			// Token: 0x04001DC6 RID: 7622
			Hides,
			// Token: 0x04001DC7 RID: 7623
			Clay,
			// Token: 0x04001DC8 RID: 7624
			Date_Fruit,
			// Token: 0x04001DC9 RID: 7625
			Bread,
			// Token: 0x04001DCA RID: 7626
			Beer,
			// Token: 0x04001DCB RID: 7627
			Wine,
			// Token: 0x04001DCC RID: 7628
			Tools,
			// Token: 0x04001DCD RID: 7629
			Pottery,
			// Token: 0x04001DCE RID: 7630
			Cloth,
			// Token: 0x04001DCF RID: 7631
			Linen,
			// Token: 0x04001DD0 RID: 7632
			Leather,
			// Token: 0x04001DD1 RID: 7633
			Velvet,
			// Token: 0x04001DD2 RID: 7634
			Saddle_Horse,
			// Token: 0x04001DD3 RID: 7635
			Steppe_Horse,
			// Token: 0x04001DD4 RID: 7636
			Hunter,
			// Token: 0x04001DD5 RID: 7637
			Desert_Horse,
			// Token: 0x04001DD6 RID: 7638
			Charger,
			// Token: 0x04001DD7 RID: 7639
			War_Horse,
			// Token: 0x04001DD8 RID: 7640
			Steppe_Charger,
			// Token: 0x04001DD9 RID: 7641
			Desert_War_Horse,
			// Token: 0x04001DDA RID: 7642
			Unknown,
			// Token: 0x04001DDB RID: 7643
			NumberOfTradeItems
		}
	}
}
