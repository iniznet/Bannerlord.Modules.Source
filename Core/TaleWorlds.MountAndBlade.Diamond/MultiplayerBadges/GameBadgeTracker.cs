using System;

namespace TaleWorlds.MountAndBlade.Diamond.MultiplayerBadges
{
	// Token: 0x02000160 RID: 352
	public abstract class GameBadgeTracker
	{
		// Token: 0x060008CE RID: 2254 RVA: 0x0000F586 File Offset: 0x0000D786
		public virtual void OnPlayerJoin(PlayerData playerData)
		{
		}

		// Token: 0x060008CF RID: 2255 RVA: 0x0000F588 File Offset: 0x0000D788
		public virtual void OnKill(KillData killData)
		{
		}

		// Token: 0x060008D0 RID: 2256 RVA: 0x0000F58A File Offset: 0x0000D78A
		public virtual void OnStartingNextBattle()
		{
		}
	}
}
