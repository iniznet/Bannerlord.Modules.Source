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
	// Token: 0x0200004B RID: 75
	public class MultiplayerTeamSelectVM : ViewModel
	{
		// Token: 0x170001D0 RID: 464
		// (get) Token: 0x06000624 RID: 1572 RVA: 0x000196B0 File Offset: 0x000178B0
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

		// Token: 0x06000625 RID: 1573 RVA: 0x000196D0 File Offset: 0x000178D0
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

		// Token: 0x06000626 RID: 1574 RVA: 0x000198E4 File Offset: 0x00017AE4
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

		// Token: 0x06000627 RID: 1575 RVA: 0x00019960 File Offset: 0x00017B60
		public void Tick(float dt)
		{
			this.RemainingRoundTime = TimeSpan.FromSeconds((double)MathF.Ceiling(this._gameMode.RemainingTime)).ToString("mm':'ss");
		}

		// Token: 0x06000628 RID: 1576 RVA: 0x00019998 File Offset: 0x00017B98
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

		// Token: 0x06000629 RID: 1577 RVA: 0x00019ABC File Offset: 0x00017CBC
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

		// Token: 0x0600062A RID: 1578 RVA: 0x00019B7D File Offset: 0x00017D7D
		public void RefreshFriendsPerTeam(IEnumerable<MissionPeer> friendsTeamOne, IEnumerable<MissionPeer> friendsTeamTwo)
		{
			this.Team1.RefreshFriends(friendsTeamOne);
			this.Team2.RefreshFriends(friendsTeamTwo);
		}

		// Token: 0x0600062B RID: 1579 RVA: 0x00019B97 File Offset: 0x00017D97
		[UsedImplicitly]
		public void ExecuteCancel()
		{
			this._onClose();
		}

		// Token: 0x0600062C RID: 1580 RVA: 0x00019BA4 File Offset: 0x00017DA4
		[UsedImplicitly]
		public void ExecuteAutoAssign()
		{
			this._onAutoAssign();
		}

		// Token: 0x170001D1 RID: 465
		// (get) Token: 0x0600062D RID: 1581 RVA: 0x00019BB1 File Offset: 0x00017DB1
		// (set) Token: 0x0600062E RID: 1582 RVA: 0x00019BB9 File Offset: 0x00017DB9
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

		// Token: 0x170001D2 RID: 466
		// (get) Token: 0x0600062F RID: 1583 RVA: 0x00019BD7 File Offset: 0x00017DD7
		// (set) Token: 0x06000630 RID: 1584 RVA: 0x00019BDF File Offset: 0x00017DDF
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

		// Token: 0x170001D3 RID: 467
		// (get) Token: 0x06000631 RID: 1585 RVA: 0x00019BFD File Offset: 0x00017DFD
		// (set) Token: 0x06000632 RID: 1586 RVA: 0x00019C05 File Offset: 0x00017E05
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

		// Token: 0x170001D4 RID: 468
		// (get) Token: 0x06000633 RID: 1587 RVA: 0x00019C23 File Offset: 0x00017E23
		// (set) Token: 0x06000634 RID: 1588 RVA: 0x00019C2B File Offset: 0x00017E2B
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

		// Token: 0x170001D5 RID: 469
		// (get) Token: 0x06000635 RID: 1589 RVA: 0x00019C40 File Offset: 0x00017E40
		// (set) Token: 0x06000636 RID: 1590 RVA: 0x00019C48 File Offset: 0x00017E48
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

		// Token: 0x170001D6 RID: 470
		// (get) Token: 0x06000637 RID: 1591 RVA: 0x00019C66 File Offset: 0x00017E66
		// (set) Token: 0x06000638 RID: 1592 RVA: 0x00019C6E File Offset: 0x00017E6E
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

		// Token: 0x170001D7 RID: 471
		// (get) Token: 0x06000639 RID: 1593 RVA: 0x00019C91 File Offset: 0x00017E91
		// (set) Token: 0x0600063A RID: 1594 RVA: 0x00019C99 File Offset: 0x00017E99
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

		// Token: 0x170001D8 RID: 472
		// (get) Token: 0x0600063B RID: 1595 RVA: 0x00019CAE File Offset: 0x00017EAE
		// (set) Token: 0x0600063C RID: 1596 RVA: 0x00019CB6 File Offset: 0x00017EB6
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

		// Token: 0x170001D9 RID: 473
		// (get) Token: 0x0600063D RID: 1597 RVA: 0x00019CCB File Offset: 0x00017ECB
		// (set) Token: 0x0600063E RID: 1598 RVA: 0x00019CD3 File Offset: 0x00017ED3
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

		// Token: 0x0400031B RID: 795
		private readonly Action _onClose;

		// Token: 0x0400031C RID: 796
		private readonly Action _onAutoAssign;

		// Token: 0x0400031D RID: 797
		private readonly MissionMultiplayerGameModeBaseClient _gameMode;

		// Token: 0x0400031E RID: 798
		private readonly MissionPeer _missionPeer;

		// Token: 0x0400031F RID: 799
		private readonly string _gamemodeStr;

		// Token: 0x04000320 RID: 800
		private string _teamSelectTitle;

		// Token: 0x04000321 RID: 801
		private bool _isRoundCountdownAvailable;

		// Token: 0x04000322 RID: 802
		private string _remainingRoundTime;

		// Token: 0x04000323 RID: 803
		private string _gamemodeLbl;

		// Token: 0x04000324 RID: 804
		private string _autoassignLbl;

		// Token: 0x04000325 RID: 805
		private bool _isCancelDisabled;

		// Token: 0x04000326 RID: 806
		private TeamSelectTeamInstanceVM _team1;

		// Token: 0x04000327 RID: 807
		private TeamSelectTeamInstanceVM _team2;

		// Token: 0x04000328 RID: 808
		private TeamSelectTeamInstanceVM _teamSpectators;
	}
}
