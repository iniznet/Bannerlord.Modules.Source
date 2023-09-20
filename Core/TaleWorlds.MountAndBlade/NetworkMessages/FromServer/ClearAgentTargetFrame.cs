using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class ClearAgentTargetFrame : GameNetworkMessage
	{
		public Agent Agent { get; private set; }

		public ClearAgentTargetFrame(Agent agent)
		{
			this.Agent = agent;
		}

		public ClearAgentTargetFrame()
		{
		}

		protected override bool OnRead()
		{
			bool flag = true;
			this.Agent = GameNetworkMessage.ReadAgentReferenceFromPacket(ref flag, false);
			return flag;
		}

		protected override void OnWrite()
		{
			GameNetworkMessage.WriteAgentReferenceToPacket(this.Agent);
		}

		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.AgentsDetailed;
		}

		protected override string OnGetLogFormat()
		{
			return string.Concat(new object[]
			{
				"Clear target frame on agent with name: ",
				this.Agent.Name,
				" and index: ",
				this.Agent.Index
			});
		}
	}
}
