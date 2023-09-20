using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class AgentSetTeam : GameNetworkMessage
	{
		public int AgentIndex { get; private set; }

		public int TeamIndex { get; private set; }

		public AgentSetTeam(int agentIndex, int teamIndex)
		{
			this.AgentIndex = agentIndex;
			this.TeamIndex = teamIndex;
		}

		public AgentSetTeam()
		{
		}

		protected override bool OnRead()
		{
			bool flag = true;
			this.AgentIndex = GameNetworkMessage.ReadAgentIndexFromPacket(ref flag);
			this.TeamIndex = GameNetworkMessage.ReadTeamIndexFromPacket(ref flag);
			return flag;
		}

		protected override void OnWrite()
		{
			GameNetworkMessage.WriteAgentIndexToPacket(this.AgentIndex);
			GameNetworkMessage.WriteTeamIndexToPacket(this.TeamIndex);
		}

		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.Agents;
		}

		protected override string OnGetLogFormat()
		{
			return string.Concat(new object[] { "Assign agent with agent-index: ", this.AgentIndex, " to team: ", this.TeamIndex });
		}
	}
}
