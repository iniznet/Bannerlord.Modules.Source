using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromClient
{
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromClient)]
	public sealed class UnselectFormation : GameNetworkMessage
	{
		public int FormationIndex { get; private set; }

		public UnselectFormation(int formationIndex)
		{
			this.FormationIndex = formationIndex;
		}

		public UnselectFormation()
		{
		}

		protected override bool OnRead()
		{
			bool flag = true;
			this.FormationIndex = GameNetworkMessage.ReadIntFromPacket(CompressionMission.FormationClassCompressionInfo, ref flag);
			return flag;
		}

		protected override void OnWrite()
		{
			GameNetworkMessage.WriteIntToPacket(this.FormationIndex, CompressionMission.FormationClassCompressionInfo);
		}

		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.Formations;
		}

		protected override string OnGetLogFormat()
		{
			return "Deselect Formation with index: " + this.FormationIndex;
		}
	}
}
