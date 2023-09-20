using System;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.Diamond;
using TaleWorlds.PlatformService;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x0200022B RID: 555
	public sealed class LobbyGameStateMatchmakerClient : LobbyGameState
	{
		// Token: 0x06001E54 RID: 7764 RVA: 0x0006CF38 File Offset: 0x0006B138
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

		// Token: 0x06001E55 RID: 7765 RVA: 0x0006CF88 File Offset: 0x0006B188
		protected override void OnActivate()
		{
			base.OnActivate();
			if (this._gameClient != null && (this._gameClient.CurrentState == LobbyClient.State.AtLobby || this._gameClient.CurrentState == LobbyClient.State.QuittingFromBattle || !this._gameClient.Connected))
			{
				base.GameStateManager.PopState(0);
			}
		}

		// Token: 0x06001E56 RID: 7766 RVA: 0x0006CFDC File Offset: 0x0006B1DC
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

		// Token: 0x04000B30 RID: 2864
		private LobbyClient _gameClient;

		// Token: 0x04000B31 RID: 2865
		private int _playerIndex;

		// Token: 0x04000B32 RID: 2866
		private int _sessionKey;

		// Token: 0x04000B33 RID: 2867
		private string _address;

		// Token: 0x04000B34 RID: 2868
		private int _assignedPort;

		// Token: 0x04000B35 RID: 2869
		private string _multiplayerGameType;

		// Token: 0x04000B36 RID: 2870
		private string _scene;

		// Token: 0x04000B37 RID: 2871
		private LobbyGameClientHandler _lobbyGameClientHandler;
	}
}
