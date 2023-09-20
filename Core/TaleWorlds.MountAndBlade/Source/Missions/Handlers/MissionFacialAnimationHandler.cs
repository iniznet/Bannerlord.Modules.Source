using System;
using TaleWorlds.Core;

namespace TaleWorlds.MountAndBlade.Source.Missions.Handlers
{
	// Token: 0x020003FB RID: 1019
	public class MissionFacialAnimationHandler : MissionLogic
	{
		// Token: 0x06003507 RID: 13575 RVA: 0x000DCE2C File Offset: 0x000DB02C
		public override void EarlyStart()
		{
			this._animRefreshTimer = new Timer(base.Mission.CurrentTime, 5f, true);
		}

		// Token: 0x06003508 RID: 13576 RVA: 0x000DCE4A File Offset: 0x000DB04A
		public override void AfterStart()
		{
		}

		// Token: 0x06003509 RID: 13577 RVA: 0x000DCE4C File Offset: 0x000DB04C
		public override void OnMissionTick(float dt)
		{
		}

		// Token: 0x0600350A RID: 13578 RVA: 0x000DCE50 File Offset: 0x000DB050
		private void SetDefaultFacialAnimationsForAllAgents()
		{
			foreach (Agent agent in base.Mission.Agents)
			{
				if (agent.IsActive() && agent.IsHuman)
				{
					agent.SetAgentFacialAnimation(Agent.FacialAnimChannel.Low, "idle_tired", true);
				}
			}
		}

		// Token: 0x040016B8 RID: 5816
		private Timer _animRefreshTimer;
	}
}
