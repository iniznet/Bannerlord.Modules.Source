using System;
using TaleWorlds.Diamond.Rest;
using TaleWorlds.Library.Http;

namespace TaleWorlds.Diamond.ClientApplication
{
	// Token: 0x02000058 RID: 88
	public class GenericRestSessionProvider<T> : IClientSessionProvider<T> where T : Client<T>
	{
		// Token: 0x06000210 RID: 528 RVA: 0x0000633F File Offset: 0x0000453F
		public GenericRestSessionProvider(string address, ushort port, bool isSecure, IHttpDriver httpDriver)
		{
			this._address = address;
			this._port = port;
			this._isSecure = isSecure;
			this._httpDriver = httpDriver;
		}

		// Token: 0x06000211 RID: 529 RVA: 0x00006364 File Offset: 0x00004564
		public IClientSession CreateSession(T session)
		{
			return new ClientRestSession(session, this._address, this._port, this._isSecure, this._httpDriver);
		}

		// Token: 0x040000C0 RID: 192
		private string _address;

		// Token: 0x040000C1 RID: 193
		private ushort _port;

		// Token: 0x040000C2 RID: 194
		private bool _isSecure;

		// Token: 0x040000C3 RID: 195
		private IHttpDriver _httpDriver;
	}
}
