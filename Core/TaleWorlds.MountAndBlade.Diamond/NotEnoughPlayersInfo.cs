using System;

namespace TaleWorlds.MountAndBlade.Diamond
{
	// Token: 0x020000FB RID: 251
	[Serializable]
	public class NotEnoughPlayersInfo
	{
		// Token: 0x170001B5 RID: 437
		// (get) Token: 0x0600046E RID: 1134 RVA: 0x00006710 File Offset: 0x00004910
		// (set) Token: 0x0600046F RID: 1135 RVA: 0x00006718 File Offset: 0x00004918
		public int CurrentPlayerCount { get; private set; }

		// Token: 0x170001B6 RID: 438
		// (get) Token: 0x06000470 RID: 1136 RVA: 0x00006721 File Offset: 0x00004921
		// (set) Token: 0x06000471 RID: 1137 RVA: 0x00006729 File Offset: 0x00004929
		public int RequiredPlayerCount { get; private set; }

		// Token: 0x06000472 RID: 1138 RVA: 0x00006732 File Offset: 0x00004932
		public NotEnoughPlayersInfo(int currentPlayerCount, int requiredPlayerCount)
		{
			this.CurrentPlayerCount = currentPlayerCount;
			this.RequiredPlayerCount = requiredPlayerCount;
		}
	}
}
