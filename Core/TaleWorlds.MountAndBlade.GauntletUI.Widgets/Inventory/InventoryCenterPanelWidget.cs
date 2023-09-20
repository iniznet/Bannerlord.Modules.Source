using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Inventory
{
	// Token: 0x0200011B RID: 283
	public class InventoryCenterPanelWidget : Widget
	{
		// Token: 0x06000E60 RID: 3680 RVA: 0x00027E67 File Offset: 0x00026067
		public InventoryCenterPanelWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x06000E61 RID: 3681 RVA: 0x00027E70 File Offset: 0x00026070
		protected override bool OnPreviewDragBegin()
		{
			return false;
		}

		// Token: 0x06000E62 RID: 3682 RVA: 0x00027E73 File Offset: 0x00026073
		protected override bool OnPreviewDrop()
		{
			return true;
		}

		// Token: 0x06000E63 RID: 3683 RVA: 0x00027E76 File Offset: 0x00026076
		protected override bool OnPreviewDragHover()
		{
			return false;
		}

		// Token: 0x06000E64 RID: 3684 RVA: 0x00027E79 File Offset: 0x00026079
		protected override bool OnPreviewMouseMove()
		{
			return false;
		}

		// Token: 0x06000E65 RID: 3685 RVA: 0x00027E7C File Offset: 0x0002607C
		protected override bool OnPreviewMousePressed()
		{
			return false;
		}

		// Token: 0x06000E66 RID: 3686 RVA: 0x00027E7F File Offset: 0x0002607F
		protected override bool OnPreviewMouseReleased()
		{
			return false;
		}

		// Token: 0x06000E67 RID: 3687 RVA: 0x00027E82 File Offset: 0x00026082
		protected override bool OnPreviewMouseScroll()
		{
			return false;
		}
	}
}
