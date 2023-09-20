using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets
{
	// Token: 0x0200003B RID: 59
	public class ToggleStateButtonWidget : ButtonWidget
	{
		// Token: 0x0600032A RID: 810 RVA: 0x0000A248 File Offset: 0x00008448
		public ToggleStateButtonWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x0600032B RID: 811 RVA: 0x0000A260 File Offset: 0x00008460
		protected override void HandleClick()
		{
			foreach (Action<Widget> action in this.ClickEventHandlers)
			{
				action(this);
			}
			bool isSelected = base.IsSelected;
			if (!base.IsSelected)
			{
				base.IsSelected = true;
			}
			else if (this.AllowSwitchOff)
			{
				base.IsSelected = false;
			}
			if (base.IsSelected && !isSelected && this.NotifyParentForSelection && base.ParentWidget is Container)
			{
				(base.ParentWidget as Container).OnChildSelected(this);
			}
			if (this.AllowSwitchOff && !base.IsSelected && this.NotifyParentForSelection && base.ParentWidget is Container)
			{
				(base.ParentWidget as Container).OnChildSelected(null);
			}
			this.OnClick();
			base.EventFired("Click", Array.Empty<object>());
			if (base.Context.EventManager.Time - this._lastClickTime < 0.5f)
			{
				base.EventFired("DoubleClick", Array.Empty<object>());
				return;
			}
			this._lastClickTime = base.Context.EventManager.Time;
		}

		// Token: 0x0600032C RID: 812 RVA: 0x0000A39C File Offset: 0x0000859C
		protected override void RefreshState()
		{
			base.RefreshState();
			if (base.UpdateChildrenStates)
			{
				this.UpdateChildrenStatesRecursively(this);
			}
			if (this._widgetToClose != null)
			{
				this._widgetToClose.IsVisible = base.IsSelected;
			}
		}

		// Token: 0x0600032D RID: 813 RVA: 0x0000A3CC File Offset: 0x000085CC
		private void UpdateChildrenStatesRecursively(Widget parent)
		{
			parent.SetState(base.CurrentState);
			if (parent.ChildCount > 0)
			{
				foreach (Widget widget in parent.Children)
				{
					this.UpdateChildrenStatesRecursively(widget);
				}
			}
		}

		// Token: 0x1700011C RID: 284
		// (get) Token: 0x0600032E RID: 814 RVA: 0x0000A434 File Offset: 0x00008634
		// (set) Token: 0x0600032F RID: 815 RVA: 0x0000A43C File Offset: 0x0000863C
		[Editor(false)]
		public Widget WidgetToClose
		{
			get
			{
				return this._widgetToClose;
			}
			set
			{
				if (this._widgetToClose != value)
				{
					this._widgetToClose = value;
					base.OnPropertyChanged<Widget>(value, "WidgetToClose");
					if (this._widgetToClose != null)
					{
						this._widgetToClose.IsVisible = base.IsSelected;
					}
				}
			}
		}

		// Token: 0x1700011D RID: 285
		// (get) Token: 0x06000330 RID: 816 RVA: 0x0000A473 File Offset: 0x00008673
		// (set) Token: 0x06000331 RID: 817 RVA: 0x0000A47B File Offset: 0x0000867B
		[Editor(false)]
		public bool AllowSwitchOff
		{
			get
			{
				return this._allowSwitchOff;
			}
			set
			{
				if (this._allowSwitchOff != value)
				{
					this._allowSwitchOff = value;
					base.OnPropertyChanged(value, "AllowSwitchOff");
				}
			}
		}

		// Token: 0x1700011E RID: 286
		// (get) Token: 0x06000332 RID: 818 RVA: 0x0000A499 File Offset: 0x00008699
		// (set) Token: 0x06000333 RID: 819 RVA: 0x0000A4A1 File Offset: 0x000086A1
		[Editor(false)]
		public bool NotifyParentForSelection
		{
			get
			{
				return this._notifyParentForSelection;
			}
			set
			{
				if (this._notifyParentForSelection != value)
				{
					this._notifyParentForSelection = value;
					base.OnPropertyChanged(value, "NotifyParentForSelection");
				}
			}
		}

		// Token: 0x0400014D RID: 333
		private Widget _widgetToClose;

		// Token: 0x0400014E RID: 334
		private bool _allowSwitchOff = true;

		// Token: 0x0400014F RID: 335
		private bool _notifyParentForSelection = true;
	}
}
