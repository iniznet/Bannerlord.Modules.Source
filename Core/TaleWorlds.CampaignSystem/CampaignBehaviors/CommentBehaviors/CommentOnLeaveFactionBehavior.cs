using System;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.LogEntries;

namespace TaleWorlds.CampaignSystem.CampaignBehaviors.CommentBehaviors
{
	// Token: 0x020003F2 RID: 1010
	public class CommentOnLeaveFactionBehavior : CampaignBehaviorBase
	{
		// Token: 0x06003CE7 RID: 15591 RVA: 0x00121B6E File Offset: 0x0011FD6E
		public override void RegisterEvents()
		{
			CampaignEvents.ClanChangedKingdom.AddNonSerializedListener(this, new Action<Clan, Kingdom, Kingdom, ChangeKingdomAction.ChangeKingdomActionDetail, bool>(this.OnClanLeaveKingdom));
		}

		// Token: 0x06003CE8 RID: 15592 RVA: 0x00121B87 File Offset: 0x0011FD87
		public override void SyncData(IDataStore dataStore)
		{
		}

		// Token: 0x06003CE9 RID: 15593 RVA: 0x00121B89 File Offset: 0x0011FD89
		private void OnClanLeaveKingdom(Clan clan, Kingdom oldKingdom, Kingdom newKingdom, ChangeKingdomAction.ChangeKingdomActionDetail detail, bool showNotification)
		{
			LogEntry.AddLogEntry(new ClanChangeKingdomLogEntry(clan, oldKingdom, newKingdom, detail == ChangeKingdomAction.ChangeKingdomActionDetail.LeaveWithRebellion));
		}
	}
}
