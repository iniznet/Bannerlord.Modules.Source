using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class SetAgentPrefabComponentVisibility : GameNetworkMessage
	{
		public int AgentIndex { get; private set; }

		public int ComponentIndex { get; private set; }

		public bool Visibility { get; private set; }

		public SetAgentPrefabComponentVisibility(int agentIndex, int componentIndex, bool visibility)
		{
			this.AgentIndex = agentIndex;
			this.ComponentIndex = componentIndex;
			this.Visibility = visibility;
		}

		public SetAgentPrefabComponentVisibility()
		{
		}

		protected override bool OnRead()
		{
			bool flag = true;
			this.AgentIndex = GameNetworkMessage.ReadAgentIndexFromPacket(ref flag);
			this.ComponentIndex = GameNetworkMessage.ReadIntFromPacket(CompressionMission.AgentPrefabComponentIndexCompressionInfo, ref flag);
			this.Visibility = GameNetworkMessage.ReadBoolFromPacket(ref flag);
			return flag;
		}

		protected override void OnWrite()
		{
			GameNetworkMessage.WriteAgentIndexToPacket(this.AgentIndex);
			GameNetworkMessage.WriteIntToPacket(this.ComponentIndex, CompressionMission.AgentPrefabComponentIndexCompressionInfo);
			GameNetworkMessage.WriteBoolToPacket(this.Visibility);
		}

		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.AgentsDetailed;
		}

		protected override string OnGetLogFormat()
		{
			return string.Concat(new object[]
			{
				"Set Component with index: ",
				this.ComponentIndex,
				" to be ",
				this.Visibility ? "visible" : "invisible",
				" on Agent with agent-index: ",
				this.AgentIndex
			});
		}
	}
}
