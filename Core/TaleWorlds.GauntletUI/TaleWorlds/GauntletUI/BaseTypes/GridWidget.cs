using System;
using System.Numerics;
using TaleWorlds.GauntletUI.Layout;

namespace TaleWorlds.GauntletUI.BaseTypes
{
	// Token: 0x0200005B RID: 91
	public class GridWidget : Container
	{
		// Token: 0x170001AB RID: 427
		// (get) Token: 0x060005DB RID: 1499 RVA: 0x0001A2F7 File Offset: 0x000184F7
		// (set) Token: 0x060005DC RID: 1500 RVA: 0x0001A2FF File Offset: 0x000184FF
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

		// Token: 0x170001AC RID: 428
		// (get) Token: 0x060005DD RID: 1501 RVA: 0x0001A31D File Offset: 0x0001851D
		public float DefaultScaledCellWidth
		{
			get
			{
				return this.DefaultCellWidth * base._scaleToUse;
			}
		}

		// Token: 0x170001AD RID: 429
		// (get) Token: 0x060005DE RID: 1502 RVA: 0x0001A32C File Offset: 0x0001852C
		// (set) Token: 0x060005DF RID: 1503 RVA: 0x0001A334 File Offset: 0x00018534
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

		// Token: 0x170001AE RID: 430
		// (get) Token: 0x060005E0 RID: 1504 RVA: 0x0001A352 File Offset: 0x00018552
		public float DefaultScaledCellHeight
		{
			get
			{
				return this.DefaultCellHeight * base._scaleToUse;
			}
		}

		// Token: 0x170001AF RID: 431
		// (get) Token: 0x060005E1 RID: 1505 RVA: 0x0001A361 File Offset: 0x00018561
		// (set) Token: 0x060005E2 RID: 1506 RVA: 0x0001A369 File Offset: 0x00018569
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

		// Token: 0x170001B0 RID: 432
		// (get) Token: 0x060005E3 RID: 1507 RVA: 0x0001A387 File Offset: 0x00018587
		// (set) Token: 0x060005E4 RID: 1508 RVA: 0x0001A38F File Offset: 0x0001858F
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

		// Token: 0x170001B1 RID: 433
		// (get) Token: 0x060005E5 RID: 1509 RVA: 0x0001A3AD File Offset: 0x000185AD
		// (set) Token: 0x060005E6 RID: 1510 RVA: 0x0001A3B5 File Offset: 0x000185B5
		public override Predicate<Widget> AcceptDropPredicate { get; set; }

		// Token: 0x170001B2 RID: 434
		// (get) Token: 0x060005E7 RID: 1511 RVA: 0x0001A3BE File Offset: 0x000185BE
		public override bool IsDragHovering
		{
			get
			{
				return false;
			}
		}

		// Token: 0x060005E8 RID: 1512 RVA: 0x0001A3C1 File Offset: 0x000185C1
		public GridWidget(UIContext context)
			: base(context)
		{
			base.LayoutImp = new GridLayout();
			this.RowCount = 3;
			this.ColumnCount = 3;
		}

		// Token: 0x060005E9 RID: 1513 RVA: 0x0001A3E3 File Offset: 0x000185E3
		public override Vector2 GetDropGizmoPosition(Vector2 draggedWidgetPosition)
		{
			throw new NotImplementedException();
		}

		// Token: 0x060005EA RID: 1514 RVA: 0x0001A3EA File Offset: 0x000185EA
		public override int GetIndexForDrop(Vector2 draggedWidgetPosition)
		{
			throw new NotImplementedException();
		}

		// Token: 0x060005EB RID: 1515 RVA: 0x0001A3F4 File Offset: 0x000185F4
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

		// Token: 0x040002D2 RID: 722
		private float _defaultCellWidth;

		// Token: 0x040002D3 RID: 723
		private float _defaultCellHeight;

		// Token: 0x040002D4 RID: 724
		private int _rowCount;

		// Token: 0x040002D5 RID: 725
		private int _columnCount;
	}
}
