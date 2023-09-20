using System;

namespace TaleWorlds.CampaignSystem.Actions
{
	// Token: 0x02000434 RID: 1076
	internal class ChangeRulingClanAction
	{
		// Token: 0x06003EC3 RID: 16067 RVA: 0x0012C0FD File Offset: 0x0012A2FD
		private static void ApplyInternal(Kingdom kingdom, Clan clan)
		{
			kingdom.RulingClan = clan;
			CampaignEventDispatcher.Instance.OnRulingClanChanged(kingdom, clan);
		}

		// Token: 0x06003EC4 RID: 16068 RVA: 0x0012C112 File Offset: 0x0012A312
		public static void Apply(Kingdom kingdom, Clan clan)
		{
			ChangeRulingClanAction.ApplyInternal(kingdom, clan);
		}
	}
}
