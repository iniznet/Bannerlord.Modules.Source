using System;
using System.Numerics;

namespace TaleWorlds.GauntletUI.BaseTypes
{
	// Token: 0x02000052 RID: 82
	public class BasicContainer : Container
	{
		// Token: 0x06000538 RID: 1336 RVA: 0x00016DD0 File Offset: 0x00014FD0
		public BasicContainer(UIContext context)
			: base(context)
		{
		}

		// Token: 0x17000187 RID: 391
		// (get) Token: 0x06000539 RID: 1337 RVA: 0x00016DD9 File Offset: 0x00014FD9
		// (set) Token: 0x0600053A RID: 1338 RVA: 0x00016DE1 File Offset: 0x00014FE1
		public override Predicate<Widget> AcceptDropPredicate { get; set; }

		// Token: 0x0600053B RID: 1339 RVA: 0x00016DEA File Offset: 0x00014FEA
		public override Vector2 GetDropGizmoPosition(Vector2 draggedWidgetPosition)
		{
			throw new NotImplementedException();
		}

		// Token: 0x0600053C RID: 1340 RVA: 0x00016DF1 File Offset: 0x00014FF1
		public override int GetIndexForDrop(Vector2 draggedWidgetPosition)
		{
			throw new NotImplementedException();
		}

		// Token: 0x17000188 RID: 392
		// (get) Token: 0x0600053D RID: 1341 RVA: 0x00016DF8 File Offset: 0x00014FF8
		public override bool IsDragHovering { get; }

		// Token: 0x0600053E RID: 1342 RVA: 0x00016E00 File Offset: 0x00015000
		public override void OnChildSelected(Widget widget)
		{
			int num = -1;
			for (int i = 0; i < base.ChildCount; i++)
			{
				if (widget == base.GetChild(i))
				{
					num = i;
				}
			}
			base.IntValue = num;
		}
	}
}
