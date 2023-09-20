using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Steamworks;
using TaleWorlds.AchievementSystem;
using TaleWorlds.ActivitySystem;
using TaleWorlds.Avatar.PlayerServices;
using TaleWorlds.Diamond;
using TaleWorlds.Diamond.AccessProvider.Steam;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.ModuleManager;
using TaleWorlds.PlayerServices;
using TaleWorlds.PlayerServices.Avatar;

namespace TaleWorlds.PlatformService.Steam
{
	public class SteamPlatformServices : IPlatformServices
	{
		private static SteamPlatformServices Instance
		{
			get
			{
				return PlatformServices.Instance as SteamPlatformServices;
			}
		}

		internal bool Initialized { get; private set; }

		public SteamPlatformServices(PlatformInitParams initParams)
		{
			this._initParams = initParams;
			AvatarServices.AddAvatarService(PlayerIdProvidedTypes.Steam, new SteamPlatformAvatarService(this));
			this._achievementService = new SteamAchievementService(this);
			this._steamFriendListService = new SteamFriendListService(this);
		}

		string IPlatformServices.ProviderName
		{
			get
			{
				return "Steam";
			}
		}

		string IPlatformServices.UserId
		{
			get
			{
				return ((ulong)SteamUser.GetSteamID()).ToString();
			}
		}

		bool IPlatformServices.UserLoggedIn
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		void IPlatformServices.LoginUser()
		{
			throw new NotImplementedException();
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
			this._friendListServices = new IFriendListService[additionalFriendListServices.Length + 1];
			this._friendListServices[0] = this._steamFriendListService;
			for (int i = 0; i < additionalFriendListServices.Length; i++)
			{
				this._friendListServices[i + 1] = additionalFriendListServices[i];
			}
			if (!SteamAPI.Init())
			{
				return false;
			}
			ModuleHelper.InitializePlatformModuleExtension(new SteamModuleExtension());
			this.InitCallbacks();
			this._achievementService.Initialize();
			SteamUserStats.RequestCurrentStats();
			this.Initialized = true;
			return true;
		}

		void IPlatformServices.Tick(float dt)
		{
			SteamAPI.RunCallbacks();
			this._achievementService.Tick(dt);
		}

		void IPlatformServices.Terminate()
		{
			SteamAPI.Shutdown();
		}

		public event Action<AvatarData> OnAvatarUpdated;

		public event Action<string> OnNameUpdated;

		public event Action<bool, TextObject> OnSignInStateUpdated;

		public event Action OnBlockedUserListUpdated;

		bool IPlatformServices.IsPlayerProfileCardAvailable(PlayerId providedId)
		{
			return false;
		}

		void IPlatformServices.ShowPlayerProfileCard(PlayerId providedId)
		{
			SteamFriends.ActivateGameOverlayToUser("steamid", providedId.ToSteamId());
		}

		async Task<AvatarData> IPlatformServices.GetUserAvatar(PlayerId providedId)
		{
			AvatarData avatarData;
			if (!providedId.IsValid)
			{
				avatarData = null;
			}
			else if (this._avatarCache.ContainsKey(providedId))
			{
				avatarData = this._avatarCache[providedId];
			}
			else
			{
				if (this._avatarCache.Count > 300)
				{
					this._avatarCache.Clear();
				}
				long startTime = DateTime.UtcNow.Ticks;
				CSteamID steamId = providedId.ToSteamId();
				if (SteamFriends.RequestUserInformation(steamId, false))
				{
					while (!SteamPlatformServices._avatarUpdates.Contains(steamId) && !this.TimedOut(startTime, 5000L))
					{
						await Task.Delay(5);
					}
					SteamPlatformServices._avatarUpdates.Remove(steamId);
				}
				int userAvatar = SteamFriends.GetLargeFriendAvatar(steamId);
				if (userAvatar == -1)
				{
					while (!SteamPlatformServices._avatarLoadedUpdates.Contains(steamId) && !this.TimedOut(startTime, 5000L))
					{
						await Task.Delay(5);
					}
					SteamPlatformServices._avatarLoadedUpdates.Remove(steamId);
					while (userAvatar == -1 && !this.TimedOut(startTime, 5000L))
					{
						userAvatar = SteamFriends.GetLargeFriendAvatar(steamId);
					}
				}
				if (userAvatar != -1)
				{
					uint num;
					uint num2;
					SteamUtils.GetImageSize(userAvatar, ref num, ref num2);
					if (num != 0U)
					{
						uint num3 = num * num2 * 4U;
						byte[] array = new byte[num3];
						if (SteamUtils.GetImageRGBA(userAvatar, array, (int)num3))
						{
							AvatarData avatarData2 = new AvatarData(array, num, num2);
							Dictionary<PlayerId, AvatarData> avatarCache = this._avatarCache;
							lock (avatarCache)
							{
								if (!this._avatarCache.ContainsKey(providedId))
								{
									this._avatarCache.Add(providedId, avatarData2);
								}
							}
							return avatarData2;
						}
					}
				}
				avatarData = null;
			}
			return avatarData;
		}

		public void ClearAvatarCache()
		{
			this._avatarCache.Clear();
		}

		private bool TimedOut(long startUTCTicks, long timeOut)
		{
			return (long)(DateTime.Now - new DateTime(startUTCTicks)).Milliseconds > timeOut;
		}

		internal async Task<string> GetUserName(PlayerId providedId)
		{
			string text;
			if (!providedId.IsValid || providedId.ProvidedType != PlayerIdProvidedTypes.Steam)
			{
				text = null;
			}
			else
			{
				long startTime = DateTime.UtcNow.Ticks;
				CSteamID steamId = providedId.ToSteamId();
				if (SteamFriends.RequestUserInformation(steamId, false))
				{
					while (!SteamPlatformServices._nameUpdates.Contains(steamId) && !this.TimedOut(startTime, 5000L))
					{
						await Task.Delay(5);
					}
					SteamPlatformServices._nameUpdates.Remove(steamId);
				}
				string friendPersonaName = SteamFriends.GetFriendPersonaName(steamId);
				if (!string.IsNullOrEmpty(friendPersonaName))
				{
					text = friendPersonaName;
				}
				else
				{
					text = null;
				}
			}
			return text;
		}

		PlatformInitParams IPlatformServices.GetInitParams()
		{
			return this._initParams;
		}

		IAchievementService IPlatformServices.GetAchievementService()
		{
			return this._achievementService;
		}

		IActivityService IPlatformServices.GetActivityService()
		{
			return new TestActivityService();
		}

		async Task<bool> IPlatformServices.ShowOverlayForWebPage(string url)
		{
			await Task.Delay(0);
			SteamFriends.ActivateGameOverlayToWebPage(url);
			return true;
		}

		void IPlatformServices.CheckPrivilege(Privilege privilege, bool displayResolveUI, PrivilegeResult callback)
		{
			callback(true);
		}

		void IPlatformServices.CheckPermissionWithUser(Permission privilege, PlayerId targetPlayerId, PermissionResult callback)
		{
			callback(true);
		}

		bool IPlatformServices.RegisterPermissionChangeEvent(PlayerId targetPlayerId, Permission permission, PermissionChanged callback)
		{
			return false;
		}

		bool IPlatformServices.UnregisterPermissionChangeEvent(PlayerId targetPlayerId, Permission permission, PermissionChanged callback)
		{
			return false;
		}

		void IPlatformServices.ShowRestrictedInformation()
		{
		}

		Task<bool> IPlatformServices.VerifyString(string content)
		{
			return Task.FromResult<bool>(true);
		}

		void IPlatformServices.GetPlatformId(PlayerId playerId, Action<object> callback)
		{
			callback(playerId.ToSteamId());
		}

		void IPlatformServices.OnFocusGained()
		{
		}

		internal Task<bool> GetUserOnlineStatus(PlayerId providedId)
		{
			SteamUtils.GetAppID();
			if (SteamFriends.GetFriendPersonaState(new CSteamID(providedId.Part4)) != null)
			{
				return Task.FromResult<bool>(true);
			}
			return Task.FromResult<bool>(false);
		}

		internal Task<bool> IsPlayingThisGame(PlayerId providedId)
		{
			AppId_t appID = SteamUtils.GetAppID();
			FriendGameInfo_t friendGameInfo_t;
			if (SteamFriends.GetFriendGamePlayed(new CSteamID(providedId.Part4), ref friendGameInfo_t) && friendGameInfo_t.m_gameID.AppID() == appID)
			{
				return Task.FromResult<bool>(true);
			}
			return Task.FromResult<bool>(false);
		}

		internal async Task<PlayerId> GetUserWithName(string name)
		{
			await Task.Delay(0);
			int num = SteamFriends.GetFriendCount(4);
			CSteamID csteamID = default(CSteamID);
			int num2 = 0;
			for (int i = 0; i < num; i++)
			{
				CSteamID friendByIndex = SteamFriends.GetFriendByIndex(i, 4);
				if (SteamFriends.GetFriendPersonaName(friendByIndex).Equals(name))
				{
					csteamID = friendByIndex;
					num2++;
				}
			}
			num = SteamFriends.GetCoplayFriendCount();
			for (int j = 0; j < num; j++)
			{
				CSteamID coplayFriend = SteamFriends.GetCoplayFriend(j);
				if (SteamFriends.GetFriendPersonaName(coplayFriend).Equals(name))
				{
					csteamID = coplayFriend;
					num2++;
				}
			}
			PlayerId playerId;
			if (num2 != 1)
			{
				playerId = default(PlayerId);
			}
			else
			{
				playerId = csteamID.ToPlayerId();
			}
			return playerId;
		}

		private async void OnAvatarUpdateReceived(ulong userId)
		{
			int userAvatar = -1;
			while (userAvatar == -1)
			{
				userAvatar = SteamFriends.GetLargeFriendAvatar(new CSteamID(userId));
				await Task.Delay(5);
			}
			if (userAvatar != -1)
			{
				uint num;
				uint num2;
				SteamUtils.GetImageSize(userAvatar, ref num, ref num2);
				if (num != 0U)
				{
					uint num3 = num * num2 * 4U;
					byte[] array = new byte[num3];
					if (SteamUtils.GetImageRGBA(userAvatar, array, (int)num3))
					{
						Action<AvatarData> onAvatarUpdated = this.OnAvatarUpdated;
						if (onAvatarUpdated != null)
						{
							onAvatarUpdated(new AvatarData(array, num, num2));
						}
					}
				}
			}
		}

		private void OnNameUpdateReceived(PlayerId userId)
		{
			string friendPersonaName = SteamFriends.GetFriendPersonaName(userId.ToSteamId());
			if (!string.IsNullOrEmpty(friendPersonaName))
			{
				Action<string> onNameUpdated = this.OnNameUpdated;
				if (onNameUpdated == null)
				{
					return;
				}
				onNameUpdated(friendPersonaName);
			}
		}

		private void Dummy()
		{
			if (this.OnSignInStateUpdated != null)
			{
				this.OnSignInStateUpdated(false, null);
			}
			if (this.OnBlockedUserListUpdated != null)
			{
				this.OnBlockedUserListUpdated();
			}
		}

		private void InitCallbacks()
		{
			this._personaStateChangeT = Callback<PersonaStateChange_t>.Create(new Callback<PersonaStateChange_t>.DispatchDelegate(SteamPlatformServices.UserInformationUpdated));
			this._avatarImageLoadedT = Callback<AvatarImageLoaded_t>.Create(new Callback<AvatarImageLoaded_t>.DispatchDelegate(SteamPlatformServices.AvatarLoaded));
		}

		private static void AvatarLoaded(AvatarImageLoaded_t avatarImageLoadedT)
		{
			SteamPlatformServices._avatarLoadedUpdates.Add(avatarImageLoadedT.m_steamID);
		}

		private static void UserInformationUpdated(PersonaStateChange_t pCallback)
		{
			if ((pCallback.m_nChangeFlags & 64) != null)
			{
				SteamPlatformServices._avatarUpdates.Add(new CSteamID(pCallback.m_ulSteamID));
				SteamPlatformServices.Instance.OnAvatarUpdateReceived(pCallback.m_ulSteamID);
				return;
			}
			if ((pCallback.m_nChangeFlags & 1) != null)
			{
				SteamPlatformServices._nameUpdates.Add(new CSteamID(pCallback.m_ulSteamID));
				SteamPlatformServices.Instance.OnNameUpdateReceived(new CSteamID(pCallback.m_ulSteamID).ToPlayerId());
				return;
			}
			if ((pCallback.m_nChangeFlags & 16) != null)
			{
				SteamPlatformServices.HandleOnUserStatusChanged(new CSteamID(pCallback.m_ulSteamID).ToPlayerId());
			}
		}

		private static void HandleOnUserStatusChanged(PlayerId playerId)
		{
			SteamPlatformServices.Instance._steamFriendListService.HandleOnUserStatusChanged(playerId);
		}

		Task<ILoginAccessProvider> IPlatformServices.CreateLobbyClientLoginProvider()
		{
			return Task.FromResult<ILoginAccessProvider>(new SteamLoginAccessProvider());
		}

		IFriendListService[] IPlatformServices.GetFriendListServices()
		{
			return this._friendListServices;
		}

		private PlatformInitParams _initParams;

		private SteamFriendListService _steamFriendListService;

		private IFriendListService[] _friendListServices;

		public SteamAchievementService _achievementService;

		private Dictionary<PlayerId, AvatarData> _avatarCache = new Dictionary<PlayerId, AvatarData>();

		private const int CommandRequestTimeOut = 5000;

		private Callback<PersonaStateChange_t> _personaStateChangeT;

		private Callback<AvatarImageLoaded_t> _avatarImageLoadedT;

		private static List<CSteamID> _avatarUpdates = new List<CSteamID>();

		private static List<CSteamID> _avatarLoadedUpdates = new List<CSteamID>();

		private static List<CSteamID> _nameUpdates = new List<CSteamID>();
	}
}
