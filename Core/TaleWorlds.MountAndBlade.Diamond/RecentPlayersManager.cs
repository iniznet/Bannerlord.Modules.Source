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
	public static class RecentPlayersManager
	{
		private static PlatformFilePath RecentPlayerFilePath
		{
			get
			{
				PlatformDirectoryPath platformDirectoryPath = new PlatformDirectoryPath(PlatformFileType.User, "Data");
				return new PlatformFilePath(platformDirectoryPath, "RecentPlayers.json");
			}
		}

		public static MBReadOnlyList<RecentPlayerInfo> RecentPlayers
		{
			get
			{
				return RecentPlayersManager._recentPlayers;
			}
		}

		public static async void Initialize()
		{
			await RecentPlayersManager.LoadRecentPlayers();
			RecentPlayersManager.DecayPlayers();
		}

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

		public static async Task<MBReadOnlyList<RecentPlayerInfo>> GetRecentPlayerInfos()
		{
			await RecentPlayersManager.LoadRecentPlayers();
			return RecentPlayersManager.RecentPlayers;
		}

		public static PlayerId[] GetRecentPlayerIds()
		{
			return RecentPlayersManager._recentPlayers.Select((RecentPlayerInfo p) => PlayerId.FromString(p.PlayerId)).ToArray<PlayerId>();
		}

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

		public static event Action<PlayerId, InteractionType> OnRecentPlayerInteraction;

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

		public static IEnumerable<PlayerId> GetPlayersOrdered()
		{
			return from p in RecentPlayersManager._recentPlayers
				orderby p.InteractionTime descending
				select PlayerId.FromString(p.PlayerId);
		}

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

		private const string RecentPlayersDirectoryName = "Data";

		private const string RecentPlayersFileName = "RecentPlayers.json";

		private static bool IsRecentPlayersCacheDirty = true;

		private static readonly object _lockObject = new object();

		private static MBList<RecentPlayerInfo> _recentPlayers = new MBList<RecentPlayerInfo>();

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

		private class InteractionTypeInfo
		{
			public int Score { get; private set; }

			public RecentPlayersManager.InteractionTypeInfo.InteractionProcessType ProcessType { get; private set; }

			public InteractionTypeInfo(int score, RecentPlayersManager.InteractionTypeInfo.InteractionProcessType type)
			{
				this.Score = score;
				this.ProcessType = type;
			}

			public enum InteractionProcessType
			{
				Cumulative,
				Fixed
			}
		}
	}
}
