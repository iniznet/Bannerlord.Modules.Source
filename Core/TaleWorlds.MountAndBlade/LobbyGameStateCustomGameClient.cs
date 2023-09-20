using System;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.Diamond;
using TaleWorlds.PlatformService;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x0200022A RID: 554
	public sealed class LobbyGameStateCustomGameClient : LobbyGameState
	{
		// Token: 0x06001E4E RID: 7758 RVA: 0x0006CDE0 File Offset: 0x0006AFE0
		public void SetStartingParameters(LobbyClient gameClient, string address, int port, int peerIndex, int sessionKey)
		{
			this._gameClient = gameClient;
			this._address = address;
			this._port = port;
			this._peerIndex = peerIndex;
			this._sessionKey = sessionKey;
		}

		// Token: 0x06001E4F RID: 7759 RVA: 0x0006CE08 File Offset: 0x0006B008
		protected override void OnActivate()
		{
			base.OnActivate();
			this._inactivityTimer = new Timer(MBCommon.GetApplicationTime(), LobbyGameStateCustomGameClient.InactivityThreshold, true);
			if (this._gameClient != null && (this._gameClient.AtLobby || !this._gameClient.Connected))
			{
				base.GameStateManager.PopState(0);
			}
		}

		// Token: 0x06001E50 RID: 7760 RVA: 0x0006CE5F File Offset: 0x0006B05F
		protected override void OnTick(float dt)
		{
			base.OnTick(dt);
			if (GameNetwork.IsClient && this._inactivityTimer.Check(MBCommon.GetApplicationTime()))
			{
				this._gameClient.IsInCriticalState = MBAPI.IMBNetwork.ElapsedTimeSinceLastUdpPacketArrived() > (double)LobbyGameStateCustomGameClient.InactivityThreshold;
			}
		}

		// Token: 0x06001E51 RID: 7761 RVA: 0x0006CEA0 File Offset: 0x0006B0A0
		protected override void StartMultiplayer()
		{
			MBDebug.Print("CUSTOM GAME SERVER ADDRESS: " + this._address, 0, Debug.DebugColor.White, 17592186044416UL);
			GameNetwork.StartMultiplayerOnClient(this._address, this._port, this._sessionKey, this._peerIndex);
			BannerlordNetwork.StartMultiplayerLobbyMission(LobbyMissionType.Custom);
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

		// Token: 0x04000B29 RID: 2857
		private LobbyClient _gameClient;

		// Token: 0x04000B2A RID: 2858
		private string _address;

		// Token: 0x04000B2B RID: 2859
		private int _port;

		// Token: 0x04000B2C RID: 2860
		private int _peerIndex;

		// Token: 0x04000B2D RID: 2861
		private int _sessionKey;

		// Token: 0x04000B2E RID: 2862
		private Timer _inactivityTimer;

		// Token: 0x04000B2F RID: 2863
		private static readonly float InactivityThreshold = 2f;
	}
}
