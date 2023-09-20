using System;
using TaleWorlds.Core;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000229 RID: 553
	public abstract class LobbyGameState : GameState, IUdpNetworkHandler
	{
		// Token: 0x1700060C RID: 1548
		// (get) Token: 0x06001E3B RID: 7739 RVA: 0x0006CD8C File Offset: 0x0006AF8C
		public override bool IsMusicMenuState
		{
			get
			{
				return true;
			}
		}

		// Token: 0x06001E3D RID: 7741 RVA: 0x0006CD97 File Offset: 0x0006AF97
		protected override void OnInitialize()
		{
			base.OnInitialize();
			this.StartMultiplayer();
			GameNetwork.AddNetworkHandler(this);
		}

		// Token: 0x06001E3E RID: 7742 RVA: 0x0006CDAB File Offset: 0x0006AFAB
		protected override void OnActivate()
		{
			base.OnActivate();
		}

		// Token: 0x06001E3F RID: 7743 RVA: 0x0006CDB3 File Offset: 0x0006AFB3
		protected override void OnFinalize()
		{
			base.OnFinalize();
			GameNetwork.RemoveNetworkHandler(this);
			GameNetwork.EndMultiplayer();
		}

		// Token: 0x06001E40 RID: 7744 RVA: 0x0006CDC6 File Offset: 0x0006AFC6
		void IUdpNetworkHandler.OnUdpNetworkHandlerClose()
		{
		}

		// Token: 0x06001E41 RID: 7745 RVA: 0x0006CDC8 File Offset: 0x0006AFC8
		void IUdpNetworkHandler.OnUdpNetworkHandlerTick(float dt)
		{
		}

		// Token: 0x06001E42 RID: 7746 RVA: 0x0006CDCA File Offset: 0x0006AFCA
		void IUdpNetworkHandler.HandleNewClientConnect(PlayerConnectionInfo clientConnectionInfo)
		{
		}

		// Token: 0x06001E43 RID: 7747 RVA: 0x0006CDCC File Offset: 0x0006AFCC
		void IUdpNetworkHandler.HandleEarlyNewClientAfterLoadingFinished(NetworkCommunicator networkPeer)
		{
		}

		// Token: 0x06001E44 RID: 7748 RVA: 0x0006CDCE File Offset: 0x0006AFCE
		void IUdpNetworkHandler.HandleNewClientAfterLoadingFinished(NetworkCommunicator networkPeer)
		{
		}

		// Token: 0x06001E45 RID: 7749 RVA: 0x0006CDD0 File Offset: 0x0006AFD0
		void IUdpNetworkHandler.HandleLateNewClientAfterLoadingFinished(NetworkCommunicator networkPeer)
		{
		}

		// Token: 0x06001E46 RID: 7750 RVA: 0x0006CDD2 File Offset: 0x0006AFD2
		void IUdpNetworkHandler.HandleNewClientAfterSynchronized(NetworkCommunicator networkPeer)
		{
		}

		// Token: 0x06001E47 RID: 7751 RVA: 0x0006CDD4 File Offset: 0x0006AFD4
		void IUdpNetworkHandler.HandleLateNewClientAfterSynchronized(NetworkCommunicator networkPeer)
		{
		}

		// Token: 0x06001E48 RID: 7752 RVA: 0x0006CDD6 File Offset: 0x0006AFD6
		void IUdpNetworkHandler.HandleEarlyPlayerDisconnect(NetworkCommunicator networkPeer)
		{
		}

		// Token: 0x06001E49 RID: 7753 RVA: 0x0006CDD8 File Offset: 0x0006AFD8
		void IUdpNetworkHandler.HandlePlayerDisconnect(NetworkCommunicator networkPeer)
		{
		}

		// Token: 0x06001E4A RID: 7754 RVA: 0x0006CDDA File Offset: 0x0006AFDA
		void IUdpNetworkHandler.OnEveryoneUnSynchronized()
		{
		}

		// Token: 0x06001E4B RID: 7755 RVA: 0x0006CDDC File Offset: 0x0006AFDC
		void IUdpNetworkHandler.OnPlayerDisconnectedFromServer(NetworkCommunicator networkPeer)
		{
		}

		// Token: 0x06001E4C RID: 7756 RVA: 0x0006CDDE File Offset: 0x0006AFDE
		void IUdpNetworkHandler.OnDisconnectedFromServer()
		{
		}

		// Token: 0x06001E4D RID: 7757
		protected abstract void StartMultiplayer();
	}
}
