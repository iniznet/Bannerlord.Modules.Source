using System;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.Party;

namespace TaleWorlds.CampaignSystem.GameComponents
{
	// Token: 0x02000149 RID: 329
	public class DefaultValuationModel : ValuationModel
	{
		// Token: 0x06001802 RID: 6146 RVA: 0x000797F3 File Offset: 0x000779F3
		public override float GetMilitaryValueOfParty(MobileParty party)
		{
			return party.Party.TotalStrength * 15f;
		}

		// Token: 0x06001803 RID: 6147 RVA: 0x00079806 File Offset: 0x00077A06
		public override float GetValueOfTroop(CharacterObject troop)
		{
			return troop.GetPower() * 15f;
		}

		// Token: 0x06001804 RID: 6148 RVA: 0x00079814 File Offset: 0x00077A14
		public override float GetValueOfHero(Hero hero)
		{
			if (hero.Clan != null)
			{
				return ((float)hero.Clan.Gold * 0.15f + (float)((1 + hero.Clan.Tier * hero.Clan.Tier) * 500)) * ((hero.Clan.Leader == hero) ? 4f : 1f);
			}
			return 500f;
		}
	}
}
