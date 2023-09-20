using System;

namespace TaleWorlds.Diamond
{
	// Token: 0x02000021 RID: 33
	internal abstract class ThreadedClientTask
	{
		// Token: 0x17000023 RID: 35
		// (get) Token: 0x0600009F RID: 159 RVA: 0x00002F55 File Offset: 0x00001155
		// (set) Token: 0x060000A0 RID: 160 RVA: 0x00002F5D File Offset: 0x0000115D
		public IClient Client { get; private set; }

		// Token: 0x060000A1 RID: 161 RVA: 0x00002F66 File Offset: 0x00001166
		protected ThreadedClientTask(IClient client)
		{
			this.Client = client;
		}

		// Token: 0x060000A2 RID: 162
		public abstract void DoJob();
	}
}
