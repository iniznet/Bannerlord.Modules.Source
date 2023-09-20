using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	// Token: 0x0200008B RID: 139
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class SetAgentHealth : GameNetworkMessage
	{
		// Token: 0x17000141 RID: 321
		// (get) Token: 0x0600059D RID: 1437 RVA: 0x0000A833 File Offset: 0x00008A33
		// (set) Token: 0x0600059E RID: 1438 RVA: 0x0000A83B File Offset: 0x00008A3B
		public Agent Agent { get; private set; }

		// Token: 0x17000142 RID: 322
		// (get) Token: 0x0600059F RID: 1439 RVA: 0x0000A844 File Offset: 0x00008A44
		// (set) Token: 0x060005A0 RID: 1440 RVA: 0x0000A84C File Offset: 0x00008A4C
		public int Health { get; private set; }

		// Token: 0x060005A1 RID: 1441 RVA: 0x0000A855 File Offset: 0x00008A55
		public SetAgentHealth(Agent agent, int newHealth)
		{
			this.Agent = agent;
			this.Health = newHealth;
		}

		// Token: 0x060005A2 RID: 1442 RVA: 0x0000A86B File Offset: 0x00008A6B
		public SetAgentHealth()
		{
		}

		// Token: 0x060005A3 RID: 1443 RVA: 0x0000A874 File Offset: 0x00008A74
		protected override bool OnRead()
		{
			bool flag = true;
			this.Agent = GameNetworkMessage.ReadAgentReferenceFromPacket(ref flag, false);
			this.Health = GameNetworkMessage.ReadIntFromPacket(CompressionMission.AgentHealthCompressionInfo, ref flag);
			return flag;
		}

		// Token: 0x060005A4 RID: 1444 RVA: 0x0000A8A4 File Offset: 0x00008AA4
		protected override void OnWrite()
		{
			GameNetworkMessage.WriteAgentReferenceToPacket(this.Agent);
			GameNetworkMessage.WriteIntToPacket(this.Health, CompressionMission.AgentHealthCompressionInfo);
		}

		// Token: 0x060005A5 RID: 1445 RVA: 0x0000A8C1 File Offset: 0x00008AC1
		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.AgentsDetailed;
		}

		// Token: 0x060005A6 RID: 1446 RVA: 0x0000A8CC File Offset: 0x00008ACC
		protected override string OnGetLogFormat()
		{
			return string.Concat(new object[]
			{
				"Set agent health to: ",
				this.Health,
				", for ",
				(!this.Agent.IsMount) ? " YOU" : " YOUR MOUNT"
			});
		}
	}
}
