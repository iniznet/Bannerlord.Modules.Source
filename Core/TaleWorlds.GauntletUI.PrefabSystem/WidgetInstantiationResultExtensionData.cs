using System;

namespace TaleWorlds.GauntletUI.PrefabSystem
{
	// Token: 0x0200001B RID: 27
	internal struct WidgetInstantiationResultExtensionData
	{
		// Token: 0x1700002E RID: 46
		// (get) Token: 0x060000C5 RID: 197 RVA: 0x00003FE0 File Offset: 0x000021E0
		// (set) Token: 0x060000C6 RID: 198 RVA: 0x00003FE8 File Offset: 0x000021E8
		public string Name { get; set; }

		// Token: 0x1700002F RID: 47
		// (get) Token: 0x060000C7 RID: 199 RVA: 0x00003FF1 File Offset: 0x000021F1
		// (set) Token: 0x060000C8 RID: 200 RVA: 0x00003FF9 File Offset: 0x000021F9
		public bool PassToChildWidgetCreation { get; set; }

		// Token: 0x17000030 RID: 48
		// (get) Token: 0x060000C9 RID: 201 RVA: 0x00004002 File Offset: 0x00002202
		// (set) Token: 0x060000CA RID: 202 RVA: 0x0000400A File Offset: 0x0000220A
		public object Data { get; set; }
	}
}
