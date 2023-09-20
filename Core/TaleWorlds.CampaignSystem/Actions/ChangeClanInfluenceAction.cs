using System;

namespace TaleWorlds.CampaignSystem.Actions
{
	// Token: 0x02000429 RID: 1065
	public static class ChangeClanInfluenceAction
	{
		// Token: 0x06003E95 RID: 16021 RVA: 0x0012AF33 File Offset: 0x00129133
		private static void ApplyInternal(Clan clan, float amount)
		{
			clan.Influence += amount;
			CampaignEventDispatcher.Instance.OnClanInfluenceChanged(clan, amount);
		}

		// Token: 0x06003E96 RID: 16022 RVA: 0x0012AF4F File Offset: 0x0012914F
		public static void Apply(Clan clan, float amount)
		{
			ChangeClanInfluenceAction.ApplyInternal(clan, amount);
		}
	}
}
