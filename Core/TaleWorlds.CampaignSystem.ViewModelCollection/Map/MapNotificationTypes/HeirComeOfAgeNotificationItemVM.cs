using System;
using TaleWorlds.CampaignSystem.MapNotificationTypes;
using TaleWorlds.CampaignSystem.SceneInformationPopupTypes;
using TaleWorlds.Core;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.Map.MapNotificationTypes
{
	// Token: 0x02000039 RID: 57
	public class HeirComeOfAgeNotificationItemVM : MapNotificationItemBaseVM
	{
		// Token: 0x0600050E RID: 1294 RVA: 0x00019FF4 File Offset: 0x000181F4
		public HeirComeOfAgeNotificationItemVM(HeirComeOfAgeMapNotification data)
			: base(data)
		{
			HeirComeOfAgeNotificationItemVM <>4__this = this;
			base.NotificationIdentifier = "comeofage";
			this._onInspect = delegate
			{
				<>4__this.OnInspect(data);
			};
		}

		// Token: 0x0600050F RID: 1295 RVA: 0x0001A040 File Offset: 0x00018240
		private void OnInspect(HeirComeOfAgeMapNotification data)
		{
			SceneNotificationData sceneNotificationData;
			if (data.ComeOfAgeHero.IsFemale)
			{
				sceneNotificationData = new HeirComingOfAgeFemaleSceneNotificationItem(data.MentorHero, data.ComeOfAgeHero);
			}
			else
			{
				sceneNotificationData = new HeirComingOfAgeSceneNotificationItem(data.MentorHero, data.ComeOfAgeHero);
			}
			MBInformationManager.ShowSceneNotification(sceneNotificationData);
			base.ExecuteRemove();
		}
	}
}
