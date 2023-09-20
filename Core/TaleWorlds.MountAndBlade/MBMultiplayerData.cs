using System;
using TaleWorlds.MountAndBlade.Diamond;

namespace TaleWorlds.MountAndBlade
{
	public class MBMultiplayerData
	{
		public static Guid ServerId { get; set; }

		[MBCallback]
		public static string GetServerId()
		{
			return MBMultiplayerData.ServerId.ToString();
		}

		[MBCallback]
		public static string GetServerName()
		{
			return MBMultiplayerData.ServerName;
		}

		[MBCallback]
		public static string GetGameModule()
		{
			return MBMultiplayerData.GameModule;
		}

		[MBCallback]
		public static string GetGameType()
		{
			return MBMultiplayerData.GameType;
		}

		[MBCallback]
		public static string GetMap()
		{
			return MBMultiplayerData.Map;
		}

		[MBCallback]
		public static int GetCurrentPlayerCount()
		{
			return GameNetwork.NetworkPeerCount;
		}

		[MBCallback]
		public static int GetPlayerCountLimit()
		{
			return MBMultiplayerData.PlayerCountLimit;
		}

		public static event MBMultiplayerData.GameServerInfoReceivedDelegate GameServerInfoReceived;

		[MBCallback]
		public static void UpdateGameServerInfo(string id, string gameServer, string gameModule, string gameType, string map, int currentPlayerCount, int maxPlayerCount, string address, int port)
		{
			if (MBMultiplayerData.GameServerInfoReceived != null)
			{
				MBMultiplayerData.GameServerInfoReceived(new CustomBattleId(id), gameServer, gameModule, gameType, map, currentPlayerCount, maxPlayerCount, address, port);
			}
		}

		public static string ServerName;

		public static string GameModule;

		public static string GameType;

		public static string Map;

		public static int PlayerCountLimit;

		public delegate void GameServerInfoReceivedDelegate(CustomBattleId id, string gameServer, string gameModule, string gameType, string map, int currentPlayerCount, int maxPlayerCount, string address, int port);
	}
}
