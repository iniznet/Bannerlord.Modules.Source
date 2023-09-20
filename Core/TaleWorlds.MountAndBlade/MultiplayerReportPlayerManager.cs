using System;
using System.Collections.Generic;
using TaleWorlds.PlayerServices;

namespace TaleWorlds.MountAndBlade
{
	public static class MultiplayerReportPlayerManager
	{
		public static event Action<string, PlayerId, string, bool> ReportHandlers;

		public static void RequestReportPlayer(string gameId, PlayerId playerId, string playerName, bool isRequestedFromMission)
		{
			Action<string, PlayerId, string, bool> reportHandlers = MultiplayerReportPlayerManager.ReportHandlers;
			if (reportHandlers == null)
			{
				return;
			}
			reportHandlers(gameId, playerId, playerName, isRequestedFromMission);
		}

		public static void OnPlayerReported(PlayerId playerId)
		{
			MultiplayerReportPlayerManager.IncrementReportOfPlayer(playerId);
		}

		public static bool IsPlayerReportedOverLimit(PlayerId player)
		{
			int num;
			return MultiplayerReportPlayerManager._reportsPerPlayer.TryGetValue(player, out num) && num == 3;
		}

		private static void IncrementReportOfPlayer(PlayerId player)
		{
			if (MultiplayerReportPlayerManager._reportsPerPlayer.ContainsKey(player))
			{
				Dictionary<PlayerId, int> reportsPerPlayer = MultiplayerReportPlayerManager._reportsPerPlayer;
				int num = reportsPerPlayer[player];
				reportsPerPlayer[player] = num + 1;
				return;
			}
			MultiplayerReportPlayerManager._reportsPerPlayer.Add(player, 1);
		}

		private static Dictionary<PlayerId, int> _reportsPerPlayer = new Dictionary<PlayerId, int>();

		private const int _maxReportsPerPlayer = 3;
	}
}
