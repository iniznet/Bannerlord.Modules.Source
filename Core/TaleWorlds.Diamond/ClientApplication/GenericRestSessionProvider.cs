using System;
using TaleWorlds.Diamond.Rest;
using TaleWorlds.Library.Http;

namespace TaleWorlds.Diamond.ClientApplication
{
	public class GenericRestSessionProvider<T> : IClientSessionProvider<T> where T : Client<T>
	{
		public GenericRestSessionProvider(string address, ushort port, bool isSecure, IHttpDriver httpDriver)
		{
			this._address = address;
			this._port = port;
			this._isSecure = isSecure;
			this._httpDriver = httpDriver;
		}

		public IClientSession CreateSession(T session)
		{
			return new ClientRestSession(session, this._address, this._port, this._isSecure, this._httpDriver);
		}

		private string _address;

		private ushort _port;

		private bool _isSecure;

		private IHttpDriver _httpDriver;
	}
}
