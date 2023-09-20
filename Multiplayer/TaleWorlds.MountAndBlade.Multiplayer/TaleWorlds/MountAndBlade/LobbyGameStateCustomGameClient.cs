using System;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.MountAndBlade.Diamond;
using TaleWorlds.PlatformService;

namespace TaleWorlds.MountAndBlade
{
	public sealed class LobbyGameStateCustomGameClient : LobbyGameState
	{
		public void SetStartingParameters(LobbyClient gameClient, string address, int port, int peerIndex, int sessionKey)
		{
			this._gameClient = gameClient;
			this._address = address;
			this._port = port;
			this._peerIndex = peerIndex;
			this._sessionKey = sessionKey;
		}

		protected override void OnActivate()
		{
			base.OnActivate();
			this._inactivityTimer = new Timer(MBCommon.GetApplicationTime(), LobbyGameStateCustomGameClient.InactivityThreshold, true);
			if (this._gameClient != null && (this._gameClient.AtLobby || !this._gameClient.Connected))
			{
				base.GameStateManager.PopState(0);
			}
		}

		protected override void OnTick(float dt)
		{
			base.OnTick(dt);
			if (GameNetwork.IsClient && this._inactivityTimer.Check(MBCommon.GetApplicationTime()) && this._gameClient != null)
			{
				this._gameClient.IsInCriticalState = GameNetwork.ElapsedTimeSinceLastUdpPacketArrived() > (double)LobbyGameStateCustomGameClient.InactivityThreshold;
			}
		}

		protected override void StartMultiplayer()
		{
			MBDebug.Print("CUSTOM GAME SERVER ADDRESS: " + this._address, 0, 12, 17592186044416UL);
			GameNetwork.StartMultiplayerOnClient(this._address, this._port, this._sessionKey, this._peerIndex);
			BannerlordNetwork.StartMultiplayerLobbyMission(1);
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

		private LobbyClient _gameClient;

		private string _address;

		private int _port;

		private int _peerIndex;

		private int _sessionKey;

		private Timer _inactivityTimer;

		private static readonly float InactivityThreshold = 2f;
	}
}
