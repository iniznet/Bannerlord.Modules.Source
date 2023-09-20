using System;

namespace TaleWorlds.Library
{
	// Token: 0x0200009F RID: 159
	public class VirtualFileAttribute : Attribute
	{
		// Token: 0x170000A7 RID: 167
		// (get) Token: 0x060005D1 RID: 1489 RVA: 0x00012A66 File Offset: 0x00010C66
		// (set) Token: 0x060005D2 RID: 1490 RVA: 0x00012A6E File Offset: 0x00010C6E
		public string Name { get; private set; }

		// Token: 0x170000A8 RID: 168
		// (get) Token: 0x060005D3 RID: 1491 RVA: 0x00012A77 File Offset: 0x00010C77
		// (set) Token: 0x060005D4 RID: 1492 RVA: 0x00012A7F File Offset: 0x00010C7F
		public string Content { get; private set; }

		// Token: 0x060005D5 RID: 1493 RVA: 0x00012A88 File Offset: 0x00010C88
		public VirtualFileAttribute(string name, string content)
		{
			this.Name = name;
			this.Content = content;
		}
	}
}
