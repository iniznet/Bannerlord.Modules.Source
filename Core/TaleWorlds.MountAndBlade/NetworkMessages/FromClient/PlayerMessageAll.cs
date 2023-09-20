using System;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromClient
{
	// Token: 0x02000005 RID: 5
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromClient)]
	public sealed class PlayerMessageAll : GameNetworkMessage
	{
		// Token: 0x17000001 RID: 1
		// (get) Token: 0x06000004 RID: 4 RVA: 0x00002058 File Offset: 0x00000258
		// (set) Token: 0x06000005 RID: 5 RVA: 0x00002060 File Offset: 0x00000260
		public string Message { get; private set; }

		// Token: 0x17000002 RID: 2
		// (get) Token: 0x06000006 RID: 6 RVA: 0x00002069 File Offset: 0x00000269
		// (set) Token: 0x06000007 RID: 7 RVA: 0x00002071 File Offset: 0x00000271
		public List<VirtualPlayer> ReceiverList { get; private set; }

		// Token: 0x17000003 RID: 3
		// (get) Token: 0x06000008 RID: 8 RVA: 0x0000207A File Offset: 0x0000027A
		// (set) Token: 0x06000009 RID: 9 RVA: 0x00002082 File Offset: 0x00000282
		public bool HasReceiverList { get; private set; }

		// Token: 0x0600000A RID: 10 RVA: 0x0000208B File Offset: 0x0000028B
		public PlayerMessageAll(string message, List<VirtualPlayer> receiverList)
		{
			this.Message = message;
			this.ReceiverList = receiverList;
			this.HasReceiverList = true;
		}

		// Token: 0x0600000B RID: 11 RVA: 0x000020A8 File Offset: 0x000002A8
		public PlayerMessageAll(string message)
		{
			this.Message = message;
		}

		// Token: 0x0600000C RID: 12 RVA: 0x000020B7 File Offset: 0x000002B7
		public PlayerMessageAll()
		{
		}

		// Token: 0x0600000D RID: 13 RVA: 0x000020C0 File Offset: 0x000002C0
		protected override void OnWrite()
		{
			GameNetworkMessage.WriteStringToPacket(this.Message);
			int num = 0;
			if (this.ReceiverList != null)
			{
				num = this.ReceiverList.Count;
			}
			GameNetworkMessage.WriteBoolToPacket(this.HasReceiverList);
			GameNetworkMessage.WriteIntToPacket(num, CompressionBasic.PlayerCompressionInfo);
			for (int i = 0; i < num; i++)
			{
				GameNetworkMessage.WriteVirtualPlayerReferenceToPacket(this.ReceiverList[i]);
			}
		}

		// Token: 0x0600000E RID: 14 RVA: 0x00002124 File Offset: 0x00000324
		protected override bool OnRead()
		{
			bool flag = true;
			this.Message = GameNetworkMessage.ReadStringFromPacket(ref flag);
			this.HasReceiverList = GameNetworkMessage.ReadBoolFromPacket(ref flag);
			int num = GameNetworkMessage.ReadIntFromPacket(CompressionBasic.PlayerCompressionInfo, ref flag);
			if (this.HasReceiverList)
			{
				this.ReceiverList = new List<VirtualPlayer>();
				if (num > 0)
				{
					for (int i = 0; i < num; i++)
					{
						VirtualPlayer virtualPlayer = GameNetworkMessage.ReadVirtualPlayerReferenceToPacket(ref flag, false);
						this.ReceiverList.Add(virtualPlayer);
					}
				}
			}
			return flag;
		}

		// Token: 0x0600000F RID: 15 RVA: 0x00002193 File Offset: 0x00000393
		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.Messaging;
		}

		// Token: 0x06000010 RID: 16 RVA: 0x00002197 File Offset: 0x00000397
		protected override string OnGetLogFormat()
		{
			return "Receiving Player message to all: " + this.Message;
		}
	}
}
