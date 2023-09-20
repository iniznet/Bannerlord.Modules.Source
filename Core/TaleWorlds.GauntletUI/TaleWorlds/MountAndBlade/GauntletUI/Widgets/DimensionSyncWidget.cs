using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets
{
	public class DimensionSyncWidget : Widget
	{
		public DimensionSyncWidget(UIContext context)
			: base(context)
		{
		}

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

		private Widget _widgetToCopyHeightFrom;

		private DimensionSyncWidget.Dimensions _dimensionToSync;

		private int _paddingAmount;

		public enum Dimensions
		{
			None,
			Horizontal,
			Vertical,
			HorizontalAndVertical
		}
	}
}
