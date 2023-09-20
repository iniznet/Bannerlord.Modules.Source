using System;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.Multiplayer.ViewModelCollection.Lobby
{
	public class MPLobbyGameTypeVM : ViewModel
	{
		public MPLobbyGameTypeVM(string gameType, bool isCasual, Action<string> onSelection)
		{
			this.GameTypeID = gameType;
			this.IsCasual = isCasual;
			this._onSelection = onSelection;
			this.RefreshValues();
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
			this.Hint = new HintViewModel(GameTexts.FindText("str_multiplayer_game_stats_description", this.GameTypeID), null);
		}

		private void OnSelected()
		{
			Action<string> onSelection = this._onSelection;
			if (onSelection == null)
			{
				return;
			}
			onSelection(this.GameTypeID);
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
					if (value)
					{
						this.OnSelected();
					}
				}
			}
		}

		[DataSourceProperty]
		public string GameTypeID
		{
			get
			{
				return this._gameTypeID;
			}
			set
			{
				if (value != this._gameTypeID)
				{
					this._gameTypeID = value;
					base.OnPropertyChangedWithValue<string>(value, "GameTypeID");
				}
			}
		}

		[DataSourceProperty]
		public HintViewModel Hint
		{
			get
			{
				return this._hint;
			}
			set
			{
				if (value != this._hint)
				{
					this._hint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "Hint");
				}
			}
		}

		private readonly Action<string> _onSelection;

		public readonly bool IsCasual;

		private bool _isSelected;

		private string _gameTypeID;

		private HintViewModel _hint;
	}
}
