using System;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000283 RID: 643
	public abstract class MissionNetwork : MissionLogic, IUdpNetworkHandler
	{
		// Token: 0x06002212 RID: 8722 RVA: 0x0007CEF0 File Offset: 0x0007B0F0
		public override void OnAfterMissionCreated()
		{
			this._missionNetworkMessageHandlerRegisterer = new GameNetwork.NetworkMessageHandlerRegistererContainer();
			this.AddRemoveMessageHandlers(this._missionNetworkMessageHandlerRegisterer);
			this._missionNetworkMessageHandlerRegisterer.RegisterMessages();
		}

		// Token: 0x06002213 RID: 8723 RVA: 0x0007CF14 File Offset: 0x0007B114
		public override void OnBehaviorInitialize()
		{
			base.OnBehaviorInitialize();
			GameNetwork.AddNetworkHandler(this);
		}

		// Token: 0x06002214 RID: 8724 RVA: 0x0007CF22 File Offset: 0x0007B122
		public override void OnRemoveBehavior()
		{
			GameNetwork.RemoveNetworkHandler(this);
			base.OnRemoveBehavior();
		}

		// Token: 0x06002215 RID: 8725 RVA: 0x0007CF30 File Offset: 0x0007B130
		protected virtual void AddRemoveMessageHandlers(GameNetwork.NetworkMessageHandlerRegistererContainer registerer)
		{
		}

		// Token: 0x06002216 RID: 8726 RVA: 0x0007CF32 File Offset: 0x0007B132
		public virtual void OnPlayerConnectedToServer(NetworkCommunicator networkPeer)
		{
		}

		// Token: 0x06002217 RID: 8727 RVA: 0x0007CF34 File Offset: 0x0007B134
		public virtual void OnPlayerDisconnectedFromServer(NetworkCommunicator networkPeer)
		{
		}

		// Token: 0x06002218 RID: 8728 RVA: 0x0007CF36 File Offset: 0x0007B136
		void IUdpNetworkHandler.OnUdpNetworkHandlerTick(float dt)
		{
			this.OnUdpNetworkHandlerTick();
		}

		// Token: 0x06002219 RID: 8729 RVA: 0x0007CF3E File Offset: 0x0007B13E
		void IUdpNetworkHandler.OnUdpNetworkHandlerClose()
		{
			this.OnUdpNetworkHandlerClose();
			GameNetwork.NetworkMessageHandlerRegistererContainer missionNetworkMessageHandlerRegisterer = this._missionNetworkMessageHandlerRegisterer;
			if (missionNetworkMessageHandlerRegisterer == null)
			{
				return;
			}
			missionNetworkMessageHandlerRegisterer.UnregisterMessages();
		}

		// Token: 0x0600221A RID: 8730 RVA: 0x0007CF56 File Offset: 0x0007B156
		void IUdpNetworkHandler.HandleNewClientConnect(PlayerConnectionInfo clientConnectionInfo)
		{
			this.HandleNewClientConnect(clientConnectionInfo);
		}

		// Token: 0x0600221B RID: 8731 RVA: 0x0007CF5F File Offset: 0x0007B15F
		void IUdpNetworkHandler.HandleEarlyNewClientAfterLoadingFinished(NetworkCommunicator networkPeer)
		{
			this.HandleEarlyNewClientAfterLoadingFinished(networkPeer);
		}

		// Token: 0x0600221C RID: 8732 RVA: 0x0007CF68 File Offset: 0x0007B168
		void IUdpNetworkHandler.HandleNewClientAfterLoadingFinished(NetworkCommunicator networkPeer)
		{
			this.HandleNewClientAfterLoadingFinished(networkPeer);
		}

		// Token: 0x0600221D RID: 8733 RVA: 0x0007CF71 File Offset: 0x0007B171
		void IUdpNetworkHandler.HandleLateNewClientAfterLoadingFinished(NetworkCommunicator networkPeer)
		{
			this.HandleLateNewClientAfterLoadingFinished(networkPeer);
		}

		// Token: 0x0600221E RID: 8734 RVA: 0x0007CF7A File Offset: 0x0007B17A
		void IUdpNetworkHandler.HandleNewClientAfterSynchronized(NetworkCommunicator networkPeer)
		{
			this.HandleNewClientAfterSynchronized(networkPeer);
		}

		// Token: 0x0600221F RID: 8735 RVA: 0x0007CF83 File Offset: 0x0007B183
		void IUdpNetworkHandler.HandleLateNewClientAfterSynchronized(NetworkCommunicator networkPeer)
		{
			this.HandleLateNewClientAfterSynchronized(networkPeer);
		}

		// Token: 0x06002220 RID: 8736 RVA: 0x0007CF8C File Offset: 0x0007B18C
		void IUdpNetworkHandler.HandleEarlyPlayerDisconnect(NetworkCommunicator networkPeer)
		{
			this.HandleEarlyPlayerDisconnect(networkPeer);
		}

		// Token: 0x06002221 RID: 8737 RVA: 0x0007CF95 File Offset: 0x0007B195
		void IUdpNetworkHandler.HandlePlayerDisconnect(NetworkCommunicator networkPeer)
		{
			this.HandlePlayerDisconnect(networkPeer);
		}

		// Token: 0x06002222 RID: 8738 RVA: 0x0007CF9E File Offset: 0x0007B19E
		void IUdpNetworkHandler.OnEveryoneUnSynchronized()
		{
		}

		// Token: 0x06002223 RID: 8739 RVA: 0x0007CFA0 File Offset: 0x0007B1A0
		void IUdpNetworkHandler.OnPlayerDisconnectedFromServer(NetworkCommunicator networkPeer)
		{
		}

		// Token: 0x06002224 RID: 8740 RVA: 0x0007CFA2 File Offset: 0x0007B1A2
		void IUdpNetworkHandler.OnDisconnectedFromServer()
		{
		}

		// Token: 0x06002225 RID: 8741 RVA: 0x0007CFA4 File Offset: 0x0007B1A4
		protected virtual void OnUdpNetworkHandlerTick()
		{
		}

		// Token: 0x06002226 RID: 8742 RVA: 0x0007CFA6 File Offset: 0x0007B1A6
		protected virtual void OnUdpNetworkHandlerClose()
		{
		}

		// Token: 0x06002227 RID: 8743 RVA: 0x0007CFA8 File Offset: 0x0007B1A8
		protected virtual void HandleNewClientConnect(PlayerConnectionInfo clientConnectionInfo)
		{
		}

		// Token: 0x06002228 RID: 8744 RVA: 0x0007CFAA File Offset: 0x0007B1AA
		protected virtual void HandleEarlyNewClientAfterLoadingFinished(NetworkCommunicator networkPeer)
		{
		}

		// Token: 0x06002229 RID: 8745 RVA: 0x0007CFAC File Offset: 0x0007B1AC
		protected virtual void HandleNewClientAfterLoadingFinished(NetworkCommunicator networkPeer)
		{
		}

		// Token: 0x0600222A RID: 8746 RVA: 0x0007CFAE File Offset: 0x0007B1AE
		protected virtual void HandleLateNewClientAfterLoadingFinished(NetworkCommunicator networkPeer)
		{
		}

		// Token: 0x0600222B RID: 8747 RVA: 0x0007CFB0 File Offset: 0x0007B1B0
		protected virtual void HandleNewClientAfterSynchronized(NetworkCommunicator networkPeer)
		{
		}

		// Token: 0x0600222C RID: 8748 RVA: 0x0007CFB2 File Offset: 0x0007B1B2
		protected virtual void HandleLateNewClientAfterSynchronized(NetworkCommunicator networkPeer)
		{
		}

		// Token: 0x0600222D RID: 8749 RVA: 0x0007CFB4 File Offset: 0x0007B1B4
		protected virtual void HandleEarlyPlayerDisconnect(NetworkCommunicator networkPeer)
		{
		}

		// Token: 0x0600222E RID: 8750 RVA: 0x0007CFB6 File Offset: 0x0007B1B6
		protected virtual void HandlePlayerDisconnect(NetworkCommunicator networkPeer)
		{
		}

		// Token: 0x04000CCE RID: 3278
		private GameNetwork.NetworkMessageHandlerRegistererContainer _missionNetworkMessageHandlerRegisterer;
	}
}
