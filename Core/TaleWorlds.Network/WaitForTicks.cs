using System;

namespace TaleWorlds.Network
{
	public class WaitForTicks : CoroutineState
	{
		internal int TickCount { get; private set; }

		public WaitForTicks(int tickCount)
		{
			this.TickCount = tickCount;
		}

		protected internal override void Initialize(CoroutineManager coroutineManager)
		{
			base.Initialize(coroutineManager);
			this._beginTick = coroutineManager.CurrentTick;
		}

		protected internal override bool IsFinished
		{
			get
			{
				return this._beginTick + this.TickCount >= base.CoroutineManager.CurrentTick;
			}
		}

		private int _beginTick;
	}
}
