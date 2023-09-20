using System;
using System.Threading.Tasks;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.Diamond;
using TaleWorlds.PlatformService;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x0200022C RID: 556
	public sealed class LobbyGameStatePlayerBasedCustomServer : LobbyGameState
	{
		// Token: 0x06001E58 RID: 7768 RVA: 0x0006D075 File Offset: 0x0006B275
		public void SetStartingParameters(LobbyGameClientHandler lobbyGameClientHandler)
		{
			this._gameClient = lobbyGameClientHandler.GameClient;
		}

		// Token: 0x06001E59 RID: 7769 RVA: 0x0006D083 File Offset: 0x0006B283
		protected override void OnActivate()
		{
			base.OnActivate();
			if (this._gameClient != null && (this._gameClient.AtLobby || !this._gameClient.Connected))
			{
				base.GameStateManager.PopState(0);
			}
		}

		// Token: 0x06001E5A RID: 7770 RVA: 0x0006D0B9 File Offset: 0x0006B2B9
		protected override void StartMultiplayer()
		{
			this.HandleServerStartMultiplayer();
		}

		// Token: 0x06001E5B RID: 7771 RVA: 0x0006D0C4 File Offset: 0x0006B2C4
		private async void HandleServerStartMultiplayer()
		{
			GameNetwork.PreStartMultiplayerOnServer();
			BannerlordNetwork.StartMultiplayerLobbyMission(LobbyMissionType.Custom);
			if (!Module.CurrentModule.StartMultiplayerGame(this._gameClient.CustomGameType, this._gameClient.CustomGameScene))
			{
				Debug.FailedAssert("[DEBUG]Invalid multiplayer game type.", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\GameState\\LobbyGameState.cs", "HandleServerStartMultiplayer", 284);
			}
			while (Mission.Current == null || Mission.Current.CurrentState != Mission.State.Continuing)
			{
				await Task.Delay(1);
			}
			GameNetwork.StartMultiplayerOnServer(9999);
			if (this._gameClient.IsInGame)
			{
				BannerlordNetwork.CreateServerPeer();
				MBDebug.Print("Server: I finished loading and I am now visible to clients in the server list.", 0, Debug.DebugColor.White, 17179869184UL);
				if (!GameNetwork.IsDedicatedServer)
				{
					GameNetwork.ClientFinishedLoading(GameNetwork.MyPeer);
				}
			}
			IPlatformServices instance = PlatformServices.Instance;
			if (instance != null)
			{
				instance.CheckPrivilege(Privilege.Chat, false, delegate(bool result)
				{
					if (!result)
					{
						PlatformServices.Instance.ShowRestrictedInformation();
					}
				});
			}
		}

		// Token: 0x04000B38 RID: 2872
		private LobbyClient _gameClient;
	}
}
