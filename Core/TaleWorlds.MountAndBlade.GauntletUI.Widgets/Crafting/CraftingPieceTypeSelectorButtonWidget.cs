using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Crafting
{
	// Token: 0x02000147 RID: 327
	public class CraftingPieceTypeSelectorButtonWidget : ButtonWidget
	{
		// Token: 0x06001128 RID: 4392 RVA: 0x0002F970 File Offset: 0x0002DB70
		public CraftingPieceTypeSelectorButtonWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x06001129 RID: 4393 RVA: 0x0002F979 File Offset: 0x0002DB79
		public override void SetState(string stateName)
		{
			base.SetState(stateName);
			Widget visualsWidget = this.VisualsWidget;
			if (visualsWidget == null)
			{
				return;
			}
			visualsWidget.SetState(stateName);
		}

		// Token: 0x17000613 RID: 1555
		// (get) Token: 0x0600112A RID: 4394 RVA: 0x0002F993 File Offset: 0x0002DB93
		// (set) Token: 0x0600112B RID: 4395 RVA: 0x0002F99B File Offset: 0x0002DB9B
		public Widget VisualsWidget
		{
			get
			{
				return this._visualsWidget;
			}
			set
			{
				if (value != this._visualsWidget)
				{
					this._visualsWidget = value;
				}
			}
		}

		// Token: 0x040007DF RID: 2015
		private Widget _visualsWidget;
	}
}
