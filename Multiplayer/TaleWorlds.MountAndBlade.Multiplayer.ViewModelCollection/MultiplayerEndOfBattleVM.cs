using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.MissionRepresentatives;

namespace TaleWorlds.MountAndBlade.Multiplayer.ViewModelCollection
{
	public class MultiplayerEndOfBattleVM : ViewModel
	{
		public MultiplayerEndOfBattleVM()
		{
			this._activeDelay = MissionLobbyComponent.PostMatchWaitDuration / 2f;
			this._gameMode = Mission.Current.GetMissionBehavior<MissionMultiplayerGameModeBaseClient>();
			this.RefreshValues();
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
			this.TitleText = new TextObject("{=GPfkMajw}Battle Ended", null).ToString();
			this.DescriptionText = new TextObject("{=ADPaaX8R}Best Players of This Battle", null).ToString();
		}

		public void OnTick(float dt)
		{
			if (this._isBattleEnded)
			{
				this._activateTimeElapsed += dt;
				if (this._activateTimeElapsed >= this._activeDelay)
				{
					this._isBattleEnded = false;
					this.OnEnabled();
				}
			}
		}

		private void OnEnabled()
		{
			MissionScoreboardComponent missionBehavior = Mission.Current.GetMissionBehavior<MissionScoreboardComponent>();
			List<MissionPeer> list = new List<MissionPeer>();
			foreach (MissionScoreboardComponent.MissionScoreboardSide missionScoreboardSide in missionBehavior.Sides.Where((MissionScoreboardComponent.MissionScoreboardSide s) => s != null && s.Side != -1))
			{
				foreach (MissionPeer missionPeer in missionScoreboardSide.Players)
				{
					list.Add(missionPeer);
				}
			}
			list.Sort((MissionPeer p1, MissionPeer p2) => this.GetPeerScore(p2).CompareTo(this.GetPeerScore(p1)));
			if (list.Count > 0)
			{
				this.HasFirstPlace = true;
				MissionPeer missionPeer2 = list[0];
				this.FirstPlacePlayer = new MPEndOfBattlePlayerVM(missionPeer2, this.GetPeerScore(missionPeer2), 1);
			}
			if (list.Count > 1)
			{
				this.HasSecondPlace = true;
				MissionPeer missionPeer3 = list[1];
				this.SecondPlacePlayer = new MPEndOfBattlePlayerVM(missionPeer3, this.GetPeerScore(missionPeer3), 2);
			}
			if (list.Count > 2)
			{
				this.HasThirdPlace = true;
				MissionPeer missionPeer4 = list[2];
				this.ThirdPlacePlayer = new MPEndOfBattlePlayerVM(missionPeer4, this.GetPeerScore(missionPeer4), 3);
			}
			this.IsEnabled = true;
		}

		public void OnBattleEnded()
		{
			this._isBattleEnded = true;
		}

		private int GetPeerScore(MissionPeer peer)
		{
			if (peer == null)
			{
				return 0;
			}
			if (this._gameMode.GameType != 2)
			{
				return peer.Score;
			}
			DuelMissionRepresentative component = peer.GetComponent<DuelMissionRepresentative>();
			if (component == null)
			{
				return 0;
			}
			return component.Score;
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
		public bool HasFirstPlace
		{
			get
			{
				return this._hasFirstPlace;
			}
			set
			{
				if (value != this._hasFirstPlace)
				{
					this._hasFirstPlace = value;
					base.OnPropertyChangedWithValue(value, "HasFirstPlace");
				}
			}
		}

		[DataSourceProperty]
		public bool HasSecondPlace
		{
			get
			{
				return this._hasSecondPlace;
			}
			set
			{
				if (value != this._hasSecondPlace)
				{
					this._hasSecondPlace = value;
					base.OnPropertyChangedWithValue(value, "HasSecondPlace");
				}
			}
		}

		[DataSourceProperty]
		public bool HasThirdPlace
		{
			get
			{
				return this._hasThirdPlace;
			}
			set
			{
				if (value != this._hasThirdPlace)
				{
					this._hasThirdPlace = value;
					base.OnPropertyChangedWithValue(value, "HasThirdPlace");
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
		public string DescriptionText
		{
			get
			{
				return this._descriptionText;
			}
			set
			{
				if (value != this._descriptionText)
				{
					this._descriptionText = value;
					base.OnPropertyChangedWithValue<string>(value, "DescriptionText");
				}
			}
		}

		[DataSourceProperty]
		public MPEndOfBattlePlayerVM FirstPlacePlayer
		{
			get
			{
				return this._firstPlacePlayer;
			}
			set
			{
				if (value != this._firstPlacePlayer)
				{
					this._firstPlacePlayer = value;
					base.OnPropertyChangedWithValue<MPEndOfBattlePlayerVM>(value, "FirstPlacePlayer");
				}
			}
		}

		[DataSourceProperty]
		public MPEndOfBattlePlayerVM SecondPlacePlayer
		{
			get
			{
				return this._secondPlacePlayer;
			}
			set
			{
				if (value != this._secondPlacePlayer)
				{
					this._secondPlacePlayer = value;
					base.OnPropertyChangedWithValue<MPEndOfBattlePlayerVM>(value, "SecondPlacePlayer");
				}
			}
		}

		[DataSourceProperty]
		public MPEndOfBattlePlayerVM ThirdPlacePlayer
		{
			get
			{
				return this._thirdPlacePlayer;
			}
			set
			{
				if (value != this._thirdPlacePlayer)
				{
					this._thirdPlacePlayer = value;
					base.OnPropertyChangedWithValue<MPEndOfBattlePlayerVM>(value, "ThirdPlacePlayer");
				}
			}
		}

		private readonly MissionMultiplayerGameModeBaseClient _gameMode;

		private readonly float _activeDelay;

		private bool _isBattleEnded;

		private float _activateTimeElapsed;

		private bool _isEnabled;

		private bool _hasFirstPlace;

		private bool _hasSecondPlace;

		private bool _hasThirdPlace;

		private string _titleText;

		private string _descriptionText;

		private MPEndOfBattlePlayerVM _firstPlacePlayer;

		private MPEndOfBattlePlayerVM _secondPlacePlayer;

		private MPEndOfBattlePlayerVM _thirdPlacePlayer;
	}
}
