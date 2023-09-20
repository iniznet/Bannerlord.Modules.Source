using System;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.GameComponents
{
	public class DefaultBribeCalculationModel : BribeCalculationModel
	{
		public override bool IsBribeNotNeededToEnterKeep(Settlement settlement)
		{
			SettlementAccessModel.AccessDetails accessDetails;
			Campaign.Current.Models.SettlementAccessModel.CanMainHeroEnterLordsHall(settlement, out accessDetails);
			return accessDetails.AccessLevel != SettlementAccessModel.AccessLevel.LimitedAccess || (accessDetails.AccessLevel == SettlementAccessModel.AccessLevel.LimitedAccess && accessDetails.LimitedAccessSolution != SettlementAccessModel.LimitedAccessSolution.Bribe);
		}

		public override bool IsBribeNotNeededToEnterDungeon(Settlement settlement)
		{
			SettlementAccessModel.AccessDetails accessDetails;
			Campaign.Current.Models.SettlementAccessModel.CanMainHeroEnterDungeon(settlement, out accessDetails);
			return accessDetails.AccessLevel != SettlementAccessModel.AccessLevel.LimitedAccess || (accessDetails.AccessLevel == SettlementAccessModel.AccessLevel.LimitedAccess && accessDetails.LimitedAccessSolution != SettlementAccessModel.LimitedAccessSolution.Bribe);
		}

		private float GetSkillFactor()
		{
			return (1f - (float)Hero.MainHero.GetSkillValue(DefaultSkills.Roguery) / 300f) * 0.65f + 0.35f;
		}

		private int GetBribeForCriminalRating(IFaction faction)
		{
			return MathF.Round(Campaign.Current.Models.CrimeModel.GetCost(faction, CrimeModel.PaymentMethod.Gold, 0f)) / 5;
		}

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

		public override int GetBribeToEnterLordsHall(Settlement settlement)
		{
			if (this.IsBribeNotNeededToEnterKeep(settlement))
			{
				return 0;
			}
			return this.GetBribeInternal(settlement);
		}

		public override int GetBribeToEnterDungeon(Settlement settlement)
		{
			return this.GetBribeToEnterLordsHall(settlement);
		}

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
