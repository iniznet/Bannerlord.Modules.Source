using System;
using System.Collections.Generic;
using TaleWorlds.Diamond;
using TaleWorlds.MountAndBlade.Diamond;

namespace Messages.FromBattleServerManager.ToBattleServer
{
	// Token: 0x020000DA RID: 218
	[MessageDescription("BattleServerManager", "BattleServer")]
	[Serializable]
	public class NewPlayerMessage : Message
	{
		// Token: 0x17000136 RID: 310
		// (get) Token: 0x0600031C RID: 796 RVA: 0x00004328 File Offset: 0x00002528
		// (set) Token: 0x0600031D RID: 797 RVA: 0x00004330 File Offset: 0x00002530
		public PlayerBattleInfo PlayerBattleInfo { get; private set; }

		// Token: 0x17000137 RID: 311
		// (get) Token: 0x0600031E RID: 798 RVA: 0x00004339 File Offset: 0x00002539
		// (set) Token: 0x0600031F RID: 799 RVA: 0x00004341 File Offset: 0x00002541
		public PlayerData PlayerData { get; private set; }

		// Token: 0x17000138 RID: 312
		// (get) Token: 0x06000320 RID: 800 RVA: 0x0000434A File Offset: 0x0000254A
		// (set) Token: 0x06000321 RID: 801 RVA: 0x00004352 File Offset: 0x00002552
		public Guid PlayerParty { get; private set; }

		// Token: 0x17000139 RID: 313
		// (get) Token: 0x06000322 RID: 802 RVA: 0x0000435B File Offset: 0x0000255B
		// (set) Token: 0x06000323 RID: 803 RVA: 0x00004363 File Offset: 0x00002563
		public Dictionary<string, List<string>> UsedCosmetics { get; private set; }

		// Token: 0x06000324 RID: 804 RVA: 0x0000436C File Offset: 0x0000256C
		public NewPlayerMessage(PlayerData playerData, PlayerBattleInfo playerBattleInfo, Guid playerParty, Dictionary<string, List<string>> usedCosmetics)
		{
			this.PlayerBattleInfo = playerBattleInfo;
			this.PlayerData = playerData;
			this.PlayerParty = playerParty;
			this.UsedCosmetics = usedCosmetics;
		}
	}
}
