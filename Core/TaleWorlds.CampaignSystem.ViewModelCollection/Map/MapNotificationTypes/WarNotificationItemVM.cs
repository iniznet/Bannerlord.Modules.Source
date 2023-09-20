using System;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.MapNotificationTypes;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.Map.MapNotificationTypes
{
	public class WarNotificationItemVM : MapNotificationItemBaseVM
	{
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

		public override void OnFinalize()
		{
			base.OnFinalize();
			CampaignEvents.MakePeace.ClearListeners(this);
		}

		private void OnPeaceMade(IFaction faction1, IFaction faction2, MakePeaceAction.MakePeaceDetail detail)
		{
			if (faction1 == Hero.MainHero.Clan || (Hero.MainHero.MapFaction != null && (faction1 == Hero.MainHero.MapFaction || faction2 == Hero.MainHero.MapFaction)))
			{
				base.ExecuteRemove();
			}
		}
	}
}
