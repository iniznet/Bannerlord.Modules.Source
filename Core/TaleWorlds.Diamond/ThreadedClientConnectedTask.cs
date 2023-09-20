using System;

namespace TaleWorlds.Diamond
{
	// Token: 0x02000023 RID: 35
	internal sealed class ThreadedClientConnectedTask : ThreadedClientTask
	{
		// Token: 0x060000A7 RID: 167 RVA: 0x00002FA9 File Offset: 0x000011A9
		public ThreadedClientConnectedTask(IClient client)
			: base(client)
		{
		}

		// Token: 0x060000A8 RID: 168 RVA: 0x00002FB2 File Offset: 0x000011B2
		public override void DoJob()
		{
			base.Client.OnConnected();
		}
	}
}
