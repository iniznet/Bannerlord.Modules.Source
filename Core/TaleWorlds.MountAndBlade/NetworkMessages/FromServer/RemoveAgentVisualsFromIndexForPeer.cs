using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class RemoveAgentVisualsFromIndexForPeer : GameNetworkMessage
	{
		public NetworkCommunicator Peer { get; private set; }

		public int VisualsIndex { get; private set; }

		public RemoveAgentVisualsFromIndexForPeer(NetworkCommunicator peer, int index)
		{
			this.Peer = peer;
			this.VisualsIndex = index;
		}

		public RemoveAgentVisualsFromIndexForPeer()
		{
		}

		protected override bool OnRead()
		{
			bool flag = true;
			this.Peer = GameNetworkMessage.ReadNetworkPeerReferenceFromPacket(ref flag, false);
			this.VisualsIndex = GameNetworkMessage.ReadIntFromPacket(CompressionMission.AgentOffsetCompressionInfo, ref flag);
			return flag;
		}

		protected override void OnWrite()
		{
			GameNetworkMessage.WriteNetworkPeerReferenceToPacket(this.Peer);
			GameNetworkMessage.WriteIntToPacket(this.VisualsIndex, CompressionMission.AgentOffsetCompressionInfo);
		}

		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.Agents;
		}

		protected override string OnGetLogFormat()
		{
			return string.Concat(new object[]
			{
				"Removing AgentVisuals with Index: ",
				this.VisualsIndex,
				", for peer: ",
				this.Peer.UserName
			});
		}
	}
}
