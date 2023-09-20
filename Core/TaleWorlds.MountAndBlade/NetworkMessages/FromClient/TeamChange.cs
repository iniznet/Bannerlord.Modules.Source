using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromClient
{
	// Token: 0x02000016 RID: 22
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromClient)]
	public sealed class TeamChange : GameNetworkMessage
	{
		// Token: 0x1700001C RID: 28
		// (get) Token: 0x060000A1 RID: 161 RVA: 0x00002C43 File Offset: 0x00000E43
		// (set) Token: 0x060000A2 RID: 162 RVA: 0x00002C4B File Offset: 0x00000E4B
		public bool AutoAssign { get; private set; }

		// Token: 0x1700001D RID: 29
		// (get) Token: 0x060000A3 RID: 163 RVA: 0x00002C54 File Offset: 0x00000E54
		// (set) Token: 0x060000A4 RID: 164 RVA: 0x00002C5C File Offset: 0x00000E5C
		public int TeamIndex { get; private set; }

		// Token: 0x1700001E RID: 30
		// (get) Token: 0x060000A5 RID: 165 RVA: 0x00002C68 File Offset: 0x00000E68
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

		// Token: 0x060000A6 RID: 166 RVA: 0x00002CA9 File Offset: 0x00000EA9
		public TeamChange(bool autoAssign, Team team)
		{
			this.AutoAssign = autoAssign;
			this.TeamIndex = ((team == null) ? MBTeam.InvalidTeam.Index : team.MBTeam.Index);
		}

		// Token: 0x060000A7 RID: 167 RVA: 0x00002CD8 File Offset: 0x00000ED8
		public TeamChange()
		{
		}

		// Token: 0x060000A8 RID: 168 RVA: 0x00002CE0 File Offset: 0x00000EE0
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

		// Token: 0x060000A9 RID: 169 RVA: 0x00002D17 File Offset: 0x00000F17
		protected override void OnWrite()
		{
			GameNetworkMessage.WriteBoolToPacket(this.AutoAssign);
			if (!this.AutoAssign)
			{
				GameNetworkMessage.WriteIntToPacket(this.TeamIndex, CompressionMission.TeamCompressionInfo);
			}
		}

		// Token: 0x060000AA RID: 170 RVA: 0x00002D3C File Offset: 0x00000F3C
		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.Mission;
		}

		// Token: 0x060000AB RID: 171 RVA: 0x00002D44 File Offset: 0x00000F44
		protected override string OnGetLogFormat()
		{
			string text = "Changed team to: ";
			Team team = this.Team;
			return text + (((team != null) ? team.ToString() : null) ?? "null");
		}
	}
}
