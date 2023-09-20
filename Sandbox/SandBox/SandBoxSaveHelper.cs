using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.ModuleManager;
using TaleWorlds.MountAndBlade;
using TaleWorlds.SaveSystem;
using TaleWorlds.SaveSystem.Load;

namespace SandBox
{
	public static class SandBoxSaveHelper
	{
		public static event Action<SandBoxSaveHelper.SaveHelperState> OnStateChange;

		public static void TryLoadSave(SaveGameFileInfo saveInfo, Action<LoadResult> onStartGame, Action onCancel = null)
		{
			SandBoxSaveHelper._newlineTextObject.SetTextVariable("newline", "\n");
			Action<SandBoxSaveHelper.SaveHelperState> onStateChange = SandBoxSaveHelper.OnStateChange;
			if (onStateChange != null)
			{
				onStateChange(SandBoxSaveHelper.SaveHelperState.Start);
			}
			bool flag = true;
			ApplicationVersion applicationVersion = MetaDataExtensions.GetApplicationVersion(saveInfo.MetaData);
			if (applicationVersion < SandBoxSaveHelper.SaveResetVersion)
			{
				InquiryData inquiryData = new InquiryData(SandBoxSaveHelper._moduleMissmatchInquiryTitle.ToString(), SandBoxSaveHelper._saveResetVersionProblemText.ToString(), true, false, new TextObject("{=yS7PvrTD}OK", null).ToString(), null, delegate
				{
					SandBoxSaveHelper._isInquiryActive = false;
					Action onCancel2 = onCancel;
					if (onCancel2 == null)
					{
						return;
					}
					onCancel2();
				}, null, "", 0f, null, null, null);
				SandBoxSaveHelper._isInquiryActive = true;
				InformationManager.ShowInquiry(inquiryData, false, false);
				Action<SandBoxSaveHelper.SaveHelperState> onStateChange2 = SandBoxSaveHelper.OnStateChange;
				if (onStateChange2 == null)
				{
					return;
				}
				onStateChange2(SandBoxSaveHelper.SaveHelperState.Inquiry);
				return;
			}
			else
			{
				List<SandBoxSaveHelper.ModuleCheckResult> list = SandBoxSaveHelper.CheckModules(saveInfo.MetaData);
				if (list.Count <= 0)
				{
					SandBoxSaveHelper.LoadGameAction(saveInfo, onStartGame, onCancel);
					return;
				}
				IEnumerable<IGrouping<ModuleCheckResultType, SandBoxSaveHelper.ModuleCheckResult>> enumerable = from m in list
					group m by m.Type;
				string text = string.Empty;
				GameTextManager globalTextManager = Module.CurrentModule.GlobalTextManager;
				foreach (IGrouping<ModuleCheckResultType, SandBoxSaveHelper.ModuleCheckResult> grouping in enumerable)
				{
					SandBoxSaveHelper._stringSpaceStringTextObject.SetTextVariable("STR1", globalTextManager.FindText("str_load_module_error", Enum.GetName(typeof(ModuleCheckResultType), grouping.Key)));
					SandBoxSaveHelper._stringSpaceStringTextObject.SetTextVariable("STR2", grouping.ElementAt(0).ModuleName);
					text += SandBoxSaveHelper._stringSpaceStringTextObject.ToString();
					for (int i = 1; i < grouping.Count<SandBoxSaveHelper.ModuleCheckResult>(); i++)
					{
						SandBoxSaveHelper._stringSpaceStringTextObject.SetTextVariable("STR1", text);
						SandBoxSaveHelper._stringSpaceStringTextObject.SetTextVariable("STR2", grouping.ElementAt(i).ModuleName);
						text = SandBoxSaveHelper._stringSpaceStringTextObject.ToString();
					}
					SandBoxSaveHelper._newlineTextObject.SetTextVariable("STR1", text);
					SandBoxSaveHelper._newlineTextObject.SetTextVariable("STR2", "");
					text = SandBoxSaveHelper._newlineTextObject.ToString();
				}
				SandBoxSaveHelper._newlineTextObject.SetTextVariable("STR1", text);
				SandBoxSaveHelper._newlineTextObject.SetTextVariable("STR2", " ");
				text = SandBoxSaveHelper._newlineTextObject.ToString();
				bool flag2 = ApplicationVersion.FromParametersFile(null) >= applicationVersion || flag;
				if (flag2)
				{
					SandBoxSaveHelper._newlineTextObject.SetTextVariable("STR1", text);
					SandBoxSaveHelper._newlineTextObject.SetTextVariable("STR2", new TextObject("{=lh0so0uX}Do you want to load the saved game with different modules?", null));
					text = SandBoxSaveHelper._newlineTextObject.ToString();
				}
				InquiryData inquiryData2 = new InquiryData(SandBoxSaveHelper._moduleMissmatchInquiryTitle.ToString(), text, flag2, true, new TextObject("{=aeouhelq}Yes", null).ToString(), new TextObject("{=3CpNUnVl}Cancel", null).ToString(), delegate
				{
					SandBoxSaveHelper._isInquiryActive = false;
					SandBoxSaveHelper.LoadGameAction(saveInfo, onStartGame, onCancel);
				}, delegate
				{
					SandBoxSaveHelper._isInquiryActive = false;
					Action onCancel3 = onCancel;
					if (onCancel3 == null)
					{
						return;
					}
					onCancel3();
				}, "", 0f, null, null, null);
				SandBoxSaveHelper._isInquiryActive = true;
				InformationManager.ShowInquiry(inquiryData2, false, false);
				Action<SandBoxSaveHelper.SaveHelperState> onStateChange3 = SandBoxSaveHelper.OnStateChange;
				if (onStateChange3 == null)
				{
					return;
				}
				onStateChange3(SandBoxSaveHelper.SaveHelperState.Inquiry);
				return;
			}
		}

		private static List<SandBoxSaveHelper.ModuleCheckResult> CheckModules(MetaData fileMetaData)
		{
			List<ModuleInfo> moduleInfos = ModuleHelper.GetModuleInfos(Utilities.GetModulesNames());
			string[] modulesInSaveFile = MetaDataExtensions.GetModules(fileMetaData);
			List<SandBoxSaveHelper.ModuleCheckResult> list = new List<SandBoxSaveHelper.ModuleCheckResult>();
			string[] modulesInSaveFile2 = modulesInSaveFile;
			for (int i = 0; i < modulesInSaveFile2.Length; i++)
			{
				string moduleName = modulesInSaveFile2[i];
				if (moduleInfos.All((ModuleInfo loadedModule) => loadedModule.Name != moduleName))
				{
					list.Add(new SandBoxSaveHelper.ModuleCheckResult(moduleName, 0));
				}
				else if (!MetaDataExtensions.GetModuleVersion(fileMetaData, moduleName).IsSame(moduleInfos.Single((ModuleInfo loadedModule) => loadedModule.Name == moduleName).Version))
				{
					list.Add(new SandBoxSaveHelper.ModuleCheckResult(moduleName, 2));
				}
			}
			IEnumerable<ModuleInfo> enumerable = moduleInfos;
			Func<ModuleInfo, bool> <>9__2;
			Func<ModuleInfo, bool> func;
			if ((func = <>9__2) == null)
			{
				func = (<>9__2 = (ModuleInfo loadedModule) => modulesInSaveFile.All((string moduleName) => loadedModule.Name != moduleName));
			}
			foreach (ModuleInfo moduleInfo in enumerable.Where(func))
			{
				list.Add(new SandBoxSaveHelper.ModuleCheckResult(moduleInfo.Name, 1));
			}
			return list;
		}

		private static void LoadGameAction(SaveGameFileInfo saveInfo, Action<LoadResult> onStartGame, Action onCancel)
		{
			Action<SandBoxSaveHelper.SaveHelperState> onStateChange = SandBoxSaveHelper.OnStateChange;
			if (onStateChange != null)
			{
				onStateChange(SandBoxSaveHelper.SaveHelperState.LoadGame);
			}
			LoadResult loadResult = MBSaveLoad.LoadSaveGameData(saveInfo.Name);
			if (loadResult != null)
			{
				if (onStartGame != null)
				{
					onStartGame(loadResult);
					return;
				}
			}
			else
			{
				InquiryData inquiryData = new InquiryData(SandBoxSaveHelper._errorTitle.ToString(), SandBoxSaveHelper._saveLoadingProblemText.ToString(), true, false, new TextObject("{=WiNRdfsm}Done", null).ToString(), string.Empty, delegate
				{
					SandBoxSaveHelper._isInquiryActive = false;
					Action onCancel2 = onCancel;
					if (onCancel2 == null)
					{
						return;
					}
					onCancel2();
				}, null, "", 0f, null, null, null);
				SandBoxSaveHelper._isInquiryActive = true;
				InformationManager.ShowInquiry(inquiryData, false, false);
				Action<SandBoxSaveHelper.SaveHelperState> onStateChange2 = SandBoxSaveHelper.OnStateChange;
				if (onStateChange2 == null)
				{
					return;
				}
				onStateChange2(SandBoxSaveHelper.SaveHelperState.Inquiry);
			}
		}

		private static readonly ApplicationVersion SaveResetVersion = new ApplicationVersion(2, 1, 7, 0, 0);

		private static readonly TextObject _stringSpaceStringTextObject = new TextObject("{=7AFlpaem}{STR1} {STR2}", null);

		private static readonly TextObject _newlineTextObject = new TextObject("{=ol0rBSrb}{STR1}{newline}{STR2}", null);

		private static readonly TextObject _moduleMissmatchInquiryTitle = new TextObject("{=r7xdYj4q}Module Mismatch", null);

		private static readonly TextObject _errorTitle = new TextObject("{=oZrVNUOk}Error", null);

		private static readonly TextObject _saveLoadingProblemText = new TextObject("{=onLDP7mP}A problem occured while trying to load the saved game.", null);

		private static readonly TextObject _saveResetVersionProblemText = new TextObject("{=5hbSkbQg}This save file is from a game version that is older than e1.7.0. Please switch your game version to e1.7.0, load the save file and save the game. This will allow it to work on newer versions beyond e1.7.0.", null);

		private static bool _isInquiryActive;

		public enum SaveHelperState
		{
			Start,
			Inquiry,
			LoadGame
		}

		private readonly struct ModuleCheckResult
		{
			public ModuleCheckResult(string moduleName, ModuleCheckResultType type)
			{
				this.ModuleName = moduleName;
				this.Type = type;
			}

			public readonly string ModuleName;

			public readonly ModuleCheckResultType Type;
		}
	}
}
