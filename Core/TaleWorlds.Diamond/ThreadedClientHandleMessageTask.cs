using System;

namespace TaleWorlds.Diamond
{
	// Token: 0x02000022 RID: 34
	internal sealed class ThreadedClientHandleMessageTask : ThreadedClientTask
	{
		// Token: 0x17000024 RID: 36
		// (get) Token: 0x060000A3 RID: 163 RVA: 0x00002F75 File Offset: 0x00001175
		// (set) Token: 0x060000A4 RID: 164 RVA: 0x00002F7D File Offset: 0x0000117D
		public Message Message { get; private set; }

		// Token: 0x060000A5 RID: 165 RVA: 0x00002F86 File Offset: 0x00001186
		public ThreadedClientHandleMessageTask(IClient client, Message message)
			: base(client)
		{
			this.Message = message;
		}

		// Token: 0x060000A6 RID: 166 RVA: 0x00002F96 File Offset: 0x00001196
		public override void DoJob()
		{
			base.Client.HandleMessage(this.Message);
		}
	}
}
