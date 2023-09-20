using System;
using TaleWorlds.CampaignSystem.MapNotificationTypes;
using TaleWorlds.CampaignSystem.SceneInformationPopupTypes;
using TaleWorlds.Core;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.Map.MapNotificationTypes
{
	// Token: 0x0200003A RID: 58
	public class KingdomDestroyedNotificationItemVM : MapNotificationItemBaseVM
	{
		// Token: 0x06000510 RID: 1296 RVA: 0x0001A08C File Offset: 0x0001828C
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

		// Token: 0x06000511 RID: 1297 RVA: 0x0001A0D6 File Offset: 0x000182D6
		private void OnInspect(KingdomDestroyedMapNotification data)
		{
			MBInformationManager.ShowSceneNotification(new KingdomDestroyedSceneNotificationItem(data.DestroyedKingdom));
			base.ExecuteRemove();
		}
	}
}
