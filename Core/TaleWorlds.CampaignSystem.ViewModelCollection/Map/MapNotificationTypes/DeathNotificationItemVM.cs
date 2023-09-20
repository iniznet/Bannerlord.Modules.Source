using System;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.MapNotificationTypes;
using TaleWorlds.CampaignSystem.SceneInformationPopupTypes;
using TaleWorlds.Core;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.Map.MapNotificationTypes
{
	public class DeathNotificationItemVM : MapNotificationItemBaseVM
	{
		public DeathNotificationItemVM(DeathMapNotification data)
			: base(data)
		{
			DeathNotificationItemVM <>4__this = this;
			base.NotificationIdentifier = "death";
			if (data.VictimHero == Hero.MainHero)
			{
				this._onInspect = delegate
				{
					INavigationHandler navigationHandler = <>4__this.NavigationHandler;
					if (navigationHandler == null)
					{
						return;
					}
					navigationHandler.OpenCharacterDeveloper(Hero.MainHero);
				};
				return;
			}
			if (data.KillDetail == KillCharacterAction.KillCharacterActionDetail.DiedInBattle)
			{
				this._onInspect = delegate
				{
					MBInformationManager.ShowSceneNotification(new ClanMemberWarDeathSceneNotificationItem(data.VictimHero));
				};
				return;
			}
			this._onInspect = delegate
			{
				MBInformationManager.ShowSceneNotification(new ClanMemberPeaceDeathSceneNotificationItem(data.VictimHero));
			};
		}
	}
}
