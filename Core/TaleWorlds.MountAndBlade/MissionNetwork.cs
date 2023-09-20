using System;

namespace TaleWorlds.MountAndBlade
{
	public abstract class MissionNetwork : MissionLogic, IUdpNetworkHandler
	{
		public override void OnAfterMissionCreated()
		{
			this._missionNetworkMessageHandlerRegisterer = new GameNetwork.NetworkMessageHandlerRegistererContainer();
			this.AddRemoveMessageHandlers(this._missionNetworkMessageHandlerRegisterer);
			this._missionNetworkMessageHandlerRegisterer.RegisterMessages();
		}

		public override void OnBehaviorInitialize()
		{
			base.OnBehaviorInitialize();
			GameNetwork.AddNetworkHandler(this);
		}

		public override void OnRemoveBehavior()
		{
			GameNetwork.RemoveNetworkHandler(this);
			base.OnRemoveBehavior();
		}

		protected virtual void AddRemoveMessageHandlers(GameNetwork.NetworkMessageHandlerRegistererContainer registerer)
		{
		}

		public virtual void OnPlayerConnectedToServer(NetworkCommunicator networkPeer)
		{
		}

		public virtual void OnPlayerDisconnectedFromServer(NetworkCommunicator networkPeer)
		{
		}

		void IUdpNetworkHandler.OnUdpNetworkHandlerTick(float dt)
		{
			this.OnUdpNetworkHandlerTick();
		}

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

		void IUdpNetworkHandler.HandleNewClientConnect(PlayerConnectionInfo clientConnectionInfo)
		{
			this.HandleNewClientConnect(clientConnectionInfo);
		}

		void IUdpNetworkHandler.HandleEarlyNewClientAfterLoadingFinished(NetworkCommunicator networkPeer)
		{
			this.HandleEarlyNewClientAfterLoadingFinished(networkPeer);
		}

		void IUdpNetworkHandler.HandleNewClientAfterLoadingFinished(NetworkCommunicator networkPeer)
		{
			this.HandleNewClientAfterLoadingFinished(networkPeer);
		}

		void IUdpNetworkHandler.HandleLateNewClientAfterLoadingFinished(NetworkCommunicator networkPeer)
		{
			this.HandleLateNewClientAfterLoadingFinished(networkPeer);
		}

		void IUdpNetworkHandler.HandleNewClientAfterSynchronized(NetworkCommunicator networkPeer)
		{
			this.HandleNewClientAfterSynchronized(networkPeer);
		}

		void IUdpNetworkHandler.HandleLateNewClientAfterSynchronized(NetworkCommunicator networkPeer)
		{
			this.HandleLateNewClientAfterSynchronized(networkPeer);
		}

		void IUdpNetworkHandler.HandleEarlyPlayerDisconnect(NetworkCommunicator networkPeer)
		{
			this.HandleEarlyPlayerDisconnect(networkPeer);
		}

		void IUdpNetworkHandler.HandlePlayerDisconnect(NetworkCommunicator networkPeer)
		{
			this.HandlePlayerDisconnect(networkPeer);
		}

		void IUdpNetworkHandler.OnEveryoneUnSynchronized()
		{
		}

		void IUdpNetworkHandler.OnPlayerDisconnectedFromServer(NetworkCommunicator networkPeer)
		{
		}

		void IUdpNetworkHandler.OnDisconnectedFromServer()
		{
		}

		protected virtual void OnUdpNetworkHandlerTick()
		{
		}

		protected virtual void OnUdpNetworkHandlerClose()
		{
		}

		protected virtual void HandleNewClientConnect(PlayerConnectionInfo clientConnectionInfo)
		{
		}

		protected virtual void HandleEarlyNewClientAfterLoadingFinished(NetworkCommunicator networkPeer)
		{
		}

		protected virtual void HandleNewClientAfterLoadingFinished(NetworkCommunicator networkPeer)
		{
		}

		protected virtual void HandleLateNewClientAfterLoadingFinished(NetworkCommunicator networkPeer)
		{
		}

		protected virtual void HandleNewClientAfterSynchronized(NetworkCommunicator networkPeer)
		{
		}

		protected virtual void HandleLateNewClientAfterSynchronized(NetworkCommunicator networkPeer)
		{
		}

		protected virtual void HandleEarlyPlayerDisconnect(NetworkCommunicator networkPeer)
		{
		}

		protected virtual void HandlePlayerDisconnect(NetworkCommunicator networkPeer)
		{
		}

		private GameNetwork.NetworkMessageHandlerRegistererContainer _missionNetworkMessageHandlerRegisterer;
	}
}
