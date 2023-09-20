using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	// Token: 0x02000065 RID: 101
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class AgentSetTeam : GameNetworkMessage
	{
		// Token: 0x170000B1 RID: 177
		// (get) Token: 0x0600039C RID: 924 RVA: 0x00006F6B File Offset: 0x0000516B
		// (set) Token: 0x0600039D RID: 925 RVA: 0x00006F73 File Offset: 0x00005173
		public Agent Agent { get; private set; }

		// Token: 0x170000B2 RID: 178
		// (get) Token: 0x0600039E RID: 926 RVA: 0x00006F7C File Offset: 0x0000517C
		// (set) Token: 0x0600039F RID: 927 RVA: 0x00006F84 File Offset: 0x00005184
		public MBTeam Team { get; private set; }

		// Token: 0x060003A0 RID: 928 RVA: 0x00006F8D File Offset: 0x0000518D
		public AgentSetTeam(Agent agent, Team team)
		{
			this.Agent = agent;
			this.Team = ((team != null) ? team.MBTeam : MBTeam.InvalidTeam);
		}

		// Token: 0x060003A1 RID: 929 RVA: 0x00006FB2 File Offset: 0x000051B2
		public AgentSetTeam()
		{
		}

		// Token: 0x060003A2 RID: 930 RVA: 0x00006FBC File Offset: 0x000051BC
		protected override bool OnRead()
		{
			bool flag = true;
			this.Agent = GameNetworkMessage.ReadAgentReferenceFromPacket(ref flag, false);
			this.Team = GameNetworkMessage.ReadMBTeamReferenceFromPacket(CompressionMission.TeamCompressionInfo, ref flag);
			return flag;
		}

		// Token: 0x060003A3 RID: 931 RVA: 0x00006FEC File Offset: 0x000051EC
		protected override void OnWrite()
		{
			GameNetworkMessage.WriteAgentReferenceToPacket(this.Agent);
			GameNetworkMessage.WriteMBTeamReferenceToPacket(this.Team, CompressionMission.TeamCompressionInfo);
		}

		// Token: 0x060003A4 RID: 932 RVA: 0x00007009 File Offset: 0x00005209
		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.Agents;
		}

		// Token: 0x060003A5 RID: 933 RVA: 0x00007014 File Offset: 0x00005214
		protected override string OnGetLogFormat()
		{
			return string.Concat(new object[]
			{
				"Assign agent with name: ",
				this.Agent.Name,
				", and index: ",
				this.Agent.Index,
				" to team: ",
				this.Team
			});
		}
	}
}
