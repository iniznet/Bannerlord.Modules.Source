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
	public static class NetworkMain
	{
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

		private static async void CheckInternetConnection()
		{
			if (NetworkMain.GameClient != null)
			{
				NetworkMain.InternetConnectionAvailable = await NetworkMain.GameClient.CheckConnection();
			}
			NetworkMain._lastInternetConnectionCheck = DateTime.Now.Ticks;
			NetworkMain._checkingConnection = false;
		}

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

		public static bool IsInitialized { get; private set; } = false;

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

		public static LobbyClient GameClient { get; private set; }

		public static MissionLobbyComponent.MultiplayerGameType[] GetAvailableRankedGameModes()
		{
			return new MissionLobbyComponent.MultiplayerGameType[]
			{
				MissionLobbyComponent.MultiplayerGameType.Captain,
				MissionLobbyComponent.MultiplayerGameType.Skirmish
			};
		}

		public static MissionLobbyComponent.MultiplayerGameType[] GetAvailableCustomGameModes()
		{
			return new MissionLobbyComponent.MultiplayerGameType[]
			{
				MissionLobbyComponent.MultiplayerGameType.TeamDeathmatch,
				MissionLobbyComponent.MultiplayerGameType.Siege
			};
		}

		public static MissionLobbyComponent.MultiplayerGameType[] GetAvailableQuickPlayGameModes()
		{
			return new MissionLobbyComponent.MultiplayerGameType[]
			{
				MissionLobbyComponent.MultiplayerGameType.Captain,
				MissionLobbyComponent.MultiplayerGameType.Skirmish
			};
		}

		public static string[] GetAvailableMatchmakerRegions()
		{
			return new string[] { "USE", "USW", "EU", "CN" };
		}

		public static string GetUserDefaultRegion()
		{
			return "None";
		}

		public static string GetUserCurrentRegion()
		{
			LobbyClient gameClient = NetworkMain.GameClient;
			if (gameClient != null && gameClient.LoggedIn && NetworkMain.GameClient.PlayerData != null)
			{
				return NetworkMain.GameClient.PlayerData.LastRegion;
			}
			return NetworkMain.GetUserDefaultRegion();
		}

		public static string[] GetUserSelectedGameTypes()
		{
			LobbyClient gameClient = NetworkMain.GameClient;
			if (gameClient != null && gameClient.LoggedIn)
			{
				return NetworkMain.GameClient.PlayerData.LastGameTypes;
			}
			return new string[0];
		}

		public static Action<bool> OnInternetConnectionAvailabilityChanged;

		private static bool _internetConnectionAvailable;

		private static ClientApplicationConfiguration _lobbyClientApplicationConfiguration;

		private static long _lastInternetConnectionCheck;

		private static bool _checkingConnection;

		private const long InternetConnectionCheckIntervalShort = 100000000L;

		private const long InternetConnectionCheckIntervalLong = 300000000L;

		private static DiamondClientApplication _diamondClientApplication;
	}
}
