using System;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.KillFeed.Personal
{
	// Token: 0x020000B1 RID: 177
	public class MPPersonalKillNotificationVM : ViewModel
	{
		// Token: 0x060010C2 RID: 4290 RVA: 0x00037741 File Offset: 0x00035941
		public MPPersonalKillNotificationVM()
		{
			this.NotificationList = new MBBindingList<MPPersonalKillNotificationItemVM>();
		}

		// Token: 0x060010C3 RID: 4291 RVA: 0x00037754 File Offset: 0x00035954
		public void OnGoldChange(int changeAmount, GoldGainFlags goldGainType)
		{
			this.NotificationList.Add(new MPPersonalKillNotificationItemVM(changeAmount, goldGainType, new Action<MPPersonalKillNotificationItemVM>(this.RemoveItem)));
		}

		// Token: 0x060010C4 RID: 4292 RVA: 0x00037774 File Offset: 0x00035974
		public void OnPersonalHit(int damageAmount, bool isFatal, bool isMountDamage, bool isFriendlyFire, bool isHeadshot, string killedAgentName)
		{
			this.NotificationList.Add(new MPPersonalKillNotificationItemVM(damageAmount, isFatal, isMountDamage, isFriendlyFire, isHeadshot, killedAgentName, new Action<MPPersonalKillNotificationItemVM>(this.RemoveItem)));
		}

		// Token: 0x060010C5 RID: 4293 RVA: 0x000377A6 File Offset: 0x000359A6
		public void OnPersonalAssist(string killedAgentName)
		{
			this.NotificationList.Add(new MPPersonalKillNotificationItemVM(killedAgentName, new Action<MPPersonalKillNotificationItemVM>(this.RemoveItem)));
		}

		// Token: 0x060010C6 RID: 4294 RVA: 0x000377C5 File Offset: 0x000359C5
		private void RemoveItem(MPPersonalKillNotificationItemVM item)
		{
			this.NotificationList.Remove(item);
		}

		// Token: 0x17000564 RID: 1380
		// (get) Token: 0x060010C7 RID: 4295 RVA: 0x000377D4 File Offset: 0x000359D4
		// (set) Token: 0x060010C8 RID: 4296 RVA: 0x000377DC File Offset: 0x000359DC
		[DataSourceProperty]
		public MBBindingList<MPPersonalKillNotificationItemVM> NotificationList
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
					base.OnPropertyChangedWithValue<MBBindingList<MPPersonalKillNotificationItemVM>>(value, "NotificationList");
				}
			}
		}

		// Token: 0x040007F7 RID: 2039
		private MBBindingList<MPPersonalKillNotificationItemVM> _notificationList;
	}
}
