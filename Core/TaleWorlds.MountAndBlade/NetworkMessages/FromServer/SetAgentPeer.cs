using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class SetAgentPeer : GameNetworkMessage
	{
		public Agent Agent { get; private set; }

		public NetworkCommunicator Peer { get; private set; }

		public SetAgentPeer(Agent agent, NetworkCommunicator peer)
		{
			this.Agent = agent;
			this.Peer = peer;
		}

		public SetAgentPeer()
		{
		}

		protected override bool OnRead()
		{
			bool flag = true;
			this.Agent = GameNetworkMessage.ReadAgentReferenceFromPacket(ref flag, true);
			this.Peer = GameNetworkMessage.ReadNetworkPeerReferenceFromPacket(ref flag, true);
			return flag;
		}

		protected override void OnWrite()
		{
			GameNetworkMessage.WriteAgentReferenceToPacket(this.Agent);
			GameNetworkMessage.WriteNetworkPeerReferenceToPacket(this.Peer);
		}

		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.Peers | MultiplayerMessageFilter.Agents;
		}

		protected override string OnGetLogFormat()
		{
			if (this.Agent == null)
			{
				return "Ignoring the message for invalid agent.";
			}
			return string.Concat(new object[]
			{
				"Set NetworkPeer ",
				(this.Peer != null) ? "" : "(to NULL) ",
				"on Agent with name: ",
				this.Agent.Name,
				" and agent-index: ",
				this.Agent.Index
			});
		}
	}
}
