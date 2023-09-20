using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromClient
{
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromClient)]
	public sealed class ApplySiegeWeaponOrder : GameNetworkMessage
	{
		public SiegeWeaponOrderType OrderType { get; private set; }

		public ApplySiegeWeaponOrder(SiegeWeaponOrderType orderType)
		{
			this.OrderType = orderType;
		}

		public ApplySiegeWeaponOrder()
		{
		}

		protected override bool OnRead()
		{
			bool flag = true;
			this.OrderType = (SiegeWeaponOrderType)GameNetworkMessage.ReadIntFromPacket(CompressionOrder.OrderTypeCompressionInfo, ref flag);
			return flag;
		}

		protected override void OnWrite()
		{
			GameNetworkMessage.WriteIntToPacket((int)this.OrderType, CompressionOrder.OrderTypeCompressionInfo);
		}

		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.SiegeWeaponsDetailed | MultiplayerMessageFilter.Orders;
		}

		protected override string OnGetLogFormat()
		{
			return "Apply siege order: " + this.OrderType;
		}
	}
}
