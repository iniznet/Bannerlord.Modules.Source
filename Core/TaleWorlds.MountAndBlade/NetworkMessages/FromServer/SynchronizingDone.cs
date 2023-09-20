using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	// Token: 0x020000C8 RID: 200
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class SynchronizingDone : GameNetworkMessage
	{
		// Token: 0x170001DC RID: 476
		// (get) Token: 0x0600083A RID: 2106 RVA: 0x0000EB9F File Offset: 0x0000CD9F
		// (set) Token: 0x0600083B RID: 2107 RVA: 0x0000EBA7 File Offset: 0x0000CDA7
		public NetworkCommunicator Peer { get; private set; }

		// Token: 0x170001DD RID: 477
		// (get) Token: 0x0600083C RID: 2108 RVA: 0x0000EBB0 File Offset: 0x0000CDB0
		// (set) Token: 0x0600083D RID: 2109 RVA: 0x0000EBB8 File Offset: 0x0000CDB8
		public bool Synchronized { get; private set; }

		// Token: 0x0600083E RID: 2110 RVA: 0x0000EBC1 File Offset: 0x0000CDC1
		public SynchronizingDone(NetworkCommunicator peer, bool synchronized)
		{
			this.Peer = peer;
			this.Synchronized = synchronized;
		}

		// Token: 0x0600083F RID: 2111 RVA: 0x0000EBD7 File Offset: 0x0000CDD7
		public SynchronizingDone()
		{
		}

		// Token: 0x06000840 RID: 2112 RVA: 0x0000EBE0 File Offset: 0x0000CDE0
		protected override bool OnRead()
		{
			bool flag = true;
			this.Peer = GameNetworkMessage.ReadNetworkPeerReferenceFromPacket(ref flag, false);
			this.Synchronized = GameNetworkMessage.ReadBoolFromPacket(ref flag);
			return flag;
		}

		// Token: 0x06000841 RID: 2113 RVA: 0x0000EC0B File Offset: 0x0000CE0B
		protected override void OnWrite()
		{
			GameNetworkMessage.WriteNetworkPeerReferenceToPacket(this.Peer);
			GameNetworkMessage.WriteBoolToPacket(this.Synchronized);
		}

		// Token: 0x06000842 RID: 2114 RVA: 0x0000EC23 File Offset: 0x0000CE23
		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.General;
		}

		// Token: 0x06000843 RID: 2115 RVA: 0x0000EC28 File Offset: 0x0000CE28
		protected override string OnGetLogFormat()
		{
			string text = string.Concat(new object[]
			{
				"peer with name: ",
				this.Peer.UserName,
				", and index: ",
				this.Peer.Index
			});
			if (!this.Synchronized)
			{
				return "Synchronized: FALSE for " + text + " (Peer will not receive broadcasted messages)";
			}
			return "Synchronized: TRUE for " + text + " (received all initial data from the server and will now receive broadcasted messages)";
		}
	}
}
