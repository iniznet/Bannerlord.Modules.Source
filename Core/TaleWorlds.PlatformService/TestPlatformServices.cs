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
	// Token: 0x02000013 RID: 19
	public class TestPlatformServices : IPlatformServices
	{
		// Token: 0x060000AC RID: 172 RVA: 0x00002924 File Offset: 0x00000B24
		public TestPlatformServices(string userName)
		{
			this._userName = userName;
			this._loginAccessProvider = new TestLoginAccessProvider();
			ILoginAccessProvider loginAccessProvider = this._loginAccessProvider;
			loginAccessProvider.Initialize(this._userName, null);
			this._playerId = loginAccessProvider.GetPlayerId();
			this._testFriendListService = new TestFriendListService(userName, this._playerId);
		}

		// Token: 0x1700001E RID: 30
		// (get) Token: 0x060000AD RID: 173 RVA: 0x0000297B File Offset: 0x00000B7B
		string IPlatformServices.ProviderName
		{
			get
			{
				return "Test";
			}
		}

		// Token: 0x1700001F RID: 31
		// (get) Token: 0x060000AE RID: 174 RVA: 0x00002984 File Offset: 0x00000B84
		string IPlatformServices.UserId
		{
			get
			{
				return this._playerId.ToString();
			}
		}

		// Token: 0x17000020 RID: 32
		// (get) Token: 0x060000AF RID: 175 RVA: 0x000029A5 File Offset: 0x00000BA5
		string IPlatformServices.UserDisplayName
		{
			get
			{
				return this._userName;
			}
		}

		// Token: 0x17000021 RID: 33
		// (get) Token: 0x060000B0 RID: 176 RVA: 0x000029AD File Offset: 0x00000BAD
		IReadOnlyCollection<PlayerId> IPlatformServices.BlockedUsers
		{
			get
			{
				return new List<PlayerId>();
			}
		}

		// Token: 0x17000022 RID: 34
		// (get) Token: 0x060000B1 RID: 177 RVA: 0x000029B4 File Offset: 0x00000BB4
		bool IPlatformServices.IsPermanentMuteAvailable
		{
			get
			{
				return true;
			}
		}

		// Token: 0x060000B2 RID: 178 RVA: 0x000029B7 File Offset: 0x00000BB7
		bool IPlatformServices.Initialize(IFriendListService[] additionalFriendListServices)
		{
			return false;
		}

		// Token: 0x060000B3 RID: 179 RVA: 0x000029BA File Offset: 0x00000BBA
		void IPlatformServices.Terminate()
		{
		}

		// Token: 0x060000B4 RID: 180 RVA: 0x000029BC File Offset: 0x00000BBC
		bool IPlatformServices.IsPlayerProfileCardAvailable(PlayerId providedId)
		{
			return false;
		}

		// Token: 0x17000023 RID: 35
		// (get) Token: 0x060000B5 RID: 181 RVA: 0x000029BF File Offset: 0x00000BBF
		bool IPlatformServices.UserLoggedIn
		{
			get
			{
				return true;
			}
		}

		// Token: 0x060000B6 RID: 182 RVA: 0x000029C2 File Offset: 0x00000BC2
		void IPlatformServices.LoginUser()
		{
		}

		// Token: 0x060000B7 RID: 183 RVA: 0x000029C4 File Offset: 0x00000BC4
		void IPlatformServices.ShowPlayerProfileCard(PlayerId providedId)
		{
		}

		// Token: 0x060000B8 RID: 184 RVA: 0x000029C6 File Offset: 0x00000BC6
		Task<AvatarData> IPlatformServices.GetUserAvatar(PlayerId providedId)
		{
			return Task.FromResult<AvatarData>(null);
		}

		// Token: 0x060000B9 RID: 185 RVA: 0x000029CE File Offset: 0x00000BCE
		IFriendListService[] IPlatformServices.GetFriendListServices()
		{
			return new IFriendListService[] { this._testFriendListService };
		}

		// Token: 0x060000BA RID: 186 RVA: 0x000029DF File Offset: 0x00000BDF
		IAchievementService IPlatformServices.GetAchievementService()
		{
			return new TestAchievementService();
		}

		// Token: 0x060000BB RID: 187 RVA: 0x000029E6 File Offset: 0x00000BE6
		IActivityService IPlatformServices.GetActivityService()
		{
			return new TestActivityService();
		}

		// Token: 0x060000BC RID: 188 RVA: 0x000029ED File Offset: 0x00000BED
		Task<bool> IPlatformServices.ShowOverlayForWebPage(string url)
		{
			return Task.FromResult<bool>(false);
		}

		// Token: 0x14000010 RID: 16
		// (add) Token: 0x060000BD RID: 189 RVA: 0x000029F8 File Offset: 0x00000BF8
		// (remove) Token: 0x060000BE RID: 190 RVA: 0x00002A30 File Offset: 0x00000C30
		public event Action<AvatarData> OnAvatarUpdated;

		// Token: 0x14000011 RID: 17
		// (add) Token: 0x060000BF RID: 191 RVA: 0x00002A68 File Offset: 0x00000C68
		// (remove) Token: 0x060000C0 RID: 192 RVA: 0x00002AA0 File Offset: 0x00000CA0
		public event Action<string> OnNameUpdated;

		// Token: 0x14000012 RID: 18
		// (add) Token: 0x060000C1 RID: 193 RVA: 0x00002AD8 File Offset: 0x00000CD8
		// (remove) Token: 0x060000C2 RID: 194 RVA: 0x00002B10 File Offset: 0x00000D10
		public event Action<bool, TextObject> OnSignInStateUpdated;

		// Token: 0x060000C3 RID: 195 RVA: 0x00002B48 File Offset: 0x00000D48
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
			if (this.OnSignInStateUpdated != null)
			{
				this.OnSignInStateUpdated(false, null);
			}
			if (this.OnBlockedUserListUpdated != null)
			{
				this.OnBlockedUserListUpdated();
			}
		}

		// Token: 0x14000013 RID: 19
		// (add) Token: 0x060000C4 RID: 196 RVA: 0x00002BA8 File Offset: 0x00000DA8
		// (remove) Token: 0x060000C5 RID: 197 RVA: 0x00002BE0 File Offset: 0x00000DE0
		public event Action OnBlockedUserListUpdated;

		// Token: 0x060000C6 RID: 198 RVA: 0x00002C15 File Offset: 0x00000E15
		public void Tick(float dt)
		{
		}

		// Token: 0x060000C7 RID: 199 RVA: 0x00002C17 File Offset: 0x00000E17
		PlatformInitParams IPlatformServices.GetInitParams()
		{
			return new PlatformInitParams();
		}

		// Token: 0x060000C8 RID: 200 RVA: 0x00002C1E File Offset: 0x00000E1E
		public void ActivateFriendList()
		{
		}

		// Token: 0x060000C9 RID: 201 RVA: 0x00002C20 File Offset: 0x00000E20
		Task<ILoginAccessProvider> IPlatformServices.CreateLobbyClientLoginProvider()
		{
			return Task.FromResult<ILoginAccessProvider>(this._loginAccessProvider);
		}

		// Token: 0x060000CA RID: 202 RVA: 0x00002C2D File Offset: 0x00000E2D
		void IPlatformServices.CheckPrivilege(Privilege privilege, bool displayResolveUI, PrivilegeResult callback)
		{
			callback(true);
		}

		// Token: 0x060000CB RID: 203 RVA: 0x00002C36 File Offset: 0x00000E36
		void IPlatformServices.CheckPermissionWithUser(Permission privilege, PlayerId targetPlayerId, PermissionResult callback)
		{
			callback(true);
		}

		// Token: 0x060000CC RID: 204 RVA: 0x00002C3F File Offset: 0x00000E3F
		Task<bool> IPlatformServices.VerifyString(string content)
		{
			return Task.FromResult<bool>(true);
		}

		// Token: 0x060000CD RID: 205 RVA: 0x00002C47 File Offset: 0x00000E47
		void IPlatformServices.GetPlatformId(PlayerId playerId, Action<object> callback)
		{
			callback(0);
		}

		// Token: 0x060000CE RID: 206 RVA: 0x00002C55 File Offset: 0x00000E55
		void IPlatformServices.ShowRestrictedInformation()
		{
		}

		// Token: 0x060000CF RID: 207 RVA: 0x00002C57 File Offset: 0x00000E57
		void IPlatformServices.OnFocusGained()
		{
		}

		// Token: 0x060000D0 RID: 208 RVA: 0x00002C59 File Offset: 0x00000E59
		bool IPlatformServices.RegisterPermissionChangeEvent(PlayerId targetPlayerId, Permission permission, PermissionChanged callback)
		{
			return true;
		}

		// Token: 0x060000D1 RID: 209 RVA: 0x00002C5C File Offset: 0x00000E5C
		bool IPlatformServices.UnregisterPermissionChangeEvent(PlayerId targetPlayerId, Permission permission, PermissionChanged callback)
		{
			return true;
		}

		// Token: 0x04000036 RID: 54
		private readonly string _userName;

		// Token: 0x04000037 RID: 55
		private readonly PlayerId _playerId;

		// Token: 0x04000038 RID: 56
		private TestLoginAccessProvider _loginAccessProvider;

		// Token: 0x04000039 RID: 57
		private TestFriendListService _testFriendListService;
	}
}
