using System;
using TaleWorlds.CampaignSystem.LogEntries;

namespace TaleWorlds.CampaignSystem.CampaignBehaviors.CommentBehaviors
{
	// Token: 0x020003EB RID: 1003
	public class CommentOnClanDestroyedBehavior : CampaignBehaviorBase
	{
		// Token: 0x06003CCB RID: 15563 RVA: 0x00121968 File Offset: 0x0011FB68
		public override void RegisterEvents()
		{
			CampaignEvents.OnClanDestroyedEvent.AddNonSerializedListener(this, new Action<Clan>(this.OnClanDestroyed));
		}

		// Token: 0x06003CCC RID: 15564 RVA: 0x00121981 File Offset: 0x0011FB81
		public override void SyncData(IDataStore dataStore)
		{
		}

		// Token: 0x06003CCD RID: 15565 RVA: 0x00121983 File Offset: 0x0011FB83
		private void OnClanDestroyed(Clan destroyedClan)
		{
			LogEntry.AddLogEntry(new ClanDestroyedLogEntry(destroyedClan));
		}
	}
}
