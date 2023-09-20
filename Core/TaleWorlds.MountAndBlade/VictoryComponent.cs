using System;

namespace TaleWorlds.MountAndBlade
{
	public class VictoryComponent : AgentComponent
	{
		public VictoryComponent(Agent agent, RandomTimer timer)
			: base(agent)
		{
			this._timer = timer;
		}

		public bool CheckTimer()
		{
			return this._timer.Check(Mission.Current.CurrentTime);
		}

		public void ChangeTimerDuration(float min, float max)
		{
			this._timer.ChangeDuration(min, max);
		}

		private readonly RandomTimer _timer;
	}
}
