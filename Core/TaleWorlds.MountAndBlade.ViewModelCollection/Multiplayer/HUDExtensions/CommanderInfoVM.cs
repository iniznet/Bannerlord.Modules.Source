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
	// Token: 0x020000B8 RID: 184
	public class CommanderInfoVM : ViewModel
	{
		// Token: 0x0600114D RID: 4429 RVA: 0x00038F04 File Offset: 0x00037104
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

		// Token: 0x0600114E RID: 4430 RVA: 0x00039060 File Offset: 0x00037260
		private void OnRoundStarted()
		{
			this.OnTeamChanged();
			if (this.UsePowerComparer)
			{
				this._attackerTeamInitialMemberCount = this._missionScoreboardComponent.Sides[1].Players.Count<MissionPeer>();
				this._defenderTeamInitialMemberCount = this._missionScoreboardComponent.Sides[0].Players.Count<MissionPeer>();
			}
		}

		// Token: 0x0600114F RID: 4431 RVA: 0x000390B8 File Offset: 0x000372B8
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

		// Token: 0x06001150 RID: 4432 RVA: 0x00039136 File Offset: 0x00037336
		private void OnPreparationEnded()
		{
			this.ShowTacticalInfo = true;
			this.OnTeamChanged();
		}

		// Token: 0x06001151 RID: 4433 RVA: 0x00039148 File Offset: 0x00037348
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

		// Token: 0x06001152 RID: 4434 RVA: 0x000391F3 File Offset: 0x000373F3
		public void UpdateWarmupDependentFlags(bool isInWarmup)
		{
			this.UseMoraleComparer = !isInWarmup && this._gameMode.IsGameModeTactical && this._commanderInfo != null;
			this.ShowControlPointStatus = !isInWarmup;
			if (!isInWarmup && this.UseMoraleComparer)
			{
				this.RegisterMoraleEvents();
			}
		}

		// Token: 0x06001153 RID: 4435 RVA: 0x00039234 File Offset: 0x00037434
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

		// Token: 0x06001154 RID: 4436 RVA: 0x0003929C File Offset: 0x0003749C
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

		// Token: 0x06001155 RID: 4437 RVA: 0x000392F8 File Offset: 0x000374F8
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

		// Token: 0x06001156 RID: 4438 RVA: 0x000394EC File Offset: 0x000376EC
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

		// Token: 0x06001157 RID: 4439 RVA: 0x0003951C File Offset: 0x0003771C
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

		// Token: 0x06001158 RID: 4440 RVA: 0x00039634 File Offset: 0x00037834
		private void OnNumberOfCapturePointsChanged()
		{
			this.ResetCapturePointLists();
			this.InitCapturePoints();
		}

		// Token: 0x06001159 RID: 4441 RVA: 0x00039644 File Offset: 0x00037844
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

		// Token: 0x0600115A RID: 4442 RVA: 0x000396E0 File Offset: 0x000378E0
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

		// Token: 0x0600115B RID: 4443 RVA: 0x00039808 File Offset: 0x00037A08
		private void RefreshMoraleIncreaseLevels()
		{
			this.AllyMoraleIncreaseLevel = MathF.Max(0, this.AllyControlPoints.Count - this.EnemyControlPoints.Count);
			this.EnemyMoraleIncreaseLevel = MathF.Max(0, this.EnemyControlPoints.Count - this.AllyControlPoints.Count);
		}

		// Token: 0x0600115C RID: 4444 RVA: 0x0003985C File Offset: 0x00037A5C
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

		// Token: 0x0600115D RID: 4445 RVA: 0x000398BC File Offset: 0x00037ABC
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

		// Token: 0x0600115E RID: 4446 RVA: 0x0003994C File Offset: 0x00037B4C
		private void ResetCapturePointLists()
		{
			this.AllyControlPoints.Clear();
			this.NeutralControlPoints.Clear();
			this.EnemyControlPoints.Clear();
		}

		// Token: 0x0600115F RID: 4447 RVA: 0x00039970 File Offset: 0x00037B70
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

		// Token: 0x06001160 RID: 4448 RVA: 0x000399E2 File Offset: 0x00037BE2
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

		// Token: 0x17000599 RID: 1433
		// (get) Token: 0x06001161 RID: 4449 RVA: 0x00039A20 File Offset: 0x00037C20
		// (set) Token: 0x06001162 RID: 4450 RVA: 0x00039A28 File Offset: 0x00037C28
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

		// Token: 0x1700059A RID: 1434
		// (get) Token: 0x06001163 RID: 4451 RVA: 0x00039A46 File Offset: 0x00037C46
		// (set) Token: 0x06001164 RID: 4452 RVA: 0x00039A4E File Offset: 0x00037C4E
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

		// Token: 0x1700059B RID: 1435
		// (get) Token: 0x06001165 RID: 4453 RVA: 0x00039A6C File Offset: 0x00037C6C
		// (set) Token: 0x06001166 RID: 4454 RVA: 0x00039A74 File Offset: 0x00037C74
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

		// Token: 0x1700059C RID: 1436
		// (get) Token: 0x06001167 RID: 4455 RVA: 0x00039A92 File Offset: 0x00037C92
		// (set) Token: 0x06001168 RID: 4456 RVA: 0x00039A9A File Offset: 0x00037C9A
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

		// Token: 0x1700059D RID: 1437
		// (get) Token: 0x06001169 RID: 4457 RVA: 0x00039ABD File Offset: 0x00037CBD
		// (set) Token: 0x0600116A RID: 4458 RVA: 0x00039AC5 File Offset: 0x00037CC5
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

		// Token: 0x1700059E RID: 1438
		// (get) Token: 0x0600116B RID: 4459 RVA: 0x00039AE8 File Offset: 0x00037CE8
		// (set) Token: 0x0600116C RID: 4460 RVA: 0x00039AF0 File Offset: 0x00037CF0
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

		// Token: 0x1700059F RID: 1439
		// (get) Token: 0x0600116D RID: 4461 RVA: 0x00039B13 File Offset: 0x00037D13
		// (set) Token: 0x0600116E RID: 4462 RVA: 0x00039B1B File Offset: 0x00037D1B
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

		// Token: 0x170005A0 RID: 1440
		// (get) Token: 0x0600116F RID: 4463 RVA: 0x00039B3E File Offset: 0x00037D3E
		// (set) Token: 0x06001170 RID: 4464 RVA: 0x00039B46 File Offset: 0x00037D46
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

		// Token: 0x170005A1 RID: 1441
		// (get) Token: 0x06001171 RID: 4465 RVA: 0x00039B64 File Offset: 0x00037D64
		// (set) Token: 0x06001172 RID: 4466 RVA: 0x00039B6C File Offset: 0x00037D6C
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

		// Token: 0x170005A2 RID: 1442
		// (get) Token: 0x06001173 RID: 4467 RVA: 0x00039B8A File Offset: 0x00037D8A
		// (set) Token: 0x06001174 RID: 4468 RVA: 0x00039B92 File Offset: 0x00037D92
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

		// Token: 0x170005A3 RID: 1443
		// (get) Token: 0x06001175 RID: 4469 RVA: 0x00039BB0 File Offset: 0x00037DB0
		// (set) Token: 0x06001176 RID: 4470 RVA: 0x00039BB8 File Offset: 0x00037DB8
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

		// Token: 0x170005A4 RID: 1444
		// (get) Token: 0x06001177 RID: 4471 RVA: 0x00039BD6 File Offset: 0x00037DD6
		// (set) Token: 0x06001178 RID: 4472 RVA: 0x00039BDE File Offset: 0x00037DDE
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

		// Token: 0x170005A5 RID: 1445
		// (get) Token: 0x06001179 RID: 4473 RVA: 0x00039BFC File Offset: 0x00037DFC
		// (set) Token: 0x0600117A RID: 4474 RVA: 0x00039C04 File Offset: 0x00037E04
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

		// Token: 0x170005A6 RID: 1446
		// (get) Token: 0x0600117B RID: 4475 RVA: 0x00039C22 File Offset: 0x00037E22
		// (set) Token: 0x0600117C RID: 4476 RVA: 0x00039C2A File Offset: 0x00037E2A
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

		// Token: 0x170005A7 RID: 1447
		// (get) Token: 0x0600117D RID: 4477 RVA: 0x00039C48 File Offset: 0x00037E48
		// (set) Token: 0x0600117E RID: 4478 RVA: 0x00039C50 File Offset: 0x00037E50
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

		// Token: 0x170005A8 RID: 1448
		// (get) Token: 0x0600117F RID: 4479 RVA: 0x00039C6E File Offset: 0x00037E6E
		// (set) Token: 0x06001180 RID: 4480 RVA: 0x00039C76 File Offset: 0x00037E76
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

		// Token: 0x170005A9 RID: 1449
		// (get) Token: 0x06001181 RID: 4481 RVA: 0x00039C94 File Offset: 0x00037E94
		// (set) Token: 0x06001182 RID: 4482 RVA: 0x00039C9C File Offset: 0x00037E9C
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

		// Token: 0x170005AA RID: 1450
		// (get) Token: 0x06001183 RID: 4483 RVA: 0x00039CBA File Offset: 0x00037EBA
		// (set) Token: 0x06001184 RID: 4484 RVA: 0x00039CC2 File Offset: 0x00037EC2
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

		// Token: 0x170005AB RID: 1451
		// (get) Token: 0x06001185 RID: 4485 RVA: 0x00039CE0 File Offset: 0x00037EE0
		// (set) Token: 0x06001186 RID: 4486 RVA: 0x00039CE8 File Offset: 0x00037EE8
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

		// Token: 0x0400083B RID: 2107
		private readonly MissionRepresentativeBase _missionRepresentative;

		// Token: 0x0400083C RID: 2108
		private readonly MissionMultiplayerGameModeBaseClient _gameMode;

		// Token: 0x0400083D RID: 2109
		private readonly MissionMultiplayerSiegeClient _siegeClient;

		// Token: 0x0400083E RID: 2110
		private readonly MissionScoreboardComponent _missionScoreboardComponent;

		// Token: 0x0400083F RID: 2111
		private const float InitialArmyStrength = 1f;

		// Token: 0x04000840 RID: 2112
		private int _attackerTeamInitialMemberCount;

		// Token: 0x04000841 RID: 2113
		private int _defenderTeamInitialMemberCount;

		// Token: 0x04000842 RID: 2114
		private Team _allyTeam;

		// Token: 0x04000843 RID: 2115
		private Team _enemyTeam;

		// Token: 0x04000844 RID: 2116
		private ICommanderInfo _commanderInfo;

		// Token: 0x04000845 RID: 2117
		private bool _areMoraleEventsRegistered;

		// Token: 0x04000846 RID: 2118
		private MBBindingList<CapturePointVM> _allyControlPoints;

		// Token: 0x04000847 RID: 2119
		private MBBindingList<CapturePointVM> _neutralControlPoints;

		// Token: 0x04000848 RID: 2120
		private MBBindingList<CapturePointVM> _enemyControlPoints;

		// Token: 0x04000849 RID: 2121
		private int _allyMoraleIncreaseLevel;

		// Token: 0x0400084A RID: 2122
		private int _enemyMoraleIncreaseLevel;

		// Token: 0x0400084B RID: 2123
		private int _allyMoralePercentage;

		// Token: 0x0400084C RID: 2124
		private int _enemyMoralePercentage;

		// Token: 0x0400084D RID: 2125
		private int _allyMemberCount;

		// Token: 0x0400084E RID: 2126
		private int _enemyMemberCount;

		// Token: 0x0400084F RID: 2127
		private PowerLevelComparer _powerLevelComparer;

		// Token: 0x04000850 RID: 2128
		private bool _showTacticalInfo;

		// Token: 0x04000851 RID: 2129
		private bool _usePowerComparer;

		// Token: 0x04000852 RID: 2130
		private bool _useMoraleComparer;

		// Token: 0x04000853 RID: 2131
		private bool _areMoralesIndependent;

		// Token: 0x04000854 RID: 2132
		private bool _showControlPointStatus;

		// Token: 0x04000855 RID: 2133
		private string _allyTeamColor;

		// Token: 0x04000856 RID: 2134
		private string _allyTeamColorSecondary;

		// Token: 0x04000857 RID: 2135
		private string _enemyTeamColor;

		// Token: 0x04000858 RID: 2136
		private string _enemyTeamColorSecondary;
	}
}
