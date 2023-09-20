using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	// Token: 0x0200008D RID: 141
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class SetAgentPeer : GameNetworkMessage
	{
		// Token: 0x17000145 RID: 325
		// (get) Token: 0x060005B1 RID: 1457 RVA: 0x0000AA0C File Offset: 0x00008C0C
		// (set) Token: 0x060005B2 RID: 1458 RVA: 0x0000AA14 File Offset: 0x00008C14
		public Agent Agent { get; private set; }

		// Token: 0x17000146 RID: 326
		// (get) Token: 0x060005B3 RID: 1459 RVA: 0x0000AA1D File Offset: 0x00008C1D
		// (set) Token: 0x060005B4 RID: 1460 RVA: 0x0000AA25 File Offset: 0x00008C25
		public NetworkCommunicator Peer { get; private set; }

		// Token: 0x060005B5 RID: 1461 RVA: 0x0000AA2E File Offset: 0x00008C2E
		public SetAgentPeer(Agent agent, NetworkCommunicator peer)
		{
			this.Agent = agent;
			this.Peer = peer;
		}

		// Token: 0x060005B6 RID: 1462 RVA: 0x0000AA44 File Offset: 0x00008C44
		public SetAgentPeer()
		{
		}

		// Token: 0x060005B7 RID: 1463 RVA: 0x0000AA4C File Offset: 0x00008C4C
		protected override bool OnRead()
		{
			bool flag = true;
			this.Agent = GameNetworkMessage.ReadAgentReferenceFromPacket(ref flag, true);
			this.Peer = GameNetworkMessage.ReadNetworkPeerReferenceFromPacket(ref flag, true);
			return flag;
		}

		// Token: 0x060005B8 RID: 1464 RVA: 0x0000AA78 File Offset: 0x00008C78
		protected override void OnWrite()
		{
			GameNetworkMessage.WriteAgentReferenceToPacket(this.Agent);
			GameNetworkMessage.WriteNetworkPeerReferenceToPacket(this.Peer);
		}

		// Token: 0x060005B9 RID: 1465 RVA: 0x0000AA90 File Offset: 0x00008C90
		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.Peers | MultiplayerMessageFilter.Agents;
		}

		// Token: 0x060005BA RID: 1466 RVA: 0x0000AA98 File Offset: 0x00008C98
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
