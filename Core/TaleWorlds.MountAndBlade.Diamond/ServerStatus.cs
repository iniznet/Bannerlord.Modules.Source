using System;

namespace TaleWorlds.MountAndBlade.Diamond
{
	[Serializable]
	public class ServerStatus
	{
		public bool IsMatchmakingEnabled { get; set; }

		public bool IsCustomBattleEnabled { get; set; }

		public bool IsPlayerBasedCustomBattleEnabled { get; set; }

		public bool IsPremadeGameEnabled { get; set; }

		public bool IsTestRegionEnabled { get; set; }

		public Announcement Announcement { get; set; }

		public ServerNotification[] ServerNotifications { get; }

		public int FriendListUpdatePeriod { get; set; }

		public ServerStatus()
		{
		}

		public ServerStatus(bool isMatchmakingEnabled, bool isCustomBattleEnabled, bool isPlayerBasedCustomBattleEnabled, bool isPremadeGameEnabled, bool isTestRegionEnabled, Announcement announcement, ServerNotification[] serverNotifications, int friendListUpdatePeriod)
		{
			this.IsMatchmakingEnabled = isMatchmakingEnabled;
			this.IsCustomBattleEnabled = isCustomBattleEnabled;
			this.IsPlayerBasedCustomBattleEnabled = isPlayerBasedCustomBattleEnabled;
			this.IsPremadeGameEnabled = isPremadeGameEnabled;
			this.IsTestRegionEnabled = isTestRegionEnabled;
			this.Announcement = announcement;
			this.ServerNotifications = serverNotifications;
			this.FriendListUpdatePeriod = friendListUpdatePeriod;
		}
	}
}
