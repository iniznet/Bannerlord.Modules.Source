using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	// Token: 0x020000C1 RID: 193
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.DebugFromServer)]
	internal sealed class DebugAgentScaleOnNetworkTest : GameNetworkMessage
	{
		// Token: 0x170001CF RID: 463
		// (get) Token: 0x060007FE RID: 2046 RVA: 0x0000E6C3 File Offset: 0x0000C8C3
		// (set) Token: 0x060007FF RID: 2047 RVA: 0x0000E6CB File Offset: 0x0000C8CB
		internal Agent AgentToTest { get; private set; }

		// Token: 0x170001D0 RID: 464
		// (get) Token: 0x06000800 RID: 2048 RVA: 0x0000E6D4 File Offset: 0x0000C8D4
		// (set) Token: 0x06000801 RID: 2049 RVA: 0x0000E6DC File Offset: 0x0000C8DC
		internal float ScaleToTest { get; private set; }

		// Token: 0x06000802 RID: 2050 RVA: 0x0000E6E5 File Offset: 0x0000C8E5
		public DebugAgentScaleOnNetworkTest()
		{
		}

		// Token: 0x06000803 RID: 2051 RVA: 0x0000E6ED File Offset: 0x0000C8ED
		internal DebugAgentScaleOnNetworkTest(Agent toTest, float scale)
		{
			this.AgentToTest = toTest;
			this.ScaleToTest = scale;
		}

		// Token: 0x06000804 RID: 2052 RVA: 0x0000E704 File Offset: 0x0000C904
		protected override bool OnRead()
		{
			bool flag = true;
			this.AgentToTest = GameNetworkMessage.ReadAgentReferenceFromPacket(ref flag, true);
			this.ScaleToTest = GameNetworkMessage.ReadFloatFromPacket(CompressionMission.DebugScaleValueCompressionInfo, ref flag);
			return flag;
		}

		// Token: 0x06000805 RID: 2053 RVA: 0x0000E734 File Offset: 0x0000C934
		protected override void OnWrite()
		{
			GameNetworkMessage.WriteAgentReferenceToPacket(this.AgentToTest);
			GameNetworkMessage.WriteFloatToPacket(this.ScaleToTest, CompressionMission.DebugScaleValueCompressionInfo);
		}

		// Token: 0x06000806 RID: 2054 RVA: 0x0000E751 File Offset: 0x0000C951
		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.AgentsDetailed;
		}

		// Token: 0x06000807 RID: 2055 RVA: 0x0000E759 File Offset: 0x0000C959
		protected override string OnGetLogFormat()
		{
			return "DebugAgentScaleOnNetworkTest";
		}
	}
}
