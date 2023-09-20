using System;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace SandBox.Missions.AgentBehaviors
{
	public class EscortAgentBehavior : AgentBehavior
	{
		public Agent EscortedAgent
		{
			get
			{
				return this._escortedAgent;
			}
		}

		public Agent TargetAgent
		{
			get
			{
				return this._targetAgent;
			}
		}

		public EscortAgentBehavior(AgentBehaviorGroup behaviorGroup)
			: base(behaviorGroup)
		{
			this._targetAgent = null;
			this._escortedAgent = null;
			this._myLastStateWasRunning = false;
			this._initialMaxSpeedLimit = 1f;
		}

		public void Initialize(Agent escortedAgent, Agent targetAgent, EscortAgentBehavior.OnTargetReachedDelegate onTargetReached = null)
		{
			this._escortedAgent = escortedAgent;
			this._targetAgent = targetAgent;
			this._targetMachine = null;
			this._targetPosition = null;
			this._onTargetReached = onTargetReached;
			this._escortFinished = false;
			this._initialMaxSpeedLimit = base.OwnerAgent.GetMaximumSpeedLimit();
			this._state = EscortAgentBehavior.State.Escorting;
		}

		public void Initialize(Agent escortedAgent, UsableMachine targetMachine, EscortAgentBehavior.OnTargetReachedDelegate onTargetReached = null)
		{
			this._escortedAgent = escortedAgent;
			this._targetAgent = null;
			this._targetMachine = targetMachine;
			this._targetPosition = null;
			this._onTargetReached = onTargetReached;
			this._escortFinished = false;
			this._initialMaxSpeedLimit = base.OwnerAgent.GetMaximumSpeedLimit();
			this._state = EscortAgentBehavior.State.Escorting;
		}

		public void Initialize(Agent escortedAgent, Vec3? targetPosition, EscortAgentBehavior.OnTargetReachedDelegate onTargetReached = null)
		{
			this._escortedAgent = escortedAgent;
			this._targetAgent = null;
			this._targetMachine = null;
			this._targetPosition = targetPosition;
			this._onTargetReached = onTargetReached;
			this._escortFinished = false;
			this._initialMaxSpeedLimit = base.OwnerAgent.GetMaximumSpeedLimit();
			this._state = EscortAgentBehavior.State.Escorting;
		}

		public override void Tick(float dt, bool isSimulation)
		{
			if (this._escortedAgent == null || !this._escortedAgent.IsActive() || this._targetAgent == null || !this._targetAgent.IsActive())
			{
				this._state = EscortAgentBehavior.State.NotEscorting;
			}
			if (this._escortedAgent != null && this._state != EscortAgentBehavior.State.NotEscorting)
			{
				this.ControlMovement();
			}
		}

		public bool IsEscortFinished()
		{
			return this._escortFinished;
		}

		private void ControlMovement()
		{
			int nearbyEnemyAgentCount = base.Mission.GetNearbyEnemyAgentCount(this._escortedAgent.Team, this._escortedAgent.Position.AsVec2, 5f);
			if (this._state != EscortAgentBehavior.State.NotEscorting && nearbyEnemyAgentCount > 0)
			{
				this._state = EscortAgentBehavior.State.NotEscorting;
				base.OwnerAgent.ResetLookAgent();
				base.Navigator.ClearTarget();
				base.OwnerAgent.DisableScriptedMovement();
				base.OwnerAgent.SetMaximumSpeedLimit(this._initialMaxSpeedLimit, false);
				Debug.Print("[Escort agent behavior] Escorted agent got into a fight... Disable!", 0, 12, 17592186044416UL);
				return;
			}
			float num = (base.OwnerAgent.HasMount ? 2.2f : 1.2f);
			float num2 = base.OwnerAgent.Position.DistanceSquared(this._escortedAgent.Position);
			float num3;
			WorldPosition worldPosition;
			float num4;
			if (this._targetAgent != null)
			{
				num3 = base.OwnerAgent.Position.DistanceSquared(this._targetAgent.Position);
				worldPosition = this._targetAgent.GetWorldPosition();
				MatrixFrame matrixFrame = this._targetAgent.Frame;
				num4 = matrixFrame.rotation.f.AsVec2.RotationInRadians;
			}
			else if (this._targetMachine != null)
			{
				MatrixFrame globalFrame = this._targetMachine.GameEntity.GetGlobalFrame();
				num3 = base.OwnerAgent.Position.DistanceSquared(globalFrame.origin);
				worldPosition = ModuleExtensions.ToWorldPosition(globalFrame.origin);
				num4 = globalFrame.rotation.f.AsVec2.RotationInRadians;
			}
			else if (this._targetPosition != null)
			{
				num3 = base.OwnerAgent.Position.DistanceSquared(this._targetPosition.Value);
				worldPosition = ModuleExtensions.ToWorldPosition(this._targetPosition.Value);
				num4 = (this._targetPosition.Value - base.OwnerAgent.Position).AsVec2.RotationInRadians;
			}
			else
			{
				Debug.FailedAssert("At least one target must be specified for the escort behavior.", "C:\\Develop\\MB3\\Source\\Bannerlord\\SandBox\\Missions\\AgentBehaviors\\EscortAgentBehavior.cs", "ControlMovement", 158);
				num3 = 0f;
				worldPosition = base.OwnerAgent.GetWorldPosition();
				num4 = 0f;
			}
			if (this._escortFinished)
			{
				bool flag = false;
				base.OwnerAgent.SetMaximumSpeedLimit(this._initialMaxSpeedLimit, false);
				if (this._onTargetReached != null)
				{
					flag = this._onTargetReached(base.OwnerAgent, ref this._escortedAgent, ref this._targetAgent, ref this._targetMachine, ref this._targetPosition);
				}
				if (flag && this._escortedAgent != null && (this._targetAgent != null || this._targetMachine != null || this._targetPosition != null))
				{
					this._state = EscortAgentBehavior.State.Escorting;
				}
				else
				{
					this._state = EscortAgentBehavior.State.NotEscorting;
				}
			}
			switch (this._state)
			{
			case EscortAgentBehavior.State.ReturnToEscortedAgent:
				if (num2 < 25f)
				{
					this._state = EscortAgentBehavior.State.Wait;
				}
				else
				{
					WorldPosition worldPosition2 = this._escortedAgent.GetWorldPosition();
					MatrixFrame matrixFrame = this._escortedAgent.Frame;
					this.SetMovePos(worldPosition2, matrixFrame.rotation.f.AsVec2.RotationInRadians, num);
				}
				break;
			case EscortAgentBehavior.State.Wait:
				if (num2 < 25f)
				{
					this._state = EscortAgentBehavior.State.Escorting;
					Debug.Print("[Escort agent behavior] Escorting!", 0, 12, 17592186044416UL);
				}
				else if (num2 > 100f)
				{
					this._state = EscortAgentBehavior.State.ReturnToEscortedAgent;
					Debug.Print("[Escort agent behavior] Escorted agent is too far away! Return to escorted agent!", 0, 12, 17592186044416UL);
				}
				else
				{
					WorldPosition worldPosition3 = base.OwnerAgent.GetWorldPosition();
					MatrixFrame matrixFrame = base.OwnerAgent.Frame;
					this.SetMovePos(worldPosition3, matrixFrame.rotation.f.AsVec2.RotationInRadians, 0f);
				}
				break;
			case EscortAgentBehavior.State.Escorting:
				if (num2 >= 25f)
				{
					this._state = EscortAgentBehavior.State.Wait;
					Debug.Print("[Escort agent behavior] Stop walking! Wait", 0, 12, 17592186044416UL);
				}
				else
				{
					this.SetMovePos(worldPosition, num4, 3f);
				}
				break;
			}
			if (this._state == EscortAgentBehavior.State.Escorting && num3 < 16f && num2 < 16f)
			{
				this._escortFinished = true;
			}
		}

		private void SetMovePos(WorldPosition targetPosition, float targetRotation, float rangeThreshold)
		{
			Agent.AIScriptedFrameFlags aiscriptedFrameFlags = 2;
			if (base.Navigator.CharacterHasVisiblePrefabs)
			{
				this._myLastStateWasRunning = false;
			}
			else
			{
				float num = base.OwnerAgent.Position.AsVec2.Distance(targetPosition.AsVec2);
				float length = this._escortedAgent.Velocity.AsVec2.Length;
				if (num - rangeThreshold <= 0.5f * (this._myLastStateWasRunning ? 1f : 1.2f) && length <= base.OwnerAgent.Monster.WalkingSpeedLimit * (this._myLastStateWasRunning ? 1f : 1.2f))
				{
					this._myLastStateWasRunning = false;
				}
				else
				{
					base.OwnerAgent.SetMaximumSpeedLimit(num - rangeThreshold + length, false);
					this._myLastStateWasRunning = true;
				}
			}
			if (!this._myLastStateWasRunning)
			{
				aiscriptedFrameFlags |= 16;
			}
			base.Navigator.SetTargetFrame(targetPosition, targetRotation, rangeThreshold, -10f, aiscriptedFrameFlags, false);
		}

		public override float GetAvailability(bool isSimulation)
		{
			return (float)((this._state == EscortAgentBehavior.State.NotEscorting) ? 0 : 1);
		}

		protected override void OnDeactivate()
		{
			this._escortedAgent = null;
			this._targetAgent = null;
			this._targetMachine = null;
			this._targetPosition = null;
			this._onTargetReached = null;
			this._state = EscortAgentBehavior.State.NotEscorting;
			base.OwnerAgent.DisableScriptedMovement();
			base.OwnerAgent.ResetLookAgent();
			base.Navigator.ClearTarget();
		}

		public override string GetDebugInfo()
		{
			return string.Concat(new object[]
			{
				"Escort ",
				this._escortedAgent.Name,
				" (id:",
				this._escortedAgent.Index,
				")",
				(this._targetAgent != null) ? string.Concat(new object[]
				{
					" to ",
					this._targetAgent.Name,
					" (id:",
					this._targetAgent.Index,
					")"
				}) : ((this._targetMachine != null) ? string.Concat(new object[]
				{
					" to ",
					this._targetMachine,
					"(id:",
					this._targetMachine.Id,
					")"
				}) : ((this._targetPosition != null) ? (" to position: " + this._targetPosition.Value) : " to NO TARGET"))
			});
		}

		public static void AddEscortAgentBehavior(Agent ownerAgent, Agent targetAgent, EscortAgentBehavior.OnTargetReachedDelegate onTargetReached)
		{
			AgentNavigator agentNavigator = ownerAgent.GetComponent<CampaignAgentComponent>().AgentNavigator;
			InterruptingBehaviorGroup interruptingBehaviorGroup = ((agentNavigator != null) ? agentNavigator.GetBehaviorGroup<InterruptingBehaviorGroup>() : null);
			if (interruptingBehaviorGroup == null)
			{
				return;
			}
			bool flag = interruptingBehaviorGroup.GetBehavior<EscortAgentBehavior>() == null;
			EscortAgentBehavior escortAgentBehavior = interruptingBehaviorGroup.GetBehavior<EscortAgentBehavior>() ?? interruptingBehaviorGroup.AddBehavior<EscortAgentBehavior>();
			if (flag)
			{
				interruptingBehaviorGroup.SetScriptedBehavior<EscortAgentBehavior>();
			}
			escortAgentBehavior.Initialize(Agent.Main, targetAgent, onTargetReached);
		}

		public static void RemoveEscortBehaviorOfAgent(Agent ownerAgent)
		{
			AgentNavigator agentNavigator = ownerAgent.GetComponent<CampaignAgentComponent>().AgentNavigator;
			InterruptingBehaviorGroup interruptingBehaviorGroup = ((agentNavigator != null) ? agentNavigator.GetBehaviorGroup<InterruptingBehaviorGroup>() : null);
			if (interruptingBehaviorGroup == null)
			{
				return;
			}
			if (interruptingBehaviorGroup.GetBehavior<EscortAgentBehavior>() != null)
			{
				interruptingBehaviorGroup.RemoveBehavior<EscortAgentBehavior>();
			}
		}

		public static bool CheckIfAgentIsEscortedBy(Agent ownerAgent, Agent escortedAgent)
		{
			AgentNavigator agentNavigator = ownerAgent.GetComponent<CampaignAgentComponent>().AgentNavigator;
			InterruptingBehaviorGroup interruptingBehaviorGroup = ((agentNavigator != null) ? agentNavigator.GetBehaviorGroup<InterruptingBehaviorGroup>() : null);
			EscortAgentBehavior escortAgentBehavior = ((interruptingBehaviorGroup != null) ? interruptingBehaviorGroup.GetBehavior<EscortAgentBehavior>() : null);
			return escortAgentBehavior != null && escortAgentBehavior.EscortedAgent == escortedAgent;
		}

		private const float StartWaitingDistanceSquared = 25f;

		private const float ReturnToEscortedAgentDistanceSquared = 100f;

		private const float EscortFinishedDistanceSquared = 16f;

		private const float TargetProximityThreshold = 3f;

		private const float MountedMoveProximityThreshold = 2.2f;

		private const float OnFootMoveProximityThreshold = 1.2f;

		private EscortAgentBehavior.State _state;

		private Agent _escortedAgent;

		private Agent _targetAgent;

		private UsableMachine _targetMachine;

		private Vec3? _targetPosition;

		private bool _myLastStateWasRunning;

		private float _initialMaxSpeedLimit;

		private EscortAgentBehavior.OnTargetReachedDelegate _onTargetReached;

		private bool _escortFinished;

		public delegate bool OnTargetReachedDelegate(Agent agent, ref Agent escortedAgent, ref Agent targetAgent, ref UsableMachine targetMachine, ref Vec3? targetPosition);

		private enum State
		{
			NotEscorting,
			ReturnToEscortedAgent,
			Wait,
			Escorting
		}
	}
}
