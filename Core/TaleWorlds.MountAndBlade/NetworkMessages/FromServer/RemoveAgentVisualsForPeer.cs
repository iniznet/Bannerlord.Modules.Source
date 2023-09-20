using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class RemoveAgentVisualsForPeer : GameNetworkMessage
	{
		public NetworkCommunicator Peer { get; private set; }

		public RemoveAgentVisualsForPeer(NetworkCommunicator peer)
		{
			this.Peer = peer;
		}

		public RemoveAgentVisualsForPeer()
		{
		}

		protected override bool OnRead()
		{
			bool flag = true;
			this.Peer = GameNetworkMessage.ReadNetworkPeerReferenceFromPacket(ref flag, false);
			return flag;
		}

		protected override void OnWrite()
		{
			GameNetworkMessage.WriteNetworkPeerReferenceToPacket(this.Peer);
		}

		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.Agents;
		}

		protected override string OnGetLogFormat()
		{
			return "Removing all AgentVisuals for peer: " + this.Peer.UserName;
		}
	}
}
