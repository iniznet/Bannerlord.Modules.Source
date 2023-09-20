using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class TeamSetIsEnemyOf : GameNetworkMessage
	{
		public int Team1Index { get; private set; }

		public int Team2Index { get; private set; }

		public bool IsEnemyOf { get; private set; }

		public TeamSetIsEnemyOf(int team1Index, int team2Index, bool isEnemyOf)
		{
			this.Team1Index = team1Index;
			this.Team2Index = team2Index;
			this.IsEnemyOf = isEnemyOf;
		}

		public TeamSetIsEnemyOf()
		{
		}

		protected override void OnWrite()
		{
			GameNetworkMessage.WriteTeamIndexToPacket(this.Team1Index);
			GameNetworkMessage.WriteTeamIndexToPacket(this.Team2Index);
			GameNetworkMessage.WriteBoolToPacket(this.IsEnemyOf);
		}

		protected override bool OnRead()
		{
			bool flag = true;
			this.Team1Index = GameNetworkMessage.ReadTeamIndexFromPacket(ref flag);
			this.Team2Index = GameNetworkMessage.ReadTeamIndexFromPacket(ref flag);
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
				this.Team1Index,
				" is now ",
				this.IsEnemyOf ? "" : "not an ",
				"enemy of ",
				this.Team2Index
			});
		}
	}
}
