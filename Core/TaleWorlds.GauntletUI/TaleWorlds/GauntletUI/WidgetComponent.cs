using System;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.GauntletUI
{
	// Token: 0x02000032 RID: 50
	public abstract class WidgetComponent
	{
		// Token: 0x17000115 RID: 277
		// (get) Token: 0x0600035D RID: 861 RVA: 0x0000EA45 File Offset: 0x0000CC45
		// (set) Token: 0x0600035E RID: 862 RVA: 0x0000EA4D File Offset: 0x0000CC4D
		public Widget Target { get; private set; }

		// Token: 0x0600035F RID: 863 RVA: 0x0000EA56 File Offset: 0x0000CC56
		protected WidgetComponent(Widget target)
		{
			this.Target = target;
		}
	}
}
