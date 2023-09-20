using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Newtonsoft.Json;
using TaleWorlds.Library;
using TaleWorlds.PlayerServices;

namespace TaleWorlds.MountAndBlade.Diamond
{
	// Token: 0x02000135 RID: 309
	public static class PermaMuteList
	{
		// Token: 0x17000284 RID: 644
		// (get) Token: 0x06000751 RID: 1873 RVA: 0x0000BD36 File Offset: 0x00009F36
		// (set) Token: 0x06000752 RID: 1874 RVA: 0x0000BD3D File Offset: 0x00009F3D
		public static bool HasMutedPlayersLoaded { get; private set; }

		// Token: 0x17000285 RID: 645
		// (get) Token: 0x06000753 RID: 1875 RVA: 0x0000BD48 File Offset: 0x00009F48
		private static PlatformFilePath PermaMuteFilePath
		{
			get
			{
				PlatformDirectoryPath platformDirectoryPath = new PlatformDirectoryPath(PlatformFileType.User, "Data");
				return new PlatformFilePath(platformDirectoryPath, "Muted.json");
			}
		}

		// Token: 0x17000286 RID: 646
		// (get) Token: 0x06000754 RID: 1876 RVA: 0x0000BD70 File Offset: 0x00009F70
		[TupleElementNames(new string[] { "Id", "Name" })]
		public static IReadOnlyList<ValueTuple<string, string>> MutedPlayers
		{
			[return: TupleElementNames(new string[] { "Id", "Name" })]
			get
			{
				List<ValueTuple<string, string>> list;
				if (!PermaMuteList.HasMutedPlayersLoaded || !PermaMuteList._mutedPlayers.TryGetValue(PermaMuteList.CurrentPlayerId, out list))
				{
					return new List<ValueTuple<string, string>>();
				}
				return list;
			}
		}

		// Token: 0x06000756 RID: 1878 RVA: 0x0000BDAA File Offset: 0x00009FAA
		public static void SetPermanentMuteAvailableCallback(Func<bool> getPermanentMuteAvailable)
		{
			PermaMuteList._getPermanentMuteAvailable = getPermanentMuteAvailable;
		}

		// Token: 0x06000757 RID: 1879 RVA: 0x0000BDB4 File Offset: 0x00009FB4
		public static async Task LoadMutedPlayers(PlayerId currentPlayerId)
		{
			PermaMuteList.CurrentPlayerId = currentPlayerId.ToString();
			if (FileHelper.FileExists(PermaMuteList.PermaMuteFilePath))
			{
				try
				{
					Dictionary<string, List<ValueTuple<string, string>>> dictionary = JsonConvert.DeserializeObject<Dictionary<string, List<ValueTuple<string, string>>>>(await FileHelper.GetFileContentStringAsync(PermaMuteList.PermaMuteFilePath));
					if (dictionary != null)
					{
						PermaMuteList._mutedPlayers = dictionary;
					}
					PermaMuteList.HasMutedPlayersLoaded = true;
				}
				catch (Exception ex)
				{
					Debug.FailedAssert("Could not load muted players. " + ex.Message, "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade.Diamond\\PermaMuteList.cs", "LoadMutedPlayers", 61);
					try
					{
						FileHelper.DeleteFile(PermaMuteList.PermaMuteFilePath);
					}
					catch (Exception ex2)
					{
						Debug.FailedAssert("Could not delete muted players file. " + ex2.Message, "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade.Diamond\\PermaMuteList.cs", "LoadMutedPlayers", 68);
					}
				}
			}
		}

		// Token: 0x06000758 RID: 1880 RVA: 0x0000BDFC File Offset: 0x00009FFC
		public static async void SaveMutedPlayers()
		{
			try
			{
				byte[] array = Common.SerializeObjectAsJson(PermaMuteList._mutedPlayers);
				await FileHelper.SaveFileAsync(PermaMuteList.PermaMuteFilePath, array);
			}
			catch (Exception ex)
			{
				Debug.FailedAssert("Could not save muted players. " + ex.Message, "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade.Diamond\\PermaMuteList.cs", "SaveMutedPlayers", 83);
			}
		}

		// Token: 0x06000759 RID: 1881 RVA: 0x0000BE30 File Offset: 0x0000A030
		public static bool IsPlayerMuted(PlayerId player)
		{
			Func<bool> getPermanentMuteAvailable = PermaMuteList._getPermanentMuteAvailable;
			if ((getPermanentMuteAvailable == null || getPermanentMuteAvailable()) && PermaMuteList.CurrentPlayerId != null)
			{
				string text = player.ToString();
				Dictionary<string, List<ValueTuple<string, string>>> mutedPlayers = PermaMuteList._mutedPlayers;
				lock (mutedPlayers)
				{
					List<ValueTuple<string, string>> list;
					if (!PermaMuteList._mutedPlayers.TryGetValue(PermaMuteList.CurrentPlayerId, out list))
					{
						return false;
					}
					using (List<ValueTuple<string, string>>.Enumerator enumerator = list.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							if (enumerator.Current.Item1 == text)
							{
								return true;
							}
						}
					}
				}
				return false;
			}
			return false;
		}

		// Token: 0x0600075A RID: 1882 RVA: 0x0000BEF8 File Offset: 0x0000A0F8
		public static void MutePlayer(PlayerId player, string name)
		{
			Func<bool> getPermanentMuteAvailable = PermaMuteList._getPermanentMuteAvailable;
			if (getPermanentMuteAvailable == null || getPermanentMuteAvailable())
			{
				Dictionary<string, List<ValueTuple<string, string>>> mutedPlayers = PermaMuteList._mutedPlayers;
				lock (mutedPlayers)
				{
					List<ValueTuple<string, string>> list;
					if (!PermaMuteList._mutedPlayers.TryGetValue(PermaMuteList.CurrentPlayerId, out list))
					{
						list = new List<ValueTuple<string, string>>();
						PermaMuteList._mutedPlayers.Add(PermaMuteList.CurrentPlayerId, list);
					}
					list.Add(new ValueTuple<string, string>(player.ToString(), name));
				}
			}
		}

		// Token: 0x0600075B RID: 1883 RVA: 0x0000BF88 File Offset: 0x0000A188
		public static void RemoveMutedPlayer(PlayerId player)
		{
			Func<bool> getPermanentMuteAvailable = PermaMuteList._getPermanentMuteAvailable;
			if (getPermanentMuteAvailable == null || getPermanentMuteAvailable())
			{
				string text = player.ToString();
				Dictionary<string, List<ValueTuple<string, string>>> mutedPlayers = PermaMuteList._mutedPlayers;
				lock (mutedPlayers)
				{
					List<ValueTuple<string, string>> list;
					if (PermaMuteList._mutedPlayers.TryGetValue(PermaMuteList.CurrentPlayerId, out list))
					{
						int num = -1;
						for (int i = 0; i < list.Count; i++)
						{
							if (list[i].Item1 == text)
							{
								num = i;
								break;
							}
						}
						if (num >= 0)
						{
							list.RemoveAt(num);
						}
					}
				}
			}
		}

		// Token: 0x04000360 RID: 864
		[TupleElementNames(new string[] { "Id", "Name" })]
		private static Dictionary<string, List<ValueTuple<string, string>>> _mutedPlayers = new Dictionary<string, List<ValueTuple<string, string>>>();

		// Token: 0x04000361 RID: 865
		private static string CurrentPlayerId;

		// Token: 0x04000362 RID: 866
		private static Func<bool> _getPermanentMuteAvailable;
	}
}
