using System;

namespace TaleWorlds.SaveSystem
{
	// Token: 0x02000009 RID: 9
	[AttributeUsage(AttributeTargets.Property)]
	public class SaveablePropertyAttribute : Attribute
	{
		// Token: 0x17000003 RID: 3
		// (get) Token: 0x06000015 RID: 21 RVA: 0x00002241 File Offset: 0x00000441
		// (set) Token: 0x06000016 RID: 22 RVA: 0x00002249 File Offset: 0x00000449
		public short LocalSaveId { get; set; }

		// Token: 0x06000017 RID: 23 RVA: 0x00002252 File Offset: 0x00000452
		public SaveablePropertyAttribute(short localSaveId)
		{
			this.LocalSaveId = localSaveId;
		}
	}
}
