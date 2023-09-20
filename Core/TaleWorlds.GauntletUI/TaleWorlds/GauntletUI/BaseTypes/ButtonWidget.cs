using System;
using System.Collections.Generic;

namespace TaleWorlds.GauntletUI.BaseTypes
{
	// Token: 0x02000054 RID: 84
	public class ButtonWidget : ImageWidget
	{
		// Token: 0x1700018D RID: 397
		// (get) Token: 0x06000555 RID: 1365 RVA: 0x0001736F File Offset: 0x0001556F
		// (set) Token: 0x06000556 RID: 1366 RVA: 0x00017377 File Offset: 0x00015577
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

		// Token: 0x06000557 RID: 1367 RVA: 0x0001738F File Offset: 0x0001558F
		protected override bool OnPreviewMousePressed()
		{
			base.OnPreviewMousePressed();
			return true;
		}

		// Token: 0x06000558 RID: 1368 RVA: 0x0001739C File Offset: 0x0001559C
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

		// Token: 0x06000559 RID: 1369 RVA: 0x00017480 File Offset: 0x00015680
		private void Refresh()
		{
			if (this.IsToggle)
			{
				this.ShowHideToggle();
			}
		}

		// Token: 0x0600055A RID: 1370 RVA: 0x00017490 File Offset: 0x00015690
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

		// Token: 0x0600055B RID: 1371 RVA: 0x000174B9 File Offset: 0x000156B9
		public ButtonWidget(UIContext context)
			: base(context)
		{
			base.FrictionEnabled = true;
		}

		// Token: 0x0600055C RID: 1372 RVA: 0x000174DC File Offset: 0x000156DC
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

		// Token: 0x0600055D RID: 1373 RVA: 0x0001752C File Offset: 0x0001572C
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

		// Token: 0x0600055E RID: 1374 RVA: 0x00017589 File Offset: 0x00015789
		private bool IsPointInsideMeasuredAreaAndCheckIfVisible()
		{
			return base.IsPointInsideMeasuredArea(base.EventManager.MousePosition) && base.IsRecursivelyVisible();
		}

		// Token: 0x0600055F RID: 1375 RVA: 0x000175AC File Offset: 0x000157AC
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

		// Token: 0x06000560 RID: 1376 RVA: 0x000175FC File Offset: 0x000157FC
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

		// Token: 0x06000561 RID: 1377 RVA: 0x0001765C File Offset: 0x0001585C
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

		// Token: 0x06000562 RID: 1378 RVA: 0x00017760 File Offset: 0x00015960
		private void HandleAlternateClick()
		{
			this.OnAlternateClick();
			base.EventFired("AlternateClick", Array.Empty<object>());
		}

		// Token: 0x06000563 RID: 1379 RVA: 0x00017778 File Offset: 0x00015978
		protected virtual void OnClick()
		{
		}

		// Token: 0x06000564 RID: 1380 RVA: 0x0001777A File Offset: 0x0001597A
		protected virtual void OnAlternateClick()
		{
		}

		// Token: 0x1700018E RID: 398
		// (get) Token: 0x06000565 RID: 1381 RVA: 0x0001777C File Offset: 0x0001597C
		public bool IsToggle
		{
			get
			{
				return this.ButtonType == ButtonType.Toggle;
			}
		}

		// Token: 0x1700018F RID: 399
		// (get) Token: 0x06000566 RID: 1382 RVA: 0x00017787 File Offset: 0x00015987
		public bool IsRadio
		{
			get
			{
				return this.ButtonType == ButtonType.Radio;
			}
		}

		// Token: 0x17000190 RID: 400
		// (get) Token: 0x06000567 RID: 1383 RVA: 0x00017792 File Offset: 0x00015992
		// (set) Token: 0x06000568 RID: 1384 RVA: 0x0001779A File Offset: 0x0001599A
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

		// Token: 0x17000191 RID: 401
		// (get) Token: 0x06000569 RID: 1385 RVA: 0x000177B2 File Offset: 0x000159B2
		// (set) Token: 0x0600056A RID: 1386 RVA: 0x000177BA File Offset: 0x000159BA
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

		// Token: 0x17000192 RID: 402
		// (get) Token: 0x0600056B RID: 1387 RVA: 0x000177E4 File Offset: 0x000159E4
		// (set) Token: 0x0600056C RID: 1388 RVA: 0x000177EC File Offset: 0x000159EC
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

		// Token: 0x04000293 RID: 659
		protected const float _maxDoubleClickDeltaTimeInSeconds = 0.5f;

		// Token: 0x04000294 RID: 660
		protected float _lastClickTime;

		// Token: 0x04000295 RID: 661
		private ButtonWidget.ButtonClickState _clickState;

		// Token: 0x04000296 RID: 662
		private ButtonType _buttonType;

		// Token: 0x04000297 RID: 663
		public List<Action<Widget>> ClickEventHandlers = new List<Action<Widget>>();

		// Token: 0x04000298 RID: 664
		private Widget _toggleIndicator;

		// Token: 0x04000299 RID: 665
		private bool _isSelected;

		// Token: 0x0400029A RID: 666
		private bool _dominantSelectedState = true;

		// Token: 0x02000087 RID: 135
		private enum ButtonClickState
		{
			// Token: 0x0400044A RID: 1098
			None,
			// Token: 0x0400044B RID: 1099
			HandlingClick,
			// Token: 0x0400044C RID: 1100
			HandlingAlternateClick
		}
	}
}
