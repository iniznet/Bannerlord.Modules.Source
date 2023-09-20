using System;
using TaleWorlds.Core;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x0200028B RID: 651
	public interface IRoundComponent : IMissionBehavior
	{
		// Token: 0x14000033 RID: 51
		// (add) Token: 0x06002285 RID: 8837
		// (remove) Token: 0x06002286 RID: 8838
		event Action OnRoundStarted;

		// Token: 0x14000034 RID: 52
		// (add) Token: 0x06002287 RID: 8839
		// (remove) Token: 0x06002288 RID: 8840
		event Action OnPreparationEnded;

		// Token: 0x14000035 RID: 53
		// (add) Token: 0x06002289 RID: 8841
		// (remove) Token: 0x0600228A RID: 8842
		event Action OnPreRoundEnding;

		// Token: 0x14000036 RID: 54
		// (add) Token: 0x0600228B RID: 8843
		// (remove) Token: 0x0600228C RID: 8844
		event Action OnRoundEnding;

		// Token: 0x14000037 RID: 55
		// (add) Token: 0x0600228D RID: 8845
		// (remove) Token: 0x0600228E RID: 8846
		event Action OnPostRoundEnded;

		// Token: 0x14000038 RID: 56
		// (add) Token: 0x0600228F RID: 8847
		// (remove) Token: 0x06002290 RID: 8848
		event Action OnCurrentRoundStateChanged;

		// Token: 0x1700068D RID: 1677
		// (get) Token: 0x06002291 RID: 8849
		float LastRoundEndRemainingTime { get; }

		// Token: 0x1700068E RID: 1678
		// (get) Token: 0x06002292 RID: 8850
		float RemainingRoundTime { get; }

		// Token: 0x1700068F RID: 1679
		// (get) Token: 0x06002293 RID: 8851
		MultiplayerRoundState CurrentRoundState { get; }

		// Token: 0x17000690 RID: 1680
		// (get) Token: 0x06002294 RID: 8852
		int RoundCount { get; }

		// Token: 0x17000691 RID: 1681
		// (get) Token: 0x06002295 RID: 8853
		BattleSideEnum RoundWinner { get; }

		// Token: 0x17000692 RID: 1682
		// (get) Token: 0x06002296 RID: 8854
		RoundEndReason RoundEndReason { get; }
	}
}
