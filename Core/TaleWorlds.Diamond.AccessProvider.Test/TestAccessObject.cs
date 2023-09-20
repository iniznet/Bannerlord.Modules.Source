using System;

namespace TaleWorlds.Diamond.AccessProvider.Test
{
	// Token: 0x02000003 RID: 3
	[Serializable]
	public class TestAccessObject
	{
		// Token: 0x17000001 RID: 1
		// (get) Token: 0x06000008 RID: 8 RVA: 0x000020FF File Offset: 0x000002FF
		// (set) Token: 0x06000009 RID: 9 RVA: 0x00002107 File Offset: 0x00000307
		public string UserName { get; private set; }

		// Token: 0x17000002 RID: 2
		// (get) Token: 0x0600000A RID: 10 RVA: 0x00002110 File Offset: 0x00000310
		// (set) Token: 0x0600000B RID: 11 RVA: 0x00002118 File Offset: 0x00000318
		public string Password { get; private set; }

		// Token: 0x0600000C RID: 12 RVA: 0x00002121 File Offset: 0x00000321
		public TestAccessObject(string userName, string password)
		{
			this.UserName = userName;
			this.Password = password;
		}
	}
}
