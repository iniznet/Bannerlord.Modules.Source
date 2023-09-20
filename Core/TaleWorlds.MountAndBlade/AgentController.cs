using System;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x020000ED RID: 237
	public class AgentController
	{
		// Token: 0x17000299 RID: 665
		// (get) Token: 0x06000B2E RID: 2862 RVA: 0x000159E3 File Offset: 0x00013BE3
		// (set) Token: 0x06000B2F RID: 2863 RVA: 0x000159EB File Offset: 0x00013BEB
		public Agent Owner { get; set; }

		// Token: 0x1700029A RID: 666
		// (get) Token: 0x06000B30 RID: 2864 RVA: 0x000159F4 File Offset: 0x00013BF4
		// (set) Token: 0x06000B31 RID: 2865 RVA: 0x000159FC File Offset: 0x00013BFC
		public Mission Mission { get; set; }

		// Token: 0x06000B32 RID: 2866 RVA: 0x00015A05 File Offset: 0x00013C05
		public virtual void OnInitialize()
		{
		}
	}
}
