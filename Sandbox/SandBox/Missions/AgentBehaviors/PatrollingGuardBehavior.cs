using System;
using SandBox.Missions.MissionLogics;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace SandBox.Missions.AgentBehaviors
{
	// Token: 0x02000076 RID: 118
	public class PatrollingGuardBehavior : AgentBehavior
	{
		// Token: 0x0600051C RID: 1308 RVA: 0x00024FC0 File Offset: 0x000231C0
		public PatrollingGuardBehavior(AgentBehaviorGroup behaviorGroup)
			: base(behaviorGroup)
		{
			this._missionAgentHandler = base.Mission.GetMissionBehavior<MissionAgentHandler>();
		}

		// Token: 0x0600051D RID: 1309 RVA: 0x00024FDC File Offset: 0x000231DC
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

		// Token: 0x0600051E RID: 1310 RVA: 0x00025081 File Offset: 0x00023281
		public override float GetAvailability(bool isSimulation)
		{
			if (this._missionAgentHandler.GetAllUsablePointsWithTag(base.Navigator.SpecialTargetTag).Count <= 0)
			{
				return 0f;
			}
			return 1f;
		}

		// Token: 0x0600051F RID: 1311 RVA: 0x000250AC File Offset: 0x000232AC
		protected override void OnDeactivate()
		{
			this._target = null;
			base.Navigator.ClearTarget();
		}

		// Token: 0x06000520 RID: 1312 RVA: 0x000250C0 File Offset: 0x000232C0
		public override string GetDebugInfo()
		{
			return "Guard patrol";
		}

		// Token: 0x04000287 RID: 647
		private readonly MissionAgentHandler _missionAgentHandler;

		// Token: 0x04000288 RID: 648
		private UsableMachine _target;
	}
}
