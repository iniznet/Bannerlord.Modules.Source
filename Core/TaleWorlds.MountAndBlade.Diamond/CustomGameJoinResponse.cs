using System;

namespace TaleWorlds.MountAndBlade.Diamond
{
	// Token: 0x02000127 RID: 295
	public enum CustomGameJoinResponse
	{
		// Token: 0x04000318 RID: 792
		Success,
		// Token: 0x04000319 RID: 793
		IncorrectPlayerState,
		// Token: 0x0400031A RID: 794
		ServerCapacityIsFull,
		// Token: 0x0400031B RID: 795
		ErrorOnGameServer,
		// Token: 0x0400031C RID: 796
		GameServerAccessError,
		// Token: 0x0400031D RID: 797
		CustomGameServerNotAvailable,
		// Token: 0x0400031E RID: 798
		CustomGameServerFinishing,
		// Token: 0x0400031F RID: 799
		IncorrectPassword,
		// Token: 0x04000320 RID: 800
		PlayerBanned,
		// Token: 0x04000321 RID: 801
		HostReplyTimedOut,
		// Token: 0x04000322 RID: 802
		NoPlayerDataFound,
		// Token: 0x04000323 RID: 803
		UnspecifiedError,
		// Token: 0x04000324 RID: 804
		NoPlayersCanJoin,
		// Token: 0x04000325 RID: 805
		AlreadyRequestedWaitingForServerResponse,
		// Token: 0x04000326 RID: 806
		RequesterIsNotPartyLeader,
		// Token: 0x04000327 RID: 807
		NotAllPlayersReady,
		// Token: 0x04000328 RID: 808
		NotAllPlayersModulesMatchWithServer
	}
}
