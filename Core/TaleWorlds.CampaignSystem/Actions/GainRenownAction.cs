using System;

namespace TaleWorlds.CampaignSystem.Actions
{
	// Token: 0x02000442 RID: 1090
	public static class GainRenownAction
	{
		// Token: 0x06003F08 RID: 16136 RVA: 0x0012D03B File Offset: 0x0012B23B
		private static void ApplyInternal(Hero hero, float gainedRenown, bool doNotNotify)
		{
			if (gainedRenown > 0f)
			{
				hero.Clan.AddRenown(gainedRenown, true);
				CampaignEventDispatcher.Instance.OnRenownGained(hero, (int)gainedRenown, doNotNotify);
			}
		}

		// Token: 0x06003F09 RID: 16137 RVA: 0x0012D060 File Offset: 0x0012B260
		public static void Apply(Hero hero, float renownValue, bool doNotNotify = false)
		{
			GainRenownAction.ApplyInternal(hero, renownValue, doNotNotify);
		}
	}
}
