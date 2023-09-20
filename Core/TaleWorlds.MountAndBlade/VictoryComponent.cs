using System;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x020000F8 RID: 248
	public class VictoryComponent : AgentComponent
	{
		// Token: 0x06000C55 RID: 3157 RVA: 0x000180B9 File Offset: 0x000162B9
		public VictoryComponent(Agent agent, RandomTimer timer)
			: base(agent)
		{
			this._timer = timer;
		}

		// Token: 0x06000C56 RID: 3158 RVA: 0x000180C9 File Offset: 0x000162C9
		public bool CheckTimer()
		{
			return this._timer.Check(Mission.Current.CurrentTime);
		}

		// Token: 0x06000C57 RID: 3159 RVA: 0x000180E0 File Offset: 0x000162E0
		public void ChangeTimerDuration(float min, float max)
		{
			this._timer.ChangeDuration(min, max);
		}

		// Token: 0x040002CA RID: 714
		private readonly RandomTimer _timer;
	}
}
