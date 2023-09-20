using System;
using System.Collections.Generic;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.ViewModelCollection.Input;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Order
{
	public class OrderSubjectVM : ViewModel
	{
		private bool _isGamepadActive
		{
			get
			{
				return Input.IsControllerConnected && !Input.IsMouseActive;
			}
		}

		public OrderSubjectVM()
		{
			this.ActiveOrders = new List<OrderItemVM>();
		}

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

		internal List<OrderItemVM> ActiveOrders;

		private int _behaviorType;

		private int _underAttackOfType;

		private bool _isSelectable;

		private bool _isSelected;

		private bool _isSelectionActive;

		private string _shortcutText;

		private InputKeyItemVM _selectionKey;
	}
}
