using System;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.GameComponents
{
	// Token: 0x020000F4 RID: 244
	public class DefaultBribeCalculationModel : BribeCalculationModel
	{
		// Token: 0x06001494 RID: 5268 RVA: 0x0005BB5C File Offset: 0x00059D5C
		public override bool IsBribeNotNeededToEnterKeep(Settlement settlement)
		{
			SettlementAccessModel.AccessDetails accessDetails;
			Campaign.Current.Models.SettlementAccessModel.CanMainHeroEnterLordsHall(settlement, out accessDetails);
			return accessDetails.AccessLevel != SettlementAccessModel.AccessLevel.LimitedAccess || (accessDetails.AccessLevel == SettlementAccessModel.AccessLevel.LimitedAccess && accessDetails.LimitedAccessSolution != SettlementAccessModel.LimitedAccessSolution.Bribe);
		}

		// Token: 0x06001495 RID: 5269 RVA: 0x0005BBA4 File Offset: 0x00059DA4
		public override bool IsBribeNotNeededToEnterDungeon(Settlement settlement)
		{
			SettlementAccessModel.AccessDetails accessDetails;
			Campaign.Current.Models.SettlementAccessModel.CanMainHeroEnterDungeon(settlement, out accessDetails);
			return accessDetails.AccessLevel != SettlementAccessModel.AccessLevel.LimitedAccess || (accessDetails.AccessLevel == SettlementAccessModel.AccessLevel.LimitedAccess && accessDetails.LimitedAccessSolution != SettlementAccessModel.LimitedAccessSolution.Bribe);
		}

		// Token: 0x06001496 RID: 5270 RVA: 0x0005BBEA File Offset: 0x00059DEA
		private float GetSkillFactor()
		{
			return (1f - (float)Hero.MainHero.GetSkillValue(DefaultSkills.Roguery) / 300f) * 0.65f + 0.35f;
		}

		// Token: 0x06001497 RID: 5271 RVA: 0x0005BC14 File Offset: 0x00059E14
		private int GetBribeForCriminalRating(IFaction faction)
		{
			return MathF.Round(Campaign.Current.Models.CrimeModel.GetCost(faction, CrimeModel.PaymentMethod.Gold, 0f)) / 5;
		}

		// Token: 0x06001498 RID: 5272 RVA: 0x0005BC38 File Offset: 0x00059E38
		private int GetBaseBribeValue(IFaction faction)
		{
			if (faction.IsAtWarWith(Clan.PlayerClan))
			{
				return 5000;
			}
			if (faction.IsAtWarWith(Hero.MainHero.MapFaction))
			{
				return 3000;
			}
			if (FactionManager.IsNeutralWithFaction(faction, Clan.PlayerClan))
			{
				return 100;
			}
			if (Hero.MainHero.Clan == faction)
			{
				return 0;
			}
			if (Hero.MainHero.MapFaction == faction)
			{
				return 0;
			}
			if (faction is Clan)
			{
				IFaction mapFaction = Hero.MainHero.MapFaction;
				Kingdom kingdom = (faction as Clan).Kingdom;
				return 0;
			}
			return 0;
		}

		// Token: 0x06001499 RID: 5273 RVA: 0x0005BCC0 File Offset: 0x00059EC0
		public override int GetBribeToEnterLordsHall(Settlement settlement)
		{
			if (this.IsBribeNotNeededToEnterKeep(settlement))
			{
				return 0;
			}
			return this.GetBribeInternal(settlement);
		}

		// Token: 0x0600149A RID: 5274 RVA: 0x0005BCD4 File Offset: 0x00059ED4
		public override int GetBribeToEnterDungeon(Settlement settlement)
		{
			return this.GetBribeToEnterLordsHall(settlement);
		}

		// Token: 0x0600149B RID: 5275 RVA: 0x0005BCE0 File Offset: 0x00059EE0
		private int GetBribeInternal(Settlement settlement)
		{
			int num = this.GetBaseBribeValue(settlement.MapFaction);
			num += this.GetBribeForCriminalRating(settlement.MapFaction);
			if (Clan.PlayerClan.Renown < 500f)
			{
				num += (500 - (int)Clan.PlayerClan.Renown) * 15 / 10;
				num = MathF.Max(num, 50);
			}
			num = (int)((float)num * this.GetSkillFactor() / 25f) * 25;
			return MathF.Max(num - settlement.BribePaid, 0);
		}
	}
}
