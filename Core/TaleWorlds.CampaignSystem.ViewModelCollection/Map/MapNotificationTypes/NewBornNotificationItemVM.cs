using System;
using TaleWorlds.CampaignSystem.MapNotificationTypes;
using TaleWorlds.CampaignSystem.SceneInformationPopupTypes;
using TaleWorlds.Core;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.Map.MapNotificationTypes
{
	public class NewBornNotificationItemVM : MapNotificationItemBaseVM
	{
		public NewBornNotificationItemVM(ChildBornMapNotification data)
			: base(data)
		{
			base.NotificationIdentifier = "newborn";
			if (data.NewbornHero != null)
			{
				Hero mother = data.NewbornHero.Mother;
				if (mother.Spouse == Hero.MainHero)
				{
					Hero spouse = mother.Spouse;
					if (spouse != null && spouse.IsAlive)
					{
						this._notification = new NewBornFemaleHeroSceneNotificationItem(mother.Spouse, mother, data.CreationTime);
					}
					else
					{
						this._notification = new NewBornFemaleHeroSceneAlternateNotificationItem(mother.Spouse, mother, data.CreationTime);
					}
				}
				else
				{
					Hero spouse2 = mother.Spouse;
					if (spouse2 != null && spouse2.IsAlive)
					{
						this._notification = new NewBornSceneNotificationItem(mother.Spouse, mother, data.CreationTime);
					}
					else
					{
						this._notification = new NewBornFemaleHeroSceneAlternateNotificationItem(mother.Spouse, mother, data.CreationTime);
					}
				}
			}
			if (this._notification != null)
			{
				this._onInspect = delegate
				{
					MBInformationManager.ShowSceneNotification(this._notification);
				};
				return;
			}
			this._onInspect = delegate
			{
			};
		}

		private readonly SceneNotificationData _notification;
	}
}
