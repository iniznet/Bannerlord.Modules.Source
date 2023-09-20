using System;

namespace TaleWorlds.CampaignSystem.Overlay
{
	// Token: 0x020000C7 RID: 199
	public class GameOverlays
	{
		// Token: 0x020004DD RID: 1245
		public enum MapOverlayType
		{
			// Token: 0x04001508 RID: 5384
			None,
			// Token: 0x04001509 RID: 5385
			Army
		}

		// Token: 0x020004DE RID: 1246
		public enum MenuOverlayType
		{
			// Token: 0x0400150B RID: 5387
			None,
			// Token: 0x0400150C RID: 5388
			SettlementWithParties,
			// Token: 0x0400150D RID: 5389
			SettlementWithCharacters,
			// Token: 0x0400150E RID: 5390
			SettlementWithBoth,
			// Token: 0x0400150F RID: 5391
			Encounter
		}
	}
}
