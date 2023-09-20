using System;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromClient
{
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromClient)]
	public sealed class ApplyOrderWithPosition : GameNetworkMessage
	{
		public OrderType OrderType { get; private set; }

		public Vec3 Position { get; private set; }

		public ApplyOrderWithPosition(OrderType orderType, Vec3 position)
		{
			this.OrderType = orderType;
			this.Position = position;
		}

		public ApplyOrderWithPosition()
		{
		}

		protected override bool OnRead()
		{
			bool flag = true;
			this.OrderType = (OrderType)GameNetworkMessage.ReadIntFromPacket(CompressionOrder.OrderTypeCompressionInfo, ref flag);
			this.Position = GameNetworkMessage.ReadVec3FromPacket(CompressionOrder.OrderPositionCompressionInfo, ref flag);
			return flag;
		}

		protected override void OnWrite()
		{
			GameNetworkMessage.WriteIntToPacket((int)this.OrderType, CompressionOrder.OrderTypeCompressionInfo);
			GameNetworkMessage.WriteVec3ToPacket(this.Position, CompressionOrder.OrderPositionCompressionInfo);
		}

		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.Orders;
		}

		protected override string OnGetLogFormat()
		{
			return string.Concat(new object[] { "Apply order: ", this.OrderType, ", to position: ", this.Position });
		}
	}
}
