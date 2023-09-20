using System;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.GauntletUI.ExtraWidgets
{
	// Token: 0x0200000F RID: 15
	public class SiblingIndexVisibilityWidget : Widget
	{
		// Token: 0x1700005D RID: 93
		// (get) Token: 0x060000EC RID: 236 RVA: 0x00005F76 File Offset: 0x00004176
		// (set) Token: 0x060000ED RID: 237 RVA: 0x00005F7E File Offset: 0x0000417E
		public SiblingIndexVisibilityWidget.WatchTypes WatchType { get; set; }

		// Token: 0x060000EE RID: 238 RVA: 0x00005F87 File Offset: 0x00004187
		public SiblingIndexVisibilityWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x060000EF RID: 239 RVA: 0x00005F90 File Offset: 0x00004190
		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			this.UpdateVisibility();
		}

		// Token: 0x060000F0 RID: 240 RVA: 0x00005FA0 File Offset: 0x000041A0
		private void UpdateVisibility()
		{
			Widget widget = this.WidgetToWatch ?? this;
			if (((widget != null) ? widget.ParentWidget : null) != null)
			{
				switch (this.WatchType)
				{
				case SiblingIndexVisibilityWidget.WatchTypes.Equal:
					base.IsVisible = widget.GetSiblingIndex() == this.IndexToBeVisible;
					return;
				case SiblingIndexVisibilityWidget.WatchTypes.BiggerThan:
					base.IsVisible = widget.GetSiblingIndex() > this.IndexToBeVisible;
					return;
				case SiblingIndexVisibilityWidget.WatchTypes.BiggerThanEqual:
					base.IsVisible = widget.GetSiblingIndex() >= this.IndexToBeVisible;
					return;
				case SiblingIndexVisibilityWidget.WatchTypes.LessThan:
					base.IsVisible = widget.GetSiblingIndex() < this.IndexToBeVisible;
					return;
				case SiblingIndexVisibilityWidget.WatchTypes.LessThanEqual:
					base.IsVisible = widget.GetSiblingIndex() <= this.IndexToBeVisible;
					return;
				case SiblingIndexVisibilityWidget.WatchTypes.Odd:
					base.IsVisible = widget.GetSiblingIndex() % 2 == 1;
					return;
				case SiblingIndexVisibilityWidget.WatchTypes.Even:
					base.IsVisible = widget.GetSiblingIndex() % 2 == 0;
					break;
				default:
					return;
				}
			}
		}

		// Token: 0x060000F1 RID: 241 RVA: 0x00006086 File Offset: 0x00004286
		private void OnWidgetToWatchParentEventFired(Widget arg1, string arg2, object[] arg3)
		{
			if (arg2 == "ItemAdd" || arg2 == "ItemRemove")
			{
				this.UpdateVisibility();
			}
		}

		// Token: 0x1700005E RID: 94
		// (get) Token: 0x060000F2 RID: 242 RVA: 0x000060A8 File Offset: 0x000042A8
		// (set) Token: 0x060000F3 RID: 243 RVA: 0x000060B0 File Offset: 0x000042B0
		[Editor(false)]
		public int IndexToBeVisible
		{
			get
			{
				return this._indexToBeVisible;
			}
			set
			{
				if (this._indexToBeVisible != value)
				{
					this._indexToBeVisible = value;
					base.OnPropertyChanged(value, "IndexToBeVisible");
				}
			}
		}

		// Token: 0x1700005F RID: 95
		// (get) Token: 0x060000F4 RID: 244 RVA: 0x000060CE File Offset: 0x000042CE
		// (set) Token: 0x060000F5 RID: 245 RVA: 0x000060D6 File Offset: 0x000042D6
		[Editor(false)]
		public Widget WidgetToWatch
		{
			get
			{
				return this._widgetToWatch;
			}
			set
			{
				if (this._widgetToWatch != value)
				{
					this._widgetToWatch = value;
					base.OnPropertyChanged<Widget>(value, "WidgetToWatch");
					value.ParentWidget.EventFire += this.OnWidgetToWatchParentEventFired;
					this.UpdateVisibility();
				}
			}
		}

		// Token: 0x04000071 RID: 113
		private Widget _widgetToWatch;

		// Token: 0x04000072 RID: 114
		private int _indexToBeVisible;

		// Token: 0x02000017 RID: 23
		public enum WatchTypes
		{
			// Token: 0x0400009E RID: 158
			Equal,
			// Token: 0x0400009F RID: 159
			BiggerThan,
			// Token: 0x040000A0 RID: 160
			BiggerThanEqual,
			// Token: 0x040000A1 RID: 161
			LessThan,
			// Token: 0x040000A2 RID: 162
			LessThanEqual,
			// Token: 0x040000A3 RID: 163
			Odd,
			// Token: 0x040000A4 RID: 164
			Even
		}
	}
}
