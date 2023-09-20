using System;
using System.Numerics;
using TaleWorlds.GauntletUI.Layout;

namespace TaleWorlds.GauntletUI.BaseTypes
{
	public class GridWidget : Container
	{
		[Editor(false)]
		public float DefaultCellWidth
		{
			get
			{
				return this._defaultCellWidth;
			}
			set
			{
				if (this._defaultCellWidth != value)
				{
					this._defaultCellWidth = value;
					base.OnPropertyChanged(value, "DefaultCellWidth");
				}
			}
		}

		public float DefaultScaledCellWidth
		{
			get
			{
				return this.DefaultCellWidth * base._scaleToUse;
			}
		}

		[Editor(false)]
		public float DefaultCellHeight
		{
			get
			{
				return this._defaultCellHeight;
			}
			set
			{
				if (this._defaultCellHeight != value)
				{
					this._defaultCellHeight = value;
					base.OnPropertyChanged(value, "DefaultCellHeight");
				}
			}
		}

		public float DefaultScaledCellHeight
		{
			get
			{
				return this.DefaultCellHeight * base._scaleToUse;
			}
		}

		[Editor(false)]
		public int RowCount
		{
			get
			{
				return this._rowCount;
			}
			set
			{
				if (this._rowCount != value)
				{
					this._rowCount = value;
					base.OnPropertyChanged(value, "RowCount");
				}
			}
		}

		[Editor(false)]
		public int ColumnCount
		{
			get
			{
				return this._columnCount;
			}
			set
			{
				if (this._columnCount != value)
				{
					this._columnCount = value;
					base.OnPropertyChanged(value, "ColumnCount");
				}
			}
		}

		public override Predicate<Widget> AcceptDropPredicate { get; set; }

		public override bool IsDragHovering
		{
			get
			{
				return false;
			}
		}

		public GridWidget(UIContext context)
			: base(context)
		{
			base.LayoutImp = new GridLayout();
			this.RowCount = 3;
			this.ColumnCount = 3;
		}

		public override Vector2 GetDropGizmoPosition(Vector2 draggedWidgetPosition)
		{
			throw new NotImplementedException();
		}

		public override int GetIndexForDrop(Vector2 draggedWidgetPosition)
		{
			throw new NotImplementedException();
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

		private float _defaultCellWidth;

		private float _defaultCellHeight;

		private int _rowCount;

		private int _columnCount;
	}
}
