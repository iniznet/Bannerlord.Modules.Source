using System;

namespace TaleWorlds.Diamond
{
	// Token: 0x02000025 RID: 37
	internal sealed class ThreadedClientCantConnectTask : ThreadedClientTask
	{
		// Token: 0x060000AB RID: 171 RVA: 0x00002FD5 File Offset: 0x000011D5
		public ThreadedClientCantConnectTask(IClient client)
			: base(client)
		{
		}

		// Token: 0x060000AC RID: 172 RVA: 0x00002FDE File Offset: 0x000011DE
		public override void DoJob()
		{
			base.Client.OnCantConnect();
		}
	}
}
