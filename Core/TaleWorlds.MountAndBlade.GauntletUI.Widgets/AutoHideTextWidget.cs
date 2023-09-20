using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets
{
	// Token: 0x02000003 RID: 3
	public class AutoHideTextWidget : TextWidget
	{
		// Token: 0x06000005 RID: 5 RVA: 0x000020C7 File Offset: 0x000002C7
		public AutoHideTextWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x06000006 RID: 6 RVA: 0x000020D0 File Offset: 0x000002D0
		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			if (this.WidgetToHideIfEmpty != null)
			{
				this.WidgetToHideIfEmpty.IsVisible = base.Text != string.Empty;
			}
			base.IsVisible = base.Text != string.Empty;
		}

		// Token: 0x17000002 RID: 2
		// (get) Token: 0x06000007 RID: 7 RVA: 0x0000211D File Offset: 0x0000031D
		// (set) Token: 0x06000008 RID: 8 RVA: 0x00002125 File Offset: 0x00000325
		[Editor(false)]
		public Widget WidgetToHideIfEmpty
		{
			get
			{
				return this._widgetToHideIfEmpty;
			}
			set
			{
				if (this._widgetToHideIfEmpty != value)
				{
					this._widgetToHideIfEmpty = value;
					base.OnPropertyChanged<Widget>(value, "WidgetToHideIfEmpty");
				}
			}
		}

		// Token: 0x04000002 RID: 2
		private Widget _widgetToHideIfEmpty;
	}
}
