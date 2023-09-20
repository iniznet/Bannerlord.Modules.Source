using System;
using System.Collections.Generic;
using NetworkMessages.FromServer;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade.Diamond;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x020002FA RID: 762
	public class LobbyNetworkComponent : UdpNetworkComponent
	{
		// Token: 0x06002972 RID: 10610 RVA: 0x000A063D File Offset: 0x0009E83D
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

		// Token: 0x06002973 RID: 10611 RVA: 0x000A0674 File Offset: 0x0009E874
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

		// Token: 0x06002974 RID: 10612 RVA: 0x000A06D4 File Offset: 0x0009E8D4
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

		// Token: 0x06002975 RID: 10613 RVA: 0x000A0758 File Offset: 0x0009E958
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

		// Token: 0x06002976 RID: 10614 RVA: 0x000A083C File Offset: 0x0009EA3C
		public override void HandleLateNewClientAfterLoadingFinished(NetworkCommunicator networkPeer)
		{
		}

		// Token: 0x06002977 RID: 10615 RVA: 0x000A083E File Offset: 0x0009EA3E
		public override void HandlePlayerDisconnect(NetworkCommunicator networkPeer)
		{
		}

		// Token: 0x06002978 RID: 10616 RVA: 0x000A0840 File Offset: 0x0009EA40
		public override void OnUdpNetworkHandlerTick(float dt)
		{
		}

		// Token: 0x04000F79 RID: 3961
		public const int MaxForcedAvatarIndex = 100;
	}
}
