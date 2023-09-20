using System;

namespace TaleWorlds.Diamond
{
	// Token: 0x02000027 RID: 39
	internal abstract class ThreadedClientSessionTask
	{
		// Token: 0x17000025 RID: 37
		// (get) Token: 0x060000B9 RID: 185 RVA: 0x00003352 File Offset: 0x00001552
		// (set) Token: 0x060000BA RID: 186 RVA: 0x0000335A File Offset: 0x0000155A
		public IClientSession Session { get; private set; }

		// Token: 0x17000026 RID: 38
		// (get) Token: 0x060000BB RID: 187 RVA: 0x00003363 File Offset: 0x00001563
		// (set) Token: 0x060000BC RID: 188 RVA: 0x0000336B File Offset: 0x0000156B
		public bool Finished { get; protected set; }

		// Token: 0x060000BD RID: 189 RVA: 0x00003374 File Offset: 0x00001574
		protected ThreadedClientSessionTask(IClientSession session)
		{
			this.Session = session;
		}

		// Token: 0x060000BE RID: 190
		public abstract void BeginJob();

		// Token: 0x060000BF RID: 191
		public abstract void DoMainThreadJob();
	}
}
