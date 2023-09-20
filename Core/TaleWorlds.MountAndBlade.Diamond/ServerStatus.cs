using System;

namespace TaleWorlds.MountAndBlade.Diamond
{
	// Token: 0x02000150 RID: 336
	[Serializable]
	public class ServerStatus
	{
		// Token: 0x170002EE RID: 750
		// (get) Token: 0x06000860 RID: 2144 RVA: 0x0000E34D File Offset: 0x0000C54D
		// (set) Token: 0x06000861 RID: 2145 RVA: 0x0000E355 File Offset: 0x0000C555
		public bool IsMatchmakingEnabled { get; private set; }

		// Token: 0x170002EF RID: 751
		// (get) Token: 0x06000862 RID: 2146 RVA: 0x0000E35E File Offset: 0x0000C55E
		// (set) Token: 0x06000863 RID: 2147 RVA: 0x0000E366 File Offset: 0x0000C566
		public bool IsCustomBattleEnabled { get; private set; }

		// Token: 0x170002F0 RID: 752
		// (get) Token: 0x06000864 RID: 2148 RVA: 0x0000E36F File Offset: 0x0000C56F
		// (set) Token: 0x06000865 RID: 2149 RVA: 0x0000E377 File Offset: 0x0000C577
		public bool IsPlayerBasedCustomBattleEnabled { get; private set; }

		// Token: 0x170002F1 RID: 753
		// (get) Token: 0x06000866 RID: 2150 RVA: 0x0000E380 File Offset: 0x0000C580
		// (set) Token: 0x06000867 RID: 2151 RVA: 0x0000E388 File Offset: 0x0000C588
		public bool IsPremadeGameEnabled { get; private set; }

		// Token: 0x170002F2 RID: 754
		// (get) Token: 0x06000868 RID: 2152 RVA: 0x0000E391 File Offset: 0x0000C591
		// (set) Token: 0x06000869 RID: 2153 RVA: 0x0000E399 File Offset: 0x0000C599
		public bool IsTestRegionEnabled { get; set; }

		// Token: 0x170002F3 RID: 755
		// (get) Token: 0x0600086A RID: 2154 RVA: 0x0000E3A2 File Offset: 0x0000C5A2
		// (set) Token: 0x0600086B RID: 2155 RVA: 0x0000E3AA File Offset: 0x0000C5AA
		public Announcement Announcement { get; private set; }

		// Token: 0x170002F4 RID: 756
		// (get) Token: 0x0600086C RID: 2156 RVA: 0x0000E3B3 File Offset: 0x0000C5B3
		public ServerNotification[] ServerNotifications { get; }

		// Token: 0x170002F5 RID: 757
		// (get) Token: 0x0600086D RID: 2157 RVA: 0x0000E3BB File Offset: 0x0000C5BB
		// (set) Token: 0x0600086E RID: 2158 RVA: 0x0000E3C3 File Offset: 0x0000C5C3
		public int FriendListUpdatePeriod { get; private set; }

		// Token: 0x0600086F RID: 2159 RVA: 0x0000E3CC File Offset: 0x0000C5CC
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
