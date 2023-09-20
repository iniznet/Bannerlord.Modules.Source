using System;

namespace TaleWorlds.Diamond
{
	// Token: 0x02000028 RID: 40
	internal sealed class ThreadedClientSessionConnectTask : ThreadedClientSessionTask
	{
		// Token: 0x060000C0 RID: 192 RVA: 0x00003383 File Offset: 0x00001583
		public ThreadedClientSessionConnectTask(IClientSession session)
			: base(session)
		{
		}

		// Token: 0x060000C1 RID: 193 RVA: 0x0000338C File Offset: 0x0000158C
		public override void BeginJob()
		{
			base.Session.Connect();
		}

		// Token: 0x060000C2 RID: 194 RVA: 0x00003399 File Offset: 0x00001599
		public override void DoMainThreadJob()
		{
			base.Finished = true;
		}
	}
}
