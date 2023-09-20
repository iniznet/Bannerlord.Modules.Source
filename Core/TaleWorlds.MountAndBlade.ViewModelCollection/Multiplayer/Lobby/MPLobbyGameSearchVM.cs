using System;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.Diamond;
using TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.Lobby.CustomGame;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.Lobby
{
	public class MPLobbyGameSearchVM : ViewModel
	{
		public MPCustomGameVM.CustomGameMode CustomGameMode { get; private set; }

		public MPLobbyGameSearchVM()
		{
			this.GameTypesText = new TextObject("{=cK5DE88I}N/A", null).ToString();
			this.RefreshValues();
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
			if (this.CustomGameMode == MPCustomGameVM.CustomGameMode.PremadeGame)
			{
				this.TitleText = new TextObject("{=dkPL25g9}Waiting for an opponent team", null).ToString();
				this.GameTypesText = "";
				this.ShowStats = false;
			}
			else
			{
				this.TitleText = new TextObject("{=FD7EQDmW}Looking for game", null).ToString();
				this.ShowStats = true;
			}
			GameTexts.SetVariable("STR1", "");
			GameTexts.SetVariable("STR2", new TextObject("{=mFMPj9zg}Searching for matches", null));
			this.CurrentWaitingTimeDescription = GameTexts.FindText("str_STR1_space_STR2", null).ToString();
			GameTexts.SetVariable("STR2", new TextObject("{=18yFEEIL}Estimated wait time", null));
			this.AverageWaitingTimeDescription = GameTexts.FindText("str_STR1_space_STR2", null).ToString();
			this.CancelText = new TextObject("{=3CpNUnVl}Cancel", null).ToString();
		}

		public void OnTick(float dt)
		{
			if (this.IsEnabled)
			{
				this._waitingTimeElapsed += dt;
				this.CurrentWaitingTime = this.SecondsToString(this._waitingTimeElapsed);
			}
		}

		public void SetEnabled(bool enabled)
		{
			this.IsEnabled = enabled;
			if (enabled)
			{
				this.CanCancelSearch = true;
				this._waitingTimeElapsed = 0f;
			}
			this.RefreshValues();
			if (this.CustomGameMode != MPCustomGameVM.CustomGameMode.PremadeGame)
			{
				this.UpdateCanCancel();
			}
		}

		public void UpdateData(MatchmakingWaitTimeStats matchmakingWaitTimeStats, string[] gameTypeInfo)
		{
			this.ShowStats = true;
			this.CustomGameMode = MPCustomGameVM.CustomGameMode.CustomServer;
			this.TitleText = new TextObject("{=FD7EQDmW}Looking for game", null).ToString();
			int num = 0;
			string[] array = this.GameTypesText.Replace(" ", "").Split(new char[] { ',' });
			foreach (string text in array)
			{
				WaitTimeStatType waitTimeStatType = WaitTimeStatType.SoloDuo;
				if (NetworkMain.GameClient.PlayersInParty.Count >= 3 && NetworkMain.GameClient.PlayersInParty.Count <= 5)
				{
					waitTimeStatType = WaitTimeStatType.Party;
				}
				else if (NetworkMain.GameClient.IsPartyFull)
				{
					waitTimeStatType = WaitTimeStatType.Premade;
				}
				num += matchmakingWaitTimeStats.GetWaitTime(NetworkMain.GetUserCurrentRegion(), text, waitTimeStatType);
			}
			this.AverageWaitingTime = this.SecondsToString((float)(num / array.Length));
			if (gameTypeInfo != null)
			{
				this.GameTypesText = MPLobbyVM.GetLocalizedGameTypesString(gameTypeInfo);
			}
		}

		public void UpdatePremadeGameData()
		{
			this.ShowStats = false;
			this.CustomGameMode = MPCustomGameVM.CustomGameMode.PremadeGame;
			this.TitleText = new TextObject("{=dkPL25g9}Waiting for an opponent team", null).ToString();
			this.GameTypesText = "";
		}

		public void OnJoinPremadeGameRequestSuccessful()
		{
			this.TitleText = new TextObject("{=5coyTZOI}Game is starting!", null).ToString();
		}

		public void OnRequestedToCancelSearchBattle()
		{
			this.CanCancelSearch = false;
		}

		public void UpdateCanCancel()
		{
			this.CanCancelSearch = !NetworkMain.GameClient.IsInParty || NetworkMain.GameClient.IsPartyLeader;
		}

		private void ExecuteCancel()
		{
			if (this.CustomGameMode != MPCustomGameVM.CustomGameMode.PremadeGame)
			{
				NetworkMain.GameClient.CancelFindGame();
				return;
			}
			NetworkMain.GameClient.CancelCreatingPremadeGame();
		}

		private string SecondsToString(float seconds)
		{
			return TimeSpan.FromSeconds((double)seconds).ToString((seconds >= 3600f) ? this._longTimeTextFormat : this._shortTimeTextFormat);
		}

		[DataSourceProperty]
		public bool IsEnabled
		{
			get
			{
				return this._isEnabled;
			}
			set
			{
				if (value != this._isEnabled)
				{
					this._isEnabled = value;
					base.OnPropertyChangedWithValue(value, "IsEnabled");
				}
			}
		}

		[DataSourceProperty]
		public bool CanCancelSearch
		{
			get
			{
				return this._canCancelSearch;
			}
			set
			{
				if (value != this._canCancelSearch)
				{
					this._canCancelSearch = value;
					base.OnPropertyChangedWithValue(value, "CanCancelSearch");
				}
			}
		}

		[DataSourceProperty]
		public bool ShowStats
		{
			get
			{
				return this._showStats;
			}
			set
			{
				if (value != this._showStats)
				{
					this._showStats = value;
					base.OnPropertyChangedWithValue(value, "ShowStats");
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
		public string GameTypesText
		{
			get
			{
				return this._gameTypesText;
			}
			set
			{
				if (value != this._gameTypesText)
				{
					this._gameTypesText = value;
					base.OnPropertyChangedWithValue<string>(value, "GameTypesText");
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
		public string AverageWaitingTime
		{
			get
			{
				return this._averageWaitingTime;
			}
			set
			{
				if (value != this._averageWaitingTime)
				{
					this._averageWaitingTime = value;
					base.OnPropertyChangedWithValue<string>(value, "AverageWaitingTime");
				}
			}
		}

		[DataSourceProperty]
		public string AverageWaitingTimeDescription
		{
			get
			{
				return this._averageWaitingTimeDescription;
			}
			set
			{
				if (value != this._averageWaitingTimeDescription)
				{
					this._averageWaitingTimeDescription = value;
					base.OnPropertyChangedWithValue<string>(value, "AverageWaitingTimeDescription");
				}
			}
		}

		[DataSourceProperty]
		public string CurrentWaitingTime
		{
			get
			{
				return this._currentWaitingTime;
			}
			set
			{
				if (value != this._currentWaitingTime)
				{
					this._currentWaitingTime = value;
					base.OnPropertyChangedWithValue<string>(value, "CurrentWaitingTime");
				}
			}
		}

		[DataSourceProperty]
		public string CurrentWaitingTimeDescription
		{
			get
			{
				return this._currentWaitingTimeDescription;
			}
			set
			{
				if (value != this._currentWaitingTimeDescription)
				{
					this._currentWaitingTimeDescription = value;
					base.OnPropertyChangedWithValue<string>(value, "CurrentWaitingTimeDescription");
				}
			}
		}

		private float _waitingTimeElapsed;

		private string _shortTimeTextFormat = "mm\\:ss";

		private string _longTimeTextFormat = "hh\\:mm\\:ss";

		private bool _isEnabled;

		private bool _canCancelSearch;

		private bool _showStats;

		private string _titleText;

		private string _gameTypesText;

		private string _cancelText;

		private string _averageWaitingTime;

		private string _averageWaitingTimeDescription;

		private string _currentWaitingTime;

		private string _currentWaitingTimeDescription;
	}
}
