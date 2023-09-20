using System;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.LogEntries;
using TaleWorlds.CampaignSystem.MapNotificationTypes;

namespace TaleWorlds.CampaignSystem.CampaignBehaviors.CommentBehaviors
{
	// Token: 0x020003ED RID: 1005
	public class CommentOnDeclareWarBehavior : CampaignBehaviorBase
	{
		// Token: 0x06003CD3 RID: 15571 RVA: 0x001219C9 File Offset: 0x0011FBC9
		public override void RegisterEvents()
		{
			CampaignEvents.WarDeclared.AddNonSerializedListener(this, new Action<IFaction, IFaction, DeclareWarAction.DeclareWarDetail>(this.OnWarDeclared));
		}

		// Token: 0x06003CD4 RID: 15572 RVA: 0x001219E2 File Offset: 0x0011FBE2
		public override void SyncData(IDataStore dataStore)
		{
		}

		// Token: 0x06003CD5 RID: 15573 RVA: 0x001219E4 File Offset: 0x0011FBE4
		private void OnWarDeclared(IFaction faction1, IFaction faction2, DeclareWarAction.DeclareWarDetail detail)
		{
			DeclareWarLogEntry declareWarLogEntry = new DeclareWarLogEntry(faction1, faction2);
			LogEntry.AddLogEntry(declareWarLogEntry);
			if (faction2 == Hero.MainHero.MapFaction || (faction1 == Hero.MainHero.MapFaction && detail != DeclareWarAction.DeclareWarDetail.CausedByKingdomDecision))
			{
				Campaign.Current.CampaignInformationManager.NewMapNoticeAdded(new WarMapNotification(faction1, faction2, declareWarLogEntry.GetEncyclopediaText()));
			}
		}
	}
}
