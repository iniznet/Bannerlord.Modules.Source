using System;
using TaleWorlds.CampaignSystem.MapNotificationTypes;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Siege;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.Map.MapNotificationTypes
{
	// Token: 0x02000048 RID: 72
	public class SettlementUnderSiegeMapNotificationItemVM : MapNotificationItemBaseVM
	{
		// Token: 0x06000563 RID: 1379 RVA: 0x0001AF5C File Offset: 0x0001915C
		public SettlementUnderSiegeMapNotificationItemVM(SettlementUnderSiegeMapNotification data)
			: base(data)
		{
			this._settlement = data.BesiegedSettlement;
			base.NotificationIdentifier = "settlementundersiege";
			this._onInspect = delegate
			{
				base.GoToMapPosition(this._settlement.Position2D);
			};
			CampaignEvents.OnSiegeEventEndedEvent.AddNonSerializedListener(this, new Action<SiegeEvent>(this.OnSiegeEventEnded));
		}

		// Token: 0x06000564 RID: 1380 RVA: 0x0001AFB0 File Offset: 0x000191B0
		private void OnSiegeEventEnded(SiegeEvent obj)
		{
			if (obj.BesiegedSettlement == this._settlement)
			{
				base.ExecuteRemove();
			}
		}

		// Token: 0x06000565 RID: 1381 RVA: 0x0001AFC6 File Offset: 0x000191C6
		public override void OnFinalize()
		{
			base.OnFinalize();
			CampaignEvents.OnSiegeEventEndedEvent.ClearListeners(this);
		}

		// Token: 0x0400024E RID: 590
		private Settlement _settlement;
	}
}
