using System;

namespace TaleWorlds.SaveSystem
{
	// Token: 0x0200000A RID: 10
	[AttributeUsage(AttributeTargets.Class)]
	public class SaveableRootClassAttribute : Attribute
	{
		// Token: 0x17000004 RID: 4
		// (get) Token: 0x06000018 RID: 24 RVA: 0x00002261 File Offset: 0x00000461
		// (set) Token: 0x06000019 RID: 25 RVA: 0x00002269 File Offset: 0x00000469
		public int SaveId { get; set; }

		// Token: 0x0600001A RID: 26 RVA: 0x00002272 File Offset: 0x00000472
		public SaveableRootClassAttribute(int saveId)
		{
			this.SaveId = saveId;
		}
	}
}
