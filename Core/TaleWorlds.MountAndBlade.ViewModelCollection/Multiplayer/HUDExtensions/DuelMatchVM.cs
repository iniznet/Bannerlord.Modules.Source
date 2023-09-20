using System;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.HUDExtensions
{
	public class DuelMatchVM : ViewModel
	{
		public MissionPeer FirstPlayerPeer { get; private set; }

		public MissionPeer SecondPlayerPeer { get; private set; }

		public DuelMatchVM()
		{
			this.IsEnabled = false;
			this._duelCountdownText = new TextObject("{=cO2FDHCa}Duel with {OPPONENT_NAME} is starting in {DUEL_REMAINING_TIME} seconds.", null);
			this.RefreshValues();
		}

		public void OnDuelPrepStarted(MissionPeer opponentPeer, int prepDuration)
		{
			this._prepTimeRemaining = (float)prepDuration;
			GameTexts.SetVariable("OPPONENT_NAME", opponentPeer.DisplayedName);
			this.IsPreparing = true;
		}

		public void Tick(float dt)
		{
			if (this._prepTimeRemaining > 0f)
			{
				GameTexts.SetVariable("DUEL_REMAINING_TIME", (float)MathF.Ceiling(this._prepTimeRemaining));
				this.CountdownMessage = this._duelCountdownText.ToString();
				this._prepTimeRemaining -= dt;
				return;
			}
			this.IsPreparing = false;
		}

		public void OnDuelStarted(MissionPeer firstPeer, MissionPeer secondPeer, int arenaType)
		{
			this.FirstPlayerPeer = firstPeer;
			this.SecondPlayerPeer = secondPeer;
			this.FirstPlayerScore = 0;
			this.SecondPlayerScore = 0;
			this.FirstPlayer = new MPPlayerVM(firstPeer);
			this.SecondPlayer = new MPPlayerVM(secondPeer);
			this.FirstPlayer.RefreshDivision(true);
			this.SecondPlayer.RefreshDivision(true);
			this.ArenaType = arenaType;
			this.UpdateScore();
			this.IsEnabled = true;
		}

		public void OnDuelEnded()
		{
			this.FirstPlayerPeer = null;
			this.SecondPlayerPeer = null;
			this.IsEnabled = false;
		}

		public void OnPeerScored(MissionPeer peer)
		{
			if (peer == this.FirstPlayerPeer)
			{
				int num = this.FirstPlayerScore;
				this.FirstPlayerScore = num + 1;
			}
			else if (peer == this.SecondPlayerPeer)
			{
				int num = this.SecondPlayerScore;
				this.SecondPlayerScore = num + 1;
			}
			this.UpdateScore();
		}

		private void UpdateScore()
		{
			GameTexts.SetVariable("LEFT", this.FirstPlayerScore);
			GameTexts.SetVariable("RIGHT", this.SecondPlayerScore);
			this.Score = GameTexts.FindText("str_LEFT_dash_RIGHT", null).ToString();
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
		public bool IsPreparing
		{
			get
			{
				return this._isPreparing;
			}
			set
			{
				if (value != this._isPreparing)
				{
					this._isPreparing = value;
					base.OnPropertyChangedWithValue(value, "IsPreparing");
				}
			}
		}

		[DataSourceProperty]
		public string CountdownMessage
		{
			get
			{
				return this._countdownMessage;
			}
			set
			{
				if (value != this._countdownMessage)
				{
					this._countdownMessage = value;
					base.OnPropertyChangedWithValue<string>(value, "CountdownMessage");
				}
			}
		}

		[DataSourceProperty]
		public string Score
		{
			get
			{
				return this._score;
			}
			set
			{
				if (value != this._score)
				{
					this._score = value;
					base.OnPropertyChangedWithValue<string>(value, "Score");
				}
			}
		}

		[DataSourceProperty]
		public int ArenaType
		{
			get
			{
				return this._arenaType;
			}
			set
			{
				if (value != this._arenaType)
				{
					this._arenaType = value;
					base.OnPropertyChangedWithValue(value, "ArenaType");
				}
			}
		}

		[DataSourceProperty]
		public int FirstPlayerScore
		{
			get
			{
				return this._firstPlayerScore;
			}
			set
			{
				if (value != this._firstPlayerScore)
				{
					this._firstPlayerScore = value;
					base.OnPropertyChangedWithValue(value, "FirstPlayerScore");
				}
			}
		}

		[DataSourceProperty]
		public int SecondPlayerScore
		{
			get
			{
				return this._secondPlayerScore;
			}
			set
			{
				if (value != this._secondPlayerScore)
				{
					this._secondPlayerScore = value;
					base.OnPropertyChangedWithValue(value, "SecondPlayerScore");
				}
			}
		}

		[DataSourceProperty]
		public MPPlayerVM FirstPlayer
		{
			get
			{
				return this._firstPlayer;
			}
			set
			{
				if (value != this._firstPlayer)
				{
					this._firstPlayer = value;
					base.OnPropertyChangedWithValue<MPPlayerVM>(value, "FirstPlayer");
				}
			}
		}

		[DataSourceProperty]
		public MPPlayerVM SecondPlayer
		{
			get
			{
				return this._secondPlayer;
			}
			set
			{
				if (value != this._secondPlayer)
				{
					this._secondPlayer = value;
					base.OnPropertyChangedWithValue<MPPlayerVM>(value, "SecondPlayer");
				}
			}
		}

		private float _prepTimeRemaining;

		private TextObject _duelCountdownText;

		private bool _isEnabled;

		private bool _isPreparing;

		private string _countdownMessage;

		private string _score;

		private int _arenaType;

		private int _firstPlayerScore;

		private int _secondPlayerScore;

		private MPPlayerVM _firstPlayer;

		private MPPlayerVM _secondPlayer;
	}
}
