using System;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromClient
{
	// Token: 0x0200001E RID: 30
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromClient)]
	public sealed class ApplyOrderWithPosition : GameNetworkMessage
	{
		// Token: 0x1700002B RID: 43
		// (get) Token: 0x060000ED RID: 237 RVA: 0x000032E6 File Offset: 0x000014E6
		// (set) Token: 0x060000EE RID: 238 RVA: 0x000032EE File Offset: 0x000014EE
		public OrderType OrderType { get; private set; }

		// Token: 0x1700002C RID: 44
		// (get) Token: 0x060000EF RID: 239 RVA: 0x000032F7 File Offset: 0x000014F7
		// (set) Token: 0x060000F0 RID: 240 RVA: 0x000032FF File Offset: 0x000014FF
		public Vec3 Position { get; private set; }

		// Token: 0x060000F1 RID: 241 RVA: 0x00003308 File Offset: 0x00001508
		public ApplyOrderWithPosition(OrderType orderType, Vec3 position)
		{
			this.OrderType = orderType;
			this.Position = position;
		}

		// Token: 0x060000F2 RID: 242 RVA: 0x0000331E File Offset: 0x0000151E
		public ApplyOrderWithPosition()
		{
		}

		// Token: 0x060000F3 RID: 243 RVA: 0x00003328 File Offset: 0x00001528
		protected override bool OnRead()
		{
			bool flag = true;
			this.OrderType = (OrderType)GameNetworkMessage.ReadIntFromPacket(CompressionOrder.OrderTypeCompressionInfo, ref flag);
			this.Position = GameNetworkMessage.ReadVec3FromPacket(CompressionOrder.OrderPositionCompressionInfo, ref flag);
			return flag;
		}

		// Token: 0x060000F4 RID: 244 RVA: 0x0000335C File Offset: 0x0000155C
		protected override void OnWrite()
		{
			GameNetworkMessage.WriteIntToPacket((int)this.OrderType, CompressionOrder.OrderTypeCompressionInfo);
			GameNetworkMessage.WriteVec3ToPacket(this.Position, CompressionOrder.OrderPositionCompressionInfo);
		}

		// Token: 0x060000F5 RID: 245 RVA: 0x0000337E File Offset: 0x0000157E
		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.Orders;
		}

		// Token: 0x060000F6 RID: 246 RVA: 0x00003386 File Offset: 0x00001586
		protected override string OnGetLogFormat()
		{
			return string.Concat(new object[] { "Apply order: ", this.OrderType, ", to position: ", this.Position });
		}
	}
}
