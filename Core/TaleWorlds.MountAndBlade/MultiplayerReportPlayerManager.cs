using System;
using System.Collections.Generic;
using TaleWorlds.PlayerServices;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x020002DF RID: 735
	public static class MultiplayerReportPlayerManager
	{
		// Token: 0x14000075 RID: 117
		// (add) Token: 0x06002846 RID: 10310 RVA: 0x0009BD98 File Offset: 0x00099F98
		// (remove) Token: 0x06002847 RID: 10311 RVA: 0x0009BDCC File Offset: 0x00099FCC
		public static event Action<string, PlayerId, string, bool> ReportHandlers;

		// Token: 0x06002848 RID: 10312 RVA: 0x0009BDFF File Offset: 0x00099FFF
		public static void RequestReportPlayer(string gameId, PlayerId playerId, string playerName, bool isRequestedFromMission)
		{
			Action<string, PlayerId, string, bool> reportHandlers = MultiplayerReportPlayerManager.ReportHandlers;
			if (reportHandlers == null)
			{
				return;
			}
			reportHandlers(gameId, playerId, playerName, isRequestedFromMission);
		}

		// Token: 0x06002849 RID: 10313 RVA: 0x0009BE14 File Offset: 0x0009A014
		public static void OnPlayerReported(PlayerId playerId)
		{
			MultiplayerReportPlayerManager.IncrementReportOfPlayer(playerId);
		}

		// Token: 0x0600284A RID: 10314 RVA: 0x0009BE1C File Offset: 0x0009A01C
		public static bool IsPlayerReportedOverLimit(PlayerId player)
		{
			int num;
			return MultiplayerReportPlayerManager._reportsPerPlayer.TryGetValue(player, out num) && num == 3;
		}

		// Token: 0x0600284B RID: 10315 RVA: 0x0009BE40 File Offset: 0x0009A040
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

		// Token: 0x04000EC4 RID: 3780
		private static Dictionary<PlayerId, int> _reportsPerPlayer = new Dictionary<PlayerId, int>();

		// Token: 0x04000EC5 RID: 3781
		private const int _maxReportsPerPlayer = 3;
	}
}
