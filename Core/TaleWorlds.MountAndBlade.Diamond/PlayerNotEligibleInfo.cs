using System;
using TaleWorlds.PlayerServices;

namespace TaleWorlds.MountAndBlade.Diamond
{
	// Token: 0x020000FC RID: 252
	[Serializable]
	public class PlayerNotEligibleInfo
	{
		// Token: 0x170001B7 RID: 439
		// (get) Token: 0x06000473 RID: 1139 RVA: 0x00006748 File Offset: 0x00004948
		// (set) Token: 0x06000474 RID: 1140 RVA: 0x00006750 File Offset: 0x00004950
		public PlayerId PlayerId { get; private set; }

		// Token: 0x170001B8 RID: 440
		// (get) Token: 0x06000475 RID: 1141 RVA: 0x00006759 File Offset: 0x00004959
		// (set) Token: 0x06000476 RID: 1142 RVA: 0x00006761 File Offset: 0x00004961
		public PlayerNotEligibleError[] Errors { get; private set; }

		// Token: 0x06000477 RID: 1143 RVA: 0x0000676A File Offset: 0x0000496A
		public PlayerNotEligibleInfo(PlayerId playerId, PlayerNotEligibleError[] errors)
		{
			this.PlayerId = playerId;
			this.Errors = errors;
		}
	}
}
