using System;
using System.Threading.Tasks;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.Diamond;
using TaleWorlds.PlatformService;

namespace TaleWorlds.MountAndBlade
{
	public sealed class LobbyGameStatePlayerBasedCustomServer : LobbyGameState
	{
		public void SetStartingParameters(LobbyGameClientHandler lobbyGameClientHandler)
		{
			this._gameClient = lobbyGameClientHandler.GameClient;
		}

		protected override void OnActivate()
		{
			base.OnActivate();
			if (this._gameClient != null && (this._gameClient.AtLobby || !this._gameClient.Connected))
			{
				base.GameStateManager.PopState(0);
			}
		}

		protected override void StartMultiplayer()
		{
			this.HandleServerStartMultiplayer();
		}

		private async void HandleServerStartMultiplayer()
		{
			GameNetwork.PreStartMultiplayerOnServer();
			BannerlordNetwork.StartMultiplayerLobbyMission(1);
			if (!Module.CurrentModule.StartMultiplayerGame(this._gameClient.CustomGameType, this._gameClient.CustomGameScene))
			{
				Debug.FailedAssert("[DEBUG]Invalid multiplayer game type.", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade.Multiplayer\\LobbyGameState.cs", "HandleServerStartMultiplayer", 346);
			}
			while (Mission.Current == null || Mission.Current.CurrentState != 2)
			{
				await Task.Delay(1);
			}
			GameNetwork.StartMultiplayerOnServer(9999);
			if (this._gameClient.IsInGame)
			{
				BannerlordNetwork.CreateServerPeer();
				MBDebug.Print("Server: I finished loading and I am now visible to clients in the server list.", 0, 12, 17179869184UL);
				if (!GameNetwork.IsDedicatedServer)
				{
					GameNetwork.ClientFinishedLoading(GameNetwork.MyPeer);
				}
			}
			IPlatformServices instance = PlatformServices.Instance;
			if (instance != null)
			{
				instance.CheckPrivilege(1, false, delegate(bool result)
				{
					if (!result)
					{
						PlatformServices.Instance.ShowRestrictedInformation();
					}
				});
			}
		}

		private LobbyClient _gameClient;
	}
}
