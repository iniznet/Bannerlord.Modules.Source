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
	public class TestPlatformServices : IPlatformServices
	{
		public TestPlatformServices(string userName)
		{
			this._userName = userName;
			this._loginAccessProvider = new TestLoginAccessProvider();
			ILoginAccessProvider loginAccessProvider = this._loginAccessProvider;
			loginAccessProvider.Initialize(this._userName, null);
			this._playerId = loginAccessProvider.GetPlayerId();
			this._testFriendListService = new TestFriendListService(userName, this._playerId);
		}

		string IPlatformServices.ProviderName
		{
			get
			{
				return "Test";
			}
		}

		string IPlatformServices.UserId
		{
			get
			{
				return this._playerId.ToString();
			}
		}

		string IPlatformServices.UserDisplayName
		{
			get
			{
				return this._userName;
			}
		}

		IReadOnlyCollection<PlayerId> IPlatformServices.BlockedUsers
		{
			get
			{
				return new List<PlayerId>();
			}
		}

		bool IPlatformServices.IsPermanentMuteAvailable
		{
			get
			{
				return true;
			}
		}

		bool IPlatformServices.Initialize(IFriendListService[] additionalFriendListServices)
		{
			return false;
		}

		void IPlatformServices.Terminate()
		{
		}

		bool IPlatformServices.IsPlayerProfileCardAvailable(PlayerId providedId)
		{
			return false;
		}

		bool IPlatformServices.UserLoggedIn
		{
			get
			{
				return true;
			}
		}

		void IPlatformServices.LoginUser()
		{
		}

		void IPlatformServices.ShowPlayerProfileCard(PlayerId providedId)
		{
		}

		Task<AvatarData> IPlatformServices.GetUserAvatar(PlayerId providedId)
		{
			return Task.FromResult<AvatarData>(null);
		}

		IFriendListService[] IPlatformServices.GetFriendListServices()
		{
			return new IFriendListService[] { this._testFriendListService };
		}

		IAchievementService IPlatformServices.GetAchievementService()
		{
			return new TestAchievementService();
		}

		IActivityService IPlatformServices.GetActivityService()
		{
			return new TestActivityService();
		}

		Task<bool> IPlatformServices.ShowOverlayForWebPage(string url)
		{
			return Task.FromResult<bool>(false);
		}

		public event Action<AvatarData> OnAvatarUpdated;

		public event Action<string> OnNameUpdated;

		public event Action<bool, TextObject> OnSignInStateUpdated;

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

		public event Action OnBlockedUserListUpdated;

		public void Tick(float dt)
		{
		}

		PlatformInitParams IPlatformServices.GetInitParams()
		{
			return new PlatformInitParams();
		}

		public void ActivateFriendList()
		{
		}

		Task<ILoginAccessProvider> IPlatformServices.CreateLobbyClientLoginProvider()
		{
			return Task.FromResult<ILoginAccessProvider>(this._loginAccessProvider);
		}

		void IPlatformServices.CheckPrivilege(Privilege privilege, bool displayResolveUI, PrivilegeResult callback)
		{
			callback(true);
		}

		void IPlatformServices.CheckPermissionWithUser(Permission privilege, PlayerId targetPlayerId, PermissionResult callback)
		{
			callback(true);
		}

		Task<bool> IPlatformServices.VerifyString(string content)
		{
			return Task.FromResult<bool>(true);
		}

		void IPlatformServices.GetPlatformId(PlayerId playerId, Action<object> callback)
		{
			callback(0);
		}

		void IPlatformServices.ShowRestrictedInformation()
		{
		}

		void IPlatformServices.OnFocusGained()
		{
		}

		bool IPlatformServices.RegisterPermissionChangeEvent(PlayerId targetPlayerId, Permission permission, PermissionChanged callback)
		{
			return true;
		}

		bool IPlatformServices.UnregisterPermissionChangeEvent(PlayerId targetPlayerId, Permission permission, PermissionChanged callback)
		{
			return true;
		}

		private readonly string _userName;

		private readonly PlayerId _playerId;

		private TestLoginAccessProvider _loginAccessProvider;

		private TestFriendListService _testFriendListService;
	}
}
