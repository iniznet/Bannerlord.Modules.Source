using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class SetAgentIsPlayer : GameNetworkMessage
	{
		public int AgentIndex { get; private set; }

		public bool IsPlayer { get; private set; }

		public SetAgentIsPlayer(int agentIndex, bool isPlayer)
		{
			this.AgentIndex = agentIndex;
			this.IsPlayer = isPlayer;
		}

		public SetAgentIsPlayer()
		{
		}

		protected override bool OnRead()
		{
			bool flag = true;
			this.AgentIndex = GameNetworkMessage.ReadAgentIndexFromPacket(ref flag);
			this.IsPlayer = GameNetworkMessage.ReadBoolFromPacket(ref flag);
			return flag;
		}

		protected override void OnWrite()
		{
			GameNetworkMessage.WriteAgentIndexToPacket(this.AgentIndex);
			GameNetworkMessage.WriteBoolToPacket(this.IsPlayer);
		}

		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.AgentsDetailed;
		}

		protected override string OnGetLogFormat()
		{
			return "Set Controller is player on Agent with agent-index: " + this.AgentIndex + (this.IsPlayer ? " - TRUE." : " - FALSE.");
		}
	}
}
