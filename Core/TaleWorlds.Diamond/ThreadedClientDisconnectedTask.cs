using System;

namespace TaleWorlds.Diamond
{
	// Token: 0x02000024 RID: 36
	internal sealed class ThreadedClientDisconnectedTask : ThreadedClientTask
	{
		// Token: 0x060000A9 RID: 169 RVA: 0x00002FBF File Offset: 0x000011BF
		public ThreadedClientDisconnectedTask(IClient client)
			: base(client)
		{
		}

		// Token: 0x060000AA RID: 170 RVA: 0x00002FC8 File Offset: 0x000011C8
		public override void DoJob()
		{
			base.Client.OnDisconnected();
		}
	}
}
