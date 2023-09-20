using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core.ViewModelCollection.Selector;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.CustomBattle.CustomBattle.SelectionItem;

namespace TaleWorlds.MountAndBlade.CustomBattle.CustomBattle
{
	// Token: 0x0200001C RID: 28
	public class MapSelectionGroupVM : ViewModel
	{
		// Token: 0x17000059 RID: 89
		// (get) Token: 0x06000139 RID: 313 RVA: 0x00009299 File Offset: 0x00007499
		// (set) Token: 0x0600013A RID: 314 RVA: 0x000092A1 File Offset: 0x000074A1
		public int SelectedWallBreachedCount { get; private set; }

		// Token: 0x1700005A RID: 90
		// (get) Token: 0x0600013B RID: 315 RVA: 0x000092AA File Offset: 0x000074AA
		// (set) Token: 0x0600013C RID: 316 RVA: 0x000092B2 File Offset: 0x000074B2
		public int SelectedSceneLevel { get; private set; }

		// Token: 0x1700005B RID: 91
		// (get) Token: 0x0600013D RID: 317 RVA: 0x000092BB File Offset: 0x000074BB
		// (set) Token: 0x0600013E RID: 318 RVA: 0x000092C3 File Offset: 0x000074C3
		public int SelectedTimeOfDay { get; private set; }

		// Token: 0x1700005C RID: 92
		// (get) Token: 0x0600013F RID: 319 RVA: 0x000092CC File Offset: 0x000074CC
		// (set) Token: 0x06000140 RID: 320 RVA: 0x000092D4 File Offset: 0x000074D4
		public string SelectedSeasonId { get; private set; }

		// Token: 0x1700005D RID: 93
		// (get) Token: 0x06000141 RID: 321 RVA: 0x000092DD File Offset: 0x000074DD
		// (set) Token: 0x06000142 RID: 322 RVA: 0x000092E5 File Offset: 0x000074E5
		public MapItemVM SelectedMap { get; private set; }

		// Token: 0x1700005E RID: 94
		// (get) Token: 0x06000143 RID: 323 RVA: 0x000092EE File Offset: 0x000074EE
		// (set) Token: 0x06000144 RID: 324 RVA: 0x000092F6 File Offset: 0x000074F6
		private List<MapItemVM> _battleMaps { get; set; }

		// Token: 0x1700005F RID: 95
		// (get) Token: 0x06000145 RID: 325 RVA: 0x000092FF File Offset: 0x000074FF
		// (set) Token: 0x06000146 RID: 326 RVA: 0x00009307 File Offset: 0x00007507
		private List<MapItemVM> _villageMaps { get; set; }

		// Token: 0x17000060 RID: 96
		// (get) Token: 0x06000147 RID: 327 RVA: 0x00009310 File Offset: 0x00007510
		// (set) Token: 0x06000148 RID: 328 RVA: 0x00009318 File Offset: 0x00007518
		private List<MapItemVM> _siegeMaps { get; set; }

		// Token: 0x17000061 RID: 97
		// (get) Token: 0x06000149 RID: 329 RVA: 0x00009321 File Offset: 0x00007521
		// (set) Token: 0x0600014A RID: 330 RVA: 0x00009329 File Offset: 0x00007529
		private List<MapItemVM> _availableMaps { get; set; }

		// Token: 0x0600014B RID: 331 RVA: 0x00009334 File Offset: 0x00007534
		public MapSelectionGroupVM()
		{
			this._battleMaps = new List<MapItemVM>();
			this._villageMaps = new List<MapItemVM>();
			this._siegeMaps = new List<MapItemVM>();
			this._availableMaps = this._battleMaps;
			this.MapSelection = new SelectorVM<MapItemVM>(0, new Action<SelectorVM<MapItemVM>>(this.OnMapSelection));
			this.WallHitpointSelection = new SelectorVM<WallHitpointItemVM>(0, new Action<SelectorVM<WallHitpointItemVM>>(this.OnWallHitpointSelection));
			this.SceneLevelSelection = new SelectorVM<SceneLevelItemVM>(0, new Action<SelectorVM<SceneLevelItemVM>>(this.OnSceneLevelSelection));
			this.SeasonSelection = new SelectorVM<SeasonItemVM>(0, new Action<SelectorVM<SeasonItemVM>>(this.OnSeasonSelection));
			this.TimeOfDaySelection = new SelectorVM<TimeOfDayItemVM>(0, new Action<SelectorVM<TimeOfDayItemVM>>(this.OnTimeOfDaySelection));
			this.RefreshValues();
		}

		// Token: 0x0600014C RID: 332 RVA: 0x000093F4 File Offset: 0x000075F4
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.PrepareMapLists();
			this.TitleText = new TextObject("{=customgametitle}Map", null).ToString();
			this.MapText = new TextObject("{=customgamemapname}Map", null).ToString();
			this.SeasonText = new TextObject("{=xTzDM5XE}Season", null).ToString();
			this.TimeOfDayText = new TextObject("{=DszSWnc3}Time of Day", null).ToString();
			this.SceneLevelText = new TextObject("{=0s52GQJt}Scene Level", null).ToString();
			this.WallHitpointsText = new TextObject("{=4IuXGSdc}Wall Hitpoints", null).ToString();
			this.AttackerSiegeMachinesText = new TextObject("{=AmfIfeIc}Choose Attacker Siege Machines", null).ToString();
			this.DefenderSiegeMachinesText = new TextObject("{=UoiSWe87}Choose Defender Siege Machines", null).ToString();
			this.SalloutText = new TextObject("{=EcKMGoFv}Sallyout", null).ToString();
			this.MapSelection.ItemList.Clear();
			this.WallHitpointSelection.ItemList.Clear();
			this.SceneLevelSelection.ItemList.Clear();
			this.SeasonSelection.ItemList.Clear();
			this.TimeOfDaySelection.ItemList.Clear();
			foreach (MapItemVM mapItemVM in this._availableMaps)
			{
				this.MapSelection.AddItem(new MapItemVM(mapItemVM.MapName, mapItemVM.MapId));
			}
			foreach (Tuple<string, int> tuple in CustomBattleData.WallHitpoints)
			{
				this.WallHitpointSelection.AddItem(new WallHitpointItemVM(tuple.Item1, tuple.Item2));
			}
			foreach (int num in CustomBattleData.SceneLevels)
			{
				this.SceneLevelSelection.AddItem(new SceneLevelItemVM(num));
			}
			foreach (Tuple<string, string> tuple2 in CustomBattleData.Seasons)
			{
				this.SeasonSelection.AddItem(new SeasonItemVM(tuple2.Item1, tuple2.Item2));
			}
			foreach (Tuple<string, CustomBattleTimeOfDay> tuple3 in CustomBattleData.TimesOfDay)
			{
				this.TimeOfDaySelection.AddItem(new TimeOfDayItemVM(tuple3.Item1, (int)tuple3.Item2));
			}
			this.MapSelection.SelectedIndex = 0;
			this.WallHitpointSelection.SelectedIndex = 0;
			this.SceneLevelSelection.SelectedIndex = 0;
			this.SeasonSelection.SelectedIndex = 0;
			this.TimeOfDaySelection.SelectedIndex = 0;
		}

		// Token: 0x0600014D RID: 333 RVA: 0x00009708 File Offset: 0x00007908
		public void ExecuteSallyOutChange()
		{
			this.IsSallyOutSelected = !this.IsSallyOutSelected;
		}

		// Token: 0x0600014E RID: 334 RVA: 0x0000971C File Offset: 0x0000791C
		private void PrepareMapLists()
		{
			this._battleMaps.Clear();
			this._villageMaps.Clear();
			this._siegeMaps.Clear();
			IEnumerable<CustomBattleSceneData> enumerable;
			if (Module.CurrentModule.IsOnlyCoreContentEnabled)
			{
				enumerable = CustomGame.Current.CustomBattleScenes.Where((CustomBattleSceneData s) => s.SceneID == "battle_terrain_029");
			}
			else
			{
				IEnumerable<CustomBattleSceneData> enumerable2 = CustomGame.Current.CustomBattleScenes.ToList<CustomBattleSceneData>();
				enumerable = enumerable2;
			}
			foreach (CustomBattleSceneData customBattleSceneData in enumerable)
			{
				MapItemVM mapItemVM = new MapItemVM(customBattleSceneData.Name.ToString(), customBattleSceneData.SceneID);
				if (customBattleSceneData.IsVillageMap)
				{
					this._villageMaps.Add(mapItemVM);
				}
				else if (customBattleSceneData.IsSiegeMap)
				{
					this._siegeMaps.Add(mapItemVM);
				}
				else if (!customBattleSceneData.IsLordsHallMap)
				{
					this._battleMaps.Add(mapItemVM);
				}
			}
			Comparer<MapItemVM> comparer = Comparer<MapItemVM>.Create((MapItemVM x, MapItemVM y) => x.MapName.CompareTo(y.MapName));
			this._battleMaps.Sort(comparer);
			this._villageMaps.Sort(comparer);
			this._siegeMaps.Sort(comparer);
		}

		// Token: 0x0600014F RID: 335 RVA: 0x00009874 File Offset: 0x00007A74
		private void OnMapSelection(SelectorVM<MapItemVM> selector)
		{
			this.SelectedMap = selector.SelectedItem;
		}

		// Token: 0x06000150 RID: 336 RVA: 0x00009882 File Offset: 0x00007A82
		private void OnWallHitpointSelection(SelectorVM<WallHitpointItemVM> selector)
		{
			this.SelectedWallBreachedCount = selector.SelectedItem.BreachedWallCount;
		}

		// Token: 0x06000151 RID: 337 RVA: 0x00009895 File Offset: 0x00007A95
		private void OnSceneLevelSelection(SelectorVM<SceneLevelItemVM> selector)
		{
			this.SelectedSceneLevel = selector.SelectedItem.Level;
		}

		// Token: 0x06000152 RID: 338 RVA: 0x000098A8 File Offset: 0x00007AA8
		private void OnSeasonSelection(SelectorVM<SeasonItemVM> selector)
		{
			this.SelectedSeasonId = selector.SelectedItem.SeasonId;
		}

		// Token: 0x06000153 RID: 339 RVA: 0x000098BB File Offset: 0x00007ABB
		private void OnTimeOfDaySelection(SelectorVM<TimeOfDayItemVM> selector)
		{
			this.SelectedTimeOfDay = selector.SelectedItem.TimeOfDay;
		}

		// Token: 0x06000154 RID: 340 RVA: 0x000098D0 File Offset: 0x00007AD0
		public void OnGameTypeChange(CustomBattleGameType gameType)
		{
			this.MapSelection.ItemList.Clear();
			if (gameType == CustomBattleGameType.Battle)
			{
				this.IsCurrentMapSiege = false;
				this._availableMaps = this._battleMaps;
			}
			else if (gameType == CustomBattleGameType.Village)
			{
				this.IsCurrentMapSiege = false;
				this._availableMaps = this._villageMaps;
			}
			else if (gameType == CustomBattleGameType.Siege)
			{
				this.IsCurrentMapSiege = true;
				this._availableMaps = this._siegeMaps;
			}
			foreach (MapItemVM mapItemVM in this._availableMaps)
			{
				this.MapSelection.AddItem(mapItemVM);
			}
			this.MapSelection.SelectedIndex = 0;
		}

		// Token: 0x06000155 RID: 341 RVA: 0x0000998C File Offset: 0x00007B8C
		public void RandomizeAll()
		{
			this.MapSelection.ExecuteRandomize();
			this.SceneLevelSelection.ExecuteRandomize();
			this.SeasonSelection.ExecuteRandomize();
			this.WallHitpointSelection.ExecuteRandomize();
			this.TimeOfDaySelection.ExecuteRandomize();
		}

		// Token: 0x17000062 RID: 98
		// (get) Token: 0x06000156 RID: 342 RVA: 0x000099C5 File Offset: 0x00007BC5
		// (set) Token: 0x06000157 RID: 343 RVA: 0x000099CD File Offset: 0x00007BCD
		[DataSourceProperty]
		public SelectorVM<MapItemVM> MapSelection
		{
			get
			{
				return this._mapSelection;
			}
			set
			{
				if (value != this._mapSelection)
				{
					this._mapSelection = value;
					base.OnPropertyChangedWithValue<SelectorVM<MapItemVM>>(value, "MapSelection");
				}
			}
		}

		// Token: 0x17000063 RID: 99
		// (get) Token: 0x06000158 RID: 344 RVA: 0x000099EB File Offset: 0x00007BEB
		// (set) Token: 0x06000159 RID: 345 RVA: 0x000099F3 File Offset: 0x00007BF3
		[DataSourceProperty]
		public SelectorVM<SceneLevelItemVM> SceneLevelSelection
		{
			get
			{
				return this._sceneLevelSelection;
			}
			set
			{
				if (value != this._sceneLevelSelection)
				{
					this._sceneLevelSelection = value;
					base.OnPropertyChangedWithValue<SelectorVM<SceneLevelItemVM>>(value, "SceneLevelSelection");
				}
			}
		}

		// Token: 0x17000064 RID: 100
		// (get) Token: 0x0600015A RID: 346 RVA: 0x00009A11 File Offset: 0x00007C11
		// (set) Token: 0x0600015B RID: 347 RVA: 0x00009A19 File Offset: 0x00007C19
		[DataSourceProperty]
		public SelectorVM<WallHitpointItemVM> WallHitpointSelection
		{
			get
			{
				return this._wallHitpointSelection;
			}
			set
			{
				if (value != this._wallHitpointSelection)
				{
					this._wallHitpointSelection = value;
					base.OnPropertyChangedWithValue<SelectorVM<WallHitpointItemVM>>(value, "WallHitpointSelection");
				}
			}
		}

		// Token: 0x17000065 RID: 101
		// (get) Token: 0x0600015C RID: 348 RVA: 0x00009A37 File Offset: 0x00007C37
		// (set) Token: 0x0600015D RID: 349 RVA: 0x00009A3F File Offset: 0x00007C3F
		[DataSourceProperty]
		public SelectorVM<SeasonItemVM> SeasonSelection
		{
			get
			{
				return this._seasonSelection;
			}
			set
			{
				if (value != this._seasonSelection)
				{
					this._seasonSelection = value;
					base.OnPropertyChangedWithValue<SelectorVM<SeasonItemVM>>(value, "SeasonSelection");
				}
			}
		}

		// Token: 0x17000066 RID: 102
		// (get) Token: 0x0600015E RID: 350 RVA: 0x00009A5D File Offset: 0x00007C5D
		// (set) Token: 0x0600015F RID: 351 RVA: 0x00009A65 File Offset: 0x00007C65
		[DataSourceProperty]
		public SelectorVM<TimeOfDayItemVM> TimeOfDaySelection
		{
			get
			{
				return this._timeOfDaySelection;
			}
			set
			{
				if (value != this._timeOfDaySelection)
				{
					this._timeOfDaySelection = value;
					base.OnPropertyChangedWithValue<SelectorVM<TimeOfDayItemVM>>(value, "TimeOfDaySelection");
				}
			}
		}

		// Token: 0x17000067 RID: 103
		// (get) Token: 0x06000160 RID: 352 RVA: 0x00009A83 File Offset: 0x00007C83
		// (set) Token: 0x06000161 RID: 353 RVA: 0x00009A8B File Offset: 0x00007C8B
		[DataSourceProperty]
		public bool IsCurrentMapSiege
		{
			get
			{
				return this._isCurrentMapSiege;
			}
			set
			{
				if (value != this._isCurrentMapSiege)
				{
					this._isCurrentMapSiege = value;
					base.OnPropertyChangedWithValue(value, "IsCurrentMapSiege");
				}
			}
		}

		// Token: 0x17000068 RID: 104
		// (get) Token: 0x06000162 RID: 354 RVA: 0x00009AA9 File Offset: 0x00007CA9
		// (set) Token: 0x06000163 RID: 355 RVA: 0x00009AB1 File Offset: 0x00007CB1
		[DataSourceProperty]
		public bool IsSallyOutSelected
		{
			get
			{
				return this._isSallyOutSelected;
			}
			set
			{
				if (value != this._isSallyOutSelected)
				{
					this._isSallyOutSelected = value;
					base.OnPropertyChangedWithValue(value, "IsSallyOutSelected");
				}
			}
		}

		// Token: 0x17000069 RID: 105
		// (get) Token: 0x06000164 RID: 356 RVA: 0x00009ACF File Offset: 0x00007CCF
		// (set) Token: 0x06000165 RID: 357 RVA: 0x00009AD7 File Offset: 0x00007CD7
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

		// Token: 0x1700006A RID: 106
		// (get) Token: 0x06000166 RID: 358 RVA: 0x00009AFA File Offset: 0x00007CFA
		// (set) Token: 0x06000167 RID: 359 RVA: 0x00009B02 File Offset: 0x00007D02
		[DataSourceProperty]
		public string MapText
		{
			get
			{
				return this._mapText;
			}
			set
			{
				if (value != this._mapText)
				{
					this._mapText = value;
					base.OnPropertyChangedWithValue<string>(value, "MapText");
				}
			}
		}

		// Token: 0x1700006B RID: 107
		// (get) Token: 0x06000168 RID: 360 RVA: 0x00009B25 File Offset: 0x00007D25
		// (set) Token: 0x06000169 RID: 361 RVA: 0x00009B2D File Offset: 0x00007D2D
		[DataSourceProperty]
		public string SeasonText
		{
			get
			{
				return this._seasonText;
			}
			set
			{
				if (value != this._seasonText)
				{
					this._seasonText = value;
					base.OnPropertyChangedWithValue<string>(value, "SeasonText");
				}
			}
		}

		// Token: 0x1700006C RID: 108
		// (get) Token: 0x0600016A RID: 362 RVA: 0x00009B50 File Offset: 0x00007D50
		// (set) Token: 0x0600016B RID: 363 RVA: 0x00009B58 File Offset: 0x00007D58
		[DataSourceProperty]
		public string TimeOfDayText
		{
			get
			{
				return this._timeOfDayText;
			}
			set
			{
				if (value != this._timeOfDayText)
				{
					this._timeOfDayText = value;
					base.OnPropertyChangedWithValue<string>(value, "TimeOfDayText");
				}
			}
		}

		// Token: 0x1700006D RID: 109
		// (get) Token: 0x0600016C RID: 364 RVA: 0x00009B7B File Offset: 0x00007D7B
		// (set) Token: 0x0600016D RID: 365 RVA: 0x00009B83 File Offset: 0x00007D83
		[DataSourceProperty]
		public string SceneLevelText
		{
			get
			{
				return this._sceneLevelText;
			}
			set
			{
				if (value != this._sceneLevelText)
				{
					this._sceneLevelText = value;
					base.OnPropertyChangedWithValue<string>(value, "SceneLevelText");
				}
			}
		}

		// Token: 0x1700006E RID: 110
		// (get) Token: 0x0600016E RID: 366 RVA: 0x00009BA6 File Offset: 0x00007DA6
		// (set) Token: 0x0600016F RID: 367 RVA: 0x00009BAE File Offset: 0x00007DAE
		[DataSourceProperty]
		public string WallHitpointsText
		{
			get
			{
				return this._wallHitpointsText;
			}
			set
			{
				if (value != this._wallHitpointsText)
				{
					this._wallHitpointsText = value;
					base.OnPropertyChangedWithValue<string>(value, "WallHitpointsText");
				}
			}
		}

		// Token: 0x1700006F RID: 111
		// (get) Token: 0x06000170 RID: 368 RVA: 0x00009BD1 File Offset: 0x00007DD1
		// (set) Token: 0x06000171 RID: 369 RVA: 0x00009BD9 File Offset: 0x00007DD9
		[DataSourceProperty]
		public string AttackerSiegeMachinesText
		{
			get
			{
				return this._attackerSiegeMachinesText;
			}
			set
			{
				if (value != this._attackerSiegeMachinesText)
				{
					this._attackerSiegeMachinesText = value;
					base.OnPropertyChangedWithValue<string>(value, "AttackerSiegeMachinesText");
				}
			}
		}

		// Token: 0x17000070 RID: 112
		// (get) Token: 0x06000172 RID: 370 RVA: 0x00009BFC File Offset: 0x00007DFC
		// (set) Token: 0x06000173 RID: 371 RVA: 0x00009C04 File Offset: 0x00007E04
		[DataSourceProperty]
		public string DefenderSiegeMachinesText
		{
			get
			{
				return this._defenderSiegeMachinesText;
			}
			set
			{
				if (value != this._defenderSiegeMachinesText)
				{
					this._defenderSiegeMachinesText = value;
					base.OnPropertyChangedWithValue<string>(value, "DefenderSiegeMachinesText");
				}
			}
		}

		// Token: 0x17000071 RID: 113
		// (get) Token: 0x06000174 RID: 372 RVA: 0x00009C27 File Offset: 0x00007E27
		// (set) Token: 0x06000175 RID: 373 RVA: 0x00009C2F File Offset: 0x00007E2F
		[DataSourceProperty]
		public string SalloutText
		{
			get
			{
				return this._salloutText;
			}
			set
			{
				if (value != this._salloutText)
				{
					this._salloutText = value;
					base.OnPropertyChangedWithValue<string>(value, "SalloutText");
				}
			}
		}

		// Token: 0x040000EE RID: 238
		private bool _isCurrentMapSiege;

		// Token: 0x040000EF RID: 239
		private bool _isSallyOutSelected;

		// Token: 0x040000F0 RID: 240
		private SelectorVM<MapItemVM> _mapSelection;

		// Token: 0x040000F1 RID: 241
		private SelectorVM<SceneLevelItemVM> _sceneLevelSelection;

		// Token: 0x040000F2 RID: 242
		private SelectorVM<WallHitpointItemVM> _wallHitpointSelection;

		// Token: 0x040000F3 RID: 243
		private SelectorVM<SeasonItemVM> _seasonSelection;

		// Token: 0x040000F4 RID: 244
		private SelectorVM<TimeOfDayItemVM> _timeOfDaySelection;

		// Token: 0x040000F5 RID: 245
		private string _titleText;

		// Token: 0x040000F6 RID: 246
		private string _mapText;

		// Token: 0x040000F7 RID: 247
		private string _seasonText;

		// Token: 0x040000F8 RID: 248
		private string _timeOfDayText;

		// Token: 0x040000F9 RID: 249
		private string _sceneLevelText;

		// Token: 0x040000FA RID: 250
		private string _wallHitpointsText;

		// Token: 0x040000FB RID: 251
		private string _attackerSiegeMachinesText;

		// Token: 0x040000FC RID: 252
		private string _defenderSiegeMachinesText;

		// Token: 0x040000FD RID: 253
		private string _salloutText;
	}
}
