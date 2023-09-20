using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Menu.TownManagement
{
	// Token: 0x020000EF RID: 239
	public class SliderPopupWidget : Widget
	{
		// Token: 0x06000C62 RID: 3170 RVA: 0x00022C6A File Offset: 0x00020E6A
		public SliderPopupWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x06000C63 RID: 3171 RVA: 0x00022C74 File Offset: 0x00020E74
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

		// Token: 0x1700046D RID: 1133
		// (get) Token: 0x06000C64 RID: 3172 RVA: 0x00022D1F File Offset: 0x00020F1F
		// (set) Token: 0x06000C65 RID: 3173 RVA: 0x00022D27 File Offset: 0x00020F27
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

		// Token: 0x1700046E RID: 1134
		// (get) Token: 0x06000C66 RID: 3174 RVA: 0x00022D45 File Offset: 0x00020F45
		// (set) Token: 0x06000C67 RID: 3175 RVA: 0x00022D4D File Offset: 0x00020F4D
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

		// Token: 0x1700046F RID: 1135
		// (get) Token: 0x06000C68 RID: 3176 RVA: 0x00022D6B File Offset: 0x00020F6B
		// (set) Token: 0x06000C69 RID: 3177 RVA: 0x00022D73 File Offset: 0x00020F73
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

		// Token: 0x17000470 RID: 1136
		// (get) Token: 0x06000C6A RID: 3178 RVA: 0x00022D91 File Offset: 0x00020F91
		// (set) Token: 0x06000C6B RID: 3179 RVA: 0x00022D99 File Offset: 0x00020F99
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

		// Token: 0x040005B7 RID: 1463
		private ButtonWidget _closePopupWidget;

		// Token: 0x040005B8 RID: 1464
		private TextWidget _sliderValueTextWidget;

		// Token: 0x040005B9 RID: 1465
		private SliderWidget _reserveAmountSlider;

		// Token: 0x040005BA RID: 1466
		private Widget _popupParentWidget;
	}
}
