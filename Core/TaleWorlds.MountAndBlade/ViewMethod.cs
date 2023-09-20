using System;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x020002CF RID: 719
	public class ViewMethod : Attribute
	{
		// Token: 0x17000723 RID: 1827
		// (get) Token: 0x0600275A RID: 10074 RVA: 0x000973A7 File Offset: 0x000955A7
		// (set) Token: 0x0600275B RID: 10075 RVA: 0x000973AF File Offset: 0x000955AF
		public string Name { get; private set; }

		// Token: 0x0600275C RID: 10076 RVA: 0x000973B8 File Offset: 0x000955B8
		public ViewMethod(string name)
		{
			this.Name = name;
		}
	}
}
