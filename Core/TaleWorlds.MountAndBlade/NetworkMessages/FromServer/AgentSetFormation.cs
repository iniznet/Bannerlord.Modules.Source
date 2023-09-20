using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	// Token: 0x02000064 RID: 100
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class AgentSetFormation : GameNetworkMessage
	{
		// Token: 0x170000AF RID: 175
		// (get) Token: 0x06000392 RID: 914 RVA: 0x00006E71 File Offset: 0x00005071
		// (set) Token: 0x06000393 RID: 915 RVA: 0x00006E79 File Offset: 0x00005079
		public Agent Agent { get; private set; }

		// Token: 0x170000B0 RID: 176
		// (get) Token: 0x06000394 RID: 916 RVA: 0x00006E82 File Offset: 0x00005082
		// (set) Token: 0x06000395 RID: 917 RVA: 0x00006E8A File Offset: 0x0000508A
		public int FormationIndex { get; private set; }

		// Token: 0x06000396 RID: 918 RVA: 0x00006E93 File Offset: 0x00005093
		public AgentSetFormation(Agent agent, int formationIndex)
		{
			this.Agent = agent;
			this.FormationIndex = formationIndex;
		}

		// Token: 0x06000397 RID: 919 RVA: 0x00006EA9 File Offset: 0x000050A9
		public AgentSetFormation()
		{
		}

		// Token: 0x06000398 RID: 920 RVA: 0x00006EB4 File Offset: 0x000050B4
		protected override bool OnRead()
		{
			bool flag = true;
			this.Agent = GameNetworkMessage.ReadAgentReferenceFromPacket(ref flag, false);
			this.FormationIndex = GameNetworkMessage.ReadIntFromPacket(CompressionOrder.FormationClassCompressionInfo, ref flag);
			return flag;
		}

		// Token: 0x06000399 RID: 921 RVA: 0x00006EE4 File Offset: 0x000050E4
		protected override void OnWrite()
		{
			GameNetworkMessage.WriteAgentReferenceToPacket(this.Agent);
			GameNetworkMessage.WriteIntToPacket(this.FormationIndex, CompressionOrder.FormationClassCompressionInfo);
		}

		// Token: 0x0600039A RID: 922 RVA: 0x00006F01 File Offset: 0x00005101
		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.Formations | MultiplayerMessageFilter.AgentsDetailed;
		}

		// Token: 0x0600039B RID: 923 RVA: 0x00006F0C File Offset: 0x0000510C
		protected override string OnGetLogFormat()
		{
			return string.Concat(new object[]
			{
				"Assign agent with name: ",
				this.Agent.Name,
				", and index: ",
				this.Agent.Index,
				" to formation with index: ",
				this.FormationIndex
			});
		}
	}
}
