using System;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.MapNotificationTypes;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.Map.MapNotificationTypes
{
	// Token: 0x02000042 RID: 66
	public class PeaceNotificationItemVM : MapNotificationItemBaseVM
	{
		// Token: 0x0600054C RID: 1356 RVA: 0x0001AA54 File Offset: 0x00018C54
		public PeaceNotificationItemVM(PeaceMapNotification data)
			: base(data)
		{
			base.NotificationIdentifier = "peace";
			CampaignEvents.WarDeclared.AddNonSerializedListener(this, new Action<IFaction, IFaction, DeclareWarAction.DeclareWarDetail>(this.OnWarDeclared));
			this._otherFaction = ((data.FirstFaction == Hero.MainHero.MapFaction) ? data.SecondFaction : data.FirstFaction);
			this._onInspect = delegate
			{
				INavigationHandler navigationHandler = base.NavigationHandler;
				if (navigationHandler == null)
				{
					return;
				}
				navigationHandler.OpenKingdom();
			};
		}

		// Token: 0x0600054D RID: 1357 RVA: 0x0001AAC2 File Offset: 0x00018CC2
		public override void OnFinalize()
		{
			base.OnFinalize();
			CampaignEvents.MakePeace.ClearListeners(this);
		}

		// Token: 0x0600054E RID: 1358 RVA: 0x0001AAD5 File Offset: 0x00018CD5
		private void OnWarDeclared(IFaction faction1, IFaction faction2, DeclareWarAction.DeclareWarDetail detail)
		{
			if ((faction1 == Hero.MainHero.Clan && this._otherFaction == faction2) || (faction2 == Hero.MainHero.Clan && this._otherFaction == faction1))
			{
				base.ExecuteRemove();
			}
		}

		// Token: 0x0400023F RID: 575
		private IFaction _otherFaction;
	}
}
