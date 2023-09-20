using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace TaleWorlds.PlayerServices.Avatar
{
	public class SteamAvatarService : ApiAvatarServiceBase
	{
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

		private const int FetchTaskWaitTime = 3000;

		private const string SteamWebApiKey = "820D6EC50E6AAE61E460EA207D8966F7";

		private const int MaxAccountsPerRequest = 100;

		private class GetPlayerSummariesResult
		{
			public SteamAvatarService.SteamPlayers response { get; set; }
		}

		private class SteamPlayers
		{
			public SteamAvatarService.SteamPlayerSummary[] players { get; set; }
		}

		private class SteamPlayerSummary
		{
			public string avatar { get; set; }

			public string avatarfull { get; set; }

			public string avatarmedium { get; set; }

			public int communityvisibilitystate { get; set; }

			public int lastlogoff { get; set; }

			public string personaname { get; set; }

			public int personastate { get; set; }

			public int personastateflags { get; set; }

			public string primaryclanid { get; set; }

			public int profilestate { get; set; }

			public string profileurl { get; set; }

			public string realname { get; set; }

			public string steamid { get; set; }

			public int timecreated { get; set; }
		}
	}
}
