using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	// Token: 0x020000C3 RID: 195
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class PingReplication : GameNetworkMessage
	{
		// Token: 0x170001D2 RID: 466
		// (get) Token: 0x06000810 RID: 2064 RVA: 0x0000E7CB File Offset: 0x0000C9CB
		// (set) Token: 0x06000811 RID: 2065 RVA: 0x0000E7D3 File Offset: 0x0000C9D3
		internal NetworkCommunicator Peer { get; private set; }

		// Token: 0x170001D3 RID: 467
		// (get) Token: 0x06000812 RID: 2066 RVA: 0x0000E7DC File Offset: 0x0000C9DC
		// (set) Token: 0x06000813 RID: 2067 RVA: 0x0000E7E4 File Offset: 0x0000C9E4
		internal int PingValue { get; private set; }

		// Token: 0x06000814 RID: 2068 RVA: 0x0000E7ED File Offset: 0x0000C9ED
		public PingReplication()
		{
		}

		// Token: 0x06000815 RID: 2069 RVA: 0x0000E7F5 File Offset: 0x0000C9F5
		internal PingReplication(NetworkCommunicator peer, int ping)
		{
			this.Peer = peer;
			this.PingValue = ping;
			if (this.PingValue > 1023)
			{
				this.PingValue = 1023;
			}
		}

		// Token: 0x06000816 RID: 2070 RVA: 0x0000E824 File Offset: 0x0000CA24
		protected override bool OnRead()
		{
			bool flag = true;
			this.Peer = GameNetworkMessage.ReadNetworkPeerReferenceFromPacket(ref flag, true);
			this.PingValue = GameNetworkMessage.ReadIntFromPacket(CompressionBasic.PingValueCompressionInfo, ref flag);
			return flag;
		}

		// Token: 0x06000817 RID: 2071 RVA: 0x0000E854 File Offset: 0x0000CA54
		protected override void OnWrite()
		{
			GameNetworkMessage.WriteNetworkPeerReferenceToPacket(this.Peer);
			GameNetworkMessage.WriteIntToPacket(this.PingValue, CompressionBasic.PingValueCompressionInfo);
		}

		// Token: 0x06000818 RID: 2072 RVA: 0x0000E871 File Offset: 0x0000CA71
		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.MissionDetailed;
		}

		// Token: 0x06000819 RID: 2073 RVA: 0x0000E879 File Offset: 0x0000CA79
		protected override string OnGetLogFormat()
		{
			return "PingReplication";
		}

		// Token: 0x040001D4 RID: 468
		public const int MaxPingToReplicate = 1023;
	}
}
