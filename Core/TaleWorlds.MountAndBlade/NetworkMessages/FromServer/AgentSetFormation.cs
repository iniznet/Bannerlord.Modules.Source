using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class AgentSetFormation : GameNetworkMessage
	{
		public Agent Agent { get; private set; }

		public int FormationIndex { get; private set; }

		public AgentSetFormation(Agent agent, int formationIndex)
		{
			this.Agent = agent;
			this.FormationIndex = formationIndex;
		}

		public AgentSetFormation()
		{
		}

		protected override bool OnRead()
		{
			bool flag = true;
			this.Agent = GameNetworkMessage.ReadAgentReferenceFromPacket(ref flag, false);
			this.FormationIndex = GameNetworkMessage.ReadIntFromPacket(CompressionOrder.FormationClassCompressionInfo, ref flag);
			return flag;
		}

		protected override void OnWrite()
		{
			GameNetworkMessage.WriteAgentReferenceToPacket(this.Agent);
			GameNetworkMessage.WriteIntToPacket(this.FormationIndex, CompressionOrder.FormationClassCompressionInfo);
		}

		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.Formations | MultiplayerMessageFilter.AgentsDetailed;
		}

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
