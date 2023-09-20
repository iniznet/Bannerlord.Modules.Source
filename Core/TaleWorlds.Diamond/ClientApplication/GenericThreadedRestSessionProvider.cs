using System;
using TaleWorlds.Diamond.Rest;
using TaleWorlds.Library.Http;

namespace TaleWorlds.Diamond.ClientApplication
{
	public class GenericThreadedRestSessionProvider<T> : IClientSessionProvider<T> where T : Client<T>
	{
		public GenericThreadedRestSessionProvider(string address, ushort port, bool isSecure, IHttpDriver httpDriver)
		{
			this._address = address;
			this._port = port;
			this._isSecure = isSecure;
			this._httpDriver = httpDriver;
		}

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

		public const int DefaultThreadSleepTime = 100;

		private string _address;

		private ushort _port;

		private bool _isSecure;

		private IHttpDriver _httpDriver;
	}
}
