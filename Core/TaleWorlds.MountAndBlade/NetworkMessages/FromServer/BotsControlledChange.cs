using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class BotsControlledChange : GameNetworkMessage
	{
		public NetworkCommunicator Peer { get; private set; }

		public int AliveCount { get; private set; }

		public int TotalCount { get; private set; }

		public BotsControlledChange(NetworkCommunicator peer, int aliveCount, int totalCount)
		{
			this.Peer = peer;
			this.AliveCount = aliveCount;
			this.TotalCount = totalCount;
		}

		public BotsControlledChange()
		{
		}

		protected override bool OnRead()
		{
			bool flag = true;
			this.Peer = GameNetworkMessage.ReadNetworkPeerReferenceFromPacket(ref flag, false);
			this.AliveCount = GameNetworkMessage.ReadIntFromPacket(CompressionMission.AgentOffsetCompressionInfo, ref flag);
			this.TotalCount = GameNetworkMessage.ReadIntFromPacket(CompressionMission.AgentOffsetCompressionInfo, ref flag);
			return flag;
		}

		protected override void OnWrite()
		{
			GameNetworkMessage.WriteNetworkPeerReferenceToPacket(this.Peer);
			GameNetworkMessage.WriteIntToPacket(this.AliveCount, CompressionMission.AgentOffsetCompressionInfo);
			GameNetworkMessage.WriteIntToPacket(this.TotalCount, CompressionMission.AgentOffsetCompressionInfo);
		}

		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.Administration;
		}

		protected override string OnGetLogFormat()
		{
			return string.Concat(new object[]
			{
				"Bot Controlled Count Changed. Peer: ",
				this.Peer.UserName,
				" now has ",
				this.AliveCount,
				" alive bots, out of: ",
				this.TotalCount,
				" total bots."
			});
		}
	}
}
