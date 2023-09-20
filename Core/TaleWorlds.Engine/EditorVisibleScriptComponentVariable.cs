using System;

namespace TaleWorlds.Engine
{
	// Token: 0x02000081 RID: 129
	public class EditorVisibleScriptComponentVariable : Attribute
	{
		// Token: 0x17000079 RID: 121
		// (get) Token: 0x060009D7 RID: 2519 RVA: 0x0000A91F File Offset: 0x00008B1F
		// (set) Token: 0x060009D8 RID: 2520 RVA: 0x0000A927 File Offset: 0x00008B27
		public bool Visible { get; set; }

		// Token: 0x060009D9 RID: 2521 RVA: 0x0000A930 File Offset: 0x00008B30
		public EditorVisibleScriptComponentVariable(bool visible)
		{
			this.Visible = visible;
		}
	}
}
