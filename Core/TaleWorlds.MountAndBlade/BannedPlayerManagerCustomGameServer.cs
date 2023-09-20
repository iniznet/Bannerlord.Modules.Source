using System;
using System.Collections.Generic;
using System.IO;
using TaleWorlds.PlayerServices;

namespace TaleWorlds.MountAndBlade
{
	public static class BannedPlayerManagerCustomGameServer
	{
		public static void AddBannedPlayer(PlayerId playerId, string playerName, int banDueTime)
		{
			if (BannedPlayerManagerCustomGameServer._bannedPlayers.ContainsKey(playerId))
			{
				object bannedPlayersWriteLock = BannedPlayerManagerCustomGameServer._bannedPlayersWriteLock;
				lock (bannedPlayersWriteLock)
				{
					StreamWriter streamWriter = new StreamWriter("bannedUsers.txt", true);
					streamWriter.WriteLine(playerId + "\t" + playerName);
					streamWriter.Close();
				}
			}
			BannedPlayerManagerCustomGameServer._bannedPlayers[playerId] = new BannedPlayerManagerCustomGameServer.BannedPlayer
			{
				PlayerId = playerId,
				BanDueTime = banDueTime,
				PlayerName = playerName
			};
		}

		public static bool IsUserBanned(PlayerId playerId)
		{
			return BannedPlayerManagerCustomGameServer._bannedPlayers.ContainsKey(playerId) && (BannedPlayerManagerCustomGameServer._bannedPlayers[playerId].BanDueTime == 0 || BannedPlayerManagerCustomGameServer._bannedPlayers[playerId].BanDueTime > Environment.TickCount);
		}

		public static void LoadPlayers()
		{
			object bannedPlayersWriteLock = BannedPlayerManagerCustomGameServer._bannedPlayersWriteLock;
			lock (bannedPlayersWriteLock)
			{
				StreamReader streamReader = new StreamReader("bannedUsers.txt");
				while (streamReader.Peek() > 0)
				{
					string[] array = streamReader.ReadLine().Split(new char[] { '\t' });
					PlayerId playerId = PlayerId.FromString(array[0]);
					string text = array[1];
					BannedPlayerManagerCustomGameServer._bannedPlayers[playerId] = new BannedPlayerManagerCustomGameServer.BannedPlayer
					{
						PlayerId = playerId,
						PlayerName = text,
						BanDueTime = 0
					};
				}
				streamReader.Close();
			}
		}

		private static object _bannedPlayersWriteLock = new object();

		private static Dictionary<PlayerId, BannedPlayerManagerCustomGameServer.BannedPlayer> _bannedPlayers = new Dictionary<PlayerId, BannedPlayerManagerCustomGameServer.BannedPlayer>();

		private struct BannedPlayer
		{
			public PlayerId PlayerId { get; set; }

			public string PlayerName { get; set; }

			public int BanDueTime { get; set; }
		}
	}
}
