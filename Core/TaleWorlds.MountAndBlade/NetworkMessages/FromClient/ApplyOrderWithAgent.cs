using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromClient
{
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromClient)]
	public sealed class ApplyOrderWithAgent : GameNetworkMessage
	{
		public OrderType OrderType { get; private set; }

		public Agent Agent { get; private set; }

		public ApplyOrderWithAgent(OrderType orderType, Agent agent)
		{
			this.OrderType = orderType;
			this.Agent = agent;
		}

		public ApplyOrderWithAgent()
		{
		}

		protected override bool OnRead()
		{
			bool flag = true;
			this.OrderType = (OrderType)GameNetworkMessage.ReadIntFromPacket(CompressionOrder.OrderTypeCompressionInfo, ref flag);
			this.Agent = GameNetworkMessage.ReadAgentReferenceFromPacket(ref flag, false);
			return flag;
		}

		protected override void OnWrite()
		{
			GameNetworkMessage.WriteIntToPacket((int)this.OrderType, CompressionOrder.OrderTypeCompressionInfo);
			GameNetworkMessage.WriteAgentReferenceToPacket(this.Agent);
		}

		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.AgentsDetailed | MultiplayerMessageFilter.Orders;
		}

		protected override string OnGetLogFormat()
		{
			return string.Concat(new object[]
			{
				"Apply order: ",
				this.OrderType,
				", to agent with name: ",
				this.Agent.Name,
				" and index: ",
				this.Agent.Index
			});
		}
	}
}
