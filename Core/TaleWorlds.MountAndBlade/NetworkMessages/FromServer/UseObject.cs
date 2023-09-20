using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class UseObject : GameNetworkMessage
	{
		public int AgentIndex { get; private set; }

		public MissionObjectId UsableGameObjectId { get; private set; }

		public UseObject(int agentIndex, MissionObjectId usableGameObjectId)
		{
			this.AgentIndex = agentIndex;
			this.UsableGameObjectId = usableGameObjectId;
		}

		public UseObject()
		{
		}

		protected override bool OnRead()
		{
			bool flag = true;
			this.AgentIndex = GameNetworkMessage.ReadAgentIndexFromPacket(ref flag);
			this.UsableGameObjectId = GameNetworkMessage.ReadMissionObjectIdFromPacket(ref flag);
			return flag;
		}

		protected override void OnWrite()
		{
			GameNetworkMessage.WriteAgentIndexToPacket(this.AgentIndex);
			GameNetworkMessage.WriteMissionObjectIdToPacket((this.UsableGameObjectId.Id >= 0) ? this.UsableGameObjectId : MissionObjectId.Invalid);
		}

		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.Agents | MultiplayerMessageFilter.MissionObjects;
		}

		protected override string OnGetLogFormat()
		{
			string text = "Use UsableMissionObject with ID: ";
			if (this.UsableGameObjectId != MissionObjectId.Invalid)
			{
				text += this.UsableGameObjectId;
			}
			else
			{
				text += "null";
			}
			text += " by Agent with name: ";
			if (this.AgentIndex >= 0)
			{
				text = text + "agent-index: " + this.AgentIndex;
			}
			else
			{
				text += "null";
			}
			return text;
		}
	}
}
