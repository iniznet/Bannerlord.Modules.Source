using System;
using System.Collections.Generic;
using System.Linq;
using SandBox.ViewModelCollection.Input;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Extensions;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Engine;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.ModuleManager;
using TaleWorlds.MountAndBlade;
using TaleWorlds.SaveSystem;
using TaleWorlds.ScreenSystem;

namespace SandBox.ViewModelCollection.SaveLoad
{
	// Token: 0x02000012 RID: 18
	public class SaveLoadVM : ViewModel
	{
		// Token: 0x17000088 RID: 136
		// (get) Token: 0x06000180 RID: 384 RVA: 0x0000854A File Offset: 0x0000674A
		// (set) Token: 0x06000181 RID: 385 RVA: 0x00008552 File Offset: 0x00006752
		public bool IsBusyWithAnAction { get; private set; }

		// Token: 0x17000089 RID: 137
		// (get) Token: 0x06000182 RID: 386 RVA: 0x0000855B File Offset: 0x0000675B
		private IEnumerable<SaveGameFileInfo> _allSavedGames
		{
			get
			{
				return this.SaveGroups.SelectMany((SavedGameGroupVM s) => s.SavedGamesList.Select((SavedGameVM v) => v.Save));
			}
		}

		// Token: 0x1700008A RID: 138
		// (get) Token: 0x06000183 RID: 387 RVA: 0x00008587 File Offset: 0x00006787
		private SavedGameVM _defaultFirstSavedGame
		{
			get
			{
				SavedGameGroupVM savedGameGroupVM = this.SaveGroups.FirstOrDefault((SavedGameGroupVM x) => x.SavedGamesList.Count > 0);
				if (savedGameGroupVM == null)
				{
					return null;
				}
				return savedGameGroupVM.SavedGamesList.FirstOrDefault<SavedGameVM>();
			}
		}

		// Token: 0x06000184 RID: 388 RVA: 0x000085C4 File Offset: 0x000067C4
		public SaveLoadVM(bool isSaving, bool isCampaignMapOnStack)
		{
			this._isSaving = isSaving;
			this.SaveGroups = new MBBindingList<SavedGameGroupVM>();
			this.IsVisualDisabled = false;
			List<ModuleInfo> moduleInfos = ModuleHelper.GetModuleInfos(Utilities.GetModulesNames());
			int num = 0;
			SaveGameFileInfo[] saveFiles = MBSaveLoad.GetSaveFiles(null);
			IEnumerable<SaveGameFileInfo> enumerable = saveFiles.Where((SaveGameFileInfo s) => s.IsCorrupted);
			foreach (IGrouping<string, SaveGameFileInfo> grouping in from s in saveFiles
				where !s.IsCorrupted
				select s into m
				group m by MetaDataExtensions.GetUniqueGameId(m.MetaData) into s
				orderby this.GetMostRecentSaveInGroup(s) descending
				select s)
			{
				SavedGameGroupVM savedGameGroupVM = new SavedGameGroupVM();
				if (string.IsNullOrWhiteSpace(grouping.Key))
				{
					savedGameGroupVM.IdentifierID = this._uncategorizedSaveGroupName.ToString();
				}
				else
				{
					num++;
					this._categorizedSaveGroupName.SetTextVariable("ID", num);
					savedGameGroupVM.IdentifierID = this._categorizedSaveGroupName.ToString();
				}
				foreach (SaveGameFileInfo saveGameFileInfo in grouping.OrderByDescending((SaveGameFileInfo s) => MetaDataExtensions.GetCreationTime(s.MetaData)))
				{
					bool flag = SaveLoadVM.IsAnyModuleMissingFromSaveOrCurrentModules(moduleInfos, MetaDataExtensions.GetModules(saveGameFileInfo.MetaData));
					savedGameGroupVM.SavedGamesList.Add(new SavedGameVM(saveGameFileInfo, this.IsSaving, new Action<SavedGameVM>(this.OnDeleteSavedGame), new Action<SavedGameVM>(this.OnSaveSelection), new Action(this.OnCancelLoadSave), new Action(this.ExecuteDone), false, flag));
				}
				this.SaveGroups.Add(savedGameGroupVM);
			}
			if (enumerable.Any<SaveGameFileInfo>())
			{
				SavedGameGroupVM savedGameGroupVM2 = new SavedGameGroupVM
				{
					IdentifierID = new TextObject("{=o9PIe7am}Corrupted", null).ToString()
				};
				foreach (SaveGameFileInfo saveGameFileInfo2 in enumerable)
				{
					savedGameGroupVM2.SavedGamesList.Add(new SavedGameVM(saveGameFileInfo2, this.IsSaving, new Action<SavedGameVM>(this.OnDeleteSavedGame), new Action<SavedGameVM>(this.OnSaveSelection), new Action(this.OnCancelLoadSave), new Action(this.ExecuteDone), true, false));
				}
				this.SaveGroups.Add(savedGameGroupVM2);
			}
			this.RefreshCanCreateNewSave();
			this.OnSaveSelection(this._defaultFirstSavedGame);
			this.RefreshValues();
		}

		// Token: 0x06000185 RID: 389 RVA: 0x0000895C File Offset: 0x00006B5C
		private void RefreshCanCreateNewSave()
		{
			this.CanCreateNewSave = !MBSaveLoad.IsMaxNumberOfSavesReached();
			this.CreateNewSaveHint = new HintViewModel(this.CanCreateNewSave ? TextObject.Empty : new TextObject("{=DeXfSjgY}Cannot create a new save. Save limit reached.", null), null);
		}

		// Token: 0x06000186 RID: 390 RVA: 0x00008994 File Offset: 0x00006B94
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.TitleText = new TextObject("{=hiCxFj4E}Saved Campaigns", null).ToString();
			this.DoneText = new TextObject("{=WiNRdfsm}Done", null).ToString();
			this.CreateNewSaveSlotText = new TextObject("{=eL8nhkhQ}Create New Save Slot", null).ToString();
			this.CancelText = new TextObject("{=3CpNUnVl}Cancel", null).ToString();
			this.SaveLoadText = (this._isSaving ? new TextObject("{=bV75iwKa}Save", null).ToString() : new TextObject("{=9NuttOBC}Load", null).ToString());
			this.SearchPlaceholderText = new TextObject("{=tQOPRBFg}Search...", null).ToString();
			if (this.IsVisualDisabled)
			{
				this.VisualDisabledText = this._visualIsDisabledText.ToString();
			}
			this.SaveGroups.ApplyActionOnAllItems(delegate(SavedGameGroupVM x)
			{
				x.RefreshValues();
			});
			SavedGameVM currentSelectedSave = this.CurrentSelectedSave;
			if (currentSelectedSave == null)
			{
				return;
			}
			currentSelectedSave.RefreshValues();
		}

		// Token: 0x06000187 RID: 391 RVA: 0x00008A98 File Offset: 0x00006C98
		private DateTime GetMostRecentSaveInGroup(IGrouping<string, SaveGameFileInfo> group)
		{
			SaveGameFileInfo saveGameFileInfo = group.OrderByDescending((SaveGameFileInfo g) => MetaDataExtensions.GetCreationTime(g.MetaData)).FirstOrDefault<SaveGameFileInfo>();
			if (saveGameFileInfo == null)
			{
				return default(DateTime);
			}
			return MetaDataExtensions.GetCreationTime(saveGameFileInfo.MetaData);
		}

		// Token: 0x06000188 RID: 392 RVA: 0x00008AE8 File Offset: 0x00006CE8
		private void OnSaveSelection(SavedGameVM save)
		{
			if (save != this.CurrentSelectedSave)
			{
				if (this.CurrentSelectedSave != null)
				{
					this.CurrentSelectedSave.IsSelected = false;
				}
				this.CurrentSelectedSave = save;
				if (this.CurrentSelectedSave != null)
				{
					this.CurrentSelectedSave.IsSelected = true;
				}
				this.IsAnyItemSelected = this.CurrentSelectedSave != null;
				this.IsActionEnabled = this.IsAnyItemSelected && !this.CurrentSelectedSave.IsCorrupted;
			}
		}

		// Token: 0x06000189 RID: 393 RVA: 0x00008B5C File Offset: 0x00006D5C
		public void ExecuteCreateNewSaveGame()
		{
			InformationManager.ShowTextInquiry(new TextInquiryData(new TextObject("{=7WdWK2Dt}Save Game", null).ToString(), new TextObject("{=WDlVhNuq}Name your save file", null).ToString(), true, true, new TextObject("{=WiNRdfsm}Done", null).ToString(), new TextObject("{=3CpNUnVl}Cancel", null).ToString(), new Action<string>(this.OnSaveAsDone), null, false, new Func<string, Tuple<bool, string>>(this.IsSaveGameNameApplicable), "", ""), false, false);
		}

		// Token: 0x0600018A RID: 394 RVA: 0x00008BDC File Offset: 0x00006DDC
		private Tuple<bool, string> IsSaveGameNameApplicable(string inputText)
		{
			string text = string.Empty;
			bool flag = true;
			if (string.IsNullOrEmpty(inputText))
			{
				text = this._textIsEmptyReasonText.ToString();
				flag = false;
			}
			else if (inputText.All((char c) => char.IsWhiteSpace(c)))
			{
				text = this._allSpaceReasonText.ToString();
				flag = false;
			}
			else if (inputText.Any((char c) => !char.IsLetterOrDigit(c) && !char.IsWhiteSpace(c)))
			{
				text = this._textHasSpecialCharReasonText.ToString();
				flag = false;
			}
			else if (inputText.Length >= 30)
			{
				this._textTooLongReasonText.SetTextVariable("MAX_LENGTH", 30);
				text = this._textTooLongReasonText.ToString();
				flag = false;
			}
			else if (MBSaveLoad.IsSaveFileNameReserved(inputText))
			{
				text = this._saveNameReservedReasonText.ToString();
				flag = false;
			}
			else if (this._allSavedGames.Any((SaveGameFileInfo s) => string.Equals(s.Name, inputText, StringComparison.InvariantCultureIgnoreCase)))
			{
				text = this._saveAlreadyExistsReasonText.ToString();
				flag = false;
			}
			return new Tuple<bool, string>(flag, text);
		}

		// Token: 0x0600018B RID: 395 RVA: 0x00008D16 File Offset: 0x00006F16
		private void OnSaveAsDone(string saveName)
		{
			Campaign.Current.SaveHandler.SaveAs(saveName);
			this.ExecuteDone();
		}

		// Token: 0x0600018C RID: 396 RVA: 0x00008D2E File Offset: 0x00006F2E
		public void ExecuteDone()
		{
			ScreenManager.PopScreen();
		}

		// Token: 0x0600018D RID: 397 RVA: 0x00008D35 File Offset: 0x00006F35
		public void ExecuteLoadSave()
		{
			this.LoadSelectedSave();
		}

		// Token: 0x0600018E RID: 398 RVA: 0x00008D3D File Offset: 0x00006F3D
		private void LoadSelectedSave()
		{
			if (!this.IsBusyWithAnAction && this.CurrentSelectedSave != null && !this.CurrentSelectedSave.IsCorrupted)
			{
				this.CurrentSelectedSave.ExecuteSaveLoad();
				this.IsBusyWithAnAction = true;
			}
		}

		// Token: 0x0600018F RID: 399 RVA: 0x00008D6E File Offset: 0x00006F6E
		private void OnCancelLoadSave()
		{
			this.IsBusyWithAnAction = false;
		}

		// Token: 0x06000190 RID: 400 RVA: 0x00008D77 File Offset: 0x00006F77
		private void ExecuteResetCurrentSave()
		{
			this.CurrentSelectedSave = null;
		}

		// Token: 0x06000191 RID: 401 RVA: 0x00008D80 File Offset: 0x00006F80
		private void OnDeleteSavedGame(SavedGameVM savedGame)
		{
			if (!this.IsBusyWithAnAction)
			{
				this.IsBusyWithAnAction = true;
				string text = new TextObject("{=M1AEHJ76}Please notice that this save is created for a session which has Ironman mode enabled. There is no other save file for the related session. Are you sure you want to delete this save game?", null).ToString();
				string text2 = new TextObject("{=HH2mZq8J}Are you sure you want to delete this save game?", null).ToString();
				string text3 = new TextObject("{=QHV8aeEg}Delete Save", null).ToString();
				string text4 = (MetaDataExtensions.GetIronmanMode(savedGame.Save.MetaData) ? text : text2);
				InformationManager.ShowInquiry(new InquiryData(text3, text4, true, true, new TextObject("{=aeouhelq}Yes", null).ToString(), new TextObject("{=8OkPHu4f}No", null).ToString(), delegate
				{
					this.IsBusyWithAnAction = true;
					bool flag = MBSaveLoad.DeleteSaveGame(savedGame.Save.Name);
					this.IsBusyWithAnAction = false;
					if (flag)
					{
						this.DeleteSave(savedGame);
						this.OnSaveSelection(this._defaultFirstSavedGame);
						this.RefreshCanCreateNewSave();
						return;
					}
					this.OnDeleteSaveUnsuccessful();
				}, delegate
				{
					this.IsBusyWithAnAction = false;
				}, "", 0f, null, null, null), false, false);
			}
		}

		// Token: 0x06000192 RID: 402 RVA: 0x00008E58 File Offset: 0x00007058
		private void OnDeleteSaveUnsuccessful()
		{
			string text = new TextObject("{=oZrVNUOk}Error", null).ToString();
			string text2 = new TextObject("{=PY00wRz4}Failed to delete the save file.", null).ToString();
			InformationManager.ShowInquiry(new InquiryData(text, text2, true, false, new TextObject("{=WiNRdfsm}Done", null).ToString(), string.Empty, null, null, "", 0f, null, null, null), false, false);
		}

		// Token: 0x06000193 RID: 403 RVA: 0x00008EBC File Offset: 0x000070BC
		private void DeleteSave(SavedGameVM save)
		{
			foreach (SavedGameGroupVM savedGameGroupVM in this.SaveGroups)
			{
				if (savedGameGroupVM.SavedGamesList.Contains(save))
				{
					savedGameGroupVM.SavedGamesList.Remove(save);
					break;
				}
			}
			if (string.IsNullOrEmpty(BannerlordConfig.LatestSaveGameName) || save.Save.Name == BannerlordConfig.LatestSaveGameName)
			{
				SavedGameVM defaultFirstSavedGame = this._defaultFirstSavedGame;
				BannerlordConfig.LatestSaveGameName = ((defaultFirstSavedGame != null) ? defaultFirstSavedGame.Save.Name : null) ?? string.Empty;
				BannerlordConfig.Save();
			}
		}

		// Token: 0x06000194 RID: 404 RVA: 0x00008F70 File Offset: 0x00007170
		public void DeleteSelectedSave()
		{
			if (this.CurrentSelectedSave != null)
			{
				this.OnDeleteSavedGame(this.CurrentSelectedSave);
			}
		}

		// Token: 0x06000195 RID: 405 RVA: 0x00008F86 File Offset: 0x00007186
		public override void OnFinalize()
		{
			base.OnFinalize();
			InputKeyItemVM doneInputKey = this.DoneInputKey;
			if (doneInputKey != null)
			{
				doneInputKey.OnFinalize();
			}
			InputKeyItemVM cancelInputKey = this.CancelInputKey;
			if (cancelInputKey != null)
			{
				cancelInputKey.OnFinalize();
			}
			InputKeyItemVM deleteInputKey = this.DeleteInputKey;
			if (deleteInputKey == null)
			{
				return;
			}
			deleteInputKey.OnFinalize();
		}

		// Token: 0x06000196 RID: 406 RVA: 0x00008FC0 File Offset: 0x000071C0
		private static bool IsAnyModuleMissingFromSaveOrCurrentModules(List<ModuleInfo> loadedModules, string[] modulesInSave)
		{
			string[] modulesInSave2 = modulesInSave;
			for (int i = 0; i < modulesInSave2.Length; i++)
			{
				string moduleName = modulesInSave2[i];
				if (loadedModules.All((ModuleInfo loadedModule) => loadedModule.Name != moduleName))
				{
					return true;
				}
			}
			Func<ModuleInfo, bool> <>9__1;
			Func<ModuleInfo, bool> func;
			if ((func = <>9__1) == null)
			{
				func = (<>9__1 = (ModuleInfo loadedModule) => modulesInSave.All((string moduleName) => loadedModule.Name != moduleName));
			}
			using (IEnumerator<ModuleInfo> enumerator = loadedModules.Where(func).GetEnumerator())
			{
				if (enumerator.MoveNext())
				{
					ModuleInfo moduleInfo = enumerator.Current;
					return true;
				}
			}
			return false;
		}

		// Token: 0x1700008B RID: 139
		// (get) Token: 0x06000197 RID: 407 RVA: 0x0000907C File Offset: 0x0000727C
		// (set) Token: 0x06000198 RID: 408 RVA: 0x00009084 File Offset: 0x00007284
		[DataSourceProperty]
		public bool IsSearchAvailable
		{
			get
			{
				return this._isSearchAvailable;
			}
			set
			{
				if (value != this._isSearchAvailable)
				{
					this._isSearchAvailable = value;
					base.OnPropertyChangedWithValue(value, "IsSearchAvailable");
				}
			}
		}

		// Token: 0x1700008C RID: 140
		// (get) Token: 0x06000199 RID: 409 RVA: 0x000090A2 File Offset: 0x000072A2
		// (set) Token: 0x0600019A RID: 410 RVA: 0x000090AA File Offset: 0x000072AA
		[DataSourceProperty]
		public string SearchText
		{
			get
			{
				return this._searchText;
			}
			set
			{
				if (value != this._searchText)
				{
					value.IndexOf(this._searchText ?? "");
					this._searchText = value;
					base.OnPropertyChangedWithValue<string>(value, "SearchText");
				}
			}
		}

		// Token: 0x1700008D RID: 141
		// (get) Token: 0x0600019B RID: 411 RVA: 0x000090E3 File Offset: 0x000072E3
		// (set) Token: 0x0600019C RID: 412 RVA: 0x000090EB File Offset: 0x000072EB
		[DataSourceProperty]
		public string SearchPlaceholderText
		{
			get
			{
				return this._searchPlaceholderText;
			}
			set
			{
				if (value != this._searchPlaceholderText)
				{
					this._searchPlaceholderText = value;
					base.OnPropertyChangedWithValue<string>(value, "SearchPlaceholderText");
				}
			}
		}

		// Token: 0x1700008E RID: 142
		// (get) Token: 0x0600019D RID: 413 RVA: 0x0000910E File Offset: 0x0000730E
		// (set) Token: 0x0600019E RID: 414 RVA: 0x00009116 File Offset: 0x00007316
		[DataSourceProperty]
		public string VisualDisabledText
		{
			get
			{
				return this._visualDisabledText;
			}
			set
			{
				if (value != this._visualDisabledText)
				{
					this._visualDisabledText = value;
					base.OnPropertyChangedWithValue<string>(value, "VisualDisabledText");
				}
			}
		}

		// Token: 0x1700008F RID: 143
		// (get) Token: 0x0600019F RID: 415 RVA: 0x00009139 File Offset: 0x00007339
		// (set) Token: 0x060001A0 RID: 416 RVA: 0x00009141 File Offset: 0x00007341
		[DataSourceProperty]
		public MBBindingList<SavedGameGroupVM> SaveGroups
		{
			get
			{
				return this._saveGroups;
			}
			set
			{
				if (value != this._saveGroups)
				{
					this._saveGroups = value;
					base.OnPropertyChangedWithValue<MBBindingList<SavedGameGroupVM>>(value, "SaveGroups");
				}
			}
		}

		// Token: 0x17000090 RID: 144
		// (get) Token: 0x060001A1 RID: 417 RVA: 0x0000915F File Offset: 0x0000735F
		// (set) Token: 0x060001A2 RID: 418 RVA: 0x00009167 File Offset: 0x00007367
		[DataSourceProperty]
		public SavedGameVM CurrentSelectedSave
		{
			get
			{
				return this._currentSelectedSave;
			}
			set
			{
				if (value != this._currentSelectedSave)
				{
					this._currentSelectedSave = value;
					base.OnPropertyChangedWithValue<SavedGameVM>(value, "CurrentSelectedSave");
				}
			}
		}

		// Token: 0x17000091 RID: 145
		// (get) Token: 0x060001A3 RID: 419 RVA: 0x00009185 File Offset: 0x00007385
		// (set) Token: 0x060001A4 RID: 420 RVA: 0x0000918D File Offset: 0x0000738D
		[DataSourceProperty]
		public string CreateNewSaveSlotText
		{
			get
			{
				return this._createNewSaveSlotText;
			}
			set
			{
				if (value != this._createNewSaveSlotText)
				{
					this._createNewSaveSlotText = value;
					base.OnPropertyChangedWithValue<string>(value, "CreateNewSaveSlotText");
				}
			}
		}

		// Token: 0x17000092 RID: 146
		// (get) Token: 0x060001A5 RID: 421 RVA: 0x000091B0 File Offset: 0x000073B0
		// (set) Token: 0x060001A6 RID: 422 RVA: 0x000091B8 File Offset: 0x000073B8
		[DataSourceProperty]
		public string TitleText
		{
			get
			{
				return this._titleText;
			}
			set
			{
				if (value != this._titleText)
				{
					this._titleText = value;
					base.OnPropertyChangedWithValue<string>(value, "TitleText");
				}
			}
		}

		// Token: 0x17000093 RID: 147
		// (get) Token: 0x060001A7 RID: 423 RVA: 0x000091DB File Offset: 0x000073DB
		// (set) Token: 0x060001A8 RID: 424 RVA: 0x000091E3 File Offset: 0x000073E3
		[DataSourceProperty]
		public string CancelText
		{
			get
			{
				return this._cancelText;
			}
			set
			{
				if (value != this._cancelText)
				{
					this._cancelText = value;
					base.OnPropertyChangedWithValue<string>(value, "CancelText");
				}
			}
		}

		// Token: 0x17000094 RID: 148
		// (get) Token: 0x060001A9 RID: 425 RVA: 0x00009206 File Offset: 0x00007406
		// (set) Token: 0x060001AA RID: 426 RVA: 0x0000920E File Offset: 0x0000740E
		[DataSourceProperty]
		public bool IsSaving
		{
			get
			{
				return this._isSaving;
			}
			set
			{
				if (value != this._isSaving)
				{
					this._isSaving = value;
					base.OnPropertyChangedWithValue(value, "IsSaving");
				}
			}
		}

		// Token: 0x17000095 RID: 149
		// (get) Token: 0x060001AB RID: 427 RVA: 0x0000922C File Offset: 0x0000742C
		// (set) Token: 0x060001AC RID: 428 RVA: 0x00009234 File Offset: 0x00007434
		[DataSourceProperty]
		public bool CanCreateNewSave
		{
			get
			{
				return this._canCreateNewSave;
			}
			set
			{
				if (value != this._canCreateNewSave)
				{
					this._canCreateNewSave = value;
					base.OnPropertyChangedWithValue(value, "CanCreateNewSave");
				}
			}
		}

		// Token: 0x17000096 RID: 150
		// (get) Token: 0x060001AD RID: 429 RVA: 0x00009252 File Offset: 0x00007452
		// (set) Token: 0x060001AE RID: 430 RVA: 0x0000925A File Offset: 0x0000745A
		[DataSourceProperty]
		public bool IsVisualDisabled
		{
			get
			{
				return this._isVisualDisabled;
			}
			set
			{
				if (value != this._isVisualDisabled)
				{
					this._isVisualDisabled = value;
					base.OnPropertyChangedWithValue(value, "IsVisualDisabled");
				}
			}
		}

		// Token: 0x17000097 RID: 151
		// (get) Token: 0x060001AF RID: 431 RVA: 0x00009278 File Offset: 0x00007478
		// (set) Token: 0x060001B0 RID: 432 RVA: 0x00009280 File Offset: 0x00007480
		[DataSourceProperty]
		public HintViewModel CreateNewSaveHint
		{
			get
			{
				return this._createNewSaveHint;
			}
			set
			{
				if (value != this._createNewSaveHint)
				{
					this._createNewSaveHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "CreateNewSaveHint");
				}
			}
		}

		// Token: 0x17000098 RID: 152
		// (get) Token: 0x060001B1 RID: 433 RVA: 0x0000929E File Offset: 0x0000749E
		// (set) Token: 0x060001B2 RID: 434 RVA: 0x000092A6 File Offset: 0x000074A6
		[DataSourceProperty]
		public bool IsActionEnabled
		{
			get
			{
				return this._isActionEnabled;
			}
			set
			{
				if (value != this._isActionEnabled)
				{
					this._isActionEnabled = value;
					base.OnPropertyChangedWithValue(value, "IsActionEnabled");
				}
			}
		}

		// Token: 0x17000099 RID: 153
		// (get) Token: 0x060001B3 RID: 435 RVA: 0x000092C4 File Offset: 0x000074C4
		// (set) Token: 0x060001B4 RID: 436 RVA: 0x000092CC File Offset: 0x000074CC
		[DataSourceProperty]
		public bool IsAnyItemSelected
		{
			get
			{
				return this._isAnyItemSelected;
			}
			set
			{
				if (value != this._isAnyItemSelected)
				{
					this._isAnyItemSelected = value;
					base.OnPropertyChangedWithValue(value, "IsAnyItemSelected");
				}
			}
		}

		// Token: 0x1700009A RID: 154
		// (get) Token: 0x060001B5 RID: 437 RVA: 0x000092EA File Offset: 0x000074EA
		// (set) Token: 0x060001B6 RID: 438 RVA: 0x000092F2 File Offset: 0x000074F2
		[DataSourceProperty]
		public string DoneText
		{
			get
			{
				return this._doneText;
			}
			set
			{
				if (value != this._doneText)
				{
					this._doneText = value;
					base.OnPropertyChangedWithValue<string>(value, "DoneText");
				}
			}
		}

		// Token: 0x1700009B RID: 155
		// (get) Token: 0x060001B7 RID: 439 RVA: 0x00009315 File Offset: 0x00007515
		// (set) Token: 0x060001B8 RID: 440 RVA: 0x0000931D File Offset: 0x0000751D
		[DataSourceProperty]
		public string SaveLoadText
		{
			get
			{
				return this._saveLoadText;
			}
			set
			{
				if (value != this._saveLoadText)
				{
					this._saveLoadText = value;
					base.OnPropertyChangedWithValue<string>(value, "SaveLoadText");
				}
			}
		}

		// Token: 0x060001B9 RID: 441 RVA: 0x00009340 File Offset: 0x00007540
		public void SetDoneInputKey(HotKey hotkey)
		{
			this.DoneInputKey = InputKeyItemVM.CreateFromHotKey(hotkey, true);
		}

		// Token: 0x060001BA RID: 442 RVA: 0x0000934F File Offset: 0x0000754F
		public void SetCancelInputKey(HotKey hotkey)
		{
			this.CancelInputKey = InputKeyItemVM.CreateFromHotKey(hotkey, true);
		}

		// Token: 0x060001BB RID: 443 RVA: 0x0000935E File Offset: 0x0000755E
		public void SetDeleteInputKey(HotKey hotkey)
		{
			this.DeleteInputKey = InputKeyItemVM.CreateFromHotKey(hotkey, true);
		}

		// Token: 0x1700009C RID: 156
		// (get) Token: 0x060001BC RID: 444 RVA: 0x0000936D File Offset: 0x0000756D
		// (set) Token: 0x060001BD RID: 445 RVA: 0x00009375 File Offset: 0x00007575
		public InputKeyItemVM DoneInputKey
		{
			get
			{
				return this._doneInputKey;
			}
			set
			{
				if (value != this._doneInputKey)
				{
					this._doneInputKey = value;
					base.OnPropertyChangedWithValue<InputKeyItemVM>(value, "DoneInputKey");
				}
			}
		}

		// Token: 0x1700009D RID: 157
		// (get) Token: 0x060001BE RID: 446 RVA: 0x00009393 File Offset: 0x00007593
		// (set) Token: 0x060001BF RID: 447 RVA: 0x0000939B File Offset: 0x0000759B
		public InputKeyItemVM CancelInputKey
		{
			get
			{
				return this._cancelInputKey;
			}
			set
			{
				if (value != this._cancelInputKey)
				{
					this._cancelInputKey = value;
					base.OnPropertyChangedWithValue<InputKeyItemVM>(value, "CancelInputKey");
				}
			}
		}

		// Token: 0x1700009E RID: 158
		// (get) Token: 0x060001C0 RID: 448 RVA: 0x000093B9 File Offset: 0x000075B9
		// (set) Token: 0x060001C1 RID: 449 RVA: 0x000093C1 File Offset: 0x000075C1
		public InputKeyItemVM DeleteInputKey
		{
			get
			{
				return this._deleteInputKey;
			}
			set
			{
				if (value != this._deleteInputKey)
				{
					this._deleteInputKey = value;
					base.OnPropertyChangedWithValue<InputKeyItemVM>(value, "DeleteInputKey");
				}
			}
		}

		// Token: 0x040000A0 RID: 160
		private const int _maxSaveFileNameLength = 30;

		// Token: 0x040000A2 RID: 162
		private readonly TextObject _categorizedSaveGroupName = new TextObject("{=nVGqjtaa}Campaign {ID}", null);

		// Token: 0x040000A3 RID: 163
		private readonly TextObject _uncategorizedSaveGroupName = new TextObject("{=uncategorized_save}Uncategorized", null);

		// Token: 0x040000A4 RID: 164
		private readonly TextObject _textIsEmptyReasonText = new TextObject("{=7AI8jA0b}Input text cannot be empty.", null);

		// Token: 0x040000A5 RID: 165
		private readonly TextObject _textHasSpecialCharReasonText = new TextObject("{=kXRdeawC}Input text cannot include special characters.", null);

		// Token: 0x040000A6 RID: 166
		private readonly TextObject _textTooLongReasonText = new TextObject("{=B3W6fcQX}Input text cannot be longer than {MAX_LENGTH} characters.", null);

		// Token: 0x040000A7 RID: 167
		private readonly TextObject _saveAlreadyExistsReasonText = new TextObject("{=aG6XMhA1}A saved game file already exists with this name.", null);

		// Token: 0x040000A8 RID: 168
		private readonly TextObject _saveNameReservedReasonText = new TextObject("{=M4WMKyE1}Input text includes reserved text.", null);

		// Token: 0x040000A9 RID: 169
		private readonly TextObject _allSpaceReasonText = new TextObject("{=Rtakaivj}Input text needs to include at least one non-space character.", null);

		// Token: 0x040000AA RID: 170
		private readonly TextObject _visualIsDisabledText = new TextObject("{=xlEZ02Qw}Character visual is disabled during 'Save As' on the campaign map.", null);

		// Token: 0x040000AB RID: 171
		private bool _isSearchAvailable;

		// Token: 0x040000AC RID: 172
		private string _searchText;

		// Token: 0x040000AD RID: 173
		private string _searchPlaceholderText;

		// Token: 0x040000AE RID: 174
		private string _doneText;

		// Token: 0x040000AF RID: 175
		private string _createNewSaveSlotText;

		// Token: 0x040000B0 RID: 176
		private string _titleText;

		// Token: 0x040000B1 RID: 177
		private string _visualDisabledText;

		// Token: 0x040000B2 RID: 178
		private bool _isSaving;

		// Token: 0x040000B3 RID: 179
		private bool _isActionEnabled;

		// Token: 0x040000B4 RID: 180
		private bool _isAnyItemSelected;

		// Token: 0x040000B5 RID: 181
		private bool _canCreateNewSave;

		// Token: 0x040000B6 RID: 182
		private bool _isVisualDisabled;

		// Token: 0x040000B7 RID: 183
		private string _saveLoadText;

		// Token: 0x040000B8 RID: 184
		private string _cancelText;

		// Token: 0x040000B9 RID: 185
		private HintViewModel _createNewSaveHint;

		// Token: 0x040000BA RID: 186
		private MBBindingList<SavedGameGroupVM> _saveGroups;

		// Token: 0x040000BB RID: 187
		private SavedGameVM _currentSelectedSave;

		// Token: 0x040000BC RID: 188
		private InputKeyItemVM _doneInputKey;

		// Token: 0x040000BD RID: 189
		private InputKeyItemVM _cancelInputKey;

		// Token: 0x040000BE RID: 190
		private InputKeyItemVM _deleteInputKey;
	}
}
