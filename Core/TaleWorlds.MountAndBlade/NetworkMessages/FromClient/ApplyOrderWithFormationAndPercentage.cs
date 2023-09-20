using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromClient
{
	// Token: 0x0200001C RID: 28
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromClient)]
	public sealed class ApplyOrderWithFormationAndPercentage : GameNetworkMessage
	{
		// Token: 0x17000027 RID: 39
		// (get) Token: 0x060000D9 RID: 217 RVA: 0x00003106 File Offset: 0x00001306
		// (set) Token: 0x060000DA RID: 218 RVA: 0x0000310E File Offset: 0x0000130E
		public OrderType OrderType { get; private set; }

		// Token: 0x17000028 RID: 40
		// (get) Token: 0x060000DB RID: 219 RVA: 0x00003117 File Offset: 0x00001317
		// (set) Token: 0x060000DC RID: 220 RVA: 0x0000311F File Offset: 0x0000131F
		public int FormationIndex { get; private set; }

		// Token: 0x17000029 RID: 41
		// (get) Token: 0x060000DD RID: 221 RVA: 0x00003128 File Offset: 0x00001328
		// (set) Token: 0x060000DE RID: 222 RVA: 0x00003130 File Offset: 0x00001330
		public int Percentage { get; private set; }

		// Token: 0x060000DF RID: 223 RVA: 0x00003139 File Offset: 0x00001339
		public ApplyOrderWithFormationAndPercentage(OrderType orderType, int formationIndex, int percentage)
		{
			this.OrderType = orderType;
			this.FormationIndex = formationIndex;
			this.Percentage = percentage;
		}

		// Token: 0x060000E0 RID: 224 RVA: 0x00003156 File Offset: 0x00001356
		public ApplyOrderWithFormationAndPercentage()
		{
		}

		// Token: 0x060000E1 RID: 225 RVA: 0x00003160 File Offset: 0x00001360
		protected override bool OnRead()
		{
			bool flag = true;
			this.OrderType = (OrderType)GameNetworkMessage.ReadIntFromPacket(CompressionOrder.OrderTypeCompressionInfo, ref flag);
			this.FormationIndex = GameNetworkMessage.ReadIntFromPacket(CompressionOrder.FormationClassCompressionInfo, ref flag);
			this.Percentage = GameNetworkMessage.ReadIntFromPacket(CompressionBasic.PercentageCompressionInfo, ref flag);
			return flag;
		}

		// Token: 0x060000E2 RID: 226 RVA: 0x000031A6 File Offset: 0x000013A6
		protected override void OnWrite()
		{
			GameNetworkMessage.WriteIntToPacket((int)this.OrderType, CompressionOrder.OrderTypeCompressionInfo);
			GameNetworkMessage.WriteIntToPacket(this.FormationIndex, CompressionOrder.FormationClassCompressionInfo);
			GameNetworkMessage.WriteIntToPacket(this.Percentage, CompressionBasic.PercentageCompressionInfo);
		}

		// Token: 0x060000E3 RID: 227 RVA: 0x000031D8 File Offset: 0x000013D8
		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.Formations | MultiplayerMessageFilter.Orders;
		}

		// Token: 0x060000E4 RID: 228 RVA: 0x000031E0 File Offset: 0x000013E0
		protected override string OnGetLogFormat()
		{
			return string.Concat(new object[] { "Apply order: ", this.OrderType, ", to formation with index: ", this.FormationIndex, " and percentage: ", this.Percentage });
		}
	}
}
