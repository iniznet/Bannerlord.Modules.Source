using System;
using TaleWorlds.Core.ViewModelCollection.Selector;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.CustomBattle.CustomBattle.SelectionItem;

namespace TaleWorlds.MountAndBlade.CustomBattle.CustomBattle
{
	// Token: 0x0200001B RID: 27
	public class GameTypeSelectionGroupVM : ViewModel
	{
		// Token: 0x17000050 RID: 80
		// (get) Token: 0x06000121 RID: 289 RVA: 0x00008EDA File Offset: 0x000070DA
		// (set) Token: 0x06000122 RID: 290 RVA: 0x00008EE2 File Offset: 0x000070E2
		public CustomBattleGameType SelectedGameType { get; private set; }

		// Token: 0x17000051 RID: 81
		// (get) Token: 0x06000123 RID: 291 RVA: 0x00008EEB File Offset: 0x000070EB
		// (set) Token: 0x06000124 RID: 292 RVA: 0x00008EF3 File Offset: 0x000070F3
		public CustomBattlePlayerType SelectedPlayerType { get; private set; }

		// Token: 0x17000052 RID: 82
		// (get) Token: 0x06000125 RID: 293 RVA: 0x00008EFC File Offset: 0x000070FC
		// (set) Token: 0x06000126 RID: 294 RVA: 0x00008F04 File Offset: 0x00007104
		public CustomBattlePlayerSide SelectedPlayerSide { get; private set; }

		// Token: 0x06000127 RID: 295 RVA: 0x00008F10 File Offset: 0x00007110
		public GameTypeSelectionGroupVM(Action<CustomBattlePlayerType> onPlayerTypeChange, Action<CustomBattleGameType> onGameTypeChange)
		{
			this._onPlayerTypeChange = onPlayerTypeChange;
			this._onGameTypeChange = onGameTypeChange;
			this.GameTypeSelection = new SelectorVM<GameTypeItemVM>(0, new Action<SelectorVM<GameTypeItemVM>>(this.OnGameTypeSelection));
			this.PlayerTypeSelection = new SelectorVM<PlayerTypeItemVM>(0, new Action<SelectorVM<PlayerTypeItemVM>>(this.OnPlayerTypeSelection));
			this.PlayerSideSelection = new SelectorVM<PlayerSideItemVM>(0, new Action<SelectorVM<PlayerSideItemVM>>(this.OnPlayerSideSelection));
			this.RefreshValues();
		}

		// Token: 0x06000128 RID: 296 RVA: 0x00008F80 File Offset: 0x00007180
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

		// Token: 0x06000129 RID: 297 RVA: 0x00009128 File Offset: 0x00007328
		public void RandomizeAll()
		{
			this.GameTypeSelection.ExecuteRandomize();
			this.PlayerTypeSelection.ExecuteRandomize();
			this.PlayerSideSelection.ExecuteRandomize();
		}

		// Token: 0x0600012A RID: 298 RVA: 0x0000914B File Offset: 0x0000734B
		private void OnGameTypeSelection(SelectorVM<GameTypeItemVM> selector)
		{
			this.SelectedGameType = selector.SelectedItem.GameType;
			this._onGameTypeChange(this.SelectedGameType);
		}

		// Token: 0x0600012B RID: 299 RVA: 0x0000916F File Offset: 0x0000736F
		private void OnPlayerTypeSelection(SelectorVM<PlayerTypeItemVM> selector)
		{
			this.SelectedPlayerType = selector.SelectedItem.PlayerType;
			this._onPlayerTypeChange(this.SelectedPlayerType);
		}

		// Token: 0x0600012C RID: 300 RVA: 0x00009193 File Offset: 0x00007393
		private void OnPlayerSideSelection(SelectorVM<PlayerSideItemVM> selector)
		{
			this.SelectedPlayerSide = selector.SelectedItem.PlayerSide;
		}

		// Token: 0x17000053 RID: 83
		// (get) Token: 0x0600012D RID: 301 RVA: 0x000091A6 File Offset: 0x000073A6
		// (set) Token: 0x0600012E RID: 302 RVA: 0x000091AE File Offset: 0x000073AE
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

		// Token: 0x17000054 RID: 84
		// (get) Token: 0x0600012F RID: 303 RVA: 0x000091CC File Offset: 0x000073CC
		// (set) Token: 0x06000130 RID: 304 RVA: 0x000091D4 File Offset: 0x000073D4
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

		// Token: 0x17000055 RID: 85
		// (get) Token: 0x06000131 RID: 305 RVA: 0x000091F2 File Offset: 0x000073F2
		// (set) Token: 0x06000132 RID: 306 RVA: 0x000091FA File Offset: 0x000073FA
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

		// Token: 0x17000056 RID: 86
		// (get) Token: 0x06000133 RID: 307 RVA: 0x00009218 File Offset: 0x00007418
		// (set) Token: 0x06000134 RID: 308 RVA: 0x00009220 File Offset: 0x00007420
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

		// Token: 0x17000057 RID: 87
		// (get) Token: 0x06000135 RID: 309 RVA: 0x00009243 File Offset: 0x00007443
		// (set) Token: 0x06000136 RID: 310 RVA: 0x0000924B File Offset: 0x0000744B
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

		// Token: 0x17000058 RID: 88
		// (get) Token: 0x06000137 RID: 311 RVA: 0x0000926E File Offset: 0x0000746E
		// (set) Token: 0x06000138 RID: 312 RVA: 0x00009276 File Offset: 0x00007476
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

		// Token: 0x040000DD RID: 221
		private readonly Action<CustomBattlePlayerType> _onPlayerTypeChange;

		// Token: 0x040000DE RID: 222
		private readonly Action<CustomBattleGameType> _onGameTypeChange;

		// Token: 0x040000DF RID: 223
		private SelectorVM<GameTypeItemVM> _gameTypeSelection;

		// Token: 0x040000E0 RID: 224
		private SelectorVM<PlayerTypeItemVM> _playerTypeSelection;

		// Token: 0x040000E1 RID: 225
		private SelectorVM<PlayerSideItemVM> _playerSideSelection;

		// Token: 0x040000E2 RID: 226
		private string _gameTypeText;

		// Token: 0x040000E3 RID: 227
		private string _playerTypeText;

		// Token: 0x040000E4 RID: 228
		private string _playerSideText;
	}
}
