using System;

namespace TaleWorlds.MountAndBlade.View
{
	// Token: 0x0200001B RID: 27
	public class OverrideView : Attribute
	{
		// Token: 0x1700000C RID: 12
		// (get) Token: 0x060000DB RID: 219 RVA: 0x0000772D File Offset: 0x0000592D
		// (set) Token: 0x060000DC RID: 220 RVA: 0x00007735 File Offset: 0x00005935
		public Type BaseType { get; private set; }

		// Token: 0x060000DD RID: 221 RVA: 0x0000773E File Offset: 0x0000593E
		public OverrideView(Type baseType)
		{
			this.BaseType = baseType;
		}
	}
}
