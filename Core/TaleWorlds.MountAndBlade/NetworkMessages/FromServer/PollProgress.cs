using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class PollProgress : GameNetworkMessage
	{
		public int VotesAccepted { get; private set; }

		public int VotesRejected { get; private set; }

		public PollProgress(int votesAccepted, int votesRejected)
		{
			this.VotesAccepted = votesAccepted;
			this.VotesRejected = votesRejected;
		}

		public PollProgress()
		{
		}

		protected override bool OnRead()
		{
			bool flag = true;
			this.VotesAccepted = GameNetworkMessage.ReadIntFromPacket(CompressionBasic.PlayerCompressionInfo, ref flag);
			this.VotesRejected = GameNetworkMessage.ReadIntFromPacket(CompressionBasic.PlayerCompressionInfo, ref flag);
			return flag;
		}

		protected override void OnWrite()
		{
			GameNetworkMessage.WriteIntToPacket(this.VotesAccepted, CompressionBasic.PlayerCompressionInfo);
			GameNetworkMessage.WriteIntToPacket(this.VotesRejected, CompressionBasic.PlayerCompressionInfo);
		}

		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.Administration;
		}

		protected override string OnGetLogFormat()
		{
			return "Update on the voting progress.";
		}
	}
}
