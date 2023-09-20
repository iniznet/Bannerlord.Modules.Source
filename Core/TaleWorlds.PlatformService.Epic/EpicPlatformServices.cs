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
	public class EpicPlatformServices : IPlatformServices
	{
		private static EpicPlatformServices Instance
		{
			get
			{
				return PlatformServices.Instance as EpicPlatformServices;
			}
		}

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

		public EpicPlatformServices(PlatformInitParams initParams)
		{
			this._initParams = initParams;
			AvatarServices.AddAvatarService(PlayerIdProvidedTypes.Epic, new EpicPlatformAvatarService());
			this._epicFriendListService = new EpicFriendListService(this);
		}

		public event Action<AvatarData> OnAvatarUpdated;

		public event Action<string> OnNameUpdated;

		public event Action<bool, TextObject> OnSignInStateUpdated;

		public event Action OnBlockedUserListUpdated;

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
				ProductVersion = "1.0"
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

		private string ExchangeCode
		{
			get
			{
				return (string)this._initParams["ExchangeCode"];
			}
		}

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

		private void RefreshConnection(AuthExpirationCallbackInfo clientData)
		{
			this.Connect();
		}

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

		public void Terminate()
		{
			if (this._platform != null)
			{
				this._platform.Release();
				this._platform = null;
				PlatformInterface.Shutdown();
			}
		}

		public void Tick(float dt)
		{
			if (this._platform != null)
			{
				this._platform.Tick();
				this.ProcessIngestStatsQueue();
			}
		}

		string IPlatformServices.ProviderName
		{
			get
			{
				return "Epic";
			}
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
				throw new NotImplementedException();
			}
		}

		void IPlatformServices.LoginUser()
		{
			throw new NotImplementedException();
		}

		Task<AvatarData> IPlatformServices.GetUserAvatar(PlayerId providedId)
		{
			return Task.FromResult<AvatarData>(null);
		}

		Task<bool> IPlatformServices.ShowOverlayForWebPage(string url)
		{
			return Task.FromResult<bool>(false);
		}

		Task<ILoginAccessProvider> IPlatformServices.CreateLobbyClientLoginProvider()
		{
			return Task.FromResult<ILoginAccessProvider>(new EpicLoginAccessProvider(this._platform, this._epicAccountId, this._epicUserName, this._accessToken, this._initFailReason));
		}

		PlatformInitParams IPlatformServices.GetInitParams()
		{
			return this._initParams;
		}

		IAchievementService IPlatformServices.GetAchievementService()
		{
			return new EpicAchievementService(this);
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

		void IPlatformServices.OnFocusGained()
		{
		}

		void IPlatformServices.GetPlatformId(PlayerId playerId, Action<object> callback)
		{
			callback(EpicPlatformServices.PlayerIdToEpicAccountId(playerId));
		}

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

		public void QueryDefinitions()
		{
			AchievementsInterface achievementsInterface = this._platform.GetAchievementsInterface();
			QueryDefinitionsOptions queryDefinitionsOptions = new QueryDefinitionsOptions();
			queryDefinitionsOptions.LocalUserId = this._localUserId;
			achievementsInterface.QueryDefinitions(queryDefinitionsOptions, null, delegate(OnQueryDefinitionsCompleteCallbackInfo data)
			{
			});
		}

		internal bool SetStat(string name, int value)
		{
			this._ingestStatsQueue.Add(new EpicPlatformServices.IngestStatsQueueItem
			{
				Name = name,
				Value = value
			});
			return true;
		}

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

		internal Task<int[]> GetStats(string[] names)
		{
			List<int> list = new List<int>();
			foreach (string text in names)
			{
				list.Add(this.GetStat(text).Result);
			}
			return Task.FromResult<int[]>(list.ToArray());
		}

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

		private static PlayerId EpicAccountIdToPlayerId(EpicAccountId epicAccountId)
		{
			string text;
			epicAccountId.ToString(out text);
			return new PlayerId(3, text);
		}

		private static EpicAccountId PlayerIdToEpicAccountId(PlayerId playerId)
		{
			byte[] array = new ArraySegment<byte>(playerId.ToByteArray(), 16, 16).ToArray<byte>();
			Guid guid = new Guid(array);
			return EpicAccountId.FromString(guid.ToString("N"));
		}

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

		IFriendListService[] IPlatformServices.GetFriendListServices()
		{
			return this._friendListServices;
		}

		private EpicAccountId _epicAccountId;

		private ProductUserId _localUserId;

		private string _accessToken;

		private string _epicUserName;

		private PlatformInterface _platform;

		private PlatformInitParams _initParams;

		private EpicFriendListService _epicFriendListService;

		private IFriendListService[] _friendListServices;

		private TextObject _initFailReason;

		private ulong _refreshConnectionCallbackId;

		private ConcurrentBag<EpicPlatformServices.IngestStatsQueueItem> _ingestStatsQueue = new ConcurrentBag<EpicPlatformServices.IngestStatsQueueItem>();

		private bool _writingStats;

		private DateTime _statsLastWrittenOn = DateTime.MinValue;

		private const int MinStatsWriteInterval = 5;

		private class IngestStatsQueueItem
		{
			public string Name { get; set; }

			public int Value { get; set; }
		}

		private class EpicAuthErrorResponse
		{
			[JsonProperty("errorCode")]
			public string ErrorCode { get; set; }

			[JsonProperty("errorMessage")]
			public string ErrorMessage { get; set; }

			[JsonProperty("numericErrorCode")]
			public int NumericErrorCode { get; set; }

			[JsonProperty("error_description")]
			public string ErrorDescription { get; set; }

			[JsonProperty("error")]
			public string Error { get; set; }
		}

		private class EpicAuthResponse
		{
			[JsonProperty("access_token")]
			public string AccessToken { get; set; }

			[JsonProperty("expires_in")]
			public int ExpiresIn { get; set; }

			[JsonProperty("expires_at")]
			public DateTime ExpiresAt { get; set; }

			[JsonProperty("token_type")]
			public string TokenType { get; set; }

			[JsonProperty("refresh_token")]
			public string RefreshToken { get; set; }

			[JsonProperty("refresh_expires")]
			public int RefreshExpires { get; set; }

			[JsonProperty("refresh_expires_at")]
			public DateTime RefreshExpiresAt { get; set; }

			[JsonProperty("account_id")]
			public string AccountId { get; set; }

			[JsonProperty("client_id")]
			public string ClientId { get; set; }

			[JsonProperty("internal_client")]
			public bool InternalClient { get; set; }

			[JsonProperty("client_service")]
			public string ClientService { get; set; }

			[JsonProperty("displayName")]
			public string DisplayName { get; set; }

			[JsonProperty("app")]
			public string App { get; set; }

			[JsonProperty("in_app_id")]
			public string InAppId { get; set; }

			[JsonProperty("device_id")]
			public string DeviceId { get; set; }

			[JsonProperty("product_id")]
			public string ProductId { get; set; }
		}
	}
}
