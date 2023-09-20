using System;

namespace TaleWorlds.Diamond
{
	// Token: 0x02000029 RID: 41
	internal sealed class ThreadedClientSessionDisconnectTask : ThreadedClientSessionTask
	{
		// Token: 0x060000C3 RID: 195 RVA: 0x000033A2 File Offset: 0x000015A2
		public ThreadedClientSessionDisconnectTask(IClientSession session)
			: base(session)
		{
		}

		// Token: 0x060000C4 RID: 196 RVA: 0x000033AB File Offset: 0x000015AB
		public override void BeginJob()
		{
			base.Session.Disconnect();
		}

		// Token: 0x060000C5 RID: 197 RVA: 0x000033B8 File Offset: 0x000015B8
		public override void DoMainThreadJob()
		{
			base.Finished = true;
		}
	}
}
