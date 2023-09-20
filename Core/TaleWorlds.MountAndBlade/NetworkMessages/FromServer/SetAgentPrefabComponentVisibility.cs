using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	// Token: 0x0200008E RID: 142
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class SetAgentPrefabComponentVisibility : GameNetworkMessage
	{
		// Token: 0x17000147 RID: 327
		// (get) Token: 0x060005BB RID: 1467 RVA: 0x0000AB0E File Offset: 0x00008D0E
		// (set) Token: 0x060005BC RID: 1468 RVA: 0x0000AB16 File Offset: 0x00008D16
		public Agent Agent { get; private set; }

		// Token: 0x17000148 RID: 328
		// (get) Token: 0x060005BD RID: 1469 RVA: 0x0000AB1F File Offset: 0x00008D1F
		// (set) Token: 0x060005BE RID: 1470 RVA: 0x0000AB27 File Offset: 0x00008D27
		public int ComponentIndex { get; private set; }

		// Token: 0x17000149 RID: 329
		// (get) Token: 0x060005BF RID: 1471 RVA: 0x0000AB30 File Offset: 0x00008D30
		// (set) Token: 0x060005C0 RID: 1472 RVA: 0x0000AB38 File Offset: 0x00008D38
		public bool Visibility { get; private set; }

		// Token: 0x060005C1 RID: 1473 RVA: 0x0000AB41 File Offset: 0x00008D41
		public SetAgentPrefabComponentVisibility(Agent agent, int componentIndex, bool visibility)
		{
			this.Agent = agent;
			this.ComponentIndex = componentIndex;
			this.Visibility = visibility;
		}

		// Token: 0x060005C2 RID: 1474 RVA: 0x0000AB5E File Offset: 0x00008D5E
		public SetAgentPrefabComponentVisibility()
		{
		}

		// Token: 0x060005C3 RID: 1475 RVA: 0x0000AB68 File Offset: 0x00008D68
		protected override bool OnRead()
		{
			bool flag = true;
			this.Agent = GameNetworkMessage.ReadAgentReferenceFromPacket(ref flag, false);
			this.ComponentIndex = GameNetworkMessage.ReadIntFromPacket(CompressionMission.AgentPrefabComponentIndexCompressionInfo, ref flag);
			this.Visibility = GameNetworkMessage.ReadBoolFromPacket(ref flag);
			return flag;
		}

		// Token: 0x060005C4 RID: 1476 RVA: 0x0000ABA5 File Offset: 0x00008DA5
		protected override void OnWrite()
		{
			GameNetworkMessage.WriteAgentReferenceToPacket(this.Agent);
			GameNetworkMessage.WriteIntToPacket(this.ComponentIndex, CompressionMission.AgentPrefabComponentIndexCompressionInfo);
			GameNetworkMessage.WriteBoolToPacket(this.Visibility);
		}

		// Token: 0x060005C5 RID: 1477 RVA: 0x0000ABCD File Offset: 0x00008DCD
		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.AgentsDetailed;
		}

		// Token: 0x060005C6 RID: 1478 RVA: 0x0000ABD8 File Offset: 0x00008DD8
		protected override string OnGetLogFormat()
		{
			return string.Concat(new object[]
			{
				"Set Component with index: ",
				this.ComponentIndex,
				" to be ",
				this.Visibility ? "visible" : "invisible",
				" on Agent with name: ",
				this.Agent.Name,
				" and agent-index: ",
				this.Agent.Index
			});
		}
	}
}
