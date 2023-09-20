using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Menu.TownManagement
{
	// Token: 0x020000E7 RID: 231
	public class AutoClosePopupClosingWidget : Widget
	{
		// Token: 0x1700044C RID: 1100
		// (get) Token: 0x06000C02 RID: 3074 RVA: 0x0002191F File Offset: 0x0001FB1F
		// (set) Token: 0x06000C03 RID: 3075 RVA: 0x00021927 File Offset: 0x0001FB27
		public Widget Target { get; set; }

		// Token: 0x1700044D RID: 1101
		// (get) Token: 0x06000C04 RID: 3076 RVA: 0x00021930 File Offset: 0x0001FB30
		// (set) Token: 0x06000C05 RID: 3077 RVA: 0x00021938 File Offset: 0x0001FB38
		public bool IncludeChildren { get; set; }

		// Token: 0x1700044E RID: 1102
		// (get) Token: 0x06000C06 RID: 3078 RVA: 0x00021941 File Offset: 0x0001FB41
		// (set) Token: 0x06000C07 RID: 3079 RVA: 0x00021949 File Offset: 0x0001FB49
		public bool IncludeTarget { get; set; }

		// Token: 0x06000C08 RID: 3080 RVA: 0x00021952 File Offset: 0x0001FB52
		public AutoClosePopupClosingWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x06000C09 RID: 3081 RVA: 0x0002195C File Offset: 0x0001FB5C
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
