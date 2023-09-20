using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromClient
{
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromClient)]
	public sealed class TeamChange : GameNetworkMessage
	{
		public bool AutoAssign { get; private set; }

		public int TeamIndex { get; private set; }

		public Team Team
		{
			get
			{
				if (Mission.Current == null || this.TeamIndex < 0)
				{
					return null;
				}
				MBTeam mbteam = new MBTeam(Mission.Current, this.TeamIndex);
				return Mission.Current.Teams.Find(mbteam);
			}
		}

		public TeamChange(bool autoAssign, Team team)
		{
			this.AutoAssign = autoAssign;
			this.TeamIndex = ((team == null) ? MBTeam.InvalidTeam.Index : team.MBTeam.Index);
		}

		public TeamChange()
		{
		}

		protected override bool OnRead()
		{
			bool flag = true;
			this.AutoAssign = GameNetworkMessage.ReadBoolFromPacket(ref flag);
			if (!this.AutoAssign)
			{
				this.TeamIndex = GameNetworkMessage.ReadIntFromPacket(CompressionMission.TeamCompressionInfo, ref flag);
			}
			return flag;
		}

		protected override void OnWrite()
		{
			GameNetworkMessage.WriteBoolToPacket(this.AutoAssign);
			if (!this.AutoAssign)
			{
				GameNetworkMessage.WriteIntToPacket(this.TeamIndex, CompressionMission.TeamCompressionInfo);
			}
		}

		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.Mission;
		}

		protected override string OnGetLogFormat()
		{
			string text = "Changed team to: ";
			Team team = this.Team;
			return text + (((team != null) ? team.ToString() : null) ?? "null");
		}
	}
}
