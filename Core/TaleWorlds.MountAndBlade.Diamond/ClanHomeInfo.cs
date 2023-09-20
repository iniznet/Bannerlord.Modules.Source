using System;

namespace TaleWorlds.MountAndBlade.Diamond
{
	// Token: 0x020000FA RID: 250
	[Serializable]
	public class ClanHomeInfo
	{
		// Token: 0x170001AF RID: 431
		// (get) Token: 0x0600045D RID: 1117 RVA: 0x00006641 File Offset: 0x00004841
		// (set) Token: 0x0600045E RID: 1118 RVA: 0x00006649 File Offset: 0x00004849
		public bool IsInClan { get; private set; }

		// Token: 0x170001B0 RID: 432
		// (get) Token: 0x0600045F RID: 1119 RVA: 0x00006652 File Offset: 0x00004852
		// (set) Token: 0x06000460 RID: 1120 RVA: 0x0000665A File Offset: 0x0000485A
		public bool CanCreateClan { get; private set; }

		// Token: 0x170001B1 RID: 433
		// (get) Token: 0x06000461 RID: 1121 RVA: 0x00006663 File Offset: 0x00004863
		// (set) Token: 0x06000462 RID: 1122 RVA: 0x0000666B File Offset: 0x0000486B
		public ClanInfo ClanInfo { get; private set; }

		// Token: 0x170001B2 RID: 434
		// (get) Token: 0x06000463 RID: 1123 RVA: 0x00006674 File Offset: 0x00004874
		// (set) Token: 0x06000464 RID: 1124 RVA: 0x0000667C File Offset: 0x0000487C
		public NotEnoughPlayersInfo NotEnoughPlayersInfo { get; private set; }

		// Token: 0x170001B3 RID: 435
		// (get) Token: 0x06000465 RID: 1125 RVA: 0x00006685 File Offset: 0x00004885
		// (set) Token: 0x06000466 RID: 1126 RVA: 0x0000668D File Offset: 0x0000488D
		public PlayerNotEligibleInfo[] PlayerNotEligibleInfos { get; private set; }

		// Token: 0x170001B4 RID: 436
		// (get) Token: 0x06000467 RID: 1127 RVA: 0x00006696 File Offset: 0x00004896
		// (set) Token: 0x06000468 RID: 1128 RVA: 0x0000669E File Offset: 0x0000489E
		public ClanPlayerInfo[] ClanPlayerInfos { get; private set; }

		// Token: 0x06000469 RID: 1129 RVA: 0x000066A7 File Offset: 0x000048A7
		public ClanHomeInfo(bool isInClan, bool canCreateClan, ClanInfo clanInfo, NotEnoughPlayersInfo notEnoughPlayersInfo, PlayerNotEligibleInfo[] playerNotEligibleInfos, ClanPlayerInfo[] clanPlayerInfos)
		{
			this.IsInClan = isInClan;
			this.CanCreateClan = canCreateClan;
			this.ClanInfo = clanInfo;
			this.NotEnoughPlayersInfo = notEnoughPlayersInfo;
			this.PlayerNotEligibleInfos = playerNotEligibleInfos;
			this.ClanPlayerInfos = clanPlayerInfos;
		}

		// Token: 0x0600046A RID: 1130 RVA: 0x000066DC File Offset: 0x000048DC
		public static ClanHomeInfo CreateInClanInfo(ClanInfo clanInfo, ClanPlayerInfo[] clanPlayerInfos)
		{
			return new ClanHomeInfo(true, false, clanInfo, null, null, clanPlayerInfos);
		}

		// Token: 0x0600046B RID: 1131 RVA: 0x000066E9 File Offset: 0x000048E9
		public static ClanHomeInfo CreateCanCreateClanInfo()
		{
			return new ClanHomeInfo(false, true, null, null, null, null);
		}

		// Token: 0x0600046C RID: 1132 RVA: 0x000066F6 File Offset: 0x000048F6
		public static ClanHomeInfo CreateCantCreateClanInfo(NotEnoughPlayersInfo notEnoughPlayersInfo, PlayerNotEligibleInfo[] playerNotEligibleInfos)
		{
			return new ClanHomeInfo(false, false, null, notEnoughPlayersInfo, playerNotEligibleInfos, null);
		}

		// Token: 0x0600046D RID: 1133 RVA: 0x00006703 File Offset: 0x00004903
		public static ClanHomeInfo CreateInvalidStateClanInfo()
		{
			return new ClanHomeInfo(false, false, null, null, null, null);
		}
	}
}
