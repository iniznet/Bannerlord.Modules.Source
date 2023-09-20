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
	public class SaveLoadVM : ViewModel
	{
		public bool IsBusyWithAnAction { get; private set; }

		private IEnumerable<SaveGameFileInfo> _allSavedGames
		{
			get
			{
				return this.SaveGroups.SelectMany((SavedGameGroupVM s) => s.SavedGamesList.Select((SavedGameVM v) => v.Save));
			}
		}

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

		private void RefreshCanCreateNewSave()
		{
			this.CanCreateNewSave = !MBSaveLoad.IsMaxNumberOfSavesReached();
			this.CreateNewSaveHint = new HintViewModel(this.CanCreateNewSave ? TextObject.Empty : new TextObject("{=DeXfSjgY}Cannot create a new save. Save limit reached.", null), null);
		}

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

		private DateTime GetMostRecentSaveInGroup(IGrouping<string, SaveGameFileInfo> group)
		{
			SaveGameFileInfo saveGameFileInfo = group.OrderByDescending((SaveGameFileInfo g) => MetaDataExtensions.GetCreationTime(g.MetaData)).FirstOrDefault<SaveGameFileInfo>();
			if (saveGameFileInfo == null)
			{
				return default(DateTime);
			}
			return MetaDataExtensions.GetCreationTime(saveGameFileInfo.MetaData);
		}

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

		public void ExecuteCreateNewSaveGame()
		{
			InformationManager.ShowTextInquiry(new TextInquiryData(new TextObject("{=7WdWK2Dt}Save Game", null).ToString(), new TextObject("{=WDlVhNuq}Name your save file", null).ToString(), true, true, new TextObject("{=WiNRdfsm}Done", null).ToString(), new TextObject("{=3CpNUnVl}Cancel", null).ToString(), new Action<string>(this.OnSaveAsDone), null, false, new Func<string, Tuple<bool, string>>(this.IsSaveGameNameApplicable), "", ""), false, false);
		}

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

		private void OnSaveAsDone(string saveName)
		{
			Campaign.Current.SaveHandler.SaveAs(saveName);
			this.ExecuteDone();
		}

		public void ExecuteDone()
		{
			ScreenManager.PopScreen();
		}

		public void ExecuteLoadSave()
		{
			this.LoadSelectedSave();
		}

		private void LoadSelectedSave()
		{
			if (!this.IsBusyWithAnAction && this.CurrentSelectedSave != null && !this.CurrentSelectedSave.IsCorrupted)
			{
				this.CurrentSelectedSave.ExecuteSaveLoad();
				this.IsBusyWithAnAction = true;
			}
		}

		private void OnCancelLoadSave()
		{
			this.IsBusyWithAnAction = false;
		}

		private void ExecuteResetCurrentSave()
		{
			this.CurrentSelectedSave = null;
		}

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

		private void OnDeleteSaveUnsuccessful()
		{
			string text = new TextObject("{=oZrVNUOk}Error", null).ToString();
			string text2 = new TextObject("{=PY00wRz4}Failed to delete the save file.", null).ToString();
			InformationManager.ShowInquiry(new InquiryData(text, text2, true, false, new TextObject("{=WiNRdfsm}Done", null).ToString(), string.Empty, null, null, "", 0f, null, null, null), false, false);
		}

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

		public void DeleteSelectedSave()
		{
			if (this.CurrentSelectedSave != null)
			{
				this.OnDeleteSavedGame(this.CurrentSelectedSave);
			}
		}

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

		public void SetDoneInputKey(HotKey hotkey)
		{
			this.DoneInputKey = InputKeyItemVM.CreateFromHotKey(hotkey, true);
		}

		public void SetCancelInputKey(HotKey hotkey)
		{
			this.CancelInputKey = InputKeyItemVM.CreateFromHotKey(hotkey, true);
		}

		public void SetDeleteInputKey(HotKey hotkey)
		{
			this.DeleteInputKey = InputKeyItemVM.CreateFromHotKey(hotkey, true);
		}

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

		private const int _maxSaveFileNameLength = 30;

		private readonly TextObject _categorizedSaveGroupName = new TextObject("{=nVGqjtaa}Campaign {ID}", null);

		private readonly TextObject _uncategorizedSaveGroupName = new TextObject("{=uncategorized_save}Uncategorized", null);

		private readonly TextObject _textIsEmptyReasonText = new TextObject("{=7AI8jA0b}Input text cannot be empty.", null);

		private readonly TextObject _textHasSpecialCharReasonText = new TextObject("{=kXRdeawC}Input text cannot include special characters.", null);

		private readonly TextObject _textTooLongReasonText = new TextObject("{=B3W6fcQX}Input text cannot be longer than {MAX_LENGTH} characters.", null);

		private readonly TextObject _saveAlreadyExistsReasonText = new TextObject("{=aG6XMhA1}A saved game file already exists with this name.", null);

		private readonly TextObject _saveNameReservedReasonText = new TextObject("{=M4WMKyE1}Input text includes reserved text.", null);

		private readonly TextObject _allSpaceReasonText = new TextObject("{=Rtakaivj}Input text needs to include at least one non-space character.", null);

		private readonly TextObject _visualIsDisabledText = new TextObject("{=xlEZ02Qw}Character visual is disabled during 'Save As' on the campaign map.", null);

		private bool _isSearchAvailable;

		private string _searchText;

		private string _searchPlaceholderText;

		private string _doneText;

		private string _createNewSaveSlotText;

		private string _titleText;

		private string _visualDisabledText;

		private bool _isSaving;

		private bool _isActionEnabled;

		private bool _isAnyItemSelected;

		private bool _canCreateNewSave;

		private bool _isVisualDisabled;

		private string _saveLoadText;

		private string _cancelText;

		private HintViewModel _createNewSaveHint;

		private MBBindingList<SavedGameGroupVM> _saveGroups;

		private SavedGameVM _currentSelectedSave;

		private InputKeyItemVM _doneInputKey;

		private InputKeyItemVM _cancelInputKey;

		private InputKeyItemVM _deleteInputKey;
	}
}
