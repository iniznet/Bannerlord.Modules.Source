using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromClient
{
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromClient)]
	public sealed class ApplyOrder : GameNetworkMessage
	{
		public OrderType OrderType { get; private set; }

		public ApplyOrder(OrderType orderType)
		{
			this.OrderType = orderType;
		}

		public ApplyOrder()
		{
		}

		protected override bool OnRead()
		{
			bool flag = true;
			this.OrderType = (OrderType)GameNetworkMessage.ReadIntFromPacket(CompressionOrder.OrderTypeCompressionInfo, ref flag);
			return flag;
		}

		protected override void OnWrite()
		{
			GameNetworkMessage.WriteIntToPacket((int)this.OrderType, CompressionOrder.OrderTypeCompressionInfo);
		}

		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.Orders;
		}

		protected override string OnGetLogFormat()
		{
			return "Apply order: " + this.OrderType;
		}
	}
}
