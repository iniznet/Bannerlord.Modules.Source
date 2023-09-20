using System;

namespace TaleWorlds.Network
{
	// Token: 0x02000007 RID: 7
	public class WaitForTicks : CoroutineState
	{
		// Token: 0x1700000A RID: 10
		// (get) Token: 0x0600002D RID: 45 RVA: 0x00002696 File Offset: 0x00000896
		// (set) Token: 0x0600002E RID: 46 RVA: 0x0000269E File Offset: 0x0000089E
		internal int TickCount { get; private set; }

		// Token: 0x0600002F RID: 47 RVA: 0x000026A7 File Offset: 0x000008A7
		public WaitForTicks(int tickCount)
		{
			this.TickCount = tickCount;
		}

		// Token: 0x06000030 RID: 48 RVA: 0x000026B6 File Offset: 0x000008B6
		protected internal override void Initialize(CoroutineManager coroutineManager)
		{
			base.Initialize(coroutineManager);
			this._beginTick = coroutineManager.CurrentTick;
		}

		// Token: 0x1700000B RID: 11
		// (get) Token: 0x06000031 RID: 49 RVA: 0x000026CB File Offset: 0x000008CB
		protected internal override bool IsFinished
		{
			get
			{
				return this._beginTick + this.TickCount >= base.CoroutineManager.CurrentTick;
			}
		}

		// Token: 0x04000018 RID: 24
		private int _beginTick;
	}
}
