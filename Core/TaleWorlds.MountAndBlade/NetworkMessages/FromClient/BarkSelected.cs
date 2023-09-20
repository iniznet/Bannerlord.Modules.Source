using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromClient
{
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromClient)]
	public sealed class BarkSelected : GameNetworkMessage
	{
		public int IndexOfBark { get; private set; }

		public BarkSelected(int indexOfBark)
		{
			this.IndexOfBark = indexOfBark;
		}

		public BarkSelected()
		{
		}

		protected override bool OnRead()
		{
			bool flag = true;
			this.IndexOfBark = GameNetworkMessage.ReadIntFromPacket(CompressionMission.BarkIndexCompressionInfo, ref flag);
			return flag;
		}

		protected override void OnWrite()
		{
			GameNetworkMessage.WriteIntToPacket(this.IndexOfBark, CompressionMission.BarkIndexCompressionInfo);
		}

		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.None;
		}

		protected override string OnGetLogFormat()
		{
			return "FromClient.BarkSelected: " + this.IndexOfBark;
		}
	}
}
