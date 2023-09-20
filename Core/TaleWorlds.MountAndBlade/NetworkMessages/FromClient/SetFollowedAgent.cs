using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromClient
{
	// Token: 0x0200002F RID: 47
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromClient)]
	public sealed class SetFollowedAgent : GameNetworkMessage
	{
		// Token: 0x1700003B RID: 59
		// (get) Token: 0x0600016D RID: 365 RVA: 0x00003AC2 File Offset: 0x00001CC2
		// (set) Token: 0x0600016E RID: 366 RVA: 0x00003ACA File Offset: 0x00001CCA
		public Agent Agent { get; private set; }

		// Token: 0x0600016F RID: 367 RVA: 0x00003AD3 File Offset: 0x00001CD3
		public SetFollowedAgent(Agent agent)
		{
			this.Agent = agent;
		}

		// Token: 0x06000170 RID: 368 RVA: 0x00003AE2 File Offset: 0x00001CE2
		public SetFollowedAgent()
		{
		}

		// Token: 0x06000171 RID: 369 RVA: 0x00003AEC File Offset: 0x00001CEC
		protected override bool OnRead()
		{
			bool flag = true;
			this.Agent = GameNetworkMessage.ReadAgentReferenceFromPacket(ref flag, true);
			return flag;
		}

		// Token: 0x06000172 RID: 370 RVA: 0x00003B0A File Offset: 0x00001D0A
		protected override void OnWrite()
		{
			GameNetworkMessage.WriteAgentReferenceToPacket(this.Agent);
		}

		// Token: 0x06000173 RID: 371 RVA: 0x00003B17 File Offset: 0x00001D17
		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.MissionDetailed;
		}

		// Token: 0x06000174 RID: 372 RVA: 0x00003B1F File Offset: 0x00001D1F
		protected override string OnGetLogFormat()
		{
			return "Peer switched spectating an agent";
		}
	}
}
