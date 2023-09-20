using System;
using TaleWorlds.PlayerServices;

namespace TaleWorlds.MountAndBlade.Diamond
{
	// Token: 0x020000FD RID: 253
	[Serializable]
	public class ClanPlayerInfo
	{
		// Token: 0x170001B9 RID: 441
		// (get) Token: 0x06000478 RID: 1144 RVA: 0x00006780 File Offset: 0x00004980
		// (set) Token: 0x06000479 RID: 1145 RVA: 0x00006788 File Offset: 0x00004988
		public PlayerId PlayerId { get; private set; }

		// Token: 0x170001BA RID: 442
		// (get) Token: 0x0600047A RID: 1146 RVA: 0x00006791 File Offset: 0x00004991
		// (set) Token: 0x0600047B RID: 1147 RVA: 0x00006799 File Offset: 0x00004999
		public string PlayerName { get; private set; }

		// Token: 0x170001BB RID: 443
		// (get) Token: 0x0600047C RID: 1148 RVA: 0x000067A2 File Offset: 0x000049A2
		// (set) Token: 0x0600047D RID: 1149 RVA: 0x000067AA File Offset: 0x000049AA
		public AnotherPlayerState State { get; private set; }

		// Token: 0x170001BC RID: 444
		// (get) Token: 0x0600047E RID: 1150 RVA: 0x000067B3 File Offset: 0x000049B3
		// (set) Token: 0x0600047F RID: 1151 RVA: 0x000067BB File Offset: 0x000049BB
		public string ActiveBadgeId { get; private set; }

		// Token: 0x06000480 RID: 1152 RVA: 0x000067C4 File Offset: 0x000049C4
		public ClanPlayerInfo(PlayerId playerId, string playerName, AnotherPlayerState anotherPlayerState, string activeBadgeId)
		{
			this.PlayerId = playerId;
			this.PlayerName = playerName;
			this.ActiveBadgeId = activeBadgeId;
			this.State = anotherPlayerState;
		}
	}
}
