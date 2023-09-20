using System;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	// Token: 0x0200008F RID: 143
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class SetAgentTargetPosition : GameNetworkMessage
	{
		// Token: 0x1700014A RID: 330
		// (get) Token: 0x060005C7 RID: 1479 RVA: 0x0000AC56 File Offset: 0x00008E56
		// (set) Token: 0x060005C8 RID: 1480 RVA: 0x0000AC5E File Offset: 0x00008E5E
		public Agent Agent { get; private set; }

		// Token: 0x1700014B RID: 331
		// (get) Token: 0x060005C9 RID: 1481 RVA: 0x0000AC67 File Offset: 0x00008E67
		// (set) Token: 0x060005CA RID: 1482 RVA: 0x0000AC6F File Offset: 0x00008E6F
		public Vec2 Position { get; private set; }

		// Token: 0x060005CB RID: 1483 RVA: 0x0000AC78 File Offset: 0x00008E78
		public SetAgentTargetPosition(Agent agent, ref Vec2 position)
		{
			this.Agent = agent;
			this.Position = position;
		}

		// Token: 0x060005CC RID: 1484 RVA: 0x0000AC93 File Offset: 0x00008E93
		public SetAgentTargetPosition()
		{
		}

		// Token: 0x060005CD RID: 1485 RVA: 0x0000AC9C File Offset: 0x00008E9C
		protected override bool OnRead()
		{
			bool flag = true;
			this.Agent = GameNetworkMessage.ReadAgentReferenceFromPacket(ref flag, false);
			this.Position = GameNetworkMessage.ReadVec2FromPacket(CompressionBasic.PositionCompressionInfo, ref flag);
			return flag;
		}

		// Token: 0x060005CE RID: 1486 RVA: 0x0000ACCC File Offset: 0x00008ECC
		protected override void OnWrite()
		{
			GameNetworkMessage.WriteAgentReferenceToPacket(this.Agent);
			GameNetworkMessage.WriteVec2ToPacket(this.Position, CompressionBasic.PositionCompressionInfo);
		}

		// Token: 0x060005CF RID: 1487 RVA: 0x0000ACE9 File Offset: 0x00008EE9
		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.AgentsDetailed;
		}

		// Token: 0x060005D0 RID: 1488 RVA: 0x0000ACF4 File Offset: 0x00008EF4
		protected override string OnGetLogFormat()
		{
			return string.Concat(new object[]
			{
				"Set Target Position: ",
				this.Position,
				" on Agent with name: ",
				this.Agent.Name,
				" and agent-index: ",
				this.Agent.Index
			});
		}
	}
}
