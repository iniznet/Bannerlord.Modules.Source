using System;
using TaleWorlds.CampaignSystem.MapNotificationTypes;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.Map.MapNotificationTypes
{
	public class VassalOfferMapNotificationItemVM : MapNotificationItemBaseVM
	{
		public VassalOfferMapNotificationItemVM(VassalOfferMapNotification data)
			: base(data)
		{
			this._offeredKingdom = data.OfferedKingdom;
			base.NotificationIdentifier = "vote";
			this._onInspect = delegate
			{
				CampaignEventDispatcher.Instance.OnVassalOrMercenaryServiceOfferedToPlayer(this._offeredKingdom);
				base.ExecuteRemove();
			};
			CampaignEvents.OnVassalOrMercenaryServiceOfferCanceledEvent.AddNonSerializedListener(this, new Action<Kingdom>(this.OnVassalOrMercenaryServiceOfferCanceled));
		}

		private void OnVassalOrMercenaryServiceOfferCanceled(Kingdom offeredKingdom)
		{
			if (Campaign.Current.CampaignInformationManager.InformationDataExists<VassalOfferMapNotification>((VassalOfferMapNotification x) => x.OfferedKingdom == offeredKingdom))
			{
				base.ExecuteRemove();
			}
		}

		public override void OnFinalize()
		{
			base.OnFinalize();
			CampaignEventDispatcher.Instance.RemoveListeners(this);
		}

		private readonly Kingdom _offeredKingdom;
	}
}
