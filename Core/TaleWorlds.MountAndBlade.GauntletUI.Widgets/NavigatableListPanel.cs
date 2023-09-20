using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.GauntletUI.Layout;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets
{
	// Token: 0x02000028 RID: 40
	public class NavigatableListPanel : ListPanel
	{
		// Token: 0x170000A8 RID: 168
		// (get) Token: 0x060001F8 RID: 504 RVA: 0x00007758 File Offset: 0x00005958
		// (set) Token: 0x060001F9 RID: 505 RVA: 0x00007760 File Offset: 0x00005960
		public ScrollablePanel ParentPanel { get; set; }

		// Token: 0x060001FA RID: 506 RVA: 0x00007769 File Offset: 0x00005969
		public NavigatableListPanel(UIContext context)
			: base(context)
		{
		}

		// Token: 0x060001FB RID: 507 RVA: 0x00007784 File Offset: 0x00005984
		protected override void OnLateUpdate(float dt)
		{
			if (this._areIndicesDirty)
			{
				this.RefreshChildNavigationIndices();
				this._areIndicesDirty = false;
			}
		}

		// Token: 0x060001FC RID: 508 RVA: 0x0000779B File Offset: 0x0000599B
		protected override void OnConnectedToRoot()
		{
			base.OnConnectedToRoot();
			if (this.ParentPanel == null)
			{
				this.ParentPanel = base.FindParentPanel();
			}
		}

		// Token: 0x060001FD RID: 509 RVA: 0x000077B8 File Offset: 0x000059B8
		protected override void OnChildAdded(Widget child)
		{
			base.OnChildAdded(child);
			child.OnGamepadNavigationFocusGained = new Action<Widget>(this.OnWidgetGainedGamepadFocus);
			child.EventFire += this.OnChildSiblingIndexChanged;
			child.boolPropertyChanged += this.OnChildVisibilityChanged;
			this._areIndicesDirty = true;
			this.UpdateEmptyNavigationWidget();
		}

		// Token: 0x060001FE RID: 510 RVA: 0x00007810 File Offset: 0x00005A10
		protected override void OnAfterChildRemoved(Widget child)
		{
			base.OnAfterChildRemoved(child);
			child.OnGamepadNavigationFocusGained = null;
			child.EventFire -= this.OnChildSiblingIndexChanged;
			child.boolPropertyChanged -= this.OnChildVisibilityChanged;
			child.GamepadNavigationIndex = -1;
			this.UpdateEmptyNavigationWidget();
		}

		// Token: 0x060001FF RID: 511 RVA: 0x0000785C File Offset: 0x00005A5C
		protected override void OnDisconnectedFromRoot()
		{
			base.OnDisconnectedFromRoot();
			for (int i = 0; i < base.Children.Count; i++)
			{
				base.Children[i].OnGamepadNavigationFocusGained = null;
				base.Children[i].EventFire -= this.OnChildSiblingIndexChanged;
				base.Children[i].boolPropertyChanged -= this.OnChildVisibilityChanged;
				base.Children[i].GamepadNavigationIndex = -1;
			}
		}

		// Token: 0x06000200 RID: 512 RVA: 0x000078E4 File Offset: 0x00005AE4
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

		// Token: 0x06000201 RID: 513 RVA: 0x00007917 File Offset: 0x00005B17
		private void OnWidgetGainedGamepadFocus(Widget widget)
		{
			if (this.ParentPanel != null)
			{
				this.ParentPanel.ScrollToChild(widget, -1f, -1f, this.AutoScrollXOffset, this.AutoScrollYOffset, 0f, 0f);
			}
		}

		// Token: 0x06000202 RID: 514 RVA: 0x0000794D File Offset: 0x00005B4D
		private void OnChildSiblingIndexChanged(Widget widget, string eventName, object[] parameters)
		{
			if (eventName == "SiblingIndexChanged")
			{
				this._areIndicesDirty = true;
			}
		}

		// Token: 0x06000203 RID: 515 RVA: 0x00007964 File Offset: 0x00005B64
		private void SetNavigationIndexForChild(Widget widget)
		{
			int num;
			if (base.StackLayout.LayoutMethod == LayoutMethod.VerticalTopToBottom || base.StackLayout.LayoutMethod == LayoutMethod.HorizontalRightToLeft)
			{
				num = this.MaxIndex - widget.GetSiblingIndex() * this.StepSize;
			}
			else
			{
				num = this.MinIndex + widget.GetSiblingIndex() * this.StepSize;
			}
			if (num <= this.MaxIndex)
			{
				widget.GamepadNavigationIndex = num;
			}
		}

		// Token: 0x06000204 RID: 516 RVA: 0x000079C9 File Offset: 0x00005BC9
		protected override void OnGamepadNavigationIndexUpdated(int newIndex)
		{
			if (newIndex != -1 && this.UseSelfIndexForMinimum)
			{
				this.SetNavigationIndicesFromSelf();
			}
		}

		// Token: 0x06000205 RID: 517 RVA: 0x000079DD File Offset: 0x00005BDD
		private void SetNavigationIndicesFromSelf()
		{
			this.MinIndex = base.GamepadNavigationIndex;
			base.GamepadNavigationIndex = -1;
			this._areIndicesDirty = true;
		}

		// Token: 0x06000206 RID: 518 RVA: 0x000079FC File Offset: 0x00005BFC
		protected void RefreshChildNavigationIndices()
		{
			for (int i = 0; i < base.Children.Count; i++)
			{
				this.SetNavigationIndexForChild(base.Children[i]);
			}
		}

		// Token: 0x06000207 RID: 519 RVA: 0x00007A31 File Offset: 0x00005C31
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

		// Token: 0x170000A9 RID: 169
		// (get) Token: 0x06000208 RID: 520 RVA: 0x00007A66 File Offset: 0x00005C66
		// (set) Token: 0x06000209 RID: 521 RVA: 0x00007A6E File Offset: 0x00005C6E
		public int AutoScrollXOffset { get; set; }

		// Token: 0x170000AA RID: 170
		// (get) Token: 0x0600020A RID: 522 RVA: 0x00007A77 File Offset: 0x00005C77
		// (set) Token: 0x0600020B RID: 523 RVA: 0x00007A7F File Offset: 0x00005C7F
		public int AutoScrollYOffset { get; set; }

		// Token: 0x170000AB RID: 171
		// (get) Token: 0x0600020C RID: 524 RVA: 0x00007A88 File Offset: 0x00005C88
		// (set) Token: 0x0600020D RID: 525 RVA: 0x00007A90 File Offset: 0x00005C90
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

		// Token: 0x170000AC RID: 172
		// (get) Token: 0x0600020E RID: 526 RVA: 0x00007AA8 File Offset: 0x00005CA8
		// (set) Token: 0x0600020F RID: 527 RVA: 0x00007AB0 File Offset: 0x00005CB0
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

		// Token: 0x170000AD RID: 173
		// (get) Token: 0x06000210 RID: 528 RVA: 0x00007AC8 File Offset: 0x00005CC8
		// (set) Token: 0x06000211 RID: 529 RVA: 0x00007AD0 File Offset: 0x00005CD0
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

		// Token: 0x170000AE RID: 174
		// (get) Token: 0x06000212 RID: 530 RVA: 0x00007AE8 File Offset: 0x00005CE8
		// (set) Token: 0x06000213 RID: 531 RVA: 0x00007AF0 File Offset: 0x00005CF0
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

		// Token: 0x170000AF RID: 175
		// (get) Token: 0x06000214 RID: 532 RVA: 0x00007B19 File Offset: 0x00005D19
		// (set) Token: 0x06000215 RID: 533 RVA: 0x00007B21 File Offset: 0x00005D21
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

		// Token: 0x040000EF RID: 239
		private bool _areIndicesDirty;

		// Token: 0x040000F1 RID: 241
		private int _minIndex;

		// Token: 0x040000F2 RID: 242
		private int _maxIndex = int.MaxValue;

		// Token: 0x040000F3 RID: 243
		private int _stepSize = 1;

		// Token: 0x040000F4 RID: 244
		private bool _useSelfIndexForMinimum;

		// Token: 0x040000F5 RID: 245
		private Widget _emptyNavigationWidget;
	}
}
