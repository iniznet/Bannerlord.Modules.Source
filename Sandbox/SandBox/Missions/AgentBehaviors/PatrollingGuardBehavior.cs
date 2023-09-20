using System;
using SandBox.Missions.MissionLogics;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace SandBox.Missions.AgentBehaviors
{
	public class PatrollingGuardBehavior : AgentBehavior
	{
		public PatrollingGuardBehavior(AgentBehaviorGroup behaviorGroup)
			: base(behaviorGroup)
		{
			this._missionAgentHandler = base.Mission.GetMissionBehavior<MissionAgentHandler>();
		}

		public override void Tick(float dt, bool isSimulation)
		{
			if (this._target == null)
			{
				UsableMachine usableMachine = ((base.Navigator.SpecialTargetTag == null || Extensions.IsEmpty<char>(base.Navigator.SpecialTargetTag)) ? this._missionAgentHandler.FindUnusedPointWithTagForAgent(base.OwnerAgent, "npc_common") : this._missionAgentHandler.FindUnusedPointWithTagForAgent(base.OwnerAgent, base.Navigator.SpecialTargetTag));
				if (usableMachine != null)
				{
					this._target = usableMachine;
					base.Navigator.SetTarget(this._target, false);
					return;
				}
			}
			else if (base.Navigator.TargetUsableMachine == null)
			{
				base.Navigator.SetTarget(this._target, false);
			}
		}

		public override float GetAvailability(bool isSimulation)
		{
			if (this._missionAgentHandler.GetAllUsablePointsWithTag(base.Navigator.SpecialTargetTag).Count <= 0)
			{
				return 0f;
			}
			return 1f;
		}

		protected override void OnDeactivate()
		{
			this._target = null;
			base.Navigator.ClearTarget();
		}

		public override string GetDebugInfo()
		{
			return "Guard patrol";
		}

		private readonly MissionAgentHandler _missionAgentHandler;

		private UsableMachine _target;
	}
}
