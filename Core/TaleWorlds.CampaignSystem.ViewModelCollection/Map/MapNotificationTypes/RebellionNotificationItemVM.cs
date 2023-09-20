using System;
using TaleWorlds.CampaignSystem.MapNotificationTypes;
using TaleWorlds.CampaignSystem.Settlements;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.Map.MapNotificationTypes
{
	public class RebellionNotificationItemVM : MapNotificationItemBaseVM
	{
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

		public override void ManualRefreshRelevantStatus()
		{
			base.ManualRefreshRelevantStatus();
		}

		private Settlement _settlement;

		protected Action _onInspectAction;
	}
}
