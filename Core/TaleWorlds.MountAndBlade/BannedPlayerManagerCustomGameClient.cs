using System;
using System.Collections.Generic;
using TaleWorlds.PlayerServices;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x020002E1 RID: 737
	public static class BannedPlayerManagerCustomGameClient
	{
		// Token: 0x0600284F RID: 10319 RVA: 0x0009BF0C File Offset: 0x0009A10C
		public static void AddBannedPlayer(PlayerId playerId, int banDueTime)
		{
			BannedPlayerManagerCustomGameClient._bannedPlayers[playerId] = new BannedPlayerManagerCustomGameClient.BannedPlayer
			{
				PlayerId = playerId,
				BanDueTime = banDueTime
			};
		}

		// Token: 0x06002850 RID: 10320 RVA: 0x0009BF40 File Offset: 0x0009A140
		public static bool IsUserBanned(PlayerId playerId)
		{
			return BannedPlayerManagerCustomGameClient._bannedPlayers.ContainsKey(playerId) && BannedPlayerManagerCustomGameClient._bannedPlayers[playerId].BanDueTime > Environment.TickCount;
		}

		// Token: 0x04000ECC RID: 3788
		private static Dictionary<PlayerId, BannedPlayerManagerCustomGameClient.BannedPlayer> _bannedPlayers = new Dictionary<PlayerId, BannedPlayerManagerCustomGameClient.BannedPlayer>();

		// Token: 0x020005F3 RID: 1523
		private struct BannedPlayer
		{
			// Token: 0x170009B8 RID: 2488
			// (get) Token: 0x06003CDF RID: 15583 RVA: 0x000F1B66 File Offset: 0x000EFD66
			// (set) Token: 0x06003CE0 RID: 15584 RVA: 0x000F1B6E File Offset: 0x000EFD6E
			public PlayerId PlayerId { get; set; }

			// Token: 0x170009B9 RID: 2489
			// (get) Token: 0x06003CE1 RID: 15585 RVA: 0x000F1B77 File Offset: 0x000EFD77
			// (set) Token: 0x06003CE2 RID: 15586 RVA: 0x000F1B7F File Offset: 0x000EFD7F
			public int BanDueTime { get; set; }
		}
	}
}
