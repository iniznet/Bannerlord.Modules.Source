using System;
using TaleWorlds.Core;

namespace TaleWorlds.MountAndBlade.Source.Missions.Handlers
{
	public class MissionFacialAnimationHandler : MissionLogic
	{
		public override void EarlyStart()
		{
			this._animRefreshTimer = new Timer(base.Mission.CurrentTime, 5f, true);
		}

		public override void AfterStart()
		{
		}

		public override void OnMissionTick(float dt)
		{
		}

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

		private Timer _animRefreshTimer;
	}
}
