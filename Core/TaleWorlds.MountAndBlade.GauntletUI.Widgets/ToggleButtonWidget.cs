using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets
{
	// Token: 0x0200003A RID: 58
	public class ToggleButtonWidget : ButtonWidget
	{
		// Token: 0x06000324 RID: 804 RVA: 0x0000A1A0 File Offset: 0x000083A0
		public ToggleButtonWidget(UIContext context)
			: base(context)
		{
			this.ClickEventHandlers.Add(new Action<Widget>(this.OnClick));
		}

		// Token: 0x06000325 RID: 805 RVA: 0x0000A1C1 File Offset: 0x000083C1
		protected virtual void OnClick(Widget widget)
		{
			if (this._widgetToClose != null)
			{
				this.IsTargetVisible = !this._widgetToClose.IsVisible;
			}
		}

		// Token: 0x1700011A RID: 282
		// (get) Token: 0x06000326 RID: 806 RVA: 0x0000A1DF File Offset: 0x000083DF
		// (set) Token: 0x06000327 RID: 807 RVA: 0x0000A1F2 File Offset: 0x000083F2
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

		// Token: 0x1700011B RID: 283
		// (get) Token: 0x06000328 RID: 808 RVA: 0x0000A222 File Offset: 0x00008422
		// (set) Token: 0x06000329 RID: 809 RVA: 0x0000A22A File Offset: 0x0000842A
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

		// Token: 0x0400014C RID: 332
		private Widget _widgetToClose;
	}
}
