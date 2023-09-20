using System;
using Galaxy.Api;

namespace TaleWorlds.Diamond.AccessProvider.GOG
{
	// Token: 0x02000003 RID: 3
	internal class EncryptedAppTicketListener : IEncryptedAppTicketListener
	{
		// Token: 0x17000001 RID: 1
		// (get) Token: 0x06000006 RID: 6 RVA: 0x0000216A File Offset: 0x0000036A
		// (set) Token: 0x06000007 RID: 7 RVA: 0x00002172 File Offset: 0x00000372
		public bool GotResult { get; private set; }

		// Token: 0x06000008 RID: 8 RVA: 0x0000217B File Offset: 0x0000037B
		public override void OnEncryptedAppTicketRetrieveFailure(IEncryptedAppTicketListener.FailureReason failureReason)
		{
			this.GotResult = true;
		}

		// Token: 0x06000009 RID: 9 RVA: 0x00002184 File Offset: 0x00000384
		public override void OnEncryptedAppTicketRetrieveSuccess()
		{
			this.GotResult = true;
		}
	}
}
