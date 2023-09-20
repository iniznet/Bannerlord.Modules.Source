using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Menu.TownManagement
{
	public class SliderPopupWidget : Widget
	{
		public SliderPopupWidget(UIContext context)
			: base(context)
		{
		}

		protected override void OnUpdate(float dt)
		{
			base.OnUpdate(dt);
			if (this.SliderValueTextWidget != null && this.ReserveAmountSlider != null)
			{
				this.SliderValueTextWidget.Text = this.ReserveAmountSlider.ValueInt.ToString();
			}
			if (base.ParentWidget.IsVisible && base.EventManager.LatestMouseDownWidget != this.PopupParentWidget && base.EventManager.LatestMouseUpWidget != this.PopupParentWidget && !base.CheckIsMyChildRecursive(base.EventManager.LatestMouseUpWidget) && !base.CheckIsMyChildRecursive(base.EventManager.LatestMouseDownWidget))
			{
				base.EventFired("ClosePopup", Array.Empty<object>());
			}
		}

		[Editor(false)]
		public Widget PopupParentWidget
		{
			get
			{
				return this._popupParentWidget;
			}
			set
			{
				if (this._popupParentWidget != value)
				{
					this._popupParentWidget = value;
					base.OnPropertyChanged<Widget>(value, "PopupParentWidget");
				}
			}
		}

		[Editor(false)]
		public ButtonWidget ClosePopupWidget
		{
			get
			{
				return this._closePopupWidget;
			}
			set
			{
				if (this._closePopupWidget != value)
				{
					this._closePopupWidget = value;
					base.OnPropertyChanged<ButtonWidget>(value, "ClosePopupWidget");
				}
			}
		}

		[Editor(false)]
		public TextWidget SliderValueTextWidget
		{
			get
			{
				return this._sliderValueTextWidget;
			}
			set
			{
				if (this._sliderValueTextWidget != value)
				{
					this._sliderValueTextWidget = value;
					base.OnPropertyChanged<TextWidget>(value, "SliderValueTextWidget");
				}
			}
		}

		[Editor(false)]
		public SliderWidget ReserveAmountSlider
		{
			get
			{
				return this._reserveAmountSlider;
			}
			set
			{
				if (this._reserveAmountSlider != value)
				{
					this._reserveAmountSlider = value;
					base.OnPropertyChanged<SliderWidget>(value, "ReserveAmountSlider");
				}
			}
		}

		private ButtonWidget _closePopupWidget;

		private TextWidget _sliderValueTextWidget;

		private SliderWidget _reserveAmountSlider;

		private Widget _popupParentWidget;
	}
}
