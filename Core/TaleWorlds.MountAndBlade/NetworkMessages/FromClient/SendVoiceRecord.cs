using System;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromClient
{
	// Token: 0x02000034 RID: 52
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromClient)]
	public sealed class SendVoiceRecord : GameNetworkMessage
	{
		// Token: 0x17000043 RID: 67
		// (get) Token: 0x0600019B RID: 411 RVA: 0x00003E2A File Offset: 0x0000202A
		// (set) Token: 0x0600019C RID: 412 RVA: 0x00003E32 File Offset: 0x00002032
		public byte[] Buffer { get; private set; }

		// Token: 0x17000044 RID: 68
		// (get) Token: 0x0600019D RID: 413 RVA: 0x00003E3B File Offset: 0x0000203B
		// (set) Token: 0x0600019E RID: 414 RVA: 0x00003E43 File Offset: 0x00002043
		public int BufferLength { get; private set; }

		// Token: 0x17000045 RID: 69
		// (get) Token: 0x0600019F RID: 415 RVA: 0x00003E4C File Offset: 0x0000204C
		// (set) Token: 0x060001A0 RID: 416 RVA: 0x00003E54 File Offset: 0x00002054
		public List<VirtualPlayer> ReceiverList { get; private set; }

		// Token: 0x17000046 RID: 70
		// (get) Token: 0x060001A1 RID: 417 RVA: 0x00003E5D File Offset: 0x0000205D
		// (set) Token: 0x060001A2 RID: 418 RVA: 0x00003E65 File Offset: 0x00002065
		public bool HasReceiverList { get; private set; }

		// Token: 0x060001A3 RID: 419 RVA: 0x00003E6E File Offset: 0x0000206E
		public SendVoiceRecord()
		{
		}

		// Token: 0x060001A4 RID: 420 RVA: 0x00003E76 File Offset: 0x00002076
		public SendVoiceRecord(byte[] buffer, int bufferLength)
		{
			this.Buffer = buffer;
			this.BufferLength = bufferLength;
		}

		// Token: 0x060001A5 RID: 421 RVA: 0x00003E8C File Offset: 0x0000208C
		public SendVoiceRecord(byte[] buffer, int bufferLength, List<VirtualPlayer> receiverList)
		{
			this.Buffer = buffer;
			this.BufferLength = bufferLength;
			this.ReceiverList = receiverList;
			this.HasReceiverList = true;
		}

		// Token: 0x060001A6 RID: 422 RVA: 0x00003EB0 File Offset: 0x000020B0
		protected override void OnWrite()
		{
			GameNetworkMessage.WriteByteArrayToPacket(this.Buffer, 0, this.BufferLength);
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

		// Token: 0x060001A7 RID: 423 RVA: 0x00003F18 File Offset: 0x00002118
		protected override bool OnRead()
		{
			bool flag = true;
			this.Buffer = new byte[1440];
			this.BufferLength = GameNetworkMessage.ReadByteArrayFromPacket(this.Buffer, 0, 1440, ref flag);
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

		// Token: 0x060001A8 RID: 424 RVA: 0x00003FA3 File Offset: 0x000021A3
		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.None;
		}

		// Token: 0x060001A9 RID: 425 RVA: 0x00003FA7 File Offset: 0x000021A7
		protected override string OnGetLogFormat()
		{
			return string.Empty;
		}
	}
}
