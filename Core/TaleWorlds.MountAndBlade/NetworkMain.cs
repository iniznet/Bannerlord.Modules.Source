using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TaleWorlds.Diamond.ClientApplication;
using TaleWorlds.Library;
using TaleWorlds.ModuleManager;
using TaleWorlds.MountAndBlade.Diamond;
using TaleWorlds.PlatformService;
using TaleWorlds.ServiceDiscovery.Client;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000328 RID: 808
	public static class NetworkMain
	{
		// Token: 0x170007DA RID: 2010
		// (get) Token: 0x06002BC9 RID: 11209 RVA: 0x000AA3BD File Offset: 0x000A85BD
		// (set) Token: 0x06002BCA RID: 11210 RVA: 0x000AA3C4 File Offset: 0x000A85C4
		public static bool InternetConnectionAvailable
		{
			get
			{
				return NetworkMain._internetConnectionAvailable;
			}
			private set
			{
				if (value != NetworkMain._internetConnectionAvailable)
				{
					NetworkMain._internetConnectionAvailable = value;
					Action<bool> onInternetConnectionAvailabilityChanged = NetworkMain.OnInternetConnectionAvailabilityChanged;
					if (onInternetConnectionAvailabilityChanged == null)
					{
						return;
					}
					onInternetConnectionAvailabilityChanged(value);
				}
			}
		}

		// Token: 0x06002BCB RID: 11211 RVA: 0x000AA3E4 File Offset: 0x000A85E4
		static NetworkMain()
		{
			ServiceAddressManager.Initalize();
			NetworkMain._lobbyClientApplicationConfiguration = new ClientApplicationConfiguration();
			NetworkMain._lobbyClientApplicationConfiguration.FillFrom("LobbyClient");
			ModuleInfo moduleInfo = ModuleHelper.GetModuleInfo("Multiplayer");
			if (!GameNetwork.IsDedicatedServer && moduleInfo != null)
			{
				NetworkMain._diamondClientApplication = new DiamondClientApplication(moduleInfo.Version);
				NetworkMain._diamondClientApplication.Initialize(NetworkMain._lobbyClientApplicationConfiguration);
				NetworkMain.GameClient = NetworkMain._diamondClientApplication.GetClient<LobbyClient>("LobbyClient");
				MachineId.Initialize();
			}
		}

		// Token: 0x06002BCC RID: 11212 RVA: 0x000AA464 File Offset: 0x000A8664
		public static void Initialize()
		{
			Debug.Print("Initializing NetworkMain", 0, Debug.DebugColor.White, 17592186044416UL);
			MBCommon.CurrentGameType = MBCommon.GameType.Single;
			GameNetwork.InitializeCompressionInfos();
			if (!NetworkMain.IsInitialized)
			{
				NetworkMain.IsInitialized = true;
				GameNetwork.Initialize(new GameNetworkHandler());
			}
			PermaMuteList.SetPermanentMuteAvailableCallback(() => PlatformServices.Instance.IsPermanentMuteAvailable);
			Debug.Print("NetworkMain Initialized", 0, Debug.DebugColor.White, 17592186044416UL);
		}

		// Token: 0x06002BCD RID: 11213 RVA: 0x000AA4E4 File Offset: 0x000A86E4
		private static async void CheckInternetConnection()
		{
			if (NetworkMain.GameClient != null)
			{
				NetworkMain.InternetConnectionAvailable = await NetworkMain.GameClient.CheckConnection();
			}
			NetworkMain._lastInternetConnectionCheck = DateTime.Now.Ticks;
			NetworkMain._checkingConnection = false;
		}

		// Token: 0x06002BCE RID: 11214 RVA: 0x000AA515 File Offset: 0x000A8715
		public static void InitializeAsDedicatedServer()
		{
			MBCommon.CurrentGameType = MBCommon.GameType.MultiServer;
			GameNetwork.InitializeCompressionInfos();
			if (!NetworkMain.IsInitialized)
			{
				NetworkMain.IsInitialized = true;
				GameNetwork.Initialize(new GameNetworkHandler());
				GameStartupInfo startupInfo = Module.CurrentModule.StartupInfo;
			}
		}

		// Token: 0x06002BCF RID: 11215 RVA: 0x000AA544 File Offset: 0x000A8744
		internal static void Tick(float dt)
		{
			if (NetworkMain.IsInitialized)
			{
				if (NetworkMain.GameClient != null)
				{
					NetworkMain.GameClient.Update();
				}
				if (NetworkMain._diamondClientApplication != null)
				{
					NetworkMain._diamondClientApplication.Update();
				}
				GameNetwork.Tick(dt);
			}
			long num = (NetworkMain.InternetConnectionAvailable ? 300000000L : 100000000L);
			if (Module.CurrentModule != null && Module.CurrentModule.StartupInfo.StartupType != GameStartupType.Singleplayer && !NetworkMain._checkingConnection && DateTime.Now.Ticks - NetworkMain._lastInternetConnectionCheck > num)
			{
				NetworkMain._checkingConnection = true;
				Task.Run(delegate
				{
					NetworkMain.CheckInternetConnection();
				});
			}
		}

		// Token: 0x170007DB RID: 2011
		// (get) Token: 0x06002BD0 RID: 11216 RVA: 0x000AA5F7 File Offset: 0x000A87F7
		// (set) Token: 0x06002BD1 RID: 11217 RVA: 0x000AA5FE File Offset: 0x000A87FE
		public static bool IsInitialized { get; private set; } = false;

		// Token: 0x06002BD2 RID: 11218 RVA: 0x000AA608 File Offset: 0x000A8808
		[CommandLineFunctionality.CommandLineArgumentFunction("gettoken", "customserver")]
		public static string GetDedicatedCustomServerAuthToken(List<string> strings)
		{
			if (NetworkMain.GameClient == null)
			{
				return "Not logged into lobby.";
			}
			Task<string> dedicatedCustomServerAuthToken = NetworkMain.GameClient.GetDedicatedCustomServerAuthToken();
			while (!dedicatedCustomServerAuthToken.IsCompleted)
			{
				NetworkMain.GameClient.Update();
			}
			if (dedicatedCustomServerAuthToken.Result == null)
			{
				return "Could not get token.";
			}
			PlatformDirectoryPath platformDirectoryPath = new PlatformDirectoryPath(PlatformFileType.User, "Tokens");
			PlatformFilePath platformFilePath = new PlatformFilePath(platformDirectoryPath, "DedicatedCustomServerAuthToken.txt");
			FileHelper.SaveFileString(platformFilePath, dedicatedCustomServerAuthToken.Result);
			return dedicatedCustomServerAuthToken.Result + " (Saved to " + platformFilePath.FileFullPath + ")";
		}

		// Token: 0x170007DC RID: 2012
		// (get) Token: 0x06002BD3 RID: 11219 RVA: 0x000AA692 File Offset: 0x000A8892
		// (set) Token: 0x06002BD4 RID: 11220 RVA: 0x000AA699 File Offset: 0x000A8899
		public static LobbyClient GameClient { get; private set; }

		// Token: 0x06002BD5 RID: 11221 RVA: 0x000AA6A1 File Offset: 0x000A88A1
		public static MissionLobbyComponent.MultiplayerGameType[] GetAvailableRankedGameModes()
		{
			return new MissionLobbyComponent.MultiplayerGameType[]
			{
				MissionLobbyComponent.MultiplayerGameType.Captain,
				MissionLobbyComponent.MultiplayerGameType.Skirmish
			};
		}

		// Token: 0x06002BD6 RID: 11222 RVA: 0x000AA6B1 File Offset: 0x000A88B1
		public static MissionLobbyComponent.MultiplayerGameType[] GetAvailableCustomGameModes()
		{
			return new MissionLobbyComponent.MultiplayerGameType[]
			{
				MissionLobbyComponent.MultiplayerGameType.TeamDeathmatch,
				MissionLobbyComponent.MultiplayerGameType.Siege
			};
		}

		// Token: 0x06002BD7 RID: 11223 RVA: 0x000AA6C1 File Offset: 0x000A88C1
		public static MissionLobbyComponent.MultiplayerGameType[] GetAvailableQuickPlayGameModes()
		{
			return new MissionLobbyComponent.MultiplayerGameType[]
			{
				MissionLobbyComponent.MultiplayerGameType.Captain,
				MissionLobbyComponent.MultiplayerGameType.Skirmish
			};
		}

		// Token: 0x06002BD8 RID: 11224 RVA: 0x000AA6D1 File Offset: 0x000A88D1
		public static string[] GetAvailableMatchmakerRegions()
		{
			return new string[] { "USE", "USW", "EU", "CN" };
		}

		// Token: 0x06002BD9 RID: 11225 RVA: 0x000AA6F9 File Offset: 0x000A88F9
		public static string GetUserDefaultRegion()
		{
			return "None";
		}

		// Token: 0x06002BDA RID: 11226 RVA: 0x000AA700 File Offset: 0x000A8900
		public static string GetUserCurrentRegion()
		{
			LobbyClient gameClient = NetworkMain.GameClient;
			if (gameClient != null && gameClient.LoggedIn && NetworkMain.GameClient.PlayerData != null)
			{
				return NetworkMain.GameClient.PlayerData.LastRegion;
			}
			return NetworkMain.GetUserDefaultRegion();
		}

		// Token: 0x06002BDB RID: 11227 RVA: 0x000AA736 File Offset: 0x000A8936
		public static string[] GetUserSelectedGameTypes()
		{
			LobbyClient gameClient = NetworkMain.GameClient;
			if (gameClient != null && gameClient.LoggedIn)
			{
				return NetworkMain.GameClient.PlayerData.LastGameTypes;
			}
			return new string[0];
		}

		// Token: 0x0400108D RID: 4237
		public static Action<bool> OnInternetConnectionAvailabilityChanged;

		// Token: 0x0400108E RID: 4238
		private static bool _internetConnectionAvailable;

		// Token: 0x0400108F RID: 4239
		private static ClientApplicationConfiguration _lobbyClientApplicationConfiguration;

		// Token: 0x04001090 RID: 4240
		private static long _lastInternetConnectionCheck;

		// Token: 0x04001091 RID: 4241
		private static bool _checkingConnection;

		// Token: 0x04001092 RID: 4242
		private const long InternetConnectionCheckIntervalShort = 100000000L;

		// Token: 0x04001093 RID: 4243
		private const long InternetConnectionCheckIntervalLong = 300000000L;

		// Token: 0x04001094 RID: 4244
		private static DiamondClientApplication _diamondClientApplication;
	}
}
