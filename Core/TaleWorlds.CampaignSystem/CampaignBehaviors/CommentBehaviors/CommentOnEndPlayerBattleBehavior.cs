using System;
using TaleWorlds.CampaignSystem.LogEntries;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.Core;

namespace TaleWorlds.CampaignSystem.CampaignBehaviors.CommentBehaviors
{
	// Token: 0x020003F0 RID: 1008
	public class CommentOnEndPlayerBattleBehavior : CampaignBehaviorBase
	{
		// Token: 0x06003CDF RID: 15583 RVA: 0x00121AFE File Offset: 0x0011FCFE
		public override void RegisterEvents()
		{
			CampaignEvents.OnPlayerBattleEndEvent.AddNonSerializedListener(this, new Action<MapEvent>(this.OnPlayerBattleEnded));
		}

		// Token: 0x06003CE0 RID: 15584 RVA: 0x00121B17 File Offset: 0x0011FD17
		public override void SyncData(IDataStore dataStore)
		{
		}

		// Token: 0x06003CE1 RID: 15585 RVA: 0x00121B19 File Offset: 0x0011FD19
		private void OnPlayerBattleEnded(MapEvent mapEvent)
		{
			if (!mapEvent.IsHideoutBattle || mapEvent.BattleState != BattleState.None)
			{
				LogEntry.AddLogEntry(new PlayerBattleEndedLogEntry(mapEvent));
			}
		}
	}
}
