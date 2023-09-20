using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets
{
	public class ToggleButtonWidget : ButtonWidget
	{
		public ToggleButtonWidget(UIContext context)
			: base(context)
		{
			this.ClickEventHandlers.Add(new Action<Widget>(this.OnClick));
		}

		protected virtual void OnClick(Widget widget)
		{
			if (this._widgetToClose != null)
			{
				this.IsTargetVisible = !this._widgetToClose.IsVisible;
			}
		}

		public bool IsTargetVisible
		{
			get
			{
				Widget widgetToClose = this._widgetToClose;
				return widgetToClose != null && widgetToClose.IsVisible;
			}
			set
			{
				if (this._widgetToClose != null && this._widgetToClose.IsVisible != value)
				{
					this._widgetToClose.IsVisible = value;
					base.OnPropertyChanged(value, "IsTargetVisible");
				}
			}
		}

		[Editor(false)]
		public Widget WidgetToClose
		{
			get
			{
				return this._widgetToClose;
			}
			set
			{
				if (this._widgetToClose != value)
				{
					this._widgetToClose = value;
					base.OnPropertyChanged<Widget>(value, "WidgetToClose");
				}
			}
		}

		private Widget _widgetToClose;
	}
}
