using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets
{
	// Token: 0x02000002 RID: 2
	public class AutoHideRichTextWidget : RichTextWidget
	{
		// Token: 0x06000001 RID: 1 RVA: 0x00002048 File Offset: 0x00000248
		public AutoHideRichTextWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x06000002 RID: 2 RVA: 0x00002054 File Offset: 0x00000254
		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			if (this.WidgetToHideIfEmpty != null)
			{
				this.WidgetToHideIfEmpty.IsVisible = base.Text != string.Empty;
			}
			base.IsVisible = base.Text != string.Empty;
		}

		// Token: 0x17000001 RID: 1
		// (get) Token: 0x06000003 RID: 3 RVA: 0x000020A1 File Offset: 0x000002A1
		// (set) Token: 0x06000004 RID: 4 RVA: 0x000020A9 File Offset: 0x000002A9
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

		// Token: 0x04000001 RID: 1
		private Widget _widgetToHideIfEmpty;
	}
}
