using System;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.KillFeed.General
{
	public class MPGeneralKillNotificationVM : ViewModel
	{
		public MPGeneralKillNotificationVM()
		{
			this.NotificationList = new MBBindingList<MPGeneralKillNotificationItemVM>();
		}

		public void OnAgentRemoved(Agent affectedAgent, Agent affectorAgent, Agent assistedAgent)
		{
			this.NotificationList.Add(new MPGeneralKillNotificationItemVM(affectedAgent, affectorAgent, assistedAgent, new Action<MPGeneralKillNotificationItemVM>(this.RemoveItem)));
		}

		private void RemoveItem(MPGeneralKillNotificationItemVM item)
		{
			this.NotificationList.Remove(item);
		}

		[DataSourceProperty]
		public MBBindingList<MPGeneralKillNotificationItemVM> NotificationList
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
					base.OnPropertyChangedWithValue<MBBindingList<MPGeneralKillNotificationItemVM>>(value, "NotificationList");
				}
			}
		}

		private MBBindingList<MPGeneralKillNotificationItemVM> _notificationList;
	}
}
