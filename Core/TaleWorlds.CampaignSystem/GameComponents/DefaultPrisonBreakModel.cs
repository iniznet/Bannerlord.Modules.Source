using System;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.GameComponents
{
	// Token: 0x0200012E RID: 302
	public class DefaultPrisonBreakModel : PrisonBreakModel
	{
		// Token: 0x060016D2 RID: 5842 RVA: 0x0006FFD4 File Offset: 0x0006E1D4
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

		// Token: 0x060016D3 RID: 5843 RVA: 0x00070054 File Offset: 0x0006E254
		public override int GetPrisonBreakStartCost(Hero prisonerHero)
		{
			int num = MathF.Ceiling((float)Campaign.Current.Models.RansomValueCalculationModel.PrisonerRansomValue(prisonerHero.CharacterObject, null) / 2000f * prisonerHero.CurrentSettlement.Town.Security * 35f - (float)(Hero.MainHero.GetSkillValue(DefaultSkills.Roguery) * 10));
			num = ((num < 100) ? 0 : (num / 100 * 100));
			return num + 1000;
		}

		// Token: 0x060016D4 RID: 5844 RVA: 0x000700CB File Offset: 0x0006E2CB
		public override int GetRelationRewardOnPrisonBreak(Hero prisonerHero)
		{
			return 15;
		}

		// Token: 0x060016D5 RID: 5845 RVA: 0x000700CF File Offset: 0x0006E2CF
		public override float GetRogueryRewardOnPrisonBreak(Hero prisonerHero, bool isSuccess)
		{
			return (float)(isSuccess ? MBRandom.RandomInt(3500, 6000) : MBRandom.RandomInt(1000, 2500));
		}

		// Token: 0x04000819 RID: 2073
		private const int BasePrisonBreakCost = 1000;
	}
}
