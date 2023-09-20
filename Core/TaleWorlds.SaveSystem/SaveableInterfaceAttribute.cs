using System;

namespace TaleWorlds.SaveSystem
{
	// Token: 0x02000008 RID: 8
	[AttributeUsage(AttributeTargets.Interface)]
	public class SaveableInterfaceAttribute : Attribute
	{
		// Token: 0x17000002 RID: 2
		// (get) Token: 0x06000012 RID: 18 RVA: 0x00002221 File Offset: 0x00000421
		// (set) Token: 0x06000013 RID: 19 RVA: 0x00002229 File Offset: 0x00000429
		public int SaveId { get; set; }

		// Token: 0x06000014 RID: 20 RVA: 0x00002232 File Offset: 0x00000432
		public SaveableInterfaceAttribute(int saveId)
		{
			this.SaveId = saveId;
		}
	}
}
