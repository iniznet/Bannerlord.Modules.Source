using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromClient
{
	// Token: 0x0200001B RID: 27
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromClient)]
	public sealed class ApplyOrderWithFormationAndNumber : GameNetworkMessage
	{
		// Token: 0x17000024 RID: 36
		// (get) Token: 0x060000CD RID: 205 RVA: 0x00002FD3 File Offset: 0x000011D3
		// (set) Token: 0x060000CE RID: 206 RVA: 0x00002FDB File Offset: 0x000011DB
		public OrderType OrderType { get; private set; }

		// Token: 0x17000025 RID: 37
		// (get) Token: 0x060000CF RID: 207 RVA: 0x00002FE4 File Offset: 0x000011E4
		// (set) Token: 0x060000D0 RID: 208 RVA: 0x00002FEC File Offset: 0x000011EC
		public int FormationIndex { get; private set; }

		// Token: 0x17000026 RID: 38
		// (get) Token: 0x060000D1 RID: 209 RVA: 0x00002FF5 File Offset: 0x000011F5
		// (set) Token: 0x060000D2 RID: 210 RVA: 0x00002FFD File Offset: 0x000011FD
		public int Number { get; private set; }

		// Token: 0x060000D3 RID: 211 RVA: 0x00003006 File Offset: 0x00001206
		public ApplyOrderWithFormationAndNumber(OrderType orderType, int formationIndex, int number)
		{
			this.OrderType = orderType;
			this.FormationIndex = formationIndex;
			this.Number = number;
		}

		// Token: 0x060000D4 RID: 212 RVA: 0x00003023 File Offset: 0x00001223
		public ApplyOrderWithFormationAndNumber()
		{
		}

		// Token: 0x060000D5 RID: 213 RVA: 0x0000302C File Offset: 0x0000122C
		protected override bool OnRead()
		{
			bool flag = true;
			this.OrderType = (OrderType)GameNetworkMessage.ReadIntFromPacket(CompressionOrder.OrderTypeCompressionInfo, ref flag);
			this.FormationIndex = GameNetworkMessage.ReadIntFromPacket(CompressionOrder.FormationClassCompressionInfo, ref flag);
			this.Number = GameNetworkMessage.ReadIntFromPacket(CompressionBasic.DebugIntNonCompressionInfo, ref flag);
			return flag;
		}

		// Token: 0x060000D6 RID: 214 RVA: 0x00003072 File Offset: 0x00001272
		protected override void OnWrite()
		{
			GameNetworkMessage.WriteIntToPacket((int)this.OrderType, CompressionOrder.OrderTypeCompressionInfo);
			GameNetworkMessage.WriteIntToPacket(this.FormationIndex, CompressionOrder.FormationClassCompressionInfo);
			GameNetworkMessage.WriteIntToPacket(this.Number, CompressionBasic.DebugIntNonCompressionInfo);
		}

		// Token: 0x060000D7 RID: 215 RVA: 0x000030A4 File Offset: 0x000012A4
		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.Formations | MultiplayerMessageFilter.Orders;
		}

		// Token: 0x060000D8 RID: 216 RVA: 0x000030AC File Offset: 0x000012AC
		protected override string OnGetLogFormat()
		{
			return string.Concat(new object[] { "Apply order: ", this.OrderType, ", to formation with index: ", this.FormationIndex, " and number: ", this.Number });
		}
	}
}
