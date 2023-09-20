using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromClient
{
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromClient)]
	public sealed class CheerSelected : GameNetworkMessage
	{
		public int IndexOfCheer { get; private set; }

		public CheerSelected(int indexOfCheer)
		{
			this.IndexOfCheer = indexOfCheer;
		}

		public CheerSelected()
		{
		}

		protected override bool OnRead()
		{
			bool flag = true;
			this.IndexOfCheer = GameNetworkMessage.ReadIntFromPacket(CompressionMission.CheerIndexCompressionInfo, ref flag);
			return flag;
		}

		protected override void OnWrite()
		{
			GameNetworkMessage.WriteIntToPacket(this.IndexOfCheer, CompressionMission.CheerIndexCompressionInfo);
		}

		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.None;
		}

		protected override string OnGetLogFormat()
		{
			return "FromClient.CheerSelected: " + this.IndexOfCheer;
		}
	}
}
