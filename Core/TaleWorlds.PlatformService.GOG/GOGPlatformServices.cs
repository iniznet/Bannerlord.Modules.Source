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
	// Token: 0x02000008 RID: 8
	public class GOGPlatformServices : IPlatformServices
	{
		// Token: 0x17000006 RID: 6
		// (get) Token: 0x0600002C RID: 44 RVA: 0x00002449 File Offset: 0x00000649
		private static GOGPlatformServices Instance
		{
			get
			{
				return PlatformServices.Instance as GOGPlatformServices;
			}
		}

		// Token: 0x0600002D RID: 45 RVA: 0x00002458 File Offset: 0x00000658
		public GOGPlatformServices(PlatformInitParams initParams)
		{
			this.LoadAchievementDataFromXml((string)initParams["AchievementDataXmlPath"]);
			this._initParams = initParams;
			AvatarServices.AddAvatarService(PlayerIdProvidedTypes.GOG, new GOGPlatformAvatarService(this));
			this._gogFriendListService = new GOGFriendListService(this);
		}

		// Token: 0x17000007 RID: 7
		// (get) Token: 0x0600002E RID: 46 RVA: 0x000024B6 File Offset: 0x000006B6
		string IPlatformServices.ProviderName
		{
			get
			{
				return "GOG";
			}
		}

		// Token: 0x17000008 RID: 8
		// (get) Token: 0x0600002F RID: 47 RVA: 0x000024C0 File Offset: 0x000006C0
		string IPlatformServices.UserId
		{
			get
			{
				return GalaxyInstance.User().GetGalaxyID().ToUint64()
					.ToString();
			}
		}

		// Token: 0x17000009 RID: 9
		// (get) Token: 0x06000030 RID: 48 RVA: 0x000024E4 File Offset: 0x000006E4
		bool IPlatformServices.UserLoggedIn
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		// Token: 0x06000031 RID: 49 RVA: 0x000024EB File Offset: 0x000006EB
		void IPlatformServices.LoginUser()
		{
			throw new NotImplementedException();
		}

		// Token: 0x1700000A RID: 10
		// (get) Token: 0x06000032 RID: 50 RVA: 0x000024F2 File Offset: 0x000006F2
		string IPlatformServices.UserDisplayName
		{
			get
			{
				return "";
			}
		}

		// Token: 0x1700000B RID: 11
		// (get) Token: 0x06000033 RID: 51 RVA: 0x000024F9 File Offset: 0x000006F9
		IReadOnlyCollection<PlayerId> IPlatformServices.BlockedUsers
		{
			get
			{
				return new List<PlayerId>();
			}
		}

		// Token: 0x06000034 RID: 52 RVA: 0x00002500 File Offset: 0x00000700
		IAchievementService IPlatformServices.GetAchievementService()
		{
			return new GOGAchievementService(this);
		}

		// Token: 0x06000035 RID: 53 RVA: 0x00002508 File Offset: 0x00000708
		IActivityService IPlatformServices.GetActivityService()
		{
			return new TestActivityService();
		}

		// Token: 0x06000036 RID: 54 RVA: 0x0000250F File Offset: 0x0000070F
		Task<bool> IPlatformServices.VerifyString(string content)
		{
			return Task.FromResult<bool>(true);
		}

		// Token: 0x06000037 RID: 55 RVA: 0x00002517 File Offset: 0x00000717
		void IPlatformServices.GetPlatformId(PlayerId playerId, Action<object> callback)
		{
			callback(new GalaxyID(playerId.Part4));
		}

		// Token: 0x1700000C RID: 12
		// (get) Token: 0x06000038 RID: 56 RVA: 0x0000252B File Offset: 0x0000072B
		bool IPlatformServices.IsPermanentMuteAvailable
		{
			get
			{
				return true;
			}
		}

		// Token: 0x06000039 RID: 57 RVA: 0x00002530 File Offset: 0x00000730
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

		// Token: 0x0600003A RID: 58 RVA: 0x00002668 File Offset: 0x00000868
		void IPlatformServices.Tick(float dt)
		{
			GalaxyInstance.ProcessData();
			if (this._initialized)
			{
				this.CheckStoreStats();
			}
		}

		// Token: 0x0600003B RID: 59 RVA: 0x0000267D File Offset: 0x0000087D
		void IPlatformServices.Terminate()
		{
			GalaxyInstance.Shutdown(true);
		}

		// Token: 0x0600003C RID: 60 RVA: 0x00002685 File Offset: 0x00000885
		private void InvalidateStats()
		{
			this._statsLastInvalidated = new DateTime?(DateTime.Now);
		}

		// Token: 0x0600003D RID: 61 RVA: 0x00002698 File Offset: 0x00000898
		private void CheckStoreStats()
		{
			if (this._statsLastInvalidated != null && DateTime.Now.Subtract(this._statsLastInvalidated.Value).TotalSeconds > 5.0 && DateTime.Now.Subtract(this._statsLastStored).TotalSeconds > 30.0)
			{
				this._statsLastStored = DateTime.Now;
				GalaxyInstance.Stats().StoreStatsAndAchievements();
				this._statsLastInvalidated = null;
			}
		}

		// Token: 0x14000004 RID: 4
		// (add) Token: 0x0600003E RID: 62 RVA: 0x00002724 File Offset: 0x00000924
		// (remove) Token: 0x0600003F RID: 63 RVA: 0x0000275C File Offset: 0x0000095C
		public event Action<AvatarData> OnAvatarUpdated;

		// Token: 0x14000005 RID: 5
		// (add) Token: 0x06000040 RID: 64 RVA: 0x00002794 File Offset: 0x00000994
		// (remove) Token: 0x06000041 RID: 65 RVA: 0x000027CC File Offset: 0x000009CC
		public event Action<string> OnNameUpdated;

		// Token: 0x14000006 RID: 6
		// (add) Token: 0x06000042 RID: 66 RVA: 0x00002804 File Offset: 0x00000A04
		// (remove) Token: 0x06000043 RID: 67 RVA: 0x0000283C File Offset: 0x00000A3C
		public event Action<bool, TextObject> OnSignInStateUpdated;

		// Token: 0x14000007 RID: 7
		// (add) Token: 0x06000044 RID: 68 RVA: 0x00002874 File Offset: 0x00000A74
		// (remove) Token: 0x06000045 RID: 69 RVA: 0x000028AC File Offset: 0x00000AAC
		public event Action OnBlockedUserListUpdated;

		// Token: 0x06000046 RID: 70 RVA: 0x000028E4 File Offset: 0x00000AE4
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

		// Token: 0x06000047 RID: 71 RVA: 0x00002941 File Offset: 0x00000B41
		bool IPlatformServices.IsPlayerProfileCardAvailable(PlayerId providedId)
		{
			return false;
		}

		// Token: 0x06000048 RID: 72 RVA: 0x00002944 File Offset: 0x00000B44
		void IPlatformServices.ShowPlayerProfileCard(PlayerId providedId)
		{
		}

		// Token: 0x06000049 RID: 73 RVA: 0x00002946 File Offset: 0x00000B46
		internal void ClearAvatarCache()
		{
			this._avatarCache.Clear();
		}

		// Token: 0x0600004A RID: 74 RVA: 0x00002954 File Offset: 0x00000B54
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

		// Token: 0x0600004B RID: 75 RVA: 0x000029A1 File Offset: 0x00000BA1
		PlatformInitParams IPlatformServices.GetInitParams()
		{
			return this._initParams;
		}

		// Token: 0x0600004C RID: 76 RVA: 0x000029A9 File Offset: 0x00000BA9
		internal Task<PlayerId> GetUserWithName(string name)
		{
			return Task.FromResult<PlayerId>(PlayerId.Empty);
		}

		// Token: 0x0600004D RID: 77 RVA: 0x000029B8 File Offset: 0x00000BB8
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

		// Token: 0x0600004E RID: 78 RVA: 0x00002A60 File Offset: 0x00000C60
		Task<ILoginAccessProvider> IPlatformServices.CreateLobbyClientLoginProvider()
		{
			return Task.FromResult<ILoginAccessProvider>(new GOGLoginAccessProvider());
		}

		// Token: 0x0600004F RID: 79 RVA: 0x00002A6C File Offset: 0x00000C6C
		IFriendListService[] IPlatformServices.GetFriendListServices()
		{
			return this._friendListServices;
		}

		// Token: 0x06000050 RID: 80 RVA: 0x00002A74 File Offset: 0x00000C74
		void IPlatformServices.CheckPrivilege(Privilege privilege, bool displayResolveUI, PrivilegeResult callback)
		{
			callback(true);
		}

		// Token: 0x06000051 RID: 81 RVA: 0x00002A7D File Offset: 0x00000C7D
		void IPlatformServices.CheckPermissionWithUser(Permission privilege, PlayerId targetPlayerId, PermissionResult callback)
		{
			callback(true);
		}

		// Token: 0x06000052 RID: 82 RVA: 0x00002A86 File Offset: 0x00000C86
		bool IPlatformServices.RegisterPermissionChangeEvent(PlayerId targetPlayerId, Permission permission, PermissionChanged callback)
		{
			return false;
		}

		// Token: 0x06000053 RID: 83 RVA: 0x00002A89 File Offset: 0x00000C89
		bool IPlatformServices.UnregisterPermissionChangeEvent(PlayerId targetPlayerId, Permission permission, PermissionChanged callback)
		{
			return false;
		}

		// Token: 0x06000054 RID: 84 RVA: 0x00002A8C File Offset: 0x00000C8C
		void IPlatformServices.ShowRestrictedInformation()
		{
		}

		// Token: 0x06000055 RID: 85 RVA: 0x00002A8E File Offset: 0x00000C8E
		void IPlatformServices.OnFocusGained()
		{
		}

		// Token: 0x06000056 RID: 86 RVA: 0x00002A90 File Offset: 0x00000C90
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

		// Token: 0x06000057 RID: 87 RVA: 0x00002BC0 File Offset: 0x00000DC0
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

		// Token: 0x06000058 RID: 88 RVA: 0x00002C08 File Offset: 0x00000E08
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

		// Token: 0x06000059 RID: 89 RVA: 0x00002D1C File Offset: 0x00000F1C
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

		// Token: 0x0600005A RID: 90 RVA: 0x00002D74 File Offset: 0x00000F74
		internal Task<int[]> GetStats(string[] names)
		{
			List<int> list = new List<int>();
			foreach (string text in names)
			{
				list.Add(this.GetStat(text).Result);
			}
			return Task.FromResult<int[]>(list.ToArray());
		}

		// Token: 0x0600005B RID: 91 RVA: 0x00002DB8 File Offset: 0x00000FB8
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

		// Token: 0x0600005C RID: 92 RVA: 0x00002E00 File Offset: 0x00001000
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

		// Token: 0x0600005D RID: 93 RVA: 0x00002E60 File Offset: 0x00001060
		private void CheckStatsAndUnlockAchievements()
		{
			for (int i = 0; i < this._achievementDatas.Count; i++)
			{
				GOGPlatformServices.AchievementData achievementData = this._achievementDatas[i];
				this.CheckStatsAndUnlockAchievement(achievementData);
			}
		}

		// Token: 0x0600005E RID: 94 RVA: 0x00002E98 File Offset: 0x00001098
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

		// Token: 0x0600005F RID: 95 RVA: 0x00002F50 File Offset: 0x00001150
		private void InitListeners()
		{
			this._achievementRetrieveListener = new UserStatsAndAchievementsRetrieveListener();
			this._achievementRetrieveListener.OnUserStatsAndAchievementsRetrieved += this.OnUserStatsAndAchievementsRetrieved;
			this._statsAndAchievementsStoreListener = new StatsAndAchievementsStoreListener();
			this._statsAndAchievementsStoreListener.OnUserStatsAndAchievementsStored += this.OnUserStatsAndAchievementsStored;
		}

		// Token: 0x06000060 RID: 96 RVA: 0x00002FA1 File Offset: 0x000011A1
		private void OnUserStatsAndAchievementsStored(bool success, IStatsAndAchievementsStoreListener.FailureReason? failureReason)
		{
			if (!success)
			{
				Debug.Print("Failed to store user stats and achievements: " + failureReason.ToString(), 0, Debug.DebugColor.White, 17592186044416UL);
			}
		}

		// Token: 0x06000061 RID: 97 RVA: 0x00002FCE File Offset: 0x000011CE
		private void OnUserStatsAndAchievementsRetrieved(GalaxyID userID, bool success, IUserStatsAndAchievementsRetrieveListener.FailureReason? failureReason)
		{
			if (success)
			{
				this.CheckStatsAndUnlockAchievements();
				return;
			}
			Debug.Print("Failed to receive user stats and achievements: " + failureReason.ToString(), 0, Debug.DebugColor.White, 17592186044416UL);
		}

		// Token: 0x04000008 RID: 8
		private const string ClientID = "53550366963454221";

		// Token: 0x04000009 RID: 9
		private const string ClientSecret = "c17786edab4b6b3915ab55cfc5bb5a9a0a80b9a2d55d22c0767c9c18477efdb9";

		// Token: 0x0400000A RID: 10
		private PlatformInitParams _initParams;

		// Token: 0x0400000B RID: 11
		private GOGFriendListService _gogFriendListService;

		// Token: 0x0400000C RID: 12
		private IFriendListService[] _friendListServices;

		// Token: 0x0400000D RID: 13
		private bool _initialized;

		// Token: 0x0400000E RID: 14
		private DateTime? _statsLastInvalidated;

		// Token: 0x0400000F RID: 15
		private DateTime _statsLastStored = DateTime.MinValue;

		// Token: 0x04000010 RID: 16
		private UserStatsAndAchievementsRetrieveListener _achievementRetrieveListener;

		// Token: 0x04000011 RID: 17
		private StatsAndAchievementsStoreListener _statsAndAchievementsStoreListener;

		// Token: 0x04000012 RID: 18
		private List<GOGPlatformServices.AchievementData> _achievementDatas;

		// Token: 0x04000013 RID: 19
		private Dictionary<PlayerId, AvatarData> _avatarCache = new Dictionary<PlayerId, AvatarData>();

		// Token: 0x02000015 RID: 21
		private readonly struct AchievementData
		{
			// Token: 0x06000097 RID: 151 RVA: 0x000037AE File Offset: 0x000019AE
			public AchievementData(string achievementName, [TupleElementNames(new string[] { "StatName", "Threshold" })] IReadOnlyList<ValueTuple<string, int>> requiredStats)
			{
				this.AchievementName = achievementName;
				this.RequiredStats = requiredStats;
			}

			// Token: 0x0400003E RID: 62
			public readonly string AchievementName;

			// Token: 0x0400003F RID: 63
			[TupleElementNames(new string[] { "StatName", "Threshold" })]
			public readonly IReadOnlyList<ValueTuple<string, int>> RequiredStats;
		}
	}
}
