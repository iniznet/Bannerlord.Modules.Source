using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	// Token: 0x02000036 RID: 54
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class PlayerMessageAll : GameNetworkMessage
	{
		// Token: 0x17000049 RID: 73
		// (get) Token: 0x060001B4 RID: 436 RVA: 0x0000409F File Offset: 0x0000229F
		// (set) Token: 0x060001B5 RID: 437 RVA: 0x000040A7 File Offset: 0x000022A7
		public NetworkCommunicator Player { get; private set; }

		// Token: 0x1700004A RID: 74
		// (get) Token: 0x060001B6 RID: 438 RVA: 0x000040B0 File Offset: 0x000022B0
		// (set) Token: 0x060001B7 RID: 439 RVA: 0x000040B8 File Offset: 0x000022B8
		public string Message { get; private set; }

		// Token: 0x060001B8 RID: 440 RVA: 0x000040C1 File Offset: 0x000022C1
		public PlayerMessageAll(NetworkCommunicator player, string message)
		{
			this.Player = player;
			this.Message = message;
		}

		// Token: 0x060001B9 RID: 441 RVA: 0x000040D7 File Offset: 0x000022D7
		public PlayerMessageAll()
		{
		}

		// Token: 0x060001BA RID: 442 RVA: 0x000040DF File Offset: 0x000022DF
		protected override void OnWrite()
		{
			GameNetworkMessage.WriteNetworkPeerReferenceToPacket(this.Player);
			GameNetworkMessage.WriteStringToPacket(this.Message);
		}

		// Token: 0x060001BB RID: 443 RVA: 0x000040F8 File Offset: 0x000022F8
		protected override bool OnRead()
		{
			bool flag = true;
			this.Player = GameNetworkMessage.ReadNetworkPeerReferenceFromPacket(ref flag, false);
			this.Message = GameNetworkMessage.ReadStringFromPacket(ref flag);
			return flag;
		}

		// Token: 0x060001BC RID: 444 RVA: 0x00004123 File Offset: 0x00002323
		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.Messaging;
		}

		// Token: 0x060001BD RID: 445 RVA: 0x00004127 File Offset: 0x00002327
		protected override string OnGetLogFormat()
		{
			return "Receiving Player message to all: " + this.Message;
		}
	}
}
