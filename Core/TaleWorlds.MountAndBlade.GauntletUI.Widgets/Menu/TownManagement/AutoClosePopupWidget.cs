using System;
using System.Collections.Generic;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Menu.TownManagement
{
	// Token: 0x020000E8 RID: 232
	public class AutoClosePopupWidget : Widget
	{
		// Token: 0x06000C0A RID: 3082 RVA: 0x000219AC File Offset: 0x0001FBAC
		public AutoClosePopupWidget(UIContext context)
			: base(context)
		{
			for (int i = 0; i < base.ChildCount; i++)
			{
				AutoClosePopupClosingWidget autoClosePopupClosingWidget;
				if ((autoClosePopupClosingWidget = base.GetChild(i) as AutoClosePopupClosingWidget) != null)
				{
					this._closingWidgets.Add(autoClosePopupClosingWidget);
				}
			}
		}

		// Token: 0x06000C0B RID: 3083 RVA: 0x000219F8 File Offset: 0x0001FBF8
		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			if (base.IsVisible && base.EventManager.LatestMouseUpWidget != this.PopupParentWidget && base.EventManager.LatestMouseUpWidget != this._lastCheckedMouseUpWidget)
			{
				base.IsVisible = base.EventManager.LatestMouseUpWidget == this || base.CheckIsMyChildRecursive(base.EventManager.LatestMouseUpWidget);
				this.CheckClosingWidgetsAndUpdateVisibility();
				this._lastCheckedMouseUpWidget = (base.IsVisible ? base.EventManager.LatestMouseUpWidget : null);
			}
		}

		// Token: 0x06000C0C RID: 3084 RVA: 0x00021A84 File Offset: 0x0001FC84
		protected override void OnChildAdded(Widget child)
		{
			base.OnChildAdded(child);
			AutoClosePopupClosingWidget autoClosePopupClosingWidget;
			if ((autoClosePopupClosingWidget = child as AutoClosePopupClosingWidget) != null)
			{
				this._closingWidgets.Add(autoClosePopupClosingWidget);
			}
		}

		// Token: 0x06000C0D RID: 3085 RVA: 0x00021AB0 File Offset: 0x0001FCB0
		protected void CheckClosingWidgetsAndUpdateVisibility()
		{
			if (base.IsVisible)
			{
				using (List<AutoClosePopupClosingWidget>.Enumerator enumerator = this._closingWidgets.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (enumerator.Current.ShouldClosePopup())
						{
							base.IsVisible = false;
							break;
						}
					}
				}
			}
		}

		// Token: 0x1700044F RID: 1103
		// (get) Token: 0x06000C0E RID: 3086 RVA: 0x00021B14 File Offset: 0x0001FD14
		// (set) Token: 0x06000C0F RID: 3087 RVA: 0x00021B1C File Offset: 0x0001FD1C
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

		// Token: 0x0400058E RID: 1422
		private List<AutoClosePopupClosingWidget> _closingWidgets = new List<AutoClosePopupClosingWidget>();

		// Token: 0x0400058F RID: 1423
		protected Widget _lastCheckedMouseUpWidget;

		// Token: 0x04000590 RID: 1424
		private Widget _popupParentWidget;
	}
}
