using System;
using SandBox.Missions.MissionLogics;
using TaleWorlds.MountAndBlade;

namespace SandBox.Missions.AgentBehaviors
{
	// Token: 0x02000078 RID: 120
	public class StandGuardBehavior : AgentBehavior
	{
		// Token: 0x0600052E RID: 1326 RVA: 0x00025616 File Offset: 0x00023816
		public StandGuardBehavior(AgentBehaviorGroup behaviorGroup)
			: base(behaviorGroup)
		{
			this._missionAgentHandler = base.Mission.GetMissionBehavior<MissionAgentHandler>();
		}

		// Token: 0x0600052F RID: 1327 RVA: 0x00025630 File Offset: 0x00023830
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

		// Token: 0x06000530 RID: 1328 RVA: 0x000256CA File Offset: 0x000238CA
		protected override void OnDeactivate()
		{
			base.Navigator.ClearTarget();
			this._standPoint = null;
		}

		// Token: 0x06000531 RID: 1329 RVA: 0x000256DE File Offset: 0x000238DE
		public override float GetAvailability(bool isSimulation)
		{
			return 1f;
		}

		// Token: 0x06000532 RID: 1330 RVA: 0x000256E5 File Offset: 0x000238E5
		public override string GetDebugInfo()
		{
			return "Guard stand";
		}

		// Token: 0x04000290 RID: 656
		private UsableMachine _oldStandPoint;

		// Token: 0x04000291 RID: 657
		private UsableMachine _standPoint;

		// Token: 0x04000292 RID: 658
		private readonly MissionAgentHandler _missionAgentHandler;

		// Token: 0x02000148 RID: 328
		private enum GuardState
		{
			// Token: 0x040005FC RID: 1532
			StandIdle,
			// Token: 0x040005FD RID: 1533
			StandAttention,
			// Token: 0x040005FE RID: 1534
			StandCautious,
			// Token: 0x040005FF RID: 1535
			GotToStandPoint
		}
	}
}
