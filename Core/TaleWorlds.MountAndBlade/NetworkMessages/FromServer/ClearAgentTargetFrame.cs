using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	// Token: 0x0200006E RID: 110
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class ClearAgentTargetFrame : GameNetworkMessage
	{
		// Token: 0x170000C8 RID: 200
		// (get) Token: 0x06000400 RID: 1024 RVA: 0x00007A12 File Offset: 0x00005C12
		// (set) Token: 0x06000401 RID: 1025 RVA: 0x00007A1A File Offset: 0x00005C1A
		public Agent Agent { get; private set; }

		// Token: 0x06000402 RID: 1026 RVA: 0x00007A23 File Offset: 0x00005C23
		public ClearAgentTargetFrame(Agent agent)
		{
			this.Agent = agent;
		}

		// Token: 0x06000403 RID: 1027 RVA: 0x00007A32 File Offset: 0x00005C32
		public ClearAgentTargetFrame()
		{
		}

		// Token: 0x06000404 RID: 1028 RVA: 0x00007A3C File Offset: 0x00005C3C
		protected override bool OnRead()
		{
			bool flag = true;
			this.Agent = GameNetworkMessage.ReadAgentReferenceFromPacket(ref flag, false);
			return flag;
		}

		// Token: 0x06000405 RID: 1029 RVA: 0x00007A5A File Offset: 0x00005C5A
		protected override void OnWrite()
		{
			GameNetworkMessage.WriteAgentReferenceToPacket(this.Agent);
		}

		// Token: 0x06000406 RID: 1030 RVA: 0x00007A67 File Offset: 0x00005C67
		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.AgentsDetailed;
		}

		// Token: 0x06000407 RID: 1031 RVA: 0x00007A6F File Offset: 0x00005C6F
		protected override string OnGetLogFormat()
		{
			return string.Concat(new object[]
			{
				"Clear target frame on agent with name: ",
				this.Agent.Name,
				" and index: ",
				this.Agent.Index
			});
		}
	}
}
