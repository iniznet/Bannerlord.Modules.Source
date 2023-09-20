using System;
using TaleWorlds.Library;

namespace TaleWorlds.GauntletUI.Data
{
	// Token: 0x0200000C RID: 12
	internal class ViewBindDataInfo
	{
		// Token: 0x17000026 RID: 38
		// (get) Token: 0x06000092 RID: 146 RVA: 0x00004341 File Offset: 0x00002541
		// (set) Token: 0x06000093 RID: 147 RVA: 0x00004349 File Offset: 0x00002549
		internal GauntletView Owner { get; private set; }

		// Token: 0x17000027 RID: 39
		// (get) Token: 0x06000094 RID: 148 RVA: 0x00004352 File Offset: 0x00002552
		// (set) Token: 0x06000095 RID: 149 RVA: 0x0000435A File Offset: 0x0000255A
		internal string Property { get; private set; }

		// Token: 0x17000028 RID: 40
		// (get) Token: 0x06000096 RID: 150 RVA: 0x00004363 File Offset: 0x00002563
		// (set) Token: 0x06000097 RID: 151 RVA: 0x0000436B File Offset: 0x0000256B
		internal BindingPath Path { get; private set; }

		// Token: 0x06000098 RID: 152 RVA: 0x00004374 File Offset: 0x00002574
		internal ViewBindDataInfo(GauntletView view, string property, BindingPath path)
		{
			this.Owner = view;
			this.Property = property;
			this.Path = path;
		}
	}
}
