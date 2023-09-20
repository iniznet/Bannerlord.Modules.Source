using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Mission
{
	public class ReloadBarHeightAdjustmentWidget : Widget
	{
		public ReloadBarHeightAdjustmentWidget(UIContext context)
			: base(context)
		{
		}

		private void Refresh()
		{
			if (this.FillWidget != null)
			{
				base.ScaledSuggestedHeight = 50f * this.RelativeDurationToMaxDuration * base._scaleToUse;
				this.FillWidget.ScaledSuggestedHeight = base.ScaledSuggestedHeight - (this.FillWidget.MarginBottom + this.FillWidget.MarginTop) * base._scaleToUse;
			}
		}

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

		private const float _baseHeight = 50f;

		private float _relativeDurationToMaxDuration;

		private Widget _fillWidget;
	}
}
