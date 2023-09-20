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
	public abstract class TeamAIComponent
	{
		public MBReadOnlyList<StrategicArea> StrategicAreas
		{
			get
			{
				return this._strategicAreas;
			}
		}

		public bool HasStrategicAreas
		{
			get
			{
				return !this._strategicAreas.IsEmpty<StrategicArea>();
			}
		}

		public bool IsDefenseApplicable { get; private set; }

		public bool GetIsFirstTacticChosen { get; private set; }

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

		public void AddStrategicArea(StrategicArea strategicArea)
		{
			this._strategicAreas.Add(strategicArea);
		}

		public void RemoveStrategicArea(StrategicArea strategicArea)
		{
			if (this.Team.DetachmentManager.ContainsDetachment(strategicArea))
			{
				this.Team.DetachmentManager.DestroyDetachment(strategicArea);
			}
			this._strategicAreas.Remove(strategicArea);
		}

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

		public void AddTacticOption(TacticComponent tacticOption)
		{
			this._availableTactics.Add(tacticOption);
		}

		public void RemoveTacticOption(Type tacticType)
		{
			this._availableTactics.RemoveAll((TacticComponent at) => tacticType == at.GetType());
		}

		public void ClearTacticOptions()
		{
			this._availableTactics.Clear();
		}

		[Conditional("DEBUG")]
		public void AssertTeam(Team team)
		{
		}

		public void NotifyTacticalDecision(in TacticalDecision decision)
		{
			TeamAIComponent.TacticalDecisionDelegate onNotifyTacticalDecision = this.OnNotifyTacticalDecision;
			if (onNotifyTacticalDecision == null)
			{
				return;
			}
			onNotifyTacticalDecision(decision);
		}

		public virtual void OnDeploymentFinished()
		{
		}

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

		public void OnTacticAppliedForFirstTime()
		{
			this.GetIsFirstTacticChosen = false;
		}

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

		public void TickOccasionally()
		{
			if (Mission.Current.AllowAiTicking && this.Team.HasBots)
			{
				this.CurrentTactic.TickOccasionally();
			}
		}

		public bool IsCurrentTactic(TacticComponent tactic)
		{
			return tactic == this.CurrentTactic;
		}

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

		public abstract void OnUnitAddedToFormationForTheFirstTime(Formation formation);

		protected internal virtual void CreateMissionSpecificBehaviors()
		{
		}

		protected internal virtual void InitializeDetachments(Mission mission)
		{
			DeploymentHandler missionBehavior = this.Mission.GetMissionBehavior<DeploymentHandler>();
			if (missionBehavior == null)
			{
				return;
			}
			missionBehavior.InitializeDeploymentPoints();
		}

		public TeamAIComponent.TacticalDecisionDelegate OnNotifyTacticalDecision;

		public const int BattleTokenForceSize = 10;

		private readonly List<TacticComponent> _availableTactics;

		private static bool _retreatScriptActive;

		protected readonly Mission Mission;

		protected readonly Team Team;

		private readonly Timer _thinkTimer;

		private readonly Timer _applyTimer;

		private TacticComponent _currentTactic;

		public List<TacticalPosition> TacticalPositions;

		public List<TacticalRegion> TacticalRegions;

		private readonly MBList<StrategicArea> _strategicAreas;

		private readonly float _occasionalTickTime;

		private MissionTime _nextTacticChooseTime;

		private MissionTime _nextOccasionalTickTime;

		protected class TacticOption
		{
			public string Id { get; private set; }

			public Lazy<TacticComponent> Tactic { get; private set; }

			public float Weight { get; set; }

			public TacticOption(string id, Lazy<TacticComponent> tactic, float weight)
			{
				this.Id = id;
				this.Tactic = tactic;
				this.Weight = weight;
			}
		}

		public delegate void TacticalDecisionDelegate(in TacticalDecision decision);
	}
}
