using System;
using Helpers;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.Extensions;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.GameComponents
{
	// Token: 0x02000146 RID: 326
	public class DefaultTradeItemPriceFactorModel : TradeItemPriceFactorModel
	{
		// Token: 0x060017F3 RID: 6131 RVA: 0x000788D0 File Offset: 0x00076AD0
		public override float GetTradePenalty(ItemObject item, MobileParty clientParty, PartyBase merchant, bool isSelling, float inStore, float supply, float demand)
		{
			Settlement settlement = ((merchant != null) ? merchant.Settlement : null);
			float num = 0.06f;
			bool flag = clientParty != null && clientParty.IsCaravan;
			bool flag2;
			if (merchant == null)
			{
				flag2 = false;
			}
			else
			{
				MobileParty mobileParty = merchant.MobileParty;
				bool? flag3 = ((mobileParty != null) ? new bool?(mobileParty.IsCaravan) : null);
				bool flag4 = true;
				flag2 = (flag3.GetValueOrDefault() == flag4) & (flag3 != null);
			}
			if (clientParty != null && merchant != null && clientParty.MapFaction.IsAtWarWith(merchant.MapFaction))
			{
				num += 0.5f;
			}
			if (!item.IsTradeGood && !item.IsAnimal && !item.HasHorseComponent && !flag && isSelling)
			{
				float num2 = 1.5f + Math.Max(0f, item.Tierf - 1f) * 0.25f;
				if (item.IsCraftedWeapon && item.IsCraftedByPlayer && clientParty != null && clientParty.HasPerk(DefaultPerks.Crafting.ArtisanSmith, false))
				{
					num2 *= 0.5f;
				}
				num += num2;
			}
			if (item.HasHorseComponent && item.HorseComponent.IsPackAnimal && !flag && isSelling)
			{
				num += 0.8f;
			}
			if (item.HasHorseComponent && item.HorseComponent.IsMount && !flag && isSelling)
			{
				num += 0.8f;
			}
			if (settlement != null && settlement.IsVillage)
			{
				num += (isSelling ? 1f : 0.1f);
			}
			if (flag2)
			{
				if (item.ItemCategory == DefaultItemCategories.PackAnimal && !isSelling)
				{
					num += 2f;
				}
				num += (isSelling ? 1f : 0.1f);
			}
			bool flag5 = clientParty == null;
			if (flag)
			{
				num *= 0.5f;
			}
			else if (flag5)
			{
				num *= 0.2f;
			}
			float num3 = ((clientParty != null) ? Campaign.Current.Models.PartyTradeModel.GetTradePenaltyFactor(clientParty) : 1f);
			num *= num3;
			ExplainedNumber explainedNumber = new ExplainedNumber(num, false, null);
			if (clientParty != null)
			{
				if (settlement != null && clientParty.MapFaction == settlement.MapFaction)
				{
					if (settlement.IsVillage)
					{
						PerkHelper.AddPerkBonusForParty(DefaultPerks.Scouting.VillageNetwork, clientParty, true, ref explainedNumber);
					}
					else if (settlement.IsTown)
					{
						PerkHelper.AddPerkBonusForParty(DefaultPerks.Scouting.RumourNetwork, clientParty, true, ref explainedNumber);
					}
				}
				if (item.IsTradeGood)
				{
					if (clientParty.HasPerk(DefaultPerks.Trade.WholeSeller, false) && isSelling)
					{
						PerkHelper.AddPerkBonusForParty(DefaultPerks.Trade.WholeSeller, clientParty, true, ref explainedNumber);
					}
					if (isSelling && item.IsFood && clientParty.LeaderHero != null)
					{
						PerkHelper.AddPerkBonusForCharacter(DefaultPerks.Trade.GranaryAccountant, clientParty.LeaderHero.CharacterObject, true, ref explainedNumber);
					}
				}
				else if (!item.IsTradeGood && (clientParty.HasPerk(DefaultPerks.Trade.Appraiser, false) && isSelling))
				{
					PerkHelper.AddPerkBonusForParty(DefaultPerks.Trade.Appraiser, clientParty, true, ref explainedNumber);
				}
				if (PartyBaseHelper.HasFeat(clientParty.Party, DefaultCulturalFeats.AseraiTraderFeat))
				{
					explainedNumber.AddFactor(-0.1f, null);
				}
				if (item.WeaponComponent != null && isSelling)
				{
					PerkHelper.AddPerkBonusForParty(DefaultPerks.Roguery.ArmsDealer, clientParty, true, ref explainedNumber);
				}
				if (!isSelling && item.IsFood)
				{
					PerkHelper.AddPerkBonusForParty(DefaultPerks.Trade.InsurancePlans, clientParty, false, ref explainedNumber);
				}
				if (item.HorseComponent != null && item.HorseComponent.IsPackAnimal && clientParty.HasPerk(DefaultPerks.Steward.ArenicosMules, true))
				{
					PerkHelper.AddPerkBonusForParty(DefaultPerks.Steward.ArenicosMules, clientParty, false, ref explainedNumber);
				}
				if (isSelling && (item.ItemCategory == DefaultItemCategories.Pottery || item.ItemCategory == DefaultItemCategories.Tools || item.ItemCategory == DefaultItemCategories.Cotton || item.ItemCategory == DefaultItemCategories.Jewelry) && clientParty.LeaderHero != null)
				{
					PerkHelper.AddPerkBonusForCharacter(DefaultPerks.Trade.GranaryAccountant, clientParty.LeaderHero.CharacterObject, true, ref explainedNumber);
				}
				if (item.IsMountable)
				{
					if (clientParty.HasPerk(DefaultPerks.Riding.DeeperSacks, true))
					{
						explainedNumber.AddFactor(DefaultPerks.Riding.DeeperSacks.SecondaryBonus, DefaultPerks.Riding.DeeperSacks.Name);
					}
					if (clientParty.LeaderHero != null && clientParty.LeaderHero.GetPerkValue(DefaultPerks.Steward.ArenicosHorses))
					{
						PerkHelper.AddPerkBonusForCharacter(DefaultPerks.Steward.ArenicosHorses, clientParty.LeaderHero.CharacterObject, false, ref explainedNumber);
					}
				}
				if (clientParty.IsMainParty && Hero.MainHero.GetPerkValue(DefaultPerks.Roguery.SmugglerConnections) && ((merchant != null) ? merchant.MapFaction : null) != null && merchant.MapFaction.MainHeroCrimeRating > 0f)
				{
					PerkHelper.AddPerkBonusForParty(DefaultPerks.Roguery.SmugglerConnections, clientParty, false, ref explainedNumber);
				}
				if (!isSelling && merchant != null && merchant.IsSettlement && merchant.Settlement.IsVillage && clientParty.HasPerk(DefaultPerks.Trade.DistributedGoods, true))
				{
					PerkHelper.AddPerkBonusForParty(DefaultPerks.Trade.DistributedGoods, clientParty, false, ref explainedNumber);
				}
				if (isSelling && item.HasHorseComponent && clientParty.HasPerk(DefaultPerks.Trade.LocalConnection, true))
				{
					explainedNumber.AddFactor(DefaultPerks.Trade.LocalConnection.SecondaryBonus, DefaultPerks.Trade.LocalConnection.Name);
				}
				if (isSelling && (item.ItemCategory == DefaultItemCategories.Pottery || item.ItemCategory == DefaultItemCategories.Tools || item.ItemCategory == DefaultItemCategories.Jewelry || item.ItemCategory == DefaultItemCategories.Cotton) && clientParty.LeaderHero != null)
				{
					PerkHelper.AddPerkBonusForCharacter(DefaultPerks.Trade.TradeyardForeman, clientParty.LeaderHero.CharacterObject, true, ref explainedNumber);
				}
				if (!isSelling && (item.ItemCategory == DefaultItemCategories.Clay || item.ItemCategory == DefaultItemCategories.Iron || item.ItemCategory == DefaultItemCategories.Silver || item.ItemCategory == DefaultItemCategories.Cotton) && clientParty.HasPerk(DefaultPerks.Trade.RapidDevelopment, false))
				{
					PerkHelper.AddPerkBonusForParty(DefaultPerks.Trade.RapidDevelopment, clientParty, false, ref explainedNumber);
				}
			}
			return explainedNumber.ResultNumber;
		}

		// Token: 0x060017F4 RID: 6132 RVA: 0x00078E2C File Offset: 0x0007702C
		private float GetPriceFactor(ItemObject item, MobileParty tradingParty, PartyBase merchant, float inStoreValue, float supply, float demand, bool isSelling)
		{
			float basePriceFactor = this.GetBasePriceFactor(item.GetItemCategory(), inStoreValue, supply, demand, isSelling, item.Value);
			float tradePenalty = this.GetTradePenalty(item, tradingParty, merchant, isSelling, inStoreValue, supply, demand);
			if (!isSelling)
			{
				return basePriceFactor * (1f + tradePenalty);
			}
			return basePriceFactor * 1f / (1f + tradePenalty);
		}

		// Token: 0x060017F5 RID: 6133 RVA: 0x00078E84 File Offset: 0x00077084
		public override float GetBasePriceFactor(ItemCategory itemCategory, float inStoreValue, float supply, float demand, bool isSelling, int transferValue)
		{
			if (isSelling)
			{
				inStoreValue += (float)transferValue;
			}
			float num = MathF.Pow(demand / (0.1f * supply + inStoreValue * 0.04f + 2f), itemCategory.IsAnimal ? 0.3f : 0.6f);
			if (itemCategory.IsTradeGood)
			{
				return MathF.Clamp(num, 0.1f, 10f);
			}
			return MathF.Clamp(num, 0.8f, 1.3f);
		}

		// Token: 0x060017F6 RID: 6134 RVA: 0x00078EF8 File Offset: 0x000770F8
		public override int GetPrice(EquipmentElement itemRosterElement, MobileParty clientParty, PartyBase merchant, bool isSelling, float inStoreValue, float supply, float demand)
		{
			float priceFactor = this.GetPriceFactor(itemRosterElement.Item, clientParty, merchant, inStoreValue, supply, demand, isSelling);
			float num = (float)itemRosterElement.ItemValue * priceFactor;
			int num2 = (isSelling ? MathF.Floor(num) : MathF.Ceiling(num));
			if (!isSelling && ((merchant != null) ? merchant.MobileParty : null) != null && merchant.MobileParty.IsCaravan && clientParty.HasPerk(DefaultPerks.Trade.SilverTongue, true))
			{
				num2 = MathF.Ceiling((float)num2 * (1f + DefaultPerks.Trade.SilverTongue.SecondaryBonus));
			}
			return MathF.Max(num2, 1);
		}

		// Token: 0x060017F7 RID: 6135 RVA: 0x00078F88 File Offset: 0x00077188
		public override int GetTheoreticalMaxItemMarketValue(ItemObject item)
		{
			if (item.IsTradeGood || item.IsAnimal)
			{
				return MathF.Round((float)item.Value * 10f);
			}
			return MathF.Round((float)item.Value * 1.3f);
		}

		// Token: 0x04000879 RID: 2169
		private const float MinPriceFactor = 0.1f;

		// Token: 0x0400087A RID: 2170
		private const float MaxPriceFactor = 10f;

		// Token: 0x0400087B RID: 2171
		private const float MinPriceFactorNonTrade = 0.8f;

		// Token: 0x0400087C RID: 2172
		private const float MaxPriceFactorNonTrade = 1.3f;

		// Token: 0x0400087D RID: 2173
		private const float HighTradePenaltyBaseValue = 1.5f;

		// Token: 0x0400087E RID: 2174
		private const float PackAnimalTradePenalty = 0.8f;

		// Token: 0x0400087F RID: 2175
		private const float MountTradePenalty = 0.8f;
	}
}
