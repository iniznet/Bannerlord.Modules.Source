using System;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	// Token: 0x02000090 RID: 144
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class SetAgentTargetPositionAndDirection : GameNetworkMessage
	{
		// Token: 0x1700014C RID: 332
		// (get) Token: 0x060005D1 RID: 1489 RVA: 0x0000AD53 File Offset: 0x00008F53
		// (set) Token: 0x060005D2 RID: 1490 RVA: 0x0000AD5B File Offset: 0x00008F5B
		public Agent Agent { get; private set; }

		// Token: 0x1700014D RID: 333
		// (get) Token: 0x060005D3 RID: 1491 RVA: 0x0000AD64 File Offset: 0x00008F64
		// (set) Token: 0x060005D4 RID: 1492 RVA: 0x0000AD6C File Offset: 0x00008F6C
		public Vec2 Position { get; private set; }

		// Token: 0x1700014E RID: 334
		// (get) Token: 0x060005D5 RID: 1493 RVA: 0x0000AD75 File Offset: 0x00008F75
		// (set) Token: 0x060005D6 RID: 1494 RVA: 0x0000AD7D File Offset: 0x00008F7D
		public Vec3 Direction { get; private set; }

		// Token: 0x060005D7 RID: 1495 RVA: 0x0000AD86 File Offset: 0x00008F86
		public SetAgentTargetPositionAndDirection(Agent agent, ref Vec2 position, ref Vec3 direction)
		{
			this.Agent = agent;
			this.Position = position;
			this.Direction = direction;
		}

		// Token: 0x060005D8 RID: 1496 RVA: 0x0000ADAD File Offset: 0x00008FAD
		public SetAgentTargetPositionAndDirection()
		{
		}

		// Token: 0x060005D9 RID: 1497 RVA: 0x0000ADB8 File Offset: 0x00008FB8
		protected override bool OnRead()
		{
			bool flag = true;
			this.Agent = GameNetworkMessage.ReadAgentReferenceFromPacket(ref flag, false);
			this.Position = GameNetworkMessage.ReadVec2FromPacket(CompressionBasic.PositionCompressionInfo, ref flag);
			this.Direction = GameNetworkMessage.ReadVec3FromPacket(CompressionBasic.UnitVectorCompressionInfo, ref flag);
			return flag;
		}

		// Token: 0x060005DA RID: 1498 RVA: 0x0000ADFA File Offset: 0x00008FFA
		protected override void OnWrite()
		{
			GameNetworkMessage.WriteAgentReferenceToPacket(this.Agent);
			GameNetworkMessage.WriteVec2ToPacket(this.Position, CompressionBasic.PositionCompressionInfo);
			GameNetworkMessage.WriteVec3ToPacket(this.Direction, CompressionBasic.UnitVectorCompressionInfo);
		}

		// Token: 0x060005DB RID: 1499 RVA: 0x0000AE27 File Offset: 0x00009027
		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.AgentsDetailed;
		}

		// Token: 0x060005DC RID: 1500 RVA: 0x0000AE30 File Offset: 0x00009030
		protected override string OnGetLogFormat()
		{
			return string.Concat(new object[]
			{
				"Set TargetPositionAndDirection: ",
				this.Position,
				" ",
				this.Direction,
				" on Agent with name: ",
				this.Agent.Name,
				" and agent-index: ",
				this.Agent.Index
			});
		}
	}
}
