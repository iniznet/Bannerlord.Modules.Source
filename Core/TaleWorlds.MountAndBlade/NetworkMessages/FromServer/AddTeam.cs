using System;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	// Token: 0x02000063 RID: 99
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class AddTeam : GameNetworkMessage
	{
		// Token: 0x170000A8 RID: 168
		// (get) Token: 0x0600037E RID: 894 RVA: 0x00006C64 File Offset: 0x00004E64
		// (set) Token: 0x0600037F RID: 895 RVA: 0x00006C6C File Offset: 0x00004E6C
		public MBTeam Team { get; private set; }

		// Token: 0x170000A9 RID: 169
		// (get) Token: 0x06000380 RID: 896 RVA: 0x00006C75 File Offset: 0x00004E75
		// (set) Token: 0x06000381 RID: 897 RVA: 0x00006C7D File Offset: 0x00004E7D
		public BattleSideEnum Side { get; private set; }

		// Token: 0x170000AA RID: 170
		// (get) Token: 0x06000382 RID: 898 RVA: 0x00006C86 File Offset: 0x00004E86
		// (set) Token: 0x06000383 RID: 899 RVA: 0x00006C8E File Offset: 0x00004E8E
		public uint Color { get; private set; }

		// Token: 0x170000AB RID: 171
		// (get) Token: 0x06000384 RID: 900 RVA: 0x00006C97 File Offset: 0x00004E97
		// (set) Token: 0x06000385 RID: 901 RVA: 0x00006C9F File Offset: 0x00004E9F
		public uint Color2 { get; private set; }

		// Token: 0x170000AC RID: 172
		// (get) Token: 0x06000386 RID: 902 RVA: 0x00006CA8 File Offset: 0x00004EA8
		// (set) Token: 0x06000387 RID: 903 RVA: 0x00006CB0 File Offset: 0x00004EB0
		public string BannerCode { get; private set; }

		// Token: 0x170000AD RID: 173
		// (get) Token: 0x06000388 RID: 904 RVA: 0x00006CB9 File Offset: 0x00004EB9
		// (set) Token: 0x06000389 RID: 905 RVA: 0x00006CC1 File Offset: 0x00004EC1
		public bool IsPlayerGeneral { get; private set; }

		// Token: 0x170000AE RID: 174
		// (get) Token: 0x0600038A RID: 906 RVA: 0x00006CCA File Offset: 0x00004ECA
		// (set) Token: 0x0600038B RID: 907 RVA: 0x00006CD2 File Offset: 0x00004ED2
		public bool IsPlayerSergeant { get; private set; }

		// Token: 0x0600038C RID: 908 RVA: 0x00006CDC File Offset: 0x00004EDC
		public AddTeam(Team team)
		{
			this.Team = team.MBTeam;
			this.Side = team.Side;
			this.Color = team.Color;
			this.Color2 = team.Color2;
			this.BannerCode = ((team.Banner != null) ? TaleWorlds.Core.BannerCode.CreateFrom(team.Banner).Code : string.Empty);
			this.IsPlayerGeneral = team.IsPlayerGeneral;
			this.IsPlayerSergeant = team.IsPlayerSergeant;
		}

		// Token: 0x0600038D RID: 909 RVA: 0x00006D5C File Offset: 0x00004F5C
		public AddTeam()
		{
		}

		// Token: 0x0600038E RID: 910 RVA: 0x00006D64 File Offset: 0x00004F64
		protected override bool OnRead()
		{
			bool flag = true;
			this.Team = GameNetworkMessage.ReadMBTeamReferenceFromPacket(CompressionMission.TeamCompressionInfo, ref flag);
			this.Side = (BattleSideEnum)GameNetworkMessage.ReadIntFromPacket(CompressionMission.TeamSideCompressionInfo, ref flag);
			this.Color = GameNetworkMessage.ReadUintFromPacket(CompressionGeneric.ColorCompressionInfo, ref flag);
			this.Color2 = GameNetworkMessage.ReadUintFromPacket(CompressionGeneric.ColorCompressionInfo, ref flag);
			this.BannerCode = GameNetworkMessage.ReadStringFromPacket(ref flag);
			this.IsPlayerGeneral = GameNetworkMessage.ReadBoolFromPacket(ref flag);
			this.IsPlayerSergeant = GameNetworkMessage.ReadBoolFromPacket(ref flag);
			return flag;
		}

		// Token: 0x0600038F RID: 911 RVA: 0x00006DE4 File Offset: 0x00004FE4
		protected override void OnWrite()
		{
			GameNetworkMessage.WriteMBTeamReferenceToPacket(this.Team, CompressionMission.TeamCompressionInfo);
			GameNetworkMessage.WriteIntToPacket((int)this.Side, CompressionMission.TeamSideCompressionInfo);
			GameNetworkMessage.WriteUintToPacket(this.Color, CompressionGeneric.ColorCompressionInfo);
			GameNetworkMessage.WriteUintToPacket(this.Color2, CompressionGeneric.ColorCompressionInfo);
			GameNetworkMessage.WriteStringToPacket(this.BannerCode);
			GameNetworkMessage.WriteBoolToPacket(this.IsPlayerGeneral);
			GameNetworkMessage.WriteBoolToPacket(this.IsPlayerSergeant);
		}

		// Token: 0x06000390 RID: 912 RVA: 0x00006E52 File Offset: 0x00005052
		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.Mission;
		}

		// Token: 0x06000391 RID: 913 RVA: 0x00006E5A File Offset: 0x0000505A
		protected override string OnGetLogFormat()
		{
			return "Add team with side: " + this.Side;
		}
	}
}
