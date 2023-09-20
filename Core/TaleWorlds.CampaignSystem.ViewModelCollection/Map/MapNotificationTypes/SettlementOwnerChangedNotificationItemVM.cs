using System;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.MapNotificationTypes;
using TaleWorlds.CampaignSystem.Settlements;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.Map.MapNotificationTypes
{
	public class SettlementOwnerChangedNotificationItemVM : MapNotificationItemBaseVM
	{
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

		private void OnSettlementOwnerChanged(Settlement settlement, bool openToClaim, Hero newOwner, Hero oldOwner, Hero capturerHero, ChangeOwnerOfSettlementAction.ChangeOwnerOfSettlementDetail detail)
		{
			if (settlement == this._settlement && newOwner != this._newOwner)
			{
				base.ExecuteRemove();
			}
		}

		public override void OnFinalize()
		{
			base.OnFinalize();
			CampaignEvents.OnSettlementOwnerChangedEvent.ClearListeners(this);
		}

		private Settlement _settlement;

		private Hero _newOwner;
	}
}
