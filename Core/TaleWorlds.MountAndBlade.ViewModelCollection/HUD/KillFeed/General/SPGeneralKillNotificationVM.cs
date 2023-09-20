using System;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.HUD.KillFeed.General
{
	public class SPGeneralKillNotificationVM : ViewModel
	{
		public SPGeneralKillNotificationVM()
		{
			this.NotificationList = new MBBindingList<SPGeneralKillNotificationItemVM>();
		}

		public void OnAgentRemoved(Agent affectedAgent, Agent affectorAgent, Agent assistedAgent, bool isHeadshot)
		{
			this.NotificationList.Add(new SPGeneralKillNotificationItemVM(affectedAgent, affectorAgent, assistedAgent, isHeadshot, new Action<SPGeneralKillNotificationItemVM>(this.RemoveItem)));
		}

		private void RemoveItem(SPGeneralKillNotificationItemVM item)
		{
			this.NotificationList.Remove(item);
		}

		[DataSourceProperty]
		public MBBindingList<SPGeneralKillNotificationItemVM> NotificationList
		{
			get
			{
				return this._notificationList;
			}
			set
			{
				if (value != this._notificationList)
				{
					this._notificationList = value;
					base.OnPropertyChangedWithValue<MBBindingList<SPGeneralKillNotificationItemVM>>(value, "NotificationList");
				}
			}
		}

		private MBBindingList<SPGeneralKillNotificationItemVM> _notificationList;
	}
}
