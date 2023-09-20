using System;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.GauntletUI.ExtraWidgets
{
	public class SiblingIndexVisibilityWidget : Widget
	{
		public SiblingIndexVisibilityWidget.WatchTypes WatchType { get; set; }

		public SiblingIndexVisibilityWidget(UIContext context)
			: base(context)
		{
		}

		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			this.UpdateVisibility();
		}

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

		private void OnWidgetToWatchParentEventFired(Widget arg1, string arg2, object[] arg3)
		{
			if (arg2 == "ItemAdd" || arg2 == "ItemRemove")
			{
				this.UpdateVisibility();
			}
		}

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

		private Widget _widgetToWatch;

		private int _indexToBeVisible;

		public enum WatchTypes
		{
			Equal,
			BiggerThan,
			BiggerThanEqual,
			LessThan,
			LessThanEqual,
			Odd,
			Even
		}
	}
}
