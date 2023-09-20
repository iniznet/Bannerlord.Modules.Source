using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.ObjectSystem;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.TeamSelection
{
	public class MultiplayerTeamSelectVM : ViewModel
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

		public MultiplayerTeamSelectVM(Mission mission, Action<Team> onChangeTeamTo, Action onAutoAssign, Action onClose, IEnumerable<Team> teams, string gamemode)
		{
			this._onClose = onClose;
			this._onAutoAssign = onAutoAssign;
			this._gamemodeStr = gamemode;
			Debug.Print("MultiplayerTeamSelectVM 1", 0, Debug.DebugColor.White, 17179869184UL);
			this._gameMode = mission.GetMissionBehavior<MissionMultiplayerGameModeBaseClient>();
			MissionScoreboardComponent missionBehavior = mission.GetMissionBehavior<MissionScoreboardComponent>();
			Debug.Print("MultiplayerTeamSelectVM 2", 0, Debug.DebugColor.White, 17179869184UL);
			this.IsRoundCountdownAvailable = this._gameMode.IsGameModeUsingRoundCountdown;
			Debug.Print("MultiplayerTeamSelectVM 3", 0, Debug.DebugColor.White, 17179869184UL);
			Team team = teams.FirstOrDefault((Team t) => t.Side == BattleSideEnum.None);
			this.TeamSpectators = new TeamSelectTeamInstanceVM(missionBehavior, team, null, null, onChangeTeamTo, false);
			Debug.Print("MultiplayerTeamSelectVM 4", 0, Debug.DebugColor.White, 17179869184UL);
			Team team2 = teams.FirstOrDefault((Team t) => t.Side == BattleSideEnum.Attacker);
			BasicCultureObject basicCultureObject = MBObjectManager.Instance.GetObject<BasicCultureObject>(MultiplayerOptions.OptionType.CultureTeam1.GetStrValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions));
			this.Team1 = new TeamSelectTeamInstanceVM(missionBehavior, team2, basicCultureObject, BannerCode.CreateFrom(team2.Banner), onChangeTeamTo, false);
			Debug.Print("MultiplayerTeamSelectVM 5", 0, Debug.DebugColor.White, 17179869184UL);
			Team team3 = teams.FirstOrDefault((Team t) => t.Side == BattleSideEnum.Defender);
			basicCultureObject = MBObjectManager.Instance.GetObject<BasicCultureObject>(MultiplayerOptions.OptionType.CultureTeam2.GetStrValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions));
			this.Team2 = new TeamSelectTeamInstanceVM(missionBehavior, team3, basicCultureObject, BannerCode.CreateFrom(team3.Banner), onChangeTeamTo, true);
			Debug.Print("MultiplayerTeamSelectVM 6", 0, Debug.DebugColor.White, 17179869184UL);
			if (GameNetwork.IsMyPeerReady)
			{
				this._missionPeer = GameNetwork.MyPeer.GetComponent<MissionPeer>();
				this.IsCancelDisabled = this._missionPeer.Team == null;
			}
			Debug.Print("MultiplayerTeamSelectVM 7", 0, Debug.DebugColor.White, 17179869184UL);
			this.RefreshValues();
			Debug.Print("MultiplayerTeamSelectVM 8", 0, Debug.DebugColor.White, 17179869184UL);
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
			this.AutoassignLbl = new TextObject("{=bON4Kn6B}Auto Assign", null).ToString();
			this.TeamSelectTitle = new TextObject("{=aVixswW5}Team Selection", null).ToString();
			this.GamemodeLbl = GameTexts.FindText("str_multiplayer_official_game_type_name", this._gamemodeStr).ToString();
			this.Team1.RefreshValues();
			this._team2.RefreshValues();
			this._teamSpectators.RefreshValues();
		}

		public void Tick(float dt)
		{
			this.RemainingRoundTime = TimeSpan.FromSeconds((double)MathF.Ceiling(this._gameMode.RemainingTime)).ToString("mm':'ss");
		}

		public void RefreshDisabledTeams(List<Team> disabledTeams)
		{
			if (disabledTeams == null)
			{
				TeamSelectTeamInstanceVM teamSpectators = this.TeamSpectators;
				if (teamSpectators != null)
				{
					teamSpectators.SetIsDisabled(false, false);
				}
				TeamSelectTeamInstanceVM team = this.Team1;
				if (team != null)
				{
					team.SetIsDisabled(false, false);
				}
				TeamSelectTeamInstanceVM team2 = this.Team2;
				if (team2 == null)
				{
					return;
				}
				team2.SetIsDisabled(false, false);
				return;
			}
			else
			{
				TeamSelectTeamInstanceVM teamSpectators2 = this.TeamSpectators;
				if (teamSpectators2 != null)
				{
					bool flag = false;
					bool flag2;
					if (disabledTeams == null)
					{
						flag2 = false;
					}
					else
					{
						TeamSelectTeamInstanceVM teamSpectators3 = this.TeamSpectators;
						flag2 = disabledTeams.Contains((teamSpectators3 != null) ? teamSpectators3.Team : null);
					}
					teamSpectators2.SetIsDisabled(flag, flag2);
				}
				TeamSelectTeamInstanceVM team3 = this.Team1;
				if (team3 != null)
				{
					TeamSelectTeamInstanceVM team4 = this.Team1;
					Team team5 = ((team4 != null) ? team4.Team : null);
					MissionPeer missionPeer = this._missionPeer;
					bool flag3 = team5 == ((missionPeer != null) ? missionPeer.Team : null);
					bool flag4;
					if (disabledTeams == null)
					{
						flag4 = false;
					}
					else
					{
						TeamSelectTeamInstanceVM team6 = this.Team1;
						flag4 = disabledTeams.Contains((team6 != null) ? team6.Team : null);
					}
					team3.SetIsDisabled(flag3, flag4);
				}
				TeamSelectTeamInstanceVM team7 = this.Team2;
				if (team7 == null)
				{
					return;
				}
				TeamSelectTeamInstanceVM team8 = this.Team2;
				Team team9 = ((team8 != null) ? team8.Team : null);
				MissionPeer missionPeer2 = this._missionPeer;
				bool flag5 = team9 == ((missionPeer2 != null) ? missionPeer2.Team : null);
				bool flag6;
				if (disabledTeams == null)
				{
					flag6 = false;
				}
				else
				{
					TeamSelectTeamInstanceVM team10 = this.Team2;
					flag6 = disabledTeams.Contains((team10 != null) ? team10.Team : null);
				}
				team7.SetIsDisabled(flag5, flag6);
				return;
			}
		}

		public void RefreshPlayerAndBotCount(int playersCountOne, int playersCountTwo, int botsCountOne, int botsCountTwo)
		{
			MBTextManager.SetTextVariable("PLAYER_COUNT", playersCountOne.ToString(), false);
			this.Team1.DisplayedSecondary = new TextObject("{=Etjqamlh}{PLAYER_COUNT} Players", null).ToString();
			MBTextManager.SetTextVariable("BOT_COUNT", botsCountOne.ToString(), false);
			this.Team1.DisplayedSecondarySub = new TextObject("{=eCOJSSUH}({BOT_COUNT} Bots)", null).ToString();
			MBTextManager.SetTextVariable("PLAYER_COUNT", playersCountTwo.ToString(), false);
			this.Team2.DisplayedSecondary = new TextObject("{=Etjqamlh}{PLAYER_COUNT} Players", null).ToString();
			MBTextManager.SetTextVariable("BOT_COUNT", botsCountTwo.ToString(), false);
			this.Team2.DisplayedSecondarySub = new TextObject("{=eCOJSSUH}({BOT_COUNT} Bots)", null).ToString();
		}

		public void RefreshFriendsPerTeam(IEnumerable<MissionPeer> friendsTeamOne, IEnumerable<MissionPeer> friendsTeamTwo)
		{
			this.Team1.RefreshFriends(friendsTeamOne);
			this.Team2.RefreshFriends(friendsTeamTwo);
		}

		[UsedImplicitly]
		public void ExecuteCancel()
		{
			this._onClose();
		}

		[UsedImplicitly]
		public void ExecuteAutoAssign()
		{
			this._onAutoAssign();
		}

		[DataSourceProperty]
		public TeamSelectTeamInstanceVM Team1
		{
			get
			{
				return this._team1;
			}
			set
			{
				if (value != this._team1)
				{
					this._team1 = value;
					base.OnPropertyChangedWithValue<TeamSelectTeamInstanceVM>(value, "Team1");
				}
			}
		}

		[DataSourceProperty]
		public TeamSelectTeamInstanceVM Team2
		{
			get
			{
				return this._team2;
			}
			set
			{
				if (value != this._team2)
				{
					this._team2 = value;
					base.OnPropertyChangedWithValue<TeamSelectTeamInstanceVM>(value, "Team2");
				}
			}
		}

		[DataSourceProperty]
		public TeamSelectTeamInstanceVM TeamSpectators
		{
			get
			{
				return this._teamSpectators;
			}
			set
			{
				if (value != this._teamSpectators)
				{
					this._teamSpectators = value;
					base.OnPropertyChangedWithValue<TeamSelectTeamInstanceVM>(value, "TeamSpectators");
				}
			}
		}

		[DataSourceProperty]
		public string TeamSelectTitle
		{
			get
			{
				return this._teamSelectTitle;
			}
			set
			{
				this._teamSelectTitle = value;
				base.OnPropertyChangedWithValue<string>(value, "TeamSelectTitle");
			}
		}

		[DataSourceProperty]
		public bool IsRoundCountdownAvailable
		{
			get
			{
				return this._isRoundCountdownAvailable;
			}
			set
			{
				if (value != this._isRoundCountdownAvailable)
				{
					this._isRoundCountdownAvailable = value;
					base.OnPropertyChangedWithValue(value, "IsRoundCountdownAvailable");
				}
			}
		}

		[DataSourceProperty]
		public string RemainingRoundTime
		{
			get
			{
				return this._remainingRoundTime;
			}
			set
			{
				if (value != this._remainingRoundTime)
				{
					this._remainingRoundTime = value;
					base.OnPropertyChangedWithValue<string>(value, "RemainingRoundTime");
				}
			}
		}

		[DataSourceProperty]
		public string GamemodeLbl
		{
			get
			{
				return this._gamemodeLbl;
			}
			set
			{
				this._gamemodeLbl = value;
				base.OnPropertyChangedWithValue<string>(value, "GamemodeLbl");
			}
		}

		[DataSourceProperty]
		public string AutoassignLbl
		{
			get
			{
				return this._autoassignLbl;
			}
			set
			{
				this._autoassignLbl = value;
				base.OnPropertyChangedWithValue<string>(value, "AutoassignLbl");
			}
		}

		[DataSourceProperty]
		public bool IsCancelDisabled
		{
			get
			{
				return this._isCancelDisabled;
			}
			set
			{
				this._isCancelDisabled = value;
				base.OnPropertyChangedWithValue(value, "IsCancelDisabled");
			}
		}

		private readonly Action _onClose;

		private readonly Action _onAutoAssign;

		private readonly MissionMultiplayerGameModeBaseClient _gameMode;

		private readonly MissionPeer _missionPeer;

		private readonly string _gamemodeStr;

		private string _teamSelectTitle;

		private bool _isRoundCountdownAvailable;

		private string _remainingRoundTime;

		private string _gamemodeLbl;

		private string _autoassignLbl;

		private bool _isCancelDisabled;

		private TeamSelectTeamInstanceVM _team1;

		private TeamSelectTeamInstanceVM _team2;

		private TeamSelectTeamInstanceVM _teamSpectators;
	}
}
