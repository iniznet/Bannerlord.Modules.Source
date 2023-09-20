using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromClient
{
	// Token: 0x02000019 RID: 25
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromClient)]
	public sealed class ApplyOrderWithAgent : GameNetworkMessage
	{
		// Token: 0x17000020 RID: 32
		// (get) Token: 0x060000B9 RID: 185 RVA: 0x00002E03 File Offset: 0x00001003
		// (set) Token: 0x060000BA RID: 186 RVA: 0x00002E0B File Offset: 0x0000100B
		public OrderType OrderType { get; private set; }

		// Token: 0x17000021 RID: 33
		// (get) Token: 0x060000BB RID: 187 RVA: 0x00002E14 File Offset: 0x00001014
		// (set) Token: 0x060000BC RID: 188 RVA: 0x00002E1C File Offset: 0x0000101C
		public Agent Agent { get; private set; }

		// Token: 0x060000BD RID: 189 RVA: 0x00002E25 File Offset: 0x00001025
		public ApplyOrderWithAgent(OrderType orderType, Agent agent)
		{
			this.OrderType = orderType;
			this.Agent = agent;
		}

		// Token: 0x060000BE RID: 190 RVA: 0x00002E3B File Offset: 0x0000103B
		public ApplyOrderWithAgent()
		{
		}

		// Token: 0x060000BF RID: 191 RVA: 0x00002E44 File Offset: 0x00001044
		protected override bool OnRead()
		{
			bool flag = true;
			this.OrderType = (OrderType)GameNetworkMessage.ReadIntFromPacket(CompressionOrder.OrderTypeCompressionInfo, ref flag);
			this.Agent = GameNetworkMessage.ReadAgentReferenceFromPacket(ref flag, false);
			return flag;
		}

		// Token: 0x060000C0 RID: 192 RVA: 0x00002E74 File Offset: 0x00001074
		protected override void OnWrite()
		{
			GameNetworkMessage.WriteIntToPacket((int)this.OrderType, CompressionOrder.OrderTypeCompressionInfo);
			GameNetworkMessage.WriteAgentReferenceToPacket(this.Agent);
		}

		// Token: 0x060000C1 RID: 193 RVA: 0x00002E91 File Offset: 0x00001091
		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.AgentsDetailed | MultiplayerMessageFilter.Orders;
		}

		// Token: 0x060000C2 RID: 194 RVA: 0x00002E9C File Offset: 0x0000109C
		protected override string OnGetLogFormat()
		{
			return string.Concat(new object[]
			{
				"Apply order: ",
				this.OrderType,
				", to agent with name: ",
				this.Agent.Name,
				" and index: ",
				this.Agent.Index
			});
		}
	}
}
