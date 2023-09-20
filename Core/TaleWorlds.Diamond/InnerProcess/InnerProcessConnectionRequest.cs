using System;

namespace TaleWorlds.Diamond.InnerProcess
{
	// Token: 0x0200004A RID: 74
	internal class InnerProcessConnectionRequest
	{
		// Token: 0x17000054 RID: 84
		// (get) Token: 0x060001A9 RID: 425 RVA: 0x000053A0 File Offset: 0x000035A0
		// (set) Token: 0x060001AA RID: 426 RVA: 0x000053A8 File Offset: 0x000035A8
		public IInnerProcessClient Client { get; private set; }

		// Token: 0x17000055 RID: 85
		// (get) Token: 0x060001AB RID: 427 RVA: 0x000053B1 File Offset: 0x000035B1
		// (set) Token: 0x060001AC RID: 428 RVA: 0x000053B9 File Offset: 0x000035B9
		public int Port { get; private set; }

		// Token: 0x060001AD RID: 429 RVA: 0x000053C2 File Offset: 0x000035C2
		public InnerProcessConnectionRequest(IInnerProcessClient client, int port)
		{
			this.Client = client;
			this.Port = port;
		}
	}
}
