using System;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.ObjectSystem;

namespace TaleWorlds.MountAndBlade.Multiplayer.ViewModelCollection.Scoreboard
{
	public class MPEndOfBattleVM : ViewModel
	{
		private MissionRepresentativeBase missionRep
		{
			get
			{
				NetworkCommunicator myPeer = GameNetwork.MyPeer;
				if (myPeer == null)
				{
					return null;
				}
				VirtualPlayer virtualPlayer = myPeer.VirtualPlayer;
				if (virtualPlayer == null)
				{
					return null;
				}
				return virtualPlayer.GetComponent<MissionRepresentativeBase>();
			}
		}

		public MPEndOfBattleVM(Mission mission, MissionScoreboardComponent missionScoreboardComponent, bool isSingleTeam)
		{
			this._missionScoreboardComponent = missionScoreboardComponent;
			this._gameMode = mission.GetMissionBehavior<MissionMultiplayerGameModeBaseClient>();
			this._lobbyComponent = mission.GetMissionBehavior<MissionLobbyComponent>();
			this._lobbyComponent.OnPostMatchEnded += this.OnPostMatchEnded;
			this._isSingleTeam = isSingleTeam;
			this.RefreshValues();
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
			this.CountdownTitle = new TextObject("{=wGjQgQlY}Next Game begins in:", null).ToString();
			this.Header = new TextObject("{=HXxNfncd}End of Battle", null).ToString();
			MPEndOfBattleSideVM allySide = this.AllySide;
			if (allySide != null)
			{
				allySide.RefreshValues();
			}
			MPEndOfBattleSideVM enemySide = this.EnemySide;
			if (enemySide == null)
			{
				return;
			}
			enemySide.RefreshValues();
		}

		public override void OnFinalize()
		{
			base.OnFinalize();
			this._lobbyComponent.OnPostMatchEnded -= this.OnPostMatchEnded;
		}

		public void Tick(float dt)
		{
			this.Countdown = MathF.Ceiling(this._gameMode.RemainingTime);
		}

		private void OnPostMatchEnded()
		{
			this.OnFinalRoundEnded();
		}

		private void OnFinalRoundEnded()
		{
			if (this._isSingleTeam)
			{
				return;
			}
			this.IsAvailable = true;
			this.InitSides();
			MissionScoreboardComponent missionScoreboardComponent = this._missionScoreboardComponent;
			BattleSideEnum battleSideEnum = ((missionScoreboardComponent != null) ? missionScoreboardComponent.GetMatchWinnerSide() : (-1));
			if (battleSideEnum == this._enemyBattleSide)
			{
				this.BattleResult = 0;
				this.ResultText = GameTexts.FindText("str_defeat", null).ToString();
				return;
			}
			if (battleSideEnum == this._allyBattleSide)
			{
				this.BattleResult = 1;
				this.ResultText = GameTexts.FindText("str_victory", null).ToString();
				return;
			}
			this.BattleResult = 2;
			this.ResultText = GameTexts.FindText("str_draw", null).ToString();
		}

		private void InitSides()
		{
			this._allyBattleSide = 1;
			this._enemyBattleSide = 0;
			NetworkCommunicator myPeer = GameNetwork.MyPeer;
			MissionPeer missionPeer = ((myPeer != null) ? PeerExtensions.GetComponent<MissionPeer>(myPeer) : null);
			if (missionPeer != null)
			{
				Team team = missionPeer.Team;
				if (team != null && team.Side == null)
				{
					this._allyBattleSide = 0;
					this._enemyBattleSide = 1;
				}
			}
			MissionScoreboardComponent.MissionScoreboardSide missionScoreboardSide = this._missionScoreboardComponent.Sides.FirstOrDefault((MissionScoreboardComponent.MissionScoreboardSide s) => s != null && s.Side == this._allyBattleSide);
			if (missionScoreboardSide != null)
			{
				string text = ((missionScoreboardSide.Side == 1) ? MultiplayerOptionsExtensions.GetStrValue(14, 0) : MultiplayerOptionsExtensions.GetStrValue(15, 0));
				this.AllySide = new MPEndOfBattleSideVM(this._missionScoreboardComponent, missionScoreboardSide, MBObjectManager.Instance.GetObject<BasicCultureObject>(text), missionScoreboardSide.Side == 0);
			}
			missionScoreboardSide = this._missionScoreboardComponent.Sides.FirstOrDefault((MissionScoreboardComponent.MissionScoreboardSide s) => s != null && s.Side == this._enemyBattleSide);
			if (missionScoreboardSide != null)
			{
				string text2 = ((missionScoreboardSide.Side == 1) ? MultiplayerOptionsExtensions.GetStrValue(14, 0) : MultiplayerOptionsExtensions.GetStrValue(15, 0));
				this.EnemySide = new MPEndOfBattleSideVM(this._missionScoreboardComponent, missionScoreboardSide, MBObjectManager.Instance.GetObject<BasicCultureObject>(text2), missionScoreboardSide.Side == 0);
			}
		}

		[DataSourceProperty]
		public bool IsAvailable
		{
			get
			{
				return this._isAvailable;
			}
			set
			{
				if (value != this._isAvailable)
				{
					this._isAvailable = value;
					base.OnPropertyChangedWithValue(value, "IsAvailable");
				}
			}
		}

		[DataSourceProperty]
		public string CountdownTitle
		{
			get
			{
				return this._countdownTitle;
			}
			set
			{
				if (value != this._countdownTitle)
				{
					this._countdownTitle = value;
					base.OnPropertyChangedWithValue<string>(value, "CountdownTitle");
				}
			}
		}

		[DataSourceProperty]
		public int Countdown
		{
			get
			{
				return this._countdown;
			}
			set
			{
				if (value != this._countdown)
				{
					this._countdown = value;
					base.OnPropertyChangedWithValue(value, "Countdown");
				}
			}
		}

		[DataSourceProperty]
		public string Header
		{
			get
			{
				return this._header;
			}
			set
			{
				if (value != this._header)
				{
					this._header = value;
					base.OnPropertyChangedWithValue<string>(value, "Header");
				}
			}
		}

		[DataSourceProperty]
		public int BattleResult
		{
			get
			{
				return this._battleResult;
			}
			set
			{
				if (value != this._battleResult)
				{
					this._battleResult = value;
					base.OnPropertyChangedWithValue(value, "BattleResult");
				}
			}
		}

		[DataSourceProperty]
		public string ResultText
		{
			get
			{
				return this._resultText;
			}
			set
			{
				if (value != this._resultText)
				{
					this._resultText = value;
					base.OnPropertyChangedWithValue<string>(value, "ResultText");
				}
			}
		}

		[DataSourceProperty]
		public MPEndOfBattleSideVM AllySide
		{
			get
			{
				return this._allySide;
			}
			set
			{
				if (value != this._allySide)
				{
					this._allySide = value;
					base.OnPropertyChangedWithValue<MPEndOfBattleSideVM>(value, "AllySide");
				}
			}
		}

		[DataSourceProperty]
		public MPEndOfBattleSideVM EnemySide
		{
			get
			{
				return this._enemySide;
			}
			set
			{
				if (value != this._enemySide)
				{
					this._enemySide = value;
					base.OnPropertyChangedWithValue<MPEndOfBattleSideVM>(value, "EnemySide");
				}
			}
		}

		private MissionScoreboardComponent _missionScoreboardComponent;

		private MissionMultiplayerGameModeBaseClient _gameMode;

		private MissionLobbyComponent _lobbyComponent;

		private bool _isSingleTeam;

		private BattleSideEnum _allyBattleSide;

		private BattleSideEnum _enemyBattleSide;

		private bool _isAvailable;

		private string _countdownTitle;

		private int _countdown;

		private string _header;

		private int _battleResult;

		private string _resultText;

		private MPEndOfBattleSideVM _allySide;

		private MPEndOfBattleSideVM _enemySide;
	}
}
