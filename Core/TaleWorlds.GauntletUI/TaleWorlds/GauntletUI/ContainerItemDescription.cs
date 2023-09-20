using System;

namespace TaleWorlds.GauntletUI
{
	// Token: 0x02000019 RID: 25
	public class ContainerItemDescription
	{
		// Token: 0x1700009B RID: 155
		// (get) Token: 0x060001D8 RID: 472 RVA: 0x0000AD1D File Offset: 0x00008F1D
		// (set) Token: 0x060001D9 RID: 473 RVA: 0x0000AD25 File Offset: 0x00008F25
		public string WidgetId { get; set; }

		// Token: 0x1700009C RID: 156
		// (get) Token: 0x060001DA RID: 474 RVA: 0x0000AD2E File Offset: 0x00008F2E
		// (set) Token: 0x060001DB RID: 475 RVA: 0x0000AD36 File Offset: 0x00008F36
		public int WidgetIndex { get; set; }

		// Token: 0x1700009D RID: 157
		// (get) Token: 0x060001DC RID: 476 RVA: 0x0000AD3F File Offset: 0x00008F3F
		// (set) Token: 0x060001DD RID: 477 RVA: 0x0000AD47 File Offset: 0x00008F47
		public float WidthStretchRatio { get; set; }

		// Token: 0x1700009E RID: 158
		// (get) Token: 0x060001DE RID: 478 RVA: 0x0000AD50 File Offset: 0x00008F50
		// (set) Token: 0x060001DF RID: 479 RVA: 0x0000AD58 File Offset: 0x00008F58
		public float HeightStretchRatio { get; set; }

		// Token: 0x060001E0 RID: 480 RVA: 0x0000AD61 File Offset: 0x00008F61
		public ContainerItemDescription()
		{
			this.WidgetId = "";
			this.WidgetIndex = -1;
			this.WidthStretchRatio = 1f;
			this.HeightStretchRatio = 1f;
		}
	}
}
