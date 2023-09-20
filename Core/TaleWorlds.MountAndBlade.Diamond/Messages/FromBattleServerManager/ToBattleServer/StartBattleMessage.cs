using System;
using TaleWorlds.Diamond;
using TaleWorlds.MountAndBlade.Diamond;
using TaleWorlds.PlayerServices;

namespace Messages.FromBattleServerManager.ToBattleServer
{
	// Token: 0x020000DE RID: 222
	[MessageDescription("BattleServerManager", "BattleServer")]
	[Serializable]
	public class StartBattleMessage : Message
	{
		// Token: 0x1700013D RID: 317
		// (get) Token: 0x0600032E RID: 814 RVA: 0x000043F1 File Offset: 0x000025F1
		// (set) Token: 0x0600032F RID: 815 RVA: 0x000043F9 File Offset: 0x000025F9
		public string SceneName { get; private set; }

		// Token: 0x1700013E RID: 318
		// (get) Token: 0x06000330 RID: 816 RVA: 0x00004402 File Offset: 0x00002602
		// (set) Token: 0x06000331 RID: 817 RVA: 0x0000440A File Offset: 0x0000260A
		public string GameType { get; private set; }

		// Token: 0x1700013F RID: 319
		// (get) Token: 0x06000332 RID: 818 RVA: 0x00004413 File Offset: 0x00002613
		// (set) Token: 0x06000333 RID: 819 RVA: 0x0000441B File Offset: 0x0000261B
		public Guid BattleId { get; private set; }

		// Token: 0x17000140 RID: 320
		// (get) Token: 0x06000334 RID: 820 RVA: 0x00004424 File Offset: 0x00002624
		// (set) Token: 0x06000335 RID: 821 RVA: 0x0000442C File Offset: 0x0000262C
		public string Faction1 { get; private set; }

		// Token: 0x17000141 RID: 321
		// (get) Token: 0x06000336 RID: 822 RVA: 0x00004435 File Offset: 0x00002635
		// (set) Token: 0x06000337 RID: 823 RVA: 0x0000443D File Offset: 0x0000263D
		public string Faction2 { get; private set; }

		// Token: 0x17000142 RID: 322
		// (get) Token: 0x06000338 RID: 824 RVA: 0x00004446 File Offset: 0x00002646
		// (set) Token: 0x06000339 RID: 825 RVA: 0x0000444E File Offset: 0x0000264E
		public int MinRequiredPlayerCountToStartBattle { get; private set; }

		// Token: 0x17000143 RID: 323
		// (get) Token: 0x0600033A RID: 826 RVA: 0x00004457 File Offset: 0x00002657
		// (set) Token: 0x0600033B RID: 827 RVA: 0x0000445F File Offset: 0x0000265F
		public int BattleSize { get; private set; }

		// Token: 0x17000144 RID: 324
		// (get) Token: 0x0600033C RID: 828 RVA: 0x00004468 File Offset: 0x00002668
		// (set) Token: 0x0600033D RID: 829 RVA: 0x00004470 File Offset: 0x00002670
		public int RoundThreshold { get; private set; }

		// Token: 0x17000145 RID: 325
		// (get) Token: 0x0600033E RID: 830 RVA: 0x00004479 File Offset: 0x00002679
		// (set) Token: 0x0600033F RID: 831 RVA: 0x00004481 File Offset: 0x00002681
		public float MoraleThreshold { get; private set; }

		// Token: 0x17000146 RID: 326
		// (get) Token: 0x06000340 RID: 832 RVA: 0x0000448A File Offset: 0x0000268A
		// (set) Token: 0x06000341 RID: 833 RVA: 0x00004492 File Offset: 0x00002692
		public bool UseAnalytics { get; private set; }

		// Token: 0x17000147 RID: 327
		// (get) Token: 0x06000342 RID: 834 RVA: 0x0000449B File Offset: 0x0000269B
		// (set) Token: 0x06000343 RID: 835 RVA: 0x000044A3 File Offset: 0x000026A3
		public bool CaptureMovementData { get; private set; }

		// Token: 0x17000148 RID: 328
		// (get) Token: 0x06000344 RID: 836 RVA: 0x000044AC File Offset: 0x000026AC
		// (set) Token: 0x06000345 RID: 837 RVA: 0x000044B4 File Offset: 0x000026B4
		public string AnalyticsServiceAddress { get; private set; }

		// Token: 0x17000149 RID: 329
		// (get) Token: 0x06000346 RID: 838 RVA: 0x000044BD File Offset: 0x000026BD
		// (set) Token: 0x06000347 RID: 839 RVA: 0x000044C5 File Offset: 0x000026C5
		public int MaxFriendlyKillCount { get; private set; }

		// Token: 0x1700014A RID: 330
		// (get) Token: 0x06000348 RID: 840 RVA: 0x000044CE File Offset: 0x000026CE
		// (set) Token: 0x06000349 RID: 841 RVA: 0x000044D6 File Offset: 0x000026D6
		public float MaxFriendlyDamage { get; private set; }

		// Token: 0x1700014B RID: 331
		// (get) Token: 0x0600034A RID: 842 RVA: 0x000044DF File Offset: 0x000026DF
		// (set) Token: 0x0600034B RID: 843 RVA: 0x000044E7 File Offset: 0x000026E7
		public float MaxFriendlyDamagePerSingleRound { get; private set; }

		// Token: 0x1700014C RID: 332
		// (get) Token: 0x0600034C RID: 844 RVA: 0x000044F0 File Offset: 0x000026F0
		// (set) Token: 0x0600034D RID: 845 RVA: 0x000044F8 File Offset: 0x000026F8
		public float RoundFriendlyDamageLimit { get; private set; }

		// Token: 0x1700014D RID: 333
		// (get) Token: 0x0600034E RID: 846 RVA: 0x00004501 File Offset: 0x00002701
		// (set) Token: 0x0600034F RID: 847 RVA: 0x00004509 File Offset: 0x00002709
		public int MaxRoundsOverLimitCount { get; private set; }

		// Token: 0x1700014E RID: 334
		// (get) Token: 0x06000350 RID: 848 RVA: 0x00004512 File Offset: 0x00002712
		// (set) Token: 0x06000351 RID: 849 RVA: 0x0000451A File Offset: 0x0000271A
		public bool IsPremadeGame { get; private set; }

		// Token: 0x1700014F RID: 335
		// (get) Token: 0x06000352 RID: 850 RVA: 0x00004523 File Offset: 0x00002723
		// (set) Token: 0x06000353 RID: 851 RVA: 0x0000452B File Offset: 0x0000272B
		public string[] ProfanityList { get; private set; }

		// Token: 0x17000150 RID: 336
		// (get) Token: 0x06000354 RID: 852 RVA: 0x00004534 File Offset: 0x00002734
		// (set) Token: 0x06000355 RID: 853 RVA: 0x0000453C File Offset: 0x0000273C
		public PremadeGameType PremadeGameType { get; private set; }

		// Token: 0x17000151 RID: 337
		// (get) Token: 0x06000356 RID: 854 RVA: 0x00004545 File Offset: 0x00002745
		// (set) Token: 0x06000357 RID: 855 RVA: 0x0000454D File Offset: 0x0000274D
		public string[] AllowList { get; private set; }

		// Token: 0x17000152 RID: 338
		// (get) Token: 0x06000358 RID: 856 RVA: 0x00004556 File Offset: 0x00002756
		// (set) Token: 0x06000359 RID: 857 RVA: 0x0000455E File Offset: 0x0000275E
		public PlayerId[] AssignedPlayers { get; private set; }

		// Token: 0x0600035A RID: 858 RVA: 0x00004568 File Offset: 0x00002768
		public StartBattleMessage(Guid battleId, string sceneName, string gameType, string faction1, string faction2, int minRequiredPlayerCountToStartBattle, int battleSize, int roundThreshold, float moraleThreshold, bool useAnalytics, bool captureMovementData, string analyticsServiceAddress, int maxFriendlyKillCount, float maxFriendlyDamage, float maxFriendlyDamagePerSingleRound, float roundFriendlyDamageLimit, int maxRoundsOverLimitCount, bool isPremadeGame, PremadeGameType premadeGameType, string[] profanityList, string[] allowList, PlayerId[] assignedPlayers)
		{
			this.SceneName = sceneName;
			this.GameType = gameType;
			this.BattleId = battleId;
			this.Faction1 = faction1;
			this.Faction2 = faction2;
			this.MinRequiredPlayerCountToStartBattle = minRequiredPlayerCountToStartBattle;
			this.BattleSize = battleSize;
			this.UseAnalytics = useAnalytics;
			this.CaptureMovementData = captureMovementData;
			this.AnalyticsServiceAddress = analyticsServiceAddress;
			this.RoundThreshold = roundThreshold;
			this.MoraleThreshold = moraleThreshold;
			this.MaxFriendlyKillCount = maxFriendlyKillCount;
			this.MaxFriendlyDamage = maxFriendlyDamage;
			this.MaxFriendlyDamagePerSingleRound = maxFriendlyDamagePerSingleRound;
			this.RoundFriendlyDamageLimit = roundFriendlyDamageLimit;
			this.MaxRoundsOverLimitCount = maxRoundsOverLimitCount;
			this.IsPremadeGame = isPremadeGame;
			this.PremadeGameType = premadeGameType;
			this.ProfanityList = profanityList;
			this.AllowList = allowList;
			this.AssignedPlayers = assignedPlayers;
		}
	}
}
