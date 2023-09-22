using System;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Extensions;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.SaveSystem;
using TaleWorlds.SaveSystem.Load;
using TaleWorlds.ScreenSystem;

namespace SandBox.ViewModelCollection.SaveLoad
{
	public class SavedGameVM : ViewModel
	{
		public SaveGameFileInfo Save { get; }

		public bool RequiresInquiryOnLoad { get; private set; }

		public bool IsModuleDiscrepancyDetected { get; private set; }

		public SavedGameVM(SaveGameFileInfo save, bool isSaving, Action<SavedGameVM> onDelete, Action<SavedGameVM> onSelection, Action onCancelLoadSave, Action onDone, bool isCorruptedSave = false, bool isDiscrepancyDetectedForSave = false, bool isIronman = false)
		{
			this.Save = save;
			this._isSaving = isSaving;
			this._onDelete = onDelete;
			this._onSelection = onSelection;
			this._onCancelLoadSave = onCancelLoadSave;
			this._onDone = onDone;
			this.IsCorrupted = isCorruptedSave;
			this.SavedGameProperties = new MBBindingList<SavedGamePropertyVM>();
			this.LoadedModulesInSave = new MBBindingList<SavedGameModuleInfoVM>();
			if (isIronman)
			{
				GameTexts.SetVariable("RANK", MetaDataExtensions.GetCharacterName(this.Save.MetaData));
				GameTexts.SetVariable("NUMBER", new TextObject("{=Fm0rjkH7}Ironman", null));
				this.NameText = new TextObject("{=AVoWvlue}{RANK} ({NUMBER})", null).ToString();
			}
			else
			{
				this.NameText = this.Save.Name;
			}
			this._newlineTextObject.SetTextVariable("newline", "\n");
			this._gameVersion = ApplicationVersion.FromParametersFile(null);
			this._saveVersion = MetaDataExtensions.GetApplicationVersion(this.Save.MetaData);
			this.IsModuleDiscrepancyDetected = isCorruptedSave || isDiscrepancyDetectedForSave;
			this.MainHeroVisualCode = (this.IsModuleDiscrepancyDetected ? string.Empty : MetaDataExtensions.GetCharacterVisualCode(this.Save.MetaData));
			this.BannerTextCode = (this.IsModuleDiscrepancyDetected ? string.Empty : MetaDataExtensions.GetClanBannerCode(this.Save.MetaData));
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
			this.LoadedModulesInSave.Clear();
			this.SavedGameProperties.Clear();
			this.SaveVersionAsString = this._saveVersion.ToString();
			if (this._gameVersion != this._saveVersion)
			{
				this.RequiresInquiryOnLoad = true;
			}
			foreach (string text in MetaDataExtensions.GetModules(this.Save.MetaData))
			{
				string text2 = MetaDataExtensions.GetModuleVersion(this.Save.MetaData, text).ToString();
				this.LoadedModulesInSave.Add(new SavedGameModuleInfoVM(text, "", text2));
			}
			this.CharacterNameText = MetaDataExtensions.GetCharacterName(this.Save.MetaData);
			this.ClanBanner = new ImageIdentifierVM(BannerCode.CreateFrom(MetaDataExtensions.GetClanBannerCode(this.Save.MetaData)), true);
			this.DeleteText = new TextObject("{=deleteaction}Delete", null).ToString();
			this.ModulesText = new TextObject("{=JXyxj1J5}Modules", null).ToString();
			DateTime creationTime = MetaDataExtensions.GetCreationTime(this.Save.MetaData);
			this.RealTimeText1 = LocalizedTextManager.GetDateFormattedByLanguage(BannerlordConfig.Language, creationTime);
			this.RealTimeText2 = LocalizedTextManager.GetTimeFormattedByLanguage(BannerlordConfig.Language, creationTime);
			int playerHealthPercentage = MetaDataExtensions.GetPlayerHealthPercentage(this.Save.MetaData);
			TextObject textObject = new TextObject("{=gYATKZJp}{NUMBER}%", null);
			textObject.SetTextVariable("NUMBER", playerHealthPercentage.ToString());
			this.SavedGameProperties.Add(new SavedGamePropertyVM(SavedGamePropertyVM.SavedGameProperty.Health, textObject, new TextObject("{=hZrwUIaq}Health", null)));
			int mainHeroGold = MetaDataExtensions.GetMainHeroGold(this.Save.MetaData);
			this.SavedGameProperties.Add(new SavedGamePropertyVM(SavedGamePropertyVM.SavedGameProperty.Gold, SavedGameVM.GetAbbreviatedValueTextFromValue(mainHeroGold), new TextObject("{=Hxf6bzmR}Current Denars", null)));
			int num = (int)MetaDataExtensions.GetClanInfluence(this.Save.MetaData);
			this.SavedGameProperties.Add(new SavedGamePropertyVM(SavedGamePropertyVM.SavedGameProperty.Influence, SavedGameVM.GetAbbreviatedValueTextFromValue(num), new TextObject("{=RVPidk5a}Influence", null)));
			int num2 = MetaDataExtensions.GetMainPartyHealthyMemberCount(this.Save.MetaData) + MetaDataExtensions.GetMainPartyWoundedMemberCount(this.Save.MetaData);
			int mainPartyPrisonerMemberCount = MetaDataExtensions.GetMainPartyPrisonerMemberCount(this.Save.MetaData);
			TextObject textObject2 = TextObject.Empty;
			if (mainPartyPrisonerMemberCount > 0)
			{
				textObject2 = new TextObject("{=6qYaQkDD}{COUNT} + {PRISONER_COUNT}p", null);
				textObject2.SetTextVariable("COUNT", num2);
				textObject2.SetTextVariable("PRISONER_COUNT", mainPartyPrisonerMemberCount);
			}
			else
			{
				textObject2 = new TextObject(num2, null);
			}
			this.SavedGameProperties.Add(new SavedGamePropertyVM(SavedGamePropertyVM.SavedGameProperty.PartySize, textObject2, new TextObject("{=IXwOaa98}Party Size", null)));
			int num3 = (int)MetaDataExtensions.GetMainPartyFood(this.Save.MetaData);
			this.SavedGameProperties.Add(new SavedGamePropertyVM(SavedGamePropertyVM.SavedGameProperty.Food, new TextObject(num3, null), new TextObject("{=qSi4DlT4}Food", null)));
			int clanFiefs = MetaDataExtensions.GetClanFiefs(this.Save.MetaData);
			this.SavedGameProperties.Add(new SavedGamePropertyVM(SavedGamePropertyVM.SavedGameProperty.Fiefs, new TextObject(clanFiefs, null), new TextObject("{=SRjrhb0A}Owned Fief Count", null)));
			TextObject textObject3 = new TextObject("{=GZWPHmAw}Day : {DAY}", null);
			string text3 = ((int)MetaDataExtensions.GetDayLong(this.Save.MetaData)).ToString();
			textObject3.SetTextVariable("DAY", text3);
			this.GameTimeText = textObject3.ToString();
			TextObject textObject4 = new TextObject("{=IwhpeT8C}Level : {PLAYER_LEVEL}", null);
			textObject4.SetTextVariable("PLAYER_LEVEL", MetaDataExtensions.GetMainHeroLevel(this.Save.MetaData).ToString());
			this.LevelText = textObject4.ToString();
			this.DateTimeHint = new HintViewModel(new TextObject("{=!}" + this.RealTimeText1, null), null);
			this.UpdateButtonHint = new HintViewModel(new TextObject("{=ZDPIq4hi}Load the selected save game, overwrite it with the current version of the game and get back to this screen.", null), null);
			this.SaveLoadText = (this._isSaving ? new TextObject("{=bV75iwKa}Save", null).ToString() : new TextObject("{=9NuttOBC}Load", null).ToString());
			this.OverrideSaveText = new TextObject("{=hYL3CFHX}Do you want to overwrite this saved game?", null).ToString();
			this.UpdateSaveText = new TextObject("{=FFiPLPbs}Update Save", null).ToString();
			this.CorruptedSaveText = new TextObject("{=RoYPofhK}Corrupted Save", null).ToString();
		}

		public void ExecuteSaveLoad()
		{
			if (!this.IsCorrupted)
			{
				if (this._isSaving)
				{
					InformationManager.ShowInquiry(new InquiryData(new TextObject("{=Q1HIlJxe}Overwrite", null).ToString(), this.OverrideSaveText, true, true, new TextObject("{=aeouhelq}Yes", null).ToString(), new TextObject("{=8OkPHu4f}No", null).ToString(), new Action(this.OnOverrideSaveAccept), delegate
					{
						Action onCancelLoadSave = this._onCancelLoadSave;
						if (onCancelLoadSave == null)
						{
							return;
						}
						onCancelLoadSave();
					}, "", 0f, null, null, null), false, false);
					return;
				}
				SandBoxSaveHelper.TryLoadSave(this.Save, new Action<LoadResult>(this.StartGame), this._onCancelLoadSave);
			}
		}

		private void StartGame(LoadResult loadResult)
		{
			if (Game.Current != null)
			{
				ScreenManager.PopScreen();
				GameStateManager.Current.CleanStates(0);
				GameStateManager.Current = Module.CurrentModule.GlobalGameStateManager;
			}
			MBSaveLoad.OnStartGame(loadResult);
			MBGameManager.StartNewGame(new SandBoxGameManager(loadResult));
		}

		private void OnOverrideSaveAccept()
		{
			Campaign.Current.SaveHandler.SaveAs(this.Save.Name);
			this._onDone();
		}

		private static TextObject GetAbbreviatedValueTextFromValue(int valueAmount)
		{
			string text = "";
			decimal num = valueAmount;
			if (valueAmount < 10000)
			{
				return new TextObject(valueAmount, null);
			}
			if (valueAmount >= 10000 && valueAmount < 1000000)
			{
				text = new TextObject("{=thousandabbr}k", null).ToString();
				num /= 1000m;
			}
			else if (valueAmount >= 1000000 && valueAmount < 1000000000)
			{
				text = new TextObject("{=millionabbr}m", null).ToString();
				num /= 1000000m;
			}
			else if (valueAmount >= 1000000000 && valueAmount <= 2147483647)
			{
				text = new TextObject("{=billionabbr}b", null).ToString();
				num /= 1000000000m;
			}
			int num2 = (int)num;
			string text2 = num2.ToString();
			if (text2.Length < 3)
			{
				text2 += ".";
				string text3 = num.ToString("F3").Split(new char[] { '.' }).ElementAtOrDefault(1);
				if (text3 != null)
				{
					for (int i = 0; i < 3 - num2.ToString().Length; i++)
					{
						if (text3.ElementAtOrDefault(i) != '\0')
						{
							text2 += text3.ElementAtOrDefault(i).ToString();
						}
					}
				}
			}
			TextObject textObject = new TextObject("{=mapbardenarvalue}{DENAR_AMOUNT}{VALUE_ABBREVIATION}", null);
			textObject.SetTextVariable("DENAR_AMOUNT", text2);
			textObject.SetTextVariable("VALUE_ABBREVIATION", text);
			return textObject;
		}

		public void ExecuteUpdate()
		{
		}

		public void ExecuteDelete()
		{
			this._onDelete(this);
		}

		public void ExecuteSelection()
		{
			this._onSelection(this);
		}

		[DataSourceProperty]
		public MBBindingList<SavedGamePropertyVM> SavedGameProperties
		{
			get
			{
				return this._savedGameProperties;
			}
			set
			{
				if (value != this._savedGameProperties)
				{
					this._savedGameProperties = value;
					base.OnPropertyChangedWithValue<MBBindingList<SavedGamePropertyVM>>(value, "SavedGameProperties");
				}
			}
		}

		[DataSourceProperty]
		public MBBindingList<SavedGameModuleInfoVM> LoadedModulesInSave
		{
			get
			{
				return this._loadedModulesInSave;
			}
			set
			{
				if (value != this._loadedModulesInSave)
				{
					this._loadedModulesInSave = value;
					base.OnPropertyChangedWithValue<MBBindingList<SavedGameModuleInfoVM>>(value, "LoadedModulesInSave");
				}
			}
		}

		[DataSourceProperty]
		public string SaveVersionAsString
		{
			get
			{
				return this._saveVersionAsString;
			}
			set
			{
				if (value != this._saveVersionAsString)
				{
					this._saveVersionAsString = value;
					base.OnPropertyChangedWithValue<string>(value, "SaveVersionAsString");
				}
			}
		}

		[DataSourceProperty]
		public string DeleteText
		{
			get
			{
				return this._deleteText;
			}
			set
			{
				if (value != this._deleteText)
				{
					this._deleteText = value;
					base.OnPropertyChangedWithValue<string>(value, "DeleteText");
				}
			}
		}

		[DataSourceProperty]
		public bool IsSelected
		{
			get
			{
				return this._isSelected;
			}
			set
			{
				if (value != this._isSelected)
				{
					this._isSelected = value;
					base.OnPropertyChangedWithValue(value, "IsSelected");
				}
			}
		}

		[DataSourceProperty]
		public bool IsCorrupted
		{
			get
			{
				return this._isCorrupted;
			}
			set
			{
				if (value != this._isCorrupted)
				{
					this._isCorrupted = value;
					base.OnPropertyChangedWithValue(value, "IsCorrupted");
				}
			}
		}

		[DataSourceProperty]
		public string BannerTextCode
		{
			get
			{
				return this._bannerTextCode;
			}
			set
			{
				if (value != this._bannerTextCode)
				{
					this._bannerTextCode = value;
					base.OnPropertyChangedWithValue<string>(value, "BannerTextCode");
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

		[DataSourceProperty]
		public string OverrideSaveText
		{
			get
			{
				return this._overwriteSaveText;
			}
			set
			{
				if (value != this._overwriteSaveText)
				{
					this._overwriteSaveText = value;
					base.OnPropertyChangedWithValue<string>(value, "OverrideSaveText");
				}
			}
		}

		[DataSourceProperty]
		public string UpdateSaveText
		{
			get
			{
				return this._updateSaveText;
			}
			set
			{
				if (value != this._updateSaveText)
				{
					this._updateSaveText = value;
					base.OnPropertyChangedWithValue<string>(value, "UpdateSaveText");
				}
			}
		}

		[DataSourceProperty]
		public string ModulesText
		{
			get
			{
				return this._modulesText;
			}
			set
			{
				if (value != this._modulesText)
				{
					this._modulesText = value;
					base.OnPropertyChangedWithValue<string>(value, "ModulesText");
				}
			}
		}

		[DataSourceProperty]
		public string CorruptedSaveText
		{
			get
			{
				return this._corruptedSaveText;
			}
			set
			{
				if (value != this._corruptedSaveText)
				{
					this._corruptedSaveText = value;
					base.OnPropertyChangedWithValue<string>(value, "CorruptedSaveText");
				}
			}
		}

		[DataSourceProperty]
		public string NameText
		{
			get
			{
				return this._nameText;
			}
			set
			{
				if (value != this._nameText)
				{
					this._nameText = value;
					base.OnPropertyChangedWithValue<string>(value, "NameText");
				}
			}
		}

		[DataSourceProperty]
		public string GameTimeText
		{
			get
			{
				return this._gameTimeText;
			}
			set
			{
				if (value != this._gameTimeText)
				{
					this._gameTimeText = value;
					base.OnPropertyChangedWithValue<string>(value, "GameTimeText");
				}
			}
		}

		[DataSourceProperty]
		public string CharacterNameText
		{
			get
			{
				return this._characterNameText;
			}
			set
			{
				if (value != this._characterNameText)
				{
					this._characterNameText = value;
					base.OnPropertyChangedWithValue<string>(value, "CharacterNameText");
				}
			}
		}

		[DataSourceProperty]
		public string MainHeroVisualCode
		{
			get
			{
				return this._mainHeroVisualCode;
			}
			set
			{
				if (value != this._mainHeroVisualCode)
				{
					this._mainHeroVisualCode = value;
					base.OnPropertyChangedWithValue<string>(value, "MainHeroVisualCode");
				}
			}
		}

		[DataSourceProperty]
		public CharacterViewModel CharacterVisual
		{
			get
			{
				return this._characterVisual;
			}
			set
			{
				if (value != this._characterVisual)
				{
					this._characterVisual = value;
					base.OnPropertyChangedWithValue<CharacterViewModel>(value, "CharacterVisual");
				}
			}
		}

		[DataSourceProperty]
		public ImageIdentifierVM ClanBanner
		{
			get
			{
				return this._clanBanner;
			}
			set
			{
				if (value != this._clanBanner)
				{
					this._clanBanner = value;
					base.OnPropertyChangedWithValue<ImageIdentifierVM>(value, "ClanBanner");
				}
			}
		}

		[DataSourceProperty]
		public string RealTimeText1
		{
			get
			{
				return this._realTimeText1;
			}
			set
			{
				if (value != this._realTimeText1)
				{
					this._realTimeText1 = value;
					base.OnPropertyChangedWithValue<string>(value, "RealTimeText1");
				}
			}
		}

		[DataSourceProperty]
		public string RealTimeText2
		{
			get
			{
				return this._realTimeText2;
			}
			set
			{
				if (value != this._realTimeText2)
				{
					this._realTimeText2 = value;
					base.OnPropertyChangedWithValue<string>(value, "RealTimeText2");
				}
			}
		}

		[DataSourceProperty]
		public string LevelText
		{
			get
			{
				return this._levelText;
			}
			set
			{
				if (value != this._levelText)
				{
					this._levelText = value;
					base.OnPropertyChangedWithValue<string>(value, "LevelText");
				}
			}
		}

		[DataSourceProperty]
		public HintViewModel DateTimeHint
		{
			get
			{
				return this._dateTimeHint;
			}
			set
			{
				if (value != this._dateTimeHint)
				{
					this._dateTimeHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "DateTimeHint");
				}
			}
		}

		[DataSourceProperty]
		public HintViewModel UpdateButtonHint
		{
			get
			{
				return this._updateButtonHint;
			}
			set
			{
				if (value != this._updateButtonHint)
				{
					this._updateButtonHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "UpdateButtonHint");
				}
			}
		}

		[DataSourceProperty]
		public bool IsFilteredOut
		{
			get
			{
				return this._isFilteredOut;
			}
			set
			{
				if (value != this._isFilteredOut)
				{
					this._isFilteredOut = value;
					base.OnPropertyChangedWithValue(value, "IsFilteredOut");
				}
			}
		}

		private readonly bool _isSaving;

		private readonly Action _onDone;

		private readonly Action<SavedGameVM> _onDelete;

		private readonly Action<SavedGameVM> _onSelection;

		private readonly Action _onCancelLoadSave;

		private readonly TextObject _newlineTextObject = new TextObject("{=ol0rBSrb}{STR1}{newline}{STR2}", null);

		private readonly ApplicationVersion _gameVersion;

		private readonly ApplicationVersion _saveVersion;

		private MBBindingList<SavedGamePropertyVM> _savedGameProperties;

		private MBBindingList<SavedGameModuleInfoVM> _loadedModulesInSave;

		private HintViewModel _dateTimeHint;

		private HintViewModel _updateButtonHint;

		private ImageIdentifierVM _clanBanner;

		private CharacterViewModel _characterVisual;

		private string _deleteText;

		private string _nameText;

		private string _gameTimeText;

		private string _realTimeText1;

		private string _realTimeText2;

		private string _levelText;

		private string _characterNameText;

		private string _saveLoadText;

		private string _overwriteSaveText;

		private string _updateSaveText;

		private string _modulesText;

		private string _corruptedSaveText;

		private string _saveVersionAsString;

		private string _mainHeroVisualCode;

		private string _bannerTextCode;

		private bool _isSelected;

		private bool _isCorrupted;

		private bool _isFilteredOut;
	}
}
