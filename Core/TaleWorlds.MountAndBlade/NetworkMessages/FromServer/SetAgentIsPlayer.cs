using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class SetAgentIsPlayer : GameNetworkMessage
	{
		public Agent Agent { get; private set; }

		public bool IsPlayer { get; private set; }

		public SetAgentIsPlayer(Agent agent, bool isPlayer)
		{
			this.Agent = agent;
			this.IsPlayer = isPlayer;
		}

		public SetAgentIsPlayer()
		{
		}

		protected override bool OnRead()
		{
			bool flag = true;
			this.Agent = GameNetworkMessage.ReadAgentReferenceFromPacket(ref flag, false);
			this.IsPlayer = GameNetworkMessage.ReadBoolFromPacket(ref flag);
			return flag;
		}

		protected override void OnWrite()
		{
			GameNetworkMessage.WriteAgentReferenceToPacket(this.Agent);
			GameNetworkMessage.WriteBoolToPacket(this.IsPlayer);
		}

		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.AgentsDetailed;
		}

		protected override string OnGetLogFormat()
		{
			return string.Concat(new object[]
			{
				"Set Controller is player on Agent with name: ",
				this.Agent.Name,
				" and agent-index: ",
				this.Agent.Index,
				this.IsPlayer ? " - TRUE." : " - FALSE."
			});
		}
	}
}
