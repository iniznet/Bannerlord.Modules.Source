﻿using System;
using TaleWorlds.CampaignSystem.MapNotificationTypes;
using TaleWorlds.CampaignSystem.SceneInformationPopupTypes;
using TaleWorlds.Core;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.Map.MapNotificationTypes
{
	// Token: 0x02000040 RID: 64
	public class NewBornNotificationItemVM : MapNotificationItemBaseVM
	{
		// Token: 0x06000545 RID: 1349 RVA: 0x0001A7CC File Offset: 0x000189CC
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
						this._notification = new NewBornFemaleHeroSceneNotificationItem(mother.Spouse, mother);
					}
					else
					{
						this._notification = new NewBornFemaleHeroSceneAlternateNotificationItem(mother.Spouse, mother);
					}
				}
				else
				{
					Hero spouse2 = mother.Spouse;
					if (spouse2 != null && spouse2.IsAlive)
					{
						this._notification = new NewBornSceneNotificationItem(mother.Spouse, mother);
					}
					else
					{
						this._notification = new NewBornFemaleHeroSceneAlternateNotificationItem(mother.Spouse, mother);
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

		// Token: 0x0400023A RID: 570
		private readonly SceneNotificationData _notification;
	}
}
