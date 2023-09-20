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
	// Token: 0x02000011 RID: 17
	public class SavedGameVM : ViewModel
	{
		// Token: 0x1700006E RID: 110
		// (get) Token: 0x06000143 RID: 323 RVA: 0x0000796B File Offset: 0x00005B6B
		public SaveGameFileInfo Save { get; }

		// Token: 0x1700006F RID: 111
		// (get) Token: 0x06000144 RID: 324 RVA: 0x00007973 File Offset: 0x00005B73
		// (set) Token: 0x06000145 RID: 325 RVA: 0x0000797B File Offset: 0x00005B7B
		public bool RequiresInquiryOnLoad { get; private set; }

		// Token: 0x17000070 RID: 112
		// (get) Token: 0x06000146 RID: 326 RVA: 0x00007984 File Offset: 0x00005B84
		// (set) Token: 0x06000147 RID: 327 RVA: 0x0000798C File Offset: 0x00005B8C
		public bool IsModuleDiscrepancyDetected { get; private set; }

		// Token: 0x06000148 RID: 328 RVA: 0x00007998 File Offset: 0x00005B98
		public SavedGameVM(SaveGameFileInfo save, bool isSaving, Action<SavedGameVM> onDelete, Action<SavedGameVM> onSelection, Action onCancelLoadSave, Action onDone, bool isCorruptedSave = false, bool isDiscrepancyDetectedForSave = false)
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
			this.NameText = ((!isCorruptedSave) ? this.Save.Name : new TextObject("{=RoYPofhK}Corrupted Save", null).ToString());
			this._newlineTextObject.SetTextVariable("newline", "\n");
			this._gameVersion = ApplicationVersion.FromParametersFile(null);
			this._saveVersion = MetaDataExtensions.GetApplicationVersion(this.Save.MetaData);
			this.IsModuleDiscrepancyDetected = isCorruptedSave || isDiscrepancyDetectedForSave;
			this.MainHeroVisualCode = (this.IsModuleDiscrepancyDetected ? string.Empty : MetaDataExtensions.GetCharacterVisualCode(this.Save.MetaData));
			this.BannerTextCode = (this.IsModuleDiscrepancyDetected ? string.Empty : MetaDataExtensions.GetClanBannerCode(this.Save.MetaData));
		}

		// Token: 0x06000149 RID: 329 RVA: 0x00007ABC File Offset: 0x00005CBC
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
		}

		// Token: 0x0600014A RID: 330 RVA: 0x00007EEC File Offset: 0x000060EC
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

		// Token: 0x0600014B RID: 331 RVA: 0x00007F93 File Offset: 0x00006193
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

		// Token: 0x0600014C RID: 332 RVA: 0x00007FCC File Offset: 0x000061CC
		private void OnOverrideSaveAccept()
		{
			Campaign.Current.SaveHandler.SaveAs(this.Save.Name);
			this._onDone();
		}

		// Token: 0x0600014D RID: 333 RVA: 0x00007FF4 File Offset: 0x000061F4
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

		// Token: 0x0600014E RID: 334 RVA: 0x0000816A File Offset: 0x0000636A
		public void ExecuteUpdate()
		{
		}

		// Token: 0x0600014F RID: 335 RVA: 0x0000816C File Offset: 0x0000636C
		public void ExecuteDelete()
		{
			this._onDelete(this);
		}

		// Token: 0x06000150 RID: 336 RVA: 0x0000817A File Offset: 0x0000637A
		public void ExecuteSelection()
		{
			this._onSelection(this);
		}

		// Token: 0x17000071 RID: 113
		// (get) Token: 0x06000151 RID: 337 RVA: 0x00008188 File Offset: 0x00006388
		// (set) Token: 0x06000152 RID: 338 RVA: 0x00008190 File Offset: 0x00006390
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

		// Token: 0x17000072 RID: 114
		// (get) Token: 0x06000153 RID: 339 RVA: 0x000081AE File Offset: 0x000063AE
		// (set) Token: 0x06000154 RID: 340 RVA: 0x000081B6 File Offset: 0x000063B6
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

		// Token: 0x17000073 RID: 115
		// (get) Token: 0x06000155 RID: 341 RVA: 0x000081D4 File Offset: 0x000063D4
		// (set) Token: 0x06000156 RID: 342 RVA: 0x000081DC File Offset: 0x000063DC
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

		// Token: 0x17000074 RID: 116
		// (get) Token: 0x06000157 RID: 343 RVA: 0x000081FF File Offset: 0x000063FF
		// (set) Token: 0x06000158 RID: 344 RVA: 0x00008207 File Offset: 0x00006407
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

		// Token: 0x17000075 RID: 117
		// (get) Token: 0x06000159 RID: 345 RVA: 0x0000822A File Offset: 0x0000642A
		// (set) Token: 0x0600015A RID: 346 RVA: 0x00008232 File Offset: 0x00006432
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

		// Token: 0x17000076 RID: 118
		// (get) Token: 0x0600015B RID: 347 RVA: 0x00008250 File Offset: 0x00006450
		// (set) Token: 0x0600015C RID: 348 RVA: 0x00008258 File Offset: 0x00006458
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

		// Token: 0x17000077 RID: 119
		// (get) Token: 0x0600015D RID: 349 RVA: 0x00008276 File Offset: 0x00006476
		// (set) Token: 0x0600015E RID: 350 RVA: 0x0000827E File Offset: 0x0000647E
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

		// Token: 0x17000078 RID: 120
		// (get) Token: 0x0600015F RID: 351 RVA: 0x000082A1 File Offset: 0x000064A1
		// (set) Token: 0x06000160 RID: 352 RVA: 0x000082A9 File Offset: 0x000064A9
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

		// Token: 0x17000079 RID: 121
		// (get) Token: 0x06000161 RID: 353 RVA: 0x000082CC File Offset: 0x000064CC
		// (set) Token: 0x06000162 RID: 354 RVA: 0x000082D4 File Offset: 0x000064D4
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

		// Token: 0x1700007A RID: 122
		// (get) Token: 0x06000163 RID: 355 RVA: 0x000082F7 File Offset: 0x000064F7
		// (set) Token: 0x06000164 RID: 356 RVA: 0x000082FF File Offset: 0x000064FF
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

		// Token: 0x1700007B RID: 123
		// (get) Token: 0x06000165 RID: 357 RVA: 0x00008322 File Offset: 0x00006522
		// (set) Token: 0x06000166 RID: 358 RVA: 0x0000832A File Offset: 0x0000652A
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

		// Token: 0x1700007C RID: 124
		// (get) Token: 0x06000167 RID: 359 RVA: 0x0000834D File Offset: 0x0000654D
		// (set) Token: 0x06000168 RID: 360 RVA: 0x00008355 File Offset: 0x00006555
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

		// Token: 0x1700007D RID: 125
		// (get) Token: 0x06000169 RID: 361 RVA: 0x00008378 File Offset: 0x00006578
		// (set) Token: 0x0600016A RID: 362 RVA: 0x00008380 File Offset: 0x00006580
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

		// Token: 0x1700007E RID: 126
		// (get) Token: 0x0600016B RID: 363 RVA: 0x000083A3 File Offset: 0x000065A3
		// (set) Token: 0x0600016C RID: 364 RVA: 0x000083AB File Offset: 0x000065AB
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

		// Token: 0x1700007F RID: 127
		// (get) Token: 0x0600016D RID: 365 RVA: 0x000083CE File Offset: 0x000065CE
		// (set) Token: 0x0600016E RID: 366 RVA: 0x000083D6 File Offset: 0x000065D6
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

		// Token: 0x17000080 RID: 128
		// (get) Token: 0x0600016F RID: 367 RVA: 0x000083F9 File Offset: 0x000065F9
		// (set) Token: 0x06000170 RID: 368 RVA: 0x00008401 File Offset: 0x00006601
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

		// Token: 0x17000081 RID: 129
		// (get) Token: 0x06000171 RID: 369 RVA: 0x0000841F File Offset: 0x0000661F
		// (set) Token: 0x06000172 RID: 370 RVA: 0x00008427 File Offset: 0x00006627
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

		// Token: 0x17000082 RID: 130
		// (get) Token: 0x06000173 RID: 371 RVA: 0x00008445 File Offset: 0x00006645
		// (set) Token: 0x06000174 RID: 372 RVA: 0x0000844D File Offset: 0x0000664D
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

		// Token: 0x17000083 RID: 131
		// (get) Token: 0x06000175 RID: 373 RVA: 0x00008470 File Offset: 0x00006670
		// (set) Token: 0x06000176 RID: 374 RVA: 0x00008478 File Offset: 0x00006678
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

		// Token: 0x17000084 RID: 132
		// (get) Token: 0x06000177 RID: 375 RVA: 0x0000849B File Offset: 0x0000669B
		// (set) Token: 0x06000178 RID: 376 RVA: 0x000084A3 File Offset: 0x000066A3
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

		// Token: 0x17000085 RID: 133
		// (get) Token: 0x06000179 RID: 377 RVA: 0x000084C6 File Offset: 0x000066C6
		// (set) Token: 0x0600017A RID: 378 RVA: 0x000084CE File Offset: 0x000066CE
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

		// Token: 0x17000086 RID: 134
		// (get) Token: 0x0600017B RID: 379 RVA: 0x000084EC File Offset: 0x000066EC
		// (set) Token: 0x0600017C RID: 380 RVA: 0x000084F4 File Offset: 0x000066F4
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

		// Token: 0x17000087 RID: 135
		// (get) Token: 0x0600017D RID: 381 RVA: 0x00008512 File Offset: 0x00006712
		// (set) Token: 0x0600017E RID: 382 RVA: 0x0000851A File Offset: 0x0000671A
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

		// Token: 0x04000081 RID: 129
		private readonly bool _isSaving;

		// Token: 0x04000082 RID: 130
		private readonly Action _onDone;

		// Token: 0x04000083 RID: 131
		private readonly Action<SavedGameVM> _onDelete;

		// Token: 0x04000084 RID: 132
		private readonly Action<SavedGameVM> _onSelection;

		// Token: 0x04000085 RID: 133
		private readonly Action _onCancelLoadSave;

		// Token: 0x04000086 RID: 134
		private readonly TextObject _newlineTextObject = new TextObject("{=ol0rBSrb}{STR1}{newline}{STR2}", null);

		// Token: 0x04000087 RID: 135
		private readonly ApplicationVersion _gameVersion;

		// Token: 0x04000088 RID: 136
		private readonly ApplicationVersion _saveVersion;

		// Token: 0x04000089 RID: 137
		private MBBindingList<SavedGamePropertyVM> _savedGameProperties;

		// Token: 0x0400008A RID: 138
		private MBBindingList<SavedGameModuleInfoVM> _loadedModulesInSave;

		// Token: 0x0400008B RID: 139
		private HintViewModel _dateTimeHint;

		// Token: 0x0400008C RID: 140
		private HintViewModel _updateButtonHint;

		// Token: 0x0400008D RID: 141
		private ImageIdentifierVM _clanBanner;

		// Token: 0x0400008E RID: 142
		private CharacterViewModel _characterVisual;

		// Token: 0x0400008F RID: 143
		private string _deleteText;

		// Token: 0x04000090 RID: 144
		private string _nameText;

		// Token: 0x04000091 RID: 145
		private string _gameTimeText;

		// Token: 0x04000092 RID: 146
		private string _realTimeText1;

		// Token: 0x04000093 RID: 147
		private string _realTimeText2;

		// Token: 0x04000094 RID: 148
		private string _levelText;

		// Token: 0x04000095 RID: 149
		private string _characterNameText;

		// Token: 0x04000096 RID: 150
		private string _saveLoadText;

		// Token: 0x04000097 RID: 151
		private string _overwriteSaveText;

		// Token: 0x04000098 RID: 152
		private string _updateSaveText;

		// Token: 0x04000099 RID: 153
		private string _modulesText;

		// Token: 0x0400009A RID: 154
		private string _saveVersionAsString;

		// Token: 0x0400009B RID: 155
		private string _mainHeroVisualCode;

		// Token: 0x0400009C RID: 156
		private string _bannerTextCode;

		// Token: 0x0400009D RID: 157
		private bool _isSelected;

		// Token: 0x0400009E RID: 158
		private bool _isCorrupted;

		// Token: 0x0400009F RID: 159
		private bool _isFilteredOut;
	}
}
