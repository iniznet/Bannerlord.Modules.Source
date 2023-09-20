using System;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.MapNotificationTypes;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.Map.MapNotificationTypes
{
	// Token: 0x0200004B RID: 75
	public class WarNotificationItemVM : MapNotificationItemBaseVM
	{
		// Token: 0x0600056D RID: 1389 RVA: 0x0001B158 File Offset: 0x00019358
		public WarNotificationItemVM(WarMapNotification data)
			: base(data)
		{
			WarNotificationItemVM <>4__this = this;
			base.NotificationIdentifier = "battle";
			CampaignEvents.MakePeace.AddNonSerializedListener(this, new Action<IFaction, IFaction, MakePeaceAction.MakePeaceDetail>(this.OnPeaceMade));
			if (!data.FirstFaction.IsRebelClan && !data.SecondFaction.IsRebelClan)
			{
				this._onInspect = delegate
				{
					INavigationHandler navigationHandler = <>4__this.NavigationHandler;
					if (navigationHandler == null)
					{
						return;
					}
					navigationHandler.OpenKingdom((data.FirstFaction == Hero.MainHero.MapFaction) ? data.SecondFaction : data.FirstFaction);
				};
				return;
			}
			this._onInspect = null;
		}

		// Token: 0x0600056E RID: 1390 RVA: 0x0001B1E5 File Offset: 0x000193E5
		public override void OnFinalize()
		{
			base.OnFinalize();
			CampaignEvents.MakePeace.ClearListeners(this);
		}

		// Token: 0x0600056F RID: 1391 RVA: 0x0001B1F8 File Offset: 0x000193F8
		private void OnPeaceMade(IFaction faction1, IFaction faction2, MakePeaceAction.MakePeaceDetail detail)
		{
			if (faction1 == Hero.MainHero.Clan || (Hero.MainHero.MapFaction != null && (faction1 == Hero.MainHero.MapFaction || faction2 == Hero.MainHero.MapFaction)))
			{
				base.ExecuteRemove();
			}
		}
	}
}
