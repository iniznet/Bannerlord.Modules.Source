using System;
using System.Collections.Generic;
using System.IO;
using TaleWorlds.PlayerServices;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x020002E2 RID: 738
	public static class BannedPlayerManagerCustomGameServer
	{
		// Token: 0x06002852 RID: 10322 RVA: 0x0009BF84 File Offset: 0x0009A184
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

		// Token: 0x06002853 RID: 10323 RVA: 0x0009C020 File Offset: 0x0009A220
		public static bool IsUserBanned(PlayerId playerId)
		{
			return BannedPlayerManagerCustomGameServer._bannedPlayers.ContainsKey(playerId) && (BannedPlayerManagerCustomGameServer._bannedPlayers[playerId].BanDueTime == 0 || BannedPlayerManagerCustomGameServer._bannedPlayers[playerId].BanDueTime > Environment.TickCount);
		}

		// Token: 0x06002854 RID: 10324 RVA: 0x0009C070 File Offset: 0x0009A270
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

		// Token: 0x04000ECD RID: 3789
		private static object _bannedPlayersWriteLock = new object();

		// Token: 0x04000ECE RID: 3790
		private static Dictionary<PlayerId, BannedPlayerManagerCustomGameServer.BannedPlayer> _bannedPlayers = new Dictionary<PlayerId, BannedPlayerManagerCustomGameServer.BannedPlayer>();

		// Token: 0x020005F4 RID: 1524
		private struct BannedPlayer
		{
			// Token: 0x170009BA RID: 2490
			// (get) Token: 0x06003CE3 RID: 15587 RVA: 0x000F1B88 File Offset: 0x000EFD88
			// (set) Token: 0x06003CE4 RID: 15588 RVA: 0x000F1B90 File Offset: 0x000EFD90
			public PlayerId PlayerId { get; set; }

			// Token: 0x170009BB RID: 2491
			// (get) Token: 0x06003CE5 RID: 15589 RVA: 0x000F1B99 File Offset: 0x000EFD99
			// (set) Token: 0x06003CE6 RID: 15590 RVA: 0x000F1BA1 File Offset: 0x000EFDA1
			public string PlayerName { get; set; }

			// Token: 0x170009BC RID: 2492
			// (get) Token: 0x06003CE7 RID: 15591 RVA: 0x000F1BAA File Offset: 0x000EFDAA
			// (set) Token: 0x06003CE8 RID: 15592 RVA: 0x000F1BB2 File Offset: 0x000EFDB2
			public int BanDueTime { get; set; }
		}
	}
}
