using System;
using TaleWorlds.Library;

namespace TaleWorlds.GauntletUI.CodeGenerator
{
	// Token: 0x02000012 RID: 18
	public class WidgetCodeGenerationInfoChildSearchResult
	{
		// Token: 0x1700003E RID: 62
		// (get) Token: 0x060000F1 RID: 241 RVA: 0x00007E3A File Offset: 0x0000603A
		// (set) Token: 0x060000F2 RID: 242 RVA: 0x00007E42 File Offset: 0x00006042
		public WidgetCodeGenerationInfo FoundWidget { get; set; }

		// Token: 0x1700003F RID: 63
		// (get) Token: 0x060000F3 RID: 243 RVA: 0x00007E4B File Offset: 0x0000604B
		// (set) Token: 0x060000F4 RID: 244 RVA: 0x00007E53 File Offset: 0x00006053
		public BindingPath RemainingPath { get; internal set; }

		// Token: 0x17000040 RID: 64
		// (get) Token: 0x060000F5 RID: 245 RVA: 0x00007E5C File Offset: 0x0000605C
		// (set) Token: 0x060000F6 RID: 246 RVA: 0x00007E64 File Offset: 0x00006064
		public WidgetCodeGenerationInfo ReachedWidget { get; internal set; }
	}
}
