using System;
using TaleWorlds.CampaignSystem.MapNotificationTypes;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.Map.MapNotificationTypes
{
	public class MarriageOfferNotificationItemVM : MapNotificationItemBaseVM
	{
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

		private void OnMarriageOfferCanceled(Hero suitor, Hero maiden)
		{
			if (Campaign.Current.CampaignInformationManager.InformationDataExists<MarriageOfferMapNotification>((MarriageOfferMapNotification x) => x.Suitor == suitor && x.Maiden == maiden))
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
				CampaignEventDispatcher.Instance.OnMarriageOfferCanceled(this._suitor, this._maiden);
			}
		}

		private bool _playerInspectedNotification;

		private readonly Hero _suitor;

		private readonly Hero _maiden;
	}
}
