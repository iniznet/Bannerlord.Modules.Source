using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class StopUsingObject : GameNetworkMessage
	{
		public int AgentIndex { get; private set; }

		public bool IsSuccessful { get; private set; }

		public StopUsingObject(int agentIndex, bool isSuccessful)
		{
			this.AgentIndex = agentIndex;
			this.IsSuccessful = isSuccessful;
		}

		public StopUsingObject()
		{
		}

		protected override bool OnRead()
		{
			bool flag = true;
			this.AgentIndex = GameNetworkMessage.ReadAgentIndexFromPacket(ref flag);
			this.IsSuccessful = GameNetworkMessage.ReadBoolFromPacket(ref flag);
			return flag;
		}

		protected override void OnWrite()
		{
			GameNetworkMessage.WriteAgentIndexToPacket(this.AgentIndex);
			GameNetworkMessage.WriteBoolToPacket(this.IsSuccessful);
		}

		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.AgentsDetailed | MultiplayerMessageFilter.MissionObjectsDetailed;
		}

		protected override string OnGetLogFormat()
		{
			return "Stop using Object on Agent with agent-index: " + this.AgentIndex;
		}
	}
}
