using System;
using System.Collections.Generic;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Menu.TownManagement
{
	public class AutoClosePopupWidget : Widget
	{
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

		protected override void OnChildAdded(Widget child)
		{
			base.OnChildAdded(child);
			AutoClosePopupClosingWidget autoClosePopupClosingWidget;
			if ((autoClosePopupClosingWidget = child as AutoClosePopupClosingWidget) != null)
			{
				this._closingWidgets.Add(autoClosePopupClosingWidget);
			}
		}

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

		private List<AutoClosePopupClosingWidget> _closingWidgets = new List<AutoClosePopupClosingWidget>();

		protected Widget _lastCheckedMouseUpWidget;

		private Widget _popupParentWidget;
	}
}
