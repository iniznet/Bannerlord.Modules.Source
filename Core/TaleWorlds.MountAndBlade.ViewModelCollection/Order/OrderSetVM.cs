using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.ViewModelCollection.Input;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Order
{
	public class OrderSetVM : ViewModel
	{
		internal IEnumerable<OrderSubType> SubOrdersSP
		{
			get
			{
				switch (this.OrderSetType)
				{
				case OrderSetType.Movement:
					yield return OrderSubType.MoveToPosition;
					yield return OrderSubType.FollowMe;
					yield return OrderSubType.Charge;
					yield return OrderSubType.Advance;
					yield return OrderSubType.Fallback;
					yield return OrderSubType.Stop;
					yield return OrderSubType.Retreat;
					yield return OrderSubType.Return;
					break;
				case OrderSetType.Form:
					yield return OrderSubType.FormLine;
					yield return OrderSubType.FormClose;
					yield return OrderSubType.FormLoose;
					yield return OrderSubType.FormCircular;
					yield return OrderSubType.FormSchiltron;
					yield return OrderSubType.FormV;
					yield return OrderSubType.FormColumn;
					yield return OrderSubType.FormScatter;
					yield return OrderSubType.Return;
					break;
				case OrderSetType.Toggle:
					yield return OrderSubType.ToggleFacing;
					yield return OrderSubType.ToggleFire;
					yield return OrderSubType.ToggleMount;
					yield return OrderSubType.ToggleAI;
					yield return OrderSubType.ToggleTransfer;
					yield return OrderSubType.Return;
					break;
				case OrderSetType.Facing:
					yield return OrderSubType.ActivationFaceDirection;
					yield return OrderSubType.FaceEnemy;
					break;
				default:
					yield return OrderSubType.None;
					break;
				}
				yield break;
			}
		}

		internal IEnumerable<OrderSubType> SubOrdersMP
		{
			get
			{
				switch (this.OrderSetType)
				{
				case OrderSetType.Movement:
					yield return OrderSubType.MoveToPosition;
					yield return OrderSubType.FollowMe;
					yield return OrderSubType.Charge;
					yield return OrderSubType.Advance;
					yield return OrderSubType.Fallback;
					yield return OrderSubType.Stop;
					yield return OrderSubType.Retreat;
					yield return OrderSubType.Return;
					break;
				case OrderSetType.Form:
					yield return OrderSubType.FormLine;
					yield return OrderSubType.FormClose;
					yield return OrderSubType.FormLoose;
					yield return OrderSubType.FormCircular;
					yield return OrderSubType.FormSchiltron;
					yield return OrderSubType.FormV;
					yield return OrderSubType.FormColumn;
					yield return OrderSubType.FormScatter;
					yield return OrderSubType.Return;
					break;
				case OrderSetType.Toggle:
					yield return OrderSubType.ToggleFacing;
					yield return OrderSubType.ToggleFire;
					yield return OrderSubType.ToggleMount;
					yield return OrderSubType.Return;
					break;
				case OrderSetType.Facing:
					yield return OrderSubType.ActivationFaceDirection;
					yield return OrderSubType.FaceEnemy;
					break;
				default:
					yield return OrderSubType.None;
					break;
				}
				yield break;
			}
		}

		internal OrderSetVM(OrderSetType orderSetType, Action<OrderItemVM, OrderSetType, bool> onExecution, bool isMultiplayer)
		{
			this.ContainsOrders = true;
			this.OrderSetType = orderSetType;
			this.OnSetExecution = onExecution;
			this._isMultiplayer = isMultiplayer;
			this.Orders = new MBBindingList<OrderItemVM>();
			this.TitleOrderKey = InputKeyItemVM.CreateFromGameKey(OrderSetVM.GetOrderGameKey((int)orderSetType), false);
			this.RefreshValues();
			this.TitleOrder.IsActive = true;
		}

		internal OrderSetVM(OrderSubType orderSubType, int index, Action<OrderItemVM, OrderSetType, bool> onExecution, bool isMultiplayer)
		{
			this.ContainsOrders = false;
			this.OrderSubType = orderSubType;
			this.OnSetExecution = onExecution;
			this._isMultiplayer = isMultiplayer;
			this._index = index;
			this.Orders = new MBBindingList<OrderItemVM>();
			this.TitleOrderKey = InputKeyItemVM.CreateFromGameKey(OrderSetVM.GetOrderGameKey(index), false);
			this.RefreshValues();
			this.TitleOrder.IsActive = true;
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
			if (this.TitleOrder != null)
			{
				this.TitleOrder.OnFinalize();
			}
			if (this.ContainsOrders)
			{
				this.TitleOrder = new OrderItemVM(this.OrderSetType, GameTexts.FindText("str_order_set_name", this.OrderSetType.ToString()), new Action<OrderItemVM, bool>(this.OnExecuteOrderSet));
				this.TitleOrder.ShortcutKey = InputKeyItemVM.CreateFromGameKey(OrderSetVM.GetOrderGameKey(this.GetOrderIndexFromOrderSetType(this.OrderSetType)), false);
				this.TitleOrder.IsTitle = true;
				this.TitleText = GameTexts.FindText("str_order_set_name", this.OrderSetType.ToString()).ToString().Trim(new char[] { ' ' });
			}
			else
			{
				this._isToggleActivationOrder = this.OrderSubType > OrderSubType.ToggleStart && this.OrderSubType < OrderSubType.ToggleEnd;
				TextObject textObject;
				if (this._isToggleActivationOrder)
				{
					textObject = GameTexts.FindText("str_order_name_off", this.OrderSubType.ToString());
				}
				else
				{
					textObject = GameTexts.FindText("str_order_name", this.OrderSubType.ToString());
				}
				this.TitleText = textObject.ToString();
				this.TitleOrder = new OrderItemVM(this.OrderSubType, OrderSetType.None, textObject, new Action<OrderItemVM, bool>(this.OnExecuteOrderSet));
				this.TitleOrder.IsTitle = true;
				this.TitleOrder.ShortcutKey = InputKeyItemVM.CreateFromGameKey(OrderSetVM.GetOrderGameKey(this._index), false);
			}
			MBTextManager.SetTextVariable("SHORTCUT", "", false);
			if (this.ContainsOrders)
			{
				OrderSubType[] array = (this._isMultiplayer ? this.SubOrdersMP.ToArray<OrderSubType>() : this.SubOrdersSP.ToArray<OrderSubType>());
				foreach (OrderItemVM orderItemVM in this.Orders)
				{
					orderItemVM.ShortcutKey.OnFinalize();
				}
				this.Orders.Clear();
				int num = 0;
				foreach (OrderSubType orderSubType in array)
				{
					TextObject textObject2;
					if (this.OrderSetType == OrderSetType.Toggle)
					{
						textObject2 = GameTexts.FindText("str_order_name_off", orderSubType.ToString());
					}
					else
					{
						textObject2 = GameTexts.FindText("str_order_name", orderSubType.ToString());
					}
					OrderItemVM orderItemVM2 = new OrderItemVM(orderSubType, this.OrderSetType, textObject2, new Action<OrderItemVM, bool>(this.OnExecuteSubOrder));
					this.Orders.Add(orderItemVM2);
					if (orderSubType == OrderSubType.Return)
					{
						orderItemVM2.ShortcutKey = InputKeyItemVM.CreateFromGameKey(HotKeyManager.GetCategory("MissionOrderHotkeyCategory").GetGameKey(76), false);
					}
					else
					{
						orderItemVM2.ShortcutKey = InputKeyItemVM.CreateFromGameKey(OrderSetVM.GetOrderGameKey(num), false);
					}
					num++;
				}
			}
		}

		private int GetOrderIndexFromOrderSetType(OrderSetType orderSetType)
		{
			int num;
			if (BannerlordConfig.OrderLayoutType == 0)
			{
				num = (int)orderSetType;
			}
			else
			{
				switch (orderSetType)
				{
				case OrderSetType.Movement:
					return 0;
				case OrderSetType.Form:
					return 2;
				case OrderSetType.Facing:
					return 1;
				}
				num = -1;
			}
			return num;
		}

		private static GameKey GetOrderGameKey(int index)
		{
			switch (index)
			{
			case 0:
				return HotKeyManager.GetCategory("MissionOrderHotkeyCategory").GetGameKey(68);
			case 1:
				return HotKeyManager.GetCategory("MissionOrderHotkeyCategory").GetGameKey(69);
			case 2:
				return HotKeyManager.GetCategory("MissionOrderHotkeyCategory").GetGameKey(70);
			case 3:
				return HotKeyManager.GetCategory("MissionOrderHotkeyCategory").GetGameKey(71);
			case 4:
				return HotKeyManager.GetCategory("MissionOrderHotkeyCategory").GetGameKey(72);
			case 5:
				return HotKeyManager.GetCategory("MissionOrderHotkeyCategory").GetGameKey(73);
			case 6:
				return HotKeyManager.GetCategory("MissionOrderHotkeyCategory").GetGameKey(74);
			case 7:
				return HotKeyManager.GetCategory("MissionOrderHotkeyCategory").GetGameKey(75);
			case 8:
				return HotKeyManager.GetCategory("MissionOrderHotkeyCategory").GetGameKey(76);
			default:
				Debug.FailedAssert("Invalid order game key index", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade.ViewModelCollection\\Order\\OrderSetVM.cs", "GetOrderGameKey", 345);
				return null;
			}
		}

		private void OnExecuteSubOrder(OrderItemVM orderItem, bool fromSelection)
		{
			this.OnSetExecution(orderItem, this.OrderSetType, fromSelection);
			if (fromSelection)
			{
				this.SelectedOrderText = orderItem.TooltipText;
			}
		}

		private void OnExecuteOrderSet(OrderItemVM orderItem, bool fromSelection)
		{
			this.OnSetExecution(orderItem, this.OrderSetType, fromSelection);
			if (fromSelection)
			{
				this.SelectedOrderText = orderItem.TooltipText;
			}
		}

		public void ResetActiveStatus(bool disable = false)
		{
			this.TitleOrder.SelectionState = (disable ? 0 : 1);
			if (this.ContainsOrders)
			{
				foreach (OrderItemVM orderItemVM in this.Orders)
				{
					orderItemVM.SelectionState = (disable ? 0 : 1);
				}
				if (this.OrderSetType == OrderSetType.Toggle)
				{
					this.Orders.ApplyActionOnAllItems(delegate(OrderItemVM o)
					{
						o.SetActiveState(false);
					});
					return;
				}
			}
			else
			{
				this.TitleOrder.SetActiveState(false);
			}
		}

		public void FinalizeActiveStatus(bool forceDisable = false)
		{
			this.TitleOrder.FinalizeActiveStatus();
			if (forceDisable)
			{
				return;
			}
			foreach (OrderItemVM orderItemVM in this.Orders)
			{
				orderItemVM.FinalizeActiveStatus();
			}
		}

		internal OrderItemVM GetOrder(OrderSubType type)
		{
			if (this.ContainsOrders)
			{
				return this.Orders.FirstOrDefault((OrderItemVM order) => order.OrderSubType == type);
			}
			if (type == this.TitleOrder.OrderSubType)
			{
				return this.TitleOrder;
			}
			Debug.FailedAssert("Couldn't find order item " + type.ToString(), "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade.ViewModelCollection\\Order\\OrderSetVM.cs", "GetOrder", 441);
			return null;
		}

		public void SetActiveOrder(OrderItemVM order)
		{
			if (this.OrderSetType != OrderSetType.Toggle)
			{
				this._selectedOrder = order.OrderSubType;
				this.TitleOrder.OrderIconID = ((order.OrderSubType == OrderSubType.None) ? "MultipleSelection" : order.OrderIconID);
				this.TitleOrder.TooltipText = ((order.OrderSubType == OrderSubType.None) ? GameTexts.FindText("str_order_set_name", this.OrderSetType.ToString()).ToString() : order.TooltipText);
				this.SelectedOrderText = order.TooltipText;
				return;
			}
			order.SetActiveState(true);
		}

		public override void OnFinalize()
		{
			base.OnFinalize();
			if (this.ContainsOrders)
			{
				foreach (OrderItemVM orderItemVM in this.Orders)
				{
					orderItemVM.ShortcutKey.OnFinalize();
				}
			}
			this.TitleOrder.ShortcutKey.OnFinalize();
			this.TitleOrderKey.OnFinalize();
		}

		public bool ContainsOrders { get; private set; }

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
					base.OnPropertyChangedWithValue(value, "CanUseShortcuts");
					if (this.TitleOrder != null)
					{
						this.TitleOrder.CanUseShortcuts = value;
					}
					for (int i = 0; i < this.Orders.Count; i++)
					{
						this.Orders[i].CanUseShortcuts = value;
					}
				}
			}
		}

		[DataSourceProperty]
		public string SelectedOrderText
		{
			get
			{
				return this._selectedOrderText;
			}
			set
			{
				if (value != this._selectedOrderText)
				{
					this._selectedOrderText = value;
					base.OnPropertyChangedWithValue<string>(value, "SelectedOrderText");
				}
			}
		}

		[DataSourceProperty]
		public string TitleText
		{
			get
			{
				return this._titleText;
			}
			set
			{
				if (value != this._titleText)
				{
					this._titleText = value;
					base.OnPropertyChangedWithValue<string>(value, "TitleText");
				}
			}
		}

		[DataSourceProperty]
		public MBBindingList<OrderItemVM> Orders
		{
			get
			{
				return this._orders;
			}
			set
			{
				if (value != this._orders)
				{
					this._orders = value;
					base.OnPropertyChangedWithValue<MBBindingList<OrderItemVM>>(value, "Orders");
				}
			}
		}

		[DataSourceProperty]
		public OrderItemVM TitleOrder
		{
			get
			{
				return this._titleOrder;
			}
			set
			{
				if (value != this._titleOrder)
				{
					this._titleOrder = value;
					base.OnPropertyChangedWithValue<OrderItemVM>(value, "TitleOrder");
				}
			}
		}

		[DataSourceProperty]
		public InputKeyItemVM TitleOrderKey
		{
			get
			{
				return this._titleOrderKey;
			}
			set
			{
				if (value != this._titleOrderKey)
				{
					this._titleOrderKey = value;
					base.OnPropertyChanged("TitleOrderKey");
				}
			}
		}

		[DataSourceProperty]
		public bool ShowOrders
		{
			get
			{
				return this._showOrders && this.ContainsOrders;
			}
			set
			{
				this._showOrders = value;
				base.OnPropertyChangedWithValue(value, "ShowOrders");
			}
		}

		private Action<OrderItemVM, OrderSetType, bool> OnSetExecution;

		internal OrderSetType OrderSetType = OrderSetType.None;

		internal OrderSubType OrderSubType = OrderSubType.None;

		private OrderSubType _selectedOrder = OrderSubType.None;

		private bool _isMultiplayer;

		private int _index = -1;

		private bool _isToggleActivationOrder;

		private bool _showOrders;

		private bool _canUseShortcuts;

		private OrderItemVM _titleOrder;

		private MBBindingList<OrderItemVM> _orders;

		private string _titleText;

		private InputKeyItemVM _titleOrderKey;

		private string _selectedOrderText;
	}
}
