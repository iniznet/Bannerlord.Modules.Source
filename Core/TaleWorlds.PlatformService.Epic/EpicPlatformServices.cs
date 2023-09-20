using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Epic.OnlineServices;
using Epic.OnlineServices.Achievements;
using Epic.OnlineServices.Auth;
using Epic.OnlineServices.Connect;
using Epic.OnlineServices.Friends;
using Epic.OnlineServices.Platform;
using Epic.OnlineServices.Presence;
using Epic.OnlineServices.Stats;
using Epic.OnlineServices.UserInfo;
using Newtonsoft.Json;
using TaleWorlds.AchievementSystem;
using TaleWorlds.ActivitySystem;
using TaleWorlds.Avatar.PlayerServices;
using TaleWorlds.Diamond;
using TaleWorlds.Diamond.AccessProvider.Epic;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.PlayerServices;
using TaleWorlds.PlayerServices.Avatar;

namespace TaleWorlds.PlatformService.Epic
{
	// Token: 0x02000005 RID: 5
	public class EpicPlatformServices : IPlatformServices
	{
		// Token: 0x17000005 RID: 5
		// (get) Token: 0x06000023 RID: 35 RVA: 0x00002397 File Offset: 0x00000597
		private static EpicPlatformServices Instance
		{
			get
			{
				return PlatformServices.Instance as EpicPlatformServices;
			}
		}

		// Token: 0x17000006 RID: 6
		// (get) Token: 0x06000024 RID: 36 RVA: 0x000023A4 File Offset: 0x000005A4
		public string UserId
		{
			get
			{
				if (this._epicAccountId == null)
				{
					return "";
				}
				string text;
				this._epicAccountId.ToString(out text);
				return text;
			}
		}

		// Token: 0x17000007 RID: 7
		// (get) Token: 0x06000025 RID: 37 RVA: 0x000023D4 File Offset: 0x000005D4
		string IPlatformServices.UserDisplayName
		{
			get
			{
				return "";
			}
		}

		// Token: 0x17000008 RID: 8
		// (get) Token: 0x06000026 RID: 38 RVA: 0x000023DB File Offset: 0x000005DB
		IReadOnlyCollection<PlayerId> IPlatformServices.BlockedUsers
		{
			get
			{
				return new List<PlayerId>();
			}
		}

		// Token: 0x17000009 RID: 9
		// (get) Token: 0x06000027 RID: 39 RVA: 0x000023E2 File Offset: 0x000005E2
		bool IPlatformServices.IsPermanentMuteAvailable
		{
			get
			{
				return true;
			}
		}

		// Token: 0x06000028 RID: 40 RVA: 0x000023E5 File Offset: 0x000005E5
		public EpicPlatformServices(PlatformInitParams initParams)
		{
			this._initParams = initParams;
			AvatarServices.AddAvatarService(PlayerIdProvidedTypes.Epic, new EpicPlatformAvatarService());
			this._epicFriendListService = new EpicFriendListService(this);
		}

		// Token: 0x14000004 RID: 4
		// (add) Token: 0x06000029 RID: 41 RVA: 0x00002424 File Offset: 0x00000624
		// (remove) Token: 0x0600002A RID: 42 RVA: 0x0000245C File Offset: 0x0000065C
		public event Action<AvatarData> OnAvatarUpdated;

		// Token: 0x14000005 RID: 5
		// (add) Token: 0x0600002B RID: 43 RVA: 0x00002494 File Offset: 0x00000694
		// (remove) Token: 0x0600002C RID: 44 RVA: 0x000024CC File Offset: 0x000006CC
		public event Action<string> OnNameUpdated;

		// Token: 0x14000006 RID: 6
		// (add) Token: 0x0600002D RID: 45 RVA: 0x00002504 File Offset: 0x00000704
		// (remove) Token: 0x0600002E RID: 46 RVA: 0x0000253C File Offset: 0x0000073C
		public event Action<bool, TextObject> OnSignInStateUpdated;

		// Token: 0x14000007 RID: 7
		// (add) Token: 0x0600002F RID: 47 RVA: 0x00002574 File Offset: 0x00000774
		// (remove) Token: 0x06000030 RID: 48 RVA: 0x000025AC File Offset: 0x000007AC
		public event Action OnBlockedUserListUpdated;

		// Token: 0x06000031 RID: 49 RVA: 0x000025E4 File Offset: 0x000007E4
		public bool Initialize(IFriendListService[] additionalFriendListServices)
		{
			this._friendListServices = new IFriendListService[additionalFriendListServices.Length + 1];
			this._friendListServices[0] = this._epicFriendListService;
			for (int i = 0; i < additionalFriendListServices.Length; i++)
			{
				this._friendListServices[i + 1] = additionalFriendListServices[i];
			}
			Result result = PlatformInterface.Initialize(new InitializeOptions
			{
				ProductName = "Bannerlord",
				ProductVersion = "1.1.4.17949"
			});
			if (result != Result.Success)
			{
				this._initFailReason = new TextObject("{=BJ1626h7}Epic platform initialization failed: {FAILREASON}.", null);
				this._initFailReason.SetTextVariable("FAILREASON", result.ToString());
				Debug.Print("Epic PlatformInterface.Initialize Failed:" + result, 0, Debug.DebugColor.White, 17592186044416UL);
				return false;
			}
			ClientCredentials clientCredentials = new ClientCredentials
			{
				ClientId = "e2cf3228b2914793b9a5e5570bad92b3",
				ClientSecret = "Fk5W1E6t1zExNqEUfjyNZinYrkDcDTA63sf5MfyDbQG4"
			};
			Options options = new Options
			{
				ProductId = "6372ed7350f34ffc9ace219dff4b9f40",
				SandboxId = "aeac94c7a11048758064b82f8f8d20ed",
				ClientCredentials = clientCredentials,
				IsServer = false,
				DeploymentId = "e77799aa8a5143f199b2cda9937a133f"
			};
			this._platform = PlatformInterface.Create(options);
			this._platform.GetFriendsInterface().AddNotifyFriendsUpdate(new AddNotifyFriendsUpdateOptions(), null, delegate(OnFriendsUpdateInfo callbackInfo)
			{
				this._epicFriendListService.UserStatusChanged(EpicPlatformServices.EpicAccountIdToPlayerId(callbackInfo.TargetUserId));
			});
			Epic.OnlineServices.Auth.Credentials credentials = new Epic.OnlineServices.Auth.Credentials
			{
				Type = LoginCredentialType.ExchangeCode,
				Token = this.ExchangeCode
			};
			bool failed = false;
			this._platform.GetAuthInterface().Login(new Epic.OnlineServices.Auth.LoginOptions
			{
				Credentials = credentials
			}, null, delegate(Epic.OnlineServices.Auth.LoginCallbackInfo callbackInfo)
			{
				if (callbackInfo.ResultCode != Result.Success)
				{
					failed = true;
					Debug.Print("Epic AuthInterface.Login Failed:" + callbackInfo.ResultCode, 0, Debug.DebugColor.White, 17592186044416UL);
					return;
				}
				EpicAccountId epicAccountId = callbackInfo.LocalUserId;
				this._platform.GetUserInfoInterface().QueryUserInfo(new QueryUserInfoOptions
				{
					LocalUserId = epicAccountId,
					TargetUserId = epicAccountId
				}, null, delegate(QueryUserInfoCallbackInfo queryCallbackInfo)
				{
					if (queryCallbackInfo.ResultCode != Result.Success)
					{
						failed = true;
						Debug.Print("Epic UserInfoInterface.QueryUserInfo Failed:" + queryCallbackInfo.ResultCode, 0, Debug.DebugColor.White, 17592186044416UL);
						return;
					}
					UserInfoData userInfoData;
					this._platform.GetUserInfoInterface().CopyUserInfo(new CopyUserInfoOptions
					{
						LocalUserId = epicAccountId,
						TargetUserId = epicAccountId
					}, out userInfoData);
					this._epicUserName = userInfoData.DisplayName;
					this._epicAccountId = epicAccountId;
				});
			});
			while (this._epicAccountId == null && !failed)
			{
				this._platform.Tick();
			}
			if (failed)
			{
				this._initFailReason = new TextObject("{=KoKdRd1u}Could not login to Epic", null);
				return false;
			}
			failed = !this.Connect();
			return failed;
		}

		// Token: 0x1700000A RID: 10
		// (get) Token: 0x06000032 RID: 50 RVA: 0x000027D1 File Offset: 0x000009D1
		private string ExchangeCode
		{
			get
			{
				return (string)this._initParams["ExchangeCode"];
			}
		}

		// Token: 0x06000033 RID: 51 RVA: 0x000027E8 File Offset: 0x000009E8
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

		// Token: 0x06000034 RID: 52 RVA: 0x00002845 File Offset: 0x00000A45
		private void RefreshConnection(AuthExpirationCallbackInfo clientData)
		{
			this.Connect();
		}

		// Token: 0x06000035 RID: 53 RVA: 0x00002850 File Offset: 0x00000A50
		private bool Connect()
		{
			bool failed = false;
			Token token;
			this._platform.GetAuthInterface().CopyUserAuthToken(new CopyUserAuthTokenOptions(), this._epicAccountId, out token);
			this._accessToken = token.AccessToken;
			this._platform.GetConnectInterface().RemoveNotifyAuthExpiration(this._refreshConnectionCallbackId);
			OnCreateUserCallback <>9__1;
			this._platform.GetConnectInterface().Login(new Epic.OnlineServices.Connect.LoginOptions
			{
				Credentials = new Epic.OnlineServices.Connect.Credentials
				{
					Token = this._accessToken,
					Type = ExternalCredentialType.Epic
				}
			}, null, delegate(Epic.OnlineServices.Connect.LoginCallbackInfo data)
			{
				if (data.ResultCode == Result.InvalidUser)
				{
					ConnectInterface connectInterface = this._platform.GetConnectInterface();
					CreateUserOptions createUserOptions = new CreateUserOptions();
					createUserOptions.ContinuanceToken = data.ContinuanceToken;
					object obj = null;
					OnCreateUserCallback onCreateUserCallback;
					if ((onCreateUserCallback = <>9__1) == null)
					{
						onCreateUserCallback = (<>9__1 = delegate(CreateUserCallbackInfo res)
						{
							if (res.ResultCode != Result.Success)
							{
								failed = true;
								return;
							}
							this._localUserId = res.LocalUserId;
						});
					}
					connectInterface.CreateUser(createUserOptions, obj, onCreateUserCallback);
					return;
				}
				if (data.ResultCode != Result.Success)
				{
					failed = true;
					return;
				}
				this._localUserId = data.LocalUserId;
			});
			while (this._localUserId == null && !failed)
			{
				this._platform.Tick();
			}
			if (failed)
			{
				this._initFailReason = new TextObject("{=KoKdRd1u}Could not login to Epic", null);
				return false;
			}
			this._refreshConnectionCallbackId = this._platform.GetConnectInterface().AddNotifyAuthExpiration(new AddNotifyAuthExpirationOptions(), token, new OnAuthExpirationCallback(this.RefreshConnection));
			this.QueryStats();
			this.QueryDefinitions();
			return true;
		}

		// Token: 0x06000036 RID: 54 RVA: 0x00002964 File Offset: 0x00000B64
		public void Terminate()
		{
			if (this._platform != null)
			{
				this._platform.Release();
				this._platform = null;
				PlatformInterface.Shutdown();
			}
		}

		// Token: 0x06000037 RID: 55 RVA: 0x0000298C File Offset: 0x00000B8C
		public void Tick(float dt)
		{
			if (this._platform != null)
			{
				this._platform.Tick();
				this.ProcessIngestStatsQueue();
			}
		}

		// Token: 0x1700000B RID: 11
		// (get) Token: 0x06000038 RID: 56 RVA: 0x000029AD File Offset: 0x00000BAD
		string IPlatformServices.ProviderName
		{
			get
			{
				return "Epic";
			}
		}

		// Token: 0x06000039 RID: 57 RVA: 0x000029B4 File Offset: 0x00000BB4
		bool IPlatformServices.IsPlayerProfileCardAvailable(PlayerId providedId)
		{
			return false;
		}

		// Token: 0x0600003A RID: 58 RVA: 0x000029B7 File Offset: 0x00000BB7
		void IPlatformServices.ShowPlayerProfileCard(PlayerId providedId)
		{
		}

		// Token: 0x1700000C RID: 12
		// (get) Token: 0x0600003B RID: 59 RVA: 0x000029B9 File Offset: 0x00000BB9
		bool IPlatformServices.UserLoggedIn
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		// Token: 0x0600003C RID: 60 RVA: 0x000029C0 File Offset: 0x00000BC0
		void IPlatformServices.LoginUser()
		{
			throw new NotImplementedException();
		}

		// Token: 0x0600003D RID: 61 RVA: 0x000029C7 File Offset: 0x00000BC7
		Task<AvatarData> IPlatformServices.GetUserAvatar(PlayerId providedId)
		{
			return Task.FromResult<AvatarData>(null);
		}

		// Token: 0x0600003E RID: 62 RVA: 0x000029CF File Offset: 0x00000BCF
		Task<bool> IPlatformServices.ShowOverlayForWebPage(string url)
		{
			return Task.FromResult<bool>(false);
		}

		// Token: 0x0600003F RID: 63 RVA: 0x000029D7 File Offset: 0x00000BD7
		Task<ILoginAccessProvider> IPlatformServices.CreateLobbyClientLoginProvider()
		{
			return Task.FromResult<ILoginAccessProvider>(new EpicLoginAccessProvider(this._platform, this._epicAccountId, this._epicUserName, this._accessToken, this._initFailReason));
		}

		// Token: 0x06000040 RID: 64 RVA: 0x00002A01 File Offset: 0x00000C01
		PlatformInitParams IPlatformServices.GetInitParams()
		{
			return this._initParams;
		}

		// Token: 0x06000041 RID: 65 RVA: 0x00002A09 File Offset: 0x00000C09
		IAchievementService IPlatformServices.GetAchievementService()
		{
			return new EpicAchievementService(this);
		}

		// Token: 0x06000042 RID: 66 RVA: 0x00002A11 File Offset: 0x00000C11
		IActivityService IPlatformServices.GetActivityService()
		{
			return new TestActivityService();
		}

		// Token: 0x06000043 RID: 67 RVA: 0x00002A18 File Offset: 0x00000C18
		void IPlatformServices.CheckPrivilege(Privilege privilege, bool displayResolveUI, PrivilegeResult callback)
		{
			callback(true);
		}

		// Token: 0x06000044 RID: 68 RVA: 0x00002A21 File Offset: 0x00000C21
		void IPlatformServices.CheckPermissionWithUser(Permission privilege, PlayerId targetPlayerId, PermissionResult callback)
		{
			callback(true);
		}

		// Token: 0x06000045 RID: 69 RVA: 0x00002A2A File Offset: 0x00000C2A
		bool IPlatformServices.RegisterPermissionChangeEvent(PlayerId targetPlayerId, Permission permission, PermissionChanged callback)
		{
			return false;
		}

		// Token: 0x06000046 RID: 70 RVA: 0x00002A2D File Offset: 0x00000C2D
		bool IPlatformServices.UnregisterPermissionChangeEvent(PlayerId targetPlayerId, Permission permission, PermissionChanged callback)
		{
			return false;
		}

		// Token: 0x06000047 RID: 71 RVA: 0x00002A30 File Offset: 0x00000C30
		void IPlatformServices.ShowRestrictedInformation()
		{
		}

		// Token: 0x06000048 RID: 72 RVA: 0x00002A32 File Offset: 0x00000C32
		Task<bool> IPlatformServices.VerifyString(string content)
		{
			return Task.FromResult<bool>(true);
		}

		// Token: 0x06000049 RID: 73 RVA: 0x00002A3A File Offset: 0x00000C3A
		void IPlatformServices.OnFocusGained()
		{
		}

		// Token: 0x0600004A RID: 74 RVA: 0x00002A3C File Offset: 0x00000C3C
		void IPlatformServices.GetPlatformId(PlayerId playerId, Action<object> callback)
		{
			callback(EpicPlatformServices.PlayerIdToEpicAccountId(playerId));
		}

		// Token: 0x0600004B RID: 75 RVA: 0x00002A4C File Offset: 0x00000C4C
		internal async Task<string> GetUserName(PlayerId providedId)
		{
			string text;
			if (!providedId.IsValid || providedId.ProvidedType != PlayerIdProvidedTypes.Epic)
			{
				text = null;
			}
			else
			{
				EpicAccountId epicAccountId = EpicPlatformServices.PlayerIdToEpicAccountId(providedId);
				UserInfoData userInfoData = await this.GetUserInfo(epicAccountId);
				if (userInfoData == null)
				{
					text = "";
				}
				else
				{
					text = userInfoData.DisplayName;
				}
			}
			return text;
		}

		// Token: 0x0600004C RID: 76 RVA: 0x00002A9C File Offset: 0x00000C9C
		internal async Task<bool> GetUserOnlineStatus(PlayerId providedId)
		{
			EpicAccountId targetUserId = EpicPlatformServices.PlayerIdToEpicAccountId(providedId);
			await this.GetUserInfo(targetUserId);
			Info info = await this.GetUserPresence(targetUserId);
			bool flag;
			if (info == null)
			{
				flag = false;
			}
			else
			{
				flag = info.Status == Status.Online;
			}
			return flag;
		}

		// Token: 0x0600004D RID: 77 RVA: 0x00002AEC File Offset: 0x00000CEC
		internal async Task<bool> IsPlayingThisGame(PlayerId providedId)
		{
			Info info = await this.GetUserPresence(EpicPlatformServices.PlayerIdToEpicAccountId(providedId));
			bool flag;
			if (info == null)
			{
				flag = false;
			}
			else
			{
				flag = info.ProductId == "6372ed7350f34ffc9ace219dff4b9f40";
			}
			return flag;
		}

		// Token: 0x0600004E RID: 78 RVA: 0x00002B3C File Offset: 0x00000D3C
		internal async Task<PlayerId> GetUserWithName(string name)
		{
			PlayerId? id = null;
			this._platform.GetUserInfoInterface().QueryUserInfoByDisplayName(new QueryUserInfoByDisplayNameOptions
			{
				LocalUserId = this._epicAccountId,
				DisplayName = name
			}, null, delegate(QueryUserInfoByDisplayNameCallbackInfo callbackInfo)
			{
				if (callbackInfo.ResultCode == Result.Success)
				{
					id = new PlayerId?(EpicPlatformServices.EpicAccountIdToPlayerId(callbackInfo.TargetUserId));
					return;
				}
				throw new Exception("Could not retrieve player from EOS");
			});
			while (id == null)
			{
				this._platform.Tick();
				await Task.Delay(5);
			}
			return id.Value;
		}

		// Token: 0x0600004F RID: 79 RVA: 0x00002B8C File Offset: 0x00000D8C
		internal IEnumerable<PlayerId> GetAllFriends()
		{
			List<PlayerId> friends = new List<PlayerId>();
			bool? success = null;
			this._platform.GetFriendsInterface().QueryFriends(new QueryFriendsOptions
			{
				LocalUserId = this._epicAccountId
			}, null, delegate(QueryFriendsCallbackInfo callbackInfo)
			{
				if (callbackInfo.ResultCode == Result.Success)
				{
					int friendsCount = this._platform.GetFriendsInterface().GetFriendsCount(new GetFriendsCountOptions
					{
						LocalUserId = this._epicAccountId
					});
					for (int i = 0; i < friendsCount; i++)
					{
						EpicAccountId friendAtIndex = this._platform.GetFriendsInterface().GetFriendAtIndex(new GetFriendAtIndexOptions
						{
							LocalUserId = this._epicAccountId,
							Index = i
						});
						friends.Add(EpicPlatformServices.EpicAccountIdToPlayerId(friendAtIndex));
					}
					success = new bool?(true);
					return;
				}
				success = new bool?(false);
			});
			while (success == null)
			{
				this._platform.Tick();
				Task.Delay(5);
			}
			return friends;
		}

		// Token: 0x06000050 RID: 80 RVA: 0x00002C14 File Offset: 0x00000E14
		public void QueryDefinitions()
		{
			AchievementsInterface achievementsInterface = this._platform.GetAchievementsInterface();
			QueryDefinitionsOptions queryDefinitionsOptions = new QueryDefinitionsOptions();
			queryDefinitionsOptions.LocalUserId = this._localUserId;
			achievementsInterface.QueryDefinitions(queryDefinitionsOptions, null, delegate(OnQueryDefinitionsCompleteCallbackInfo data)
			{
			});
		}

		// Token: 0x06000051 RID: 81 RVA: 0x00002C62 File Offset: 0x00000E62
		internal bool SetStat(string name, int value)
		{
			this._ingestStatsQueue.Add(new EpicPlatformServices.IngestStatsQueueItem
			{
				Name = name,
				Value = value
			});
			return true;
		}

		// Token: 0x06000052 RID: 82 RVA: 0x00002C84 File Offset: 0x00000E84
		internal Task<int> GetStat(string name)
		{
			Stat stat;
			if (this._platform.GetStatsInterface().CopyStatByName(new CopyStatByNameOptions
			{
				Name = name,
				TargetUserId = this._localUserId
			}, out stat) == Result.Success)
			{
				return Task.FromResult<int>(stat.Value);
			}
			return Task.FromResult<int>(-1);
		}

		// Token: 0x06000053 RID: 83 RVA: 0x00002CD0 File Offset: 0x00000ED0
		internal Task<int[]> GetStats(string[] names)
		{
			List<int> list = new List<int>();
			foreach (string text in names)
			{
				list.Add(this.GetStat(text).Result);
			}
			return Task.FromResult<int[]>(list.ToArray());
		}

		// Token: 0x06000054 RID: 84 RVA: 0x00002D14 File Offset: 0x00000F14
		private void ProcessIngestStatsQueue()
		{
			if (!this._writingStats && DateTime.Now.Subtract(this._statsLastWrittenOn).TotalSeconds > 5.0 && this._ingestStatsQueue.Count > 0)
			{
				this._statsLastWrittenOn = DateTime.Now;
				this._writingStats = true;
				StatsInterface statsInterface = this._platform.GetStatsInterface();
				List<IngestData> stats = new List<IngestData>();
				while (this._ingestStatsQueue.Count > 0)
				{
					EpicPlatformServices.IngestStatsQueueItem ingestStatsQueueItem;
					if (this._ingestStatsQueue.TryTake(out ingestStatsQueueItem))
					{
						stats.Add(new IngestData
						{
							StatName = ingestStatsQueueItem.Name,
							IngestAmount = ingestStatsQueueItem.Value
						});
					}
				}
				statsInterface.IngestStat(new IngestStatOptions
				{
					Stats = stats.ToArray(),
					LocalUserId = this._localUserId,
					TargetUserId = this._localUserId
				}, null, delegate(IngestStatCompleteCallbackInfo data)
				{
					if (data.ResultCode != Result.Success)
					{
						foreach (IngestData ingestData in stats)
						{
							this._ingestStatsQueue.Add(new EpicPlatformServices.IngestStatsQueueItem
							{
								Name = ingestData.StatName,
								Value = ingestData.IngestAmount
							});
						}
					}
					this.QueryStats();
					this._writingStats = false;
				});
			}
		}

		// Token: 0x06000055 RID: 85 RVA: 0x00002E28 File Offset: 0x00001028
		private static PlayerId EpicAccountIdToPlayerId(EpicAccountId epicAccountId)
		{
			string text;
			epicAccountId.ToString(out text);
			return new PlayerId(3, text);
		}

		// Token: 0x06000056 RID: 86 RVA: 0x00002E48 File Offset: 0x00001048
		private static EpicAccountId PlayerIdToEpicAccountId(PlayerId playerId)
		{
			byte[] array = new ArraySegment<byte>(playerId.ToByteArray(), 16, 16).ToArray<byte>();
			Guid guid = new Guid(array);
			return EpicAccountId.FromString(guid.ToString("N"));
		}

		// Token: 0x06000057 RID: 87 RVA: 0x00002E8C File Offset: 0x0000108C
		private async Task<UserInfoData> GetUserInfo(EpicAccountId targetUserId)
		{
			bool done = false;
			UserInfoData userInfoData = null;
			this._platform.GetUserInfoInterface().QueryUserInfo(new QueryUserInfoOptions
			{
				LocalUserId = this._epicAccountId,
				TargetUserId = targetUserId
			}, null, delegate(QueryUserInfoCallbackInfo callbackInfo)
			{
				if (callbackInfo.ResultCode == Result.Success)
				{
					this._platform.GetUserInfoInterface().CopyUserInfo(new CopyUserInfoOptions
					{
						LocalUserId = this._epicAccountId,
						TargetUserId = targetUserId
					}, out userInfoData);
				}
				done = true;
			});
			while (!done)
			{
				this._platform.Tick();
				await Task.Delay(5);
			}
			return userInfoData;
		}

		// Token: 0x06000058 RID: 88 RVA: 0x00002EDC File Offset: 0x000010DC
		private async Task<Info> GetUserPresence(EpicAccountId targetUserId)
		{
			Info info = null;
			bool done = false;
			this._platform.GetPresenceInterface().QueryPresence(new QueryPresenceOptions
			{
				LocalUserId = this._epicAccountId,
				TargetUserId = targetUserId
			}, null, delegate(QueryPresenceCallbackInfo callbackInfo)
			{
				if (callbackInfo.ResultCode == Result.Success && this._platform.GetPresenceInterface().HasPresence(new HasPresenceOptions
				{
					LocalUserId = this._epicAccountId,
					TargetUserId = targetUserId
				}))
				{
					this._platform.GetPresenceInterface().CopyPresence(new CopyPresenceOptions
					{
						LocalUserId = this._epicAccountId,
						TargetUserId = targetUserId
					}, out info);
				}
				done = true;
			});
			while (!done)
			{
				this._platform.Tick();
				await Task.Delay(5);
			}
			return info;
		}

		// Token: 0x06000059 RID: 89 RVA: 0x00002F2C File Offset: 0x0000112C
		private void QueryStats()
		{
			StatsInterface statsInterface = this._platform.GetStatsInterface();
			QueryStatsOptions queryStatsOptions = new QueryStatsOptions();
			queryStatsOptions.LocalUserId = this._localUserId;
			queryStatsOptions.TargetUserId = this._localUserId;
			statsInterface.QueryStats(queryStatsOptions, null, delegate(OnQueryStatsCompleteCallbackInfo data)
			{
			});
		}

		// Token: 0x0600005A RID: 90 RVA: 0x00002F86 File Offset: 0x00001186
		IFriendListService[] IPlatformServices.GetFriendListServices()
		{
			return this._friendListServices;
		}

		// Token: 0x04000009 RID: 9
		private EpicAccountId _epicAccountId;

		// Token: 0x0400000A RID: 10
		private ProductUserId _localUserId;

		// Token: 0x0400000B RID: 11
		private string _accessToken;

		// Token: 0x0400000C RID: 12
		private string _epicUserName;

		// Token: 0x0400000D RID: 13
		private PlatformInterface _platform;

		// Token: 0x0400000E RID: 14
		private PlatformInitParams _initParams;

		// Token: 0x0400000F RID: 15
		private EpicFriendListService _epicFriendListService;

		// Token: 0x04000010 RID: 16
		private IFriendListService[] _friendListServices;

		// Token: 0x04000015 RID: 21
		private TextObject _initFailReason;

		// Token: 0x04000016 RID: 22
		private ulong _refreshConnectionCallbackId;

		// Token: 0x04000017 RID: 23
		private ConcurrentBag<EpicPlatformServices.IngestStatsQueueItem> _ingestStatsQueue = new ConcurrentBag<EpicPlatformServices.IngestStatsQueueItem>();

		// Token: 0x04000018 RID: 24
		private bool _writingStats;

		// Token: 0x04000019 RID: 25
		private DateTime _statsLastWrittenOn = DateTime.MinValue;

		// Token: 0x0400001A RID: 26
		private const int MinStatsWriteInterval = 5;

		// Token: 0x02000007 RID: 7
		private class IngestStatsQueueItem
		{
			// Token: 0x17000011 RID: 17
			// (get) Token: 0x06000071 RID: 113 RVA: 0x00003192 File Offset: 0x00001392
			// (set) Token: 0x06000072 RID: 114 RVA: 0x0000319A File Offset: 0x0000139A
			public string Name { get; set; }

			// Token: 0x17000012 RID: 18
			// (get) Token: 0x06000073 RID: 115 RVA: 0x000031A3 File Offset: 0x000013A3
			// (set) Token: 0x06000074 RID: 116 RVA: 0x000031AB File Offset: 0x000013AB
			public int Value { get; set; }
		}

		// Token: 0x02000008 RID: 8
		private class EpicAuthErrorResponse
		{
			// Token: 0x17000013 RID: 19
			// (get) Token: 0x06000076 RID: 118 RVA: 0x000031BC File Offset: 0x000013BC
			// (set) Token: 0x06000077 RID: 119 RVA: 0x000031C4 File Offset: 0x000013C4
			[JsonProperty("errorCode")]
			public string ErrorCode { get; set; }

			// Token: 0x17000014 RID: 20
			// (get) Token: 0x06000078 RID: 120 RVA: 0x000031CD File Offset: 0x000013CD
			// (set) Token: 0x06000079 RID: 121 RVA: 0x000031D5 File Offset: 0x000013D5
			[JsonProperty("errorMessage")]
			public string ErrorMessage { get; set; }

			// Token: 0x17000015 RID: 21
			// (get) Token: 0x0600007A RID: 122 RVA: 0x000031DE File Offset: 0x000013DE
			// (set) Token: 0x0600007B RID: 123 RVA: 0x000031E6 File Offset: 0x000013E6
			[JsonProperty("numericErrorCode")]
			public int NumericErrorCode { get; set; }

			// Token: 0x17000016 RID: 22
			// (get) Token: 0x0600007C RID: 124 RVA: 0x000031EF File Offset: 0x000013EF
			// (set) Token: 0x0600007D RID: 125 RVA: 0x000031F7 File Offset: 0x000013F7
			[JsonProperty("error_description")]
			public string ErrorDescription { get; set; }

			// Token: 0x17000017 RID: 23
			// (get) Token: 0x0600007E RID: 126 RVA: 0x00003200 File Offset: 0x00001400
			// (set) Token: 0x0600007F RID: 127 RVA: 0x00003208 File Offset: 0x00001408
			[JsonProperty("error")]
			public string Error { get; set; }
		}

		// Token: 0x02000009 RID: 9
		private class EpicAuthResponse
		{
			// Token: 0x17000018 RID: 24
			// (get) Token: 0x06000081 RID: 129 RVA: 0x00003219 File Offset: 0x00001419
			// (set) Token: 0x06000082 RID: 130 RVA: 0x00003221 File Offset: 0x00001421
			[JsonProperty("access_token")]
			public string AccessToken { get; set; }

			// Token: 0x17000019 RID: 25
			// (get) Token: 0x06000083 RID: 131 RVA: 0x0000322A File Offset: 0x0000142A
			// (set) Token: 0x06000084 RID: 132 RVA: 0x00003232 File Offset: 0x00001432
			[JsonProperty("expires_in")]
			public int ExpiresIn { get; set; }

			// Token: 0x1700001A RID: 26
			// (get) Token: 0x06000085 RID: 133 RVA: 0x0000323B File Offset: 0x0000143B
			// (set) Token: 0x06000086 RID: 134 RVA: 0x00003243 File Offset: 0x00001443
			[JsonProperty("expires_at")]
			public DateTime ExpiresAt { get; set; }

			// Token: 0x1700001B RID: 27
			// (get) Token: 0x06000087 RID: 135 RVA: 0x0000324C File Offset: 0x0000144C
			// (set) Token: 0x06000088 RID: 136 RVA: 0x00003254 File Offset: 0x00001454
			[JsonProperty("token_type")]
			public string TokenType { get; set; }

			// Token: 0x1700001C RID: 28
			// (get) Token: 0x06000089 RID: 137 RVA: 0x0000325D File Offset: 0x0000145D
			// (set) Token: 0x0600008A RID: 138 RVA: 0x00003265 File Offset: 0x00001465
			[JsonProperty("refresh_token")]
			public string RefreshToken { get; set; }

			// Token: 0x1700001D RID: 29
			// (get) Token: 0x0600008B RID: 139 RVA: 0x0000326E File Offset: 0x0000146E
			// (set) Token: 0x0600008C RID: 140 RVA: 0x00003276 File Offset: 0x00001476
			[JsonProperty("refresh_expires")]
			public int RefreshExpires { get; set; }

			// Token: 0x1700001E RID: 30
			// (get) Token: 0x0600008D RID: 141 RVA: 0x0000327F File Offset: 0x0000147F
			// (set) Token: 0x0600008E RID: 142 RVA: 0x00003287 File Offset: 0x00001487
			[JsonProperty("refresh_expires_at")]
			public DateTime RefreshExpiresAt { get; set; }

			// Token: 0x1700001F RID: 31
			// (get) Token: 0x0600008F RID: 143 RVA: 0x00003290 File Offset: 0x00001490
			// (set) Token: 0x06000090 RID: 144 RVA: 0x00003298 File Offset: 0x00001498
			[JsonProperty("account_id")]
			public string AccountId { get; set; }

			// Token: 0x17000020 RID: 32
			// (get) Token: 0x06000091 RID: 145 RVA: 0x000032A1 File Offset: 0x000014A1
			// (set) Token: 0x06000092 RID: 146 RVA: 0x000032A9 File Offset: 0x000014A9
			[JsonProperty("client_id")]
			public string ClientId { get; set; }

			// Token: 0x17000021 RID: 33
			// (get) Token: 0x06000093 RID: 147 RVA: 0x000032B2 File Offset: 0x000014B2
			// (set) Token: 0x06000094 RID: 148 RVA: 0x000032BA File Offset: 0x000014BA
			[JsonProperty("internal_client")]
			public bool InternalClient { get; set; }

			// Token: 0x17000022 RID: 34
			// (get) Token: 0x06000095 RID: 149 RVA: 0x000032C3 File Offset: 0x000014C3
			// (set) Token: 0x06000096 RID: 150 RVA: 0x000032CB File Offset: 0x000014CB
			[JsonProperty("client_service")]
			public string ClientService { get; set; }

			// Token: 0x17000023 RID: 35
			// (get) Token: 0x06000097 RID: 151 RVA: 0x000032D4 File Offset: 0x000014D4
			// (set) Token: 0x06000098 RID: 152 RVA: 0x000032DC File Offset: 0x000014DC
			[JsonProperty("displayName")]
			public string DisplayName { get; set; }

			// Token: 0x17000024 RID: 36
			// (get) Token: 0x06000099 RID: 153 RVA: 0x000032E5 File Offset: 0x000014E5
			// (set) Token: 0x0600009A RID: 154 RVA: 0x000032ED File Offset: 0x000014ED
			[JsonProperty("app")]
			public string App { get; set; }

			// Token: 0x17000025 RID: 37
			// (get) Token: 0x0600009B RID: 155 RVA: 0x000032F6 File Offset: 0x000014F6
			// (set) Token: 0x0600009C RID: 156 RVA: 0x000032FE File Offset: 0x000014FE
			[JsonProperty("in_app_id")]
			public string InAppId { get; set; }

			// Token: 0x17000026 RID: 38
			// (get) Token: 0x0600009D RID: 157 RVA: 0x00003307 File Offset: 0x00001507
			// (set) Token: 0x0600009E RID: 158 RVA: 0x0000330F File Offset: 0x0000150F
			[JsonProperty("device_id")]
			public string DeviceId { get; set; }

			// Token: 0x17000027 RID: 39
			// (get) Token: 0x0600009F RID: 159 RVA: 0x00003318 File Offset: 0x00001518
			// (set) Token: 0x060000A0 RID: 160 RVA: 0x00003320 File Offset: 0x00001520
			[JsonProperty("product_id")]
			public string ProductId { get; set; }
		}
	}
}
