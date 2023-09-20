using System;
using System.Collections.Generic;
using Helpers;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.CampaignBehaviors
{
	// Token: 0x02000379 RID: 889
	public class BannerCampaignBehavior : CampaignBehaviorBase
	{
		// Token: 0x060033F9 RID: 13305 RVA: 0x000D9094 File Offset: 0x000D7294
		public override void RegisterEvents()
		{
			CampaignEvents.OnNewGameCreatedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(this.OnNewGameCreated));
			CampaignEvents.OnGameLoadFinishedEvent.AddNonSerializedListener(this, new Action(this.GiveBannersToHeroes));
			CampaignEvents.DailyTickHeroEvent.AddNonSerializedListener(this, new Action<Hero>(this.DailyTickHero));
			CampaignEvents.CollectLootsEvent.AddNonSerializedListener(this, new Action<MapEvent, PartyBase, Dictionary<PartyBase, ItemRoster>, ItemRoster, MBList<TroopRosterElement>, float>(this.CollectLoots));
			CampaignEvents.HeroComesOfAgeEvent.AddNonSerializedListener(this, new Action<Hero>(this.OnHeroComesOfAge));
			CampaignEvents.HeroCreated.AddNonSerializedListener(this, new Action<Hero, bool>(this.OnHeroCreated));
			CampaignEvents.OnCompanionClanCreatedEvent.AddNonSerializedListener(this, new Action<Clan>(this.OnCompanionClanCreated));
		}

		// Token: 0x060033FA RID: 13306 RVA: 0x000D9142 File Offset: 0x000D7342
		public override void SyncData(IDataStore dataStore)
		{
			dataStore.SyncData<Dictionary<Hero, CampaignTime>>("_heroNextBannerLootTime", ref this._heroNextBannerLootTime);
		}

		// Token: 0x060033FB RID: 13307 RVA: 0x000D9156 File Offset: 0x000D7356
		private void OnNewGameCreated(CampaignGameStarter campaignGameStarter)
		{
			this.GiveBannersToHeroes();
		}

		// Token: 0x060033FC RID: 13308 RVA: 0x000D9160 File Offset: 0x000D7360
		private void GiveBannersToHeroes()
		{
			foreach (Hero hero in Hero.AllAliveHeroes)
			{
				if (this.CanBannerBeGivenToHero(hero))
				{
					ItemObject randomBannerItemForHero = BannerHelper.GetRandomBannerItemForHero(hero);
					if (randomBannerItemForHero != null)
					{
						hero.BannerItem = new EquipmentElement(randomBannerItemForHero, null, null, false);
					}
				}
			}
		}

		// Token: 0x060033FD RID: 13309 RVA: 0x000D91D0 File Offset: 0x000D73D0
		private void DailyTickHero(Hero hero)
		{
			if (hero.Clan != Clan.PlayerClan)
			{
				EquipmentElement bannerItem = hero.BannerItem;
				BannerItemModel bannerItemModel = Campaign.Current.Models.BannerItemModel;
				if (!bannerItem.IsInvalid() && bannerItemModel.CanBannerBeUpdated(bannerItem.Item) && MBRandom.RandomFloat < 0.1f)
				{
					int bannerLevel = ((BannerComponent)bannerItem.Item.ItemComponent).BannerLevel;
					int bannerItemLevelForHero = bannerItemModel.GetBannerItemLevelForHero(hero);
					if (bannerLevel != bannerItemLevelForHero)
					{
						ItemObject upgradeBannerForHero = this.GetUpgradeBannerForHero(hero, bannerItemLevelForHero);
						if (upgradeBannerForHero != null)
						{
							hero.BannerItem = new EquipmentElement(upgradeBannerForHero, null, null, false);
						}
					}
				}
			}
		}

		// Token: 0x060033FE RID: 13310 RVA: 0x000D9264 File Offset: 0x000D7464
		private ItemObject GetUpgradeBannerForHero(Hero hero, int upgradeBannerLevel)
		{
			ItemObject item = hero.BannerItem.Item;
			foreach (ItemObject itemObject in Campaign.Current.Models.BannerItemModel.GetPossibleRewardBannerItems())
			{
				BannerComponent bannerComponent = (BannerComponent)itemObject.ItemComponent;
				if (itemObject.Culture == item.Culture && bannerComponent.BannerLevel == upgradeBannerLevel && bannerComponent.BannerEffect == ((BannerComponent)item.ItemComponent).BannerEffect)
				{
					return itemObject;
				}
			}
			return BannerHelper.GetRandomBannerItemForHero(hero);
		}

		// Token: 0x060033FF RID: 13311 RVA: 0x000D9314 File Offset: 0x000D7514
		private void CollectLoots(MapEvent mapEvent, PartyBase party, Dictionary<PartyBase, ItemRoster> loot, ItemRoster lootedItems, MBList<TroopRosterElement> lootedCasualties, float lootAmount)
		{
			if (party == PartyBase.MainParty && mapEvent.IsPlayerMapEvent && mapEvent.WinningSide == mapEvent.PlayerSide)
			{
				ItemObject[] array = new ItemObject[2];
				if (mapEvent.IsHideoutBattle)
				{
					array[0] = this.GetBannerRewardForHideoutBattle();
				}
				else if (mapEvent.AttackerSide.MissionSide == mapEvent.PlayerSide && mapEvent.IsSiegeAssault)
				{
					array[0] = this.GetBannerRewardForCapturingFortification(mapEvent.MapEventSettlement);
				}
				array[1] = this.GetBannerRewardForDefeatingNoble(mapEvent, lootedCasualties);
				foreach (ItemObject itemObject in array)
				{
					if (itemObject != null)
					{
						lootedItems.AddToCounts(itemObject, 1);
					}
				}
			}
		}

		// Token: 0x06003400 RID: 13312 RVA: 0x000D93B4 File Offset: 0x000D75B4
		private void OnHeroComesOfAge(Hero hero)
		{
			if (this.CanBannerBeGivenToHero(hero))
			{
				ItemObject randomBannerItemForHero = BannerHelper.GetRandomBannerItemForHero(hero);
				if (randomBannerItemForHero != null)
				{
					hero.BannerItem = new EquipmentElement(randomBannerItemForHero, null, null, false);
				}
			}
		}

		// Token: 0x06003401 RID: 13313 RVA: 0x000D93E4 File Offset: 0x000D75E4
		private void OnHeroCreated(Hero hero, bool isBornNaturally = false)
		{
			if (this.CanBannerBeGivenToHero(hero))
			{
				ItemObject randomBannerItemForHero = BannerHelper.GetRandomBannerItemForHero(hero);
				if (randomBannerItemForHero != null)
				{
					hero.BannerItem = new EquipmentElement(randomBannerItemForHero, null, null, false);
				}
			}
		}

		// Token: 0x06003402 RID: 13314 RVA: 0x000D9414 File Offset: 0x000D7614
		private void OnCompanionClanCreated(Clan clan)
		{
			Hero leader = clan.Leader;
			if (leader.BannerItem.IsInvalid())
			{
				ItemObject randomBannerItemForHero = BannerHelper.GetRandomBannerItemForHero(leader);
				if (randomBannerItemForHero != null)
				{
					leader.BannerItem = new EquipmentElement(randomBannerItemForHero, null, null, false);
				}
			}
		}

		// Token: 0x06003403 RID: 13315 RVA: 0x000D9454 File Offset: 0x000D7654
		private bool CanBannerBeLootedFromHero(Hero hero)
		{
			return !this._heroNextBannerLootTime.ContainsKey(hero) || this._heroNextBannerLootTime[hero].IsPast;
		}

		// Token: 0x06003404 RID: 13316 RVA: 0x000D9485 File Offset: 0x000D7685
		private int GetCooldownDays(int bannerLevel)
		{
			if (bannerLevel == 1)
			{
				return 4;
			}
			if (bannerLevel == 1)
			{
				return 8;
			}
			return 12;
		}

		// Token: 0x06003405 RID: 13317 RVA: 0x000D9498 File Offset: 0x000D7698
		private void LogBannerLootForHero(Hero hero, int bannerLevel)
		{
			CampaignTime campaignTime = CampaignTime.DaysFromNow((float)this.GetCooldownDays(bannerLevel));
			if (!this._heroNextBannerLootTime.ContainsKey(hero))
			{
				this._heroNextBannerLootTime.Add(hero, campaignTime);
				return;
			}
			this._heroNextBannerLootTime[hero] = campaignTime;
		}

		// Token: 0x06003406 RID: 13318 RVA: 0x000D94DC File Offset: 0x000D76DC
		private ItemObject GetBannerRewardForHideoutBattle()
		{
			if (MBRandom.RandomFloat <= Campaign.Current.Models.BattleRewardModel.DestroyHideoutBannerLootChance)
			{
				return Campaign.Current.Models.BannerItemModel.GetPossibleRewardBannerItems().ToMBList<ItemObject>().GetRandomElementWithPredicate((ItemObject i) => ((BannerComponent)i.ItemComponent).BannerLevel == 1 && (i.Culture == null || i.Culture.StringId == "neutral_culture"));
			}
			return null;
		}

		// Token: 0x06003407 RID: 13319 RVA: 0x000D9544 File Offset: 0x000D7744
		private ItemObject GetBannerRewardForCapturingFortification(Settlement settlement)
		{
			if (MBRandom.RandomFloat <= Campaign.Current.Models.BattleRewardModel.CaptureSettlementBannerLootChance)
			{
				MBList<ItemObject> mblist = Campaign.Current.Models.BannerItemModel.GetPossibleRewardBannerItems().ToMBList<ItemObject>();
				mblist.Shuffle<ItemObject>();
				int wallLevel = settlement.Town.GetWallLevel();
				foreach (ItemObject itemObject in mblist)
				{
					if (((BannerComponent)itemObject.ItemComponent).BannerLevel == wallLevel && (itemObject.Culture == null || itemObject.Culture.StringId == "neutral_culture" || itemObject.Culture == settlement.Culture))
					{
						return itemObject;
					}
				}
			}
			return null;
		}

		// Token: 0x06003408 RID: 13320 RVA: 0x000D961C File Offset: 0x000D781C
		private ItemObject GetBannerRewardForDefeatingNoble(MapEvent mapEvent, MBList<TroopRosterElement> lootedCasualties)
		{
			Hero hero = null;
			foreach (MapEventParty mapEventParty in mapEvent.PartiesOnSide(mapEvent.DefeatedSide))
			{
				if (mapEventParty.Party.IsMobile && mapEventParty.Party.MobileParty.Army != null)
				{
					hero = mapEventParty.Party.MobileParty.Army.ArmyOwner;
					if (hero.BannerItem.IsInvalid())
					{
						hero = null;
						break;
					}
					break;
				}
			}
			if (hero == null)
			{
				CharacterObject character = lootedCasualties.GetRandomElementWithPredicate((TroopRosterElement t) => t.Character.IsHero && !t.Character.HeroObject.BannerItem.IsInvalid()).Character;
				hero = ((character != null) ? character.HeroObject : null);
			}
			if (hero != null && this.CanBannerBeLootedFromHero(hero))
			{
				Clan clan = hero.Clan;
				Hero hero2;
				if (clan == null)
				{
					hero2 = null;
				}
				else
				{
					Kingdom kingdom = clan.Kingdom;
					hero2 = ((kingdom != null) ? kingdom.RulingClan.Leader : null);
				}
				float num;
				if (hero2 == hero)
				{
					num = Campaign.Current.Models.BattleRewardModel.DefeatKingdomRulerBannerLootChance;
				}
				else
				{
					Clan clan2 = hero.Clan;
					if (((clan2 != null) ? clan2.Leader : null) == hero)
					{
						num = Campaign.Current.Models.BattleRewardModel.DefeatClanLeaderBannerLootChance;
					}
					else
					{
						num = Campaign.Current.Models.BattleRewardModel.DefeatRegularHeroBannerLootChance;
					}
				}
				if (MBRandom.RandomFloat <= num)
				{
					this.LogBannerLootForHero(hero, ((BannerComponent)hero.BannerItem.Item.ItemComponent).BannerLevel);
					return hero.BannerItem.Item;
				}
			}
			return null;
		}

		// Token: 0x06003409 RID: 13321 RVA: 0x000D97C4 File Offset: 0x000D79C4
		private bool CanBannerBeGivenToHero(Hero hero)
		{
			int heroComesOfAge = Campaign.Current.Models.AgeModel.HeroComesOfAge;
			return hero.Occupation == Occupation.Lord && hero.Age >= (float)heroComesOfAge && hero.BannerItem.IsInvalid() && hero.Clan != Clan.PlayerClan;
		}

		// Token: 0x040010FF RID: 4351
		private const int BannerLevel1CooldownDays = 4;

		// Token: 0x04001100 RID: 4352
		private const int BannerLevel2CooldownDays = 8;

		// Token: 0x04001101 RID: 4353
		private const int BannerLevel3CooldownDays = 12;

		// Token: 0x04001102 RID: 4354
		private const float BannerItemUpdateChance = 0.1f;

		// Token: 0x04001103 RID: 4355
		private Dictionary<Hero, CampaignTime> _heroNextBannerLootTime = new Dictionary<Hero, CampaignTime>();
	}
}
