using System;
using System.Numerics;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Inventory
{
	public class InventoryAlternativeUsageContainer : Container
	{
		public InventoryAlternativeUsageContainer(UIContext context)
			: base(context)
		{
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

		protected override void OnChildAdded(Widget child)
		{
			base.OnChildAdded(child);
			foreach (Action<Widget, Widget> action in this.ItemAddEventHandlers)
			{
				action(this, child);
			}
			base.EventFired("ItemAdd", Array.Empty<object>());
			this.SetChildrenLayout();
		}

		protected override void OnChildRemoved(Widget child)
		{
			base.OnChildRemoved(child);
			if (base.IntValue < base.ChildCount - 1)
			{
				base.IntValue = -1;
			}
			foreach (Action<Widget, Widget> action in this.ItemRemoveEventHandlers)
			{
				action(this, child);
			}
			base.EventFired("ItemRemove", Array.Empty<object>());
			this.SetChildrenLayout();
		}

		private void SetChildrenLayout()
		{
			if (base.ChildCount == 0)
			{
				return;
			}
			int num = MathF.Ceiling((float)base.ChildCount / (float)this.ColumnLimit);
			for (int i = 0; i < num; i++)
			{
				int num2 = MathF.Min(this.ColumnLimit, base.ChildCount - (num - 1) * this.ColumnLimit);
				int num3 = i * (int)this.CellHeight;
				for (int j = 0; j < num2; j++)
				{
					int num4 = (int)(((float)j - ((float)num2 - 1f) / 2f) * this.CellWidth);
					int num5 = i * this.ColumnLimit + j;
					Widget child = base.GetChild(num5);
					if (num4 > 0)
					{
						child.MarginLeft = (float)(num4 * 2);
					}
					else if (num4 < 0)
					{
						child.MarginRight = (float)(-(float)num4 * 2);
					}
					child.MarginTop = (float)num3;
				}
			}
		}

		[Editor(false)]
		public int ColumnLimit
		{
			get
			{
				return this._columnLimit;
			}
			set
			{
				if (this._columnLimit != value)
				{
					this._columnLimit = value;
					base.OnPropertyChanged(value, "ColumnLimit");
				}
			}
		}

		[Editor(false)]
		public float CellWidth
		{
			get
			{
				return this._cellWidth;
			}
			set
			{
				if (this._cellWidth != value)
				{
					this._cellWidth = value;
					base.OnPropertyChanged(value, "CellWidth");
				}
			}
		}

		[Editor(false)]
		public float CellHeight
		{
			get
			{
				return this._cellHeight;
			}
			set
			{
				if (this._cellHeight != value)
				{
					this._cellHeight = value;
					base.OnPropertyChanged(value, "CellHeight");
				}
			}
		}

		public override Predicate<Widget> AcceptDropPredicate { get; set; }

		public override Vector2 GetDropGizmoPosition(Vector2 draggedWidgetPosition)
		{
			return Vector2.Zero;
		}

		public override int GetIndexForDrop(Vector2 draggedWidgetPosition)
		{
			return -1;
		}

		public override bool IsDragHovering { get; }

		private int _columnLimit = 2;

		private float _cellWidth = 100f;

		private float _cellHeight = 100f;
	}
}
