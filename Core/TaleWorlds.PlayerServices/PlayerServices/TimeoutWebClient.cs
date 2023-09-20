using System;
using System.Net;

namespace TaleWorlds.PlayerServices
{
	// Token: 0x02000005 RID: 5
	public class TimeoutWebClient : WebClient
	{
		// Token: 0x06000023 RID: 35 RVA: 0x00002929 File Offset: 0x00000B29
		public TimeoutWebClient()
		{
			this.Timeout = 15000;
		}

		// Token: 0x06000024 RID: 36 RVA: 0x0000293C File Offset: 0x00000B3C
		public TimeoutWebClient(int timeout)
		{
			this.Timeout = timeout;
		}

		// Token: 0x1700000B RID: 11
		// (get) Token: 0x06000025 RID: 37 RVA: 0x0000294B File Offset: 0x00000B4B
		// (set) Token: 0x06000026 RID: 38 RVA: 0x00002953 File Offset: 0x00000B53
		public int Timeout { get; set; }

		// Token: 0x06000027 RID: 39 RVA: 0x0000295C File Offset: 0x00000B5C
		protected override WebRequest GetWebRequest(Uri address)
		{
			WebRequest webRequest = base.GetWebRequest(address);
			webRequest.Timeout = this.Timeout;
			return webRequest;
		}
	}
}
