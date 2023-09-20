using System;
using TaleWorlds.MountAndBlade.Diamond;

namespace TaleWorlds.MountAndBlade
{
	public class MissionCustomGameClientComponent : MissionLobbyComponent
	{
		public override void OnBehaviorInitialize()
		{
			base.OnBehaviorInitialize();
			this._lobbyClient = NetworkMain.GameClient;
		}

		public void SetServerEndingBeforeClientLoaded(bool isServerEndingBeforeClientLoaded)
		{
			this._isServerEndedBeforeClientLoaded = isServerEndingBeforeClientLoaded;
		}

		public override void QuitMission()
		{
			base.QuitMission();
			if (GameNetwork.IsServer)
			{
				if (base.CurrentMultiplayerState != MissionLobbyComponent.MultiplayerGameState.Ending && this._lobbyClient.LoggedIn && this._lobbyClient.CurrentState == LobbyClient.State.HostingCustomGame)
				{
					this._lobbyClient.EndCustomGame();
					return;
				}
			}
			else if (!this._isServerEndedBeforeClientLoaded && base.CurrentMultiplayerState != MissionLobbyComponent.MultiplayerGameState.Ending && this._lobbyClient.LoggedIn && this._lobbyClient.CurrentState == LobbyClient.State.InCustomGame)
			{
				this._lobbyClient.QuitFromCustomGame();
			}
		}

		private LobbyClient _lobbyClient;

		private bool _isServerEndedBeforeClientLoaded;
	}
}
