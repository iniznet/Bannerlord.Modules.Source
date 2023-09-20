using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TaleWorlds.AchievementSystem;
using TaleWorlds.ActivitySystem;
using TaleWorlds.Diamond;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.PlayerServices;
using TaleWorlds.PlayerServices.Avatar;

namespace TaleWorlds.PlatformService
{
	public interface IPlatformServices
	{
		string ProviderName { get; }

		string UserId { get; }

		string UserDisplayName { get; }

		bool UserLoggedIn { get; }

		bool IsPermanentMuteAvailable { get; }

		IReadOnlyCollection<PlayerId> BlockedUsers { get; }

		void LoginUser();

		bool Initialize(IFriendListService[] additionalFriendListServices);

		PlatformInitParams GetInitParams();

		void Terminate();

		void Tick(float dt);

		Task<AvatarData> GetUserAvatar(PlayerId providedId);

		Task<bool> ShowOverlayForWebPage(string url);

		event Action<AvatarData> OnAvatarUpdated;

		event Action<string> OnNameUpdated;

		event Action<bool, TextObject> OnSignInStateUpdated;

		Task<ILoginAccessProvider> CreateLobbyClientLoginProvider();

		event Action OnBlockedUserListUpdated;

		IFriendListService[] GetFriendListServices();

		IAchievementService GetAchievementService();

		IActivityService GetActivityService();

		void CheckPrivilege(Privilege privilege, bool displayResolveUI, PrivilegeResult callback);

		void CheckPermissionWithUser(Permission permission, PlayerId targetPlayerId, PermissionResult callback);

		bool IsPlayerProfileCardAvailable(PlayerId providedId);

		void ShowPlayerProfileCard(PlayerId providedId);

		void GetPlatformId(PlayerId playerId, Action<object> callback);

		void OnFocusGained();

		void ShowRestrictedInformation();

		Task<bool> VerifyString(string content);

		bool RegisterPermissionChangeEvent(PlayerId targetPlayerId, Permission permission, PermissionChanged callback);

		bool UnregisterPermissionChangeEvent(PlayerId targetPlayerId, Permission permission, PermissionChanged callback);
	}
}
