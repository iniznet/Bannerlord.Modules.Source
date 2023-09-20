using System;
using TaleWorlds.CampaignSystem.LogEntries;

namespace TaleWorlds.CampaignSystem.CampaignBehaviors.CommentBehaviors
{
	// Token: 0x020003EC RID: 1004
	public class CommentOnClanLeaderChangedBehavior : CampaignBehaviorBase
	{
		// Token: 0x06003CCF RID: 15567 RVA: 0x00121998 File Offset: 0x0011FB98
		public override void RegisterEvents()
		{
			CampaignEvents.OnClanLeaderChangedEvent.AddNonSerializedListener(this, new Action<Hero, Hero>(CommentOnClanLeaderChangedBehavior.OnClanLeaderChanged));
		}

		// Token: 0x06003CD0 RID: 15568 RVA: 0x001219B1 File Offset: 0x0011FBB1
		private static void OnClanLeaderChanged(Hero oldLeader, Hero newLeader)
		{
			LogEntry.AddLogEntry(new ClanLeaderChangedLogEntry(oldLeader, newLeader));
		}

		// Token: 0x06003CD1 RID: 15569 RVA: 0x001219BF File Offset: 0x0011FBBF
		public override void SyncData(IDataStore dataStore)
		{
		}
	}
}
