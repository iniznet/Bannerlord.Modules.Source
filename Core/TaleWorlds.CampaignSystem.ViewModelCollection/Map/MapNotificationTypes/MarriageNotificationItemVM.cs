using System;
using TaleWorlds.CampaignSystem.MapNotificationTypes;
using TaleWorlds.CampaignSystem.SceneInformationPopupTypes;
using TaleWorlds.Core;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.Map.MapNotificationTypes
{
	public class MarriageNotificationItemVM : MapNotificationItemBaseVM
	{
		public Hero Suitor { get; private set; }

		public Hero Maiden { get; private set; }

		public MarriageNotificationItemVM(MarriageMapNotification data)
			: base(data)
		{
			this.Suitor = data.Suitor;
			this.Maiden = data.Maiden;
			base.NotificationIdentifier = "marriage";
			this._onInspect = delegate
			{
				MBInformationManager.ShowSceneNotification(new MarriageSceneNotificationItem(data.Suitor, data.Maiden, data.CreationTime, SceneNotificationData.RelevantContextType.Any));
			};
		}
	}
}
