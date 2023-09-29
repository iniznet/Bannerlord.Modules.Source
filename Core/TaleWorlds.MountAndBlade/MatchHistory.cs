using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Newtonsoft.Json;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	public static class MatchHistory
	{
		private static PlatformFilePath HistoryFilePath
		{
			get
			{
				PlatformDirectoryPath platformDirectoryPath = new PlatformDirectoryPath(PlatformFileType.User, "Data");
				return new PlatformFilePath(platformDirectoryPath, "History.json");
			}
		}

		public static MBReadOnlyList<MatchInfo> Matches
		{
			get
			{
				return MatchHistory._matches;
			}
		}

		private static void PrintDebugLog(string text)
		{
			Debug.Print("[MATCH_HISTORY]: " + text, 0, Debug.DebugColor.Yellow, 17592186044416UL);
		}

		public static async Task LoadMatchHistory()
		{
			if (MatchHistory._isHistoryCacheDirty)
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
						Debug.FailedAssert("Could not load match history. " + ex.Message, "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\Network\\MatchHistory.cs", "LoadMatchHistory", 70);
						try
						{
							FileHelper.DeleteFile(MatchHistory.HistoryFilePath);
						}
						catch (Exception ex2)
						{
							Debug.FailedAssert("Could not delete match history file. " + ex2.Message, "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\Network\\MatchHistory.cs", "LoadMatchHistory", 77);
						}
					}
				}
				MatchHistory._isHistoryCacheDirty = false;
			}
		}

		public static async Task<MBReadOnlyList<MatchInfo>> GetMatches()
		{
			await MatchHistory.LoadMatchHistory();
			return MatchHistory.Matches;
		}

		public static void AddMatch(MatchInfo match)
		{
			MatchHistory.PrintDebugLog("Add match called for match: " + match.MatchId);
			MatchInfo matchInfo;
			if (MatchHistory.TryGetMatchInfo(match.MatchId, out matchInfo))
			{
				MatchHistory.PrintDebugLog("Found existing match with id trying to replace: " + match.MatchId);
				for (int i = 0; i < MatchHistory._matches.Count; i++)
				{
					if (MatchHistory._matches[i].MatchId == match.MatchId)
					{
						MatchHistory._matches[i] = match;
						MatchHistory.PrintDebugLog("[MATCH_HISTORY]: Replaced existing match: (" + MatchHistory._matches[i].MatchId + ") with: " + match.MatchId);
						break;
					}
				}
			}
			else
			{
				int matchTypeCount = MatchHistory.GetMatchTypeCount(match.MatchType);
				if (matchTypeCount >= 10)
				{
					int num = matchTypeCount - 10 + 1;
					MatchHistory.RemoveMatches(match.MatchType, matchTypeCount - 10 + 1);
					MatchHistory.PrintDebugLog(string.Format("[MATCH_HISTORY]: Max match count is reached, removing ({0}) matches with type: {1}", num, match.MatchType));
				}
				MatchHistory._matches.Add(match);
			}
			MatchHistory._isHistoryCacheDirty = true;
		}

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

		private static void RemoveMatches(string matchType, int numMatchToRemove)
		{
			for (int i = 0; i < numMatchToRemove; i++)
			{
				MatchInfo oldestMatch = MatchHistory.GetOldestMatch(matchType);
				MatchHistory._matches.Remove(oldestMatch);
			}
			MatchHistory._isHistoryCacheDirty = true;
		}

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

		public static async void Serialize()
		{
			try
			{
				byte[] array = Common.SerializeObjectAsJson(MatchHistory._matches);
				await FileHelper.SaveFileAsync(MatchHistory.HistoryFilePath, array);
			}
			catch (Exception ex)
			{
				MatchHistory.PrintDebugLog(string.Format("Exception occured when serializing matches: {0}", ex));
			}
			MatchHistory._isHistoryCacheDirty = true;
		}

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

		private const int MaxMatchCountPerMatchType = 10;

		private const string HistoryDirectoryName = "Data";

		private const string HistoryFileName = "History.json";

		private static bool _isHistoryCacheDirty = true;

		private static MBList<MatchInfo> _matches = new MBList<MatchInfo>();
	}
}
