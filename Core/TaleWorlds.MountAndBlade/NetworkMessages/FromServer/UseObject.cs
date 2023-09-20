using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class UseObject : GameNetworkMessage
	{
		public Agent Agent { get; private set; }

		public UsableMissionObject UsableGameObject { get; private set; }

		public UseObject(Agent agent, UsableMissionObject usableGameObject)
		{
			this.Agent = agent;
			this.UsableGameObject = usableGameObject;
		}

		public UseObject()
		{
		}

		protected override bool OnRead()
		{
			bool flag = true;
			this.Agent = GameNetworkMessage.ReadAgentReferenceFromPacket(ref flag, false);
			this.UsableGameObject = GameNetworkMessage.ReadMissionObjectReferenceFromPacket(ref flag) as UsableMissionObject;
			return flag;
		}

		protected override void OnWrite()
		{
			GameNetworkMessage.WriteAgentReferenceToPacket(this.Agent);
			GameNetworkMessage.WriteMissionObjectReferenceToPacket(this.UsableGameObject);
		}

		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.Agents | MultiplayerMessageFilter.MissionObjects;
		}

		protected override string OnGetLogFormat()
		{
			string text = "Use UsableMissionObject with ID: ";
			if (this.UsableGameObject != null)
			{
				text += this.UsableGameObject.Id;
				text += "and name: ";
				if (this.UsableGameObject.GameEntity != null)
				{
					text += this.UsableGameObject.GameEntity.Name;
				}
				else
				{
					text += "log";
				}
			}
			else
			{
				text += "null";
			}
			text += " by Agent with name: ";
			if (this.Agent != null)
			{
				text = string.Concat(new object[]
				{
					text,
					this.Agent.Name,
					" and agent-index: ",
					this.Agent.Index
				});
			}
			else
			{
				text += "null";
			}
			return text;
		}
	}
}
