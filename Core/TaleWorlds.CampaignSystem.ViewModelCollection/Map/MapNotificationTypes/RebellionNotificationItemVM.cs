using System;
using TaleWorlds.CampaignSystem.MapNotificationTypes;
using TaleWorlds.CampaignSystem.Settlements;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.Map.MapNotificationTypes
{
	// Token: 0x02000046 RID: 70
	public class RebellionNotificationItemVM : MapNotificationItemBaseVM
	{
		// Token: 0x0600055C RID: 1372 RVA: 0x0001AE54 File Offset: 0x00019054
		public RebellionNotificationItemVM(SettlementRebellionMapNotification data)
			: base(data)
		{
			this._settlement = data.RebelliousSettlement;
			this._onInspect = (this._onInspectAction = delegate
			{
				INavigationHandler navigationHandler = base.NavigationHandler;
				if (navigationHandler == null)
				{
					return;
				}
				navigationHandler.OpenKingdom(this._settlement);
			});
			base.NotificationIdentifier = "rebellion";
		}

		// Token: 0x0600055D RID: 1373 RVA: 0x0001AE9A File Offset: 0x0001909A
		public override void ManualRefreshRelevantStatus()
		{
			base.ManualRefreshRelevantStatus();
		}

		// Token: 0x0400024A RID: 586
		private Settlement _settlement;

		// Token: 0x0400024B RID: 587
		protected Action _onInspectAction;
	}
}
