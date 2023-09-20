using System;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x020002A6 RID: 678
	public enum MultiplayerPollRejectReason
	{
		// Token: 0x04000DCC RID: 3532
		NotEnoughPlayersToOpenPoll,
		// Token: 0x04000DCD RID: 3533
		HasOngoingPoll,
		// Token: 0x04000DCE RID: 3534
		TooManyPollRequests,
		// Token: 0x04000DCF RID: 3535
		KickPollTargetNotSynced,
		// Token: 0x04000DD0 RID: 3536
		Count
	}
}
