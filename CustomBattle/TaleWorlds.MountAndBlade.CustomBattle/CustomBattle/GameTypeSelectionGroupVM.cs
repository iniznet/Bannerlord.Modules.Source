using System;
using TaleWorlds.Core.ViewModelCollection.Selector;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.CustomBattle.CustomBattle.SelectionItem;

namespace TaleWorlds.MountAndBlade.CustomBattle.CustomBattle
{
	public class GameTypeSelectionGroupVM : ViewModel
	{
		public CustomBattleGameType SelectedGameType { get; private set; }

		public CustomBattlePlayerType SelectedPlayerType { get; private set; }

		public CustomBattlePlayerSide SelectedPlayerSide { get; private set; }

		public GameTypeSelectionGroupVM(Action<CustomBattlePlayerType> onPlayerTypeChange, Action<CustomBattleGameType> onGameTypeChange)
		{
			this._onPlayerTypeChange = onPlayerTypeChange;
			this._onGameTypeChange = onGameTypeChange;
			this.GameTypeSelection = new SelectorVM<GameTypeItemVM>(0, new Action<SelectorVM<GameTypeItemVM>>(this.OnGameTypeSelection));
			this.PlayerTypeSelection = new SelectorVM<PlayerTypeItemVM>(0, new Action<SelectorVM<PlayerTypeItemVM>>(this.OnPlayerTypeSelection));
			this.PlayerSideSelection = new SelectorVM<PlayerSideItemVM>(0, new Action<SelectorVM<PlayerSideItemVM>>(this.OnPlayerSideSelection));
			this.RefreshValues();
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
			this.GameTypeText = new TextObject("{=JPimShCw}Game Type", null).ToString();
			this.PlayerTypeText = new TextObject("{=bKg8Mmwb}Player Type", null).ToString();
			this.PlayerSideText = new TextObject("{=P3rMg4uZ}Player Side", null).ToString();
			this.GameTypeSelection.ItemList.Clear();
			this.PlayerTypeSelection.ItemList.Clear();
			this.PlayerSideSelection.ItemList.Clear();
			foreach (Tuple<string, CustomBattleGameType> tuple in CustomBattleData.GameTypes)
			{
				this.GameTypeSelection.AddItem(new GameTypeItemVM(tuple.Item1, tuple.Item2));
			}
			foreach (Tuple<string, CustomBattlePlayerType> tuple2 in CustomBattleData.PlayerTypes)
			{
				this.PlayerTypeSelection.AddItem(new PlayerTypeItemVM(tuple2.Item1, tuple2.Item2));
			}
			foreach (Tuple<string, CustomBattlePlayerSide> tuple3 in CustomBattleData.PlayerSides)
			{
				this.PlayerSideSelection.AddItem(new PlayerSideItemVM(tuple3.Item1, tuple3.Item2));
			}
			this.GameTypeSelection.SelectedIndex = 0;
			this.PlayerTypeSelection.SelectedIndex = 0;
			this.PlayerSideSelection.SelectedIndex = 0;
		}

		public void RandomizeAll()
		{
			this.GameTypeSelection.ExecuteRandomize();
			this.PlayerTypeSelection.ExecuteRandomize();
			this.PlayerSideSelection.ExecuteRandomize();
		}

		private void OnGameTypeSelection(SelectorVM<GameTypeItemVM> selector)
		{
			this.SelectedGameType = selector.SelectedItem.GameType;
			this._onGameTypeChange(this.SelectedGameType);
		}

		private void OnPlayerTypeSelection(SelectorVM<PlayerTypeItemVM> selector)
		{
			this.SelectedPlayerType = selector.SelectedItem.PlayerType;
			this._onPlayerTypeChange(this.SelectedPlayerType);
		}

		private void OnPlayerSideSelection(SelectorVM<PlayerSideItemVM> selector)
		{
			this.SelectedPlayerSide = selector.SelectedItem.PlayerSide;
		}

		[DataSourceProperty]
		public SelectorVM<GameTypeItemVM> GameTypeSelection
		{
			get
			{
				return this._gameTypeSelection;
			}
			set
			{
				if (value != this._gameTypeSelection)
				{
					this._gameTypeSelection = value;
					base.OnPropertyChangedWithValue<SelectorVM<GameTypeItemVM>>(value, "GameTypeSelection");
				}
			}
		}

		[DataSourceProperty]
		public SelectorVM<PlayerTypeItemVM> PlayerTypeSelection
		{
			get
			{
				return this._playerTypeSelection;
			}
			set
			{
				if (value != this._playerTypeSelection)
				{
					this._playerTypeSelection = value;
					base.OnPropertyChangedWithValue<SelectorVM<PlayerTypeItemVM>>(value, "PlayerTypeSelection");
				}
			}
		}

		[DataSourceProperty]
		public SelectorVM<PlayerSideItemVM> PlayerSideSelection
		{
			get
			{
				return this._playerSideSelection;
			}
			set
			{
				if (value != this._playerSideSelection)
				{
					this._playerSideSelection = value;
					base.OnPropertyChangedWithValue<SelectorVM<PlayerSideItemVM>>(value, "PlayerSideSelection");
				}
			}
		}

		[DataSourceProperty]
		public string GameTypeText
		{
			get
			{
				return this._gameTypeText;
			}
			set
			{
				if (value != this._gameTypeText)
				{
					this._gameTypeText = value;
					base.OnPropertyChangedWithValue<string>(value, "GameTypeText");
				}
			}
		}

		[DataSourceProperty]
		public string PlayerTypeText
		{
			get
			{
				return this._playerTypeText;
			}
			set
			{
				if (value != this._playerTypeText)
				{
					this._playerTypeText = value;
					base.OnPropertyChangedWithValue<string>(value, "PlayerTypeText");
				}
			}
		}

		[DataSourceProperty]
		public string PlayerSideText
		{
			get
			{
				return this._playerSideText;
			}
			set
			{
				if (value != this._playerSideText)
				{
					this._playerSideText = value;
					base.OnPropertyChangedWithValue<string>(value, "PlayerSideText");
				}
			}
		}

		private readonly Action<CustomBattlePlayerType> _onPlayerTypeChange;

		private readonly Action<CustomBattleGameType> _onGameTypeChange;

		private SelectorVM<GameTypeItemVM> _gameTypeSelection;

		private SelectorVM<PlayerTypeItemVM> _playerTypeSelection;

		private SelectorVM<PlayerSideItemVM> _playerSideSelection;

		private string _gameTypeText;

		private string _playerTypeText;

		private string _playerSideText;
	}
}
