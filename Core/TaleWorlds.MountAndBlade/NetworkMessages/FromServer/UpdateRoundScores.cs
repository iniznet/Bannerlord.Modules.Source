using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class UpdateRoundScores : GameNetworkMessage
	{
		public int AttackerTeamScore { get; private set; }

		public int DefenderTeamScore { get; private set; }

		public UpdateRoundScores(int attackerTeamScore, int defenderTeamScore)
		{
			this.AttackerTeamScore = attackerTeamScore;
			this.DefenderTeamScore = defenderTeamScore;
		}

		public UpdateRoundScores()
		{
		}

		protected override bool OnRead()
		{
			bool flag = true;
			this.AttackerTeamScore = GameNetworkMessage.ReadIntFromPacket(CompressionMission.TeamScoreCompressionInfo, ref flag);
			this.DefenderTeamScore = GameNetworkMessage.ReadIntFromPacket(CompressionMission.TeamScoreCompressionInfo, ref flag);
			return flag;
		}

		protected override void OnWrite()
		{
			GameNetworkMessage.WriteIntToPacket(this.AttackerTeamScore, CompressionMission.TeamScoreCompressionInfo);
			GameNetworkMessage.WriteIntToPacket(this.DefenderTeamScore, CompressionMission.TeamScoreCompressionInfo);
		}

		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.Mission | MultiplayerMessageFilter.GameMode;
		}

		protected override string OnGetLogFormat()
		{
			return string.Concat(new object[] { "Update round score. Attackers: ", this.AttackerTeamScore, ", defenders: ", this.DefenderTeamScore });
		}
	}
}
