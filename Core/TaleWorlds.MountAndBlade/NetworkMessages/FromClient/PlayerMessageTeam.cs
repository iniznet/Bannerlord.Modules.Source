using System;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromClient
{
	// Token: 0x02000006 RID: 6
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromClient)]
	public sealed class PlayerMessageTeam : GameNetworkMessage
	{
		// Token: 0x17000004 RID: 4
		// (get) Token: 0x06000011 RID: 17 RVA: 0x000021A9 File Offset: 0x000003A9
		// (set) Token: 0x06000012 RID: 18 RVA: 0x000021B1 File Offset: 0x000003B1
		public string Message { get; private set; }

		// Token: 0x17000005 RID: 5
		// (get) Token: 0x06000013 RID: 19 RVA: 0x000021BA File Offset: 0x000003BA
		// (set) Token: 0x06000014 RID: 20 RVA: 0x000021C2 File Offset: 0x000003C2
		public List<VirtualPlayer> ReceiverList { get; private set; }

		// Token: 0x17000006 RID: 6
		// (get) Token: 0x06000015 RID: 21 RVA: 0x000021CB File Offset: 0x000003CB
		// (set) Token: 0x06000016 RID: 22 RVA: 0x000021D3 File Offset: 0x000003D3
		public bool HasReceiverList { get; private set; }

		// Token: 0x06000017 RID: 23 RVA: 0x000021DC File Offset: 0x000003DC
		public PlayerMessageTeam(string message, List<VirtualPlayer> receiverList)
		{
			this.Message = message;
			this.ReceiverList = receiverList;
			this.HasReceiverList = true;
		}

		// Token: 0x06000018 RID: 24 RVA: 0x000021F9 File Offset: 0x000003F9
		public PlayerMessageTeam(string message)
		{
			this.Message = message;
		}

		// Token: 0x06000019 RID: 25 RVA: 0x00002208 File Offset: 0x00000408
		public PlayerMessageTeam()
		{
		}

		// Token: 0x0600001A RID: 26 RVA: 0x00002210 File Offset: 0x00000410
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

		// Token: 0x0600001B RID: 27 RVA: 0x00002274 File Offset: 0x00000474
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

		// Token: 0x0600001C RID: 28 RVA: 0x000022E3 File Offset: 0x000004E3
		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.Messaging;
		}

		// Token: 0x0600001D RID: 29 RVA: 0x000022E7 File Offset: 0x000004E7
		protected override string OnGetLogFormat()
		{
			return "Receiving Player message to team: " + this.Message;
		}
	}
}
