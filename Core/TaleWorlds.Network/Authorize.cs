using System;

namespace TaleWorlds.Network
{
	// Token: 0x0200000B RID: 11
	public class Authorize : Attribute
	{
		// Token: 0x1700000F RID: 15
		// (get) Token: 0x0600003D RID: 61 RVA: 0x00002874 File Offset: 0x00000A74
		// (set) Token: 0x0600003E RID: 62 RVA: 0x0000287C File Offset: 0x00000A7C
		public string Users { get; set; }

		// Token: 0x17000010 RID: 16
		// (get) Token: 0x0600003F RID: 63 RVA: 0x00002885 File Offset: 0x00000A85
		// (set) Token: 0x06000040 RID: 64 RVA: 0x0000288D File Offset: 0x00000A8D
		public string Roles { get; set; }
	}
}
