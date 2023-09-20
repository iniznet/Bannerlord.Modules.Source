using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Newtonsoft.Json;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.MountAndBlade.Diamond.Cosmetics.CosmeticTypes
{
	public class TauntCosmeticElement : CosmeticElement
	{
		public static int MaxNumberOfTaunts
		{
			get
			{
				return 6;
			}
		}

		public TextObject Name { get; }

		public TauntCosmeticElement(int index, string id, CosmeticsManager.CosmeticRarity rarity, int cost, string name)
			: base(id, rarity, cost, CosmeticsManager.CosmeticType.Taunt)
		{
			this.UsageIndex = index;
			this.Name = new TextObject(name, null);
		}

		private static PlatformFilePath GetDataFilePath()
		{
			return new PlatformFilePath(new PlatformDirectoryPath(PlatformFileType.User, "Data"), "Taunts.json");
		}

		private static async Task ReadExistingSlotDataAsync()
		{
			if (!TauntCosmeticElement._isReadingData)
			{
				TauntCosmeticElement._isReadingData = true;
				PlatformFilePath dataFilePath = TauntCosmeticElement.GetDataFilePath();
				TauntCosmeticElement._localSlotData = null;
				if (FileHelper.FileExists(dataFilePath))
				{
					string text = await FileHelper.GetFileContentStringAsync(dataFilePath);
					if (!string.IsNullOrEmpty(text))
					{
						TauntCosmeticElement._localSlotData = JsonConvert.DeserializeObject<Dictionary<string, List<ValueTuple<string, int>>>>(text);
					}
				}
				TauntCosmeticElement._isReadingData = false;
			}
		}

		public static async Task<List<ValueTuple<string, int>>> GetTauntIndicesForPlayerAsync(string playerId)
		{
			await TauntCosmeticElement.ReadExistingSlotDataAsync();
			List<ValueTuple<string, int>> list = null;
			Dictionary<string, List<ValueTuple<string, int>>> localSlotData = TauntCosmeticElement._localSlotData;
			List<ValueTuple<string, int>> list2;
			if (localSlotData != null && localSlotData.TryGetValue(playerId, out list))
			{
				list2 = list;
			}
			else
			{
				list2 = null;
			}
			return list2;
		}

		public static async Task SetTauntIndicesForPlayerAsync(string playerBannerlordId, List<ValueTuple<string, int>> tauntIndices)
		{
			await TauntCosmeticElement.ReadExistingSlotDataAsync();
			Dictionary<string, List<ValueTuple<string, int>>> dictionary = TauntCosmeticElement._localSlotData ?? new Dictionary<string, List<ValueTuple<string, int>>>();
			PlatformFilePath dataFilePath = TauntCosmeticElement.GetDataFilePath();
			string text = playerBannerlordId.ToString();
			if (dictionary.ContainsKey(text))
			{
				dictionary.Remove(text);
			}
			dictionary.Add(text, tauntIndices);
			TaskAwaiter<SaveResult> taskAwaiter = FileHelper.SaveFileAsync(dataFilePath, Common.SerializeObjectAsJson(dictionary)).GetAwaiter();
			if (!taskAwaiter.IsCompleted)
			{
				await taskAwaiter;
				TaskAwaiter<SaveResult> taskAwaiter2;
				taskAwaiter = taskAwaiter2;
				taskAwaiter2 = default(TaskAwaiter<SaveResult>);
			}
			if (taskAwaiter.GetResult() != SaveResult.Success)
			{
				Debug.FailedAssert("Failed to save taunt indices", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade.Diamond\\Cosmetics\\CosmeticTypes\\TauntCosmeticElement.cs", "SetTauntIndicesForPlayerAsync", 105);
			}
		}

		private static Dictionary<string, List<ValueTuple<string, int>>> _localSlotData = new Dictionary<string, List<ValueTuple<string, int>>>();

		private static bool _isReadingData;

		private const string _dataFolder = "Data";

		private const string _dataFile = "Taunts.json";
	}
}
