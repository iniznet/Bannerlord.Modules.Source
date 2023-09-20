using System;

namespace TaleWorlds.CampaignSystem.Actions
{
	// Token: 0x02000424 RID: 1060
	public static class AdoptHeroAction
	{
		// Token: 0x06003E84 RID: 16004 RVA: 0x0012A46B File Offset: 0x0012866B
		private static void ApplyInternal(Hero adoptedHero)
		{
			if (Hero.MainHero.IsFemale)
			{
				adoptedHero.Mother = Hero.MainHero;
			}
			else
			{
				adoptedHero.Father = Hero.MainHero;
			}
			adoptedHero.Clan = Clan.PlayerClan;
		}

		// Token: 0x06003E85 RID: 16005 RVA: 0x0012A49C File Offset: 0x0012869C
		public static void Apply(Hero adoptedHero)
		{
			AdoptHeroAction.ApplyInternal(adoptedHero);
		}
	}
}
