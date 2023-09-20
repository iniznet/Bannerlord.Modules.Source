using System;
using TaleWorlds.Diamond.Rest;
using TaleWorlds.Library.Http;

namespace TaleWorlds.Diamond.ClientApplication
{
	// Token: 0x02000059 RID: 89
	public class GenericThreadedRestSessionProvider<T> : IClientSessionProvider<T> where T : Client<T>
	{
		// Token: 0x06000212 RID: 530 RVA: 0x00006389 File Offset: 0x00004589
		public GenericThreadedRestSessionProvider(string address, ushort port, bool isSecure, IHttpDriver httpDriver)
		{
			this._address = address;
			this._port = port;
			this._isSecure = isSecure;
			this._httpDriver = httpDriver;
		}

		// Token: 0x06000213 RID: 531 RVA: 0x000063B0 File Offset: 0x000045B0
		public IClientSession CreateSession(T client)
		{
			int num;
			if (!client.Application.Parameters.TryGetParameterAsInt("ThreadedClientSession.ThreadSleepTime", out num))
			{
				num = 100;
			}
			ThreadedClient threadedClient = new ThreadedClient(client);
			ClientRestSession clientRestSession = new ClientRestSession(threadedClient, this._address, this._port, this._isSecure, this._httpDriver);
			return new ThreadedClientSession(threadedClient, clientRestSession, num);
		}

		// Token: 0x040000C4 RID: 196
		public const int DefaultThreadSleepTime = 100;

		// Token: 0x040000C5 RID: 197
		private string _address;

		// Token: 0x040000C6 RID: 198
		private ushort _port;

		// Token: 0x040000C7 RID: 199
		private bool _isSecure;

		// Token: 0x040000C8 RID: 200
		private IHttpDriver _httpDriver;
	}
}
