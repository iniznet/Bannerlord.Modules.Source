using System;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.HUD.KillFeed.Personal
{
	// Token: 0x020000E7 RID: 231
	public class SPPersonalKillNotificationVM : ViewModel
	{
		// Token: 0x060014E2 RID: 5346 RVA: 0x00044057 File Offset: 0x00042257
		public SPPersonalKillNotificationVM()
		{
			this.NotificationList = new MBBindingList<SPPersonalKillNotificationItemVM>();
		}

		// Token: 0x060014E3 RID: 5347 RVA: 0x0004406C File Offset: 0x0004226C
		public void OnPersonalKill(int damageAmount, bool isMountDamage, bool isFriendlyFire, bool isHeadshot, string killedAgentName, bool isUnconscious)
		{
			this.NotificationList.Add(new SPPersonalKillNotificationItemVM(damageAmount, isMountDamage, isFriendlyFire, isHeadshot, killedAgentName, isUnconscious, new Action<SPPersonalKillNotificationItemVM>(this.RemoveItem)));
		}

		// Token: 0x060014E4 RID: 5348 RVA: 0x0004409E File Offset: 0x0004229E
		public void OnPersonalHit(int damageAmount, bool isMountDamage, bool isFriendlyFire, string killedAgentName)
		{
			this.NotificationList.Add(new SPPersonalKillNotificationItemVM(damageAmount, isMountDamage, isFriendlyFire, killedAgentName, new Action<SPPersonalKillNotificationItemVM>(this.RemoveItem)));
		}

		// Token: 0x060014E5 RID: 5349 RVA: 0x000440C1 File Offset: 0x000422C1
		public void OnPersonalAssist(string killedAgentName)
		{
			this.NotificationList.Add(new SPPersonalKillNotificationItemVM(killedAgentName, new Action<SPPersonalKillNotificationItemVM>(this.RemoveItem)));
		}

		// Token: 0x060014E6 RID: 5350 RVA: 0x000440E0 File Offset: 0x000422E0
		private void RemoveItem(SPPersonalKillNotificationItemVM item)
		{
			this.NotificationList.Remove(item);
		}

		// Token: 0x170006DE RID: 1758
		// (get) Token: 0x060014E7 RID: 5351 RVA: 0x000440EF File Offset: 0x000422EF
		// (set) Token: 0x060014E8 RID: 5352 RVA: 0x000440F7 File Offset: 0x000422F7
		[DataSourceProperty]
		public MBBindingList<SPPersonalKillNotificationItemVM> NotificationList
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
					base.OnPropertyChangedWithValue<MBBindingList<SPPersonalKillNotificationItemVM>>(value, "NotificationList");
				}
			}
		}

		// Token: 0x040009FA RID: 2554
		private MBBindingList<SPPersonalKillNotificationItemVM> _notificationList;
	}
}
