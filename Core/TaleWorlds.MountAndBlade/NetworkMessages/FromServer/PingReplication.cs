using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class PingReplication : GameNetworkMessage
	{
		internal NetworkCommunicator Peer { get; private set; }

		internal int PingValue { get; private set; }

		public PingReplication()
		{
		}

		internal PingReplication(NetworkCommunicator peer, int ping)
		{
			this.Peer = peer;
			this.PingValue = ping;
			if (this.PingValue > 1023)
			{
				this.PingValue = 1023;
			}
		}

		protected override bool OnRead()
		{
			bool flag = true;
			this.Peer = GameNetworkMessage.ReadNetworkPeerReferenceFromPacket(ref flag, true);
			this.PingValue = GameNetworkMessage.ReadIntFromPacket(CompressionBasic.PingValueCompressionInfo, ref flag);
			return flag;
		}

		protected override void OnWrite()
		{
			GameNetworkMessage.WriteNetworkPeerReferenceToPacket(this.Peer);
			GameNetworkMessage.WriteIntToPacket(this.PingValue, CompressionBasic.PingValueCompressionInfo);
		}

		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.MissionDetailed;
		}

		protected override string OnGetLogFormat()
		{
			return "PingReplication";
		}

		public const int MaxPingToReplicate = 1023;
	}
}
