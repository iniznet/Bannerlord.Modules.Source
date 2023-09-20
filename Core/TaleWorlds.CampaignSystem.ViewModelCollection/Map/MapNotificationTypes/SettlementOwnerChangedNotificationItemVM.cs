using System;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.MapNotificationTypes;
using TaleWorlds.CampaignSystem.Settlements;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.Map.MapNotificationTypes
{
	// Token: 0x02000047 RID: 71
	public class SettlementOwnerChangedNotificationItemVM : MapNotificationItemBaseVM
	{
		// Token: 0x0600055F RID: 1375 RVA: 0x0001AEBC File Offset: 0x000190BC
		public SettlementOwnerChangedNotificationItemVM(SettlementOwnerChangedMapNotification data)
			: base(data)
		{
			this._settlement = data.Settlement;
			this._newOwner = data.NewOwner;
			base.NotificationIdentifier = "settlementownerchanged";
			this._onInspect = delegate
			{
				base.GoToMapPosition(this._settlement.Position2D);
			};
			CampaignEvents.OnSettlementOwnerChangedEvent.AddNonSerializedListener(this, new Action<Settlement, bool, Hero, Hero, Hero, ChangeOwnerOfSettlementAction.ChangeOwnerOfSettlementDetail>(this.OnSettlementOwnerChanged));
		}

		// Token: 0x06000560 RID: 1376 RVA: 0x0001AF1C File Offset: 0x0001911C
		private void OnSettlementOwnerChanged(Settlement settlement, bool openToClaim, Hero newOwner, Hero oldOwner, Hero capturerHero, ChangeOwnerOfSettlementAction.ChangeOwnerOfSettlementDetail detail)
		{
			if (settlement == this._settlement && newOwner != this._newOwner)
			{
				base.ExecuteRemove();
			}
		}

		// Token: 0x06000561 RID: 1377 RVA: 0x0001AF36 File Offset: 0x00019136
		public override void OnFinalize()
		{
			base.OnFinalize();
			CampaignEvents.OnSettlementOwnerChangedEvent.ClearListeners(this);
		}

		// Token: 0x0400024C RID: 588
		private Settlement _settlement;

		// Token: 0x0400024D RID: 589
		private Hero _newOwner;
	}
}
