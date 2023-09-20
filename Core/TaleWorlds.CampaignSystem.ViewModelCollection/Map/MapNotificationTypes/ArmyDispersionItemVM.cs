using System;
using TaleWorlds.CampaignSystem.MapNotificationTypes;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.Map.MapNotificationTypes
{
	// Token: 0x02000036 RID: 54
	public class ArmyDispersionItemVM : MapNotificationItemBaseVM
	{
		// Token: 0x06000508 RID: 1288 RVA: 0x00019DE0 File Offset: 0x00017FE0
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
