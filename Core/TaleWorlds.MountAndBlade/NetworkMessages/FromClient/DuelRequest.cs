using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromClient
{
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromClient)]
	public sealed class DuelRequest : GameNetworkMessage
	{
		public Agent RequestedAgent { get; private set; }

		public DuelRequest(Agent requestedAgent)
		{
			this.RequestedAgent = requestedAgent;
		}

		public DuelRequest()
		{
		}

		protected override bool OnRead()
		{
			bool flag = true;
			this.RequestedAgent = GameNetworkMessage.ReadAgentReferenceFromPacket(ref flag, false);
			return flag;
		}

		protected override void OnWrite()
		{
			GameNetworkMessage.WriteAgentReferenceToPacket(this.RequestedAgent);
		}

		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.GameMode;
		}

		protected override string OnGetLogFormat()
		{
			return string.Concat(new object[]
			{
				"Duel requested from agent with name: ",
				this.RequestedAgent.Name,
				" and index: ",
				this.RequestedAgent.Index
			});
		}
	}
}
