using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromClient
{
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromClient)]
	public sealed class ApplyOrderWithFormationAndNumber : GameNetworkMessage
	{
		public OrderType OrderType { get; private set; }

		public int FormationIndex { get; private set; }

		public int Number { get; private set; }

		public ApplyOrderWithFormationAndNumber(OrderType orderType, int formationIndex, int number)
		{
			this.OrderType = orderType;
			this.FormationIndex = formationIndex;
			this.Number = number;
		}

		public ApplyOrderWithFormationAndNumber()
		{
		}

		protected override bool OnRead()
		{
			bool flag = true;
			this.OrderType = (OrderType)GameNetworkMessage.ReadIntFromPacket(CompressionOrder.OrderTypeCompressionInfo, ref flag);
			this.FormationIndex = GameNetworkMessage.ReadIntFromPacket(CompressionOrder.FormationClassCompressionInfo, ref flag);
			this.Number = GameNetworkMessage.ReadIntFromPacket(CompressionBasic.DebugIntNonCompressionInfo, ref flag);
			return flag;
		}

		protected override void OnWrite()
		{
			GameNetworkMessage.WriteIntToPacket((int)this.OrderType, CompressionOrder.OrderTypeCompressionInfo);
			GameNetworkMessage.WriteIntToPacket(this.FormationIndex, CompressionOrder.FormationClassCompressionInfo);
			GameNetworkMessage.WriteIntToPacket(this.Number, CompressionBasic.DebugIntNonCompressionInfo);
		}

		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.Formations | MultiplayerMessageFilter.Orders;
		}

		protected override string OnGetLogFormat()
		{
			return string.Concat(new object[] { "Apply order: ", this.OrderType, ", to formation with index: ", this.FormationIndex, " and number: ", this.Number });
		}
	}
}
