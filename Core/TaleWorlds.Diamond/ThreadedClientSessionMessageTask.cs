using System;

namespace TaleWorlds.Diamond
{
	// Token: 0x0200002B RID: 43
	internal sealed class ThreadedClientSessionMessageTask : ThreadedClientSessionTask
	{
		// Token: 0x17000028 RID: 40
		// (get) Token: 0x060000CD RID: 205 RVA: 0x000034A9 File Offset: 0x000016A9
		// (set) Token: 0x060000CE RID: 206 RVA: 0x000034B1 File Offset: 0x000016B1
		public Message Message { get; private set; }

		// Token: 0x060000CF RID: 207 RVA: 0x000034BA File Offset: 0x000016BA
		public ThreadedClientSessionMessageTask(IClientSession session, Message message)
			: base(session)
		{
			this.Message = message;
		}

		// Token: 0x060000D0 RID: 208 RVA: 0x000034CA File Offset: 0x000016CA
		public override void BeginJob()
		{
			base.Session.SendMessage(this.Message);
		}

		// Token: 0x060000D1 RID: 209 RVA: 0x000034DD File Offset: 0x000016DD
		public override void DoMainThreadJob()
		{
			base.Finished = true;
		}
	}
}
