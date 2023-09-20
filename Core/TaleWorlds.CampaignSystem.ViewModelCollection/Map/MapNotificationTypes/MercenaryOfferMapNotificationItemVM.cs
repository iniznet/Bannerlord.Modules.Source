using System;
using TaleWorlds.CampaignSystem.MapNotificationTypes;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.Map.MapNotificationTypes
{
	public class MercenaryOfferMapNotificationItemVM : MapNotificationItemBaseVM
	{
		public MercenaryOfferMapNotificationItemVM(MercenaryOfferMapNotification data)
			: base(data)
		{
			this._offeredKingdom = data.OfferedKingdom;
			base.NotificationIdentifier = "vote";
			this._onInspect = delegate
			{
				CampaignEventDispatcher.Instance.OnVassalOrMercenaryServiceOfferedToPlayer(this._offeredKingdom);
				this._playerInspectedNotification = true;
				base.ExecuteRemove();
			};
			CampaignEvents.OnVassalOrMercenaryServiceOfferCanceledEvent.AddNonSerializedListener(this, new Action<Kingdom>(this.OnVassalOrMercenaryServiceOfferCanceled));
		}

		private void OnVassalOrMercenaryServiceOfferCanceled(Kingdom offeredKingdom)
		{
			if (Campaign.Current.CampaignInformationManager.InformationDataExists<MercenaryOfferMapNotification>((MercenaryOfferMapNotification x) => x.OfferedKingdom == offeredKingdom))
			{
				base.ExecuteRemove();
			}
		}

		public override void OnFinalize()
		{
			base.OnFinalize();
			CampaignEventDispatcher.Instance.RemoveListeners(this);
			if (!this._playerInspectedNotification)
			{
				CampaignEventDispatcher.Instance.OnVassalOrMercenaryServiceOfferCanceled(this._offeredKingdom);
			}
		}

		private bool _playerInspectedNotification;

		private readonly Kingdom _offeredKingdom;
	}
}
