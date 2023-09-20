using System;
using System.Linq;
using SandBox.Missions.MissionLogics;
using SandBox.Objects.AnimationPoints;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace SandBox.Missions.AgentBehaviors
{
	public class WalkingBehavior : AgentBehavior
	{
		private bool CanWander
		{
			get
			{
				return (this._isIndoor && this._indoorWanderingIsActive) || (!this._isIndoor && this._outdoorWanderingIsActive);
			}
		}

		public WalkingBehavior(AgentBehaviorGroup behaviorGroup)
			: base(behaviorGroup)
		{
			this._missionAgentHandler = base.Mission.GetMissionBehavior<MissionAgentHandler>();
			this._wanderTarget = null;
			this._isIndoor = CampaignMission.Current.Location.IsIndoor;
			this._indoorWanderingIsActive = true;
			this._outdoorWanderingIsActive = true;
			this._wasSimulation = false;
		}

		public void SetIndoorWandering(bool isActive)
		{
			this._indoorWanderingIsActive = isActive;
		}

		public void SetOutdoorWandering(bool isActive)
		{
			this._outdoorWanderingIsActive = isActive;
		}

		public override void Tick(float dt, bool isSimulation)
		{
			if (this._wanderTarget == null || base.Navigator.TargetUsableMachine == null || this._wanderTarget.IsDisabled || !this._wanderTarget.IsStandingPointAvailableForAgent(base.OwnerAgent))
			{
				this._wanderTarget = this.FindTarget();
				this._lastTarget = this._wanderTarget;
			}
			else if (base.Navigator.GetDistanceToTarget(this._wanderTarget) < 5f)
			{
				bool flag = this._wasSimulation && !isSimulation && this._wanderTarget != null && this._waitTimer != null && MBRandom.RandomFloat < (this._isIndoor ? 0f : (Settlement.CurrentSettlement.IsVillage ? 0.6f : 0.1f));
				if (this._waitTimer == null)
				{
					if (!this._wanderTarget.GameEntity.HasTag("npc_idle"))
					{
						AnimationPoint animationPoint = base.OwnerAgent.CurrentlyUsedGameObject as AnimationPoint;
						float num = ((animationPoint != null) ? animationPoint.GetRandomWaitInSeconds() : 10f);
						this._waitTimer = new Timer(base.Mission.CurrentTime, (num < 0f) ? 2.1474836E+09f : num, true);
					}
				}
				else if (this._waitTimer.Check(base.Mission.CurrentTime) || flag)
				{
					if (this.CanWander)
					{
						this._waitTimer = null;
						UsableMachine usableMachine = this.FindTarget();
						if (usableMachine == null || this.IsChildrenOfSameParent(usableMachine, this._wanderTarget))
						{
							AnimationPoint animationPoint2 = base.OwnerAgent.CurrentlyUsedGameObject as AnimationPoint;
							float num2 = ((animationPoint2 != null) ? animationPoint2.GetRandomWaitInSeconds() : 10f);
							this._waitTimer = new Timer(base.Mission.CurrentTime, num2, true);
						}
						else
						{
							this._lastTarget = this._wanderTarget;
							this._wanderTarget = usableMachine;
						}
					}
					else
					{
						this._waitTimer.Reset(100f);
					}
				}
			}
			if (base.OwnerAgent.CurrentlyUsedGameObject != null && base.Navigator.GetDistanceToTarget(this._lastTarget) > 1f)
			{
				base.Navigator.SetTarget(this._lastTarget, this._lastTarget == this._wanderTarget);
			}
			base.Navigator.SetTarget(this._wanderTarget, false);
			this._wasSimulation = isSimulation;
		}

		private bool IsChildrenOfSameParent(UsableMachine machine, UsableMachine otherMachine)
		{
			GameEntity gameEntity = machine.GameEntity;
			while (gameEntity.Parent != null)
			{
				gameEntity = gameEntity.Parent;
			}
			GameEntity gameEntity2 = otherMachine.GameEntity;
			while (gameEntity2.Parent != null)
			{
				gameEntity2 = gameEntity2.Parent;
			}
			return gameEntity == gameEntity2;
		}

		public override void ConversationTick()
		{
			if (this._waitTimer != null)
			{
				this._waitTimer.Reset(base.Mission.CurrentTime);
			}
		}

		public override float GetAvailability(bool isSimulation)
		{
			if (this.FindTarget() == null)
			{
				return 0f;
			}
			return 1f;
		}

		public override void SetCustomWanderTarget(UsableMachine customUsableMachine)
		{
			this._wanderTarget = customUsableMachine;
			if (this._waitTimer != null)
			{
				this._waitTimer = null;
			}
		}

		private UsableMachine FindRandomWalkingTarget(bool forWaiting)
		{
			if (forWaiting && (this._wanderTarget ?? base.Navigator.TargetUsableMachine) != null)
			{
				return null;
			}
			string text = base.OwnerAgent.GetComponent<CampaignAgentComponent>().AgentNavigator.SpecialTargetTag;
			if (text == null)
			{
				text = "npc_common";
			}
			else if (!this._missionAgentHandler.GetAllSpawnTags().Contains(text))
			{
				text = "npc_common_limited";
			}
			return this._missionAgentHandler.FindUnusedPointWithTagForAgent(base.OwnerAgent, text);
		}

		private UsableMachine FindTarget()
		{
			return this.FindRandomWalkingTarget(this._isIndoor && !this._indoorWanderingIsActive);
		}

		private float GetTargetScore(UsableMachine usableMachine)
		{
			if (base.OwnerAgent.GetComponent<CampaignAgentComponent>().AgentNavigator.SpecialTargetTag != null && !usableMachine.GameEntity.HasTag(base.OwnerAgent.GetComponent<CampaignAgentComponent>().AgentNavigator.SpecialTargetTag))
			{
				return 0f;
			}
			StandingPoint vacantStandingPointForAI = usableMachine.GetVacantStandingPointForAI(base.OwnerAgent);
			if (vacantStandingPointForAI == null || vacantStandingPointForAI.IsDisabledForAgent(base.OwnerAgent))
			{
				return 0f;
			}
			float num = 1f;
			Vec3 vec = vacantStandingPointForAI.GetUserFrameForAgent(base.OwnerAgent).Origin.GetGroundVec3() - base.OwnerAgent.Position;
			if (vec.Length < 2f)
			{
				num *= vec.Length / 2f;
			}
			return num * (0.8f + MBRandom.RandomFloat * 0.2f);
		}

		public override void OnSpecialTargetChanged()
		{
			if (this._wanderTarget == null)
			{
				return;
			}
			if (!Extensions.IsEmpty<char>(base.Navigator.SpecialTargetTag) && !this._wanderTarget.GameEntity.HasTag(base.Navigator.SpecialTargetTag))
			{
				this._wanderTarget = null;
				base.Navigator.SetTarget(this._wanderTarget, false);
				return;
			}
			if (Extensions.IsEmpty<char>(base.Navigator.SpecialTargetTag) && !this._wanderTarget.GameEntity.HasTag("npc_common"))
			{
				this._wanderTarget = null;
				base.Navigator.SetTarget(this._wanderTarget, false);
			}
		}

		public override string GetDebugInfo()
		{
			string text = "Walk ";
			if (this._waitTimer != null)
			{
				text = string.Concat(new object[]
				{
					text,
					"(Wait ",
					(int)this._waitTimer.ElapsedTime(),
					"/",
					this._waitTimer.Duration,
					")"
				});
			}
			else if (this._wanderTarget == null)
			{
				text += "(search for target!)";
			}
			return text;
		}

		protected override void OnDeactivate()
		{
			base.Navigator.ClearTarget();
			this._wanderTarget = null;
			this._waitTimer = null;
		}

		private readonly MissionAgentHandler _missionAgentHandler;

		private readonly bool _isIndoor;

		private UsableMachine _wanderTarget;

		private UsableMachine _lastTarget;

		private Timer _waitTimer;

		private bool _indoorWanderingIsActive;

		private bool _outdoorWanderingIsActive;

		private bool _wasSimulation;
	}
}
