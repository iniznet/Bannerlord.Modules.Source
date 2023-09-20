using System;
using System.Collections.Generic;
using TaleWorlds.Diamond;
using TaleWorlds.MountAndBlade.Diamond.Ranked;

namespace Messages.FromLobbyServer.ToClient
{
	// Token: 0x02000003 RID: 3
	[MessageDescription("LobbyServer", "Client")]
	[Serializable]
	public class BattleOverMessage : Message
	{
		// Token: 0x17000002 RID: 2
		// (get) Token: 0x06000004 RID: 4 RVA: 0x00002068 File Offset: 0x00000268
		// (set) Token: 0x06000005 RID: 5 RVA: 0x00002070 File Offset: 0x00000270
		public int OldExperience { get; private set; }

		// Token: 0x17000003 RID: 3
		// (get) Token: 0x06000006 RID: 6 RVA: 0x00002079 File Offset: 0x00000279
		// (set) Token: 0x06000007 RID: 7 RVA: 0x00002081 File Offset: 0x00000281
		public int NewExperience { get; private set; }

		// Token: 0x17000004 RID: 4
		// (get) Token: 0x06000008 RID: 8 RVA: 0x0000208A File Offset: 0x0000028A
		// (set) Token: 0x06000009 RID: 9 RVA: 0x00002092 File Offset: 0x00000292
		public List<string> EarnedBadges { get; private set; }

		// Token: 0x17000005 RID: 5
		// (get) Token: 0x0600000A RID: 10 RVA: 0x0000209B File Offset: 0x0000029B
		// (set) Token: 0x0600000B RID: 11 RVA: 0x000020A3 File Offset: 0x000002A3
		public int GoldGained { get; private set; }

		// Token: 0x17000006 RID: 6
		// (get) Token: 0x0600000C RID: 12 RVA: 0x000020AC File Offset: 0x000002AC
		// (set) Token: 0x0600000D RID: 13 RVA: 0x000020B4 File Offset: 0x000002B4
		public RankBarInfo OldInfo { get; private set; }

		// Token: 0x17000007 RID: 7
		// (get) Token: 0x0600000E RID: 14 RVA: 0x000020BD File Offset: 0x000002BD
		// (set) Token: 0x0600000F RID: 15 RVA: 0x000020C5 File Offset: 0x000002C5
		public RankBarInfo NewInfo { get; private set; }

		// Token: 0x06000010 RID: 16 RVA: 0x000020CE File Offset: 0x000002CE
		public BattleOverMessage(int oldExperience, int newExperience, List<string> earnedBadges, int goldGained)
		{
			this.OldExperience = oldExperience;
			this.NewExperience = newExperience;
			this.EarnedBadges = earnedBadges;
			this.GoldGained = goldGained;
		}
	}
}
