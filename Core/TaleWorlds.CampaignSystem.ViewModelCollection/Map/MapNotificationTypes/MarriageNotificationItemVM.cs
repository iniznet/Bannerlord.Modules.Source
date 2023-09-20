using System;
using TaleWorlds.CampaignSystem.MapNotificationTypes;
using TaleWorlds.CampaignSystem.SceneInformationPopupTypes;
using TaleWorlds.Core;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.Map.MapNotificationTypes
{
	// Token: 0x0200003D RID: 61
	public class MarriageNotificationItemVM : MapNotificationItemBaseVM
	{
		// Token: 0x17000178 RID: 376
		// (get) Token: 0x06000538 RID: 1336 RVA: 0x0001A55E File Offset: 0x0001875E
		// (set) Token: 0x06000539 RID: 1337 RVA: 0x0001A566 File Offset: 0x00018766
		public Hero Suitor { get; private set; }

		// Token: 0x17000179 RID: 377
		// (get) Token: 0x0600053A RID: 1338 RVA: 0x0001A56F File Offset: 0x0001876F
		// (set) Token: 0x0600053B RID: 1339 RVA: 0x0001A577 File Offset: 0x00018777
		public Hero Maiden { get; private set; }

		// Token: 0x0600053C RID: 1340 RVA: 0x0001A580 File Offset: 0x00018780
		public MarriageNotificationItemVM(MarriageMapNotification data)
			: base(data)
		{
			this.Suitor = data.Suitor;
			this.Maiden = data.Maiden;
			base.NotificationIdentifier = "marriage";
			this._onInspect = delegate
			{
				MBInformationManager.ShowSceneNotification(new MarriageSceneNotificationItem(data.Suitor, data.Maiden, SceneNotificationData.RelevantContextType.Any));
			};
		}
	}
}
