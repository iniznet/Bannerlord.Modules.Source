using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	// Token: 0x02000084 RID: 132
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class RemoveAgentVisualsForPeer : GameNetworkMessage
	{
		// Token: 0x1700012D RID: 301
		// (get) Token: 0x0600054B RID: 1355 RVA: 0x0000A0BC File Offset: 0x000082BC
		// (set) Token: 0x0600054C RID: 1356 RVA: 0x0000A0C4 File Offset: 0x000082C4
		public NetworkCommunicator Peer { get; private set; }

		// Token: 0x0600054D RID: 1357 RVA: 0x0000A0CD File Offset: 0x000082CD
		public RemoveAgentVisualsForPeer(NetworkCommunicator peer)
		{
			this.Peer = peer;
		}

		// Token: 0x0600054E RID: 1358 RVA: 0x0000A0DC File Offset: 0x000082DC
		public RemoveAgentVisualsForPeer()
		{
		}

		// Token: 0x0600054F RID: 1359 RVA: 0x0000A0E4 File Offset: 0x000082E4
		protected override bool OnRead()
		{
			bool flag = true;
			this.Peer = GameNetworkMessage.ReadNetworkPeerReferenceFromPacket(ref flag, false);
			return flag;
		}

		// Token: 0x06000550 RID: 1360 RVA: 0x0000A102 File Offset: 0x00008302
		protected override void OnWrite()
		{
			GameNetworkMessage.WriteNetworkPeerReferenceToPacket(this.Peer);
		}

		// Token: 0x06000551 RID: 1361 RVA: 0x0000A10F File Offset: 0x0000830F
		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.Agents;
		}

		// Token: 0x06000552 RID: 1362 RVA: 0x0000A117 File Offset: 0x00008317
		protected override string OnGetLogFormat()
		{
			return "Removing all AgentVisuals for peer: " + this.Peer.UserName;
		}
	}
}
