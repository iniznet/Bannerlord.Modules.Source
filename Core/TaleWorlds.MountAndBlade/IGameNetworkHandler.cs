using System;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x020002ED RID: 749
	public interface IGameNetworkHandler
	{
		// Token: 0x060028DD RID: 10461
		void OnNewPlayerConnect(PlayerConnectionInfo playerConnectionInfo, NetworkCommunicator networkPeer);

		// Token: 0x060028DE RID: 10462
		void OnInitialize();

		// Token: 0x060028DF RID: 10463
		void OnPlayerConnectedToServer(NetworkCommunicator peer);

		// Token: 0x060028E0 RID: 10464
		void OnPlayerDisconnectedFromServer(NetworkCommunicator peer);

		// Token: 0x060028E1 RID: 10465
		void OnDisconnectedFromServer();

		// Token: 0x060028E2 RID: 10466
		void OnStartMultiplayer();

		// Token: 0x060028E3 RID: 10467
		void OnStartReplay();

		// Token: 0x060028E4 RID: 10468
		void OnEndMultiplayer();

		// Token: 0x060028E5 RID: 10469
		void OnEndReplay();

		// Token: 0x060028E6 RID: 10470
		void OnHandleConsoleCommand(string command);
	}
}
