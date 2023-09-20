using System;
using SandBox.Conversation.MissionLogics;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace SandBox.Missions.AgentBehaviors
{
	// Token: 0x02000079 RID: 121
	public class TalkBehavior : AgentBehavior
	{
		// Token: 0x06000533 RID: 1331 RVA: 0x000256EC File Offset: 0x000238EC
		public TalkBehavior(AgentBehaviorGroup behaviorGroup)
			: base(behaviorGroup)
		{
			this._startConversation = true;
			this._doNotMove = true;
		}

		// Token: 0x06000534 RID: 1332 RVA: 0x00025704 File Offset: 0x00023904
		public override void Tick(float dt, bool isSimulation)
		{
			if (!this._startConversation || base.Mission.MainAgent == null || !base.Mission.MainAgent.IsActive() || base.Mission.Mode == 1 || base.Mission.Mode == 2 || base.Mission.Mode == 5)
			{
				return;
			}
			float interactionDistanceToUsable = base.OwnerAgent.GetInteractionDistanceToUsable(base.Mission.MainAgent);
			if (base.OwnerAgent.Position.DistanceSquared(base.Mission.MainAgent.Position) < (interactionDistanceToUsable + 3f) * (interactionDistanceToUsable + 3f) && base.Navigator.CanSeeAgent(base.Mission.MainAgent))
			{
				AgentNavigator navigator = base.Navigator;
				WorldPosition worldPosition = base.OwnerAgent.GetWorldPosition();
				MatrixFrame matrixFrame = base.OwnerAgent.Frame;
				navigator.SetTargetFrame(worldPosition, matrixFrame.rotation.f.AsVec2.RotationInRadians, 1f, -10f, 16, false);
				MissionConversationLogic missionBehavior = base.Mission.GetMissionBehavior<MissionConversationLogic>();
				if (missionBehavior != null && missionBehavior.IsReadyForConversation)
				{
					missionBehavior.OnAgentInteraction(base.Mission.MainAgent, base.OwnerAgent);
					this._startConversation = false;
					return;
				}
			}
			else if (!this._doNotMove)
			{
				AgentNavigator navigator2 = base.Navigator;
				WorldPosition worldPosition2 = Agent.Main.GetWorldPosition();
				MatrixFrame matrixFrame = Agent.Main.Frame;
				navigator2.SetTargetFrame(worldPosition2, matrixFrame.rotation.f.AsVec2.RotationInRadians, 1f, -10f, 16, false);
			}
		}

		// Token: 0x06000535 RID: 1333 RVA: 0x00025898 File Offset: 0x00023A98
		public override float GetAvailability(bool isSimulation)
		{
			if (isSimulation)
			{
				return 0f;
			}
			if (this._startConversation && base.Mission.MainAgent != null && base.Mission.MainAgent.IsActive())
			{
				float num = base.OwnerAgent.GetInteractionDistanceToUsable(base.Mission.MainAgent) + 3f;
				if (base.OwnerAgent.Position.DistanceSquared(base.Mission.MainAgent.Position) < num * num && base.Mission.Mode != 1 && !base.Mission.MainAgent.IsEnemyOf(base.OwnerAgent))
				{
					return 1f;
				}
			}
			return 0f;
		}

		// Token: 0x06000536 RID: 1334 RVA: 0x00025951 File Offset: 0x00023B51
		public override string GetDebugInfo()
		{
			return "Talk";
		}

		// Token: 0x06000537 RID: 1335 RVA: 0x00025958 File Offset: 0x00023B58
		protected override void OnDeactivate()
		{
			base.Navigator.ClearTarget();
			this.Disable();
		}

		// Token: 0x06000538 RID: 1336 RVA: 0x0002596B File Offset: 0x00023B6B
		public void Disable()
		{
			this._startConversation = false;
			this._doNotMove = true;
		}

		// Token: 0x06000539 RID: 1337 RVA: 0x0002597B File Offset: 0x00023B7B
		public void Enable(bool doNotMove)
		{
			this._startConversation = true;
			this._doNotMove = doNotMove;
		}

		// Token: 0x04000293 RID: 659
		private bool _doNotMove;

		// Token: 0x04000294 RID: 660
		private bool _startConversation;
	}
}
