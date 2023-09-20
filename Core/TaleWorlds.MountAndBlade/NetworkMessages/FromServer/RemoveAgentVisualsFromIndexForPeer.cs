using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	// Token: 0x02000085 RID: 133
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class RemoveAgentVisualsFromIndexForPeer : GameNetworkMessage
	{
		// Token: 0x1700012E RID: 302
		// (get) Token: 0x06000553 RID: 1363 RVA: 0x0000A12E File Offset: 0x0000832E
		// (set) Token: 0x06000554 RID: 1364 RVA: 0x0000A136 File Offset: 0x00008336
		public NetworkCommunicator Peer { get; private set; }

		// Token: 0x1700012F RID: 303
		// (get) Token: 0x06000555 RID: 1365 RVA: 0x0000A13F File Offset: 0x0000833F
		// (set) Token: 0x06000556 RID: 1366 RVA: 0x0000A147 File Offset: 0x00008347
		public int VisualsIndex { get; private set; }

		// Token: 0x06000557 RID: 1367 RVA: 0x0000A150 File Offset: 0x00008350
		public RemoveAgentVisualsFromIndexForPeer(NetworkCommunicator peer, int index)
		{
			this.Peer = peer;
			this.VisualsIndex = index;
		}

		// Token: 0x06000558 RID: 1368 RVA: 0x0000A166 File Offset: 0x00008366
		public RemoveAgentVisualsFromIndexForPeer()
		{
		}

		// Token: 0x06000559 RID: 1369 RVA: 0x0000A170 File Offset: 0x00008370
		protected override bool OnRead()
		{
			bool flag = true;
			this.Peer = GameNetworkMessage.ReadNetworkPeerReferenceFromPacket(ref flag, false);
			this.VisualsIndex = GameNetworkMessage.ReadIntFromPacket(CompressionMission.AgentOffsetCompressionInfo, ref flag);
			return flag;
		}

		// Token: 0x0600055A RID: 1370 RVA: 0x0000A1A0 File Offset: 0x000083A0
		protected override void OnWrite()
		{
			GameNetworkMessage.WriteNetworkPeerReferenceToPacket(this.Peer);
			GameNetworkMessage.WriteIntToPacket(this.VisualsIndex, CompressionMission.AgentOffsetCompressionInfo);
		}

		// Token: 0x0600055B RID: 1371 RVA: 0x0000A1BD File Offset: 0x000083BD
		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.Agents;
		}

		// Token: 0x0600055C RID: 1372 RVA: 0x0000A1C5 File Offset: 0x000083C5
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
