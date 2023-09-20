using System;

namespace TaleWorlds.DotNet
{
	// Token: 0x0200000D RID: 13
	public class EditableScriptComponentVariable : Attribute
	{
		// Token: 0x17000009 RID: 9
		// (get) Token: 0x06000032 RID: 50 RVA: 0x00002CB8 File Offset: 0x00000EB8
		// (set) Token: 0x06000033 RID: 51 RVA: 0x00002CC0 File Offset: 0x00000EC0
		public bool Visible { get; set; }

		// Token: 0x06000034 RID: 52 RVA: 0x00002CC9 File Offset: 0x00000EC9
		public EditableScriptComponentVariable(bool visible)
		{
			this.Visible = visible;
		}
	}
}
