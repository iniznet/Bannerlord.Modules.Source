using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000171 RID: 369
	public abstract class TeamAIComponent
	{
		// Token: 0x1700042C RID: 1068
		// (get) Token: 0x0600131B RID: 4891 RVA: 0x0004A11C File Offset: 0x0004831C
		public MBReadOnlyList<StrategicArea> StrategicAreas
		{
			get
			{
				return this._strategicAreas;
			}
		}

		// Token: 0x1700042D RID: 1069
		// (get) Token: 0x0600131C RID: 4892 RVA: 0x0004A124 File Offset: 0x00048324
		public bool HasStrategicAreas
		{
			get
			{
				return !this._strategicAreas.IsEmpty<StrategicArea>();
			}
		}

		// Token: 0x1700042E RID: 1070
		// (get) Token: 0x0600131D RID: 4893 RVA: 0x0004A134 File Offset: 0x00048334
		// (set) Token: 0x0600131E RID: 4894 RVA: 0x0004A13C File Offset: 0x0004833C
		public bool IsDefenseApplicable { get; private set; }

		// Token: 0x1700042F RID: 1071
		// (get) Token: 0x0600131F RID: 4895 RVA: 0x0004A145 File Offset: 0x00048345
		// (set) Token: 0x06001320 RID: 4896 RVA: 0x0004A14D File Offset: 0x0004834D
		public bool GetIsFirstTacticChosen { get; private set; }

		// Token: 0x17000430 RID: 1072
		// (get) Token: 0x06001321 RID: 4897 RVA: 0x0004A156 File Offset: 0x00048356
		// (set) Token: 0x06001322 RID: 4898 RVA: 0x0004A15E File Offset: 0x0004835E
		private protected TacticComponent CurrentTactic
		{
			protected get
			{
				return this._currentTactic;
			}
			private set
			{
				TacticComponent currentTactic = this._currentTactic;
				if (currentTactic != null)
				{
					currentTactic.OnCancel();
				}
				this._currentTactic = value;
				if (this._currentTactic != null)
				{
					this._currentTactic.OnApply();
					this._currentTactic.TickOccasionally();
				}
			}
		}

		// Token: 0x06001323 RID: 4899 RVA: 0x0004A198 File Offset: 0x00048398
		protected TeamAIComponent(Mission currentMission, Team currentTeam, float thinkTimerTime, float applyTimerTime)
		{
			this.Mission = currentMission;
			this.Team = currentTeam;
			this._thinkTimer = new Timer(this.Mission.CurrentTime, thinkTimerTime, true);
			this._applyTimer = new Timer(this.Mission.CurrentTime, applyTimerTime, true);
			this._occasionalTickTime = applyTimerTime;
			this._availableTactics = new List<TacticComponent>();
			this.TacticalPositions = currentMission.ActiveMissionObjects.FindAllWithType<TacticalPosition>().ToList<TacticalPosition>();
			this.TacticalRegions = currentMission.ActiveMissionObjects.FindAllWithType<TacticalRegion>().ToList<TacticalRegion>();
			this._strategicAreas = (from amo in currentMission.ActiveMissionObjects.Where(delegate(MissionObject amo)
				{
					StrategicArea strategicArea;
					return (strategicArea = amo as StrategicArea) != null && strategicArea.IsActive && strategicArea.IsUsableBy(this.Team.Side);
				})
				select amo as StrategicArea).ToMBList<StrategicArea>();
		}

		// Token: 0x06001324 RID: 4900 RVA: 0x0004A26F File Offset: 0x0004846F
		public void AddStrategicArea(StrategicArea strategicArea)
		{
			this._strategicAreas.Add(strategicArea);
		}

		// Token: 0x06001325 RID: 4901 RVA: 0x0004A27D File Offset: 0x0004847D
		public void RemoveStrategicArea(StrategicArea strategicArea)
		{
			if (this.Team.DetachmentManager.ContainsDetachment(strategicArea))
			{
				this.Team.DetachmentManager.DestroyDetachment(strategicArea);
			}
			this._strategicAreas.Remove(strategicArea);
		}

		// Token: 0x06001326 RID: 4902 RVA: 0x0004A2B0 File Offset: 0x000484B0
		public void RemoveAllStrategicAreas()
		{
			foreach (StrategicArea strategicArea in this._strategicAreas)
			{
				if (this.Team.DetachmentManager.ContainsDetachment(strategicArea))
				{
					this.Team.DetachmentManager.DestroyDetachment(strategicArea);
				}
			}
			this._strategicAreas.Clear();
		}

		// Token: 0x06001327 RID: 4903 RVA: 0x0004A32C File Offset: 0x0004852C
		public void AddTacticOption(TacticComponent tacticOption)
		{
			this._availableTactics.Add(tacticOption);
		}

		// Token: 0x06001328 RID: 4904 RVA: 0x0004A33C File Offset: 0x0004853C
		public void RemoveTacticOption(Type tacticType)
		{
			this._availableTactics.RemoveAll((TacticComponent at) => tacticType == at.GetType());
		}

		// Token: 0x06001329 RID: 4905 RVA: 0x0004A36E File Offset: 0x0004856E
		public void ClearTacticOptions()
		{
			this._availableTactics.Clear();
		}

		// Token: 0x0600132A RID: 4906 RVA: 0x0004A37B File Offset: 0x0004857B
		[Conditional("DEBUG")]
		public void AssertTeam(Team team)
		{
		}

		// Token: 0x0600132B RID: 4907 RVA: 0x0004A37D File Offset: 0x0004857D
		public void NotifyTacticalDecision(in TacticalDecision decision)
		{
			TeamAIComponent.TacticalDecisionDelegate onNotifyTacticalDecision = this.OnNotifyTacticalDecision;
			if (onNotifyTacticalDecision == null)
			{
				return;
			}
			onNotifyTacticalDecision(decision);
		}

		// Token: 0x0600132C RID: 4908 RVA: 0x0004A390 File Offset: 0x00048590
		public virtual void OnDeploymentFinished()
		{
		}

		// Token: 0x0600132D RID: 4909 RVA: 0x0004A394 File Offset: 0x00048594
		public void OnMissionEnded()
		{
			MBDebug.Print("Mission end received by teamAI", 0, Debug.DebugColor.White, 17592186044416UL);
			foreach (Formation formation in this.Team.FormationsIncludingSpecialAndEmpty)
			{
				if (formation.CountOfUnits > 0)
				{
					foreach (UsableMachine usableMachine in formation.GetUsedMachines().ToList<UsableMachine>())
					{
						formation.StopUsingMachine(usableMachine, false);
					}
				}
			}
		}

		// Token: 0x0600132E RID: 4910 RVA: 0x0004A44C File Offset: 0x0004864C
		public void ResetTactic(bool keepCurrentTactic = true)
		{
			if (!keepCurrentTactic)
			{
				this.CurrentTactic = null;
			}
			this._thinkTimer.Reset(this.Mission.CurrentTime);
			this._applyTimer.Reset(this.Mission.CurrentTime);
			this.MakeDecision();
			this.TickOccasionally();
		}

		// Token: 0x0600132F RID: 4911 RVA: 0x0004A49C File Offset: 0x0004869C
		protected internal virtual void Tick(float dt)
		{
			if (this.Team.BodyGuardFormation != null && this.Team.BodyGuardFormation.CountOfUnits > 0 && (this.Team.GeneralsFormation == null || this.Team.GeneralsFormation.CountOfUnits == 0))
			{
				this.Team.BodyGuardFormation.AI.ResetBehaviorWeights();
				this.Team.BodyGuardFormation.AI.SetBehaviorWeight<BehaviorCharge>(1f);
			}
			if (this._nextTacticChooseTime.IsPast)
			{
				this.MakeDecision();
				this._nextTacticChooseTime = MissionTime.SecondsFromNow(5f);
			}
			if (this._nextOccasionalTickTime.IsPast)
			{
				this.TickOccasionally();
				this._nextOccasionalTickTime = MissionTime.SecondsFromNow(this._occasionalTickTime);
			}
		}

		// Token: 0x06001330 RID: 4912 RVA: 0x0004A560 File Offset: 0x00048760
		public void CheckIsDefenseApplicable()
		{
			if (this.Team.Side != BattleSideEnum.Defender)
			{
				this.IsDefenseApplicable = false;
				return;
			}
			int memberCount = this.Team.QuerySystem.MemberCount;
			float maxUnderRangedAttackRatio = this.Team.QuerySystem.MaxUnderRangedAttackRatio;
			float num = (float)memberCount * maxUnderRangedAttackRatio;
			int deathByRangedCount = this.Team.QuerySystem.DeathByRangedCount;
			int deathCount = this.Team.QuerySystem.DeathCount;
			float num2 = MBMath.ClampFloat((num + (float)deathByRangedCount) / (float)(memberCount + deathCount), 0.05f, 1f);
			int enemyUnitCount = this.Team.QuerySystem.EnemyUnitCount;
			float num3 = 0f;
			int num4 = 0;
			int num5 = 0;
			foreach (Team team in this.Mission.Teams)
			{
				if (this.Team.IsEnemyOf(team))
				{
					TeamQuerySystem querySystem = team.QuerySystem;
					num4 += querySystem.DeathByRangedCount;
					num5 += querySystem.DeathCount;
					num3 += ((enemyUnitCount == 0) ? 0f : (querySystem.MaxUnderRangedAttackRatio * ((float)querySystem.MemberCount / (float)((enemyUnitCount > 0) ? enemyUnitCount : 1))));
				}
			}
			float num6 = (float)enemyUnitCount * num3;
			int num7 = enemyUnitCount + num5;
			float num8 = MBMath.ClampFloat((num6 + (float)num4) / (float)((num7 > 0) ? num7 : 1), 0.05f, 1f);
			float num9 = MathF.Pow(num2 / num8, 3f * (this.Team.QuerySystem.EnemyRangedRatio + this.Team.QuerySystem.EnemyRangedCavalryRatio));
			this.IsDefenseApplicable = num9 <= 1.5f;
		}

		// Token: 0x06001331 RID: 4913 RVA: 0x0004A718 File Offset: 0x00048918
		public void OnTacticAppliedForFirstTime()
		{
			this.GetIsFirstTacticChosen = false;
		}

		// Token: 0x06001332 RID: 4914 RVA: 0x0004A724 File Offset: 0x00048924
		private void MakeDecision()
		{
			List<TacticComponent> availableTactics = this._availableTactics;
			if ((this.Mission.CurrentState != Mission.State.Continuing && availableTactics.Count == 0) || !this.Team.HasAnyFormationsIncludingSpecialThatIsNotEmpty())
			{
				return;
			}
			bool flag = true;
			foreach (Team team in this.Mission.Teams)
			{
				if (team.IsEnemyOf(this.Team) && team.HasAnyFormationsIncludingSpecialThatIsNotEmpty())
				{
					flag = false;
					break;
				}
			}
			if (flag)
			{
				if (this.Mission.MissionEnded)
				{
					return;
				}
				if (!(this.CurrentTactic is TacticCharge))
				{
					foreach (TacticComponent tacticComponent in availableTactics)
					{
						if (tacticComponent is TacticCharge)
						{
							if (this.CurrentTactic == null)
							{
								this.GetIsFirstTacticChosen = true;
							}
							this.CurrentTactic = tacticComponent;
							break;
						}
					}
					if (!(this.CurrentTactic is TacticCharge))
					{
						if (this.CurrentTactic == null)
						{
							this.GetIsFirstTacticChosen = true;
						}
						this.CurrentTactic = availableTactics.FirstOrDefault<TacticComponent>();
					}
				}
			}
			this.CheckIsDefenseApplicable();
			TacticComponent tacticComponent2 = availableTactics.MaxBy((TacticComponent to) => to.GetTacticWeight() * ((to == this._currentTactic) ? 1.5f : 1f));
			bool flag2 = false;
			if (this.CurrentTactic == null)
			{
				flag2 = true;
			}
			else if (this.CurrentTactic != tacticComponent2)
			{
				if (!this.CurrentTactic.ResetTacticalPositions())
				{
					flag2 = true;
				}
				else
				{
					float tacticWeight = tacticComponent2.GetTacticWeight();
					float num = this.CurrentTactic.GetTacticWeight() * 1.5f;
					if (tacticWeight > num)
					{
						flag2 = true;
					}
				}
			}
			if (flag2)
			{
				if (this.CurrentTactic == null)
				{
					this.GetIsFirstTacticChosen = true;
				}
				this.CurrentTactic = tacticComponent2;
				if (Mission.Current.MainAgent != null && this.Team.GeneralAgent != null && this.Team.IsPlayerTeam && this.Team.IsPlayerSergeant)
				{
					string name = tacticComponent2.GetType().Name;
					MBInformationManager.AddQuickInformation(GameTexts.FindText("str_team_ai_tactic_text", name), 4000, this.Team.GeneralAgent.Character, "");
				}
			}
		}

		// Token: 0x06001333 RID: 4915 RVA: 0x0004A94C File Offset: 0x00048B4C
		public void TickOccasionally()
		{
			if (Mission.Current.AllowAiTicking && this.Team.HasBots)
			{
				this.CurrentTactic.TickOccasionally();
			}
		}

		// Token: 0x06001334 RID: 4916 RVA: 0x0004A972 File Offset: 0x00048B72
		public bool IsCurrentTactic(TacticComponent tactic)
		{
			return tactic == this.CurrentTactic;
		}

		// Token: 0x06001335 RID: 4917 RVA: 0x0004A980 File Offset: 0x00048B80
		[Conditional("DEBUG")]
		protected virtual void DebugTick(float dt)
		{
			if (!MBDebug.IsDisplayingHighLevelAI)
			{
				return;
			}
			TacticComponent currentTactic = this.CurrentTactic;
			if (Input.DebugInput.IsHotKeyPressed("UsableMachineAiBaseHotkeyRetreatScriptActive"))
			{
				TeamAIComponent._retreatScriptActive = true;
			}
			else if (Input.DebugInput.IsHotKeyPressed("UsableMachineAiBaseHotkeyRetreatScriptPassive"))
			{
				TeamAIComponent._retreatScriptActive = false;
			}
			bool retreatScriptActive = TeamAIComponent._retreatScriptActive;
		}

		// Token: 0x06001336 RID: 4918
		public abstract void OnUnitAddedToFormationForTheFirstTime(Formation formation);

		// Token: 0x06001337 RID: 4919 RVA: 0x0004A9D2 File Offset: 0x00048BD2
		protected internal virtual void CreateMissionSpecificBehaviors()
		{
		}

		// Token: 0x06001338 RID: 4920 RVA: 0x0004A9D4 File Offset: 0x00048BD4
		protected internal virtual void InitializeDetachments(Mission mission)
		{
			DeploymentHandler missionBehavior = this.Mission.GetMissionBehavior<DeploymentHandler>();
			if (missionBehavior == null)
			{
				return;
			}
			missionBehavior.InitializeDeploymentPoints();
		}

		// Token: 0x04000565 RID: 1381
		public TeamAIComponent.TacticalDecisionDelegate OnNotifyTacticalDecision;

		// Token: 0x04000566 RID: 1382
		public const int BattleTokenForceSize = 10;

		// Token: 0x04000567 RID: 1383
		private readonly List<TacticComponent> _availableTactics;

		// Token: 0x04000568 RID: 1384
		private static bool _retreatScriptActive;

		// Token: 0x04000569 RID: 1385
		protected readonly Mission Mission;

		// Token: 0x0400056A RID: 1386
		protected readonly Team Team;

		// Token: 0x0400056B RID: 1387
		private readonly Timer _thinkTimer;

		// Token: 0x0400056C RID: 1388
		private readonly Timer _applyTimer;

		// Token: 0x0400056D RID: 1389
		private TacticComponent _currentTactic;

		// Token: 0x0400056E RID: 1390
		public List<TacticalPosition> TacticalPositions;

		// Token: 0x0400056F RID: 1391
		public List<TacticalRegion> TacticalRegions;

		// Token: 0x04000570 RID: 1392
		private readonly MBList<StrategicArea> _strategicAreas;

		// Token: 0x04000571 RID: 1393
		private readonly float _occasionalTickTime;

		// Token: 0x04000572 RID: 1394
		private MissionTime _nextTacticChooseTime;

		// Token: 0x04000573 RID: 1395
		private MissionTime _nextOccasionalTickTime;

		// Token: 0x020004EA RID: 1258
		protected class TacticOption
		{
			// Token: 0x17000958 RID: 2392
			// (get) Token: 0x060038B9 RID: 14521 RVA: 0x000E6B92 File Offset: 0x000E4D92
			// (set) Token: 0x060038BA RID: 14522 RVA: 0x000E6B9A File Offset: 0x000E4D9A
			public string Id { get; private set; }

			// Token: 0x17000959 RID: 2393
			// (get) Token: 0x060038BB RID: 14523 RVA: 0x000E6BA3 File Offset: 0x000E4DA3
			// (set) Token: 0x060038BC RID: 14524 RVA: 0x000E6BAB File Offset: 0x000E4DAB
			public Lazy<TacticComponent> Tactic { get; private set; }

			// Token: 0x1700095A RID: 2394
			// (get) Token: 0x060038BD RID: 14525 RVA: 0x000E6BB4 File Offset: 0x000E4DB4
			// (set) Token: 0x060038BE RID: 14526 RVA: 0x000E6BBC File Offset: 0x000E4DBC
			public float Weight { get; set; }

			// Token: 0x060038BF RID: 14527 RVA: 0x000E6BC5 File Offset: 0x000E4DC5
			public TacticOption(string id, Lazy<TacticComponent> tactic, float weight)
			{
				this.Id = id;
				this.Tactic = tactic;
				this.Weight = weight;
			}
		}

		// Token: 0x020004EB RID: 1259
		// (Invoke) Token: 0x060038C1 RID: 14529
		public delegate void TacticalDecisionDelegate(in TacticalDecision decision);
	}
}
