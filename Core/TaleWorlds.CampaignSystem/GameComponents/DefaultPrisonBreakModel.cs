using System;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.GameComponents
{
	public class DefaultPrisonBreakModel : PrisonBreakModel
	{
		public override bool CanPlayerStagePrisonBreak(Settlement settlement)
		{
			bool flag = false;
			if (settlement.IsFortification)
			{
				MobileParty garrisonParty = settlement.Town.GarrisonParty;
				bool flag2 = (garrisonParty != null && garrisonParty.PrisonRoster.TotalHeroes > 0) || settlement.Party.PrisonRoster.TotalHeroes > 0;
				flag = settlement.MapFaction != Clan.PlayerClan.MapFaction && !FactionManager.IsAlliedWithFaction(settlement.MapFaction, Clan.PlayerClan.MapFaction) && flag2;
			}
			return flag;
		}

		public override int GetPrisonBreakStartCost(Hero prisonerHero)
		{
			int num = MathF.Ceiling((float)Campaign.Current.Models.RansomValueCalculationModel.PrisonerRansomValue(prisonerHero.CharacterObject, null) / 2000f * prisonerHero.CurrentSettlement.Town.Security * 35f - (float)(Hero.MainHero.GetSkillValue(DefaultSkills.Roguery) * 10));
			num = ((num < 100) ? 0 : (num / 100 * 100));
			return num + 1000;
		}

		public override int GetRelationRewardOnPrisonBreak(Hero prisonerHero)
		{
			return 15;
		}

		public override float GetRogueryRewardOnPrisonBreak(Hero prisonerHero, bool isSuccess)
		{
			return (float)(isSuccess ? MBRandom.RandomInt(3500, 6000) : MBRandom.RandomInt(1000, 2500));
		}

		private const int BasePrisonBreakCost = 1000;
	}
}
