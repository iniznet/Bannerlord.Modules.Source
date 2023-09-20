using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class MakeAgentDead : GameNetworkMessage
	{
		public Agent Agent { get; private set; }

		public bool IsKilled { get; private set; }

		public ActionIndexValueCache ActionCodeIndex { get; private set; }

		public MakeAgentDead(Agent agent, bool isKilled, ActionIndexValueCache actionCodeIndex)
		{
			this.Agent = agent;
			this.IsKilled = isKilled;
			this.ActionCodeIndex = actionCodeIndex;
		}

		public MakeAgentDead()
		{
		}

		protected override bool OnRead()
		{
			bool flag = true;
			this.Agent = GameNetworkMessage.ReadAgentReferenceFromPacket(ref flag, false);
			this.IsKilled = GameNetworkMessage.ReadBoolFromPacket(ref flag);
			this.ActionCodeIndex = new ActionIndexValueCache(GameNetworkMessage.ReadIntFromPacket(CompressionBasic.ActionCodeCompressionInfo, ref flag));
			return flag;
		}

		protected override void OnWrite()
		{
			GameNetworkMessage.WriteAgentReferenceToPacket(this.Agent);
			GameNetworkMessage.WriteBoolToPacket(this.IsKilled);
			GameNetworkMessage.WriteIntToPacket(this.ActionCodeIndex.Index, CompressionBasic.ActionCodeCompressionInfo);
		}

		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.EquipmentDetailed;
		}

		protected override string OnGetLogFormat()
		{
			return string.Concat(new object[]
			{
				"Make Agent Dead on Agent with name: ",
				this.Agent.Name,
				" and agent-index: ",
				this.Agent.Index
			});
		}
	}
}
