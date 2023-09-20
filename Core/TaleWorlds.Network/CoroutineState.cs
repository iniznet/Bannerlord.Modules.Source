using System;

namespace TaleWorlds.Network
{
	public abstract class CoroutineState
	{
		private protected CoroutineManager CoroutineManager { protected get; private set; }

		protected internal virtual void Initialize(CoroutineManager coroutineManager)
		{
			this.CoroutineManager = coroutineManager;
		}

		protected internal abstract bool IsFinished { get; }
	}
}
