using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	// Token: 0x02000042 RID: 66
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class DuelSessionStarted : GameNetworkMessage
	{
		// Token: 0x17000062 RID: 98
		// (get) Token: 0x0600022D RID: 557 RVA: 0x00004A8E File Offset: 0x00002C8E
		// (set) Token: 0x0600022E RID: 558 RVA: 0x00004A96 File Offset: 0x00002C96
		public NetworkCommunicator RequesterPeer { get; private set; }

		// Token: 0x17000063 RID: 99
		// (get) Token: 0x0600022F RID: 559 RVA: 0x00004A9F File Offset: 0x00002C9F
		// (set) Token: 0x06000230 RID: 560 RVA: 0x00004AA7 File Offset: 0x00002CA7
		public NetworkCommunicator RequestedPeer { get; private set; }

		// Token: 0x06000231 RID: 561 RVA: 0x00004AB0 File Offset: 0x00002CB0
		public DuelSessionStarted(NetworkCommunicator requesterPeer, NetworkCommunicator requestedPeer)
		{
			this.RequesterPeer = requesterPeer;
			this.RequestedPeer = requestedPeer;
		}

		// Token: 0x06000232 RID: 562 RVA: 0x00004AC6 File Offset: 0x00002CC6
		public DuelSessionStarted()
		{
		}

		// Token: 0x06000233 RID: 563 RVA: 0x00004AD0 File Offset: 0x00002CD0
		protected override bool OnRead()
		{
			bool flag = true;
			this.RequesterPeer = GameNetworkMessage.ReadNetworkPeerReferenceFromPacket(ref flag, false);
			this.RequestedPeer = GameNetworkMessage.ReadNetworkPeerReferenceFromPacket(ref flag, false);
			return flag;
		}

		// Token: 0x06000234 RID: 564 RVA: 0x00004AFC File Offset: 0x00002CFC
		protected override void OnWrite()
		{
			GameNetworkMessage.WriteNetworkPeerReferenceToPacket(this.RequesterPeer);
			GameNetworkMessage.WriteNetworkPeerReferenceToPacket(this.RequestedPeer);
		}

		// Token: 0x06000235 RID: 565 RVA: 0x00004B14 File Offset: 0x00002D14
		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.GameMode;
		}

		// Token: 0x06000236 RID: 566 RVA: 0x00004B1C File Offset: 0x00002D1C
		protected override string OnGetLogFormat()
		{
			return string.Concat(new object[]
			{
				"Duel session started between agent with name: ",
				this.RequestedPeer.UserName,
				" and index: ",
				this.RequestedPeer.Index,
				" and agent with name: ",
				this.RequesterPeer.UserName,
				" and index: ",
				this.RequesterPeer.Index
			});
		}
	}
}
