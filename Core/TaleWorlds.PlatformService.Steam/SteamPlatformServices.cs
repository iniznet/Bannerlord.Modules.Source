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
	// Token: 0x02000006 RID: 6
	public class SteamPlatformServices : IPlatformServices
	{
		// Token: 0x17000005 RID: 5
		// (get) Token: 0x06000031 RID: 49 RVA: 0x00002531 File Offset: 0x00000731
		private static SteamPlatformServices Instance
		{
			get
			{
				return PlatformServices.Instance as SteamPlatformServices;
			}
		}

		// Token: 0x17000006 RID: 6
		// (get) Token: 0x06000032 RID: 50 RVA: 0x0000253D File Offset: 0x0000073D
		// (set) Token: 0x06000033 RID: 51 RVA: 0x00002545 File Offset: 0x00000745
		internal bool Initialized { get; private set; }

		// Token: 0x06000034 RID: 52 RVA: 0x0000254E File Offset: 0x0000074E
		public SteamPlatformServices(PlatformInitParams initParams)
		{
			this._initParams = initParams;
			AvatarServices.AddAvatarService(PlayerIdProvidedTypes.Steam, new SteamPlatformAvatarService(this));
			this._achievementService = new SteamAchievementService(this);
			this._steamFriendListService = new SteamFriendListService(this);
		}

		// Token: 0x17000007 RID: 7
		// (get) Token: 0x06000035 RID: 53 RVA: 0x0000258C File Offset: 0x0000078C
		string IPlatformServices.ProviderName
		{
			get
			{
				return "Steam";
			}
		}

		// Token: 0x17000008 RID: 8
		// (get) Token: 0x06000036 RID: 54 RVA: 0x00002594 File Offset: 0x00000794
		string IPlatformServices.UserId
		{
			get
			{
				return ((ulong)SteamUser.GetSteamID()).ToString();
			}
		}

		// Token: 0x17000009 RID: 9
		// (get) Token: 0x06000037 RID: 55 RVA: 0x000025B3 File Offset: 0x000007B3
		bool IPlatformServices.UserLoggedIn
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		// Token: 0x06000038 RID: 56 RVA: 0x000025BA File Offset: 0x000007BA
		void IPlatformServices.LoginUser()
		{
			throw new NotImplementedException();
		}

		// Token: 0x1700000A RID: 10
		// (get) Token: 0x06000039 RID: 57 RVA: 0x000025C1 File Offset: 0x000007C1
		string IPlatformServices.UserDisplayName
		{
			get
			{
				return "";
			}
		}

		// Token: 0x1700000B RID: 11
		// (get) Token: 0x0600003A RID: 58 RVA: 0x000025C8 File Offset: 0x000007C8
		IReadOnlyCollection<PlayerId> IPlatformServices.BlockedUsers
		{
			get
			{
				return new List<PlayerId>();
			}
		}

		// Token: 0x1700000C RID: 12
		// (get) Token: 0x0600003B RID: 59 RVA: 0x000025CF File Offset: 0x000007CF
		bool IPlatformServices.IsPermanentMuteAvailable
		{
			get
			{
				return true;
			}
		}

		// Token: 0x0600003C RID: 60 RVA: 0x000025D4 File Offset: 0x000007D4
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

		// Token: 0x0600003D RID: 61 RVA: 0x0000264C File Offset: 0x0000084C
		void IPlatformServices.Tick(float dt)
		{
			SteamAPI.RunCallbacks();
			this._achievementService.Tick(dt);
		}

		// Token: 0x0600003E RID: 62 RVA: 0x0000265F File Offset: 0x0000085F
		void IPlatformServices.Terminate()
		{
			SteamAPI.Shutdown();
		}

		// Token: 0x14000004 RID: 4
		// (add) Token: 0x0600003F RID: 63 RVA: 0x00002668 File Offset: 0x00000868
		// (remove) Token: 0x06000040 RID: 64 RVA: 0x000026A0 File Offset: 0x000008A0
		public event Action<AvatarData> OnAvatarUpdated;

		// Token: 0x14000005 RID: 5
		// (add) Token: 0x06000041 RID: 65 RVA: 0x000026D8 File Offset: 0x000008D8
		// (remove) Token: 0x06000042 RID: 66 RVA: 0x00002710 File Offset: 0x00000910
		public event Action<string> OnNameUpdated;

		// Token: 0x14000006 RID: 6
		// (add) Token: 0x06000043 RID: 67 RVA: 0x00002748 File Offset: 0x00000948
		// (remove) Token: 0x06000044 RID: 68 RVA: 0x00002780 File Offset: 0x00000980
		public event Action<bool, TextObject> OnSignInStateUpdated;

		// Token: 0x14000007 RID: 7
		// (add) Token: 0x06000045 RID: 69 RVA: 0x000027B8 File Offset: 0x000009B8
		// (remove) Token: 0x06000046 RID: 70 RVA: 0x000027F0 File Offset: 0x000009F0
		public event Action OnBlockedUserListUpdated;

		// Token: 0x06000047 RID: 71 RVA: 0x00002825 File Offset: 0x00000A25
		bool IPlatformServices.IsPlayerProfileCardAvailable(PlayerId providedId)
		{
			return false;
		}

		// Token: 0x06000048 RID: 72 RVA: 0x00002828 File Offset: 0x00000A28
		void IPlatformServices.ShowPlayerProfileCard(PlayerId providedId)
		{
			SteamFriends.ActivateGameOverlayToUser("steamid", providedId.ToSteamId());
		}

		// Token: 0x06000049 RID: 73 RVA: 0x0000283C File Offset: 0x00000A3C
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
					SteamUtils.GetImageSize(userAvatar, out num, out num2);
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

		// Token: 0x0600004A RID: 74 RVA: 0x00002889 File Offset: 0x00000A89
		public void ClearAvatarCache()
		{
			this._avatarCache.Clear();
		}

		// Token: 0x0600004B RID: 75 RVA: 0x00002898 File Offset: 0x00000A98
		private bool TimedOut(long startUTCTicks, long timeOut)
		{
			return (long)(DateTime.Now - new DateTime(startUTCTicks)).Milliseconds > timeOut;
		}

		// Token: 0x0600004C RID: 76 RVA: 0x000028C4 File Offset: 0x00000AC4
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

		// Token: 0x0600004D RID: 77 RVA: 0x00002911 File Offset: 0x00000B11
		PlatformInitParams IPlatformServices.GetInitParams()
		{
			return this._initParams;
		}

		// Token: 0x0600004E RID: 78 RVA: 0x00002919 File Offset: 0x00000B19
		IAchievementService IPlatformServices.GetAchievementService()
		{
			return this._achievementService;
		}

		// Token: 0x0600004F RID: 79 RVA: 0x00002921 File Offset: 0x00000B21
		IActivityService IPlatformServices.GetActivityService()
		{
			return new TestActivityService();
		}

		// Token: 0x06000050 RID: 80 RVA: 0x00002928 File Offset: 0x00000B28
		async Task<bool> IPlatformServices.ShowOverlayForWebPage(string url)
		{
			await Task.Delay(0);
			SteamFriends.ActivateGameOverlayToWebPage(url);
			return true;
		}

		// Token: 0x06000051 RID: 81 RVA: 0x0000296D File Offset: 0x00000B6D
		void IPlatformServices.CheckPrivilege(Privilege privilege, bool displayResolveUI, PrivilegeResult callback)
		{
			callback(true);
		}

		// Token: 0x06000052 RID: 82 RVA: 0x00002976 File Offset: 0x00000B76
		void IPlatformServices.CheckPermissionWithUser(Permission privilege, PlayerId targetPlayerId, PermissionResult callback)
		{
			callback(true);
		}

		// Token: 0x06000053 RID: 83 RVA: 0x0000297F File Offset: 0x00000B7F
		bool IPlatformServices.RegisterPermissionChangeEvent(PlayerId targetPlayerId, Permission permission, PermissionChanged callback)
		{
			return false;
		}

		// Token: 0x06000054 RID: 84 RVA: 0x00002982 File Offset: 0x00000B82
		bool IPlatformServices.UnregisterPermissionChangeEvent(PlayerId targetPlayerId, Permission permission, PermissionChanged callback)
		{
			return false;
		}

		// Token: 0x06000055 RID: 85 RVA: 0x00002985 File Offset: 0x00000B85
		void IPlatformServices.ShowRestrictedInformation()
		{
		}

		// Token: 0x06000056 RID: 86 RVA: 0x00002987 File Offset: 0x00000B87
		Task<bool> IPlatformServices.VerifyString(string content)
		{
			return Task.FromResult<bool>(true);
		}

		// Token: 0x06000057 RID: 87 RVA: 0x0000298F File Offset: 0x00000B8F
		void IPlatformServices.GetPlatformId(PlayerId playerId, Action<object> callback)
		{
			callback(playerId.ToSteamId());
		}

		// Token: 0x06000058 RID: 88 RVA: 0x000029A2 File Offset: 0x00000BA2
		void IPlatformServices.OnFocusGained()
		{
		}

		// Token: 0x06000059 RID: 89 RVA: 0x000029A4 File Offset: 0x00000BA4
		internal Task<bool> GetUserOnlineStatus(PlayerId providedId)
		{
			SteamUtils.GetAppID();
			if (SteamFriends.GetFriendPersonaState(new CSteamID(providedId.Part4)) != EPersonaState.k_EPersonaStateOffline)
			{
				return Task.FromResult<bool>(true);
			}
			return Task.FromResult<bool>(false);
		}

		// Token: 0x0600005A RID: 90 RVA: 0x000029CC File Offset: 0x00000BCC
		internal Task<bool> IsPlayingThisGame(PlayerId providedId)
		{
			AppId_t appID = SteamUtils.GetAppID();
			FriendGameInfo_t friendGameInfo_t;
			if (SteamFriends.GetFriendGamePlayed(new CSteamID(providedId.Part4), out friendGameInfo_t) && friendGameInfo_t.m_gameID.AppID() == appID)
			{
				return Task.FromResult<bool>(true);
			}
			return Task.FromResult<bool>(false);
		}

		// Token: 0x0600005B RID: 91 RVA: 0x00002A18 File Offset: 0x00000C18
		internal async Task<PlayerId> GetUserWithName(string name)
		{
			await Task.Delay(0);
			int num = SteamFriends.GetFriendCount(EFriendFlags.k_EFriendFlagImmediate);
			CSteamID csteamID = default(CSteamID);
			int num2 = 0;
			for (int i = 0; i < num; i++)
			{
				CSteamID friendByIndex = SteamFriends.GetFriendByIndex(i, EFriendFlags.k_EFriendFlagImmediate);
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

		// Token: 0x0600005C RID: 92 RVA: 0x00002A60 File Offset: 0x00000C60
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
				SteamUtils.GetImageSize(userAvatar, out num, out num2);
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

		// Token: 0x0600005D RID: 93 RVA: 0x00002AA4 File Offset: 0x00000CA4
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

		// Token: 0x0600005E RID: 94 RVA: 0x00002AD6 File Offset: 0x00000CD6
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

		// Token: 0x0600005F RID: 95 RVA: 0x00002B00 File Offset: 0x00000D00
		private void InitCallbacks()
		{
			this._personaStateChangeT = Callback<PersonaStateChange_t>.Create(new Callback<PersonaStateChange_t>.DispatchDelegate(SteamPlatformServices.UserInformationUpdated));
			this._avatarImageLoadedT = Callback<AvatarImageLoaded_t>.Create(new Callback<AvatarImageLoaded_t>.DispatchDelegate(SteamPlatformServices.AvatarLoaded));
		}

		// Token: 0x06000060 RID: 96 RVA: 0x00002B30 File Offset: 0x00000D30
		private static void AvatarLoaded(AvatarImageLoaded_t avatarImageLoadedT)
		{
			SteamPlatformServices._avatarLoadedUpdates.Add(avatarImageLoadedT.m_steamID);
		}

		// Token: 0x06000061 RID: 97 RVA: 0x00002B44 File Offset: 0x00000D44
		private static void UserInformationUpdated(PersonaStateChange_t pCallback)
		{
			if ((pCallback.m_nChangeFlags & EPersonaChange.k_EPersonaChangeAvatar) != (EPersonaChange)0)
			{
				SteamPlatformServices._avatarUpdates.Add(new CSteamID(pCallback.m_ulSteamID));
				SteamPlatformServices.Instance.OnAvatarUpdateReceived(pCallback.m_ulSteamID);
				return;
			}
			if ((pCallback.m_nChangeFlags & EPersonaChange.k_EPersonaChangeName) != (EPersonaChange)0)
			{
				SteamPlatformServices._nameUpdates.Add(new CSteamID(pCallback.m_ulSteamID));
				SteamPlatformServices.Instance.OnNameUpdateReceived(new CSteamID(pCallback.m_ulSteamID).ToPlayerId());
				return;
			}
			if ((pCallback.m_nChangeFlags & EPersonaChange.k_EPersonaChangeGamePlayed) != (EPersonaChange)0)
			{
				SteamPlatformServices.HandleOnUserStatusChanged(new CSteamID(pCallback.m_ulSteamID).ToPlayerId());
			}
		}

		// Token: 0x06000062 RID: 98 RVA: 0x00002BDC File Offset: 0x00000DDC
		private static void HandleOnUserStatusChanged(PlayerId playerId)
		{
			SteamPlatformServices.Instance._steamFriendListService.HandleOnUserStatusChanged(playerId);
		}

		// Token: 0x06000063 RID: 99 RVA: 0x00002BEE File Offset: 0x00000DEE
		Task<ILoginAccessProvider> IPlatformServices.CreateLobbyClientLoginProvider()
		{
			return Task.FromResult<ILoginAccessProvider>(new SteamLoginAccessProvider());
		}

		// Token: 0x06000064 RID: 100 RVA: 0x00002BFA File Offset: 0x00000DFA
		IFriendListService[] IPlatformServices.GetFriendListServices()
		{
			return this._friendListServices;
		}

		// Token: 0x04000010 RID: 16
		private PlatformInitParams _initParams;

		// Token: 0x04000011 RID: 17
		private SteamFriendListService _steamFriendListService;

		// Token: 0x04000012 RID: 18
		private IFriendListService[] _friendListServices;

		// Token: 0x04000013 RID: 19
		public SteamAchievementService _achievementService;

		// Token: 0x04000018 RID: 24
		private Dictionary<PlayerId, AvatarData> _avatarCache = new Dictionary<PlayerId, AvatarData>();

		// Token: 0x04000019 RID: 25
		private const int CommandRequestTimeOut = 5000;

		// Token: 0x0400001A RID: 26
		private Callback<PersonaStateChange_t> _personaStateChangeT;

		// Token: 0x0400001B RID: 27
		private Callback<AvatarImageLoaded_t> _avatarImageLoadedT;

		// Token: 0x0400001C RID: 28
		private static List<CSteamID> _avatarUpdates = new List<CSteamID>();

		// Token: 0x0400001D RID: 29
		private static List<CSteamID> _avatarLoadedUpdates = new List<CSteamID>();

		// Token: 0x0400001E RID: 30
		private static List<CSteamID> _nameUpdates = new List<CSteamID>();
	}
}
