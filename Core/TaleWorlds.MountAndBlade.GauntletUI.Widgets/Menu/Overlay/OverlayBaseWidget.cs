using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Menu.Overlay
{
	// Token: 0x020000F3 RID: 243
	public class OverlayBaseWidget : Widget
	{
		// Token: 0x06000CAA RID: 3242 RVA: 0x00023770 File Offset: 0x00021970
		public OverlayBaseWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x17000489 RID: 1161
		// (get) Token: 0x06000CAB RID: 3243 RVA: 0x00023779 File Offset: 0x00021979
		// (set) Token: 0x06000CAC RID: 3244 RVA: 0x00023781 File Offset: 0x00021981
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

		// Token: 0x040005D9 RID: 1497
		private OverlayPopupWidget _popupWidget;
	}
}
