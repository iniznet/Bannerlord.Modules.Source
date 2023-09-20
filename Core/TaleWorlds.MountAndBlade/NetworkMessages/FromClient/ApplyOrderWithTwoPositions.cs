using System;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromClient
{
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromClient)]
	public sealed class ApplyOrderWithTwoPositions : GameNetworkMessage
	{
		public OrderType OrderType { get; private set; }

		public Vec3 Position1 { get; private set; }

		public Vec3 Position2 { get; private set; }

		public ApplyOrderWithTwoPositions(OrderType orderType, Vec3 position1, Vec3 position2)
		{
			this.OrderType = orderType;
			this.Position1 = position1;
			this.Position2 = position2;
		}

		public ApplyOrderWithTwoPositions()
		{
		}

		protected override bool OnRead()
		{
			bool flag = true;
			this.OrderType = (OrderType)GameNetworkMessage.ReadIntFromPacket(CompressionMission.OrderTypeCompressionInfo, ref flag);
			this.Position1 = GameNetworkMessage.ReadVec3FromPacket(CompressionMission.OrderPositionCompressionInfo, ref flag);
			this.Position2 = GameNetworkMessage.ReadVec3FromPacket(CompressionMission.OrderPositionCompressionInfo, ref flag);
			return flag;
		}

		protected override void OnWrite()
		{
			GameNetworkMessage.WriteIntToPacket((int)this.OrderType, CompressionMission.OrderTypeCompressionInfo);
			GameNetworkMessage.WriteVec3ToPacket(this.Position1, CompressionMission.OrderPositionCompressionInfo);
			GameNetworkMessage.WriteVec3ToPacket(this.Position2, CompressionMission.OrderPositionCompressionInfo);
		}

		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.Orders;
		}

		protected override string OnGetLogFormat()
		{
			return string.Concat(new object[] { "Apply order: ", this.OrderType, ", to position 1: ", this.Position1, " and position 2: ", this.Position2 });
		}
	}
}
