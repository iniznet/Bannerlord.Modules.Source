using System;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.HUD.KillFeed.Personal
{
	public class SPPersonalKillNotificationVM : ViewModel
	{
		public SPPersonalKillNotificationVM()
		{
			this.NotificationList = new MBBindingList<SPPersonalKillNotificationItemVM>();
		}

		public void OnPersonalKill(int damageAmount, bool isMountDamage, bool isFriendlyFire, bool isHeadshot, string killedAgentName, bool isUnconscious)
		{
			this.NotificationList.Add(new SPPersonalKillNotificationItemVM(damageAmount, isMountDamage, isFriendlyFire, isHeadshot, killedAgentName, isUnconscious, new Action<SPPersonalKillNotificationItemVM>(this.RemoveItem)));
		}

		public void OnPersonalHit(int damageAmount, bool isMountDamage, bool isFriendlyFire, string killedAgentName)
		{
			this.NotificationList.Add(new SPPersonalKillNotificationItemVM(damageAmount, isMountDamage, isFriendlyFire, killedAgentName, new Action<SPPersonalKillNotificationItemVM>(this.RemoveItem)));
		}

		public void OnPersonalAssist(string killedAgentName)
		{
			this.NotificationList.Add(new SPPersonalKillNotificationItemVM(killedAgentName, new Action<SPPersonalKillNotificationItemVM>(this.RemoveItem)));
		}

		private void RemoveItem(SPPersonalKillNotificationItemVM item)
		{
			this.NotificationList.Remove(item);
		}

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

		private MBBindingList<SPPersonalKillNotificationItemVM> _notificationList;
	}
}
