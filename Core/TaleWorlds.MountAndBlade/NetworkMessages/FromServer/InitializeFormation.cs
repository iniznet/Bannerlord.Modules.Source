using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	// Token: 0x02000080 RID: 128
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class InitializeFormation : GameNetworkMessage
	{
		// Token: 0x17000122 RID: 290
		// (get) Token: 0x0600051D RID: 1309 RVA: 0x00009CBF File Offset: 0x00007EBF
		// (set) Token: 0x0600051E RID: 1310 RVA: 0x00009CC7 File Offset: 0x00007EC7
		public int FormationIndex { get; private set; }

		// Token: 0x17000123 RID: 291
		// (get) Token: 0x0600051F RID: 1311 RVA: 0x00009CD0 File Offset: 0x00007ED0
		// (set) Token: 0x06000520 RID: 1312 RVA: 0x00009CD8 File Offset: 0x00007ED8
		public Team Team { get; private set; }

		// Token: 0x17000124 RID: 292
		// (get) Token: 0x06000521 RID: 1313 RVA: 0x00009CE1 File Offset: 0x00007EE1
		// (set) Token: 0x06000522 RID: 1314 RVA: 0x00009CE9 File Offset: 0x00007EE9
		public string BannerCode { get; private set; }

		// Token: 0x06000523 RID: 1315 RVA: 0x00009CF2 File Offset: 0x00007EF2
		public InitializeFormation(Formation formation, Team team, string bannerCode)
		{
			this.FormationIndex = (int)formation.FormationIndex;
			this.Team = team;
			this.BannerCode = ((!string.IsNullOrEmpty(bannerCode)) ? bannerCode : string.Empty);
		}

		// Token: 0x06000524 RID: 1316 RVA: 0x00009D23 File Offset: 0x00007F23
		public InitializeFormation()
		{
		}

		// Token: 0x06000525 RID: 1317 RVA: 0x00009D2C File Offset: 0x00007F2C
		protected override bool OnRead()
		{
			bool flag = true;
			this.FormationIndex = GameNetworkMessage.ReadIntFromPacket(CompressionOrder.FormationClassCompressionInfo, ref flag);
			this.Team = GameNetworkMessage.ReadTeamReferenceFromPacket(ref flag);
			this.BannerCode = GameNetworkMessage.ReadStringFromPacket(ref flag);
			return flag;
		}

		// Token: 0x06000526 RID: 1318 RVA: 0x00009D68 File Offset: 0x00007F68
		protected override void OnWrite()
		{
			GameNetworkMessage.WriteIntToPacket(this.FormationIndex, CompressionOrder.FormationClassCompressionInfo);
			GameNetworkMessage.WriteTeamReferenceToPacket(this.Team);
			GameNetworkMessage.WriteStringToPacket(this.BannerCode);
		}

		// Token: 0x06000527 RID: 1319 RVA: 0x00009D90 File Offset: 0x00007F90
		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.Peers;
		}

		// Token: 0x06000528 RID: 1320 RVA: 0x00009D94 File Offset: 0x00007F94
		protected override string OnGetLogFormat()
		{
			return string.Concat(new object[]
			{
				"Initialize formation with index: ",
				this.FormationIndex,
				", for team: ",
				this.Team.Side
			});
		}
	}
}
