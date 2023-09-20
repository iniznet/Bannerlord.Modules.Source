using System;
using System.Collections.Generic;
using NetworkMessages.FromServer;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade.Diamond;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace TaleWorlds.MountAndBlade
{
	public class LobbyNetworkComponent : UdpNetworkComponent
	{
		protected override void AddRemoveMessageHandlers(GameNetwork.NetworkMessageHandlerRegistererContainer registerer)
		{
			if (GameNetwork.IsClient)
			{
				registerer.Register<InitializeLobbyPeer>(new GameNetworkMessage.ServerMessageHandlerDelegate<InitializeLobbyPeer>(this.HandleServerEventInitializeLobbyPeer));
				return;
			}
			if (GameNetwork.IsReplay)
			{
				registerer.Register<InitializeLobbyPeer>(new GameNetworkMessage.ServerMessageHandlerDelegate<InitializeLobbyPeer>(this.HandleServerEventInitializeLobbyPeer));
			}
		}

		private void HandleServerEventInitializeLobbyPeer(InitializeLobbyPeer message)
		{
			NetworkCommunicator peer = message.Peer;
			VirtualPlayer virtualPlayer = peer.VirtualPlayer;
			virtualPlayer.Id = message.ProvidedId;
			virtualPlayer.IsFemale = message.IsFemale;
			virtualPlayer.BannerCode = message.BannerCode;
			virtualPlayer.BodyProperties = message.BodyProperties;
			virtualPlayer.ChosenBadgeIndex = message.ChosenBadgeIndex;
			peer.ForcedAvatarIndex = message.ForcedAvatarIndex;
		}

		public override void HandleEarlyNewClientAfterLoadingFinished(NetworkCommunicator networkPeer)
		{
			PlayerData parameter = networkPeer.PlayerConnectionInfo.GetParameter<PlayerData>("PlayerData");
			Dictionary<int, List<int>> parameter2 = networkPeer.PlayerConnectionInfo.GetParameter<Dictionary<int, List<int>>>("UsedCosmetics");
			VirtualPlayer virtualPlayer = networkPeer.VirtualPlayer;
			virtualPlayer.Id = parameter.PlayerId;
			virtualPlayer.BannerCode = parameter.Sigil;
			virtualPlayer.BodyProperties = parameter.BodyProperties;
			virtualPlayer.IsFemale = parameter.IsFemale;
			virtualPlayer.ChosenBadgeIndex = parameter.ShownBadgeIndex;
			virtualPlayer.UsedCosmetics = parameter2;
			networkPeer.IsMuted = parameter.IsMuted;
		}

		public override void HandleNewClientAfterLoadingFinished(NetworkCommunicator networkPeer)
		{
			VirtualPlayer virtualPlayer = networkPeer.VirtualPlayer;
			GameNetwork.BeginBroadcastModuleEvent();
			GameNetwork.WriteMessage(new InitializeLobbyPeer(networkPeer, virtualPlayer, -1));
			GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.AddToMissionRecord | GameNetwork.EventBroadcastFlags.DontSendToPeers, null);
			foreach (NetworkCommunicator networkCommunicator in GameNetwork.NetworkPeersIncludingDisconnectedPeers)
			{
				if (networkCommunicator.IsSynchronized || networkCommunicator == networkPeer)
				{
					bool flag = MBNetwork.VirtualPlayers[networkCommunicator.VirtualPlayer.Index] != networkCommunicator.VirtualPlayer;
					if (networkCommunicator != networkPeer && !flag && networkCommunicator != GameNetwork.MyPeer)
					{
						GameNetwork.BeginModuleEventAsServer(networkCommunicator);
						GameNetwork.WriteMessage(new InitializeLobbyPeer(networkPeer, virtualPlayer, -1));
						GameNetwork.EndModuleEventAsServer();
					}
					if (!networkPeer.IsServerPeer)
					{
						GameNetwork.BeginModuleEventAsServer(networkPeer);
						GameNetwork.WriteMessage(new InitializeLobbyPeer(networkCommunicator, networkCommunicator.VirtualPlayer, -1));
						GameNetwork.EndModuleEventAsServer();
					}
				}
			}
		}

		public override void HandleLateNewClientAfterLoadingFinished(NetworkCommunicator networkPeer)
		{
		}

		public override void HandlePlayerDisconnect(NetworkCommunicator networkPeer)
		{
		}

		public override void OnUdpNetworkHandlerTick(float dt)
		{
		}

		public const int MaxForcedAvatarIndex = 100;
	}
}
