using System;
using System.Threading;

namespace TaleWorlds.Library
{
	public static class SingleThreadedSynchronizationContextManager
	{
		public static void Initialize()
		{
			if (SingleThreadedSynchronizationContextManager._synchronizationContext == null)
			{
				SingleThreadedSynchronizationContextManager._synchronizationContext = new SingleThreadedSynchronizationContext();
				SynchronizationContext.SetSynchronizationContext(SingleThreadedSynchronizationContextManager._synchronizationContext);
			}
		}

		public static void Tick()
		{
			SingleThreadedSynchronizationContextManager._synchronizationContext.Tick();
		}

		private static SingleThreadedSynchronizationContext _synchronizationContext;
	}
}
