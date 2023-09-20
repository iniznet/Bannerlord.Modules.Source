using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets
{
	public class NavigationAutoScrollWidget : Widget
	{
		public ScrollablePanel ParentPanel { get; set; }

		public NavigationAutoScrollWidget(UIContext context)
			: base(context)
		{
			base.WidthSizePolicy = SizePolicy.Fixed;
			base.HeightSizePolicy = SizePolicy.Fixed;
			base.SuggestedHeight = 0f;
			base.SuggestedWidth = 0f;
			base.IsVisible = false;
		}

		protected override void OnConnectedToRoot()
		{
			base.OnConnectedToRoot();
			if (this.ParentPanel == null && base.ParentWidget != null)
			{
				for (Widget widget = base.ParentWidget; widget != null; widget = widget.ParentWidget)
				{
					ScrollablePanel scrollablePanel;
					if ((scrollablePanel = widget as ScrollablePanel) != null)
					{
						this.ParentPanel = scrollablePanel;
						return;
					}
				}
			}
		}

		private void OnWidgetGainedGamepadFocus(Widget widget)
		{
			if (this.ParentPanel != null)
			{
				this.ParentPanel.ScrollToChild(this.ScrollTarget ?? widget, -1f, -1f, this.ScrollXOffset, this.ScrollYOffset, 0f, 0f);
			}
		}

		private void UpdateTargetAutoScrollAndChildren()
		{
			if (this._trackedWidget != null)
			{
				Widget trackedWidget = this._trackedWidget;
				trackedWidget.OnGamepadNavigationFocusGained = (Action<Widget>)Delegate.Combine(trackedWidget.OnGamepadNavigationFocusGained, new Action<Widget>(this.OnWidgetGainedGamepadFocus));
				foreach (Widget widget in this._trackedWidget.Children)
				{
					if (this.IncludeChildren)
					{
						Widget widget2 = widget;
						widget2.OnGamepadNavigationFocusGained = (Action<Widget>)Delegate.Combine(widget2.OnGamepadNavigationFocusGained, new Action<Widget>(this.OnWidgetGainedGamepadFocus));
					}
					else
					{
						Widget widget3 = widget;
						widget3.OnGamepadNavigationFocusGained = (Action<Widget>)Delegate.Remove(widget3.OnGamepadNavigationFocusGained, new Action<Widget>(this.OnWidgetGainedGamepadFocus));
					}
				}
			}
		}

		public int ScrollYOffset { get; set; }

		public int ScrollXOffset { get; set; }

		public bool IncludeChildren
		{
			get
			{
				return this._includeChildren;
			}
			set
			{
				if (value != this._includeChildren)
				{
					this._includeChildren = value;
					this.UpdateTargetAutoScrollAndChildren();
				}
			}
		}

		public Widget TrackedWidget
		{
			get
			{
				return this._trackedWidget;
			}
			set
			{
				if (value != this._trackedWidget)
				{
					if (this._trackedWidget != null)
					{
						this._trackedWidget.OnGamepadNavigationFocusGained = null;
					}
					this._trackedWidget = value;
					this.UpdateTargetAutoScrollAndChildren();
				}
			}
		}

		public Widget ScrollTarget
		{
			get
			{
				return this._scrollTarget;
			}
			set
			{
				if (value != this._scrollTarget)
				{
					this._scrollTarget = value;
				}
			}
		}

		private bool _includeChildren;

		private Widget _trackedWidget;

		private Widget _scrollTarget;
	}
}
