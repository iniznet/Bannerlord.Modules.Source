using System;
using TaleWorlds.CampaignSystem.MapNotificationTypes;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.Map.MapNotificationTypes
{
	// Token: 0x02000045 RID: 69
	public class RansomNotificationItemVM : MapNotificationItemBaseVM
	{
		// Token: 0x06000559 RID: 1369 RVA: 0x0001ADA4 File Offset: 0x00018FA4
		public RansomNotificationItemVM(RansomOfferMapNotification data)
			: base(data)
		{
			RansomNotificationItemVM <>4__this = this;
			this._hero = data.CaptiveHero;
			this._onInspect = delegate
			{
				<>4__this._playerInspectedNotification = true;
				CampaignEventDispatcher.Instance.OnRansomOfferedToPlayer(data.CaptiveHero);
				<>4__this.ExecuteRemove();
			};
			CampaignEvents.OnRansomOfferCancelledEvent.AddNonSerializedListener(this, new Action<Hero>(this.OnRansomOfferCancelled));
			base.NotificationIdentifier = "ransom";
		}

		// Token: 0x0600055A RID: 1370 RVA: 0x0001AE16 File Offset: 0x00019016
		private void OnRansomOfferCancelled(Hero captiveHero)
		{
			if (captiveHero == this._hero)
			{
				base.ExecuteRemove();
			}
		}

		// Token: 0x0600055B RID: 1371 RVA: 0x0001AE27 File Offset: 0x00019027
		public override void OnFinalize()
		{
			base.OnFinalize();
			CampaignEvents.OnRansomOfferCancelledEvent.ClearListeners(this);
			if (!this._playerInspectedNotification)
			{
				CampaignEventDispatcher.Instance.OnRansomOfferCancelled(this._hero);
			}
		}

		// Token: 0x04000248 RID: 584
		private bool _playerInspectedNotification;

		// Token: 0x04000249 RID: 585
		private Hero _hero;
	}
}
