using System;

namespace TaleWorlds.Library.Http
{
	// Token: 0x020000AA RID: 170
	public interface IHttpRequestTask
	{
		// Token: 0x170000BC RID: 188
		// (get) Token: 0x06000624 RID: 1572
		HttpRequestTaskState State { get; }

		// Token: 0x170000BD RID: 189
		// (get) Token: 0x06000625 RID: 1573
		bool Successful { get; }

		// Token: 0x170000BE RID: 190
		// (get) Token: 0x06000626 RID: 1574
		string ResponseData { get; }

		// Token: 0x170000BF RID: 191
		// (get) Token: 0x06000627 RID: 1575
		Exception Exception { get; }

		// Token: 0x06000628 RID: 1576
		void Start();
	}
}
