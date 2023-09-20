using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets
{
	public class ToggleStateButtonWidget : ButtonWidget
	{
		public ToggleStateButtonWidget(UIContext context)
			: base(context)
		{
		}

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

		private Widget _widgetToClose;

		private bool _allowSwitchOff = true;

		private bool _notifyParentForSelection = true;
	}
}
