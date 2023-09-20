using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Newtonsoft.Json;
using TaleWorlds.Library;
using TaleWorlds.PlayerServices;

namespace TaleWorlds.MountAndBlade.Diamond
{
	// Token: 0x02000149 RID: 329
	public static class RecentPlayersManager
	{
		// Token: 0x170002E6 RID: 742
		// (get) Token: 0x06000844 RID: 2116 RVA: 0x0000D29C File Offset: 0x0000B49C
		private static PlatformFilePath RecentPlayerFilePath
		{
			get
			{
				PlatformDirectoryPath platformDirectoryPath = new PlatformDirectoryPath(PlatformFileType.User, "Data");
				return new PlatformFilePath(platformDirectoryPath, "RecentPlayers.json");
			}
		}

		// Token: 0x170002E7 RID: 743
		// (get) Token: 0x06000845 RID: 2117 RVA: 0x0000D2C1 File Offset: 0x0000B4C1
		public static MBReadOnlyList<RecentPlayerInfo> RecentPlayers
		{
			get
			{
				return RecentPlayersManager._recentPlayers;
			}
		}

		// Token: 0x06000847 RID: 2119 RVA: 0x0000D334 File Offset: 0x0000B534
		public static async void Initialize()
		{
			await RecentPlayersManager.LoadRecentPlayers();
			RecentPlayersManager.DecayPlayers();
		}

		// Token: 0x06000848 RID: 2120 RVA: 0x0000D368 File Offset: 0x0000B568
		private static async Task LoadRecentPlayers()
		{
			if (RecentPlayersManager.IsRecentPlayersCacheDirty)
			{
				if (Common.PlatformFileHelper.FileExists(RecentPlayersManager.RecentPlayerFilePath))
				{
					try
					{
						TaskAwaiter<string> taskAwaiter = FileHelper.GetFileContentStringAsync(RecentPlayersManager.RecentPlayerFilePath).GetAwaiter();
						if (!taskAwaiter.IsCompleted)
						{
							await taskAwaiter;
							TaskAwaiter<string> taskAwaiter2;
							taskAwaiter = taskAwaiter2;
							taskAwaiter2 = default(TaskAwaiter<string>);
						}
						RecentPlayersManager._recentPlayers = JsonConvert.DeserializeObject<MBList<RecentPlayerInfo>>(taskAwaiter.GetResult());
						if (RecentPlayersManager._recentPlayers == null)
						{
							RecentPlayersManager._recentPlayers = new MBList<RecentPlayerInfo>();
							throw new Exception("_recentPlayers were null.");
						}
					}
					catch (Exception ex)
					{
						Debug.FailedAssert("Could not recent players. " + ex.Message, "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade.Diamond\\RecentPlayersManager.cs", "LoadRecentPlayers", 80);
						try
						{
							FileHelper.DeleteFile(RecentPlayersManager.RecentPlayerFilePath);
						}
						catch (Exception ex2)
						{
							Debug.FailedAssert("Could not delete recent players file. " + ex2.Message, "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade.Diamond\\RecentPlayersManager.cs", "LoadRecentPlayers", 87);
						}
					}
				}
				RecentPlayersManager.IsRecentPlayersCacheDirty = false;
			}
		}

		// Token: 0x06000849 RID: 2121 RVA: 0x0000D3A8 File Offset: 0x0000B5A8
		public static async Task<MBReadOnlyList<RecentPlayerInfo>> GetRecentPlayerInfos()
		{
			await RecentPlayersManager.LoadRecentPlayers();
			return RecentPlayersManager.RecentPlayers;
		}

		// Token: 0x0600084A RID: 2122 RVA: 0x0000D3E5 File Offset: 0x0000B5E5
		public static PlayerId[] GetRecentPlayerIds()
		{
			return RecentPlayersManager._recentPlayers.Select((RecentPlayerInfo p) => PlayerId.FromString(p.PlayerId)).ToArray<PlayerId>();
		}

		// Token: 0x0600084B RID: 2123 RVA: 0x0000D418 File Offset: 0x0000B618
		public static void AddOrUpdatePlayerEntry(PlayerId playerId, string playerName, InteractionType interactionType, int forcedIndex)
		{
			if (forcedIndex == -1)
			{
				object lockObject = RecentPlayersManager._lockObject;
				lock (lockObject)
				{
					RecentPlayersManager.InteractionTypeInfo interactionTypeInfo = RecentPlayersManager.InteractionTypeScoreDictionary[interactionType];
					RecentPlayerInfo recentPlayerInfo = RecentPlayersManager.TryGetPlayer(playerId);
					if (recentPlayerInfo != null)
					{
						if (interactionTypeInfo.ProcessType == RecentPlayersManager.InteractionTypeInfo.InteractionProcessType.Cumulative)
						{
							recentPlayerInfo.ImportanceScore += interactionTypeInfo.Score;
						}
						else if (interactionTypeInfo.ProcessType == RecentPlayersManager.InteractionTypeInfo.InteractionProcessType.Fixed)
						{
							recentPlayerInfo.ImportanceScore += Math.Max(interactionTypeInfo.Score, recentPlayerInfo.ImportanceScore);
						}
						recentPlayerInfo.PlayerName = playerName;
						recentPlayerInfo.InteractionTime = DateTime.Now;
					}
					else
					{
						recentPlayerInfo = new RecentPlayerInfo();
						recentPlayerInfo.PlayerId = playerId.ToString();
						recentPlayerInfo.ImportanceScore = interactionTypeInfo.Score;
						recentPlayerInfo.InteractionTime = DateTime.Now;
						recentPlayerInfo.PlayerName = playerName;
						RecentPlayersManager._recentPlayers.Add(recentPlayerInfo);
					}
					Action<PlayerId, InteractionType> onRecentPlayerInteraction = RecentPlayersManager.OnRecentPlayerInteraction;
					if (onRecentPlayerInteraction != null)
					{
						onRecentPlayerInteraction(playerId, interactionType);
					}
				}
			}
		}

		// Token: 0x14000001 RID: 1
		// (add) Token: 0x0600084C RID: 2124 RVA: 0x0000D51C File Offset: 0x0000B71C
		// (remove) Token: 0x0600084D RID: 2125 RVA: 0x0000D550 File Offset: 0x0000B750
		public static event Action<PlayerId, InteractionType> OnRecentPlayerInteraction;

		// Token: 0x0600084E RID: 2126 RVA: 0x0000D584 File Offset: 0x0000B784
		private static void DecayPlayers()
		{
			object lockObject = RecentPlayersManager._lockObject;
			lock (lockObject)
			{
				List<RecentPlayerInfo> list = new List<RecentPlayerInfo>();
				DateTime now = DateTime.Now;
				foreach (RecentPlayerInfo recentPlayerInfo in RecentPlayersManager._recentPlayers)
				{
					recentPlayerInfo.ImportanceScore -= (int)(now - recentPlayerInfo.InteractionTime).TotalHours;
					if (recentPlayerInfo.ImportanceScore <= 0)
					{
						list.Add(recentPlayerInfo);
					}
				}
				foreach (RecentPlayerInfo recentPlayerInfo2 in list)
				{
					RecentPlayersManager._recentPlayers.Remove(recentPlayerInfo2);
				}
			}
		}

		// Token: 0x0600084F RID: 2127 RVA: 0x0000D680 File Offset: 0x0000B880
		public static void Serialize()
		{
			try
			{
				byte[] array = Common.SerializeObjectAsJson(RecentPlayersManager._recentPlayers);
				FileHelper.SaveFile(RecentPlayersManager.RecentPlayerFilePath, array);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
			}
		}

		// Token: 0x06000850 RID: 2128 RVA: 0x0000D6C0 File Offset: 0x0000B8C0
		public static IEnumerable<PlayerId> GetPlayersOrdered()
		{
			return from p in RecentPlayersManager._recentPlayers
				orderby p.InteractionTime descending
				select PlayerId.FromString(p.PlayerId);
		}

		// Token: 0x06000851 RID: 2129 RVA: 0x0000D71C File Offset: 0x0000B91C
		private static RecentPlayerInfo TryGetPlayer(PlayerId playerId)
		{
			string text = playerId.ToString();
			foreach (RecentPlayerInfo recentPlayerInfo in RecentPlayersManager._recentPlayers)
			{
				if (recentPlayerInfo.PlayerId == text)
				{
					return recentPlayerInfo;
				}
			}
			return null;
		}

		// Token: 0x040003BD RID: 957
		private const string RecentPlayersDirectoryName = "Data";

		// Token: 0x040003BE RID: 958
		private const string RecentPlayersFileName = "RecentPlayers.json";

		// Token: 0x040003BF RID: 959
		private static bool IsRecentPlayersCacheDirty = true;

		// Token: 0x040003C0 RID: 960
		private static readonly object _lockObject = new object();

		// Token: 0x040003C1 RID: 961
		private static MBList<RecentPlayerInfo> _recentPlayers = new MBList<RecentPlayerInfo>();

		// Token: 0x040003C2 RID: 962
		private static readonly Dictionary<InteractionType, RecentPlayersManager.InteractionTypeInfo> InteractionTypeScoreDictionary = new Dictionary<InteractionType, RecentPlayersManager.InteractionTypeInfo>
		{
			{
				InteractionType.Killed,
				new RecentPlayersManager.InteractionTypeInfo(5, RecentPlayersManager.InteractionTypeInfo.InteractionProcessType.Cumulative)
			},
			{
				InteractionType.KilledBy,
				new RecentPlayersManager.InteractionTypeInfo(5, RecentPlayersManager.InteractionTypeInfo.InteractionProcessType.Cumulative)
			},
			{
				InteractionType.InGameTogether,
				new RecentPlayersManager.InteractionTypeInfo(24, RecentPlayersManager.InteractionTypeInfo.InteractionProcessType.Fixed)
			},
			{
				InteractionType.InPartyTogether,
				new RecentPlayersManager.InteractionTypeInfo(48, RecentPlayersManager.InteractionTypeInfo.InteractionProcessType.Fixed)
			}
		};

		// Token: 0x020001B7 RID: 439
		private class InteractionTypeInfo
		{
			// Token: 0x17000320 RID: 800
			// (get) Token: 0x06000982 RID: 2434 RVA: 0x00013FF2 File Offset: 0x000121F2
			// (set) Token: 0x06000983 RID: 2435 RVA: 0x00013FFA File Offset: 0x000121FA
			public int Score { get; private set; }

			// Token: 0x17000321 RID: 801
			// (get) Token: 0x06000984 RID: 2436 RVA: 0x00014003 File Offset: 0x00012203
			// (set) Token: 0x06000985 RID: 2437 RVA: 0x0001400B File Offset: 0x0001220B
			public RecentPlayersManager.InteractionTypeInfo.InteractionProcessType ProcessType { get; private set; }

			// Token: 0x06000986 RID: 2438 RVA: 0x00014014 File Offset: 0x00012214
			public InteractionTypeInfo(int score, RecentPlayersManager.InteractionTypeInfo.InteractionProcessType type)
			{
				this.Score = score;
				this.ProcessType = type;
			}

			// Token: 0x020001BC RID: 444
			public enum InteractionProcessType
			{
				// Token: 0x04000659 RID: 1625
				Cumulative,
				// Token: 0x0400065A RID: 1626
				Fixed
			}
		}
	}
}
