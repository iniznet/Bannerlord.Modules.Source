using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Menu.Overlay
{
	public class OverlayBaseWidget : Widget
	{
		public OverlayBaseWidget(UIContext context)
			: base(context)
		{
		}

		[Editor(false)]
		public OverlayPopupWidget PopupWidget
		{
			get
			{
				return this._popupWidget;
			}
			set
			{
				if (this._popupWidget != value)
				{
					this._popupWidget = value;
					base.OnPropertyChanged<OverlayPopupWidget>(value, "PopupWidget");
				}
			}
		}

		private OverlayPopupWidget _popupWidget;
	}
}
