using System;
using System.Numerics;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Inventory
{
	// Token: 0x02000119 RID: 281
	public class InventoryAlternativeUsageContainer : Container
	{
		// Token: 0x06000E49 RID: 3657 RVA: 0x00027AB8 File Offset: 0x00025CB8
		public InventoryAlternativeUsageContainer(UIContext context)
			: base(context)
		{
		}

		// Token: 0x06000E4A RID: 3658 RVA: 0x00027AE0 File Offset: 0x00025CE0
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

		// Token: 0x06000E4B RID: 3659 RVA: 0x00027B14 File Offset: 0x00025D14
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

		// Token: 0x06000E4C RID: 3660 RVA: 0x00027B84 File Offset: 0x00025D84
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

		// Token: 0x06000E4D RID: 3661 RVA: 0x00027C0C File Offset: 0x00025E0C
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

		// Token: 0x17000515 RID: 1301
		// (get) Token: 0x06000E4E RID: 3662 RVA: 0x00027CE1 File Offset: 0x00025EE1
		// (set) Token: 0x06000E4F RID: 3663 RVA: 0x00027CE9 File Offset: 0x00025EE9
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

		// Token: 0x17000516 RID: 1302
		// (get) Token: 0x06000E50 RID: 3664 RVA: 0x00027D07 File Offset: 0x00025F07
		// (set) Token: 0x06000E51 RID: 3665 RVA: 0x00027D0F File Offset: 0x00025F0F
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

		// Token: 0x17000517 RID: 1303
		// (get) Token: 0x06000E52 RID: 3666 RVA: 0x00027D2D File Offset: 0x00025F2D
		// (set) Token: 0x06000E53 RID: 3667 RVA: 0x00027D35 File Offset: 0x00025F35
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

		// Token: 0x17000518 RID: 1304
		// (get) Token: 0x06000E54 RID: 3668 RVA: 0x00027D53 File Offset: 0x00025F53
		// (set) Token: 0x06000E55 RID: 3669 RVA: 0x00027D5B File Offset: 0x00025F5B
		public override Predicate<Widget> AcceptDropPredicate { get; set; }

		// Token: 0x06000E56 RID: 3670 RVA: 0x00027D64 File Offset: 0x00025F64
		public override Vector2 GetDropGizmoPosition(Vector2 draggedWidgetPosition)
		{
			return Vector2.Zero;
		}

		// Token: 0x06000E57 RID: 3671 RVA: 0x00027D6B File Offset: 0x00025F6B
		public override int GetIndexForDrop(Vector2 draggedWidgetPosition)
		{
			return -1;
		}

		// Token: 0x17000519 RID: 1305
		// (get) Token: 0x06000E58 RID: 3672 RVA: 0x00027D6E File Offset: 0x00025F6E
		public override bool IsDragHovering { get; }

		// Token: 0x04000691 RID: 1681
		private int _columnLimit = 2;

		// Token: 0x04000692 RID: 1682
		private float _cellWidth = 100f;

		// Token: 0x04000693 RID: 1683
		private float _cellHeight = 100f;
	}
}
