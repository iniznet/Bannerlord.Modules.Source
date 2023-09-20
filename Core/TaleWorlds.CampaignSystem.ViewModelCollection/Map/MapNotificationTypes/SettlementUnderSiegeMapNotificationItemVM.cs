using System;
using TaleWorlds.CampaignSystem.MapNotificationTypes;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Siege;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.Map.MapNotificationTypes
{
	public class SettlementUnderSiegeMapNotificationItemVM : MapNotificationItemBaseVM
	{
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

		private void OnSiegeEventEnded(SiegeEvent obj)
		{
			if (obj.BesiegedSettlement == this._settlement)
			{
				base.ExecuteRemove();
			}
		}

		public override void OnFinalize()
		{
			base.OnFinalize();
			CampaignEvents.OnSiegeEventEndedEvent.ClearListeners(this);
		}

		private Settlement _settlement;
	}
}
