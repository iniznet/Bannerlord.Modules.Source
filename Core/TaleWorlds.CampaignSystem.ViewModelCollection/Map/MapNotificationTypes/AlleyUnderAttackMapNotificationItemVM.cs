using System;
using TaleWorlds.CampaignSystem.MapNotificationTypes;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.Map.MapNotificationTypes
{
	public class AlleyUnderAttackMapNotificationItemVM : MapNotificationItemBaseVM
	{
		public AlleyUnderAttackMapNotificationItemVM(AlleyUnderAttackMapNotification data)
			: base(data)
		{
			this._alley = data.Alley;
			base.NotificationIdentifier = "alley_under_attack";
			CampaignEvents.SettlementEntered.AddNonSerializedListener(this, new Action<MobileParty, Settlement, Hero>(this.OnSettlementEnter));
			this._onInspect = delegate
			{
				base.GoToMapPosition(this._alley.Settlement.Position2D);
			};
		}

		private void OnSettlementEnter(MobileParty party, Settlement settlement, Hero hero)
		{
			if (party != null && party.IsMainParty && settlement == this._alley.Settlement)
			{
				CampaignEventDispatcher.Instance.RemoveListeners(this);
				base.ExecuteRemove();
			}
		}

		private Alley _alley;
	}
}
