using System;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.KillFeed.Personal
{
	public class MPPersonalKillNotificationVM : ViewModel
	{
		public MPPersonalKillNotificationVM()
		{
			this.NotificationList = new MBBindingList<MPPersonalKillNotificationItemVM>();
		}

		public void OnGoldChange(int changeAmount, GoldGainFlags goldGainType)
		{
			this.NotificationList.Add(new MPPersonalKillNotificationItemVM(changeAmount, goldGainType, new Action<MPPersonalKillNotificationItemVM>(this.RemoveItem)));
		}

		public void OnPersonalHit(int damageAmount, bool isFatal, bool isMountDamage, bool isFriendlyFire, bool isHeadshot, string killedAgentName)
		{
			this.NotificationList.Add(new MPPersonalKillNotificationItemVM(damageAmount, isFatal, isMountDamage, isFriendlyFire, isHeadshot, killedAgentName, new Action<MPPersonalKillNotificationItemVM>(this.RemoveItem)));
		}

		public void OnPersonalAssist(string killedAgentName)
		{
			this.NotificationList.Add(new MPPersonalKillNotificationItemVM(killedAgentName, new Action<MPPersonalKillNotificationItemVM>(this.RemoveItem)));
		}

		private void RemoveItem(MPPersonalKillNotificationItemVM item)
		{
			this.NotificationList.Remove(item);
		}

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

		private MBBindingList<MPPersonalKillNotificationItemVM> _notificationList;
	}
}
