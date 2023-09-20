using System;

namespace TaleWorlds.Diamond
{
	// Token: 0x02000008 RID: 8
	[Serializable]
	public class GOGAccessObject
	{
		// Token: 0x17000004 RID: 4
		// (get) Token: 0x06000023 RID: 35 RVA: 0x000025D4 File Offset: 0x000007D4
		// (set) Token: 0x06000024 RID: 36 RVA: 0x000025DC File Offset: 0x000007DC
		public ulong GogId { get; private set; }

		// Token: 0x17000005 RID: 5
		// (get) Token: 0x06000025 RID: 37 RVA: 0x000025E5 File Offset: 0x000007E5
		// (set) Token: 0x06000026 RID: 38 RVA: 0x000025ED File Offset: 0x000007ED
		public string UserName { get; private set; }

		// Token: 0x17000006 RID: 6
		// (get) Token: 0x06000027 RID: 39 RVA: 0x000025F6 File Offset: 0x000007F6
		// (set) Token: 0x06000028 RID: 40 RVA: 0x000025FE File Offset: 0x000007FE
		public byte[] Ticket { get; private set; }

		// Token: 0x06000029 RID: 41 RVA: 0x00002607 File Offset: 0x00000807
		public GOGAccessObject(string userName, ulong gogId, byte[] ticket)
		{
			this.UserName = userName;
			this.GogId = gogId;
			this.Ticket = ticket;
		}
	}
}
