using System;

namespace TaleWorlds.MountAndBlade.Diamond
{
	// Token: 0x020000F7 RID: 247
	[Serializable]
	public class ClanCreationPlayerData
	{
		// Token: 0x170001AD RID: 429
		// (get) Token: 0x06000458 RID: 1112 RVA: 0x00006609 File Offset: 0x00004809
		// (set) Token: 0x06000459 RID: 1113 RVA: 0x00006611 File Offset: 0x00004811
		public PlayerSessionId PlayerSessionId { get; private set; }

		// Token: 0x170001AE RID: 430
		// (get) Token: 0x0600045A RID: 1114 RVA: 0x0000661A File Offset: 0x0000481A
		// (set) Token: 0x0600045B RID: 1115 RVA: 0x00006622 File Offset: 0x00004822
		public ClanCreationAnswer ClanCreationAnswer { get; private set; }

		// Token: 0x0600045C RID: 1116 RVA: 0x0000662B File Offset: 0x0000482B
		public ClanCreationPlayerData(PlayerSessionId playerSessionId, ClanCreationAnswer answer)
		{
			this.PlayerSessionId = playerSessionId;
			this.ClanCreationAnswer = answer;
		}
	}
}
