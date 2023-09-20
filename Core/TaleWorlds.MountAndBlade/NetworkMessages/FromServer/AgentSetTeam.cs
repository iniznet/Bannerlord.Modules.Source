using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class AgentSetTeam : GameNetworkMessage
	{
		public Agent Agent { get; private set; }

		public MBTeam Team { get; private set; }

		public AgentSetTeam(Agent agent, Team team)
		{
			this.Agent = agent;
			this.Team = ((team != null) ? team.MBTeam : MBTeam.InvalidTeam);
		}

		public AgentSetTeam()
		{
		}

		protected override bool OnRead()
		{
			bool flag = true;
			this.Agent = GameNetworkMessage.ReadAgentReferenceFromPacket(ref flag, false);
			this.Team = GameNetworkMessage.ReadMBTeamReferenceFromPacket(CompressionMission.TeamCompressionInfo, ref flag);
			return flag;
		}

		protected override void OnWrite()
		{
			GameNetworkMessage.WriteAgentReferenceToPacket(this.Agent);
			GameNetworkMessage.WriteMBTeamReferenceToPacket(this.Team, CompressionMission.TeamCompressionInfo);
		}

		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.Agents;
		}

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
