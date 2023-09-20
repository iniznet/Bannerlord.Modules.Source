using System;
using TaleWorlds.Diamond.Rest;
using TaleWorlds.Library.Http;

namespace TaleWorlds.Diamond.ClientApplication
{
	public class GenericRestSessionlessClientDriverProvider<T> : ISessionlessClientDriverProvider<T> where T : SessionlessClient<T>
	{
		public GenericRestSessionlessClientDriverProvider(string address, ushort port, bool isSecure, IHttpDriver httpDriver)
		{
			this._address = address;
			this._port = port;
			this._isSecure = isSecure;
			this._httpDriver = httpDriver;
		}

		public ISessionlessClientDriver CreateDriver(T sessionlessClient)
		{
			return new SessionlessClientRestDriver(this._address, this._port, this._isSecure, this._httpDriver);
		}

		private string _address;

		private ushort _port;

		private bool _isSecure;

		private IHttpDriver _httpDriver;
	}
}
