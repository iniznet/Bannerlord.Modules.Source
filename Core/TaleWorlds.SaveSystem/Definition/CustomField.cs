using System;

namespace TaleWorlds.SaveSystem.Definition
{
	// Token: 0x02000069 RID: 105
	public class CustomField
	{
		// Token: 0x1700008A RID: 138
		// (get) Token: 0x0600033A RID: 826 RVA: 0x0000E584 File Offset: 0x0000C784
		// (set) Token: 0x0600033B RID: 827 RVA: 0x0000E58C File Offset: 0x0000C78C
		public string Name { get; private set; }

		// Token: 0x1700008B RID: 139
		// (get) Token: 0x0600033C RID: 828 RVA: 0x0000E595 File Offset: 0x0000C795
		// (set) Token: 0x0600033D RID: 829 RVA: 0x0000E59D File Offset: 0x0000C79D
		public short SaveId { get; private set; }

		// Token: 0x0600033E RID: 830 RVA: 0x0000E5A6 File Offset: 0x0000C7A6
		public CustomField(string name, short saveId)
		{
			this.Name = name;
			this.SaveId = saveId;
		}
	}
}
