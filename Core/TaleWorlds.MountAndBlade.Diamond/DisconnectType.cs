using System;

namespace TaleWorlds.MountAndBlade.Diamond
{
	// Token: 0x02000109 RID: 265
	public enum DisconnectType
	{
		// Token: 0x0400023C RID: 572
		QuitFromGame,
		// Token: 0x0400023D RID: 573
		TimedOut,
		// Token: 0x0400023E RID: 574
		KickedByHost,
		// Token: 0x0400023F RID: 575
		KickedByPoll,
		// Token: 0x04000240 RID: 576
		BannedByPoll,
		// Token: 0x04000241 RID: 577
		Inactivity,
		// Token: 0x04000242 RID: 578
		DisconnectedFromLobby,
		// Token: 0x04000243 RID: 579
		GameEnded,
		// Token: 0x04000244 RID: 580
		ServerNotResponding,
		// Token: 0x04000245 RID: 581
		KickedDueToFriendlyDamage,
		// Token: 0x04000246 RID: 582
		PlayStateMismatch,
		// Token: 0x04000247 RID: 583
		Unknown
	}
}
