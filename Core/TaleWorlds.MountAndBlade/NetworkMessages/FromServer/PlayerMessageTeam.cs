using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	// Token: 0x02000037 RID: 55
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class PlayerMessageTeam : GameNetworkMessage
	{
		// Token: 0x1700004B RID: 75
		// (get) Token: 0x060001BE RID: 446 RVA: 0x00004139 File Offset: 0x00002339
		// (set) Token: 0x060001BF RID: 447 RVA: 0x00004141 File Offset: 0x00002341
		public string Message { get; private set; }

		// Token: 0x1700004C RID: 76
		// (get) Token: 0x060001C0 RID: 448 RVA: 0x0000414A File Offset: 0x0000234A
		// (set) Token: 0x060001C1 RID: 449 RVA: 0x00004152 File Offset: 0x00002352
		public NetworkCommunicator Player { get; private set; }

		// Token: 0x060001C2 RID: 450 RVA: 0x0000415B File Offset: 0x0000235B
		public PlayerMessageTeam(NetworkCommunicator player, string message)
		{
			this.Player = player;
			this.Message = message;
		}

		// Token: 0x060001C3 RID: 451 RVA: 0x00004171 File Offset: 0x00002371
		public PlayerMessageTeam()
		{
		}

		// Token: 0x060001C4 RID: 452 RVA: 0x00004179 File Offset: 0x00002379
		protected override void OnWrite()
		{
			GameNetworkMessage.WriteNetworkPeerReferenceToPacket(this.Player);
			GameNetworkMessage.WriteStringToPacket(this.Message);
		}

		// Token: 0x060001C5 RID: 453 RVA: 0x00004194 File Offset: 0x00002394
		protected override bool OnRead()
		{
			bool flag = true;
			this.Player = GameNetworkMessage.ReadNetworkPeerReferenceFromPacket(ref flag, false);
			this.Message = GameNetworkMessage.ReadStringFromPacket(ref flag);
			return flag;
		}

		// Token: 0x060001C6 RID: 454 RVA: 0x000041BF File Offset: 0x000023BF
		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.Messaging;
		}

		// Token: 0x060001C7 RID: 455 RVA: 0x000041C4 File Offset: 0x000023C4
		protected override string OnGetLogFormat()
		{
			return string.Concat(new object[]
			{
				"Receiving team message: ",
				this.Message,
				" from peer: ",
				this.Player.UserName,
				" index: ",
				this.Player.Index
			});
		}
	}
}
