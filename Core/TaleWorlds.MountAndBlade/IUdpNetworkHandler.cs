using System;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x0200032F RID: 815
	public interface IUdpNetworkHandler
	{
		// Token: 0x06002C07 RID: 11271
		void OnUdpNetworkHandlerClose();

		// Token: 0x06002C08 RID: 11272
		void OnUdpNetworkHandlerTick(float dt);

		// Token: 0x06002C09 RID: 11273
		void HandleNewClientConnect(PlayerConnectionInfo clientConnectionInfo);

		// Token: 0x06002C0A RID: 11274
		void HandleEarlyNewClientAfterLoadingFinished(NetworkCommunicator networkPeer);

		// Token: 0x06002C0B RID: 11275
		void HandleNewClientAfterLoadingFinished(NetworkCommunicator networkPeer);

		// Token: 0x06002C0C RID: 11276
		void HandleLateNewClientAfterLoadingFinished(NetworkCommunicator networkPeer);

		// Token: 0x06002C0D RID: 11277
		void HandleNewClientAfterSynchronized(NetworkCommunicator networkPeer);

		// Token: 0x06002C0E RID: 11278
		void HandleLateNewClientAfterSynchronized(NetworkCommunicator networkPeer);

		// Token: 0x06002C0F RID: 11279
		void HandleEarlyPlayerDisconnect(NetworkCommunicator networkPeer);

		// Token: 0x06002C10 RID: 11280
		void HandlePlayerDisconnect(NetworkCommunicator networkPeer);

		// Token: 0x06002C11 RID: 11281
		void OnPlayerDisconnectedFromServer(NetworkCommunicator networkPeer);

		// Token: 0x06002C12 RID: 11282
		void OnDisconnectedFromServer();

		// Token: 0x06002C13 RID: 11283
		void OnEveryoneUnSynchronized();
	}
}
