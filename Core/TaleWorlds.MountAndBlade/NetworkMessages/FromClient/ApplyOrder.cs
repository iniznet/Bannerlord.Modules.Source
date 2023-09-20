using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromClient
{
	// Token: 0x02000018 RID: 24
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromClient)]
	public sealed class ApplyOrder : GameNetworkMessage
	{
		// Token: 0x1700001F RID: 31
		// (get) Token: 0x060000B1 RID: 177 RVA: 0x00002D87 File Offset: 0x00000F87
		// (set) Token: 0x060000B2 RID: 178 RVA: 0x00002D8F File Offset: 0x00000F8F
		public OrderType OrderType { get; private set; }

		// Token: 0x060000B3 RID: 179 RVA: 0x00002D98 File Offset: 0x00000F98
		public ApplyOrder(OrderType orderType)
		{
			this.OrderType = orderType;
		}

		// Token: 0x060000B4 RID: 180 RVA: 0x00002DA7 File Offset: 0x00000FA7
		public ApplyOrder()
		{
		}

		// Token: 0x060000B5 RID: 181 RVA: 0x00002DB0 File Offset: 0x00000FB0
		protected override bool OnRead()
		{
			bool flag = true;
			this.OrderType = (OrderType)GameNetworkMessage.ReadIntFromPacket(CompressionOrder.OrderTypeCompressionInfo, ref flag);
			return flag;
		}

		// Token: 0x060000B6 RID: 182 RVA: 0x00002DD2 File Offset: 0x00000FD2
		protected override void OnWrite()
		{
			GameNetworkMessage.WriteIntToPacket((int)this.OrderType, CompressionOrder.OrderTypeCompressionInfo);
		}

		// Token: 0x060000B7 RID: 183 RVA: 0x00002DE4 File Offset: 0x00000FE4
		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.Orders;
		}

		// Token: 0x060000B8 RID: 184 RVA: 0x00002DEC File Offset: 0x00000FEC
		protected override string OnGetLogFormat()
		{
			return "Apply order: " + this.OrderType;
		}
	}
}
