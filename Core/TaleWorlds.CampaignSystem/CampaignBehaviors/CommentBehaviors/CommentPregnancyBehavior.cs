using System;
using TaleWorlds.CampaignSystem.LogEntries;

namespace TaleWorlds.CampaignSystem.CampaignBehaviors.CommentBehaviors
{
	// Token: 0x020003F5 RID: 1013
	public class CommentPregnancyBehavior : CampaignBehaviorBase
	{
		// Token: 0x06003CF3 RID: 15603 RVA: 0x00121C67 File Offset: 0x0011FE67
		public override void RegisterEvents()
		{
			CampaignEvents.OnChildConceivedEvent.AddNonSerializedListener(this, new Action<Hero>(this.OnChildConceived));
		}

		// Token: 0x06003CF4 RID: 15604 RVA: 0x00121C80 File Offset: 0x0011FE80
		private void OnChildConceived(Hero mother)
		{
			LogEntry.AddLogEntry(new PregnancyLogEntry(mother));
		}

		// Token: 0x06003CF5 RID: 15605 RVA: 0x00121C8D File Offset: 0x0011FE8D
		public override void SyncData(IDataStore dataStore)
		{
		}
	}
}
