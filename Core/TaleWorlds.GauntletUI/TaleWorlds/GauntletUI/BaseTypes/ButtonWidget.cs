using System;
using System.Collections.Generic;

namespace TaleWorlds.GauntletUI.BaseTypes
{
	public class ButtonWidget : ImageWidget
	{
		[Editor(false)]
		public ButtonType ButtonType
		{
			get
			{
				return this._buttonType;
			}
			set
			{
				if (this._buttonType != value)
				{
					this._buttonType = value;
					this.Refresh();
				}
			}
		}

		protected override bool OnPreviewMousePressed()
		{
			base.OnPreviewMousePressed();
			return true;
		}

		protected override void RefreshState()
		{
			base.RefreshState();
			if (!base.OverrideDefaultStateSwitchingEnabled)
			{
				if (base.IsDisabled)
				{
					this.SetState("Disabled");
				}
				else if (this.IsSelected && this.DominantSelectedState)
				{
					this.SetState("Selected");
				}
				else if (base.IsPressed)
				{
					this.SetState("Pressed");
				}
				else if (base.IsHovered)
				{
					this.SetState("Hovered");
				}
				else if (this.IsSelected && !this.DominantSelectedState)
				{
					this.SetState("Selected");
				}
				else
				{
					this.SetState("Default");
				}
			}
			if (base.UpdateChildrenStates)
			{
				for (int i = 0; i < base.ChildCount; i++)
				{
					Widget child = base.GetChild(i);
					if (!(child is ImageWidget) || !((ImageWidget)child).OverrideDefaultStateSwitchingEnabled)
					{
						child.SetState(base.CurrentState);
					}
				}
			}
		}

		private void Refresh()
		{
			if (this.IsToggle)
			{
				this.ShowHideToggle();
			}
		}

		private void ShowHideToggle()
		{
			if (this.ToggleIndicator != null)
			{
				if (this._isSelected)
				{
					this.ToggleIndicator.Show();
					return;
				}
				this.ToggleIndicator.Hide();
			}
		}

		public ButtonWidget(UIContext context)
			: base(context)
		{
			base.FrictionEnabled = true;
		}

		protected internal override void OnMousePressed()
		{
			if (this._clickState == ButtonWidget.ButtonClickState.None)
			{
				this._clickState = ButtonWidget.ButtonClickState.HandlingClick;
				base.IsPressed = true;
				if (!base.DoNotPassEventsToChildren)
				{
					for (int i = 0; i < base.ChildCount; i++)
					{
						Widget child = base.GetChild(i);
						if (child != null)
						{
							child.IsPressed = true;
						}
					}
				}
			}
		}

		protected internal override void OnMouseReleased()
		{
			if (this._clickState == ButtonWidget.ButtonClickState.HandlingClick)
			{
				this._clickState = ButtonWidget.ButtonClickState.None;
				base.IsPressed = false;
				if (!base.DoNotPassEventsToChildren)
				{
					for (int i = 0; i < base.ChildCount; i++)
					{
						Widget child = base.GetChild(i);
						if (child != null)
						{
							child.IsPressed = false;
						}
					}
				}
				if (this.IsPointInsideMeasuredAreaAndCheckIfVisible())
				{
					this.HandleClick();
				}
			}
		}

		private bool IsPointInsideMeasuredAreaAndCheckIfVisible()
		{
			return base.IsPointInsideMeasuredArea(base.EventManager.MousePosition) && base.IsRecursivelyVisible();
		}

		protected internal override void OnMouseAlternatePressed()
		{
			if (this._clickState == ButtonWidget.ButtonClickState.None)
			{
				this._clickState = ButtonWidget.ButtonClickState.HandlingAlternateClick;
				base.IsPressed = true;
				if (!base.DoNotPassEventsToChildren)
				{
					for (int i = 0; i < base.ChildCount; i++)
					{
						Widget child = base.GetChild(i);
						if (child != null)
						{
							child.IsPressed = true;
						}
					}
				}
			}
		}

		protected internal override void OnMouseAlternateReleased()
		{
			if (this._clickState == ButtonWidget.ButtonClickState.HandlingAlternateClick)
			{
				this._clickState = ButtonWidget.ButtonClickState.None;
				base.IsPressed = false;
				if (!base.DoNotPassEventsToChildren)
				{
					for (int i = 0; i < base.ChildCount; i++)
					{
						Widget child = base.GetChild(i);
						if (child != null)
						{
							child.IsPressed = false;
						}
					}
				}
				if (this.IsPointInsideMeasuredAreaAndCheckIfVisible())
				{
					this.HandleAlternateClick();
				}
			}
		}

		protected virtual void HandleClick()
		{
			foreach (Action<Widget> action in this.ClickEventHandlers)
			{
				action(this);
			}
			bool isSelected = this.IsSelected;
			if (this.IsToggle)
			{
				this.IsSelected = !this.IsSelected;
			}
			else if (this.IsRadio)
			{
				this.IsSelected = true;
				if (this.IsSelected && !isSelected && base.ParentWidget is Container)
				{
					(base.ParentWidget as Container).OnChildSelected(this);
				}
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

		private void HandleAlternateClick()
		{
			this.OnAlternateClick();
			base.EventFired("AlternateClick", Array.Empty<object>());
		}

		protected virtual void OnClick()
		{
		}

		protected virtual void OnAlternateClick()
		{
		}

		public bool IsToggle
		{
			get
			{
				return this.ButtonType == ButtonType.Toggle;
			}
		}

		public bool IsRadio
		{
			get
			{
				return this.ButtonType == ButtonType.Radio;
			}
		}

		[Editor(false)]
		public Widget ToggleIndicator
		{
			get
			{
				return this._toggleIndicator;
			}
			set
			{
				if (this._toggleIndicator != value)
				{
					this._toggleIndicator = value;
					this.Refresh();
				}
			}
		}

		[Editor(false)]
		public bool IsSelected
		{
			get
			{
				return this._isSelected;
			}
			set
			{
				if (this._isSelected != value)
				{
					this._isSelected = value;
					this.Refresh();
					this.RefreshState();
					base.OnPropertyChanged(value, "IsSelected");
				}
			}
		}

		[Editor(false)]
		public bool DominantSelectedState
		{
			get
			{
				return this._dominantSelectedState;
			}
			set
			{
				if (this._dominantSelectedState != value)
				{
					this._dominantSelectedState = value;
					base.OnPropertyChanged(value, "DominantSelectedState");
				}
			}
		}

		protected const float _maxDoubleClickDeltaTimeInSeconds = 0.5f;

		protected float _lastClickTime;

		private ButtonWidget.ButtonClickState _clickState;

		private ButtonType _buttonType;

		public List<Action<Widget>> ClickEventHandlers = new List<Action<Widget>>();

		private Widget _toggleIndicator;

		private bool _isSelected;

		private bool _dominantSelectedState = true;

		private enum ButtonClickState
		{
			None,
			HandlingClick,
			HandlingAlternateClick
		}
	}
}
