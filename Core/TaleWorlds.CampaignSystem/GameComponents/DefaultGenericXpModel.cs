using System;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.ComponentInterfaces;

namespace TaleWorlds.CampaignSystem.GameComponents
{
	// Token: 0x0200010D RID: 269
	public class DefaultGenericXpModel : GenericXpModel
	{
		// Token: 0x060015B0 RID: 5552 RVA: 0x00066886 File Offset: 0x00064A86
		public override float GetXpMultiplier(Hero hero)
		{
			if (hero.IsPlayerCompanion && Hero.MainHero.GetPerkValue(DefaultPerks.Charm.NaturalLeader))
			{
				return 1.2f;
			}
			return 1f;
		}
	}
}
