using System;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.GameComponents
{
	// Token: 0x0200013D RID: 317
	public class DefaultSettlementValueModel : SettlementValueModel
	{
		// Token: 0x06001787 RID: 6023 RVA: 0x0007470C File Offset: 0x0007290C
		public override float CalculateSettlementBaseValue(Settlement settlement)
		{
			float num = (settlement.IsCastle ? 1.25f : 1f);
			float value = settlement.GetValue(null, true);
			Settlement settlement2 = (settlement.IsVillage ? settlement.Village.Bound : settlement);
			float baseGeographicalAdvantage = this.GetBaseGeographicalAdvantage(settlement2);
			return num * value * baseGeographicalAdvantage * 0.33f;
		}

		// Token: 0x06001788 RID: 6024 RVA: 0x00074760 File Offset: 0x00072960
		public override float CalculateSettlementValueForFaction(Settlement settlement, IFaction faction)
		{
			float num = (settlement.IsCastle ? 1.25f : 1f);
			float num2 = ((settlement.MapFaction == faction.MapFaction) ? 1.1f : 1f);
			float num3 = ((settlement.Culture == ((faction != null) ? faction.Culture : null)) ? 1.1f : 1f);
			float value = settlement.GetValue(null, true);
			Settlement settlement2 = (settlement.IsVillage ? settlement.Village.Bound : settlement);
			float num4 = this.GeographicalAdvantageForFaction(settlement2, faction);
			return value * num * num2 * num3 * num4 * 0.33f;
		}

		// Token: 0x06001789 RID: 6025 RVA: 0x000747F8 File Offset: 0x000729F8
		public override float CalculateSettlementValueForEnemyHero(Settlement settlement, Hero hero)
		{
			float num = (settlement.IsCastle ? 1.25f : 1f);
			float num2 = ((settlement.OwnerClan == hero.Clan) ? 1.1f : 1f);
			float num3 = ((settlement.Culture == hero.Culture) ? 1.1f : 1f);
			float value = settlement.GetValue(null, true);
			Settlement settlement2 = (settlement.IsVillage ? settlement.Village.Bound : settlement);
			float num4 = this.GeographicalAdvantageForFaction(settlement2, hero.MapFaction);
			return value * num * num3 * num2 * num4 * 0.33f;
		}

		// Token: 0x0600178A RID: 6026 RVA: 0x00074890 File Offset: 0x00072A90
		private float GetBaseGeographicalAdvantage(Settlement settlement)
		{
			float num = Campaign.Current.Models.MapDistanceModel.GetDistance(settlement.MapFaction.FactionMidSettlement, settlement) / Campaign.AverageDistanceBetweenTwoFortifications;
			return 1f / (1f + num);
		}

		// Token: 0x0600178B RID: 6027 RVA: 0x000748D4 File Offset: 0x00072AD4
		private float GeographicalAdvantageForFaction(Settlement settlement, IFaction faction)
		{
			Settlement factionMidSettlement = faction.FactionMidSettlement;
			float distance = Campaign.Current.Models.MapDistanceModel.GetDistance(settlement, factionMidSettlement);
			float distanceToClosestNonAllyFortification = faction.DistanceToClosestNonAllyFortification;
			if (faction.FactionMidSettlement.MapFaction != faction)
			{
				return MathF.Clamp(Campaign.AverageDistanceBetweenTwoFortifications / (distance + 0.1f), 0f, 4f);
			}
			if (settlement.MapFaction == faction && distance < distanceToClosestNonAllyFortification)
			{
				return MathF.Clamp(Campaign.AverageDistanceBetweenTwoFortifications / (distanceToClosestNonAllyFortification - distance), 1f, 4f);
			}
			float num = (distance - distanceToClosestNonAllyFortification) / Campaign.AverageDistanceBetweenTwoFortifications;
			return 1f / (1f + num);
		}

		// Token: 0x04000863 RID: 2147
		private const float BenefitRatioForFaction = 0.33f;

		// Token: 0x04000864 RID: 2148
		private const float CastleMultiplier = 1.25f;

		// Token: 0x04000865 RID: 2149
		private const float SameMapFactionMultiplier = 1.1f;

		// Token: 0x04000866 RID: 2150
		private const float SameCultureMultiplier = 1.1f;

		// Token: 0x04000867 RID: 2151
		private const float BeingOwnerMultiplier = 1.1f;
	}
}
