using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace TaleWorlds.PlayerServices.Avatar
{
	// Token: 0x0200000C RID: 12
	public class SteamAvatarService : ApiAvatarServiceBase
	{
		// Token: 0x0600006A RID: 106 RVA: 0x00003078 File Offset: 0x00001278
		protected override async Task FetchAvatars()
		{
			await Task.Delay(3000);
			List<ValueTuple<ulong, AvatarData>> waitingAccounts = base.WaitingAccounts;
			lock (waitingAccounts)
			{
				if (base.WaitingAccounts.Count < 1)
				{
					return;
				}
				if (base.WaitingAccounts.Count <= 100)
				{
					base.InProgressAccounts = base.WaitingAccounts;
					base.WaitingAccounts = new List<ValueTuple<ulong, AvatarData>>();
				}
				else
				{
					base.InProgressAccounts = base.WaitingAccounts.GetRange(0, 100);
					base.WaitingAccounts.RemoveRange(0, 100);
				}
			}
			string text = "http://api.steampowered.com/ISteamUser/GetPlayerSummaries/v0002/?key=820D6EC50E6AAE61E460EA207D8966F7&steamids=" + string.Join<ulong>(",", base.InProgressAccounts.Select(([TupleElementNames(new string[] { "accountId", "avatarData" })] ValueTuple<ulong, AvatarData> a) => a.Item1));
			SteamAvatarService.SteamPlayers steamPlayers = null;
			try
			{
				SteamAvatarService.GetPlayerSummariesResult getPlayerSummariesResult = JsonConvert.DeserializeObject<SteamAvatarService.GetPlayerSummariesResult>(await new TimeoutWebClient().DownloadStringTaskAsync(text));
				bool flag2;
				if (getPlayerSummariesResult == null)
				{
					flag2 = null != null;
				}
				else
				{
					SteamAvatarService.SteamPlayers response = getPlayerSummariesResult.response;
					flag2 = ((response != null) ? response.players : null) != null;
				}
				if (flag2 && getPlayerSummariesResult.response.players.Length != 0)
				{
					steamPlayers = getPlayerSummariesResult.response;
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
			}
			if (steamPlayers == null || steamPlayers.players.Length < 1)
			{
				foreach (ValueTuple<ulong, AvatarData> valueTuple in base.InProgressAccounts)
				{
					valueTuple.Item2.SetFailed();
				}
			}
			else
			{
				List<Task> list = new List<Task>();
				foreach (ValueTuple<ulong, AvatarData> valueTuple2 in base.InProgressAccounts)
				{
					ulong item = valueTuple2.Item1;
					AvatarData item2 = valueTuple2.Item2;
					string text2 = string.Concat(item);
					string text3 = null;
					foreach (SteamAvatarService.SteamPlayerSummary steamPlayerSummary in steamPlayers.players)
					{
						if (steamPlayerSummary.steamid == text2)
						{
							text3 = steamPlayerSummary.avatarfull;
							break;
						}
					}
					if (!string.IsNullOrWhiteSpace(text3))
					{
						list.Add(this.UpdateAvatarImageData(item, text3, item2));
					}
					else
					{
						item2.SetFailed();
					}
				}
				if (list.Count > 0)
				{
					await Task.WhenAll(list);
				}
			}
		}

		// Token: 0x0600006B RID: 107 RVA: 0x000030C0 File Offset: 0x000012C0
		private async Task UpdateAvatarImageData(ulong accountId, string avatarUrl, AvatarData avatarData)
		{
			if (!string.IsNullOrWhiteSpace(avatarUrl))
			{
				byte[] array = await new TimeoutWebClient().DownloadDataTaskAsync(avatarUrl);
				if (array != null && array.Length != 0)
				{
					avatarData.SetImageData(array);
					Dictionary<ulong, AvatarData> avatarImageCache = base.AvatarImageCache;
					lock (avatarImageCache)
					{
						base.AvatarImageCache[accountId] = avatarData;
					}
				}
			}
		}

		// Token: 0x0400002C RID: 44
		private const int FetchTaskWaitTime = 3000;

		// Token: 0x0400002D RID: 45
		private const string SteamWebApiKey = "820D6EC50E6AAE61E460EA207D8966F7";

		// Token: 0x0400002E RID: 46
		private const int MaxAccountsPerRequest = 100;

		// Token: 0x02000012 RID: 18
		private class GetPlayerSummariesResult
		{
			// Token: 0x1700001A RID: 26
			// (get) Token: 0x06000077 RID: 119 RVA: 0x0000348E File Offset: 0x0000168E
			// (set) Token: 0x06000078 RID: 120 RVA: 0x00003496 File Offset: 0x00001696
			public SteamAvatarService.SteamPlayers response { get; set; }
		}

		// Token: 0x02000013 RID: 19
		private class SteamPlayers
		{
			// Token: 0x1700001B RID: 27
			// (get) Token: 0x0600007A RID: 122 RVA: 0x000034A7 File Offset: 0x000016A7
			// (set) Token: 0x0600007B RID: 123 RVA: 0x000034AF File Offset: 0x000016AF
			public SteamAvatarService.SteamPlayerSummary[] players { get; set; }
		}

		// Token: 0x02000014 RID: 20
		private class SteamPlayerSummary
		{
			// Token: 0x1700001C RID: 28
			// (get) Token: 0x0600007D RID: 125 RVA: 0x000034C0 File Offset: 0x000016C0
			// (set) Token: 0x0600007E RID: 126 RVA: 0x000034C8 File Offset: 0x000016C8
			public string avatar { get; set; }

			// Token: 0x1700001D RID: 29
			// (get) Token: 0x0600007F RID: 127 RVA: 0x000034D1 File Offset: 0x000016D1
			// (set) Token: 0x06000080 RID: 128 RVA: 0x000034D9 File Offset: 0x000016D9
			public string avatarfull { get; set; }

			// Token: 0x1700001E RID: 30
			// (get) Token: 0x06000081 RID: 129 RVA: 0x000034E2 File Offset: 0x000016E2
			// (set) Token: 0x06000082 RID: 130 RVA: 0x000034EA File Offset: 0x000016EA
			public string avatarmedium { get; set; }

			// Token: 0x1700001F RID: 31
			// (get) Token: 0x06000083 RID: 131 RVA: 0x000034F3 File Offset: 0x000016F3
			// (set) Token: 0x06000084 RID: 132 RVA: 0x000034FB File Offset: 0x000016FB
			public int communityvisibilitystate { get; set; }

			// Token: 0x17000020 RID: 32
			// (get) Token: 0x06000085 RID: 133 RVA: 0x00003504 File Offset: 0x00001704
			// (set) Token: 0x06000086 RID: 134 RVA: 0x0000350C File Offset: 0x0000170C
			public int lastlogoff { get; set; }

			// Token: 0x17000021 RID: 33
			// (get) Token: 0x06000087 RID: 135 RVA: 0x00003515 File Offset: 0x00001715
			// (set) Token: 0x06000088 RID: 136 RVA: 0x0000351D File Offset: 0x0000171D
			public string personaname { get; set; }

			// Token: 0x17000022 RID: 34
			// (get) Token: 0x06000089 RID: 137 RVA: 0x00003526 File Offset: 0x00001726
			// (set) Token: 0x0600008A RID: 138 RVA: 0x0000352E File Offset: 0x0000172E
			public int personastate { get; set; }

			// Token: 0x17000023 RID: 35
			// (get) Token: 0x0600008B RID: 139 RVA: 0x00003537 File Offset: 0x00001737
			// (set) Token: 0x0600008C RID: 140 RVA: 0x0000353F File Offset: 0x0000173F
			public int personastateflags { get; set; }

			// Token: 0x17000024 RID: 36
			// (get) Token: 0x0600008D RID: 141 RVA: 0x00003548 File Offset: 0x00001748
			// (set) Token: 0x0600008E RID: 142 RVA: 0x00003550 File Offset: 0x00001750
			public string primaryclanid { get; set; }

			// Token: 0x17000025 RID: 37
			// (get) Token: 0x0600008F RID: 143 RVA: 0x00003559 File Offset: 0x00001759
			// (set) Token: 0x06000090 RID: 144 RVA: 0x00003561 File Offset: 0x00001761
			public int profilestate { get; set; }

			// Token: 0x17000026 RID: 38
			// (get) Token: 0x06000091 RID: 145 RVA: 0x0000356A File Offset: 0x0000176A
			// (set) Token: 0x06000092 RID: 146 RVA: 0x00003572 File Offset: 0x00001772
			public string profileurl { get; set; }

			// Token: 0x17000027 RID: 39
			// (get) Token: 0x06000093 RID: 147 RVA: 0x0000357B File Offset: 0x0000177B
			// (set) Token: 0x06000094 RID: 148 RVA: 0x00003583 File Offset: 0x00001783
			public string realname { get; set; }

			// Token: 0x17000028 RID: 40
			// (get) Token: 0x06000095 RID: 149 RVA: 0x0000358C File Offset: 0x0000178C
			// (set) Token: 0x06000096 RID: 150 RVA: 0x00003594 File Offset: 0x00001794
			public string steamid { get; set; }

			// Token: 0x17000029 RID: 41
			// (get) Token: 0x06000097 RID: 151 RVA: 0x0000359D File Offset: 0x0000179D
			// (set) Token: 0x06000098 RID: 152 RVA: 0x000035A5 File Offset: 0x000017A5
			public int timecreated { get; set; }
		}
	}
}
