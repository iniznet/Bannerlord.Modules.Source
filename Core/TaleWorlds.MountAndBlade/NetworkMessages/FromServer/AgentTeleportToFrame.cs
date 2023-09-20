using System;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	// Token: 0x02000066 RID: 102
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class AgentTeleportToFrame : GameNetworkMessage
	{
		// Token: 0x170000B3 RID: 179
		// (get) Token: 0x060003A6 RID: 934 RVA: 0x00007073 File Offset: 0x00005273
		// (set) Token: 0x060003A7 RID: 935 RVA: 0x0000707B File Offset: 0x0000527B
		public Agent Agent { get; private set; }

		// Token: 0x170000B4 RID: 180
		// (get) Token: 0x060003A8 RID: 936 RVA: 0x00007084 File Offset: 0x00005284
		// (set) Token: 0x060003A9 RID: 937 RVA: 0x0000708C File Offset: 0x0000528C
		public Vec3 Position { get; private set; }

		// Token: 0x170000B5 RID: 181
		// (get) Token: 0x060003AA RID: 938 RVA: 0x00007095 File Offset: 0x00005295
		// (set) Token: 0x060003AB RID: 939 RVA: 0x0000709D File Offset: 0x0000529D
		public Vec2 Direction { get; private set; }

		// Token: 0x060003AC RID: 940 RVA: 0x000070A6 File Offset: 0x000052A6
		public AgentTeleportToFrame(Agent agent, Vec3 position, Vec2 direction)
		{
			this.Agent = agent;
			this.Position = position;
			this.Direction = direction.Normalized();
		}

		// Token: 0x060003AD RID: 941 RVA: 0x000070C9 File Offset: 0x000052C9
		public AgentTeleportToFrame()
		{
		}

		// Token: 0x060003AE RID: 942 RVA: 0x000070D4 File Offset: 0x000052D4
		protected override bool OnRead()
		{
			bool flag = true;
			this.Agent = GameNetworkMessage.ReadAgentReferenceFromPacket(ref flag, false);
			this.Position = GameNetworkMessage.ReadVec3FromPacket(CompressionBasic.PositionCompressionInfo, ref flag);
			this.Direction = GameNetworkMessage.ReadVec2FromPacket(CompressionBasic.UnitVectorCompressionInfo, ref flag);
			return flag;
		}

		// Token: 0x060003AF RID: 943 RVA: 0x00007116 File Offset: 0x00005316
		protected override void OnWrite()
		{
			GameNetworkMessage.WriteAgentReferenceToPacket(this.Agent);
			GameNetworkMessage.WriteVec3ToPacket(this.Position, CompressionBasic.PositionCompressionInfo);
			GameNetworkMessage.WriteVec2ToPacket(this.Direction, CompressionBasic.UnitVectorCompressionInfo);
		}

		// Token: 0x060003B0 RID: 944 RVA: 0x00007143 File Offset: 0x00005343
		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.AgentsDetailed;
		}

		// Token: 0x060003B1 RID: 945 RVA: 0x0000714C File Offset: 0x0000534C
		protected override string OnGetLogFormat()
		{
			return string.Concat(new object[]
			{
				"Teleporting agent with name: ",
				this.Agent.Name,
				", and index: ",
				this.Agent.Index,
				" to frame with position: ",
				this.Position,
				" and direction: ",
				this.Direction
			});
		}
	}
}
