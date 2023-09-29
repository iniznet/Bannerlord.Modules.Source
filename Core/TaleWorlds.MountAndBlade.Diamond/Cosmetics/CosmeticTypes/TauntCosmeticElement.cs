using System;
using System.Collections.Generic;
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

		private static void ReadExistingSlotData()
		{
			if (TauntCosmeticElement._isReadingData)
			{
				Debug.FailedAssert("Trying to read taunt data concurrently, this shouldn't happen", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade.Diamond\\Cosmetics\\CosmeticTypes\\TauntCosmeticElement.cs", "ReadExistingSlotData", 50);
				return;
			}
			TauntCosmeticElement._isReadingData = true;
			PlatformFilePath dataFilePath = TauntCosmeticElement.GetDataFilePath();
			TauntCosmeticElement._localSlotData = null;
			if (FileHelper.FileExists(dataFilePath))
			{
				string fileContentString = FileHelper.GetFileContentString(dataFilePath);
				if (!string.IsNullOrEmpty(fileContentString))
				{
					TauntCosmeticElement._localSlotData = JsonConvert.DeserializeObject<Dictionary<string, List<ValueTuple<string, int>>>>(fileContentString);
				}
			}
			TauntCosmeticElement._isReadingData = false;
		}

		public static List<ValueTuple<string, int>> GetTauntIndicesForPlayer(string playerId)
		{
			TauntCosmeticElement.ReadExistingSlotData();
			List<ValueTuple<string, int>> list = null;
			Dictionary<string, List<ValueTuple<string, int>>> localSlotData = TauntCosmeticElement._localSlotData;
			if (localSlotData != null && localSlotData.TryGetValue(playerId, out list))
			{
				return list;
			}
			return null;
		}

		public static void SetTauntIndicesForPlayer(string playerBannerlordId, List<ValueTuple<string, int>> tauntIndices)
		{
			if (TauntCosmeticElement._isWritingData)
			{
				Debug.FailedAssert("Trying to write taunt data concurrently. This shouldn't happen.", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade.Diamond\\Cosmetics\\CosmeticTypes\\TauntCosmeticElement.cs", "SetTauntIndicesForPlayer", 90);
				return;
			}
			TauntCosmeticElement._isWritingData = true;
			TauntCosmeticElement.ReadExistingSlotData();
			Dictionary<string, List<ValueTuple<string, int>>> dictionary = TauntCosmeticElement._localSlotData ?? new Dictionary<string, List<ValueTuple<string, int>>>();
			PlatformFilePath dataFilePath = TauntCosmeticElement.GetDataFilePath();
			string text = playerBannerlordId.ToString();
			if (dictionary.ContainsKey(text))
			{
				dictionary.Remove(text);
			}
			dictionary.Add(text, tauntIndices);
			if (FileHelper.SaveFile(dataFilePath, Common.SerializeObjectAsJson(dictionary)) != SaveResult.Success)
			{
				Debug.FailedAssert("Failed to save taunt indices", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade.Diamond\\Cosmetics\\CosmeticTypes\\TauntCosmeticElement.cs", "SetTauntIndicesForPlayer", 114);
			}
			TauntCosmeticElement._isWritingData = false;
		}

		private static Dictionary<string, List<ValueTuple<string, int>>> _localSlotData = new Dictionary<string, List<ValueTuple<string, int>>>();

		private static bool _isReadingData;

		private static bool _isWritingData;

		private const string _dataFolder = "Data";

		private const string _dataFile = "Taunts.json";
	}
}
