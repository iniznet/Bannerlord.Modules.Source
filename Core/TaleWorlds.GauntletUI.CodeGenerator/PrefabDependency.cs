using System;

namespace TaleWorlds.GauntletUI.CodeGenerator
{
	// Token: 0x0200000B RID: 11
	public class PrefabDependency
	{
		// Token: 0x1700002A RID: 42
		// (get) Token: 0x06000077 RID: 119 RVA: 0x0000391E File Offset: 0x00001B1E
		// (set) Token: 0x06000078 RID: 120 RVA: 0x00003926 File Offset: 0x00001B26
		public string Type { get; private set; }

		// Token: 0x1700002B RID: 43
		// (get) Token: 0x06000079 RID: 121 RVA: 0x0000392F File Offset: 0x00001B2F
		// (set) Token: 0x0600007A RID: 122 RVA: 0x00003937 File Offset: 0x00001B37
		public string VariantName { get; private set; }

		// Token: 0x1700002C RID: 44
		// (get) Token: 0x0600007B RID: 123 RVA: 0x00003940 File Offset: 0x00001B40
		// (set) Token: 0x0600007C RID: 124 RVA: 0x00003948 File Offset: 0x00001B48
		public bool IsRoot { get; private set; }

		// Token: 0x1700002D RID: 45
		// (get) Token: 0x0600007D RID: 125 RVA: 0x00003951 File Offset: 0x00001B51
		// (set) Token: 0x0600007E RID: 126 RVA: 0x00003959 File Offset: 0x00001B59
		public WidgetTemplateGenerateContext WidgetTemplateGenerateContext { get; private set; }

		// Token: 0x0600007F RID: 127 RVA: 0x00003962 File Offset: 0x00001B62
		public PrefabDependency(string type, string variantName, bool isRoot, WidgetTemplateGenerateContext widgetTemplateGenerateContext)
		{
			this.Type = type;
			this.VariantName = variantName;
			this.IsRoot = isRoot;
			this.WidgetTemplateGenerateContext = widgetTemplateGenerateContext;
		}
	}
}
