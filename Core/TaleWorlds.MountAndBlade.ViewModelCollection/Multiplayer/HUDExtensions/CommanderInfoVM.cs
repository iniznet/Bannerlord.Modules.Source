using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.Objects;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.HUDExtensions
{
	public class CommanderInfoVM : ViewModel
	{
		public CommanderInfoVM(MissionRepresentativeBase missionRepresentative)
		{
			this._missionRepresentative = missionRepresentative;
			this.AllyControlPoints = new MBBindingList<CapturePointVM>();
			this.NeutralControlPoints = new MBBindingList<CapturePointVM>();
			this.EnemyControlPoints = new MBBindingList<CapturePointVM>();
			this._gameMode = Mission.Current.GetMissionBehavior<MissionMultiplayerGameModeBaseClient>();
			this._missionScoreboardComponent = Mission.Current.GetMissionBehavior<MissionScoreboardComponent>();
			this._commanderInfo = Mission.Current.GetMissionBehavior<ICommanderInfo>();
			this.ShowTacticalInfo = true;
			if (this._gameMode != null)
			{
				this.UpdateWarmupDependentFlags(this._gameMode.IsInWarmup);
				this.UsePowerComparer = this._gameMode.GameType == MissionLobbyComponent.MultiplayerGameType.Battle && this._gameMode.ScoreboardComponent != null;
				if (this.UsePowerComparer)
				{
					this.PowerLevelComparer = new PowerLevelComparer(1.0, 1.0);
				}
				if (this.UseMoraleComparer)
				{
					this.RegisterMoraleEvents();
				}
			}
			this._siegeClient = Mission.Current.GetMissionBehavior<MissionMultiplayerSiegeClient>();
			if (this._siegeClient != null)
			{
				this._siegeClient.OnCapturePointRemainingMoraleGainsChangedEvent += this.OnCapturePointRemainingMoraleGainsChanged;
			}
			Mission.Current.OnMissionReset += this.OnMissionReset;
			MultiplayerMissionAgentVisualSpawnComponent missionBehavior = Mission.Current.GetMissionBehavior<MultiplayerMissionAgentVisualSpawnComponent>();
			missionBehavior.OnMyAgentSpawnedFromVisual += this.OnPreparationEnded;
			missionBehavior.OnMyAgentVisualSpawned += this.OnRoundStarted;
			this.OnTeamChanged();
		}

		private void OnRoundStarted()
		{
			this.OnTeamChanged();
			if (this.UsePowerComparer)
			{
				this._attackerTeamInitialMemberCount = this._missionScoreboardComponent.Sides[1].Players.Count<MissionPeer>();
				this._defenderTeamInitialMemberCount = this._missionScoreboardComponent.Sides[0].Players.Count<MissionPeer>();
			}
		}

		private void RegisterMoraleEvents()
		{
			if (!this._areMoraleEventsRegistered)
			{
				this._commanderInfo.OnMoraleChangedEvent += this.OnUpdateMorale;
				this._commanderInfo.OnFlagNumberChangedEvent += this.OnNumberOfCapturePointsChanged;
				this._commanderInfo.OnCapturePointOwnerChangedEvent += this.OnCapturePointOwnerChanged;
				this.AreMoralesIndependent = this._commanderInfo.AreMoralesIndependent;
				this.ResetCapturePointLists();
				this.InitCapturePoints();
				this._areMoraleEventsRegistered = true;
			}
		}

		private void OnPreparationEnded()
		{
			this.ShowTacticalInfo = true;
			this.OnTeamChanged();
		}

		public override void OnFinalize()
		{
			base.OnFinalize();
			if (this._commanderInfo != null)
			{
				this._commanderInfo.OnMoraleChangedEvent -= this.OnUpdateMorale;
				this._commanderInfo.OnFlagNumberChangedEvent -= this.InitCapturePoints;
			}
			Mission.Current.OnMissionReset -= this.OnMissionReset;
			MultiplayerMissionAgentVisualSpawnComponent missionBehavior = Mission.Current.GetMissionBehavior<MultiplayerMissionAgentVisualSpawnComponent>();
			missionBehavior.OnMyAgentSpawnedFromVisual -= this.OnPreparationEnded;
			missionBehavior.OnMyAgentVisualSpawned -= this.OnRoundStarted;
			if (this._siegeClient != null)
			{
				this._siegeClient.OnCapturePointRemainingMoraleGainsChangedEvent -= this.OnCapturePointRemainingMoraleGainsChanged;
			}
		}

		public void UpdateWarmupDependentFlags(bool isInWarmup)
		{
			this.UseMoraleComparer = !isInWarmup && this._gameMode.IsGameModeTactical && this._commanderInfo != null;
			this.ShowControlPointStatus = !isInWarmup;
			if (!isInWarmup && this.UseMoraleComparer)
			{
				this.RegisterMoraleEvents();
			}
		}

		public void OnUpdateMorale(BattleSideEnum side, float morale)
		{
			if (this._allyTeam != null && this._allyTeam.Side == side)
			{
				this.AllyMoralePercentage = MathF.Round(MathF.Abs(morale * 100f));
				return;
			}
			if (this._enemyTeam != null && this._enemyTeam.Side == side)
			{
				this.EnemyMoralePercentage = MathF.Round(MathF.Abs(morale * 100f));
			}
		}

		private void OnMissionReset(object sender, PropertyChangedEventArgs e)
		{
			if (this.UseMoraleComparer)
			{
				this.AllyMoralePercentage = 50;
				this.EnemyMoralePercentage = 50;
			}
			if (this.UsePowerComparer)
			{
				this.PowerLevelComparer.Update(1.0, 1.0, 1.0, 1.0);
			}
		}

		internal void Tick(float dt)
		{
			foreach (CapturePointVM capturePointVM in this.AllyControlPoints)
			{
				capturePointVM.Refresh(0f, 0f, 0f);
			}
			foreach (CapturePointVM capturePointVM2 in this.EnemyControlPoints)
			{
				capturePointVM2.Refresh(0f, 0f, 0f);
			}
			foreach (CapturePointVM capturePointVM3 in this.NeutralControlPoints)
			{
				capturePointVM3.Refresh(0f, 0f, 0f);
			}
			if (this._allyTeam != null && this.UsePowerComparer)
			{
				int count = Mission.Current.AttackerTeam.ActiveAgents.Count;
				int count2 = Mission.Current.DefenderTeam.ActiveAgents.Count;
				this.AllyMemberCount = ((this._allyTeam.Side == BattleSideEnum.Attacker) ? count : count2);
				this.EnemyMemberCount = ((this._allyTeam.Side == BattleSideEnum.Attacker) ? count2 : count);
				int num = ((this._allyTeam.Side == BattleSideEnum.Attacker) ? this._attackerTeamInitialMemberCount : this._defenderTeamInitialMemberCount);
				Team allyTeam = this._allyTeam;
				int num2 = ((allyTeam != null && allyTeam.Side == BattleSideEnum.Attacker) ? this._defenderTeamInitialMemberCount : this._attackerTeamInitialMemberCount);
				if (num2 == 0 && num == 0)
				{
					this.PowerLevelComparer.Update(1.0, 1.0, 1.0, 1.0);
					return;
				}
				this.PowerLevelComparer.Update((double)this.EnemyMemberCount, (double)this.AllyMemberCount, (double)num2, (double)num);
			}
		}

		private void OnCapturePointOwnerChanged(FlagCapturePoint target, Team newOwnerTeam)
		{
			CapturePointVM capturePointVM = this.FindCapturePointInLists(target);
			if (capturePointVM != null)
			{
				this.RemoveFlagFromLists(capturePointVM);
				this.HandleAddNewCapturePoint(capturePointVM);
				capturePointVM.OnOwnerChanged(newOwnerTeam);
			}
		}

		private void OnCapturePointRemainingMoraleGainsChanged(int[] remainingMoraleArr)
		{
			foreach (CapturePointVM capturePointVM in this.AllyControlPoints)
			{
				int flagIndex = capturePointVM.Target.FlagIndex;
				if (flagIndex >= 0 && remainingMoraleArr.Length > flagIndex)
				{
					capturePointVM.OnRemainingMoraleChanged(remainingMoraleArr[flagIndex]);
				}
			}
			foreach (CapturePointVM capturePointVM2 in this.EnemyControlPoints)
			{
				int flagIndex2 = capturePointVM2.Target.FlagIndex;
				if (flagIndex2 >= 0 && remainingMoraleArr.Length > flagIndex2)
				{
					capturePointVM2.OnRemainingMoraleChanged(remainingMoraleArr[flagIndex2]);
				}
			}
			foreach (CapturePointVM capturePointVM3 in this.NeutralControlPoints)
			{
				int flagIndex3 = capturePointVM3.Target.FlagIndex;
				if (flagIndex3 >= 0 && remainingMoraleArr.Length > flagIndex3)
				{
					capturePointVM3.OnRemainingMoraleChanged(remainingMoraleArr[flagIndex3]);
				}
			}
		}

		private void OnNumberOfCapturePointsChanged()
		{
			this.ResetCapturePointLists();
			this.InitCapturePoints();
		}

		private void InitCapturePoints()
		{
			if (this._commanderInfo != null)
			{
				NetworkCommunicator myPeer = GameNetwork.MyPeer;
				bool flag;
				if (myPeer == null)
				{
					flag = null != null;
				}
				else
				{
					MissionPeer component = myPeer.GetComponent<MissionPeer>();
					flag = ((component != null) ? component.Team : null) != null;
				}
				if (flag)
				{
					foreach (FlagCapturePoint flagCapturePoint in this._commanderInfo.AllCapturePoints.Where((FlagCapturePoint c) => !c.IsDeactivated).ToArray<FlagCapturePoint>())
					{
						CapturePointVM capturePointVM = new CapturePointVM(flagCapturePoint, TargetIconType.Flag_A + flagCapturePoint.FlagIndex);
						this.HandleAddNewCapturePoint(capturePointVM);
					}
					this.RefreshMoraleIncreaseLevels();
				}
			}
		}

		private void HandleAddNewCapturePoint(CapturePointVM capturePointVM)
		{
			this.RemoveFlagFromLists(capturePointVM);
			if (this._allyTeam == null)
			{
				return;
			}
			Team team = this._commanderInfo.GetFlagOwner(capturePointVM.Target);
			if (team != null && (team.Side == BattleSideEnum.None || team.Side == BattleSideEnum.NumSides))
			{
				team = null;
			}
			capturePointVM.OnOwnerChanged(team);
			bool isDeactivated = capturePointVM.Target.IsDeactivated;
			if ((team == null || team.TeamIndex == -1) && !isDeactivated)
			{
				int num = MathF.Min(this.NeutralControlPoints.Count, capturePointVM.Target.FlagIndex);
				this.NeutralControlPoints.Insert(num, capturePointVM);
			}
			else if (this._allyTeam == team)
			{
				int num2 = MathF.Min(this.AllyControlPoints.Count, capturePointVM.Target.FlagIndex);
				this.AllyControlPoints.Insert(num2, capturePointVM);
			}
			else if (this._allyTeam != team)
			{
				int num3 = MathF.Min(this.EnemyControlPoints.Count, capturePointVM.Target.FlagIndex);
				this.EnemyControlPoints.Insert(num3, capturePointVM);
			}
			else if (team.Side != BattleSideEnum.None)
			{
				Debug.FailedAssert("Incorrect flag team state", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade.ViewModelCollection\\Multiplayer\\HUDExtensions\\CommanderInfoVM.cs", "HandleAddNewCapturePoint", 317);
			}
			this.RefreshMoraleIncreaseLevels();
		}

		private void RefreshMoraleIncreaseLevels()
		{
			this.AllyMoraleIncreaseLevel = MathF.Max(0, this.AllyControlPoints.Count - this.EnemyControlPoints.Count);
			this.EnemyMoraleIncreaseLevel = MathF.Max(0, this.EnemyControlPoints.Count - this.AllyControlPoints.Count);
		}

		private void RemoveFlagFromLists(CapturePointVM capturePoint)
		{
			if (this.AllyControlPoints.Contains(capturePoint))
			{
				this.AllyControlPoints.Remove(capturePoint);
				return;
			}
			if (this.NeutralControlPoints.Contains(capturePoint))
			{
				this.NeutralControlPoints.Remove(capturePoint);
				return;
			}
			if (this.EnemyControlPoints.Contains(capturePoint))
			{
				this.EnemyControlPoints.Remove(capturePoint);
			}
		}

		public void OnTeamChanged()
		{
			if (!GameNetwork.IsMyPeerReady || !this.ShowTacticalInfo)
			{
				return;
			}
			MissionPeer component = GameNetwork.MyPeer.GetComponent<MissionPeer>();
			this._allyTeam = component.Team;
			if (this._allyTeam == null)
			{
				return;
			}
			IEnumerable<Team> enumerable = Mission.Current.Teams.Where((Team t) => t.IsEnemyOf(this._allyTeam));
			this._enemyTeam = enumerable.FirstOrDefault<Team>();
			if (this._allyTeam.Side == BattleSideEnum.None)
			{
				this._allyTeam = Mission.Current.AttackerTeam;
				return;
			}
			this.ResetCapturePointLists();
			this.InitCapturePoints();
		}

		private void ResetCapturePointLists()
		{
			this.AllyControlPoints.Clear();
			this.NeutralControlPoints.Clear();
			this.EnemyControlPoints.Clear();
		}

		private CapturePointVM FindCapturePointInLists(FlagCapturePoint target)
		{
			CapturePointVM capturePointVM = this.AllyControlPoints.SingleOrDefault((CapturePointVM c) => c.Target == target);
			if (capturePointVM != null)
			{
				return capturePointVM;
			}
			CapturePointVM capturePointVM2 = this.EnemyControlPoints.SingleOrDefault((CapturePointVM c) => c.Target == target);
			if (capturePointVM2 != null)
			{
				return capturePointVM2;
			}
			CapturePointVM capturePointVM3 = this.NeutralControlPoints.SingleOrDefault((CapturePointVM c) => c.Target == target);
			if (capturePointVM3 != null)
			{
				return capturePointVM3;
			}
			return null;
		}

		public void RefreshColors(string allyTeamColor, string allyTeamColorSecondary, string enemyTeamColor, string enemyTeamColorSecondary)
		{
			this.AllyTeamColor = allyTeamColor;
			this.AllyTeamColorSecondary = allyTeamColorSecondary;
			this.EnemyTeamColor = enemyTeamColor;
			this.EnemyTeamColorSecondary = enemyTeamColorSecondary;
			if (this.UsePowerComparer)
			{
				this.PowerLevelComparer.SetColors(this.EnemyTeamColor, this.AllyTeamColor);
			}
		}

		[DataSourceProperty]
		public MBBindingList<CapturePointVM> AllyControlPoints
		{
			get
			{
				return this._allyControlPoints;
			}
			set
			{
				if (value != this._allyControlPoints)
				{
					this._allyControlPoints = value;
					base.OnPropertyChangedWithValue<MBBindingList<CapturePointVM>>(value, "AllyControlPoints");
				}
			}
		}

		[DataSourceProperty]
		public MBBindingList<CapturePointVM> NeutralControlPoints
		{
			get
			{
				return this._neutralControlPoints;
			}
			set
			{
				if (value != this._neutralControlPoints)
				{
					this._neutralControlPoints = value;
					base.OnPropertyChangedWithValue<MBBindingList<CapturePointVM>>(value, "NeutralControlPoints");
				}
			}
		}

		[DataSourceProperty]
		public MBBindingList<CapturePointVM> EnemyControlPoints
		{
			get
			{
				return this._enemyControlPoints;
			}
			set
			{
				if (value != this._enemyControlPoints)
				{
					this._enemyControlPoints = value;
					base.OnPropertyChangedWithValue<MBBindingList<CapturePointVM>>(value, "EnemyControlPoints");
				}
			}
		}

		[DataSourceProperty]
		public string AllyTeamColor
		{
			get
			{
				return this._allyTeamColor;
			}
			set
			{
				if (value != this._allyTeamColor)
				{
					this._allyTeamColor = value;
					base.OnPropertyChangedWithValue<string>(value, "AllyTeamColor");
				}
			}
		}

		[DataSourceProperty]
		public string AllyTeamColorSecondary
		{
			get
			{
				return this._allyTeamColorSecondary;
			}
			set
			{
				if (value != this._allyTeamColorSecondary)
				{
					this._allyTeamColorSecondary = value;
					base.OnPropertyChangedWithValue<string>(value, "AllyTeamColorSecondary");
				}
			}
		}

		[DataSourceProperty]
		public string EnemyTeamColor
		{
			get
			{
				return this._enemyTeamColor;
			}
			set
			{
				if (value != this._enemyTeamColor)
				{
					this._enemyTeamColor = value;
					base.OnPropertyChangedWithValue<string>(value, "EnemyTeamColor");
				}
			}
		}

		[DataSourceProperty]
		public string EnemyTeamColorSecondary
		{
			get
			{
				return this._enemyTeamColorSecondary;
			}
			set
			{
				if (value != this._enemyTeamColorSecondary)
				{
					this._enemyTeamColorSecondary = value;
					base.OnPropertyChangedWithValue<string>(value, "EnemyTeamColorSecondary");
				}
			}
		}

		[DataSourceProperty]
		public int AllyMoraleIncreaseLevel
		{
			get
			{
				return this._allyMoraleIncreaseLevel;
			}
			set
			{
				if (value != this._allyMoraleIncreaseLevel)
				{
					this._allyMoraleIncreaseLevel = value;
					base.OnPropertyChangedWithValue(value, "AllyMoraleIncreaseLevel");
				}
			}
		}

		[DataSourceProperty]
		public int EnemyMoraleIncreaseLevel
		{
			get
			{
				return this._enemyMoraleIncreaseLevel;
			}
			set
			{
				if (value != this._enemyMoraleIncreaseLevel)
				{
					this._enemyMoraleIncreaseLevel = value;
					base.OnPropertyChangedWithValue(value, "EnemyMoraleIncreaseLevel");
				}
			}
		}

		[DataSourceProperty]
		public int AllyMoralePercentage
		{
			get
			{
				return this._allyMoralePercentage;
			}
			set
			{
				if (value != this._allyMoralePercentage)
				{
					this._allyMoralePercentage = value;
					base.OnPropertyChangedWithValue(value, "AllyMoralePercentage");
				}
			}
		}

		[DataSourceProperty]
		public int EnemyMoralePercentage
		{
			get
			{
				return this._enemyMoralePercentage;
			}
			set
			{
				if (value != this._enemyMoralePercentage)
				{
					this._enemyMoralePercentage = value;
					base.OnPropertyChangedWithValue(value, "EnemyMoralePercentage");
				}
			}
		}

		[DataSourceProperty]
		public int AllyMemberCount
		{
			get
			{
				return this._allyMemberCount;
			}
			set
			{
				if (value != this._allyMemberCount)
				{
					this._allyMemberCount = value;
					base.OnPropertyChangedWithValue(value, "AllyMemberCount");
				}
			}
		}

		[DataSourceProperty]
		public int EnemyMemberCount
		{
			get
			{
				return this._enemyMemberCount;
			}
			set
			{
				if (value != this._enemyMemberCount)
				{
					this._enemyMemberCount = value;
					base.OnPropertyChangedWithValue(value, "EnemyMemberCount");
				}
			}
		}

		[DataSourceProperty]
		public PowerLevelComparer PowerLevelComparer
		{
			get
			{
				return this._powerLevelComparer;
			}
			set
			{
				if (value != this._powerLevelComparer)
				{
					this._powerLevelComparer = value;
					base.OnPropertyChangedWithValue<PowerLevelComparer>(value, "PowerLevelComparer");
				}
			}
		}

		[DataSourceProperty]
		public bool UsePowerComparer
		{
			get
			{
				return this._usePowerComparer;
			}
			set
			{
				if (value != this._usePowerComparer)
				{
					this._usePowerComparer = value;
					base.OnPropertyChangedWithValue(value, "UsePowerComparer");
				}
			}
		}

		[DataSourceProperty]
		public bool UseMoraleComparer
		{
			get
			{
				return this._useMoraleComparer;
			}
			set
			{
				if (value != this._useMoraleComparer)
				{
					this._useMoraleComparer = value;
					base.OnPropertyChangedWithValue(value, "UseMoraleComparer");
				}
			}
		}

		[DataSourceProperty]
		public bool ShowTacticalInfo
		{
			get
			{
				return this._showTacticalInfo;
			}
			set
			{
				if (value != this._showTacticalInfo)
				{
					this._showTacticalInfo = value;
					base.OnPropertyChangedWithValue(value, "ShowTacticalInfo");
				}
			}
		}

		[DataSourceProperty]
		public bool AreMoralesIndependent
		{
			get
			{
				return this._areMoralesIndependent;
			}
			set
			{
				if (value != this._areMoralesIndependent)
				{
					this._areMoralesIndependent = value;
					base.OnPropertyChangedWithValue(value, "AreMoralesIndependent");
				}
			}
		}

		[DataSourceProperty]
		public bool ShowControlPointStatus
		{
			get
			{
				return this._showControlPointStatus;
			}
			set
			{
				if (value != this._showControlPointStatus)
				{
					this._showControlPointStatus = value;
					base.OnPropertyChangedWithValue(value, "ShowControlPointStatus");
				}
			}
		}

		private readonly MissionRepresentativeBase _missionRepresentative;

		private readonly MissionMultiplayerGameModeBaseClient _gameMode;

		private readonly MissionMultiplayerSiegeClient _siegeClient;

		private readonly MissionScoreboardComponent _missionScoreboardComponent;

		private const float InitialArmyStrength = 1f;

		private int _attackerTeamInitialMemberCount;

		private int _defenderTeamInitialMemberCount;

		private Team _allyTeam;

		private Team _enemyTeam;

		private ICommanderInfo _commanderInfo;

		private bool _areMoraleEventsRegistered;

		private MBBindingList<CapturePointVM> _allyControlPoints;

		private MBBindingList<CapturePointVM> _neutralControlPoints;

		private MBBindingList<CapturePointVM> _enemyControlPoints;

		private int _allyMoraleIncreaseLevel;

		private int _enemyMoraleIncreaseLevel;

		private int _allyMoralePercentage;

		private int _enemyMoralePercentage;

		private int _allyMemberCount;

		private int _enemyMemberCount;

		private PowerLevelComparer _powerLevelComparer;

		private bool _showTacticalInfo;

		private bool _usePowerComparer;

		private bool _useMoraleComparer;

		private bool _areMoralesIndependent;

		private bool _showControlPointStatus;

		private string _allyTeamColor;

		private string _allyTeamColorSecondary;

		private string _enemyTeamColor;

		private string _enemyTeamColorSecondary;
	}
}
