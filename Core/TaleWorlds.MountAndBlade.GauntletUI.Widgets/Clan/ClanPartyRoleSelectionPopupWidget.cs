using System;
using System.Collections.Generic;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.MountAndBlade.GauntletUI.Widgets.Menu.TownManagement;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Clan
{
	public class ClanPartyRoleSelectionPopupWidget : AutoClosePopupWidget
	{
		public ClanPartyRoleSelectionPopupWidget(UIContext context)
			: base(context)
		{
			this._toggleWidgets = new List<Widget>();
			base.IsVisible = false;
		}

		protected override void OnLateUpdate(float dt)
		{
			if (base.IsVisible && base.EventManager.LatestMouseUpWidget != this._lastCheckedMouseUpWidget && !this._toggleWidgets.Contains(base.EventManager.LatestMouseUpWidget))
			{
				base.IsVisible = base.EventManager.LatestMouseUpWidget == this || base.CheckIsMyChildRecursive(base.EventManager.LatestMouseUpWidget);
				base.CheckClosingWidgetsAndUpdateVisibility();
				if (!base.IsVisible)
				{
					this.ActiveToggleWidget = null;
				}
			}
			this._lastCheckedMouseUpWidget = base.EventManager.LatestMouseUpWidget;
		}

		public void AddToggleWidget(Widget widget)
		{
			if (!this._toggleWidgets.Contains(widget))
			{
				this._toggleWidgets.Add(widget);
			}
		}

		[Editor(false)]
		public Widget ActiveToggleWidget
		{
			get
			{
				return this._activeToggleWidget;
			}
			set
			{
				if (this._activeToggleWidget != value)
				{
					this._activeToggleWidget = value;
					base.OnPropertyChanged<Widget>(value, "ActiveToggleWidget");
				}
			}
		}

		private List<Widget> _toggleWidgets;

		private Widget _activeToggleWidget;
	}
}
