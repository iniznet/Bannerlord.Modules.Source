using System;
using TaleWorlds.Diamond.Rest;
using TaleWorlds.Library.Http;

namespace TaleWorlds.Diamond.ClientApplication
{
	// Token: 0x0200005B RID: 91
	public class GenericRestSessionlessClientDriverProvider<T> : ISessionlessClientDriverProvider<T> where T : SessionlessClient<T>
	{
		// Token: 0x06000217 RID: 535 RVA: 0x00006430 File Offset: 0x00004630
		public GenericRestSessionlessClientDriverProvider(string address, ushort port, bool isSecure, IHttpDriver httpDriver)
		{
			this._address = address;
			this._port = port;
			this._isSecure = isSecure;
			this._httpDriver = httpDriver;
		}

		// Token: 0x06000218 RID: 536 RVA: 0x00006455 File Offset: 0x00004655
		public ISessionlessClientDriver CreateDriver(T sessionlessClient)
		{
			return new SessionlessClientRestDriver(this._address, this._port, this._isSecure, this._httpDriver);
		}

		// Token: 0x040000CA RID: 202
		private string _address;

		// Token: 0x040000CB RID: 203
		private ushort _port;

		// Token: 0x040000CC RID: 204
		private bool _isSecure;

		// Token: 0x040000CD RID: 205
		private IHttpDriver _httpDriver;
	}
}
