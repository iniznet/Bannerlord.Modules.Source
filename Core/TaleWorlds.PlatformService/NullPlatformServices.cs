using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TaleWorlds.AchievementSystem;
using TaleWorlds.ActivitySystem;
using TaleWorlds.Diamond;
using TaleWorlds.Diamond.AccessProvider.Test;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.PlayerServices;
using TaleWorlds.PlayerServices.Avatar;

namespace TaleWorlds.PlatformService
{
	// Token: 0x0200000F RID: 15
	public class NullPlatformServices : IPlatformServices
	{
		// Token: 0x06000051 RID: 81 RVA: 0x00002054 File Offset: 0x00000254
		public NullPlatformServices()
		{
			this._testFriendListService = new TestFriendListService("NULL", default(PlayerId));
		}

		// Token: 0x1700000B RID: 11
		// (get) Token: 0x06000052 RID: 82 RVA: 0x00002080 File Offset: 0x00000280
		string IPlatformServices.ProviderName
		{
			get
			{
				return "Null";
			}
		}

		// Token: 0x1700000C RID: 12
		// (get) Token: 0x06000053 RID: 83 RVA: 0x00002087 File Offset: 0x00000287
		string IPlatformServices.UserId
		{
			get
			{
				return "";
			}
		}

		// Token: 0x1700000D RID: 13
		// (get) Token: 0x06000054 RID: 84 RVA: 0x0000208E File Offset: 0x0000028E
		string IPlatformServices.UserDisplayName
		{
			get
			{
				return "";
			}
		}

		// Token: 0x1700000E RID: 14
		// (get) Token: 0x06000055 RID: 85 RVA: 0x00002095 File Offset: 0x00000295
		IReadOnlyCollection<PlayerId> IPlatformServices.BlockedUsers
		{
			get
			{
				return new List<PlayerId>();
			}
		}

		// Token: 0x1700000F RID: 15
		// (get) Token: 0x06000056 RID: 86 RVA: 0x0000209C File Offset: 0x0000029C
		bool IPlatformServices.IsPermanentMuteAvailable
		{
			get
			{
				return true;
			}
		}

		// Token: 0x06000057 RID: 87 RVA: 0x0000209F File Offset: 0x0000029F
		bool IPlatformServices.Initialize(IFriendListService[] additionalFriendListServices)
		{
			return false;
		}

		// Token: 0x06000058 RID: 88 RVA: 0x000020A2 File Offset: 0x000002A2
		void IPlatformServices.Terminate()
		{
		}

		// Token: 0x06000059 RID: 89 RVA: 0x000020A4 File Offset: 0x000002A4
		bool IPlatformServices.IsPlayerProfileCardAvailable(PlayerId providedId)
		{
			return false;
		}

		// Token: 0x0600005A RID: 90 RVA: 0x000020A7 File Offset: 0x000002A7
		void IPlatformServices.ShowPlayerProfileCard(PlayerId providedId)
		{
		}

		// Token: 0x17000010 RID: 16
		// (get) Token: 0x0600005B RID: 91 RVA: 0x000020A9 File Offset: 0x000002A9
		bool IPlatformServices.UserLoggedIn
		{
			get
			{
				return false;
			}
		}

		// Token: 0x0600005C RID: 92 RVA: 0x000020AC File Offset: 0x000002AC
		void IPlatformServices.LoginUser()
		{
		}

		// Token: 0x0600005D RID: 93 RVA: 0x000020AE File Offset: 0x000002AE
		Task<AvatarData> IPlatformServices.GetUserAvatar(PlayerId providedId)
		{
			return Task.FromResult<AvatarData>(null);
		}

		// Token: 0x0600005E RID: 94 RVA: 0x000020B6 File Offset: 0x000002B6
		Task<bool> IPlatformServices.ShowOverlayForWebPage(string url)
		{
			return Task.FromResult<bool>(false);
		}

		// Token: 0x14000008 RID: 8
		// (add) Token: 0x0600005F RID: 95 RVA: 0x000020C0 File Offset: 0x000002C0
		// (remove) Token: 0x06000060 RID: 96 RVA: 0x000020F8 File Offset: 0x000002F8
		public event Action<PlayerId> OnUserStatusChanged;

		// Token: 0x14000009 RID: 9
		// (add) Token: 0x06000061 RID: 97 RVA: 0x00002130 File Offset: 0x00000330
		// (remove) Token: 0x06000062 RID: 98 RVA: 0x00002168 File Offset: 0x00000368
		public event Action<AvatarData> OnAvatarUpdated;

		// Token: 0x1400000A RID: 10
		// (add) Token: 0x06000063 RID: 99 RVA: 0x000021A0 File Offset: 0x000003A0
		// (remove) Token: 0x06000064 RID: 100 RVA: 0x000021D8 File Offset: 0x000003D8
		public event Action OnBlockedUserListUpdated;

		// Token: 0x1400000B RID: 11
		// (add) Token: 0x06000065 RID: 101 RVA: 0x00002210 File Offset: 0x00000410
		// (remove) Token: 0x06000066 RID: 102 RVA: 0x00002248 File Offset: 0x00000448
		public event Action<string> OnNameUpdated;

		// Token: 0x1400000C RID: 12
		// (add) Token: 0x06000067 RID: 103 RVA: 0x00002280 File Offset: 0x00000480
		// (remove) Token: 0x06000068 RID: 104 RVA: 0x000022B8 File Offset: 0x000004B8
		public event Action<bool, TextObject> OnSignInStateUpdated;

		// Token: 0x06000069 RID: 105 RVA: 0x000022F0 File Offset: 0x000004F0
		private void Dummy()
		{
			if (this.OnAvatarUpdated != null)
			{
				this.OnAvatarUpdated(null);
			}
			if (this.OnNameUpdated != null)
			{
				this.OnNameUpdated(null);
			}
			if (this.OnUserStatusChanged != null)
			{
				this.OnUserStatusChanged(default(PlayerId));
			}
			if (this.OnSignInStateUpdated != null)
			{
				this.OnSignInStateUpdated(false, null);
			}
			if (this.OnBlockedUserListUpdated != null)
			{
				this.OnBlockedUserListUpdated();
			}
		}

		// Token: 0x0600006A RID: 106 RVA: 0x00002369 File Offset: 0x00000569
		public void Tick(float dt)
		{
		}

		// Token: 0x0600006B RID: 107 RVA: 0x0000236B File Offset: 0x0000056B
		PlatformInitParams IPlatformServices.GetInitParams()
		{
			return new PlatformInitParams();
		}

		// Token: 0x0600006C RID: 108 RVA: 0x00002372 File Offset: 0x00000572
		IFriendListService[] IPlatformServices.GetFriendListServices()
		{
			return new IFriendListService[] { this._testFriendListService };
		}

		// Token: 0x0600006D RID: 109 RVA: 0x00002383 File Offset: 0x00000583
		public void ActivateFriendList()
		{
		}

		// Token: 0x0600006E RID: 110 RVA: 0x00002385 File Offset: 0x00000585
		Task<ILoginAccessProvider> IPlatformServices.CreateLobbyClientLoginProvider()
		{
			return Task.FromResult<ILoginAccessProvider>(new TestLoginAccessProvider());
		}

		// Token: 0x0600006F RID: 111 RVA: 0x00002391 File Offset: 0x00000591
		IAchievementService IPlatformServices.GetAchievementService()
		{
			return new TestAchievementService();
		}

		// Token: 0x06000070 RID: 112 RVA: 0x00002398 File Offset: 0x00000598
		IActivityService IPlatformServices.GetActivityService()
		{
			return new TestActivityService();
		}

		// Token: 0x06000071 RID: 113 RVA: 0x0000239F File Offset: 0x0000059F
		void IPlatformServices.CheckPrivilege(Privilege privilege, bool displayResolveUI, PrivilegeResult callback)
		{
			callback(true);
		}

		// Token: 0x06000072 RID: 114 RVA: 0x000023A8 File Offset: 0x000005A8
		void IPlatformServices.CheckPermissionWithUser(Permission privilege, PlayerId targetPlayerId, PermissionResult callback)
		{
			callback(true);
		}

		// Token: 0x06000073 RID: 115 RVA: 0x000023B1 File Offset: 0x000005B1
		void IPlatformServices.GetPlatformId(PlayerId playerId, Action<object> callback)
		{
			callback(-1);
		}

		// Token: 0x06000074 RID: 116 RVA: 0x000023BF File Offset: 0x000005BF
		Task<bool> IPlatformServices.VerifyString(string content)
		{
			return Task.FromResult<bool>(true);
		}

		// Token: 0x06000075 RID: 117 RVA: 0x000023C7 File Offset: 0x000005C7
		void IPlatformServices.ShowRestrictedInformation()
		{
		}

		// Token: 0x06000076 RID: 118 RVA: 0x000023C9 File Offset: 0x000005C9
		void IPlatformServices.OnFocusGained()
		{
		}

		// Token: 0x06000077 RID: 119 RVA: 0x000023CB File Offset: 0x000005CB
		bool IPlatformServices.RegisterPermissionChangeEvent(PlayerId targetPlayerId, Permission permission, PermissionChanged Callback)
		{
			return true;
		}

		// Token: 0x06000078 RID: 120 RVA: 0x000023CE File Offset: 0x000005CE
		bool IPlatformServices.UnregisterPermissionChangeEvent(PlayerId targetPlayerId, Permission permission, PermissionChanged Callback)
		{
			return true;
		}

		// Token: 0x0400001D RID: 29
		private TestFriendListService _testFriendListService;
	}
}
