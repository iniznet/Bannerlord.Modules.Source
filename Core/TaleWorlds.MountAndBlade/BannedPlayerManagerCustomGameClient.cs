using System;
using System.Collections.Generic;
using TaleWorlds.PlayerServices;

namespace TaleWorlds.MountAndBlade
{
	public static class BannedPlayerManagerCustomGameClient
	{
		public static void AddBannedPlayer(PlayerId playerId, int banDueTime)
		{
			BannedPlayerManagerCustomGameClient._bannedPlayers[playerId] = new BannedPlayerManagerCustomGameClient.BannedPlayer
			{
				PlayerId = playerId,
				BanDueTime = banDueTime
			};
		}

		public static bool IsUserBanned(PlayerId playerId)
		{
			return BannedPlayerManagerCustomGameClient._bannedPlayers.ContainsKey(playerId) && BannedPlayerManagerCustomGameClient._bannedPlayers[playerId].BanDueTime > Environment.TickCount;
		}

		private static Dictionary<PlayerId, BannedPlayerManagerCustomGameClient.BannedPlayer> _bannedPlayers = new Dictionary<PlayerId, BannedPlayerManagerCustomGameClient.BannedPlayer>();

		private struct BannedPlayer
		{
			public PlayerId PlayerId { get; set; }

			public int BanDueTime { get; set; }
		}
	}
}
