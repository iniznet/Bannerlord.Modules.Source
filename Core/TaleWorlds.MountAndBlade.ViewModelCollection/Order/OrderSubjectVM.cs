using System;
using System.Collections.Generic;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.ViewModelCollection.Input;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Order
{
	// Token: 0x02000026 RID: 38
	public class OrderSubjectVM : ViewModel
	{
		// Token: 0x170000D7 RID: 215
		// (get) Token: 0x060002CE RID: 718 RVA: 0x0000D3D7 File Offset: 0x0000B5D7
		private bool _isGamepadActive
		{
			get
			{
				return Input.IsControllerConnected && !Input.IsMouseActive;
			}
		}

		// Token: 0x060002CF RID: 719 RVA: 0x0000D3EA File Offset: 0x0000B5EA
		public OrderSubjectVM()
		{
			this.ActiveOrders = new List<OrderItemVM>();
		}

		// Token: 0x170000D8 RID: 216
		// (get) Token: 0x060002D0 RID: 720 RVA: 0x0000D3FD File Offset: 0x0000B5FD
		// (set) Token: 0x060002D1 RID: 721 RVA: 0x0000D405 File Offset: 0x0000B605
		[DataSourceProperty]
		public bool IsSelectable
		{
			get
			{
				return this._isSelectable;
			}
			set
			{
				if (value != this._isSelectable)
				{
					this._isSelectable = value;
					base.OnPropertyChangedWithValue(value, "IsSelectable");
				}
			}
		}

		// Token: 0x170000D9 RID: 217
		// (get) Token: 0x060002D2 RID: 722 RVA: 0x0000D423 File Offset: 0x0000B623
		// (set) Token: 0x060002D3 RID: 723 RVA: 0x0000D42B File Offset: 0x0000B62B
		[DataSourceProperty]
		public bool IsSelected
		{
			get
			{
				return this._isSelected;
			}
			set
			{
				if ((!value || this.IsSelectable) && value != this._isSelected)
				{
					this._isSelected = value;
					base.OnPropertyChangedWithValue(value, "IsSelected");
				}
			}
		}

		// Token: 0x170000DA RID: 218
		// (get) Token: 0x060002D4 RID: 724 RVA: 0x0000D454 File Offset: 0x0000B654
		// (set) Token: 0x060002D5 RID: 725 RVA: 0x0000D45C File Offset: 0x0000B65C
		[DataSourceProperty]
		public bool IsSelectionActive
		{
			get
			{
				return this._isSelectionActive;
			}
			set
			{
				if (value != this._isSelectionActive)
				{
					this._isSelectionActive = value;
					base.OnPropertyChangedWithValue(value, "IsSelectionActive");
				}
			}
		}

		// Token: 0x170000DB RID: 219
		// (get) Token: 0x060002D6 RID: 726 RVA: 0x0000D47A File Offset: 0x0000B67A
		// (set) Token: 0x060002D7 RID: 727 RVA: 0x0000D482 File Offset: 0x0000B682
		[DataSourceProperty]
		public int BehaviorType
		{
			get
			{
				return this._behaviorType;
			}
			set
			{
				if (value != this._behaviorType)
				{
					this._behaviorType = value;
					base.OnPropertyChangedWithValue(value, "BehaviorType");
				}
			}
		}

		// Token: 0x170000DC RID: 220
		// (get) Token: 0x060002D8 RID: 728 RVA: 0x0000D4A0 File Offset: 0x0000B6A0
		// (set) Token: 0x060002D9 RID: 729 RVA: 0x0000D4A8 File Offset: 0x0000B6A8
		[DataSourceProperty]
		public int UnderAttackOfType
		{
			get
			{
				return this._underAttackOfType;
			}
			set
			{
				if (value != this._underAttackOfType)
				{
					this._underAttackOfType = value;
					base.OnPropertyChangedWithValue(value, "UnderAttackOfType");
				}
			}
		}

		// Token: 0x170000DD RID: 221
		// (get) Token: 0x060002DA RID: 730 RVA: 0x0000D4C6 File Offset: 0x0000B6C6
		// (set) Token: 0x060002DB RID: 731 RVA: 0x0000D4CE File Offset: 0x0000B6CE
		[DataSourceProperty]
		public string ShortcutText
		{
			get
			{
				return this._shortcutText;
			}
			set
			{
				if (value != this._shortcutText)
				{
					this._shortcutText = value;
					base.OnPropertyChangedWithValue<string>(value, "ShortcutText");
				}
			}
		}

		// Token: 0x170000DE RID: 222
		// (get) Token: 0x060002DC RID: 732 RVA: 0x0000D4F1 File Offset: 0x0000B6F1
		// (set) Token: 0x060002DD RID: 733 RVA: 0x0000D4F9 File Offset: 0x0000B6F9
		[DataSourceProperty]
		public InputKeyItemVM SelectionKey
		{
			get
			{
				return this._selectionKey;
			}
			set
			{
				if (value != this._selectionKey)
				{
					this._selectionKey = value;
					base.OnPropertyChangedWithValue<InputKeyItemVM>(value, "SelectionKey");
				}
			}
		}

		// Token: 0x0400015E RID: 350
		internal List<OrderItemVM> ActiveOrders;

		// Token: 0x0400015F RID: 351
		private int _behaviorType;

		// Token: 0x04000160 RID: 352
		private int _underAttackOfType;

		// Token: 0x04000161 RID: 353
		private bool _isSelectable;

		// Token: 0x04000162 RID: 354
		private bool _isSelected;

		// Token: 0x04000163 RID: 355
		private bool _isSelectionActive;

		// Token: 0x04000164 RID: 356
		private string _shortcutText;

		// Token: 0x04000165 RID: 357
		private InputKeyItemVM _selectionKey;
	}
}
