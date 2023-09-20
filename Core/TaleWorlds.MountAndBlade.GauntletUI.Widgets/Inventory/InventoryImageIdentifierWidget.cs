using System;
using TaleWorlds.GauntletUI;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Inventory
{
	// Token: 0x0200011E RID: 286
	public class InventoryImageIdentifierWidget : ImageIdentifierWidget
	{
		// Token: 0x06000E91 RID: 3729 RVA: 0x00028747 File Offset: 0x00026947
		public InventoryImageIdentifierWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x06000E92 RID: 3730 RVA: 0x00028750 File Offset: 0x00026950
		public void SetRenderRequestedPreviousFrame(bool isRequested)
		{
			this._isRenderRequestedPreviousFrame = isRequested;
		}
	}
}
