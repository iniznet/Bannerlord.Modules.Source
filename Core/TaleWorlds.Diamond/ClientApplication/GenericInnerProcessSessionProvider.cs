using System;
using TaleWorlds.Diamond.InnerProcess;

namespace TaleWorlds.Diamond.ClientApplication
{
	public class GenericInnerProcessSessionProvider<T> : IClientSessionProvider<T> where T : Client<T>
	{
		public GenericInnerProcessSessionProvider(InnerProcessManager innerProcessManager, ushort port)
		{
			this._innerProcessManager = innerProcessManager;
			this._port = port;
		}

		public IClientSession CreateSession(T session)
		{
			return new InnerProcessClient(this._innerProcessManager, session, (int)this._port);
		}

		private InnerProcessManager _innerProcessManager;

		private ushort _port;
	}
}
