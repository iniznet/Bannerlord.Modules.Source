using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromClient
{
	// Token: 0x02000009 RID: 9
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromClient)]
	public sealed class DuelResponse : GameNetworkMessage
	{
		// Token: 0x1700000A RID: 10
		// (get) Token: 0x06000030 RID: 48 RVA: 0x000024C1 File Offset: 0x000006C1
		// (set) Token: 0x06000031 RID: 49 RVA: 0x000024C9 File Offset: 0x000006C9
		public NetworkCommunicator Peer { get; private set; }

		// Token: 0x1700000B RID: 11
		// (get) Token: 0x06000032 RID: 50 RVA: 0x000024D2 File Offset: 0x000006D2
		// (set) Token: 0x06000033 RID: 51 RVA: 0x000024DA File Offset: 0x000006DA
		public bool Accepted { get; private set; }

		// Token: 0x06000034 RID: 52 RVA: 0x000024E3 File Offset: 0x000006E3
		public DuelResponse(NetworkCommunicator peer, bool accepted)
		{
			this.Peer = peer;
			this.Accepted = accepted;
		}

		// Token: 0x06000035 RID: 53 RVA: 0x000024F9 File Offset: 0x000006F9
		public DuelResponse()
		{
		}

		// Token: 0x06000036 RID: 54 RVA: 0x00002504 File Offset: 0x00000704
		protected override bool OnRead()
		{
			bool flag = true;
			this.Peer = GameNetworkMessage.ReadNetworkPeerReferenceFromPacket(ref flag, false);
			this.Accepted = GameNetworkMessage.ReadBoolFromPacket(ref flag);
			return flag;
		}

		// Token: 0x06000037 RID: 55 RVA: 0x0000252F File Offset: 0x0000072F
		protected override void OnWrite()
		{
			GameNetworkMessage.WriteNetworkPeerReferenceToPacket(this.Peer);
			GameNetworkMessage.WriteBoolToPacket(this.Accepted);
		}

		// Token: 0x06000038 RID: 56 RVA: 0x00002547 File Offset: 0x00000747
		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.GameMode;
		}

		// Token: 0x06000039 RID: 57 RVA: 0x0000254F File Offset: 0x0000074F
		protected override string OnGetLogFormat()
		{
			return "Duel Response: " + (this.Accepted ? " Accepted" : " Not accepted");
		}
	}
}
