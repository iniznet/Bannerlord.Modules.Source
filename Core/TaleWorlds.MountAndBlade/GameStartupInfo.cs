using System;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000220 RID: 544
	public class GameStartupInfo
	{
		// Token: 0x170005F5 RID: 1525
		// (get) Token: 0x06001DF4 RID: 7668 RVA: 0x0006C9A5 File Offset: 0x0006ABA5
		// (set) Token: 0x06001DF5 RID: 7669 RVA: 0x0006C9AD File Offset: 0x0006ABAD
		public GameStartupType StartupType { get; internal set; }

		// Token: 0x170005F6 RID: 1526
		// (get) Token: 0x06001DF6 RID: 7670 RVA: 0x0006C9B6 File Offset: 0x0006ABB6
		// (set) Token: 0x06001DF7 RID: 7671 RVA: 0x0006C9BE File Offset: 0x0006ABBE
		public DedicatedServerType DedicatedServerType { get; internal set; }

		// Token: 0x170005F7 RID: 1527
		// (get) Token: 0x06001DF8 RID: 7672 RVA: 0x0006C9C7 File Offset: 0x0006ABC7
		// (set) Token: 0x06001DF9 RID: 7673 RVA: 0x0006C9CF File Offset: 0x0006ABCF
		public bool PlayerHostedDedicatedServer { get; internal set; }

		// Token: 0x170005F8 RID: 1528
		// (get) Token: 0x06001DFA RID: 7674 RVA: 0x0006C9D8 File Offset: 0x0006ABD8
		// (set) Token: 0x06001DFB RID: 7675 RVA: 0x0006C9E0 File Offset: 0x0006ABE0
		public bool IsSinglePlatformServer { get; internal set; }

		// Token: 0x170005F9 RID: 1529
		// (get) Token: 0x06001DFC RID: 7676 RVA: 0x0006C9E9 File Offset: 0x0006ABE9
		// (set) Token: 0x06001DFD RID: 7677 RVA: 0x0006C9F1 File Offset: 0x0006ABF1
		public string CustomServerHostIP { get; internal set; } = string.Empty;

		// Token: 0x170005FA RID: 1530
		// (get) Token: 0x06001DFE RID: 7678 RVA: 0x0006C9FA File Offset: 0x0006ABFA
		// (set) Token: 0x06001DFF RID: 7679 RVA: 0x0006CA02 File Offset: 0x0006AC02
		public int ServerPort { get; internal set; }

		// Token: 0x170005FB RID: 1531
		// (get) Token: 0x06001E00 RID: 7680 RVA: 0x0006CA0B File Offset: 0x0006AC0B
		// (set) Token: 0x06001E01 RID: 7681 RVA: 0x0006CA13 File Offset: 0x0006AC13
		public string ServerRegion { get; internal set; }

		// Token: 0x170005FC RID: 1532
		// (get) Token: 0x06001E02 RID: 7682 RVA: 0x0006CA1C File Offset: 0x0006AC1C
		// (set) Token: 0x06001E03 RID: 7683 RVA: 0x0006CA24 File Offset: 0x0006AC24
		public sbyte ServerPriority { get; internal set; }

		// Token: 0x170005FD RID: 1533
		// (get) Token: 0x06001E04 RID: 7684 RVA: 0x0006CA2D File Offset: 0x0006AC2D
		// (set) Token: 0x06001E05 RID: 7685 RVA: 0x0006CA35 File Offset: 0x0006AC35
		public string ServerGameMode { get; internal set; }

		// Token: 0x170005FE RID: 1534
		// (get) Token: 0x06001E06 RID: 7686 RVA: 0x0006CA3E File Offset: 0x0006AC3E
		// (set) Token: 0x06001E07 RID: 7687 RVA: 0x0006CA46 File Offset: 0x0006AC46
		public string CustomGameServerConfigFile { get; internal set; }

		// Token: 0x170005FF RID: 1535
		// (get) Token: 0x06001E08 RID: 7688 RVA: 0x0006CA4F File Offset: 0x0006AC4F
		// (set) Token: 0x06001E09 RID: 7689 RVA: 0x0006CA57 File Offset: 0x0006AC57
		public string CustomGameServerAuthToken { get; internal set; }

		// Token: 0x17000600 RID: 1536
		// (get) Token: 0x06001E0A RID: 7690 RVA: 0x0006CA60 File Offset: 0x0006AC60
		// (set) Token: 0x06001E0B RID: 7691 RVA: 0x0006CA68 File Offset: 0x0006AC68
		public bool CustomGameServerAllowsOptionalModules { get; internal set; } = true;

		// Token: 0x17000601 RID: 1537
		// (get) Token: 0x06001E0C RID: 7692 RVA: 0x0006CA71 File Offset: 0x0006AC71
		// (set) Token: 0x06001E0D RID: 7693 RVA: 0x0006CA79 File Offset: 0x0006AC79
		public string OverridenUserName { get; internal set; }

		// Token: 0x17000602 RID: 1538
		// (get) Token: 0x06001E0E RID: 7694 RVA: 0x0006CA82 File Offset: 0x0006AC82
		// (set) Token: 0x06001E0F RID: 7695 RVA: 0x0006CA8A File Offset: 0x0006AC8A
		public string PremadeGameType { get; internal set; }

		// Token: 0x17000603 RID: 1539
		// (get) Token: 0x06001E10 RID: 7696 RVA: 0x0006CA93 File Offset: 0x0006AC93
		// (set) Token: 0x06001E11 RID: 7697 RVA: 0x0006CA9B File Offset: 0x0006AC9B
		public int Permission { get; internal set; }

		// Token: 0x17000604 RID: 1540
		// (get) Token: 0x06001E12 RID: 7698 RVA: 0x0006CAA4 File Offset: 0x0006ACA4
		// (set) Token: 0x06001E13 RID: 7699 RVA: 0x0006CAAC File Offset: 0x0006ACAC
		public string EpicExchangeCode { get; internal set; }

		// Token: 0x17000605 RID: 1541
		// (get) Token: 0x06001E14 RID: 7700 RVA: 0x0006CAB5 File Offset: 0x0006ACB5
		// (set) Token: 0x06001E15 RID: 7701 RVA: 0x0006CABD File Offset: 0x0006ACBD
		public bool IsContinueGame { get; internal set; }
	}
}
