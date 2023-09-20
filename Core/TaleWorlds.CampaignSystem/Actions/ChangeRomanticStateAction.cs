using System;

namespace TaleWorlds.CampaignSystem.Actions
{
	// Token: 0x02000433 RID: 1075
	public static class ChangeRomanticStateAction
	{
		// Token: 0x06003EC1 RID: 16065 RVA: 0x0012C0DC File Offset: 0x0012A2DC
		private static void ApplyInternal(Hero hero1, Hero hero2, Romance.RomanceLevelEnum toWhat)
		{
			Romance.SetRomanticState(hero1, hero2, toWhat);
			CampaignEventDispatcher.Instance.OnRomanticStateChanged(hero1, hero2, toWhat);
		}

		// Token: 0x06003EC2 RID: 16066 RVA: 0x0012C0F3 File Offset: 0x0012A2F3
		public static void Apply(Hero person1, Hero person2, Romance.RomanceLevelEnum toWhat)
		{
			ChangeRomanticStateAction.ApplyInternal(person1, person2, toWhat);
		}
	}
}
