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
	// Token: 0x0200007A RID: 122
	public class WalkingBehavior : AgentBehavior
	{
		// Token: 0x17000069 RID: 105
		// (get) Token: 0x0600053A RID: 1338 RVA: 0x0002598B File Offset: 0x00023B8B
		private bool CanWander
		{
			get
			{
				return (this._isIndoor && this._indoorWanderingIsActive) || (!this._isIndoor && this._outdoorWanderingIsActive);
			}
		}

		// Token: 0x0600053B RID: 1339 RVA: 0x000259B0 File Offset: 0x00023BB0
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

		// Token: 0x0600053C RID: 1340 RVA: 0x00025A06 File Offset: 0x00023C06
		public void SetIndoorWandering(bool isActive)
		{
			this._indoorWanderingIsActive = isActive;
		}

		// Token: 0x0600053D RID: 1341 RVA: 0x00025A0F File Offset: 0x00023C0F
		public void SetOutdoorWandering(bool isActive)
		{
			this._outdoorWanderingIsActive = isActive;
		}

		// Token: 0x0600053E RID: 1342 RVA: 0x00025A18 File Offset: 0x00023C18
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

		// Token: 0x0600053F RID: 1343 RVA: 0x00025C58 File Offset: 0x00023E58
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

		// Token: 0x06000540 RID: 1344 RVA: 0x00025CA8 File Offset: 0x00023EA8
		public override void ConversationTick()
		{
			if (this._waitTimer != null)
			{
				this._waitTimer.Reset(base.Mission.CurrentTime);
			}
		}

		// Token: 0x06000541 RID: 1345 RVA: 0x00025CC8 File Offset: 0x00023EC8
		public override float GetAvailability(bool isSimulation)
		{
			if (this.FindTarget() == null)
			{
				return 0f;
			}
			return 1f;
		}

		// Token: 0x06000542 RID: 1346 RVA: 0x00025CDD File Offset: 0x00023EDD
		public override void SetCustomWanderTarget(UsableMachine customUsableMachine)
		{
			this._wanderTarget = customUsableMachine;
			if (this._waitTimer != null)
			{
				this._waitTimer = null;
			}
		}

		// Token: 0x06000543 RID: 1347 RVA: 0x00025CF8 File Offset: 0x00023EF8
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

		// Token: 0x06000544 RID: 1348 RVA: 0x00025D6D File Offset: 0x00023F6D
		private UsableMachine FindTarget()
		{
			return this.FindRandomWalkingTarget(this._isIndoor && !this._indoorWanderingIsActive);
		}

		// Token: 0x06000545 RID: 1349 RVA: 0x00025D8C File Offset: 0x00023F8C
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

		// Token: 0x06000546 RID: 1350 RVA: 0x00025E60 File Offset: 0x00024060
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

		// Token: 0x06000547 RID: 1351 RVA: 0x00025F04 File Offset: 0x00024104
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

		// Token: 0x06000548 RID: 1352 RVA: 0x00025F85 File Offset: 0x00024185
		protected override void OnDeactivate()
		{
			base.Navigator.ClearTarget();
			this._wanderTarget = null;
			this._waitTimer = null;
		}

		// Token: 0x04000295 RID: 661
		private readonly MissionAgentHandler _missionAgentHandler;

		// Token: 0x04000296 RID: 662
		private readonly bool _isIndoor;

		// Token: 0x04000297 RID: 663
		private UsableMachine _wanderTarget;

		// Token: 0x04000298 RID: 664
		private UsableMachine _lastTarget;

		// Token: 0x04000299 RID: 665
		private Timer _waitTimer;

		// Token: 0x0400029A RID: 666
		private bool _indoorWanderingIsActive;

		// Token: 0x0400029B RID: 667
		private bool _outdoorWanderingIsActive;

		// Token: 0x0400029C RID: 668
		private bool _wasSimulation;
	}
}
