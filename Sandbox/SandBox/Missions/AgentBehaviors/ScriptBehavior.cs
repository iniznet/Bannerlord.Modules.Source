using System;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace SandBox.Missions.AgentBehaviors
{
	// Token: 0x02000077 RID: 119
	public class ScriptBehavior : AgentBehavior
	{
		// Token: 0x06000521 RID: 1313 RVA: 0x000250C7 File Offset: 0x000232C7
		public ScriptBehavior(AgentBehaviorGroup behaviorGroup)
			: base(behaviorGroup)
		{
		}

		// Token: 0x06000522 RID: 1314 RVA: 0x000250D0 File Offset: 0x000232D0
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

		// Token: 0x06000523 RID: 1315 RVA: 0x00025128 File Offset: 0x00023328
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

		// Token: 0x06000524 RID: 1316 RVA: 0x00025180 File Offset: 0x00023380
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

		// Token: 0x06000525 RID: 1317 RVA: 0x000251D8 File Offset: 0x000233D8
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

		// Token: 0x06000526 RID: 1318 RVA: 0x00025237 File Offset: 0x00023437
		public bool IsNearTarget(Agent targetAgent)
		{
			return this._targetAgent == targetAgent && (this._state == ScriptBehavior.State.NearAgent || this._state == ScriptBehavior.State.NearStationaryTarget);
		}

		// Token: 0x06000527 RID: 1319 RVA: 0x00025258 File Offset: 0x00023458
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

		// Token: 0x06000528 RID: 1320 RVA: 0x00025504 File Offset: 0x00023704
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

		// Token: 0x06000529 RID: 1321 RVA: 0x00025554 File Offset: 0x00023754
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

		// Token: 0x0600052A RID: 1322 RVA: 0x000255B6 File Offset: 0x000237B6
		public override float GetAvailability(bool isSimulation)
		{
			return (float)((this._state == ScriptBehavior.State.NoTarget) ? 0 : 1);
		}

		// Token: 0x0600052B RID: 1323 RVA: 0x000255C5 File Offset: 0x000237C5
		protected override void OnDeactivate()
		{
			base.Navigator.ClearTarget();
			this.RemoveTargets();
		}

		// Token: 0x0600052C RID: 1324 RVA: 0x000255D8 File Offset: 0x000237D8
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

		// Token: 0x0600052D RID: 1325 RVA: 0x0002560F File Offset: 0x0002380F
		public override string GetDebugInfo()
		{
			return "Scripted";
		}

		// Token: 0x04000289 RID: 649
		private UsableMachine _targetUsableMachine;

		// Token: 0x0400028A RID: 650
		private Agent _targetAgent;

		// Token: 0x0400028B RID: 651
		private WorldFrame _targetFrame;

		// Token: 0x0400028C RID: 652
		private ScriptBehavior.State _state;

		// Token: 0x0400028D RID: 653
		private bool _sentToTarget;

		// Token: 0x0400028E RID: 654
		private ScriptBehavior.SelectTargetDelegate _selectTargetDelegate;

		// Token: 0x0400028F RID: 655
		private ScriptBehavior.OnTargetReachedDelegate _onTargetReachedDelegate;

		// Token: 0x02000145 RID: 325
		// (Invoke) Token: 0x06000D76 RID: 3446
		public delegate bool SelectTargetDelegate(Agent agent, ref Agent targetAgent, ref UsableMachine targetUsableMachine, ref WorldFrame targetFrame);

		// Token: 0x02000146 RID: 326
		// (Invoke) Token: 0x06000D7A RID: 3450
		public delegate bool OnTargetReachedDelegate(Agent agent, ref Agent targetAgent, ref UsableMachine targetUsableMachine, ref WorldFrame targetFrame);

		// Token: 0x02000147 RID: 327
		private enum State
		{
			// Token: 0x040005F5 RID: 1525
			NoTarget,
			// Token: 0x040005F6 RID: 1526
			GoToUsableMachine,
			// Token: 0x040005F7 RID: 1527
			GoToAgent,
			// Token: 0x040005F8 RID: 1528
			GoToTargetFrame,
			// Token: 0x040005F9 RID: 1529
			NearAgent,
			// Token: 0x040005FA RID: 1530
			NearStationaryTarget
		}
	}
}
