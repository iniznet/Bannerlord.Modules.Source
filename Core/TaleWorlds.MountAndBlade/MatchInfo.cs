using System;
using System.Collections.Generic;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000321 RID: 801
	public class MatchInfo
	{
		// Token: 0x170007B1 RID: 1969
		// (get) Token: 0x06002B5A RID: 11098 RVA: 0x000A98F0 File Offset: 0x000A7AF0
		// (set) Token: 0x06002B5B RID: 11099 RVA: 0x000A98F8 File Offset: 0x000A7AF8
		public string MatchId { get; set; }

		// Token: 0x170007B2 RID: 1970
		// (get) Token: 0x06002B5C RID: 11100 RVA: 0x000A9901 File Offset: 0x000A7B01
		// (set) Token: 0x06002B5D RID: 11101 RVA: 0x000A9909 File Offset: 0x000A7B09
		public string MatchType { get; set; }

		// Token: 0x170007B3 RID: 1971
		// (get) Token: 0x06002B5E RID: 11102 RVA: 0x000A9912 File Offset: 0x000A7B12
		// (set) Token: 0x06002B5F RID: 11103 RVA: 0x000A991A File Offset: 0x000A7B1A
		public string GameType { get; set; }

		// Token: 0x170007B4 RID: 1972
		// (get) Token: 0x06002B60 RID: 11104 RVA: 0x000A9923 File Offset: 0x000A7B23
		// (set) Token: 0x06002B61 RID: 11105 RVA: 0x000A992B File Offset: 0x000A7B2B
		public string Map { get; set; }

		// Token: 0x170007B5 RID: 1973
		// (get) Token: 0x06002B62 RID: 11106 RVA: 0x000A9934 File Offset: 0x000A7B34
		// (set) Token: 0x06002B63 RID: 11107 RVA: 0x000A993C File Offset: 0x000A7B3C
		public DateTime MatchDate { get; set; }

		// Token: 0x170007B6 RID: 1974
		// (get) Token: 0x06002B64 RID: 11108 RVA: 0x000A9945 File Offset: 0x000A7B45
		// (set) Token: 0x06002B65 RID: 11109 RVA: 0x000A994D File Offset: 0x000A7B4D
		public int WinnerTeam { get; set; }

		// Token: 0x170007B7 RID: 1975
		// (get) Token: 0x06002B66 RID: 11110 RVA: 0x000A9956 File Offset: 0x000A7B56
		// (set) Token: 0x06002B67 RID: 11111 RVA: 0x000A995E File Offset: 0x000A7B5E
		public string Faction1 { get; set; }

		// Token: 0x170007B8 RID: 1976
		// (get) Token: 0x06002B68 RID: 11112 RVA: 0x000A9967 File Offset: 0x000A7B67
		// (set) Token: 0x06002B69 RID: 11113 RVA: 0x000A996F File Offset: 0x000A7B6F
		public string Faction2 { get; set; }

		// Token: 0x170007B9 RID: 1977
		// (get) Token: 0x06002B6A RID: 11114 RVA: 0x000A9978 File Offset: 0x000A7B78
		// (set) Token: 0x06002B6B RID: 11115 RVA: 0x000A9980 File Offset: 0x000A7B80
		public int DefenderScore { get; set; }

		// Token: 0x170007BA RID: 1978
		// (get) Token: 0x06002B6C RID: 11116 RVA: 0x000A9989 File Offset: 0x000A7B89
		// (set) Token: 0x06002B6D RID: 11117 RVA: 0x000A9991 File Offset: 0x000A7B91
		public int AttackerScore { get; set; }

		// Token: 0x170007BB RID: 1979
		// (get) Token: 0x06002B6E RID: 11118 RVA: 0x000A999A File Offset: 0x000A7B9A
		// (set) Token: 0x06002B6F RID: 11119 RVA: 0x000A99A2 File Offset: 0x000A7BA2
		public List<PlayerInfo> Players { get; set; }

		// Token: 0x06002B70 RID: 11120 RVA: 0x000A99AB File Offset: 0x000A7BAB
		public MatchInfo()
		{
			this.Players = new List<PlayerInfo>();
		}

		// Token: 0x06002B71 RID: 11121 RVA: 0x000A99C0 File Offset: 0x000A7BC0
		private PlayerInfo TryGetPlayer(string id)
		{
			foreach (PlayerInfo playerInfo in this.Players)
			{
				if (playerInfo.PlayerId == id)
				{
					return playerInfo;
				}
			}
			return null;
		}

		// Token: 0x06002B72 RID: 11122 RVA: 0x000A9A24 File Offset: 0x000A7C24
		public void AddOrUpdatePlayer(string id, string username, int forcedIndex, int teamNo)
		{
			PlayerInfo playerInfo = this.TryGetPlayer(id);
			if (playerInfo == null)
			{
				this.Players.Add(new PlayerInfo
				{
					PlayerId = id,
					Username = username,
					ForcedIndex = forcedIndex,
					TeamNo = teamNo
				});
				return;
			}
			playerInfo.TeamNo = teamNo;
		}

		// Token: 0x06002B73 RID: 11123 RVA: 0x000A9A74 File Offset: 0x000A7C74
		public bool TryUpdatePlayerStats(string id, int kill, int death, int assist)
		{
			PlayerInfo playerInfo = this.TryGetPlayer(id);
			if (playerInfo != null)
			{
				playerInfo.Kill = kill;
				playerInfo.Death = death;
				playerInfo.Assist = assist;
				return true;
			}
			return false;
		}
	}
}
