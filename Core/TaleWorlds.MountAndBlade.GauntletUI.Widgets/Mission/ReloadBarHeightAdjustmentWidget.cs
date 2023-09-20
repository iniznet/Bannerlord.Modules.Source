using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Mission
{
	// Token: 0x020000CC RID: 204
	public class ReloadBarHeightAdjustmentWidget : Widget
	{
		// Token: 0x06000A66 RID: 2662 RVA: 0x0001D63F File Offset: 0x0001B83F
		public ReloadBarHeightAdjustmentWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x06000A67 RID: 2663 RVA: 0x0001D648 File Offset: 0x0001B848
		private void Refresh()
		{
			if (this.FillWidget != null)
			{
				base.ScaledSuggestedHeight = 50f * this.RelativeDurationToMaxDuration * base._scaleToUse;
				this.FillWidget.ScaledSuggestedHeight = base.ScaledSuggestedHeight - (this.FillWidget.MarginBottom + this.FillWidget.MarginTop) * base._scaleToUse;
			}
		}

		// Token: 0x170003A9 RID: 937
		// (get) Token: 0x06000A68 RID: 2664 RVA: 0x0001D6A6 File Offset: 0x0001B8A6
		// (set) Token: 0x06000A69 RID: 2665 RVA: 0x0001D6AE File Offset: 0x0001B8AE
		public float RelativeDurationToMaxDuration
		{
			get
			{
				return this._relativeDurationToMaxDuration;
			}
			set
			{
				if (value != this._relativeDurationToMaxDuration)
				{
					this._relativeDurationToMaxDuration = value;
					this.Refresh();
				}
			}
		}

		// Token: 0x170003AA RID: 938
		// (get) Token: 0x06000A6A RID: 2666 RVA: 0x0001D6C6 File Offset: 0x0001B8C6
		// (set) Token: 0x06000A6B RID: 2667 RVA: 0x0001D6CE File Offset: 0x0001B8CE
		public Widget FillWidget
		{
			get
			{
				return this._fillWidget;
			}
			set
			{
				if (value != this._fillWidget)
				{
					this._fillWidget = value;
					this.Refresh();
				}
			}
		}

		// Token: 0x040004BE RID: 1214
		private const float _baseHeight = 50f;

		// Token: 0x040004BF RID: 1215
		private float _relativeDurationToMaxDuration;

		// Token: 0x040004C0 RID: 1216
		private Widget _fillWidget;
	}
}
