using System;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.ViewModelCollection.Input;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Order
{
	// Token: 0x02000021 RID: 33
	public class OrderItemVM : ViewModel
	{
		// Token: 0x170000BD RID: 189
		// (get) Token: 0x06000287 RID: 647 RVA: 0x0000C3C2 File Offset: 0x0000A5C2
		// (set) Token: 0x06000288 RID: 648 RVA: 0x0000C3CA File Offset: 0x0000A5CA
		internal OrderSubType OrderSubType { get; private set; }

		// Token: 0x170000BE RID: 190
		// (get) Token: 0x06000289 RID: 649 RVA: 0x0000C3D3 File Offset: 0x0000A5D3
		// (set) Token: 0x0600028A RID: 650 RVA: 0x0000C3DB File Offset: 0x0000A5DB
		internal OrderSetType OrderSetType { get; private set; }

		// Token: 0x0600028B RID: 651 RVA: 0x0000C3E4 File Offset: 0x0000A5E4
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
			this.TooltipText = tooltipText.ToString();
			this.IsActive = true;
			this._isActivationOrder = this.IsTitle && this.OrderSetType == OrderSetType.None;
			this._isToggleActivationOrder = this.OrderSubType > OrderSubType.ToggleStart && this.OrderSubType < OrderSubType.ToggleEnd;
		}

		// Token: 0x0600028C RID: 652 RVA: 0x0000C498 File Offset: 0x0000A698
		public OrderItemVM(OrderSetType orderSetType, TextObject tooltipText, Action<OrderItemVM, bool> onExecuteAction)
		{
			this.OrderSubType = OrderSubType.None;
			this.IsTitle = true;
			this.OrderSetType = orderSetType;
			this.OrderIconID = orderSetType.ToString();
			this.OnExecuteAction = onExecuteAction;
			this.TooltipText = tooltipText.ToString();
			this.IsActive = true;
		}

		// Token: 0x0600028D RID: 653 RVA: 0x0000C500 File Offset: 0x0000A700
		public void SetActiveState(bool isActive)
		{
			if ((this.OrderSetType == OrderSetType.Toggle && !this.IsTitle) || this._isToggleActivationOrder)
			{
				this.TooltipText = GameTexts.FindText(isActive ? "str_order_name_on" : "str_order_name_off", this.OrderSubType.ToString()).ToString();
			}
			if (this._isToggleActivationOrder)
			{
				this.OrderIconID = (isActive ? this.OrderSubType.ToString() : (this.OrderSubType.ToString() + "Active"));
			}
			this.IsActive = true;
		}

		// Token: 0x0600028E RID: 654 RVA: 0x0000C5A5 File Offset: 0x0000A7A5
		public override void OnFinalize()
		{
			base.OnFinalize();
			this.ShortcutKey.OnFinalize();
		}

		// Token: 0x0600028F RID: 655 RVA: 0x0000C5B8 File Offset: 0x0000A7B8
		public void ExecuteAction()
		{
			this.OnExecuteAction(this, false);
		}

		// Token: 0x06000290 RID: 656 RVA: 0x0000C5C7 File Offset: 0x0000A7C7
		public void FinalizeActiveStatus()
		{
			base.OnPropertyChanged("SelectionState");
		}

		// Token: 0x06000291 RID: 657 RVA: 0x0000C5D4 File Offset: 0x0000A7D4
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

		// Token: 0x170000BF RID: 191
		// (get) Token: 0x06000292 RID: 658 RVA: 0x0000C632 File Offset: 0x0000A832
		// (set) Token: 0x06000293 RID: 659 RVA: 0x0000C63A File Offset: 0x0000A83A
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

		// Token: 0x170000C0 RID: 192
		// (get) Token: 0x06000294 RID: 660 RVA: 0x0000C65D File Offset: 0x0000A85D
		// (set) Token: 0x06000295 RID: 661 RVA: 0x0000C665 File Offset: 0x0000A865
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

		// Token: 0x170000C1 RID: 193
		// (get) Token: 0x06000296 RID: 662 RVA: 0x0000C68B File Offset: 0x0000A88B
		// (set) Token: 0x06000297 RID: 663 RVA: 0x0000C693 File Offset: 0x0000A893
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

		// Token: 0x170000C2 RID: 194
		// (get) Token: 0x06000298 RID: 664 RVA: 0x0000C6C0 File Offset: 0x0000A8C0
		// (set) Token: 0x06000299 RID: 665 RVA: 0x0000C6C8 File Offset: 0x0000A8C8
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

		// Token: 0x170000C3 RID: 195
		// (get) Token: 0x0600029A RID: 666 RVA: 0x0000C6E5 File Offset: 0x0000A8E5
		// (set) Token: 0x0600029B RID: 667 RVA: 0x0000C6ED File Offset: 0x0000A8ED
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

		// Token: 0x170000C4 RID: 196
		// (get) Token: 0x0600029C RID: 668 RVA: 0x0000C70B File Offset: 0x0000A90B
		// (set) Token: 0x0600029D RID: 669 RVA: 0x0000C713 File Offset: 0x0000A913
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

		// Token: 0x170000C5 RID: 197
		// (get) Token: 0x0600029E RID: 670 RVA: 0x0000C731 File Offset: 0x0000A931
		// (set) Token: 0x0600029F RID: 671 RVA: 0x0000C739 File Offset: 0x0000A939
		[DataSourceProperty]
		public string TooltipText
		{
			get
			{
				return this._tooltipText;
			}
			set
			{
				if (value != this._tooltipText)
				{
					this._tooltipText = value;
					base.OnPropertyChangedWithValue<string>(value, "TooltipText");
				}
			}
		}

		// Token: 0x0400011A RID: 282
		public Action<OrderItemVM, bool> OnExecuteAction;

		// Token: 0x0400011B RID: 283
		public bool IsTitle;

		// Token: 0x0400011E RID: 286
		private bool _isActivationOrder;

		// Token: 0x0400011F RID: 287
		private bool _isToggleActivationOrder;

		// Token: 0x04000120 RID: 288
		private bool _isActive;

		// Token: 0x04000121 RID: 289
		private bool _isSelected;

		// Token: 0x04000122 RID: 290
		private bool _canUseShortcuts;

		// Token: 0x04000123 RID: 291
		private int _selectionState = -1;

		// Token: 0x04000124 RID: 292
		private string _tooltipText;

		// Token: 0x04000125 RID: 293
		private InputKeyItemVM _shortcutKey;

		// Token: 0x04000126 RID: 294
		private string _orderIconID = "";

		// Token: 0x02000140 RID: 320
		public enum OrderSelectionState
		{
			// Token: 0x04000BEC RID: 3052
			Disabled,
			// Token: 0x04000BED RID: 3053
			Default,
			// Token: 0x04000BEE RID: 3054
			PartiallyActive,
			// Token: 0x04000BEF RID: 3055
			Active
		}
	}
}
