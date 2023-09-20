using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class LossReplicationMessage : GameNetworkMessage
	{
		internal int LossValue { get; private set; }

		public LossReplicationMessage()
		{
		}

		internal LossReplicationMessage(int lossValue)
		{
			this.LossValue = lossValue;
		}

		protected override bool OnRead()
		{
			bool flag = true;
			this.LossValue = GameNetworkMessage.ReadIntFromPacket(CompressionBasic.LossValueCompressionInfo, ref flag);
			return flag;
		}

		protected override void OnWrite()
		{
			GameNetworkMessage.WriteIntToPacket(this.LossValue, CompressionBasic.LossValueCompressionInfo);
		}

		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.MissionDetailed;
		}

		protected override string OnGetLogFormat()
		{
			return "LossReplicationMessage";
		}
	}
}
