using System;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.ViewModelCollection.Input;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Order
{
	public class OrderItemVM : ViewModel
	{
		internal OrderSubType OrderSubType { get; private set; }

		internal OrderSetType OrderSetType { get; private set; }

		public OrderItemVM(OrderSubType orderSubType, OrderSetType orderSetType, TextObject tooltipText, Action<OrderItemVM, bool> onExecuteAction)
		{
			this.OrderSetType = orderSetType;
			this.OrderSubType = orderSubType;
			this.OrderIconID = this.GetIconIDFromSubType(this.OrderSubType);
			if (orderSetType == OrderSetType.Toggle)
			{
				this.OrderIconID += "Active";
			}
			this.OnExecuteAction = onExecuteAction;
			this.MainTitle = tooltipText.ToString();
			this.IsActive = true;
			this._isActivationOrder = this.IsTitle && this.OrderSetType == OrderSetType.None;
			this._isToggleActivationOrder = this.OrderSubType > OrderSubType.ToggleStart && this.OrderSubType < OrderSubType.ToggleEnd;
		}

		public OrderItemVM(OrderSetType orderSetType, TextObject tooltipText, Action<OrderItemVM, bool> onExecuteAction)
		{
			this.OrderSubType = OrderSubType.None;
			this.IsTitle = true;
			this.OrderSetType = orderSetType;
			this.OrderIconID = orderSetType.ToString();
			this.OnExecuteAction = onExecuteAction;
			this.MainTitle = tooltipText.ToString();
			this.IsActive = true;
		}

		public void SetActiveState(bool isActive)
		{
			if ((this.OrderSetType == OrderSetType.Toggle && !this.IsTitle) || this._isToggleActivationOrder)
			{
				this.MainTitle = GameTexts.FindText(isActive ? "str_order_name_on" : "str_order_name_off", this.OrderSubType.ToString()).ToString();
			}
			if (this._isToggleActivationOrder)
			{
				this.OrderIconID = (isActive ? this.OrderSubType.ToString() : (this.OrderSubType.ToString() + "Active"));
			}
			this.IsActive = true;
		}

		public override void OnFinalize()
		{
			base.OnFinalize();
			this.ShortcutKey.OnFinalize();
		}

		public void ExecuteAction()
		{
			this.OnExecuteAction(this, false);
		}

		public void FinalizeActiveStatus()
		{
			base.OnPropertyChanged("SelectionState");
		}

		private string GetIconIDFromSubType(OrderSubType orderSubType)
		{
			string text = string.Empty;
			if (orderSubType != OrderSubType.ActivationFaceDirection)
			{
				if (orderSubType != OrderSubType.FaceEnemy)
				{
					text = orderSubType.ToString();
				}
				else
				{
					text = OrderSubType.ToggleFacing.ToString() + "Active";
				}
			}
			else
			{
				text = OrderSubType.ToggleFacing.ToString();
			}
			return text;
		}

		[DataSourceProperty]
		public string OrderIconID
		{
			get
			{
				return this._orderIconID;
			}
			set
			{
				if (value != this._orderIconID)
				{
					this._orderIconID = value;
					base.OnPropertyChangedWithValue<string>(value, "OrderIconID");
				}
			}
		}

		[DataSourceProperty]
		public bool IsActive
		{
			get
			{
				return this._isActive;
			}
			set
			{
				value = value && this._selectionState != 0;
				this._isActive = value;
				base.OnPropertyChangedWithValue(value, "IsActive");
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
				if (value != this._isSelected)
				{
					this._isSelected = value;
					base.OnPropertyChanged("IsSelected");
					if (value)
					{
						this.OnExecuteAction(this, true);
					}
				}
			}
		}

		[DataSourceProperty]
		public bool CanUseShortcuts
		{
			get
			{
				return this._canUseShortcuts;
			}
			set
			{
				if (value != this._canUseShortcuts)
				{
					this._canUseShortcuts = value;
					base.OnPropertyChanged("CanUseShortcuts");
				}
			}
		}

		[DataSourceProperty]
		public int SelectionState
		{
			get
			{
				return this._selectionState;
			}
			set
			{
				if (value != this._selectionState)
				{
					this._selectionState = value;
					base.OnPropertyChangedWithValue(value, "SelectionState");
				}
			}
		}

		[DataSourceProperty]
		public InputKeyItemVM ShortcutKey
		{
			get
			{
				return this._shortcutKey;
			}
			set
			{
				if (value != this._shortcutKey)
				{
					this._shortcutKey = value;
					base.OnPropertyChangedWithValue<InputKeyItemVM>(value, "ShortcutKey");
				}
			}
		}

		[DataSourceProperty]
		public string MainTitle
		{
			get
			{
				return this._mainTitle;
			}
			set
			{
				if (value != this._mainTitle)
				{
					this._mainTitle = value;
					base.OnPropertyChangedWithValue<string>(value, "MainTitle");
				}
			}
		}

		[DataSourceProperty]
		public string SubSetTitle
		{
			get
			{
				return this._subSetTitle;
			}
			set
			{
				if (value != this._subSetTitle)
				{
					this._subSetTitle = value;
					base.OnPropertyChangedWithValue<string>(value, "SubSetTitle");
				}
			}
		}

		public Action<OrderItemVM, bool> OnExecuteAction;

		public bool IsTitle;

		private bool _isActivationOrder;

		private bool _isToggleActivationOrder;

		private bool _isActive;

		private bool _isSelected;

		private bool _canUseShortcuts;

		private int _selectionState = -1;

		private string _mainTitle;

		private string _subSetTitle;

		private InputKeyItemVM _shortcutKey;

		private string _orderIconID = "";

		public enum OrderSelectionState
		{
			Disabled,
			Default,
			PartiallyActive,
			Active
		}
	}
}
