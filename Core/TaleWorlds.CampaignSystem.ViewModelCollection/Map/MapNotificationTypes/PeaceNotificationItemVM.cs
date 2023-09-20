using System;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.MapNotificationTypes;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.Map.MapNotificationTypes
{
	public class PeaceNotificationItemVM : MapNotificationItemBaseVM
	{
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

		public override void OnFinalize()
		{
			base.OnFinalize();
			CampaignEvents.MakePeace.ClearListeners(this);
		}

		private void OnWarDeclared(IFaction faction1, IFaction faction2, DeclareWarAction.DeclareWarDetail detail)
		{
			if ((faction1 == Hero.MainHero.Clan && this._otherFaction == faction2) || (faction2 == Hero.MainHero.Clan && this._otherFaction == faction1))
			{
				base.ExecuteRemove();
			}
		}

		private IFaction _otherFaction;
	}
}
