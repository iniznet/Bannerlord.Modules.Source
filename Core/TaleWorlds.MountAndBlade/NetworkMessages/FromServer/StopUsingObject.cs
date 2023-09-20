using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class StopUsingObject : GameNetworkMessage
	{
		public Agent Agent { get; private set; }

		public bool IsSuccessful { get; private set; }

		public StopUsingObject(Agent agent, bool isSuccessful)
		{
			this.Agent = agent;
			this.IsSuccessful = isSuccessful;
		}

		public StopUsingObject()
		{
		}

		protected override bool OnRead()
		{
			bool flag = true;
			this.Agent = GameNetworkMessage.ReadAgentReferenceFromPacket(ref flag, false);
			this.IsSuccessful = GameNetworkMessage.ReadBoolFromPacket(ref flag);
			return flag;
		}

		protected override void OnWrite()
		{
			GameNetworkMessage.WriteAgentReferenceToPacket(this.Agent);
			GameNetworkMessage.WriteBoolToPacket(this.IsSuccessful);
		}

		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.AgentsDetailed | MultiplayerMessageFilter.MissionObjectsDetailed;
		}

		protected override string OnGetLogFormat()
		{
			return string.Concat(new object[]
			{
				"Stop using Object on Agent with name: ",
				this.Agent.Name,
				" and agent-index: ",
				this.Agent.Index
			});
		}
	}
}
