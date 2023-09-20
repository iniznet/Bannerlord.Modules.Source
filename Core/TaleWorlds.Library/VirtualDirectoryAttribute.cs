using System;

namespace TaleWorlds.Library
{
	// Token: 0x0200009E RID: 158
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
	public class VirtualDirectoryAttribute : Attribute
	{
		// Token: 0x170000A6 RID: 166
		// (get) Token: 0x060005CE RID: 1486 RVA: 0x00012A46 File Offset: 0x00010C46
		// (set) Token: 0x060005CF RID: 1487 RVA: 0x00012A4E File Offset: 0x00010C4E
		public string Name { get; private set; }

		// Token: 0x060005D0 RID: 1488 RVA: 0x00012A57 File Offset: 0x00010C57
		public VirtualDirectoryAttribute(string name)
		{
			this.Name = name;
		}
	}
}
