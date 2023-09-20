using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.Diamond;
using TaleWorlds.MountAndBlade.GauntletUI.Multiplayer;
using TaleWorlds.ScreenSystem;

namespace TaleWorlds.MountAndBlade.DedicatedCustomServer.ClientHelper
{
	public class DedicatedCustomServerClientHelperSubModule : MBSubModuleBase
	{
		public DedicatedCustomServerClientHelperSubModule()
		{
			this._httpClient = new HttpClient();
		}

		protected override void OnSubModuleLoad()
		{
			DedicatedCustomServerClientHelperSubModule.Instance = this;
			base.OnSubModuleLoad();
			ModLogger.Log("Loaded", 0, 4);
		}

		public override void OnMultiplayerGameStart(Game game, object _)
		{
			game.GameStateManager.RegisterListener(new DedicatedCustomServerClientHelperSubModule.StateManagerListener());
		}

		public async Task DownloadMapFromHost(string hostAddress, string mapName, bool replaceExisting = false, IProgress<ProgressUpdate> progress = null, CancellationToken cancellationToken = default(CancellationToken))
		{
			string text = "http://" + hostAddress + "/maps/" + ((mapName == null) ? "current" : ("list/" + mapName));
			string text2;
			try
			{
				ModLogger.Log("Downloading from '" + hostAddress + "' in thread...", 0, 4);
				text2 = await ModHelpers.DownloadToTempFile(this._httpClient, text, progress, cancellationToken);
			}
			catch (Exception ex)
			{
				throw new Exception(new TextObject("{=lTiacca1}Failed to download map file '{MAP_NAME}'", null).SetTextVariable("MAP_NAME", mapName).ToString() + ": " + ex.Message, ex);
			}
			bool flag = true;
			string text3;
			bool flag2 = Utilities.TryGetFullFilePathOfScene(mapName, ref text3);
			bool flag3 = flag2 && !ModHelpers.DoesSceneFolderAlreadyExist(mapName);
			string text5;
			try
			{
				string text4 = ModHelpers.ExtractZipToTempDirectory(text2);
				File.Delete(text2);
				text5 = ModHelpers.ReadSceneNameOfDirectory(text4);
				string text6 = Path.Combine(ModHelpers.GetSceneObjRootPath(), text5);
				if (Directory.Exists(text6))
				{
					if (!replaceExisting)
					{
						Directory.Delete(text4, true);
						throw new Exception(new TextObject("{=5bbkOm7r}Map already exists at '{MAP_PATH}', delete this directory first if you want to re-download", null).SetTextVariable("MAP_PATH", text6).ToString());
					}
					flag = !flag2;
					ModLogger.Warn("Been told to replace existing map, deleting '" + text6 + "'");
					Directory.Delete(text6, true);
				}
				Directory.Move(text4, text6);
				ModLogger.Log("Scene is available at '" + text6 + "'", 0, 4);
			}
			catch (Exception ex2)
			{
				throw new Exception(new TextObject("{=oaNqdada}Failed to save map scene '{MAP_NAME}'", null).SetTextVariable("MAP_NAME", mapName).ToString() + ": " + ex2.Message, ex2);
			}
			if (flag3)
			{
				throw new Exception(new TextObject("{=Urgon7l2}'{MAP_NAME}' was downloaded, but another module already has a scene with this name. To play the new scene, restart the game without that module/scene.", null).SetTextVariable("MAP_NAME", mapName).ToString());
			}
			if (flag)
			{
				try
				{
					Utilities.PairSceneNameToModuleName(text5, "DedicatedCustomServerHelper");
					ModLogger.Log("RGL has been informed of the module pairing for scene '" + text5 + "'", 0, 4);
				}
				catch (Exception ex3)
				{
					throw new Exception(new TextObject("{=iRossEAk}Failed to inform RGL about the new scene '{SCENE_NAME}'", null).SetTextVariable("SCENE_NAME", text5).ToString() + ": " + ex3.Message, ex3);
				}
			}
		}

		public async Task<MapListResponse> GetMapListFromHost(string hostAddress)
		{
			MapListResponse mapListResponse;
			try
			{
				string text = await HttpHelper.DownloadStringTaskAsync("http://" + hostAddress + "/maps/list");
				ModLogger.Log("'" + hostAddress + "' has a map list of: " + text, 0, 4);
				mapListResponse = JsonConvert.DeserializeObject<MapListResponse>(text);
			}
			catch (Exception ex)
			{
				throw new Exception(new TextObject("{=5ZkdGgnQ}Failed to retrieve map list of '{HOST_ADDRESS}'", null).SetTextVariable("HOST_ADDRESS", hostAddress).ToString() + ": " + ex.Message, ex);
			}
			return mapListResponse;
		}

		[CommandLineFunctionality.CommandLineArgumentFunction("download_map", "dcshelper")]
		public static string DownloadMapCommand(List<string> strings)
		{
			DedicatedCustomServerClientHelperSubModule.<>c__DisplayClass13_0 CS$<>8__locals1 = new DedicatedCustomServerClientHelperSubModule.<>c__DisplayClass13_0();
			string text = "Usage: dcshelper.download_map [host_address[:port]] [map_name]\nOmit map_name to download the currently played map";
			if (strings.Count == 0)
			{
				return text;
			}
			CS$<>8__locals1.hostAddress = strings[0];
			CS$<>8__locals1.mapArg = ((strings.Count > 1) ? strings[1] : null);
			Task.Run(delegate
			{
				DedicatedCustomServerClientHelperSubModule.<>c__DisplayClass13_0.<<DownloadMapCommand>b__0>d <<DownloadMapCommand>b__0>d;
				<<DownloadMapCommand>b__0>d.<>4__this = CS$<>8__locals1;
				<<DownloadMapCommand>b__0>d.<>t__builder = AsyncTaskMethodBuilder.Create();
				<<DownloadMapCommand>b__0>d.<>1__state = -1;
				AsyncTaskMethodBuilder <>t__builder = <<DownloadMapCommand>b__0>d.<>t__builder;
				<>t__builder.Start<DedicatedCustomServerClientHelperSubModule.<>c__DisplayClass13_0.<<DownloadMapCommand>b__0>d>(ref <<DownloadMapCommand>b__0>d);
				return <<DownloadMapCommand>b__0>d.<>t__builder.Task;
			});
			return "Attempting to download in the background...";
		}

		[CommandLineFunctionality.CommandLineArgumentFunction("get_map_list", "dcshelper")]
		public static string GetMapListCommand(List<string> strings)
		{
			DedicatedCustomServerClientHelperSubModule.<>c__DisplayClass14_0 CS$<>8__locals1 = new DedicatedCustomServerClientHelperSubModule.<>c__DisplayClass14_0();
			string text = "Usage: dcshelper.get_map_list [host_address[:port]]";
			if (strings.Count != 1)
			{
				return text;
			}
			CS$<>8__locals1.hostAddress = strings[0];
			Task.Run(delegate
			{
				DedicatedCustomServerClientHelperSubModule.<>c__DisplayClass14_0.<<GetMapListCommand>b__0>d <<GetMapListCommand>b__0>d;
				<<GetMapListCommand>b__0>d.<>4__this = CS$<>8__locals1;
				<<GetMapListCommand>b__0>d.<>t__builder = AsyncTaskMethodBuilder.Create();
				<<GetMapListCommand>b__0>d.<>1__state = -1;
				AsyncTaskMethodBuilder <>t__builder = <<GetMapListCommand>b__0>d.<>t__builder;
				<>t__builder.Start<DedicatedCustomServerClientHelperSubModule.<>c__DisplayClass14_0.<<GetMapListCommand>b__0>d>(ref <<GetMapListCommand>b__0>d);
				return <<GetMapListCommand>b__0>d.<>t__builder.Task;
			});
			return "The map list was printed to the debug console";
		}

		[CommandLineFunctionality.CommandLineArgumentFunction("open_download_panel", "dcshelper")]
		public static string OpenDownloadPanel(List<string> strings)
		{
			DedicatedCustomServerClientHelperSubModule.<>c__DisplayClass15_0 CS$<>8__locals1 = new DedicatedCustomServerClientHelperSubModule.<>c__DisplayClass15_0();
			string text = "Usage: dcshelper.open_download_panel [host_address[:port]]";
			if (strings.Count != 1)
			{
				return text;
			}
			CS$<>8__locals1.hostAddress = strings[0];
			if (!(ScreenManager.TopScreen is MultiplayerLobbyGauntletScreen))
			{
				return "The download panel can only be opened while on the multiplayer lobby.";
			}
			Task.Run(delegate
			{
				DedicatedCustomServerClientHelperSubModule.<>c__DisplayClass15_0.<<OpenDownloadPanel>b__0>d <<OpenDownloadPanel>b__0>d;
				<<OpenDownloadPanel>b__0>d.<>4__this = CS$<>8__locals1;
				<<OpenDownloadPanel>b__0>d.<>t__builder = AsyncTaskMethodBuilder.Create();
				<<OpenDownloadPanel>b__0>d.<>1__state = -1;
				AsyncTaskMethodBuilder <>t__builder = <<OpenDownloadPanel>b__0>d.<>t__builder;
				<>t__builder.Start<DedicatedCustomServerClientHelperSubModule.<>c__DisplayClass15_0.<<OpenDownloadPanel>b__0>d>(ref <<OpenDownloadPanel>b__0>d);
				return <<OpenDownloadPanel>b__0>d.<>t__builder.Task;
			});
			return "Opening download panel for host '" + CS$<>8__locals1.hostAddress + "'...";
		}

		public const string ModuleName = "DedicatedCustomServerHelper";

		public static readonly bool DebugMode;

		public static DedicatedCustomServerClientHelperSubModule Instance;

		private readonly HttpClient _httpClient;

		private const string CommandGroup = "dcshelper";

		private const string DownloadMapCommandName = "download_map";

		private const string GetMapListCommandName = "get_map_list";

		private const string OpenDownloadPanelCommandName = "open_download_panel";

		private class LobbyStateListener : IGameStateListener
		{
			public LobbyStateListener(LobbyState lobbyState)
			{
				this._lobbyState = lobbyState;
			}

			private bool ServerSupportsDownloadPanel(GameServerEntry serverEntry)
			{
				return serverEntry.LoadedModules.Any((ModuleInfoModel m) => m.Id == "DedicatedCustomServerHelper");
			}

			private void OpenDownloadPanelForServer(GameServerEntry serverEntry)
			{
				DedicatedCustomServerClientHelperSubModule.LobbyStateListener.<>c__DisplayClass3_0 CS$<>8__locals1 = new DedicatedCustomServerClientHelperSubModule.LobbyStateListener.<>c__DisplayClass3_0();
				CS$<>8__locals1.serverEntry = serverEntry;
				Task.Run(delegate
				{
					DedicatedCustomServerClientHelperSubModule.LobbyStateListener.<>c__DisplayClass3_0.<<OpenDownloadPanelForServer>b__0>d <<OpenDownloadPanelForServer>b__0>d;
					<<OpenDownloadPanelForServer>b__0>d.<>4__this = CS$<>8__locals1;
					<<OpenDownloadPanelForServer>b__0>d.<>t__builder = AsyncTaskMethodBuilder.Create();
					<<OpenDownloadPanelForServer>b__0>d.<>1__state = -1;
					AsyncTaskMethodBuilder <>t__builder = <<OpenDownloadPanelForServer>b__0>d.<>t__builder;
					<>t__builder.Start<DedicatedCustomServerClientHelperSubModule.LobbyStateListener.<>c__DisplayClass3_0.<<OpenDownloadPanelForServer>b__0>d>(ref <<OpenDownloadPanelForServer>b__0>d);
					return <<OpenDownloadPanelForServer>b__0>d.<>t__builder.Task;
				});
			}

			private List<CustomServerAction> ActionSupplier(GameServerEntry serverEntry)
			{
				if (!this.ServerSupportsDownloadPanel(serverEntry))
				{
					return null;
				}
				return new List<CustomServerAction>
				{
					new CustomServerAction(delegate
					{
						this.OpenDownloadPanelForServer(serverEntry);
					}, serverEntry, new TextObject("{=ebuelCXT}Open Download Panel", null).ToString())
				};
			}

			private void HandleFailedServerJoinAttempt(GameServerEntry serverEntry)
			{
				if (this.ServerSupportsDownloadPanel(serverEntry))
				{
					this.OpenDownloadPanelForServer(serverEntry);
				}
			}

			public void OnActivate()
			{
				this._lobbyState.RegisterForCustomServerAction(new Func<GameServerEntry, List<CustomServerAction>>(this.ActionSupplier));
				this._lobbyState.ClientRefusedToJoinCustomServer += this.HandleFailedServerJoinAttempt;
			}

			public void OnDeactivate()
			{
				this._lobbyState.UnregisterForCustomServerAction(new Func<GameServerEntry, List<CustomServerAction>>(this.ActionSupplier));
				this._lobbyState.ClientRefusedToJoinCustomServer -= this.HandleFailedServerJoinAttempt;
			}

			public void OnFinalize()
			{
				this._lobbyState = null;
			}

			public void OnInitialize()
			{
			}

			private LobbyState _lobbyState;
		}

		private class StateManagerListener : IGameStateManagerListener
		{
			public void OnCreateState(GameState gameState)
			{
				LobbyState lobbyState;
				if ((lobbyState = gameState as LobbyState) != null)
				{
					lobbyState.RegisterListener(new DedicatedCustomServerClientHelperSubModule.LobbyStateListener(lobbyState));
				}
			}

			public void OnPopState(GameState gameState)
			{
			}

			public void OnPushState(GameState gameState, bool isTopGameState)
			{
			}

			public void OnCleanStates()
			{
			}

			public void OnSavedGameLoadFinished()
			{
			}
		}
	}
}
