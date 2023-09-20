using System;
using System.Numerics;
using TaleWorlds.GauntletUI.Layout;

namespace TaleWorlds.GauntletUI.BaseTypes
{
	// Token: 0x0200005E RID: 94
	public class ListPanel : Container
	{
		// Token: 0x170001B9 RID: 441
		// (get) Token: 0x06000600 RID: 1536 RVA: 0x0001AE08 File Offset: 0x00019008
		// (set) Token: 0x06000601 RID: 1537 RVA: 0x0001AE10 File Offset: 0x00019010
		public StackLayout StackLayout { get; private set; }

		// Token: 0x06000602 RID: 1538 RVA: 0x0001AE19 File Offset: 0x00019019
		public ListPanel(UIContext context)
			: base(context)
		{
			this.StackLayout = new StackLayout();
			base.LayoutImp = this.StackLayout;
		}

		// Token: 0x06000603 RID: 1539 RVA: 0x0001AE39 File Offset: 0x00019039
		protected override void OnUpdate(float dt)
		{
			base.OnUpdate(dt);
			this.UpdateListPanel();
			if (this.ResetSelectedOnLosingFocus && !base.CheckIsMyChildRecursive(base.EventManager.LatestMouseDownWidget))
			{
				base.IntValue = -1;
			}
		}

		// Token: 0x06000604 RID: 1540 RVA: 0x0001AE6A File Offset: 0x0001906A
		private void UpdateListPanel()
		{
			if (base.AcceptDrop && this.IsDragHovering)
			{
				base.DragHoverInsertionIndex = this.GetIndexForDrop(base.EventManager.DraggedWidgetPosition);
			}
		}

		// Token: 0x170001BA RID: 442
		// (get) Token: 0x06000605 RID: 1541 RVA: 0x0001AE93 File Offset: 0x00019093
		// (set) Token: 0x06000606 RID: 1542 RVA: 0x0001AE9B File Offset: 0x0001909B
		public override Predicate<Widget> AcceptDropPredicate { get; set; }

		// Token: 0x06000607 RID: 1543 RVA: 0x0001AEA4 File Offset: 0x000190A4
		public override int GetIndexForDrop(Vector2 draggedWidgetPosition)
		{
			return this.StackLayout.GetIndexForDrop(this, draggedWidgetPosition);
		}

		// Token: 0x06000608 RID: 1544 RVA: 0x0001AEB3 File Offset: 0x000190B3
		public override Vector2 GetDropGizmoPosition(Vector2 draggedWidgetPosition)
		{
			return this.StackLayout.GetDropGizmoPosition(this, draggedWidgetPosition);
		}

		// Token: 0x06000609 RID: 1545 RVA: 0x0001AEC4 File Offset: 0x000190C4
		public override void OnChildSelected(Widget widget)
		{
			int num = -1;
			for (int i = 0; i < base.ChildCount; i++)
			{
				if (widget == base.GetChild(i))
				{
					num = i;
				}
			}
			base.IntValue = num;
		}

		// Token: 0x0600060A RID: 1546 RVA: 0x0001AEF7 File Offset: 0x000190F7
		protected internal override void OnDragHoverBegin()
		{
			this._dragHovering = true;
			base.SetMeasureAndLayoutDirty();
		}

		// Token: 0x0600060B RID: 1547 RVA: 0x0001AF06 File Offset: 0x00019106
		protected internal override void OnDragHoverEnd()
		{
			this._dragHovering = false;
			base.SetMeasureAndLayoutDirty();
		}

		// Token: 0x0600060C RID: 1548 RVA: 0x0001AF15 File Offset: 0x00019115
		protected override bool OnPreviewDragHover()
		{
			return base.AcceptDrop;
		}

		// Token: 0x170001BB RID: 443
		// (get) Token: 0x0600060D RID: 1549 RVA: 0x0001AF1D File Offset: 0x0001911D
		public override bool IsDragHovering
		{
			get
			{
				return this._dragHovering;
			}
		}

		// Token: 0x170001BC RID: 444
		// (get) Token: 0x0600060E RID: 1550 RVA: 0x0001AF25 File Offset: 0x00019125
		// (set) Token: 0x0600060F RID: 1551 RVA: 0x0001AF2D File Offset: 0x0001912D
		[Editor(false)]
		public bool ResetSelectedOnLosingFocus
		{
			get
			{
				return this._resetSelectedOnLosingFocus;
			}
			set
			{
				if (this._resetSelectedOnLosingFocus != value)
				{
					this._resetSelectedOnLosingFocus = value;
					base.OnPropertyChanged(value, "ResetSelectedOnLosingFocus");
				}
			}
		}

		// Token: 0x040002DD RID: 733
		private bool _dragHovering;

		// Token: 0x040002DE RID: 734
		private bool _resetSelectedOnLosingFocus;
	}
}
