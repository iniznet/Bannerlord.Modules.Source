using System;
using TaleWorlds.MountAndBlade.Diamond;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x0200028D RID: 653
	public class MissionCustomGameClientComponent : MissionLobbyComponent
	{
		// Token: 0x06002299 RID: 8857 RVA: 0x0007DC95 File Offset: 0x0007BE95
		public override void OnBehaviorInitialize()
		{
			base.OnBehaviorInitialize();
			this._lobbyClient = NetworkMain.GameClient;
		}

		// Token: 0x0600229A RID: 8858 RVA: 0x0007DCA8 File Offset: 0x0007BEA8
		public void SetServerEndingBeforeClientLoaded(bool isServerEndingBeforeClientLoaded)
		{
			this._isServerEndedBeforeClientLoaded = isServerEndingBeforeClientLoaded;
		}

		// Token: 0x0600229B RID: 8859 RVA: 0x0007DCB4 File Offset: 0x0007BEB4
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

		// Token: 0x04000CE5 RID: 3301
		private LobbyClient _lobbyClient;

		// Token: 0x04000CE6 RID: 3302
		private bool _isServerEndedBeforeClientLoaded;
	}
}
