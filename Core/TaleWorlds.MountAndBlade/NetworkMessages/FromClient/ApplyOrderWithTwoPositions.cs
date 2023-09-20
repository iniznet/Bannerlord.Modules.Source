using System;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromClient
{
	// Token: 0x0200001F RID: 31
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromClient)]
	public sealed class ApplyOrderWithTwoPositions : GameNetworkMessage
	{
		// Token: 0x1700002D RID: 45
		// (get) Token: 0x060000F7 RID: 247 RVA: 0x000033BF File Offset: 0x000015BF
		// (set) Token: 0x060000F8 RID: 248 RVA: 0x000033C7 File Offset: 0x000015C7
		public OrderType OrderType { get; private set; }

		// Token: 0x1700002E RID: 46
		// (get) Token: 0x060000F9 RID: 249 RVA: 0x000033D0 File Offset: 0x000015D0
		// (set) Token: 0x060000FA RID: 250 RVA: 0x000033D8 File Offset: 0x000015D8
		public Vec3 Position1 { get; private set; }

		// Token: 0x1700002F RID: 47
		// (get) Token: 0x060000FB RID: 251 RVA: 0x000033E1 File Offset: 0x000015E1
		// (set) Token: 0x060000FC RID: 252 RVA: 0x000033E9 File Offset: 0x000015E9
		public Vec3 Position2 { get; private set; }

		// Token: 0x060000FD RID: 253 RVA: 0x000033F2 File Offset: 0x000015F2
		public ApplyOrderWithTwoPositions(OrderType orderType, Vec3 position1, Vec3 position2)
		{
			this.OrderType = orderType;
			this.Position1 = position1;
			this.Position2 = position2;
		}

		// Token: 0x060000FE RID: 254 RVA: 0x0000340F File Offset: 0x0000160F
		public ApplyOrderWithTwoPositions()
		{
		}

		// Token: 0x060000FF RID: 255 RVA: 0x00003418 File Offset: 0x00001618
		protected override bool OnRead()
		{
			bool flag = true;
			this.OrderType = (OrderType)GameNetworkMessage.ReadIntFromPacket(CompressionOrder.OrderTypeCompressionInfo, ref flag);
			this.Position1 = GameNetworkMessage.ReadVec3FromPacket(CompressionOrder.OrderPositionCompressionInfo, ref flag);
			this.Position2 = GameNetworkMessage.ReadVec3FromPacket(CompressionOrder.OrderPositionCompressionInfo, ref flag);
			return flag;
		}

		// Token: 0x06000100 RID: 256 RVA: 0x0000345E File Offset: 0x0000165E
		protected override void OnWrite()
		{
			GameNetworkMessage.WriteIntToPacket((int)this.OrderType, CompressionOrder.OrderTypeCompressionInfo);
			GameNetworkMessage.WriteVec3ToPacket(this.Position1, CompressionOrder.OrderPositionCompressionInfo);
			GameNetworkMessage.WriteVec3ToPacket(this.Position2, CompressionOrder.OrderPositionCompressionInfo);
		}

		// Token: 0x06000101 RID: 257 RVA: 0x00003490 File Offset: 0x00001690
		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.Orders;
		}

		// Token: 0x06000102 RID: 258 RVA: 0x00003498 File Offset: 0x00001698
		protected override string OnGetLogFormat()
		{
			return string.Concat(new object[] { "Apply order: ", this.OrderType, ", to position 1: ", this.Position1, " and position 2: ", this.Position2 });
		}
	}
}
