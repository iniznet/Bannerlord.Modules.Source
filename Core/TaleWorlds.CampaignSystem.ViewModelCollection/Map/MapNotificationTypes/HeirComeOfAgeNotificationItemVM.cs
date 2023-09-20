using System;
using TaleWorlds.CampaignSystem.MapNotificationTypes;
using TaleWorlds.CampaignSystem.SceneInformationPopupTypes;
using TaleWorlds.Core;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.Map.MapNotificationTypes
{
	public class HeirComeOfAgeNotificationItemVM : MapNotificationItemBaseVM
	{
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
