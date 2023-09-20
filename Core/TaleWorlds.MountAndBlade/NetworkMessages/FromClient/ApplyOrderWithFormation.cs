using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromClient
{
	// Token: 0x0200001A RID: 26
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromClient)]
	public sealed class ApplyOrderWithFormation : GameNetworkMessage
	{
		// Token: 0x17000022 RID: 34
		// (get) Token: 0x060000C3 RID: 195 RVA: 0x00002EFB File Offset: 0x000010FB
		// (set) Token: 0x060000C4 RID: 196 RVA: 0x00002F03 File Offset: 0x00001103
		public OrderType OrderType { get; private set; }

		// Token: 0x17000023 RID: 35
		// (get) Token: 0x060000C5 RID: 197 RVA: 0x00002F0C File Offset: 0x0000110C
		// (set) Token: 0x060000C6 RID: 198 RVA: 0x00002F14 File Offset: 0x00001114
		public int FormationIndex { get; private set; }

		// Token: 0x060000C7 RID: 199 RVA: 0x00002F1D File Offset: 0x0000111D
		public ApplyOrderWithFormation(OrderType orderType, int formationIndex)
		{
			this.OrderType = orderType;
			this.FormationIndex = formationIndex;
		}

		// Token: 0x060000C8 RID: 200 RVA: 0x00002F33 File Offset: 0x00001133
		public ApplyOrderWithFormation()
		{
		}

		// Token: 0x060000C9 RID: 201 RVA: 0x00002F3C File Offset: 0x0000113C
		protected override bool OnRead()
		{
			bool flag = true;
			this.OrderType = (OrderType)GameNetworkMessage.ReadIntFromPacket(CompressionOrder.OrderTypeCompressionInfo, ref flag);
			this.FormationIndex = GameNetworkMessage.ReadIntFromPacket(CompressionOrder.FormationClassCompressionInfo, ref flag);
			return flag;
		}

		// Token: 0x060000CA RID: 202 RVA: 0x00002F70 File Offset: 0x00001170
		protected override void OnWrite()
		{
			GameNetworkMessage.WriteIntToPacket((int)this.OrderType, CompressionOrder.OrderTypeCompressionInfo);
			GameNetworkMessage.WriteIntToPacket(this.FormationIndex, CompressionOrder.FormationClassCompressionInfo);
		}

		// Token: 0x060000CB RID: 203 RVA: 0x00002F92 File Offset: 0x00001192
		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.Formations | MultiplayerMessageFilter.Orders;
		}

		// Token: 0x060000CC RID: 204 RVA: 0x00002F9A File Offset: 0x0000119A
		protected override string OnGetLogFormat()
		{
			return string.Concat(new object[] { "Apply order: ", this.OrderType, ", to formation with index: ", this.FormationIndex });
		}
	}
}
