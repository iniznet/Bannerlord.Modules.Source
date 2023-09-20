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
	// Token: 0x02000004 RID: 4
	public class DedicatedCustomServerClientHelperSubModule : MBSubModuleBase
	{
		// Token: 0x06000042 RID: 66 RVA: 0x00002B31 File Offset: 0x00000D31
		public DedicatedCustomServerClientHelperSubModule()
		{
			this._httpClient = new HttpClient();
		}

		// Token: 0x06000043 RID: 67 RVA: 0x00002B44 File Offset: 0x00000D44
		protected override void OnSubModuleLoad()
		{
			DedicatedCustomServerClientHelperSubModule.Instance = this;
			base.OnSubModuleLoad();
			ModLogger.Log("Loaded", 0, 4);
		}

		// Token: 0x06000044 RID: 68 RVA: 0x00002B5E File Offset: 0x00000D5E
		public override void OnMultiplayerGameStart(Game game, object _)
		{
			game.GameStateManager.RegisterListener(new DedicatedCustomServerClientHelperSubModule.StateManagerListener());
		}

		// Token: 0x06000045 RID: 69 RVA: 0x00002B74 File Offset: 0x00000D74
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

		// Token: 0x06000046 RID: 70 RVA: 0x00002BE4 File Offset: 0x00000DE4
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

		// Token: 0x06000047 RID: 71 RVA: 0x00002C2C File Offset: 0x00000E2C
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

		// Token: 0x06000048 RID: 72 RVA: 0x00002C8C File Offset: 0x00000E8C
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

		// Token: 0x06000049 RID: 73 RVA: 0x00002CD4 File Offset: 0x00000ED4
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

		// Token: 0x0400001C RID: 28
		public const string ModuleName = "DedicatedCustomServerHelper";

		// Token: 0x0400001D RID: 29
		public static readonly bool DebugMode;

		// Token: 0x0400001E RID: 30
		public static DedicatedCustomServerClientHelperSubModule Instance;

		// Token: 0x0400001F RID: 31
		private readonly HttpClient _httpClient;

		// Token: 0x04000020 RID: 32
		private const string CommandGroup = "dcshelper";

		// Token: 0x04000021 RID: 33
		private const string DownloadMapCommandName = "download_map";

		// Token: 0x04000022 RID: 34
		private const string GetMapListCommandName = "get_map_list";

		// Token: 0x04000023 RID: 35
		private const string OpenDownloadPanelCommandName = "open_download_panel";

		// Token: 0x02000012 RID: 18
		private class LobbyStateListener : IGameStateListener
		{
			// Token: 0x0600009E RID: 158 RVA: 0x00003AF2 File Offset: 0x00001CF2
			public LobbyStateListener(LobbyState lobbyState)
			{
				this._lobbyState = lobbyState;
			}

			// Token: 0x0600009F RID: 159 RVA: 0x00003B01 File Offset: 0x00001D01
			private bool ServerSupportsDownloadPanel(GameServerEntry serverEntry)
			{
				return serverEntry.LoadedModules.Any((ModuleInfoModel m) => m.Id == "DedicatedCustomServerHelper");
			}

			// Token: 0x060000A0 RID: 160 RVA: 0x00003B2D File Offset: 0x00001D2D
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

			// Token: 0x060000A1 RID: 161 RVA: 0x00003B4C File Offset: 0x00001D4C
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

			// Token: 0x060000A2 RID: 162 RVA: 0x00003BAF File Offset: 0x00001DAF
			private void HandleFailedServerJoinAttempt(GameServerEntry serverEntry)
			{
				if (this.ServerSupportsDownloadPanel(serverEntry))
				{
					this.OpenDownloadPanelForServer(serverEntry);
				}
			}

			// Token: 0x060000A3 RID: 163 RVA: 0x00003BC1 File Offset: 0x00001DC1
			public void OnActivate()
			{
				this._lobbyState.RegisterForCustomServerAction(new Func<GameServerEntry, List<CustomServerAction>>(this.ActionSupplier));
				this._lobbyState.ClientRefusedToJoinCustomServer += this.HandleFailedServerJoinAttempt;
			}

			// Token: 0x060000A4 RID: 164 RVA: 0x00003BF1 File Offset: 0x00001DF1
			public void OnDeactivate()
			{
				this._lobbyState.UnregisterForCustomServerAction(new Func<GameServerEntry, List<CustomServerAction>>(this.ActionSupplier));
				this._lobbyState.ClientRefusedToJoinCustomServer -= this.HandleFailedServerJoinAttempt;
			}

			// Token: 0x060000A5 RID: 165 RVA: 0x00003C21 File Offset: 0x00001E21
			public void OnFinalize()
			{
				this._lobbyState = null;
			}

			// Token: 0x060000A6 RID: 166 RVA: 0x00003C2A File Offset: 0x00001E2A
			public void OnInitialize()
			{
			}

			// Token: 0x04000052 RID: 82
			private LobbyState _lobbyState;
		}

		// Token: 0x02000013 RID: 19
		private class StateManagerListener : IGameStateManagerListener
		{
			// Token: 0x060000A7 RID: 167 RVA: 0x00003C2C File Offset: 0x00001E2C
			public void OnCreateState(GameState gameState)
			{
				LobbyState lobbyState;
				if ((lobbyState = gameState as LobbyState) != null)
				{
					lobbyState.RegisterListener(new DedicatedCustomServerClientHelperSubModule.LobbyStateListener(lobbyState));
				}
			}

			// Token: 0x060000A8 RID: 168 RVA: 0x00003C50 File Offset: 0x00001E50
			public void OnPopState(GameState gameState)
			{
			}

			// Token: 0x060000A9 RID: 169 RVA: 0x00003C52 File Offset: 0x00001E52
			public void OnPushState(GameState gameState, bool isTopGameState)
			{
			}

			// Token: 0x060000AA RID: 170 RVA: 0x00003C54 File Offset: 0x00001E54
			public void OnCleanStates()
			{
			}

			// Token: 0x060000AB RID: 171 RVA: 0x00003C56 File Offset: 0x00001E56
			public void OnSavedGameLoadFinished()
			{
			}
		}
	}
}
