using System;
using SandBox.Missions.MissionLogics;
using TaleWorlds.MountAndBlade;

namespace SandBox.Missions.AgentBehaviors
{
	public class StandGuardBehavior : AgentBehavior
	{
		public StandGuardBehavior(AgentBehaviorGroup behaviorGroup)
			: base(behaviorGroup)
		{
			this._missionAgentHandler = base.Mission.GetMissionBehavior<MissionAgentHandler>();
		}

		public override void Tick(float dt, bool isSimulation)
		{
			if (base.OwnerAgent.CurrentWatchState == null)
			{
				if (this._standPoint == null || isSimulation)
				{
					UsableMachine usableMachine = this._oldStandPoint ?? this._missionAgentHandler.FindUnusedPointWithTagForAgent(base.OwnerAgent, base.Navigator.SpecialTargetTag);
					if (usableMachine != null)
					{
						this._oldStandPoint = null;
						this._standPoint = usableMachine;
						base.Navigator.SetTarget(this._standPoint, false);
						return;
					}
				}
			}
			else if (this._standPoint != null)
			{
				this._oldStandPoint = this._standPoint;
				base.Navigator.SetTarget(null, false);
				this._standPoint = null;
			}
		}

		protected override void OnDeactivate()
		{
			base.Navigator.ClearTarget();
			this._standPoint = null;
		}

		public override float GetAvailability(bool isSimulation)
		{
			return 1f;
		}

		public override string GetDebugInfo()
		{
			return "Guard stand";
		}

		private UsableMachine _oldStandPoint;

		private UsableMachine _standPoint;

		private readonly MissionAgentHandler _missionAgentHandler;

		private enum GuardState
		{
			StandIdle,
			StandAttention,
			StandCautious,
			GotToStandPoint
		}
	}
}
