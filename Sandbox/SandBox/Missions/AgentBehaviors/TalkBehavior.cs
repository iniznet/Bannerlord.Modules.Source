using System;
using SandBox.Conversation.MissionLogics;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace SandBox.Missions.AgentBehaviors
{
	public class TalkBehavior : AgentBehavior
	{
		public TalkBehavior(AgentBehaviorGroup behaviorGroup)
			: base(behaviorGroup)
		{
			this._startConversation = true;
			this._doNotMove = true;
		}

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

		public override string GetDebugInfo()
		{
			return "Talk";
		}

		protected override void OnDeactivate()
		{
			base.Navigator.ClearTarget();
			this.Disable();
		}

		public void Disable()
		{
			this._startConversation = false;
			this._doNotMove = true;
		}

		public void Enable(bool doNotMove)
		{
			this._startConversation = true;
			this._doNotMove = doNotMove;
		}

		private bool _doNotMove;

		private bool _startConversation;
	}
}
