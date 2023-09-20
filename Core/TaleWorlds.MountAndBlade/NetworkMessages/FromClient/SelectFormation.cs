using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromClient
{
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromClient)]
	public sealed class SelectFormation : GameNetworkMessage
	{
		public int FormationIndex { get; private set; }

		public SelectFormation(int formationIndex)
		{
			this.FormationIndex = formationIndex;
		}

		public SelectFormation()
		{
		}

		protected override bool OnRead()
		{
			bool flag = true;
			this.FormationIndex = GameNetworkMessage.ReadIntFromPacket(CompressionOrder.FormationClassCompressionInfo, ref flag);
			return flag;
		}

		protected override void OnWrite()
		{
			GameNetworkMessage.WriteIntToPacket(this.FormationIndex, CompressionOrder.FormationClassCompressionInfo);
		}

		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.Formations;
		}

		protected override string OnGetLogFormat()
		{
			return "Select Formation with ID: " + this.FormationIndex;
		}
	}
}
