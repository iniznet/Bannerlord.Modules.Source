using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class TeamSetIsEnemyOf : GameNetworkMessage
	{
		public MBTeam Team1 { get; private set; }

		public MBTeam Team2 { get; private set; }

		public bool IsEnemyOf { get; private set; }

		public TeamSetIsEnemyOf(Team team1, Team team2, bool isEnemyOf)
		{
			this.Team1 = team1.MBTeam;
			this.Team2 = team2.MBTeam;
			this.IsEnemyOf = isEnemyOf;
		}

		public TeamSetIsEnemyOf()
		{
		}

		protected override void OnWrite()
		{
			GameNetworkMessage.WriteMBTeamReferenceToPacket(this.Team1, CompressionMission.TeamCompressionInfo);
			GameNetworkMessage.WriteMBTeamReferenceToPacket(this.Team2, CompressionMission.TeamCompressionInfo);
			GameNetworkMessage.WriteBoolToPacket(this.IsEnemyOf);
		}

		protected override bool OnRead()
		{
			bool flag = true;
			this.Team1 = GameNetworkMessage.ReadMBTeamReferenceFromPacket(CompressionMission.TeamCompressionInfo, ref flag);
			this.Team2 = GameNetworkMessage.ReadMBTeamReferenceFromPacket(CompressionMission.TeamCompressionInfo, ref flag);
			this.IsEnemyOf = GameNetworkMessage.ReadBoolFromPacket(ref flag);
			return flag;
		}

		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.Mission;
		}

		protected override string OnGetLogFormat()
		{
			return string.Concat(new object[]
			{
				this.Team1,
				" is now ",
				this.IsEnemyOf ? "" : "not an ",
				"enemy of ",
				this.Team2
			});
		}
	}
}
