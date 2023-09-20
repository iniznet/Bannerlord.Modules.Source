using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Newtonsoft.Json;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000320 RID: 800
	public static class MatchHistory
	{
		// Token: 0x170007AF RID: 1967
		// (get) Token: 0x06002B4F RID: 11087 RVA: 0x000A9614 File Offset: 0x000A7814
		private static PlatformFilePath HistoryFilePath
		{
			get
			{
				PlatformDirectoryPath platformDirectoryPath = new PlatformDirectoryPath(PlatformFileType.User, "Data");
				return new PlatformFilePath(platformDirectoryPath, "History.json");
			}
		}

		// Token: 0x170007B0 RID: 1968
		// (get) Token: 0x06002B50 RID: 11088 RVA: 0x000A9639 File Offset: 0x000A7839
		public static MBReadOnlyList<MatchInfo> Matches
		{
			get
			{
				return MatchHistory._matches;
			}
		}

		// Token: 0x06002B52 RID: 11090 RVA: 0x000A9654 File Offset: 0x000A7854
		public static async Task LoadMatchHistory()
		{
			if (MatchHistory.IsHistoryCacheDirty)
			{
				if (FileHelper.FileExists(MatchHistory.HistoryFilePath))
				{
					try
					{
						TaskAwaiter<string> taskAwaiter = FileHelper.GetFileContentStringAsync(MatchHistory.HistoryFilePath).GetAwaiter();
						if (!taskAwaiter.IsCompleted)
						{
							await taskAwaiter;
							TaskAwaiter<string> taskAwaiter2;
							taskAwaiter = taskAwaiter2;
							taskAwaiter2 = default(TaskAwaiter<string>);
						}
						MatchHistory._matches = JsonConvert.DeserializeObject<MBList<MatchInfo>>(taskAwaiter.GetResult());
						if (MatchHistory._matches == null)
						{
							MatchHistory._matches = new MBList<MatchInfo>();
							throw new Exception("_matches were null.");
						}
					}
					catch (Exception ex)
					{
						Debug.FailedAssert("Could not load match history. " + ex.Message, "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\Network\\MatchHistory.cs", "LoadMatchHistory", 65);
						try
						{
							FileHelper.DeleteFile(MatchHistory.HistoryFilePath);
						}
						catch (Exception ex2)
						{
							Debug.FailedAssert("Could not delete match history file. " + ex2.Message, "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\Network\\MatchHistory.cs", "LoadMatchHistory", 72);
						}
					}
				}
				MatchHistory.IsHistoryCacheDirty = false;
			}
		}

		// Token: 0x06002B53 RID: 11091 RVA: 0x000A9694 File Offset: 0x000A7894
		public static async Task<MBReadOnlyList<MatchInfo>> GetMatches()
		{
			await MatchHistory.LoadMatchHistory();
			return MatchHistory.Matches;
		}

		// Token: 0x06002B54 RID: 11092 RVA: 0x000A96D4 File Offset: 0x000A78D4
		public static void AddMatch(MatchInfo match)
		{
			MatchInfo matchInfo;
			if (MatchHistory.TryGetMatchInfo(match.MatchId, out matchInfo))
			{
				for (int i = 0; i < MatchHistory._matches.Count; i++)
				{
					if (MatchHistory._matches[i].MatchId == match.MatchId)
					{
						MatchHistory._matches[i] = match;
					}
				}
				return;
			}
			int matchTypeCount = MatchHistory.GetMatchTypeCount(match.MatchType);
			if (matchTypeCount >= 10)
			{
				MatchHistory.RemoveMatches(match.MatchType, matchTypeCount - 10 + 1);
			}
			MatchHistory._matches.Add(match);
		}

		// Token: 0x06002B55 RID: 11093 RVA: 0x000A975C File Offset: 0x000A795C
		public static bool TryGetMatchInfo(string matchId, out MatchInfo matchInfo)
		{
			matchInfo = null;
			foreach (MatchInfo matchInfo2 in MatchHistory._matches)
			{
				if (matchInfo2.MatchId == matchId)
				{
					matchInfo = matchInfo2;
					return true;
				}
			}
			return false;
		}

		// Token: 0x06002B56 RID: 11094 RVA: 0x000A97C4 File Offset: 0x000A79C4
		private static void RemoveMatches(string matchType, int numMatchToRemove)
		{
			for (int i = 0; i < numMatchToRemove; i++)
			{
				MatchInfo oldestMatch = MatchHistory.GetOldestMatch(matchType);
				MatchHistory._matches.Remove(oldestMatch);
			}
		}

		// Token: 0x06002B57 RID: 11095 RVA: 0x000A97F0 File Offset: 0x000A79F0
		private static MatchInfo GetOldestMatch(string matchType)
		{
			DateTime dateTime = DateTime.MaxValue;
			MatchInfo matchInfo = null;
			foreach (MatchInfo matchInfo2 in MatchHistory._matches)
			{
				if (matchInfo2.MatchDate < dateTime)
				{
					dateTime = matchInfo2.MatchDate;
					matchInfo = matchInfo2;
				}
			}
			return matchInfo;
		}

		// Token: 0x06002B58 RID: 11096 RVA: 0x000A985C File Offset: 0x000A7A5C
		public static async void Serialize()
		{
			try
			{
				byte[] array = Common.SerializeObjectAsJson(MatchHistory._matches);
				await FileHelper.SaveFileAsync(MatchHistory.HistoryFilePath, array);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
			}
		}

		// Token: 0x06002B59 RID: 11097 RVA: 0x000A9890 File Offset: 0x000A7A90
		private static int GetMatchTypeCount(string category)
		{
			int num = 0;
			using (List<MatchInfo>.Enumerator enumerator = MatchHistory._matches.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.MatchType == category)
					{
						num++;
					}
				}
			}
			return num;
		}

		// Token: 0x04001060 RID: 4192
		private const int MaxMatchCountPerMatchType = 10;

		// Token: 0x04001061 RID: 4193
		private const string HistoryDirectoryName = "Data";

		// Token: 0x04001062 RID: 4194
		private const string HistoryFileName = "History.json";

		// Token: 0x04001063 RID: 4195
		private static bool IsHistoryCacheDirty = true;

		// Token: 0x04001064 RID: 4196
		private static MBList<MatchInfo> _matches = new MBList<MatchInfo>();
	}
}
