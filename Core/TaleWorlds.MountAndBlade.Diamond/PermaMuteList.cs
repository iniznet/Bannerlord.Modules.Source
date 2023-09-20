using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Newtonsoft.Json;
using TaleWorlds.Library;
using TaleWorlds.PlayerServices;

namespace TaleWorlds.MountAndBlade.Diamond
{
	public static class PermaMuteList
	{
		public static bool HasMutedPlayersLoaded { get; private set; }

		private static PlatformFilePath PermaMuteFilePath
		{
			get
			{
				PlatformDirectoryPath platformDirectoryPath = new PlatformDirectoryPath(PlatformFileType.User, "Data");
				return new PlatformFilePath(platformDirectoryPath, "Muted.json");
			}
		}

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

		public static void SetPermanentMuteAvailableCallback(Func<bool> getPermanentMuteAvailable)
		{
			PermaMuteList._getPermanentMuteAvailable = getPermanentMuteAvailable;
		}

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

		[TupleElementNames(new string[] { "Id", "Name" })]
		private static Dictionary<string, List<ValueTuple<string, string>>> _mutedPlayers = new Dictionary<string, List<ValueTuple<string, string>>>();

		private static string CurrentPlayerId;

		private static Func<bool> _getPermanentMuteAvailable;
	}
}
