using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.Extensions;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;

namespace TaleWorlds.CampaignSystem.CampaignBehaviors
{
	public class TradeCampaignBehavior : CampaignBehaviorBase
	{
		public void OnNewGameCreated(CampaignGameStarter campaignGameStarter)
		{
			this.InitializeMarkets();
		}

		public override void RegisterEvents()
		{
			CampaignEvents.DailyTickTownEvent.AddNonSerializedListener(this, new Action<Town>(this.DailyTickTown));
			CampaignEvents.OnNewGameCreatedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(this.OnNewGameCreated));
			CampaignEvents.OnNewGameCreatedPartialFollowUpEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter, int>(this.OnNewGameCreatedPartialFollowUp));
			CampaignEvents.OnSessionLaunchedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(this.OnSessionLaunched));
		}

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

		private void OnSessionLaunched(CampaignGameStarter campaignGameStarter)
		{
			foreach (Town town in Town.AllTowns)
			{
				this.UpdateMarketStores(town);
			}
		}

		public override void SyncData(IDataStore dataStore)
		{
			dataStore.SyncData<Dictionary<ItemCategory, float>>("_numberOfTotalItemsAtGameWorld", ref this._numberOfTotalItemsAtGameWorld);
		}

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

		public void DailyTickTown(Town town)
		{
			this.UpdateMarketStores(town);
		}

		private void UpdateMarketStores(Town town)
		{
			town.MarketData.UpdateStores();
		}

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

		private Dictionary<ItemCategory, float> _numberOfTotalItemsAtGameWorld;

		public const float MaximumTaxRatioForVillages = 1f;

		public const float MaximumTaxRatioForTowns = 0.5f;

		public enum TradeGoodType
		{
			Grain,
			Wood,
			Meat,
			Wool,
			Cheese,
			Iron,
			Salt,
			Spice,
			Raw_Silk,
			Fish,
			Flax,
			Grape,
			Hides,
			Clay,
			Date_Fruit,
			Bread,
			Beer,
			Wine,
			Tools,
			Pottery,
			Cloth,
			Linen,
			Leather,
			Velvet,
			Saddle_Horse,
			Steppe_Horse,
			Hunter,
			Desert_Horse,
			Charger,
			War_Horse,
			Steppe_Charger,
			Desert_War_Horse,
			Unknown,
			NumberOfTradeItems
		}
	}
}
