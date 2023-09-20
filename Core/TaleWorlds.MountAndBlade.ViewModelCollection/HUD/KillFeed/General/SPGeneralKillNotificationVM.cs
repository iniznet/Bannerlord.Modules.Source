using System;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.HUD.KillFeed.General
{
	// Token: 0x020000E9 RID: 233
	public class SPGeneralKillNotificationVM : ViewModel
	{
		// Token: 0x060014F1 RID: 5361 RVA: 0x000443B5 File Offset: 0x000425B5
		public SPGeneralKillNotificationVM()
		{
			this.NotificationList = new MBBindingList<SPGeneralKillNotificationItemVM>();
		}

		// Token: 0x060014F2 RID: 5362 RVA: 0x000443C8 File Offset: 0x000425C8
		public void OnAgentRemoved(Agent affectedAgent, Agent affectorAgent, Agent assistedAgent, bool isHeadshot)
		{
			this.NotificationList.Add(new SPGeneralKillNotificationItemVM(affectedAgent, affectorAgent, assistedAgent, isHeadshot, new Action<SPGeneralKillNotificationItemVM>(this.RemoveItem)));
		}

		// Token: 0x060014F3 RID: 5363 RVA: 0x000443EB File Offset: 0x000425EB
		private void RemoveItem(SPGeneralKillNotificationItemVM item)
		{
			this.NotificationList.Remove(item);
		}

		// Token: 0x170006E2 RID: 1762
		// (get) Token: 0x060014F4 RID: 5364 RVA: 0x000443FA File Offset: 0x000425FA
		// (set) Token: 0x060014F5 RID: 5365 RVA: 0x00044402 File Offset: 0x00042602
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

		// Token: 0x04000A02 RID: 2562
		private MBBindingList<SPGeneralKillNotificationItemVM> _notificationList;
	}
}
