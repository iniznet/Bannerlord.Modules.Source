using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core.ViewModelCollection.Selector;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.CustomBattle.CustomBattle.SelectionItem;

namespace TaleWorlds.MountAndBlade.CustomBattle.CustomBattle
{
	public class MapSelectionGroupVM : ViewModel
	{
		public int SelectedWallBreachedCount { get; private set; }

		public int SelectedSceneLevel { get; private set; }

		public int SelectedTimeOfDay { get; private set; }

		public string SelectedSeasonId { get; private set; }

		public MapItemVM SelectedMap { get; private set; }

		private List<MapItemVM> _battleMaps { get; set; }

		private List<MapItemVM> _villageMaps { get; set; }

		private List<MapItemVM> _siegeMaps { get; set; }

		private List<MapItemVM> _availableMaps { get; set; }

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

		public void ExecuteSallyOutChange()
		{
			this.IsSallyOutSelected = !this.IsSallyOutSelected;
		}

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

		private void OnMapSelection(SelectorVM<MapItemVM> selector)
		{
			this.SelectedMap = selector.SelectedItem;
		}

		private void OnWallHitpointSelection(SelectorVM<WallHitpointItemVM> selector)
		{
			this.SelectedWallBreachedCount = selector.SelectedItem.BreachedWallCount;
		}

		private void OnSceneLevelSelection(SelectorVM<SceneLevelItemVM> selector)
		{
			this.SelectedSceneLevel = selector.SelectedItem.Level;
		}

		private void OnSeasonSelection(SelectorVM<SeasonItemVM> selector)
		{
			this.SelectedSeasonId = selector.SelectedItem.SeasonId;
		}

		private void OnTimeOfDaySelection(SelectorVM<TimeOfDayItemVM> selector)
		{
			this.SelectedTimeOfDay = selector.SelectedItem.TimeOfDay;
		}

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

		public void RandomizeAll()
		{
			this.MapSelection.ExecuteRandomize();
			this.SceneLevelSelection.ExecuteRandomize();
			this.SeasonSelection.ExecuteRandomize();
			this.WallHitpointSelection.ExecuteRandomize();
			this.TimeOfDaySelection.ExecuteRandomize();
		}

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

		private bool _isCurrentMapSiege;

		private bool _isSallyOutSelected;

		private SelectorVM<MapItemVM> _mapSelection;

		private SelectorVM<SceneLevelItemVM> _sceneLevelSelection;

		private SelectorVM<WallHitpointItemVM> _wallHitpointSelection;

		private SelectorVM<SeasonItemVM> _seasonSelection;

		private SelectorVM<TimeOfDayItemVM> _timeOfDaySelection;

		private string _titleText;

		private string _mapText;

		private string _seasonText;

		private string _timeOfDayText;

		private string _sceneLevelText;

		private string _wallHitpointsText;

		private string _attackerSiegeMachinesText;

		private string _defenderSiegeMachinesText;

		private string _salloutText;
	}
}
