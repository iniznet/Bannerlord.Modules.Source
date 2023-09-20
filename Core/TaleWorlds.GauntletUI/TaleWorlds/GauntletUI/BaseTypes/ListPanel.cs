using System;
using System.Numerics;
using TaleWorlds.GauntletUI.Layout;

namespace TaleWorlds.GauntletUI.BaseTypes
{
	public class ListPanel : Container
	{
		public StackLayout StackLayout { get; private set; }

		public ListPanel(UIContext context)
			: base(context)
		{
			this.StackLayout = new StackLayout();
			base.LayoutImp = this.StackLayout;
		}

		protected override void OnUpdate(float dt)
		{
			base.OnUpdate(dt);
			this.UpdateListPanel();
			if (this.ResetSelectedOnLosingFocus && !base.CheckIsMyChildRecursive(base.EventManager.LatestMouseDownWidget))
			{
				base.IntValue = -1;
			}
		}

		private void UpdateListPanel()
		{
			if (base.AcceptDrop && this.IsDragHovering)
			{
				base.DragHoverInsertionIndex = this.GetIndexForDrop(base.EventManager.DraggedWidgetPosition);
			}
		}

		public override Predicate<Widget> AcceptDropPredicate { get; set; }

		public override int GetIndexForDrop(Vector2 draggedWidgetPosition)
		{
			return this.StackLayout.GetIndexForDrop(this, draggedWidgetPosition);
		}

		public override Vector2 GetDropGizmoPosition(Vector2 draggedWidgetPosition)
		{
			return this.StackLayout.GetDropGizmoPosition(this, draggedWidgetPosition);
		}

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

		protected internal override void OnDragHoverBegin()
		{
			this._dragHovering = true;
			base.SetMeasureAndLayoutDirty();
		}

		protected internal override void OnDragHoverEnd()
		{
			this._dragHovering = false;
			base.SetMeasureAndLayoutDirty();
		}

		protected override bool OnPreviewDragHover()
		{
			return base.AcceptDrop;
		}

		public override bool IsDragHovering
		{
			get
			{
				return this._dragHovering;
			}
		}

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

		private bool _dragHovering;

		private bool _resetSelectedOnLosingFocus;
	}
}
