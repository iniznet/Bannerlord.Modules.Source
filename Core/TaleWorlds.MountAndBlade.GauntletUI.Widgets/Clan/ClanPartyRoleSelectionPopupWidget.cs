using System;
using System.Collections.Generic;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.MountAndBlade.GauntletUI.Widgets.Menu.TownManagement;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Clan
{
	// Token: 0x02000155 RID: 341
	public class ClanPartyRoleSelectionPopupWidget : AutoClosePopupWidget
	{
		// Token: 0x060011A2 RID: 4514 RVA: 0x00030A48 File Offset: 0x0002EC48
		public ClanPartyRoleSelectionPopupWidget(UIContext context)
			: base(context)
		{
			this._toggleWidgets = new List<Widget>();
			base.IsVisible = false;
		}

		// Token: 0x060011A3 RID: 4515 RVA: 0x00030A64 File Offset: 0x0002EC64
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

		// Token: 0x060011A4 RID: 4516 RVA: 0x00030AF2 File Offset: 0x0002ECF2
		public void AddToggleWidget(Widget widget)
		{
			if (!this._toggleWidgets.Contains(widget))
			{
				this._toggleWidgets.Add(widget);
			}
		}

		// Token: 0x1700063D RID: 1597
		// (get) Token: 0x060011A5 RID: 4517 RVA: 0x00030B0E File Offset: 0x0002ED0E
		// (set) Token: 0x060011A6 RID: 4518 RVA: 0x00030B16 File Offset: 0x0002ED16
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

		// Token: 0x04000811 RID: 2065
		private List<Widget> _toggleWidgets;

		// Token: 0x04000812 RID: 2066
		private Widget _activeToggleWidget;
	}
}
