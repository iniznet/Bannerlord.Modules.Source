using System;
using TaleWorlds.CampaignSystem.LogEntries;

namespace TaleWorlds.CampaignSystem.CampaignBehaviors.CommentBehaviors
{
	// Token: 0x020003F1 RID: 1009
	public class CommentOnKingdomDestroyedBehavior : CampaignBehaviorBase
	{
		// Token: 0x06003CE3 RID: 15587 RVA: 0x00121B3E File Offset: 0x0011FD3E
		public override void RegisterEvents()
		{
			CampaignEvents.KingdomDestroyedEvent.AddNonSerializedListener(this, new Action<Kingdom>(this.OnKingdomDestroyed));
		}

		// Token: 0x06003CE4 RID: 15588 RVA: 0x00121B57 File Offset: 0x0011FD57
		public override void SyncData(IDataStore dataStore)
		{
		}

		// Token: 0x06003CE5 RID: 15589 RVA: 0x00121B59 File Offset: 0x0011FD59
		private void OnKingdomDestroyed(Kingdom destroyedKingdom)
		{
			LogEntry.AddLogEntry(new KingdomDestroyedLogEntry(destroyedKingdom));
		}
	}
}
