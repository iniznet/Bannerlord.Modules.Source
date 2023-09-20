using System;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.PlatformService;

namespace TaleWorlds.MountAndBlade
{
	public sealed class LobbyGameStateCommunityClient : LobbyGameState
	{
		public void SetStartingParameters(CommunityClient communityClient, string address, int port, int peerIndex, int sessionKey)
		{
			this._communityClient = communityClient;
			this._address = address;
			this._port = port;
			this._peerIndex = peerIndex;
			this._sessionKey = sessionKey;
		}

		protected override void OnActivate()
		{
			base.OnActivate();
			if (this._communityClient != null && !this._communityClient.IsInGame)
			{
				base.GameStateManager.PopState(0);
			}
		}

		protected override void StartMultiplayer()
		{
			MBDebug.Print("COMMUNITY GAME SERVER ADDRESS: " + this._address, 0, 12, 17592186044416UL);
			GameNetwork.StartMultiplayerOnClient(this._address, this._port, this._sessionKey, this._peerIndex);
			BannerlordNetwork.StartMultiplayerLobbyMission(2);
			IPlatformServices instance = PlatformServices.Instance;
			if (instance == null)
			{
				return;
			}
			instance.CheckPrivilege(1, true, delegate(bool result)
			{
				if (!result)
				{
					PlatformServices.Instance.ShowRestrictedInformation();
				}
			});
		}

		protected override void OnDisconnectedFromServer()
		{
			base.OnDisconnectedFromServer();
			if (Game.Current.GameStateManager.ActiveState == this)
			{
				base.GameStateManager.PopState(0);
			}
		}

		private CommunityClient _communityClient;

		private string _address;

		private int _port;

		private int _peerIndex;

		private int _sessionKey;
	}
}
