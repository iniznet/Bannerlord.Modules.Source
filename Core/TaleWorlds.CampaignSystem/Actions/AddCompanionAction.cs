using System;

namespace TaleWorlds.CampaignSystem.Actions
{
	// Token: 0x02000422 RID: 1058
	public static class AddCompanionAction
	{
		// Token: 0x06003E80 RID: 16000 RVA: 0x0012A34C File Offset: 0x0012854C
		private static void ApplyInternal(Clan clan, Hero companion)
		{
			if (companion.CompanionOf != null)
			{
				RemoveCompanionAction.ApplyByFire(companion.CompanionOf, companion);
			}
			companion.CompanionOf = clan;
			CampaignEventDispatcher.Instance.OnNewCompanionAdded(companion);
		}

		// Token: 0x06003E81 RID: 16001 RVA: 0x0012A374 File Offset: 0x00128574
		public static void Apply(Clan clan, Hero companion)
		{
			AddCompanionAction.ApplyInternal(clan, companion);
		}
	}
}
