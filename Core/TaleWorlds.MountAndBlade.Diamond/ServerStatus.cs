using System;

namespace TaleWorlds.MountAndBlade.Diamond
{
	[Serializable]
	public class ServerStatus
	{
		public bool IsMatchmakingEnabled { get; private set; }

		public bool IsCustomBattleEnabled { get; private set; }

		public bool IsPlayerBasedCustomBattleEnabled { get; private set; }

		public bool IsPremadeGameEnabled { get; private set; }

		public bool IsTestRegionEnabled { get; set; }

		public Announcement Announcement { get; private set; }

		public ServerNotification[] ServerNotifications { get; }

		public int FriendListUpdatePeriod { get; private set; }

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
