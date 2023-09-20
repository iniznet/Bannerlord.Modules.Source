using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Menu.TownManagement
{
	public class AutoClosePopupClosingWidget : Widget
	{
		public Widget Target { get; set; }

		public bool IncludeChildren { get; set; }

		public bool IncludeTarget { get; set; }

		public AutoClosePopupClosingWidget(UIContext context)
			: base(context)
		{
		}

		public bool ShouldClosePopup()
		{
			if (this.IncludeTarget && base.EventManager.LatestMouseUpWidget == this.Target)
			{
				return true;
			}
			if (this.IncludeChildren)
			{
				Widget target = this.Target;
				return target != null && target.CheckIsMyChildRecursive(base.EventManager.LatestMouseUpWidget);
			}
			return false;
		}
	}
}
