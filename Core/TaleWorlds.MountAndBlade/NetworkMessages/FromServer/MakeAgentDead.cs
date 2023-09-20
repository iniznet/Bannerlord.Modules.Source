using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class MakeAgentDead : GameNetworkMessage
	{
		public int AgentIndex { get; private set; }

		public bool IsKilled { get; private set; }

		public ActionIndexValueCache ActionCodeIndex { get; private set; }

		public MakeAgentDead(int agentIndex, bool isKilled, ActionIndexValueCache actionCodeIndex)
		{
			this.AgentIndex = agentIndex;
			this.IsKilled = isKilled;
			this.ActionCodeIndex = actionCodeIndex;
		}

		public MakeAgentDead()
		{
		}

		protected override bool OnRead()
		{
			bool flag = true;
			this.AgentIndex = GameNetworkMessage.ReadAgentIndexFromPacket(ref flag);
			this.IsKilled = GameNetworkMessage.ReadBoolFromPacket(ref flag);
			this.ActionCodeIndex = new ActionIndexValueCache(GameNetworkMessage.ReadIntFromPacket(CompressionBasic.ActionCodeCompressionInfo, ref flag));
			return flag;
		}

		protected override void OnWrite()
		{
			GameNetworkMessage.WriteAgentIndexToPacket(this.AgentIndex);
			GameNetworkMessage.WriteBoolToPacket(this.IsKilled);
			GameNetworkMessage.WriteIntToPacket(this.ActionCodeIndex.Index, CompressionBasic.ActionCodeCompressionInfo);
		}

		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.EquipmentDetailed;
		}

		protected override string OnGetLogFormat()
		{
			return "Make Agent Dead on Agent with agent-index: " + this.AgentIndex;
		}
	}
}
