using System;

namespace TaleWorlds.Diamond
{
	internal abstract class ThreadedClientSessionTask
	{
		public IClientSession Session { get; private set; }

		public bool Finished { get; protected set; }

		protected ThreadedClientSessionTask(IClientSession session)
		{
			this.Session = session;
		}

		public abstract void BeginJob();

		public abstract void DoMainThreadJob();
	}
}
