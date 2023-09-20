using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets
{
	// Token: 0x02000027 RID: 39
	public class NavigatableGridWidget : GridWidget
	{
		// Token: 0x170000A0 RID: 160
		// (get) Token: 0x060001DA RID: 474 RVA: 0x00007377 File Offset: 0x00005577
		// (set) Token: 0x060001DB RID: 475 RVA: 0x0000737F File Offset: 0x0000557F
		public ScrollablePanel ParentPanel { get; set; }

		// Token: 0x060001DC RID: 476 RVA: 0x00007388 File Offset: 0x00005588
		public NavigatableGridWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x060001DD RID: 477 RVA: 0x000073A4 File Offset: 0x000055A4
		protected override void OnLateUpdate(float dt)
		{
			if (this._areIndicesDirty)
			{
				for (int i = 0; i < base.ChildCount; i++)
				{
					base.Children[i].GamepadNavigationIndex = -1;
				}
				this.RefreshChildNavigationIndices();
				this._areIndicesDirty = false;
			}
		}

		// Token: 0x060001DE RID: 478 RVA: 0x000073E9 File Offset: 0x000055E9
		protected override void OnConnectedToRoot()
		{
			base.OnConnectedToRoot();
			if (this.ParentPanel == null)
			{
				this.ParentPanel = base.FindParentPanel();
			}
		}

		// Token: 0x060001DF RID: 479 RVA: 0x00007408 File Offset: 0x00005608
		protected override void OnChildAdded(Widget child)
		{
			base.OnChildAdded(child);
			this.SetNavigationIndexForChild(child);
			child.OnGamepadNavigationFocusGained = new Action<Widget>(this.OnWidgetGainedGamepadFocus);
			child.EventFire += this.OnChildSiblingIndexChanged;
			child.boolPropertyChanged += this.OnChildVisibilityChanged;
			this.UpdateEmptyNavigationWidget();
		}

		// Token: 0x060001E0 RID: 480 RVA: 0x00007460 File Offset: 0x00005660
		protected override void OnAfterChildRemoved(Widget child)
		{
			base.OnAfterChildRemoved(child);
			child.OnGamepadNavigationFocusGained = null;
			child.EventFire -= this.OnChildSiblingIndexChanged;
			child.boolPropertyChanged -= this.OnChildVisibilityChanged;
			child.GamepadNavigationIndex = -1;
			this.UpdateEmptyNavigationWidget();
		}

		// Token: 0x060001E1 RID: 481 RVA: 0x000074AC File Offset: 0x000056AC
		protected override void OnDisconnectedFromRoot()
		{
			base.OnDisconnectedFromRoot();
			for (int i = 0; i < base.Children.Count; i++)
			{
				base.Children[i].OnGamepadNavigationFocusGained = null;
				base.Children[i].EventFire -= this.OnChildSiblingIndexChanged;
				base.Children[i].boolPropertyChanged -= this.OnChildVisibilityChanged;
			}
		}

		// Token: 0x060001E2 RID: 482 RVA: 0x00007524 File Offset: 0x00005724
		private void OnChildVisibilityChanged(PropertyOwnerObject child, string propertyName, bool value)
		{
			if (propertyName == "IsVisible")
			{
				Widget widget = (Widget)child;
				if (!value)
				{
					widget.GamepadNavigationIndex = -1;
					return;
				}
				this.SetNavigationIndexForChild(widget);
			}
		}

		// Token: 0x060001E3 RID: 483 RVA: 0x00007557 File Offset: 0x00005757
		private void OnWidgetGainedGamepadFocus(Widget widget)
		{
			if (this.ParentPanel != null)
			{
				this.ParentPanel.ScrollToChild(widget, -1f, -1f, this.AutoScrollXOffset, this.AutoScrollYOffset, 0f, 0f);
			}
		}

		// Token: 0x060001E4 RID: 484 RVA: 0x0000758D File Offset: 0x0000578D
		private void OnChildSiblingIndexChanged(Widget widget, string eventName, object[] parameters)
		{
			if (eventName == "SiblingIndexChanged")
			{
				this._areIndicesDirty = true;
			}
		}

		// Token: 0x060001E5 RID: 485 RVA: 0x000075A4 File Offset: 0x000057A4
		private void SetNavigationIndexForChild(Widget widget)
		{
			int num = this.MinIndex + widget.GetSiblingIndex() * this.StepSize;
			if (num <= this.MaxIndex)
			{
				widget.GamepadNavigationIndex = num;
			}
		}

		// Token: 0x060001E6 RID: 486 RVA: 0x000075D6 File Offset: 0x000057D6
		protected override void OnGamepadNavigationIndexUpdated(int newIndex)
		{
			if (newIndex != -1 && this.UseSelfIndexForMinimum)
			{
				this.SetNavigationIndicesFromSelf();
			}
		}

		// Token: 0x060001E7 RID: 487 RVA: 0x000075EA File Offset: 0x000057EA
		private void SetNavigationIndicesFromSelf()
		{
			this.MinIndex = base.GamepadNavigationIndex;
			base.GamepadNavigationIndex = -1;
			this._areIndicesDirty = true;
		}

		// Token: 0x060001E8 RID: 488 RVA: 0x00007606 File Offset: 0x00005806
		private void UpdateEmptyNavigationWidget()
		{
			if (this._emptyNavigationWidget != null)
			{
				if (base.Children.Count == 0)
				{
					this.EmptyNavigationWidget.GamepadNavigationIndex = this.MinIndex;
					return;
				}
				this.EmptyNavigationWidget.GamepadNavigationIndex = -1;
			}
		}

		// Token: 0x060001E9 RID: 489 RVA: 0x0000763C File Offset: 0x0000583C
		protected void RefreshChildNavigationIndices()
		{
			for (int i = 0; i < base.Children.Count; i++)
			{
				this.SetNavigationIndexForChild(base.Children[i]);
			}
		}

		// Token: 0x170000A1 RID: 161
		// (get) Token: 0x060001EA RID: 490 RVA: 0x00007671 File Offset: 0x00005871
		// (set) Token: 0x060001EB RID: 491 RVA: 0x00007679 File Offset: 0x00005879
		public int AutoScrollXOffset { get; set; }

		// Token: 0x170000A2 RID: 162
		// (get) Token: 0x060001EC RID: 492 RVA: 0x00007682 File Offset: 0x00005882
		// (set) Token: 0x060001ED RID: 493 RVA: 0x0000768A File Offset: 0x0000588A
		public int AutoScrollYOffset { get; set; }

		// Token: 0x170000A3 RID: 163
		// (get) Token: 0x060001EE RID: 494 RVA: 0x00007693 File Offset: 0x00005893
		// (set) Token: 0x060001EF RID: 495 RVA: 0x0000769B File Offset: 0x0000589B
		public int MinIndex
		{
			get
			{
				return this._minIndex;
			}
			set
			{
				if (value != this._minIndex)
				{
					this._minIndex = value;
					this.RefreshChildNavigationIndices();
				}
			}
		}

		// Token: 0x170000A4 RID: 164
		// (get) Token: 0x060001F0 RID: 496 RVA: 0x000076B3 File Offset: 0x000058B3
		// (set) Token: 0x060001F1 RID: 497 RVA: 0x000076BB File Offset: 0x000058BB
		public int MaxIndex
		{
			get
			{
				return this._maxIndex;
			}
			set
			{
				if (value != this._maxIndex)
				{
					this._maxIndex = value;
					this.RefreshChildNavigationIndices();
				}
			}
		}

		// Token: 0x170000A5 RID: 165
		// (get) Token: 0x060001F2 RID: 498 RVA: 0x000076D3 File Offset: 0x000058D3
		// (set) Token: 0x060001F3 RID: 499 RVA: 0x000076DB File Offset: 0x000058DB
		public int StepSize
		{
			get
			{
				return this._stepSize;
			}
			set
			{
				if (value != this._stepSize)
				{
					this._stepSize = value;
					this.RefreshChildNavigationIndices();
				}
			}
		}

		// Token: 0x170000A6 RID: 166
		// (get) Token: 0x060001F4 RID: 500 RVA: 0x000076F3 File Offset: 0x000058F3
		// (set) Token: 0x060001F5 RID: 501 RVA: 0x000076FB File Offset: 0x000058FB
		public bool UseSelfIndexForMinimum
		{
			get
			{
				return this._useSelfIndexForMinimum;
			}
			set
			{
				if (value != this._useSelfIndexForMinimum)
				{
					this._useSelfIndexForMinimum = value;
					if (this._useSelfIndexForMinimum && base.GamepadNavigationIndex != -1)
					{
						this.SetNavigationIndicesFromSelf();
					}
				}
			}
		}

		// Token: 0x170000A7 RID: 167
		// (get) Token: 0x060001F6 RID: 502 RVA: 0x00007724 File Offset: 0x00005924
		// (set) Token: 0x060001F7 RID: 503 RVA: 0x0000772C File Offset: 0x0000592C
		public Widget EmptyNavigationWidget
		{
			get
			{
				return this._emptyNavigationWidget;
			}
			set
			{
				if (value != this._emptyNavigationWidget)
				{
					if (this._emptyNavigationWidget != null)
					{
						this._emptyNavigationWidget.GamepadNavigationIndex = -1;
					}
					this._emptyNavigationWidget = value;
					this.UpdateEmptyNavigationWidget();
				}
			}
		}

		// Token: 0x040000E7 RID: 231
		private bool _areIndicesDirty;

		// Token: 0x040000EA RID: 234
		private int _minIndex;

		// Token: 0x040000EB RID: 235
		private int _maxIndex = int.MaxValue;

		// Token: 0x040000EC RID: 236
		private int _stepSize = 1;

		// Token: 0x040000ED RID: 237
		private bool _useSelfIndexForMinimum;

		// Token: 0x040000EE RID: 238
		private Widget _emptyNavigationWidget;
	}
}
