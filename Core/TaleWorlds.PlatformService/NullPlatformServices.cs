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
	public class NullPlatformServices : IPlatformServices
	{
		public NullPlatformServices()
		{
			this._testFriendListService = new TestFriendListService("NULL", default(PlayerId));
		}

		string IPlatformServices.ProviderName
		{
			get
			{
				return "Null";
			}
		}

		string IPlatformServices.UserId
		{
			get
			{
				return "";
			}
		}

		string IPlatformServices.UserDisplayName
		{
			get
			{
				return "";
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

		void IPlatformServices.ShowPlayerProfileCard(PlayerId providedId)
		{
		}

		bool IPlatformServices.UserLoggedIn
		{
			get
			{
				return false;
			}
		}

		void IPlatformServices.LoginUser()
		{
		}

		Task<AvatarData> IPlatformServices.GetUserAvatar(PlayerId providedId)
		{
			return Task.FromResult<AvatarData>(null);
		}

		Task<bool> IPlatformServices.ShowOverlayForWebPage(string url)
		{
			return Task.FromResult<bool>(false);
		}

		public event Action<PlayerId> OnUserStatusChanged;

		public event Action<AvatarData> OnAvatarUpdated;

		public event Action OnBlockedUserListUpdated;

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

		public void Tick(float dt)
		{
		}

		PlatformInitParams IPlatformServices.GetInitParams()
		{
			return new PlatformInitParams();
		}

		IFriendListService[] IPlatformServices.GetFriendListServices()
		{
			return new IFriendListService[] { this._testFriendListService };
		}

		public void ActivateFriendList()
		{
		}

		Task<ILoginAccessProvider> IPlatformServices.CreateLobbyClientLoginProvider()
		{
			return Task.FromResult<ILoginAccessProvider>(new TestLoginAccessProvider());
		}

		IAchievementService IPlatformServices.GetAchievementService()
		{
			return new TestAchievementService();
		}

		IActivityService IPlatformServices.GetActivityService()
		{
			return new TestActivityService();
		}

		void IPlatformServices.CheckPrivilege(Privilege privilege, bool displayResolveUI, PrivilegeResult callback)
		{
			callback(true);
		}

		void IPlatformServices.CheckPermissionWithUser(Permission privilege, PlayerId targetPlayerId, PermissionResult callback)
		{
			callback(true);
		}

		void IPlatformServices.GetPlatformId(PlayerId playerId, Action<object> callback)
		{
			callback(-1);
		}

		Task<bool> IPlatformServices.VerifyString(string content)
		{
			return Task.FromResult<bool>(true);
		}

		void IPlatformServices.ShowRestrictedInformation()
		{
		}

		void IPlatformServices.OnFocusGained()
		{
		}

		bool IPlatformServices.RegisterPermissionChangeEvent(PlayerId targetPlayerId, Permission permission, PermissionChanged Callback)
		{
			return true;
		}

		bool IPlatformServices.UnregisterPermissionChangeEvent(PlayerId targetPlayerId, Permission permission, PermissionChanged Callback)
		{
			return true;
		}

		private TestFriendListService _testFriendListService;
	}
}
