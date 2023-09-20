using System;
using TaleWorlds.Diamond.InnerProcess;

namespace TaleWorlds.Diamond.ClientApplication
{
	// Token: 0x02000057 RID: 87
	public class GenericInnerProcessSessionProvider<T> : IClientSessionProvider<T> where T : Client<T>
	{
		// Token: 0x0600020E RID: 526 RVA: 0x00006310 File Offset: 0x00004510
		public GenericInnerProcessSessionProvider(InnerProcessManager innerProcessManager, ushort port)
		{
			this._innerProcessManager = innerProcessManager;
			this._port = port;
		}

		// Token: 0x0600020F RID: 527 RVA: 0x00006326 File Offset: 0x00004526
		public IClientSession CreateSession(T session)
		{
			return new InnerProcessClient(this._innerProcessManager, session, (int)this._port);
		}

		// Token: 0x040000BE RID: 190
		private InnerProcessManager _innerProcessManager;

		// Token: 0x040000BF RID: 191
		private ushort _port;
	}
}
