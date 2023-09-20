using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromClient
{
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromClient)]
	public sealed class SetFollowedAgent : GameNetworkMessage
	{
		public Agent Agent { get; private set; }

		public SetFollowedAgent(Agent agent)
		{
			this.Agent = agent;
		}

		public SetFollowedAgent()
		{
		}

		protected override bool OnRead()
		{
			bool flag = true;
			this.Agent = GameNetworkMessage.ReadAgentReferenceFromPacket(ref flag, true);
			return flag;
		}

		protected override void OnWrite()
		{
			GameNetworkMessage.WriteAgentReferenceToPacket(this.Agent);
		}

		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.MissionDetailed;
		}

		protected override string OnGetLogFormat()
		{
			return "Peer switched spectating an agent";
		}
	}
}
