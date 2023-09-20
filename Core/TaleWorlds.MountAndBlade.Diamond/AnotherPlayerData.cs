using System;

namespace TaleWorlds.MountAndBlade.Diamond
{
	// Token: 0x02000121 RID: 289
	[Serializable]
	public class AnotherPlayerData
	{
		// Token: 0x17000248 RID: 584
		// (get) Token: 0x06000685 RID: 1669 RVA: 0x0000AFE9 File Offset: 0x000091E9
		// (set) Token: 0x06000686 RID: 1670 RVA: 0x0000AFF1 File Offset: 0x000091F1
		public AnotherPlayerState PlayerState { get; private set; }

		// Token: 0x17000249 RID: 585
		// (get) Token: 0x06000687 RID: 1671 RVA: 0x0000AFFA File Offset: 0x000091FA
		// (set) Token: 0x06000688 RID: 1672 RVA: 0x0000B002 File Offset: 0x00009202
		public int Experience { get; private set; }

		// Token: 0x06000689 RID: 1673 RVA: 0x0000B00B File Offset: 0x0000920B
		public AnotherPlayerData(AnotherPlayerState anotherPlayerState, int anotherPlayerExperience)
		{
			this.PlayerState = anotherPlayerState;
			this.Experience = anotherPlayerExperience;
		}
	}
}
