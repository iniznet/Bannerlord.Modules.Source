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
	// Token: 0x02000024 RID: 36
	public class OrderSetVM : ViewModel
	{
		// Token: 0x170000C6 RID: 198
		// (get) Token: 0x060002A0 RID: 672 RVA: 0x0000C75C File Offset: 0x0000A95C
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

		// Token: 0x170000C7 RID: 199
		// (get) Token: 0x060002A1 RID: 673 RVA: 0x0000C76C File Offset: 0x0000A96C
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

		// Token: 0x060002A2 RID: 674 RVA: 0x0000C77C File Offset: 0x0000A97C
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

		// Token: 0x060002A3 RID: 675 RVA: 0x0000C7F8 File Offset: 0x0000A9F8
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

		// Token: 0x060002A4 RID: 676 RVA: 0x0000C87C File Offset: 0x0000AA7C
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

		// Token: 0x060002A5 RID: 677 RVA: 0x0000CB58 File Offset: 0x0000AD58
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

		// Token: 0x060002A6 RID: 678 RVA: 0x0000CB9C File Offset: 0x0000AD9C
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

		// Token: 0x060002A7 RID: 679 RVA: 0x0000CC94 File Offset: 0x0000AE94
		private void OnExecuteSubOrder(OrderItemVM orderItem, bool fromSelection)
		{
			this.OnSetExecution(orderItem, this.OrderSetType, fromSelection);
			if (fromSelection)
			{
				this.SelectedOrderText = orderItem.TooltipText;
			}
		}

		// Token: 0x060002A8 RID: 680 RVA: 0x0000CCB8 File Offset: 0x0000AEB8
		private void OnExecuteOrderSet(OrderItemVM orderItem, bool fromSelection)
		{
			this.OnSetExecution(orderItem, this.OrderSetType, fromSelection);
			if (fromSelection)
			{
				this.SelectedOrderText = orderItem.TooltipText;
			}
		}

		// Token: 0x060002A9 RID: 681 RVA: 0x0000CCDC File Offset: 0x0000AEDC
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

		// Token: 0x060002AA RID: 682 RVA: 0x0000CD88 File Offset: 0x0000AF88
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

		// Token: 0x060002AB RID: 683 RVA: 0x0000CDE4 File Offset: 0x0000AFE4
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

		// Token: 0x060002AC RID: 684 RVA: 0x0000CE68 File Offset: 0x0000B068
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

		// Token: 0x060002AD RID: 685 RVA: 0x0000CEFC File Offset: 0x0000B0FC
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

		// Token: 0x170000C8 RID: 200
		// (get) Token: 0x060002AE RID: 686 RVA: 0x0000CF74 File Offset: 0x0000B174
		// (set) Token: 0x060002AF RID: 687 RVA: 0x0000CF7C File Offset: 0x0000B17C
		public bool ContainsOrders { get; private set; }

		// Token: 0x170000C9 RID: 201
		// (get) Token: 0x060002B0 RID: 688 RVA: 0x0000CF85 File Offset: 0x0000B185
		// (set) Token: 0x060002B1 RID: 689 RVA: 0x0000CF90 File Offset: 0x0000B190
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

		// Token: 0x170000CA RID: 202
		// (get) Token: 0x060002B2 RID: 690 RVA: 0x0000CFF5 File Offset: 0x0000B1F5
		// (set) Token: 0x060002B3 RID: 691 RVA: 0x0000CFFD File Offset: 0x0000B1FD
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

		// Token: 0x170000CB RID: 203
		// (get) Token: 0x060002B4 RID: 692 RVA: 0x0000D020 File Offset: 0x0000B220
		// (set) Token: 0x060002B5 RID: 693 RVA: 0x0000D028 File Offset: 0x0000B228
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

		// Token: 0x170000CC RID: 204
		// (get) Token: 0x060002B6 RID: 694 RVA: 0x0000D04B File Offset: 0x0000B24B
		// (set) Token: 0x060002B7 RID: 695 RVA: 0x0000D053 File Offset: 0x0000B253
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

		// Token: 0x170000CD RID: 205
		// (get) Token: 0x060002B8 RID: 696 RVA: 0x0000D071 File Offset: 0x0000B271
		// (set) Token: 0x060002B9 RID: 697 RVA: 0x0000D079 File Offset: 0x0000B279
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

		// Token: 0x170000CE RID: 206
		// (get) Token: 0x060002BA RID: 698 RVA: 0x0000D097 File Offset: 0x0000B297
		// (set) Token: 0x060002BB RID: 699 RVA: 0x0000D09F File Offset: 0x0000B29F
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

		// Token: 0x170000CF RID: 207
		// (get) Token: 0x060002BC RID: 700 RVA: 0x0000D0BC File Offset: 0x0000B2BC
		// (set) Token: 0x060002BD RID: 701 RVA: 0x0000D0CE File Offset: 0x0000B2CE
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

		// Token: 0x04000148 RID: 328
		private Action<OrderItemVM, OrderSetType, bool> OnSetExecution;

		// Token: 0x04000149 RID: 329
		internal OrderSetType OrderSetType = OrderSetType.None;

		// Token: 0x0400014A RID: 330
		internal OrderSubType OrderSubType = OrderSubType.None;

		// Token: 0x0400014B RID: 331
		private OrderSubType _selectedOrder = OrderSubType.None;

		// Token: 0x0400014C RID: 332
		private bool _isMultiplayer;

		// Token: 0x0400014D RID: 333
		private int _index = -1;

		// Token: 0x0400014E RID: 334
		private bool _isToggleActivationOrder;

		// Token: 0x04000150 RID: 336
		private bool _showOrders;

		// Token: 0x04000151 RID: 337
		private bool _canUseShortcuts;

		// Token: 0x04000152 RID: 338
		private OrderItemVM _titleOrder;

		// Token: 0x04000153 RID: 339
		private MBBindingList<OrderItemVM> _orders;

		// Token: 0x04000154 RID: 340
		private string _titleText;

		// Token: 0x04000155 RID: 341
		private InputKeyItemVM _titleOrderKey;

		// Token: 0x04000156 RID: 342
		private string _selectedOrderText;
	}
}
