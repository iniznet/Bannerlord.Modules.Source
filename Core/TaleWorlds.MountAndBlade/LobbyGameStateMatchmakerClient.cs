using System;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.Diamond;
using TaleWorlds.PlatformService;

namespace TaleWorlds.MountAndBlade
{
	public sealed class LobbyGameStateMatchmakerClient : LobbyGameState
	{
		public void SetStartingParameters(LobbyGameClientHandler lobbyGameClientHandler, int playerIndex, int sessionKey, string address, int assignedPort, string multiplayerGameType, string scene)
		{
			this._lobbyGameClientHandler = lobbyGameClientHandler;
			this._gameClient = lobbyGameClientHandler.GameClient;
			this._playerIndex = playerIndex;
			this._sessionKey = sessionKey;
			this._address = address;
			this._assignedPort = assignedPort;
			this._multiplayerGameType = multiplayerGameType;
			this._scene = scene;
		}

		protected override void OnActivate()
		{
			base.OnActivate();
			if (this._gameClient != null && (this._gameClient.CurrentState == LobbyClient.State.AtLobby || this._gameClient.CurrentState == LobbyClient.State.QuittingFromBattle || !this._gameClient.Connected))
			{
				base.GameStateManager.PopState(0);
			}
		}

		protected override void StartMultiplayer()
		{
			GameNetwork.StartMultiplayerOnClient(this._address, this._assignedPort, this._sessionKey, this._playerIndex);
			BannerlordNetwork.StartMultiplayerLobbyMission(LobbyMissionType.Matchmaker);
			if (!Module.CurrentModule.StartMultiplayerGame(this._multiplayerGameType, this._scene))
			{
				Debug.FailedAssert("[DEBUG]Invalid multiplayer game type.", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\GameState\\LobbyGameState.cs", "StartMultiplayer", 239);
			}
			IPlatformServices instance = PlatformServices.Instance;
			if (instance == null)
			{
				return;
			}
			instance.CheckPrivilege(Privilege.Chat, true, delegate(bool result)
			{
				if (!result)
				{
					PlatformServices.Instance.ShowRestrictedInformation();
				}
			});
		}

		private LobbyClient _gameClient;

		private int _playerIndex;

		private int _sessionKey;

		private string _address;

		private int _assignedPort;

		private string _multiplayerGameType;

		private string _scene;

		private LobbyGameClientHandler _lobbyGameClientHandler;
	}
}
