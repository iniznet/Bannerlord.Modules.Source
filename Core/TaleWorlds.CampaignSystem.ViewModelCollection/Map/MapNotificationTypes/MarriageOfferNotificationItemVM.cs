using System;
using TaleWorlds.CampaignSystem.MapNotificationTypes;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.Map.MapNotificationTypes
{
	// Token: 0x0200003E RID: 62
	public class MarriageOfferNotificationItemVM : MapNotificationItemBaseVM
	{
		// Token: 0x0600053D RID: 1341 RVA: 0x0001A5E8 File Offset: 0x000187E8
		public MarriageOfferNotificationItemVM(MarriageOfferMapNotification data)
			: base(data)
		{
			this._suitor = data.Suitor;
			this._maiden = data.Maiden;
			base.NotificationIdentifier = "marriage";
			this._onInspect = delegate
			{
				CampaignEventDispatcher.Instance.OnMarriageOfferedToPlayer(this._suitor, this._maiden);
				this._playerInspectedNotification = true;
				Campaign.Current.TimeControlMode = CampaignTimeControlMode.Stop;
				base.ExecuteRemove();
			};
			CampaignEvents.OnMarriageOfferCanceledEvent.AddNonSerializedListener(this, new Action<Hero, Hero>(this.OnMarriageOfferCanceled));
		}

		// Token: 0x0600053E RID: 1342 RVA: 0x0001A648 File Offset: 0x00018848
		private void OnMarriageOfferCanceled(Hero suitor, Hero maiden)
		{
			if (Campaign.Current.CampaignInformationManager.InformationDataExists<MarriageOfferMapNotification>((MarriageOfferMapNotification x) => x.Suitor == suitor && x.Maiden == maiden))
			{
				base.ExecuteRemove();
			}
		}

		// Token: 0x0600053F RID: 1343 RVA: 0x0001A68C File Offset: 0x0001888C
		public override void OnFinalize()
		{
			base.OnFinalize();
			CampaignEventDispatcher.Instance.RemoveListeners(this);
			if (!this._playerInspectedNotification)
			{
				CampaignEventDispatcher.Instance.OnMarriageOfferCanceled(this._suitor, this._maiden);
			}
		}

		// Token: 0x04000235 RID: 565
		private bool _playerInspectedNotification;

		// Token: 0x04000236 RID: 566
		private readonly Hero _suitor;

		// Token: 0x04000237 RID: 567
		private readonly Hero _maiden;
	}
}
