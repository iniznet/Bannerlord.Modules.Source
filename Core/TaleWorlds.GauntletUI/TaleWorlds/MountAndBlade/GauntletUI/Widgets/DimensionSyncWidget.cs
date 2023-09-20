using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets
{
	// Token: 0x02000004 RID: 4
	public class DimensionSyncWidget : Widget
	{
		// Token: 0x06000003 RID: 3 RVA: 0x00002058 File Offset: 0x00000258
		public DimensionSyncWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x06000004 RID: 4 RVA: 0x00002064 File Offset: 0x00000264
		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			if (this.DimensionToSync != DimensionSyncWidget.Dimensions.None && this.WidgetToCopyHeightFrom != null)
			{
				if (this.DimensionToSync == DimensionSyncWidget.Dimensions.Horizontal || this.DimensionToSync == DimensionSyncWidget.Dimensions.HorizontalAndVertical)
				{
					base.ScaledSuggestedWidth = this.WidgetToCopyHeightFrom.Size.X + (float)this.PaddingAmount * base._scaleToUse;
				}
				if (this.DimensionToSync == DimensionSyncWidget.Dimensions.Vertical || this.DimensionToSync == DimensionSyncWidget.Dimensions.HorizontalAndVertical)
				{
					base.ScaledSuggestedHeight = this.WidgetToCopyHeightFrom.Size.Y + (float)this.PaddingAmount * base._scaleToUse;
				}
			}
		}

		// Token: 0x17000001 RID: 1
		// (get) Token: 0x06000005 RID: 5 RVA: 0x000020F6 File Offset: 0x000002F6
		// (set) Token: 0x06000006 RID: 6 RVA: 0x000020FE File Offset: 0x000002FE
		public Widget WidgetToCopyHeightFrom
		{
			get
			{
				return this._widgetToCopyHeightFrom;
			}
			set
			{
				if (this._widgetToCopyHeightFrom != value)
				{
					this._widgetToCopyHeightFrom = value;
					base.OnPropertyChanged<Widget>(value, "WidgetToCopyHeightFrom");
				}
			}
		}

		// Token: 0x17000002 RID: 2
		// (get) Token: 0x06000007 RID: 7 RVA: 0x0000211C File Offset: 0x0000031C
		// (set) Token: 0x06000008 RID: 8 RVA: 0x00002124 File Offset: 0x00000324
		public int PaddingAmount
		{
			get
			{
				return this._paddingAmount;
			}
			set
			{
				if (this._paddingAmount != value)
				{
					this._paddingAmount = value;
				}
			}
		}

		// Token: 0x17000003 RID: 3
		// (get) Token: 0x06000009 RID: 9 RVA: 0x00002136 File Offset: 0x00000336
		// (set) Token: 0x0600000A RID: 10 RVA: 0x0000213E File Offset: 0x0000033E
		public DimensionSyncWidget.Dimensions DimensionToSync
		{
			get
			{
				return this._dimensionToSync;
			}
			set
			{
				if (this._dimensionToSync != value)
				{
					this._dimensionToSync = value;
				}
			}
		}

		// Token: 0x04000001 RID: 1
		private Widget _widgetToCopyHeightFrom;

		// Token: 0x04000002 RID: 2
		private DimensionSyncWidget.Dimensions _dimensionToSync;

		// Token: 0x04000003 RID: 3
		private int _paddingAmount;

		// Token: 0x02000070 RID: 112
		public enum Dimensions
		{
			// Token: 0x040003C1 RID: 961
			None,
			// Token: 0x040003C2 RID: 962
			Horizontal,
			// Token: 0x040003C3 RID: 963
			Vertical,
			// Token: 0x040003C4 RID: 964
			HorizontalAndVertical
		}
	}
}
