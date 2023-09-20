using System;
using System.Collections.Generic;
using TaleWorlds.Diamond.ClientApplication;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.ModuleManager;
using TaleWorlds.MountAndBlade.Diamond;
using TaleWorlds.PlatformService;
using TaleWorlds.ServiceDiscovery.Client;

namespace TaleWorlds.MountAndBlade.Multiplayer
{
	public static class MultiplayerMain
	{
		public static LobbyClient GameClient
		{
			get
			{
				return NetworkMain.GameClient;
			}
		}

		static MultiplayerMain()
		{
			ServiceAddressManager.Initalize();
			MultiplayerMain._lobbyClientApplicationConfiguration = new ClientApplicationConfiguration();
			MultiplayerMain._lobbyClientApplicationConfiguration.FillFrom("LobbyClient");
			ModuleInfo moduleInfo = ModuleHelper.GetModuleInfo("Multiplayer");
			if (!GameNetwork.IsDedicatedServer && moduleInfo != null)
			{
				MultiplayerMain._diamondClientApplication = new DiamondClientApplication(moduleInfo.Version);
				MultiplayerMain._diamondClientApplication.Initialize(MultiplayerMain._lobbyClientApplicationConfiguration);
				NetworkMain.SetPeers(MultiplayerMain._diamondClientApplication.GetClient<LobbyClient>("LobbyClient"), new CommunityClient());
				MachineId.Initialize();
			}
		}

		public static void Initialize(IGameNetworkHandler gameNetworkHandler)
		{
			Debug.Print("Initializing NetworkMain", 0, 12, 17592186044416UL);
			MBCommon.CurrentGameType = 0;
			GameNetwork.InitializeCompressionInfos();
			if (!MultiplayerMain.IsInitialized)
			{
				MultiplayerMain.IsInitialized = true;
				GameNetwork.Initialize(gameNetworkHandler);
			}
			PermaMuteList.SetPermanentMuteAvailableCallback(() => PlatformServices.Instance.IsPermanentMuteAvailable);
			Debug.Print("NetworkMain Initialized", 0, 12, 17592186044416UL);
		}

		public static void InitializeAsDedicatedServer(IGameNetworkHandler gameNetworkHandler)
		{
			MBCommon.CurrentGameType = 2;
			GameNetwork.InitializeCompressionInfos();
			if (!MultiplayerMain.IsInitialized)
			{
				MultiplayerMain.IsInitialized = true;
				GameNetwork.Initialize(gameNetworkHandler);
				GameNetwork.SetServerBandwidthLimitInMbps(Module.CurrentModule.StartupInfo.ServerBandwidthLimitInMbps);
			}
		}

		internal static void Tick(float dt)
		{
			if (MultiplayerMain.IsInitialized)
			{
				if (MultiplayerMain.GameClient != null)
				{
					MultiplayerMain.GameClient.Update();
				}
				if (MultiplayerMain._diamondClientApplication != null)
				{
					MultiplayerMain._diamondClientApplication.Update();
				}
				GameNetwork.Tick(dt);
			}
		}

		public static bool IsInitialized { get; private set; } = false;

		public static MultiplayerGameType[] GetAvailableRankedGameModes()
		{
			return new MultiplayerGameType[] { 5, 6 };
		}

		public static MultiplayerGameType[] GetAvailableCustomGameModes()
		{
			return new MultiplayerGameType[] { 1, 3 };
		}

		public static MultiplayerGameType[] GetAvailableQuickPlayGameModes()
		{
			return new MultiplayerGameType[] { 5, 6 };
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
			LobbyClient gameClient = MultiplayerMain.GameClient;
			if (gameClient != null && gameClient.LoggedIn && MultiplayerMain.GameClient.PlayerData != null)
			{
				return MultiplayerMain.GameClient.PlayerData.LastRegion;
			}
			return MultiplayerMain.GetUserDefaultRegion();
		}

		public static string[] GetUserSelectedGameTypes()
		{
			LobbyClient gameClient = MultiplayerMain.GameClient;
			if (gameClient != null && gameClient.LoggedIn)
			{
				return MultiplayerMain.GameClient.PlayerData.LastGameTypes;
			}
			return new string[0];
		}

		[CommandLineFunctionality.CommandLineArgumentFunction("gettoken", "customserver")]
		public static string GetDedicatedCustomServerAuthToken(List<string> strings)
		{
			if (!(Common.PlatformFileHelper is PlatformFileHelperPC))
			{
				return "Platform not supported.";
			}
			if (MultiplayerMain.GameClient == null)
			{
				return "Not logged into lobby.";
			}
			MultiplayerMain.GetDedicatedCustomServerAuthToken();
			return string.Empty;
		}

		private static async void GetDedicatedCustomServerAuthToken()
		{
			string text = await MultiplayerMain.GameClient.GetDedicatedCustomServerAuthToken();
			if (text == null)
			{
				MBDebug.EchoCommandWindow("Could not get token.");
			}
			else
			{
				PlatformDirectoryPath platformDirectoryPath;
				platformDirectoryPath..ctor(0, "Tokens");
				PlatformFilePath platformFilePath;
				platformFilePath..ctor(platformDirectoryPath, "DedicatedCustomServerAuthToken.txt");
				FileHelper.SaveFileString(platformFilePath, text);
				MBDebug.EchoCommandWindow(text + " (Saved to " + platformFilePath.FileFullPath + ")");
			}
		}

		private static ClientApplicationConfiguration _lobbyClientApplicationConfiguration;

		private static DiamondClientApplication _diamondClientApplication;
	}
}
