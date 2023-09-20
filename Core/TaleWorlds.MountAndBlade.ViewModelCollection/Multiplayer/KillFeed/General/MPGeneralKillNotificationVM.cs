using System;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.KillFeed.General
{
	// Token: 0x020000B3 RID: 179
	public class MPGeneralKillNotificationVM : ViewModel
	{
		// Token: 0x060010F2 RID: 4338 RVA: 0x00037F0B File Offset: 0x0003610B
		public MPGeneralKillNotificationVM()
		{
			this.NotificationList = new MBBindingList<MPGeneralKillNotificationItemVM>();
		}

		// Token: 0x060010F3 RID: 4339 RVA: 0x00037F1E File Offset: 0x0003611E
		public void OnAgentRemoved(Agent affectedAgent, Agent affectorAgent, Agent assistedAgent)
		{
			this.NotificationList.Add(new MPGeneralKillNotificationItemVM(affectedAgent, affectorAgent, assistedAgent, new Action<MPGeneralKillNotificationItemVM>(this.RemoveItem)));
		}

		// Token: 0x060010F4 RID: 4340 RVA: 0x00037F3F File Offset: 0x0003613F
		private void RemoveItem(MPGeneralKillNotificationItemVM item)
		{
			this.NotificationList.Remove(item);
		}

		// Token: 0x17000576 RID: 1398
		// (get) Token: 0x060010F5 RID: 4341 RVA: 0x00037F4E File Offset: 0x0003614E
		// (set) Token: 0x060010F6 RID: 4342 RVA: 0x00037F56 File Offset: 0x00036156
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

		// Token: 0x0400080B RID: 2059
		private MBBindingList<MPGeneralKillNotificationItemVM> _notificationList;
	}
}
