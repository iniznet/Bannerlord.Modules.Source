using System;
using TaleWorlds.CampaignSystem.MapNotificationTypes;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.Map.MapNotificationTypes
{
	// Token: 0x0200003F RID: 63
	public class MercenaryOfferMapNotificationItemVM : MapNotificationItemBaseVM
	{
		// Token: 0x06000541 RID: 1345 RVA: 0x0001A6F0 File Offset: 0x000188F0
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

		// Token: 0x06000542 RID: 1346 RVA: 0x0001A744 File Offset: 0x00018944
		private void OnVassalOrMercenaryServiceOfferCanceled(Kingdom offeredKingdom)
		{
			if (Campaign.Current.CampaignInformationManager.InformationDataExists<MercenaryOfferMapNotification>((MercenaryOfferMapNotification x) => x.OfferedKingdom == offeredKingdom))
			{
				base.ExecuteRemove();
			}
		}

		// Token: 0x06000543 RID: 1347 RVA: 0x0001A781 File Offset: 0x00018981
		public override void OnFinalize()
		{
			base.OnFinalize();
			CampaignEventDispatcher.Instance.RemoveListeners(this);
			if (!this._playerInspectedNotification)
			{
				CampaignEventDispatcher.Instance.OnVassalOrMercenaryServiceOfferCanceled(this._offeredKingdom);
			}
		}

		// Token: 0x04000238 RID: 568
		private bool _playerInspectedNotification;

		// Token: 0x04000239 RID: 569
		private readonly Kingdom _offeredKingdom;
	}
}
