using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class SetAgentPeer : GameNetworkMessage
	{
		public int AgentIndex { get; private set; }

		public NetworkCommunicator Peer { get; private set; }

		public SetAgentPeer(int agentIndex, NetworkCommunicator peer)
		{
			this.AgentIndex = agentIndex;
			this.Peer = peer;
		}

		public SetAgentPeer()
		{
		}

		protected override bool OnRead()
		{
			bool flag = true;
			this.AgentIndex = GameNetworkMessage.ReadAgentIndexFromPacket(ref flag);
			this.Peer = GameNetworkMessage.ReadNetworkPeerReferenceFromPacket(ref flag, true);
			return flag;
		}

		protected override void OnWrite()
		{
			GameNetworkMessage.WriteAgentIndexToPacket(this.AgentIndex);
			GameNetworkMessage.WriteNetworkPeerReferenceToPacket(this.Peer);
		}

		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.Peers | MultiplayerMessageFilter.Agents;
		}

		protected override string OnGetLogFormat()
		{
			if (this.AgentIndex < 0)
			{
				return "Ignoring the message for invalid agent.";
			}
			return string.Concat(new object[]
			{
				"Set NetworkPeer ",
				(this.Peer != null) ? "" : "(to NULL) ",
				"on Agent with agent-index: ",
				this.AgentIndex
			});
		}
	}
}
