using System;

namespace TaleWorlds.MountAndBlade.Diamond
{
	// Token: 0x0200010C RID: 268
	[Flags]
	public enum Features
	{
		// Token: 0x0400024B RID: 587
		None = 0,
		// Token: 0x0400024C RID: 588
		Matchmaking = 1,
		// Token: 0x0400024D RID: 589
		CustomGame = 2,
		// Token: 0x0400024E RID: 590
		Party = 4,
		// Token: 0x0400024F RID: 591
		Clan = 8,
		// Token: 0x04000250 RID: 592
		BannerlordFriendList = 16,
		// Token: 0x04000251 RID: 593
		TextChat = 32,
		// Token: 0x04000252 RID: 594
		All = -1
	}
}
