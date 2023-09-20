using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.MissionRepresentatives;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class DuelPointsUpdateMessage : GameNetworkMessage
	{
		public int Bounty { get; private set; }

		public int Score { get; private set; }

		public int NumberOfWins { get; private set; }

		public NetworkCommunicator NetworkCommunicator { get; private set; }

		public DuelPointsUpdateMessage()
		{
		}

		public DuelPointsUpdateMessage(DuelMissionRepresentative representative)
		{
			this.Bounty = representative.Bounty;
			this.Score = representative.Score;
			this.NumberOfWins = representative.NumberOfWins;
			this.NetworkCommunicator = representative.GetNetworkPeer();
		}

		protected override void OnWrite()
		{
			GameNetworkMessage.WriteIntToPacket(this.Bounty, CompressionMatchmaker.ScoreCompressionInfo);
			GameNetworkMessage.WriteIntToPacket(this.Score, CompressionMatchmaker.ScoreCompressionInfo);
			GameNetworkMessage.WriteIntToPacket(this.NumberOfWins, CompressionMatchmaker.KillDeathAssistCountCompressionInfo);
			GameNetworkMessage.WriteNetworkPeerReferenceToPacket(this.NetworkCommunicator);
		}

		protected override bool OnRead()
		{
			bool flag = true;
			this.Bounty = GameNetworkMessage.ReadIntFromPacket(CompressionMatchmaker.ScoreCompressionInfo, ref flag);
			this.Score = GameNetworkMessage.ReadIntFromPacket(CompressionMatchmaker.ScoreCompressionInfo, ref flag);
			this.NumberOfWins = GameNetworkMessage.ReadIntFromPacket(CompressionMatchmaker.KillDeathAssistCountCompressionInfo, ref flag);
			this.NetworkCommunicator = GameNetworkMessage.ReadNetworkPeerReferenceFromPacket(ref flag, false);
			return flag;
		}

		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.GameMode;
		}

		protected override string OnGetLogFormat()
		{
			return "PointUpdateMessage";
		}
	}
}
