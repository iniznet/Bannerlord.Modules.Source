using System;
using TaleWorlds.MountAndBlade.Diamond;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x020001BA RID: 442
	public class MBMultiplayerData
	{
		// Token: 0x1700050D RID: 1293
		// (get) Token: 0x0600198E RID: 6542 RVA: 0x0005B9A9 File Offset: 0x00059BA9
		// (set) Token: 0x0600198F RID: 6543 RVA: 0x0005B9B0 File Offset: 0x00059BB0
		public static Guid ServerId { get; set; }

		// Token: 0x06001990 RID: 6544 RVA: 0x0005B9B8 File Offset: 0x00059BB8
		[MBCallback]
		public static string GetServerId()
		{
			return MBMultiplayerData.ServerId.ToString();
		}

		// Token: 0x06001991 RID: 6545 RVA: 0x0005B9D8 File Offset: 0x00059BD8
		[MBCallback]
		public static string GetServerName()
		{
			return MBMultiplayerData.ServerName;
		}

		// Token: 0x06001992 RID: 6546 RVA: 0x0005B9DF File Offset: 0x00059BDF
		[MBCallback]
		public static string GetGameModule()
		{
			return MBMultiplayerData.GameModule;
		}

		// Token: 0x06001993 RID: 6547 RVA: 0x0005B9E6 File Offset: 0x00059BE6
		[MBCallback]
		public static string GetGameType()
		{
			return MBMultiplayerData.GameType;
		}

		// Token: 0x06001994 RID: 6548 RVA: 0x0005B9ED File Offset: 0x00059BED
		[MBCallback]
		public static string GetMap()
		{
			return MBMultiplayerData.Map;
		}

		// Token: 0x06001995 RID: 6549 RVA: 0x0005B9F4 File Offset: 0x00059BF4
		[MBCallback]
		public static int GetCurrentPlayerCount()
		{
			return GameNetwork.NetworkPeerCount;
		}

		// Token: 0x06001996 RID: 6550 RVA: 0x0005B9FB File Offset: 0x00059BFB
		[MBCallback]
		public static int GetPlayerCountLimit()
		{
			return MBMultiplayerData.PlayerCountLimit;
		}

		// Token: 0x14000017 RID: 23
		// (add) Token: 0x06001997 RID: 6551 RVA: 0x0005BA04 File Offset: 0x00059C04
		// (remove) Token: 0x06001998 RID: 6552 RVA: 0x0005BA38 File Offset: 0x00059C38
		public static event MBMultiplayerData.GameServerInfoReceivedDelegate GameServerInfoReceived;

		// Token: 0x06001999 RID: 6553 RVA: 0x0005BA6C File Offset: 0x00059C6C
		[MBCallback]
		public static void UpdateGameServerInfo(string id, string gameServer, string gameModule, string gameType, string map, int currentPlayerCount, int maxPlayerCount, string address, int port)
		{
			if (MBMultiplayerData.GameServerInfoReceived != null)
			{
				MBMultiplayerData.GameServerInfoReceived(new CustomBattleId(id), gameServer, gameModule, gameType, map, currentPlayerCount, maxPlayerCount, address, port);
			}
		}

		// Token: 0x040007D1 RID: 2001
		public static string ServerName;

		// Token: 0x040007D2 RID: 2002
		public static string GameModule;

		// Token: 0x040007D3 RID: 2003
		public static string GameType;

		// Token: 0x040007D4 RID: 2004
		public static string Map;

		// Token: 0x040007D5 RID: 2005
		public static int PlayerCountLimit;

		// Token: 0x02000517 RID: 1303
		// (Invoke) Token: 0x06003968 RID: 14696
		public delegate void GameServerInfoReceivedDelegate(CustomBattleId id, string gameServer, string gameModule, string gameType, string map, int currentPlayerCount, int maxPlayerCount, string address, int port);
	}
}
