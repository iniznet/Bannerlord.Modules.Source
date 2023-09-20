using System;
using TaleWorlds.CampaignSystem.MapNotificationTypes;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.Map.MapNotificationTypes
{
	public class ArmyDispersionItemVM : MapNotificationItemBaseVM
	{
		public ArmyDispersionItemVM(ArmyDispersionMapNotification data)
			: base(data)
		{
			ArmyDispersionItemVM <>4__this = this;
			base.NotificationIdentifier = "armydispersion";
			this._onInspect = delegate
			{
				INavigationHandler navigationHandler = <>4__this.NavigationHandler;
				if (navigationHandler == null)
				{
					return;
				}
				navigationHandler.OpenKingdom(data.DispersedArmy);
			};
		}
	}
}
