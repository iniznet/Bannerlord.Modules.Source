using System;
using TaleWorlds.CampaignSystem.MapNotificationTypes;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.Map.MapNotificationTypes
{
	// Token: 0x02000034 RID: 52
	public class AlleyUnderAttackMapNotificationItemVM : MapNotificationItemBaseVM
	{
		// Token: 0x060004FF RID: 1279 RVA: 0x00019C24 File Offset: 0x00017E24
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

		// Token: 0x06000500 RID: 1280 RVA: 0x00019C78 File Offset: 0x00017E78
		private void OnSettlementEnter(MobileParty party, Settlement settlement, Hero hero)
		{
			if (party != null && party.IsMainParty && settlement == this._alley.Settlement)
			{
				CampaignEventDispatcher.Instance.RemoveListeners(this);
				base.ExecuteRemove();
			}
		}

		// Token: 0x0400021D RID: 541
		private Alley _alley;
	}
}
