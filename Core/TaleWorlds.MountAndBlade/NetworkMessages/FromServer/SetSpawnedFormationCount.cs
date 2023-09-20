using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class SetSpawnedFormationCount : GameNetworkMessage
	{
		public int NumOfFormationsTeamOne { get; private set; }

		public int NumOfFormationsTeamTwo { get; private set; }

		public SetSpawnedFormationCount(int numFormationsTeamOne, int numFormationsTeamTwo)
		{
			this.NumOfFormationsTeamOne = numFormationsTeamOne;
			this.NumOfFormationsTeamTwo = numFormationsTeamTwo;
		}

		public SetSpawnedFormationCount()
		{
		}

		protected override bool OnRead()
		{
			bool flag = true;
			this.NumOfFormationsTeamOne = GameNetworkMessage.ReadIntFromPacket(CompressionOrder.FormationClassCompressionInfo, ref flag);
			this.NumOfFormationsTeamTwo = GameNetworkMessage.ReadIntFromPacket(CompressionOrder.FormationClassCompressionInfo, ref flag);
			return flag;
		}

		protected override void OnWrite()
		{
			GameNetworkMessage.WriteIntToPacket(this.NumOfFormationsTeamOne, CompressionOrder.FormationClassCompressionInfo);
			GameNetworkMessage.WriteIntToPacket(this.NumOfFormationsTeamTwo, CompressionOrder.FormationClassCompressionInfo);
		}

		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.Peers;
		}

		protected override string OnGetLogFormat()
		{
			return "Syncing formation count";
		}
	}
}
