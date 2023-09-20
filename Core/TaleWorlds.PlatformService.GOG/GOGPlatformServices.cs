using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using Galaxy.Api;
using TaleWorlds.AchievementSystem;
using TaleWorlds.ActivitySystem;
using TaleWorlds.Avatar.PlayerServices;
using TaleWorlds.Diamond;
using TaleWorlds.Diamond.AccessProvider.GOG;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.PlayerServices;
using TaleWorlds.PlayerServices.Avatar;

namespace TaleWorlds.PlatformService.GOG
{
	public class GOGPlatformServices : IPlatformServices
	{
		private static GOGPlatformServices Instance
		{
			get
			{
				return PlatformServices.Instance as GOGPlatformServices;
			}
		}

		public GOGPlatformServices(PlatformInitParams initParams)
		{
			this.LoadAchievementDataFromXml((string)initParams["AchievementDataXmlPath"]);
			this._initParams = initParams;
			AvatarServices.AddAvatarService(PlayerIdProvidedTypes.GOG, new GOGPlatformAvatarService(this));
			this._gogFriendListService = new GOGFriendListService(this);
		}

		string IPlatformServices.ProviderName
		{
			get
			{
				return "GOG";
			}
		}

		string IPlatformServices.UserId
		{
			get
			{
				return GalaxyInstance.User().GetGalaxyID().ToUint64()
					.ToString();
			}
		}

		PlayerId IPlatformServices.PlayerId
		{
			get
			{
				return GalaxyInstance.User().GetGalaxyID().ToPlayerId();
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
				if (!this._initialized)
				{
					return string.Empty;
				}
				return GalaxyInstance.Friends().GetPersonaName();
			}
		}

		IReadOnlyCollection<PlayerId> IPlatformServices.BlockedUsers
		{
			get
			{
				return new List<PlayerId>();
			}
		}

		IAchievementService IPlatformServices.GetAchievementService()
		{
			return new GOGAchievementService(this);
		}

		IActivityService IPlatformServices.GetActivityService()
		{
			return new TestActivityService();
		}

		Task<bool> IPlatformServices.VerifyString(string content)
		{
			return Task.FromResult<bool>(true);
		}

		void IPlatformServices.GetPlatformId(PlayerId playerId, Action<object> callback)
		{
			callback(new GalaxyID(playerId.Part4));
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
			if (!this._initialized)
			{
				this._friendListServices = new IFriendListService[additionalFriendListServices.Length + 1];
				this._friendListServices[0] = this._gogFriendListService;
				for (int i = 0; i < additionalFriendListServices.Length; i++)
				{
					this._friendListServices[i + 1] = additionalFriendListServices[i];
				}
				this._initialized = false;
				InitParams initParams = new InitParams("53550366963454221", "c17786edab4b6b3915ab55cfc5bb5a9a0a80b9a2d55d22c0767c9c18477efdb9");
				Debug.Print("Initializing GalaxyPeer instance...", 0, Debug.DebugColor.White, 17592186044416UL);
				try
				{
					GalaxyInstance.Init(initParams);
					try
					{
						IUser user = GalaxyInstance.User();
						AuthenticationListener authenticationListener = new AuthenticationListener(this);
						user.SignInGalaxy(true, authenticationListener);
						while (!authenticationListener.GotResult)
						{
							GalaxyInstance.ProcessData();
							Thread.Sleep(5);
						}
						this._gogFriendListService.RequestFriendList();
					}
					catch (GalaxyInstance.Error error)
					{
						Debug.Print("SignInGalaxy failed for reason " + error, 0, Debug.DebugColor.White, 17592186044416UL);
					}
					this.InitListeners();
					this.RequestUserStatsAndAchievements();
					this._initialized = true;
				}
				catch (GalaxyInstance.Error error2)
				{
					Debug.Print("Galaxy Init failed for reason " + error2, 0, Debug.DebugColor.White, 17592186044416UL);
					this._initialized = false;
				}
			}
			return this._initialized;
		}

		void IPlatformServices.Tick(float dt)
		{
			GalaxyInstance.ProcessData();
			if (this._initialized)
			{
				this.CheckStoreStats();
			}
		}

		void IPlatformServices.Terminate()
		{
			GalaxyInstance.Shutdown(true);
		}

		private void InvalidateStats()
		{
			this._statsLastInvalidated = new DateTime?(DateTime.Now);
		}

		private void CheckStoreStats()
		{
			if (this._statsLastInvalidated != null && DateTime.Now.Subtract(this._statsLastInvalidated.Value).TotalSeconds > 5.0 && DateTime.Now.Subtract(this._statsLastStored).TotalSeconds > 30.0)
			{
				this._statsLastStored = DateTime.Now;
				GalaxyInstance.Stats().StoreStatsAndAchievements();
				this._statsLastInvalidated = null;
			}
		}

		public event Action<AvatarData> OnAvatarUpdated;

		public event Action<string> OnNameUpdated;

		public event Action<bool, TextObject> OnSignInStateUpdated;

		public event Action OnBlockedUserListUpdated;

		public event Action<string> OnTextEnteredFromPlatform;

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
			if (this.OnTextEnteredFromPlatform != null)
			{
				this.OnTextEnteredFromPlatform(null);
			}
		}

		bool IPlatformServices.IsPlayerProfileCardAvailable(PlayerId providedId)
		{
			return false;
		}

		void IPlatformServices.ShowPlayerProfileCard(PlayerId providedId)
		{
		}

		internal void ClearAvatarCache()
		{
			this._avatarCache.Clear();
		}

		async Task<AvatarData> IPlatformServices.GetUserAvatar(PlayerId providedId)
		{
			GalaxyID galaxyID = new GalaxyID(providedId.Part4);
			AvatarData avatarData;
			if (this._avatarCache.ContainsKey(providedId))
			{
				avatarData = this._avatarCache[providedId];
			}
			else
			{
				UserInformationRetrieveListener listener = new UserInformationRetrieveListener();
				GalaxyInstance.Friends().RequestUserInformation(galaxyID, 4U, listener);
				while (!listener.GotResult)
				{
					await Task.Delay(5);
				}
				uint num = 184U;
				uint num2 = 184U;
				uint num3 = 4U * num * num2;
				byte[] array = new byte[num3];
				GalaxyInstance.Friends().GetFriendAvatarImageRGBA(galaxyID, AvatarType.AVATAR_TYPE_LARGE, array, num3);
				AvatarData avatarData2 = new AvatarData(array, num, num2);
				Dictionary<PlayerId, AvatarData> avatarCache = this._avatarCache;
				lock (avatarCache)
				{
					if (!this._avatarCache.ContainsKey(providedId))
					{
						this._avatarCache.Add(providedId, avatarData2);
					}
				}
				avatarData = avatarData2;
			}
			return avatarData;
		}

		PlatformInitParams IPlatformServices.GetInitParams()
		{
			return this._initParams;
		}

		internal Task<PlayerId> GetUserWithName(string name)
		{
			return Task.FromResult<PlayerId>(PlayerId.Empty);
		}

		Task<bool> IPlatformServices.ShowOverlayForWebPage(string url)
		{
			bool flag = false;
			Debug.Print("Opening overlay with web page " + url, 0, Debug.DebugColor.White, 17592186044416UL);
			try
			{
				GalaxyInstance.Utils().ShowOverlayWithWebPage(url);
				Debug.Print("Opened overlay with web page " + url, 0, Debug.DebugColor.White, 17592186044416UL);
				flag = true;
			}
			catch (GalaxyInstance.Error error)
			{
				Debug.Print(string.Concat(new object[] { "Could not open overlay with web page ", url, " for reason ", error }), 0, Debug.DebugColor.White, 17592186044416UL);
				flag = false;
			}
			return Task.FromResult<bool>(flag);
		}

		Task<ILoginAccessProvider> IPlatformServices.CreateLobbyClientLoginProvider()
		{
			return Task.FromResult<ILoginAccessProvider>(new GOGLoginAccessProvider());
		}

		IFriendListService[] IPlatformServices.GetFriendListServices()
		{
			return this._friendListServices;
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

		void IPlatformServices.OnFocusGained()
		{
		}

		private void LoadAchievementDataFromXml(string xmlPath)
		{
			this._achievementDatas = new List<GOGPlatformServices.AchievementData>();
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.Load(xmlPath);
			XmlNodeList elementsByTagName = xmlDocument.GetElementsByTagName("Achievement");
			for (int i = 0; i < elementsByTagName.Count; i++)
			{
				XmlNode xmlNode = elementsByTagName[i];
				string innerText = xmlNode.Attributes.GetNamedItem("name").InnerText;
				List<ValueTuple<string, int>> list = new List<ValueTuple<string, int>>();
				XmlNodeList xmlNodeList = null;
				for (int j = 0; j < xmlNode.ChildNodes.Count; j++)
				{
					if (xmlNode.ChildNodes[j].Name == "Requirements")
					{
						xmlNodeList = xmlNode.ChildNodes[j].ChildNodes;
						break;
					}
				}
				for (int k = 0; k < xmlNodeList.Count; k++)
				{
					XmlNode xmlNode2 = xmlNodeList[k];
					string value = xmlNode2.Attributes["statName"].Value;
					int num = int.Parse(xmlNode2.Attributes["threshold"].Value);
					list.Add(new ValueTuple<string, int>(value, num));
				}
				this._achievementDatas.Add(new GOGPlatformServices.AchievementData(innerText, list));
			}
		}

		internal async Task<string> GetUserName(PlayerId providedId)
		{
			string text;
			if (!providedId.IsValid || providedId.ProvidedType != PlayerIdProvidedTypes.GOG)
			{
				text = null;
			}
			else
			{
				GalaxyID gogId = providedId.ToGOGID();
				IFriends friends = GalaxyInstance.Friends();
				UserInformationRetrieveListener informationRetriever = new UserInformationRetrieveListener();
				friends.RequestUserInformation(gogId, 0U, informationRetriever);
				while (!informationRetriever.GotResult)
				{
					await Task.Delay(5);
				}
				string friendPersonaName = friends.GetFriendPersonaName(gogId);
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

		internal bool SetStat(string name, int value)
		{
			bool flag;
			try
			{
				Debug.Print(string.Concat(new object[] { "trying to set stat:", name, " to value:", value }), 0, Debug.DebugColor.White, 17592186044416UL);
				GalaxyInstance.Stats().SetStatInt(name, value);
				for (int i = 0; i < this._achievementDatas.Count; i++)
				{
					GOGPlatformServices.AchievementData achievementData = this._achievementDatas[i];
					foreach (ValueTuple<string, int> valueTuple in achievementData.RequiredStats)
					{
						if (valueTuple.Item1 == name)
						{
							if (value >= valueTuple.Item2)
							{
								this.CheckStatsAndUnlockAchievement(achievementData);
								break;
							}
							break;
						}
					}
				}
				this.InvalidateStats();
				flag = true;
			}
			catch (Exception ex)
			{
				Debug.Print("Could not set stat: " + ex.Message, 0, Debug.DebugColor.White, 17592186044416UL);
				flag = false;
			}
			return flag;
		}

		internal Task<int> GetStat(string name)
		{
			int num = -1;
			try
			{
				num = GalaxyInstance.Stats().GetStatInt(name);
			}
			catch (Exception ex)
			{
				Debug.Print("Could not get stat: " + ex.Message, 0, Debug.DebugColor.White, 17592186044416UL);
			}
			return Task.FromResult<int>(num);
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

		private void RequestUserStatsAndAchievements()
		{
			try
			{
				GalaxyInstance.Stats().RequestUserStatsAndAchievements();
			}
			catch (GalaxyInstance.Error error)
			{
				Debug.Print("Achievements definitions could not be retrieved: " + error, 0, Debug.DebugColor.White, 17592186044416UL);
			}
		}

		private GOGAchievement GetGOGAchievement(string name, GalaxyID galaxyID)
		{
			bool flag = false;
			uint num = 0U;
			GalaxyInstance.Stats().GetAchievement(name, ref flag, ref num, GalaxyInstance.User().GetGalaxyID());
			return new GOGAchievement
			{
				AchievementName = name,
				Name = GalaxyInstance.Stats().GetAchievementDisplayName(name),
				Description = GalaxyInstance.Stats().GetAchievementDescription(name),
				Achieved = flag
			};
		}

		private void CheckStatsAndUnlockAchievements()
		{
			for (int i = 0; i < this._achievementDatas.Count; i++)
			{
				GOGPlatformServices.AchievementData achievementData = this._achievementDatas[i];
				this.CheckStatsAndUnlockAchievement(achievementData);
			}
		}

		private void CheckStatsAndUnlockAchievement(in GOGPlatformServices.AchievementData achievementData)
		{
			if (!this.GetGOGAchievement(achievementData.AchievementName, GalaxyInstance.User().GetGalaxyID()).Achieved)
			{
				bool flag = true;
				foreach (ValueTuple<string, int> valueTuple in achievementData.RequiredStats)
				{
					if (GalaxyInstance.Stats().GetStatInt(valueTuple.Item1) < valueTuple.Item2)
					{
						flag = false;
						break;
					}
				}
				if (flag)
				{
					Debug.Print("trying to set achievement:" + achievementData.AchievementName, 0, Debug.DebugColor.White, 17592186044416UL);
					GalaxyInstance.Stats().SetAchievement(achievementData.AchievementName);
				}
			}
		}

		private void InitListeners()
		{
			this._achievementRetrieveListener = new UserStatsAndAchievementsRetrieveListener();
			this._achievementRetrieveListener.OnUserStatsAndAchievementsRetrieved += this.OnUserStatsAndAchievementsRetrieved;
			this._statsAndAchievementsStoreListener = new StatsAndAchievementsStoreListener();
			this._statsAndAchievementsStoreListener.OnUserStatsAndAchievementsStored += this.OnUserStatsAndAchievementsStored;
		}

		private void OnUserStatsAndAchievementsStored(bool success, IStatsAndAchievementsStoreListener.FailureReason? failureReason)
		{
			if (!success)
			{
				Debug.Print("Failed to store user stats and achievements: " + failureReason.ToString(), 0, Debug.DebugColor.White, 17592186044416UL);
			}
		}

		private void OnUserStatsAndAchievementsRetrieved(GalaxyID userID, bool success, IUserStatsAndAchievementsRetrieveListener.FailureReason? failureReason)
		{
			if (success)
			{
				this.CheckStatsAndUnlockAchievements();
				return;
			}
			Debug.Print("Failed to receive user stats and achievements: " + failureReason.ToString(), 0, Debug.DebugColor.White, 17592186044416UL);
		}

		public void ShowGamepadTextInput(string descriptionText, string existingText, uint maxChars, bool isObfuscated)
		{
		}

		private const string ClientID = "53550366963454221";

		private const string ClientSecret = "c17786edab4b6b3915ab55cfc5bb5a9a0a80b9a2d55d22c0767c9c18477efdb9";

		private PlatformInitParams _initParams;

		private GOGFriendListService _gogFriendListService;

		private IFriendListService[] _friendListServices;

		private bool _initialized;

		private DateTime? _statsLastInvalidated;

		private DateTime _statsLastStored = DateTime.MinValue;

		private UserStatsAndAchievementsRetrieveListener _achievementRetrieveListener;

		private StatsAndAchievementsStoreListener _statsAndAchievementsStoreListener;

		private List<GOGPlatformServices.AchievementData> _achievementDatas;

		private Dictionary<PlayerId, AvatarData> _avatarCache = new Dictionary<PlayerId, AvatarData>();

		private readonly struct AchievementData
		{
			public AchievementData(string achievementName, [TupleElementNames(new string[] { "StatName", "Threshold" })] IReadOnlyList<ValueTuple<string, int>> requiredStats)
			{
				this.AchievementName = achievementName;
				this.RequiredStats = requiredStats;
			}

			public readonly string AchievementName;

			[TupleElementNames(new string[] { "StatName", "Threshold" })]
			public readonly IReadOnlyList<ValueTuple<string, int>> RequiredStats;
		}
	}
}
