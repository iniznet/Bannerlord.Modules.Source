using System;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.GauntletUI.ExtraWidgets.Graph
{
	// Token: 0x02000013 RID: 19
	public class GraphLinePointWidget : BrushWidget
	{
		// Token: 0x1700006A RID: 106
		// (get) Token: 0x06000110 RID: 272 RVA: 0x000065DE File Offset: 0x000047DE
		// (set) Token: 0x06000111 RID: 273 RVA: 0x000065E6 File Offset: 0x000047E6
		public float HorizontalValue { get; set; }

		// Token: 0x1700006B RID: 107
		// (get) Token: 0x06000112 RID: 274 RVA: 0x000065EF File Offset: 0x000047EF
		// (set) Token: 0x06000113 RID: 275 RVA: 0x000065F7 File Offset: 0x000047F7
		public float VerticalValue { get; set; }

		// Token: 0x06000114 RID: 276 RVA: 0x00006600 File Offset: 0x00004800
		public GraphLinePointWidget(UIContext context)
			: base(context)
		{
		}
	}
}
