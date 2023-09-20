using System;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x0200032E RID: 814
	public abstract class UdpNetworkComponent : IUdpNetworkHandler
	{
		// Token: 0x06002BF8 RID: 11256 RVA: 0x000AA920 File Offset: 0x000A8B20
		protected UdpNetworkComponent()
		{
			this._missionNetworkMessageHandlerRegisterer = new GameNetwork.NetworkMessageHandlerRegistererContainer();
			this.AddRemoveMessageHandlers(this._missionNetworkMessageHandlerRegisterer);
			this._missionNetworkMessageHandlerRegisterer.RegisterMessages();
		}

		// Token: 0x06002BF9 RID: 11257 RVA: 0x000AA94A File Offset: 0x000A8B4A
		protected virtual void AddRemoveMessageHandlers(GameNetwork.NetworkMessageHandlerRegistererContainer registerer)
		{
		}

		// Token: 0x06002BFA RID: 11258 RVA: 0x000AA94C File Offset: 0x000A8B4C
		public virtual void OnUdpNetworkHandlerClose()
		{
			GameNetwork.NetworkMessageHandlerRegistererContainer missionNetworkMessageHandlerRegisterer = this._missionNetworkMessageHandlerRegisterer;
			if (missionNetworkMessageHandlerRegisterer != null)
			{
				missionNetworkMessageHandlerRegisterer.UnregisterMessages();
			}
			GameNetwork.NetworkComponents.Remove(this);
		}

		// Token: 0x06002BFB RID: 11259 RVA: 0x000AA96B File Offset: 0x000A8B6B
		public virtual void OnUdpNetworkHandlerTick(float dt)
		{
		}

		// Token: 0x06002BFC RID: 11260 RVA: 0x000AA96D File Offset: 0x000A8B6D
		public virtual void HandleNewClientConnect(PlayerConnectionInfo clientConnectionInfo)
		{
		}

		// Token: 0x06002BFD RID: 11261 RVA: 0x000AA96F File Offset: 0x000A8B6F
		public virtual void HandleEarlyNewClientAfterLoadingFinished(NetworkCommunicator networkPeer)
		{
		}

		// Token: 0x06002BFE RID: 11262 RVA: 0x000AA971 File Offset: 0x000A8B71
		public virtual void HandleNewClientAfterLoadingFinished(NetworkCommunicator networkPeer)
		{
		}

		// Token: 0x06002BFF RID: 11263 RVA: 0x000AA973 File Offset: 0x000A8B73
		public virtual void HandleLateNewClientAfterLoadingFinished(NetworkCommunicator networkPeer)
		{
		}

		// Token: 0x06002C00 RID: 11264 RVA: 0x000AA975 File Offset: 0x000A8B75
		public virtual void HandleNewClientAfterSynchronized(NetworkCommunicator networkPeer)
		{
		}

		// Token: 0x06002C01 RID: 11265 RVA: 0x000AA977 File Offset: 0x000A8B77
		public virtual void HandleLateNewClientAfterSynchronized(NetworkCommunicator networkPeer)
		{
		}

		// Token: 0x06002C02 RID: 11266 RVA: 0x000AA979 File Offset: 0x000A8B79
		public virtual void OnEveryoneUnSynchronized()
		{
		}

		// Token: 0x06002C03 RID: 11267 RVA: 0x000AA97B File Offset: 0x000A8B7B
		public void HandleEarlyPlayerDisconnect(NetworkCommunicator networkPeer)
		{
		}

		// Token: 0x06002C04 RID: 11268 RVA: 0x000AA97D File Offset: 0x000A8B7D
		public virtual void HandlePlayerDisconnect(NetworkCommunicator networkPeer)
		{
		}

		// Token: 0x06002C05 RID: 11269 RVA: 0x000AA97F File Offset: 0x000A8B7F
		public virtual void OnPlayerDisconnectedFromServer(NetworkCommunicator networkPeer)
		{
		}

		// Token: 0x06002C06 RID: 11270 RVA: 0x000AA981 File Offset: 0x000A8B81
		public virtual void OnDisconnectedFromServer()
		{
		}

		// Token: 0x040010A9 RID: 4265
		private GameNetwork.NetworkMessageHandlerRegistererContainer _missionNetworkMessageHandlerRegisterer;
	}
}
