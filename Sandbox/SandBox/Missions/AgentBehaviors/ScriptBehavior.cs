using System;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace SandBox.Missions.AgentBehaviors
{
	public class ScriptBehavior : AgentBehavior
	{
		public ScriptBehavior(AgentBehaviorGroup behaviorGroup)
			: base(behaviorGroup)
		{
		}

		public static void AddUsableMachineTarget(Agent ownerAgent, UsableMachine targetUsableMachine)
		{
			DailyBehaviorGroup behaviorGroup = ownerAgent.GetComponent<CampaignAgentComponent>().AgentNavigator.GetBehaviorGroup<DailyBehaviorGroup>();
			ScriptBehavior scriptBehavior = behaviorGroup.GetBehavior<ScriptBehavior>() ?? behaviorGroup.AddBehavior<ScriptBehavior>();
			bool flag = behaviorGroup.ScriptedBehavior != scriptBehavior;
			scriptBehavior._targetUsableMachine = targetUsableMachine;
			scriptBehavior._state = ScriptBehavior.State.GoToUsableMachine;
			scriptBehavior._sentToTarget = false;
			if (flag)
			{
				behaviorGroup.SetScriptedBehavior<ScriptBehavior>();
			}
		}

		public static void AddAgentTarget(Agent ownerAgent, Agent targetAgent)
		{
			DailyBehaviorGroup behaviorGroup = ownerAgent.GetComponent<CampaignAgentComponent>().AgentNavigator.GetBehaviorGroup<DailyBehaviorGroup>();
			ScriptBehavior scriptBehavior = behaviorGroup.GetBehavior<ScriptBehavior>() ?? behaviorGroup.AddBehavior<ScriptBehavior>();
			bool flag = behaviorGroup.ScriptedBehavior != scriptBehavior;
			scriptBehavior._targetAgent = targetAgent;
			scriptBehavior._state = ScriptBehavior.State.GoToAgent;
			scriptBehavior._sentToTarget = false;
			if (flag)
			{
				behaviorGroup.SetScriptedBehavior<ScriptBehavior>();
			}
		}

		public static void AddWorldFrameTarget(Agent ownerAgent, WorldFrame targetWorldFrame)
		{
			DailyBehaviorGroup behaviorGroup = ownerAgent.GetComponent<CampaignAgentComponent>().AgentNavigator.GetBehaviorGroup<DailyBehaviorGroup>();
			ScriptBehavior scriptBehavior = behaviorGroup.GetBehavior<ScriptBehavior>() ?? behaviorGroup.AddBehavior<ScriptBehavior>();
			bool flag = behaviorGroup.ScriptedBehavior != scriptBehavior;
			scriptBehavior._targetFrame = targetWorldFrame;
			scriptBehavior._state = ScriptBehavior.State.GoToTargetFrame;
			scriptBehavior._sentToTarget = false;
			if (flag)
			{
				behaviorGroup.SetScriptedBehavior<ScriptBehavior>();
			}
		}

		public static void AddTargetWithDelegate(Agent ownerAgent, ScriptBehavior.SelectTargetDelegate selectTargetDelegate, ScriptBehavior.OnTargetReachedDelegate onTargetReachedDelegate)
		{
			DailyBehaviorGroup behaviorGroup = ownerAgent.GetComponent<CampaignAgentComponent>().AgentNavigator.GetBehaviorGroup<DailyBehaviorGroup>();
			ScriptBehavior scriptBehavior = behaviorGroup.GetBehavior<ScriptBehavior>() ?? behaviorGroup.AddBehavior<ScriptBehavior>();
			bool flag = behaviorGroup.ScriptedBehavior != scriptBehavior;
			scriptBehavior._selectTargetDelegate = selectTargetDelegate;
			scriptBehavior._onTargetReachedDelegate = onTargetReachedDelegate;
			scriptBehavior._state = ScriptBehavior.State.NoTarget;
			scriptBehavior._sentToTarget = false;
			if (flag)
			{
				behaviorGroup.SetScriptedBehavior<ScriptBehavior>();
			}
		}

		public bool IsNearTarget(Agent targetAgent)
		{
			return this._targetAgent == targetAgent && (this._state == ScriptBehavior.State.NearAgent || this._state == ScriptBehavior.State.NearStationaryTarget);
		}

		public override void Tick(float dt, bool isSimulation)
		{
			if (this._state == ScriptBehavior.State.NoTarget)
			{
				if (this._selectTargetDelegate == null)
				{
					if (this.BehaviorGroup.ScriptedBehavior == this)
					{
						this.BehaviorGroup.DisableScriptedBehavior();
					}
					return;
				}
				this.SearchForNewTarget();
			}
			switch (this._state)
			{
			case ScriptBehavior.State.GoToUsableMachine:
				if (!this._sentToTarget)
				{
					base.Navigator.SetTarget(this._targetUsableMachine, false);
					this._sentToTarget = true;
					return;
				}
				if (base.OwnerAgent.IsUsingGameObject && base.OwnerAgent.Position.DistanceSquared(this._targetUsableMachine.GameEntity.GetGlobalFrame().origin) < 1f)
				{
					if (this.CheckForSearchNewTarget(ScriptBehavior.State.NearStationaryTarget))
					{
						base.OwnerAgent.StopUsingGameObject(false, 1);
						return;
					}
					this.RemoveTargets();
					return;
				}
				break;
			case ScriptBehavior.State.GoToAgent:
			{
				float interactionDistanceToUsable = base.OwnerAgent.GetInteractionDistanceToUsable(this._targetAgent);
				if (base.OwnerAgent.Position.DistanceSquared(this._targetAgent.Position) >= interactionDistanceToUsable * interactionDistanceToUsable)
				{
					AgentNavigator navigator = base.Navigator;
					WorldPosition worldPosition = this._targetAgent.GetWorldPosition();
					MatrixFrame matrixFrame = this._targetAgent.Frame;
					navigator.SetTargetFrame(worldPosition, matrixFrame.rotation.f.AsVec2.RotationInRadians, 1f, 1f, 0, false);
					return;
				}
				if (!this.CheckForSearchNewTarget(ScriptBehavior.State.NearAgent))
				{
					AgentNavigator navigator2 = base.Navigator;
					WorldPosition worldPosition2 = base.OwnerAgent.GetWorldPosition();
					MatrixFrame matrixFrame = base.OwnerAgent.Frame;
					navigator2.SetTargetFrame(worldPosition2, matrixFrame.rotation.f.AsVec2.RotationInRadians, 1f, 1f, 0, false);
					this.RemoveTargets();
					return;
				}
				break;
			}
			case ScriptBehavior.State.GoToTargetFrame:
				if (!this._sentToTarget)
				{
					base.Navigator.SetTargetFrame(this._targetFrame.Origin, this._targetFrame.Rotation.f.AsVec2.RotationInRadians, 1f, 1f, 16, false);
					this._sentToTarget = true;
					return;
				}
				if (base.Navigator.IsTargetReached() && !this.CheckForSearchNewTarget(ScriptBehavior.State.NearStationaryTarget))
				{
					this.RemoveTargets();
					return;
				}
				break;
			case ScriptBehavior.State.NearAgent:
			{
				if (base.OwnerAgent.Position.DistanceSquared(this._targetAgent.Position) >= 1f)
				{
					this._state = ScriptBehavior.State.GoToAgent;
					return;
				}
				AgentNavigator navigator3 = base.Navigator;
				WorldPosition worldPosition3 = base.OwnerAgent.GetWorldPosition();
				MatrixFrame matrixFrame = base.OwnerAgent.Frame;
				navigator3.SetTargetFrame(worldPosition3, matrixFrame.rotation.f.AsVec2.RotationInRadians, 1f, 1f, 0, false);
				this.RemoveTargets();
				break;
			}
			default:
				return;
			}
		}

		private bool CheckForSearchNewTarget(ScriptBehavior.State endState)
		{
			bool flag = false;
			if (this._onTargetReachedDelegate != null)
			{
				flag = this._onTargetReachedDelegate(base.OwnerAgent, ref this._targetAgent, ref this._targetUsableMachine, ref this._targetFrame);
			}
			if (flag)
			{
				this.SearchForNewTarget();
			}
			else
			{
				this._state = endState;
			}
			return flag;
		}

		private void SearchForNewTarget()
		{
			Agent agent = null;
			UsableMachine usableMachine = null;
			WorldFrame invalid = WorldFrame.Invalid;
			if (this._selectTargetDelegate(base.OwnerAgent, ref agent, ref usableMachine, ref invalid))
			{
				if (agent != null)
				{
					this._targetAgent = agent;
					this._state = ScriptBehavior.State.GoToAgent;
					return;
				}
				if (usableMachine != null)
				{
					this._targetUsableMachine = usableMachine;
					this._state = ScriptBehavior.State.GoToUsableMachine;
					return;
				}
				this._targetFrame = invalid;
				this._state = ScriptBehavior.State.GoToTargetFrame;
			}
		}

		public override float GetAvailability(bool isSimulation)
		{
			return (float)((this._state == ScriptBehavior.State.NoTarget) ? 0 : 1);
		}

		protected override void OnDeactivate()
		{
			base.Navigator.ClearTarget();
			this.RemoveTargets();
		}

		private void RemoveTargets()
		{
			this._targetUsableMachine = null;
			this._targetAgent = null;
			this._targetFrame = WorldFrame.Invalid;
			this._state = ScriptBehavior.State.NoTarget;
			this._selectTargetDelegate = null;
			this._onTargetReachedDelegate = null;
			this._sentToTarget = false;
		}

		public override string GetDebugInfo()
		{
			return "Scripted";
		}

		private UsableMachine _targetUsableMachine;

		private Agent _targetAgent;

		private WorldFrame _targetFrame;

		private ScriptBehavior.State _state;

		private bool _sentToTarget;

		private ScriptBehavior.SelectTargetDelegate _selectTargetDelegate;

		private ScriptBehavior.OnTargetReachedDelegate _onTargetReachedDelegate;

		public delegate bool SelectTargetDelegate(Agent agent, ref Agent targetAgent, ref UsableMachine targetUsableMachine, ref WorldFrame targetFrame);

		public delegate bool OnTargetReachedDelegate(Agent agent, ref Agent targetAgent, ref UsableMachine targetUsableMachine, ref WorldFrame targetFrame);

		private enum State
		{
			NoTarget,
			GoToUsableMachine,
			GoToAgent,
			GoToTargetFrame,
			NearAgent,
			NearStationaryTarget
		}
	}
}
