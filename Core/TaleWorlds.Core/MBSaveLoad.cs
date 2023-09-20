using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Library;
using TaleWorlds.ModuleManager;
using TaleWorlds.SaveSystem;
using TaleWorlds.SaveSystem.Load;

namespace TaleWorlds.Core
{
	public static class MBSaveLoad
	{
		public static ApplicationVersion LastLoadedGameVersion { get; private set; }

		public static bool IsUpdatingGameVersion
		{
			get
			{
				return MBSaveLoad.LastLoadedGameVersion < ApplicationVersion.FromParametersFile(null);
			}
		}

		public static int NumberOfCurrentSaves { get; private set; }

		private static string GetAutoSaveName()
		{
			return MBSaveLoad.AutoSaveNamePrefix + MBSaveLoad.AutoSaveIndex;
		}

		private static void IncrementAutoSaveIndex()
		{
			MBSaveLoad.AutoSaveIndex++;
			if (MBSaveLoad.AutoSaveIndex > 3)
			{
				MBSaveLoad.AutoSaveIndex = 1;
			}
		}

		private static void InitializeAutoSaveIndex(string saveName)
		{
			string text = string.Empty;
			if (saveName.StartsWith(MBSaveLoad.AutoSaveNamePrefix))
			{
				text = saveName;
			}
			else
			{
				foreach (string text2 in MBSaveLoad.GetSaveFileNames())
				{
					if (text2.StartsWith(MBSaveLoad.AutoSaveNamePrefix))
					{
						text = text2;
						break;
					}
				}
			}
			if (string.IsNullOrEmpty(text))
			{
				MBSaveLoad.AutoSaveIndex = 1;
				return;
			}
			int num;
			if (int.TryParse(text.Substring(MBSaveLoad.AutoSaveNamePrefix.Length), out num) && num > 0 && num <= 3)
			{
				MBSaveLoad.AutoSaveIndex = num;
			}
		}

		public static void SetSaveDriver(ISaveDriver saveDriver)
		{
			MBSaveLoad._saveDriver = saveDriver;
		}

		public static SaveGameFileInfo[] GetSaveFiles(Func<SaveGameFileInfo, bool> condition = null)
		{
			SaveGameFileInfo[] saveGameFileInfos = MBSaveLoad._saveDriver.GetSaveGameFileInfos();
			MBSaveLoad.NumberOfCurrentSaves = saveGameFileInfos.Length;
			List<SaveGameFileInfo> list = new List<SaveGameFileInfo>();
			foreach (SaveGameFileInfo saveGameFileInfo in saveGameFileInfos)
			{
				if (condition == null || condition(saveGameFileInfo))
				{
					list.Add(saveGameFileInfo);
				}
			}
			return list.OrderByDescending((SaveGameFileInfo info) => info.MetaData.GetCreationTime()).ToArray<SaveGameFileInfo>();
		}

		public static bool IsSaveGameFileExists(string saveFileName)
		{
			return MBSaveLoad._saveDriver.IsSaveGameFileExists(saveFileName);
		}

		public static string[] GetSaveFileNames()
		{
			return MBSaveLoad._saveDriver.GetSaveGameFileNames();
		}

		public static LoadResult LoadSaveGameData(string saveName)
		{
			MBSaveLoad.InitializeAutoSaveIndex(saveName);
			ISaveDriver saveDriver = MBSaveLoad._saveDriver;
			LoadResult loadResult = SaveManager.Load(saveName, saveDriver, true);
			if (loadResult.Successful)
			{
				MBSaveLoad.ActiveSaveSlotName = saveName;
				return loadResult;
			}
			Debug.Print("Error: Could not load the game!", 0, Debug.DebugColor.White, 17592186044416UL);
			return null;
		}

		public static SaveGameFileInfo GetSaveFileWithName(string saveName)
		{
			SaveGameFileInfo[] saveFiles = MBSaveLoad.GetSaveFiles((SaveGameFileInfo x) => x.Name.Equals(saveName));
			if (saveFiles.Length == 0)
			{
				return null;
			}
			return saveFiles[0];
		}

		public static void QuickSaveCurrentGame(CampaignSaveMetaDataArgs campaignMetaData, Action<ValueTuple<SaveResult, string>> onSaveCompleted)
		{
			if (MBSaveLoad.ActiveSaveSlotName == null)
			{
				MBSaveLoad.ActiveSaveSlotName = MBSaveLoad.GetNextAvailableSaveName();
			}
			MBSaveLoad.OverwriteSaveAux(campaignMetaData, MBSaveLoad.ActiveSaveSlotName, onSaveCompleted);
		}

		public static void AutoSaveCurrentGame(CampaignSaveMetaDataArgs campaignMetaData, Action<ValueTuple<SaveResult, string>> onSaveCompleted)
		{
			MBSaveLoad.IncrementAutoSaveIndex();
			string autoSaveName = MBSaveLoad.GetAutoSaveName();
			MBSaveLoad.OverwriteSaveAux(campaignMetaData, autoSaveName, onSaveCompleted);
		}

		public static void SaveAsCurrentGame(CampaignSaveMetaDataArgs campaignMetaData, string saveName, Action<ValueTuple<SaveResult, string>> onSaveCompleted)
		{
			MBSaveLoad.ActiveSaveSlotName = saveName;
			MBSaveLoad.OverwriteSaveAux(campaignMetaData, saveName, onSaveCompleted);
		}

		private static void OverwriteSaveAux(CampaignSaveMetaDataArgs campaignMetaData, string saveName, Action<ValueTuple<SaveResult, string>> onSaveCompleted)
		{
			MetaData saveMetaData = MBSaveLoad.GetSaveMetaData(campaignMetaData);
			bool isOverwritingExistingSave = MBSaveLoad.IsSaveGameFileExists(saveName);
			if (!MBSaveLoad.IsMaxNumberOfSavesReached() || isOverwritingExistingSave)
			{
				MBSaveLoad.OverwriteSaveFile(saveMetaData, saveName, delegate(SaveResult r)
				{
					Action<ValueTuple<SaveResult, string>> onSaveCompleted3 = onSaveCompleted;
					if (onSaveCompleted3 != null)
					{
						onSaveCompleted3(new ValueTuple<SaveResult, string>(r, saveName));
					}
					if (r == SaveResult.Success && !isOverwritingExistingSave)
					{
						MBSaveLoad.NumberOfCurrentSaves++;
					}
				});
				return;
			}
			Action<ValueTuple<SaveResult, string>> onSaveCompleted2 = onSaveCompleted;
			if (onSaveCompleted2 == null)
			{
				return;
			}
			onSaveCompleted2(new ValueTuple<SaveResult, string>(SaveResult.SaveLimitReached, string.Empty));
		}

		public static bool DeleteSaveGame(string saveName)
		{
			bool flag = MBSaveLoad._saveDriver.Delete(saveName);
			if (flag)
			{
				if (MBSaveLoad.NumberOfCurrentSaves > 0)
				{
					MBSaveLoad.NumberOfCurrentSaves--;
				}
				if (MBSaveLoad.ActiveSaveSlotName == saveName)
				{
					MBSaveLoad.ActiveSaveSlotName = null;
				}
			}
			return flag;
		}

		public static void Initialize(GameTextManager localizedTextProvider)
		{
			MBSaveLoad._textProvider = localizedTextProvider;
			MBSaveLoad.NumberOfCurrentSaves = MBSaveLoad._saveDriver.GetSaveGameFileInfos().Length;
		}

		public static void OnNewGame()
		{
			MBSaveLoad.ActiveSaveSlotName = null;
			MBSaveLoad.LastLoadedGameVersion = ApplicationVersion.FromParametersFile(null);
			MBSaveLoad.AutoSaveIndex = 0;
		}

		public static void OnGameDestroy()
		{
			MBSaveLoad.LastLoadedGameVersion = ApplicationVersion.Empty;
		}

		public static void OnStartGame(LoadResult loadResult)
		{
			MBSaveLoad.LastLoadedGameVersion = loadResult.MetaData.GetApplicationVersion();
		}

		public static bool IsSaveFileNameReserved(string name)
		{
			for (int i = 1; i <= 3; i++)
			{
				if (name == MBSaveLoad.AutoSaveNamePrefix + i)
				{
					return true;
				}
			}
			return false;
		}

		private static string GetNextAvailableSaveName()
		{
			uint num = 0U;
			foreach (string text in MBSaveLoad.GetSaveFileNames())
			{
				uint num2;
				if (text.StartsWith(MBSaveLoad.DefaultSaveGamePrefix) && uint.TryParse(text.Substring(MBSaveLoad.DefaultSaveGamePrefix.Length), out num2) && num2 > num)
				{
					num = num2;
				}
			}
			num += 1U;
			return MBSaveLoad.DefaultSaveGamePrefix + num.ToString("000");
		}

		private static void OverwriteSaveFile(MetaData metaData, string name, Action<SaveResult> onSaveCompleted)
		{
			try
			{
				MBSaveLoad.SaveGame(name, metaData, delegate(SaveResult r)
				{
					onSaveCompleted(r);
					MBSaveLoad.ShowErrorFromResult(r);
				});
			}
			catch
			{
				MBSaveLoad.ShowErrorFromResult(SaveResult.GeneralFailure);
			}
		}

		private static void ShowErrorFromResult(SaveResult result)
		{
			if (result != SaveResult.Success)
			{
				if (result == SaveResult.PlatformFileHelperFailure)
				{
					Debug.FailedAssert("Save Failed:\n" + Common.PlatformFileHelper.GetError(), "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.Core\\MBSaveLoad.cs", "ShowErrorFromResult", 309);
				}
				if (!MBSaveLoad.DoNotShowSaveErrorAgain)
				{
					InformationManager.ShowInquiry(new InquiryData(MBSaveLoad._textProvider.FindText("str_save_unsuccessful_title", null).ToString(), MBSaveLoad._textProvider.FindText("str_game_save_result", result.ToString()).ToString(), true, false, MBSaveLoad._textProvider.FindText("str_ok", null).ToString(), MBSaveLoad._textProvider.FindText("str_dont_show_again", null).ToString(), null, delegate
					{
						MBSaveLoad.DoNotShowSaveErrorAgain = true;
					}, "", 0f, null, null, null), false, false);
				}
			}
		}

		private static void SaveGame(string saveName, MetaData metaData, Action<SaveResult> onSaveCompleted)
		{
			ISaveDriver saveDriver = MBSaveLoad._saveDriver;
			try
			{
				Game.Current.Save(metaData, saveName, saveDriver, onSaveCompleted);
			}
			catch (Exception ex)
			{
				Debug.Print("Unable to create save game data", 0, Debug.DebugColor.White, 17592186044416UL);
				Debug.Print(ex.Message, 0, Debug.DebugColor.White, 17592186044416UL);
				Debug.SilentAssert(ModuleHelper.GetModules().Any((ModuleInfo m) => !m.IsOfficial), ex.Message, false, "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.Core\\MBSaveLoad.cs", "SaveGame", 342);
			}
		}

		private static MetaData GetSaveMetaData(CampaignSaveMetaDataArgs data)
		{
			MetaData metaData = new MetaData();
			List<ModuleInfo> moduleInfos = ModuleHelper.GetModuleInfos(data.ModuleNames);
			metaData["Modules"] = string.Join(";", moduleInfos.Select((ModuleInfo q) => q.Name));
			foreach (ModuleInfo moduleInfo in moduleInfos)
			{
				metaData["Module_" + moduleInfo.Name] = moduleInfo.Version.ToString();
			}
			metaData.Add("ApplicationVersion", ApplicationVersion.FromParametersFile(null).ToString());
			metaData.Add("CreationTime", DateTime.Now.Ticks.ToString());
			foreach (KeyValuePair<string, string> keyValuePair in data.OtherData)
			{
				metaData.Add(keyValuePair.Key, keyValuePair.Value);
			}
			return metaData;
		}

		public static int GetMaxNumberOfSaves()
		{
			return int.MaxValue;
		}

		public static bool IsMaxNumberOfSavesReached()
		{
			return MBSaveLoad.NumberOfCurrentSaves >= MBSaveLoad.GetMaxNumberOfSaves();
		}

		private const int MaxNumberOfAutoSaveFiles = 3;

		private static ISaveDriver _saveDriver = new FileDriver();

		private static int AutoSaveIndex = 0;

		private static string DefaultSaveGamePrefix = "save";

		private static string AutoSaveNamePrefix = MBSaveLoad.DefaultSaveGamePrefix + "auto";

		private static string ActiveSaveSlotName = null;

		private static GameTextManager _textProvider;

		private static bool DoNotShowSaveErrorAgain = false;
	}
}
