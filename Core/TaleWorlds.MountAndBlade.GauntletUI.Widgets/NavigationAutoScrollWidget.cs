using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets
{
	// Token: 0x02000029 RID: 41
	public class NavigationAutoScrollWidget : Widget
	{
		// Token: 0x170000B0 RID: 176
		// (get) Token: 0x06000216 RID: 534 RVA: 0x00007B4D File Offset: 0x00005D4D
		// (set) Token: 0x06000217 RID: 535 RVA: 0x00007B55 File Offset: 0x00005D55
		public ScrollablePanel ParentPanel { get; set; }

		// Token: 0x06000218 RID: 536 RVA: 0x00007B5E File Offset: 0x00005D5E
		public NavigationAutoScrollWidget(UIContext context)
			: base(context)
		{
			base.WidthSizePolicy = SizePolicy.Fixed;
			base.HeightSizePolicy = SizePolicy.Fixed;
			base.SuggestedHeight = 0f;
			base.SuggestedWidth = 0f;
			base.IsVisible = false;
		}

		// Token: 0x06000219 RID: 537 RVA: 0x00007B94 File Offset: 0x00005D94
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

		// Token: 0x0600021A RID: 538 RVA: 0x00007BDC File Offset: 0x00005DDC
		private void OnWidgetGainedGamepadFocus(Widget widget)
		{
			if (this.ParentPanel != null)
			{
				this.ParentPanel.ScrollToChild(this.ScrollTarget ?? widget, -1f, -1f, this.ScrollXOffset, this.ScrollYOffset, 0f, 0f);
			}
		}

		// Token: 0x0600021B RID: 539 RVA: 0x00007C1C File Offset: 0x00005E1C
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

		// Token: 0x170000B1 RID: 177
		// (get) Token: 0x0600021C RID: 540 RVA: 0x00007CF0 File Offset: 0x00005EF0
		// (set) Token: 0x0600021D RID: 541 RVA: 0x00007CF8 File Offset: 0x00005EF8
		public int ScrollYOffset { get; set; }

		// Token: 0x170000B2 RID: 178
		// (get) Token: 0x0600021E RID: 542 RVA: 0x00007D01 File Offset: 0x00005F01
		// (set) Token: 0x0600021F RID: 543 RVA: 0x00007D09 File Offset: 0x00005F09
		public int ScrollXOffset { get; set; }

		// Token: 0x170000B3 RID: 179
		// (get) Token: 0x06000220 RID: 544 RVA: 0x00007D12 File Offset: 0x00005F12
		// (set) Token: 0x06000221 RID: 545 RVA: 0x00007D1A File Offset: 0x00005F1A
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

		// Token: 0x170000B4 RID: 180
		// (get) Token: 0x06000222 RID: 546 RVA: 0x00007D32 File Offset: 0x00005F32
		// (set) Token: 0x06000223 RID: 547 RVA: 0x00007D3A File Offset: 0x00005F3A
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

		// Token: 0x170000B5 RID: 181
		// (get) Token: 0x06000224 RID: 548 RVA: 0x00007D66 File Offset: 0x00005F66
		// (set) Token: 0x06000225 RID: 549 RVA: 0x00007D6E File Offset: 0x00005F6E
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

		// Token: 0x040000FB RID: 251
		private bool _includeChildren;

		// Token: 0x040000FC RID: 252
		private Widget _trackedWidget;

		// Token: 0x040000FD RID: 253
		private Widget _scrollTarget;
	}
}
