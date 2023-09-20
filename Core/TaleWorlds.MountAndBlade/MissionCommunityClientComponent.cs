using System;

namespace TaleWorlds.MountAndBlade
{
	public class MissionCommunityClientComponent : MissionLobbyComponent
	{
		public override void OnBehaviorInitialize()
		{
			base.OnBehaviorInitialize();
			this._communityClient = NetworkMain.CommunityClient;
		}

		public void SetServerEndingBeforeClientLoaded(bool isServerEndingBeforeClientLoaded)
		{
			this._isServerEndedBeforeClientLoaded = isServerEndingBeforeClientLoaded;
		}

		public override void QuitMission()
		{
			base.QuitMission();
			if (!this._isServerEndedBeforeClientLoaded && base.CurrentMultiplayerState != MissionLobbyComponent.MultiplayerGameState.Ending && this._communityClient.IsInGame)
			{
				this._communityClient.QuitFromGame();
			}
		}

		private CommunityClient _communityClient;

		private bool _isServerEndedBeforeClientLoaded;
	}
}
