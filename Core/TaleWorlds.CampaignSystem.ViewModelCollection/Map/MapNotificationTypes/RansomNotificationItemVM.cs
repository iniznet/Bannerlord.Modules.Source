using System;
using TaleWorlds.CampaignSystem.MapNotificationTypes;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.Map.MapNotificationTypes
{
	public class RansomNotificationItemVM : MapNotificationItemBaseVM
	{
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

		private void OnRansomOfferCancelled(Hero captiveHero)
		{
			if (captiveHero == this._hero)
			{
				base.ExecuteRemove();
			}
		}

		public override void OnFinalize()
		{
			base.OnFinalize();
			CampaignEvents.OnRansomOfferCancelledEvent.ClearListeners(this);
			if (!this._playerInspectedNotification)
			{
				CampaignEventDispatcher.Instance.OnRansomOfferCancelled(this._hero);
			}
		}

		private bool _playerInspectedNotification;

		private Hero _hero;
	}
}
