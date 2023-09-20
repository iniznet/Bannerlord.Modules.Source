using System;
using TaleWorlds.CampaignSystem.MapNotificationTypes;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.Map.MapNotificationTypes
{
	// Token: 0x0200004A RID: 74
	public class VassalOfferMapNotificationItemVM : MapNotificationItemBaseVM
	{
		// Token: 0x06000569 RID: 1385 RVA: 0x0001B09C File Offset: 0x0001929C
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

		// Token: 0x0600056A RID: 1386 RVA: 0x0001B0F0 File Offset: 0x000192F0
		private void OnVassalOrMercenaryServiceOfferCanceled(Kingdom offeredKingdom)
		{
			if (Campaign.Current.CampaignInformationManager.InformationDataExists<VassalOfferMapNotification>((VassalOfferMapNotification x) => x.OfferedKingdom == offeredKingdom))
			{
				base.ExecuteRemove();
			}
		}

		// Token: 0x0600056B RID: 1387 RVA: 0x0001B12D File Offset: 0x0001932D
		public override void OnFinalize()
		{
			base.OnFinalize();
			CampaignEventDispatcher.Instance.RemoveListeners(this);
		}

		// Token: 0x0400024F RID: 591
		private readonly Kingdom _offeredKingdom;
	}
}
