using System;
using TaleWorlds.CampaignSystem.MapNotificationTypes;
using TaleWorlds.CampaignSystem.SceneInformationPopupTypes;
using TaleWorlds.Core;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.Map.MapNotificationTypes
{
	public class KingdomDestroyedNotificationItemVM : MapNotificationItemBaseVM
	{
		public KingdomDestroyedNotificationItemVM(KingdomDestroyedMapNotification data)
			: base(data)
		{
			KingdomDestroyedNotificationItemVM <>4__this = this;
			base.NotificationIdentifier = "kingdomdestroyed";
			this._onInspect = delegate
			{
				<>4__this.OnInspect(data);
			};
		}

		private void OnInspect(KingdomDestroyedMapNotification data)
		{
			MBInformationManager.ShowSceneNotification(new KingdomDestroyedSceneNotificationItem(data.DestroyedKingdom));
			base.ExecuteRemove();
		}
	}
}
